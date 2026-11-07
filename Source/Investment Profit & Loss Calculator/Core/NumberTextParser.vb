#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization
Imports System.Text.RegularExpressions

#End Region

#Region " NumberTextParser "

Namespace Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Parses numbers typed by the user, accepting both the dot (".") and the comma (",")
    ''' as decimal separator, with optional thousands separators, independently of the current culture.
    ''' <para></para>
    ''' A preferred decimal separator (the one of the current display format, dot by default)
    ''' is used to resolve ambiguous texts. Being D the preferred decimal separator and G the other character:
    ''' <list type="bullet">
    '''   <item><description>
    '''   If the text contains both separators, the rightmost one is the decimal separator
    '''   and the other one is the thousands separator ("1,000.50" and "1.000,50" are 1000.5, whatever D is).
    '''   </description></item>
    '''   <item><description>
    '''   If the text only contains G, it is a thousands separator when it forms valid groups of three digits;
    '''   otherwise a single G is the decimal separator. With D = ".": "1,000" is 1000 and "1,5" is 1.5.
    '''   With D = ",": "1.000" is 1000 and "1.5" is 1.5.
    '''   </description></item>
    '''   <item><description>
    '''   If the text only contains D, two or more valid groups of three digits are thousands separators;
    '''   otherwise a single D is the decimal separator. With D = ".": "1.000.000" is 1000000 and "1.500" is 1.5.
    '''   With D = ",": "1,000,000" is 1000000 and "1,500" is 1.5.
    '''   </description></item>
    ''' </list>
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public NotInheritable Class NumberTextParser

