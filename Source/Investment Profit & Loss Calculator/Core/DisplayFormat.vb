#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization

#End Region

#Region " CurrencyKind "

Namespace Core

    ''' <summary>
    ''' Specifies the currency used to display amounts.
    ''' </summary>
    Public Enum CurrencyKind As Integer

        ''' <summary>
        ''' US dollar: the symbol is placed before the amount (e.g. "$1,000.50").
        ''' </summary>
        UsDollar = 0

        ''' <summary>
        ''' Euro: the symbol is placed after the amount (e.g. "1.000,50 €").
        ''' </summary>
        Euro = 1

    End Enum

    ''' <summary>
    ''' Specifies the decimal separator used to display and parse numbers.
    ''' The thousands separator is the other character.
    ''' </summary>
    Public Enum DecimalSeparatorKind As Integer

        ''' <summary>
        ''' Dot as decimal separator and comma as thousands separator (e.g. "1,000.50").
        ''' </summary>
        Dot = 0

        ''' <summary>
        ''' Comma as decimal separator and dot as thousands separator (e.g. "1.000,50").
        ''' </summary>
        Comma = 1

    End Enum

#End Region

#Region " DisplayFormat "

    ''' <summary>
    ''' Defines how numbers and amounts are displayed and parsed:
    ''' the currency and the decimal separator, which can be chosen independently.
    ''' <para></para>
    ''' The formats are fully defined by this class (they do not depend on the culture of the OS or the user),
    ''' so they are identical on every machine.
    ''' </summary>
    Public NotInheritable Class DisplayFormat

#Region " Fields "

        ''' <summary>
        ''' The shared instance returned by <see cref="DisplayFormat.Default"/>.
        ''' </summary>
        Private Shared ReadOnly DefaultInstance As New DisplayFormat(CurrencyKind.UsDollar, DecimalSeparatorKind.Dot)

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the default display format: US dollar with dot as decimal separator.
        ''' </summary>
        Public Shared ReadOnly Property [Default] As DisplayFormat
            Get
                Return DisplayFormat.DefaultInstance
            End Get
        End Property

        ''' <summary>
        ''' Gets the currency.
        ''' </summary>
        Public ReadOnly Property Currency As CurrencyKind

        ''' <summary>
        ''' Gets the decimal separator kind.
        ''' </summary>
        Public ReadOnly Property DecimalSeparator As DecimalSeparatorKind

        ''' <summary>
        ''' Gets the decimal separator character.
        ''' </summary>
        Public ReadOnly Property DecimalSeparatorChar As Char
            Get
                Return If(Me.DecimalSeparator = DecimalSeparatorKind.Comma, ","c, "."c)
            End Get
        End Property

        ''' <summary>
        ''' Gets the thousands separator character.
        ''' </summary>
        Public ReadOnly Property GroupSeparatorChar As Char
            Get
                Return If(Me.DecimalSeparator = DecimalSeparatorKind.Comma, "."c, ","c)
            End Get
        End Property

        ''' <summary>
        ''' Gets the currency symbol ("$" or "€").
        ''' </summary>
        Public ReadOnly Property CurrencySymbol As String
            Get
                Return If(Me.Currency = CurrencyKind.Euro, "€", "$")
            End Get
        End Property

        ''' <summary>
        ''' Gets the read-only <see cref="System.Globalization.NumberFormatInfo"/> that implements this format.
        ''' </summary>
        Public ReadOnly Property NumberFormat As NumberFormatInfo

        ''' <summary>
        ''' Gets a read-only culture based on the invariant culture, with the <see cref="DisplayFormat.NumberFormat"/>
        ''' of this format. It is used as the culture of the UI thread, so the numeric up/down controls
        ''' display and parse numbers with this format.
        ''' </summary>
        Public ReadOnly Property Culture As CultureInfo

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="DisplayFormat"/> class.
        ''' </summary>
        ''' <param name="currency">
        ''' The currency.
        ''' </param>
        '''
        ''' <param name="decimalSeparator">
        ''' The decimal separator.
        ''' </param>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="currency"/> or <paramref name="decimalSeparator"/> is not a defined value.
        ''' </exception>
        Public Sub New(currency As CurrencyKind, decimalSeparator As DecimalSeparatorKind)

            If Not [Enum].IsDefined(GetType(CurrencyKind), currency) Then
                Throw New ArgumentOutOfRangeException(NameOf(currency), currency, "Undefined currency.")
            End If
            If Not [Enum].IsDefined(GetType(DecimalSeparatorKind), decimalSeparator) Then
                Throw New ArgumentOutOfRangeException(NameOf(decimalSeparator), decimalSeparator, "Undefined decimal separator.")
            End If

            Me.Currency = currency
            Me.DecimalSeparator = decimalSeparator
            Me.NumberFormat = DisplayFormat.CreateNumberFormat(currency, decimalSeparator)

            Dim culture As CultureInfo = DirectCast(CultureInfo.InvariantCulture.Clone(), CultureInfo)
            culture.NumberFormat = Me.NumberFormat
            Me.Culture = CultureInfo.ReadOnly(culture)
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Returns a copy of this format with the specified currency.
        ''' </summary>
        Public Function WithCurrency(currency As CurrencyKind) As DisplayFormat
            Return New DisplayFormat(currency, Me.DecimalSeparator)
        End Function

        ''' <summary>
        ''' Returns a copy of this format with the specified decimal separator.
        ''' </summary>
        Public Function WithDecimalSeparator(decimalSeparator As DecimalSeparatorKind) As DisplayFormat
            Return New DisplayFormat(Me.Currency, decimalSeparator)
        End Function

        ''' <summary>
        ''' Formats a currency amount (e.g. "$1,000.50", "-$50.00", "1.000,50 €" or "-50,00 €").
        ''' </summary>
        ''' <param name="amount">
        ''' The amount.
        ''' </param>
        '''
        ''' <param name="decimals">
        ''' The number of decimals.
        ''' </param>
        Public Function FormatCurrency(amount As Double, decimals As Integer) As String
            Return amount.ToString($"C{decimals}", Me.NumberFormat)
        End Function

        ''' <summary>
        ''' Formats a number with the specified .NET custom numeric format string
        ''' (e.g. "0.##"), using the separators of this format.
        ''' </summary>
        Public Function FormatNumber(value As Double, format As String) As String
            Return value.ToString(format, Me.NumberFormat)
        End Function

        ''' <summary>
        ''' Determines whether the specified object is a <see cref="DisplayFormat"/> with the same settings.
        ''' </summary>
        Public Overrides Function Equals(obj As Object) As Boolean
            Dim other As DisplayFormat = TryCast(obj, DisplayFormat)
            Return other IsNot Nothing AndAlso other.Currency = Me.Currency AndAlso other.DecimalSeparator = Me.DecimalSeparator
        End Function

        ''' <summary>
        ''' Returns the hash code for this instance.
        ''' </summary>
        Public Overrides Function GetHashCode() As Integer
            Return (CInt(Me.Currency) << 1) Or CInt(Me.DecimalSeparator)
        End Function

        ''' <summary>
        ''' Returns a <see cref="String"/> that represents this instance.
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"{Me.Currency}, {Me.DecimalSeparator}"
        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Creates the read-only number format of the specified currency and decimal separator.
        ''' </summary>
        Private Shared Function CreateNumberFormat(currency As CurrencyKind, decimalSeparator As DecimalSeparatorKind) As NumberFormatInfo

            Dim format As NumberFormatInfo = DirectCast(CultureInfo.InvariantCulture.NumberFormat.Clone(), NumberFormatInfo)

            Dim decimalText As String = If(decimalSeparator = DecimalSeparatorKind.Comma, ",", ".")
            Dim groupText As String = If(decimalSeparator = DecimalSeparatorKind.Comma, ".", ",")

            format.NumberDecimalSeparator = decimalText
            format.NumberGroupSeparator = groupText
            format.CurrencyDecimalSeparator = decimalText
            format.CurrencyGroupSeparator = groupText
            format.PercentDecimalSeparator = decimalText
            format.PercentGroupSeparator = groupText
            format.NumberGroupSizes = New Integer() {3}
            format.CurrencyGroupSizes = New Integer() {3}
            format.CurrencyDecimalDigits = 2

            If currency = CurrencyKind.Euro Then
                format.CurrencySymbol = "€"
                format.CurrencyPositivePattern = 3 ' n $
                format.CurrencyNegativePattern = 8 ' -n $
            Else
                format.CurrencySymbol = "$"
                format.CurrencyPositivePattern = 0 ' $n
                format.CurrencyNegativePattern = 1 ' -$n
            End If

            Return NumberFormatInfo.ReadOnly(format)
        End Function

#End Region

    End Class

End Namespace

#End Region