#Region " Constants "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The default preferred decimal separator.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Const DefaultDecimalSeparator As Char = "."c

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The number styles accepted for the normalized text.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Const NormalizedNumberStyles As NumberStyles = NumberStyles.Float

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="NumberTextParser"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Converts the text typed by the user to an invariant culture number text
        ''' (dot as decimal separator, no thousands separators), using the dot as preferred decimal separator.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <inheritdoc cref="TryNormalize(String, Char, ByRef String)"/>
        Public Shared Function TryNormalize(text As String, ByRef normalized As String) As Boolean
            Return NumberTextParser.TryNormalize(text, DefaultDecimalSeparator, normalized)
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Converts the text typed by the user to an invariant culture number text
        ''' (dot as decimal separator, no thousands separators).
        ''' <para></para>
        ''' This method only normalizes the separators; it does not validate the rest of the text.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="text">
        ''' The text to normalize.
        ''' </param>
        '''
        ''' <param name="preferredDecimalSeparator">
        ''' The preferred decimal separator ("." or ","), used to resolve ambiguous texts.
        ''' </param>
        '''
        ''' <param name="normalized">
        ''' When this method returns, contains the normalized text, or <see langword="Nothing"/> if the separators are invalid.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the separators are valid; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="text"/> is <see langword="Nothing"/>.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="preferredDecimalSeparator"/> is not "." or ",".
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        Public Shared Function TryNormalize(text As String, preferredDecimalSeparator As Char, ByRef normalized As String) As Boolean

            If text Is Nothing Then
                Throw New ArgumentNullException(NameOf(text))
            End If
            If preferredDecimalSeparator <> "."c AndAlso preferredDecimalSeparator <> ","c Then
                Throw New ArgumentOutOfRangeException(NameOf(preferredDecimalSeparator), preferredDecimalSeparator, "The decimal separator must be ""."" or "",""")
            End If

            normalized = Nothing
            Dim trimmed As String = text.Trim()
            Dim preferredGroupSeparator As Char = If(preferredDecimalSeparator = "."c, ","c, "."c)

            Dim lastComma As Integer = trimmed.LastIndexOf(","c)
            Dim lastDot As Integer = trimmed.LastIndexOf("."c)

            ' Both separators: the rightmost one is the decimal separator.
            If lastComma >= 0 AndAlso lastDot >= 0 Then
                Dim decimalSeparator As Char = If(lastComma > lastDot, ","c, "."c)
                Dim groupSeparator As Char = If(decimalSeparator = ","c, "."c, ","c)
                Dim decimalIndex As Integer = Math.Max(lastComma, lastDot)

                Dim integerPart As String = trimmed.Substring(0, decimalIndex)
                Dim fractionPart As String = trimmed.Substring(decimalIndex + 1)

                If fractionPart.IndexOf(groupSeparator) >= 0 OrElse
                   (integerPart.IndexOf(groupSeparator) >= 0 AndAlso Not NumberTextParser.IsGroupedInteger(integerPart, groupSeparator, minimumGroups:=1)) Then
                    Return False
                End If

                normalized = $"{integerPart.Replace(groupSeparator.ToString(), String.Empty)}.{fractionPart}"
                Return True
            End If

            ' Only the preferred thousands separator: thousands groups (1 or more), or a single decimal separator.
            If trimmed.IndexOf(preferredGroupSeparator) >= 0 Then
                Return NumberTextParser.TryNormalizeSingleSeparatorKind(trimmed, preferredGroupSeparator, minimumGroups:=1, normalized)
            End If

            ' Only the preferred decimal separator: thousands groups (2 or more), or a single decimal separator.
            If trimmed.IndexOf(preferredDecimalSeparator) >= 0 Then
                Return NumberTextParser.TryNormalizeSingleSeparatorKind(trimmed, preferredDecimalSeparator, minimumGroups:=2, normalized)
            End If

            normalized = trimmed
            Return True
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Parses a <see cref="Double"/> typed by the user, using the dot as preferred decimal separator.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <inheritdoc cref="TryParseDouble(String, Char, ByRef Double)"/>
        Public Shared Function TryParseDouble(text As String, ByRef value As Double) As Boolean
            Return NumberTextParser.TryParseDouble(text, DefaultDecimalSeparator, value)
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Parses a <see cref="Double"/> typed by the user.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="text">
        ''' The text to parse.
        ''' </param>
        '''
        ''' <param name="preferredDecimalSeparator">
        ''' The preferred decimal separator ("." or ","), used to resolve ambiguous texts.
        ''' </param>
        '''
        ''' <param name="value">
        ''' When this method returns, contains the parsed value, or zero if the parsing failed.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="text"/> was parsed successfully
        ''' to a finite value; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="text"/> is <see langword="Nothing"/>.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="preferredDecimalSeparator"/> is not "." or ",".
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        Public Shared Function TryParseDouble(text As String, preferredDecimalSeparator As Char, ByRef value As Double) As Boolean

            Dim normalized As String = Nothing
            If Not NumberTextParser.TryNormalize(text, preferredDecimalSeparator, normalized) OrElse
               Not Double.TryParse(normalized, NormalizedNumberStyles, CultureInfo.InvariantCulture, value) OrElse
               Double.IsNaN(value) OrElse Double.IsInfinity(value) Then ' Modern .NET returns infinity on overflow.

                value = 0
                Return False
            End If

            Return True
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Parses a <see cref="Decimal"/> typed by the user, using the dot as preferred decimal separator.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <inheritdoc cref="TryParseDecimal(String, Char, ByRef Decimal)"/>
        Public Shared Function TryParseDecimal(text As String, ByRef value As Decimal) As Boolean
            Return NumberTextParser.TryParseDecimal(text, DefaultDecimalSeparator, value)
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Parses a <see cref="Decimal"/> typed by the user.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="text">
        ''' The text to parse.
        ''' </param>
        '''
        ''' <param name="preferredDecimalSeparator">
        ''' The preferred decimal separator ("." or ","), used to resolve ambiguous texts.
        ''' </param>
        '''
        ''' <param name="value">
        ''' When this method returns, contains the parsed value, or zero if the parsing failed.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="text"/> was parsed successfully; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="text"/> is <see langword="Nothing"/>.
        ''' </exception>
        '''
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="preferredDecimalSeparator"/> is not "." or ",".
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        Public Shared Function TryParseDecimal(text As String, preferredDecimalSeparator As Char, ByRef value As Decimal) As Boolean

            Dim normalized As String = Nothing
            If Not NumberTextParser.TryNormalize(text, preferredDecimalSeparator, normalized) OrElse
               Not Decimal.TryParse(normalized, NormalizedNumberStyles, CultureInfo.InvariantCulture, value) Then

                value = 0D
                Return False
            End If

            Return True
        End Function

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Normalizes a text that contains only one kind of separator:
        ''' it is a thousands separator if the text is an integer with at least <paramref name="minimumGroups"/>
        ''' valid thousands groups; otherwise a single separator is the decimal separator.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function TryNormalizeSingleSeparatorKind(text As String, separator As Char, minimumGroups As Integer, ByRef normalized As String) As Boolean

            If NumberTextParser.IsGroupedInteger(text, separator, minimumGroups) Then
                normalized = text.Replace(separator.ToString(), String.Empty)
                Return True
            End If

            If text.IndexOf(separator) <> text.LastIndexOf(separator) Then
                normalized = Nothing
                Return False ' Multiple separators that are not valid thousands groups.
            End If

            normalized = text.Replace(separator, "."c)
            Return True
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the specified text is an integer number
        ''' with at least the specified number of valid thousands groups.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function IsGroupedInteger(text As String, groupSeparator As Char, minimumGroups As Integer) As Boolean
            Dim escaped As String = Regex.Escape(groupSeparator.ToString())
            Dim pattern As String = $"^[+-]?[1-9][0-9]{{0,2}}({escaped}[0-9]{{3}}){{{minimumGroups},}}$"
            Return Regex.IsMatch(text, pattern, RegexOptions.CultureInvariant)
        End Function

#End Region

    End Class

End Namespace

#End Region
