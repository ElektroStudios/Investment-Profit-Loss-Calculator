#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization

Imports Core

Imports InvestmentProfitLossCalculator.Tests.Helpers

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " NumberTextParserTests "

Namespace Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Unit tests for the <see cref="NumberTextParser"/> class.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <TestClass>
    Public NotInheritable Class NumberTextParserTests

#Region " TryNormalize "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the normalization of valid separators to the invariant format.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("1234", "1234", DisplayName:="No separators")>
        <DataRow("  1234  ", "1234", DisplayName:="Trimmed")>
        <DataRow("1.5", "1.5", DisplayName:="Dot decimal")>
        <DataRow("1,5", "1.5", DisplayName:="Comma decimal")>
        <DataRow("0,500", "0.500", DisplayName:="Comma decimal after zero (not a thousands group)")>
        <DataRow("1000,50", "1000.50", DisplayName:="Comma decimal with two decimals")>
        <DataRow("1,000", "1000", DisplayName:="Comma thousands (ambiguous, interpreted as thousands)")>
        <DataRow("12,345,678", "12345678", DisplayName:="Multiple comma thousands")>
        <DataRow("1.000", "1.000", DisplayName:="Single dot is always decimal")>
        <DataRow("1.000.000", "1000000", DisplayName:="Multiple dot thousands")>
        <DataRow("1,000.50", "1000.50", DisplayName:="Comma thousands, dot decimal")>
        <DataRow("1.000,50", "1000.50", DisplayName:="Dot thousands, comma decimal")>
        <DataRow("1,234,567.891", "1234567.891", DisplayName:="Multiple comma thousands, dot decimal")>
        <DataRow("1.234.567,891", "1234567.891", DisplayName:="Multiple dot thousands, comma decimal")>
        <DataRow("-1,234.5", "-1234.5", DisplayName:="Negative")>
        <DataRow("+1.234,5", "+1234.5", DisplayName:="Explicit positive sign")>
        <DataRow("", "", DisplayName:="Empty")>
        Public Sub TryNormalize_ValidSeparators_ReturnsInvariantText(text As String, expected As String)
            Dim normalized As String = Nothing
            Assert.IsTrue(NumberTextParser.TryNormalize(text, normalized))
            Assert.AreEqual(expected, normalized)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that invalid separators are rejected.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("1,2,3", DisplayName:="Multiple commas, not thousands groups")>
        <DataRow("1.2.3", DisplayName:="Multiple dots, not thousands groups")>
        <DataRow("1,000,50", DisplayName:="Invalid last comma group")>
        <DataRow("1,00.5", DisplayName:="Invalid comma group before dot decimal")>
        <DataRow("1.00,5", DisplayName:="Invalid dot group before comma decimal")>
        <DataRow("1,000.000,5", DisplayName:="Both separators used as thousands separators")>
        <DataRow("1.5,000", DisplayName:="Decimal separator before thousands separator")>
        <DataRow("1,000.5,0", DisplayName:="Mixed separators")>
        Public Sub TryNormalize_InvalidSeparators_ReturnsFalse(text As String)
            Dim normalized As String = "unchanged"
            Assert.IsFalse(NumberTextParser.TryNormalize(text, normalized))
            Assert.IsNull(normalized)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that a null text throws <see cref="ArgumentNullException"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub NullText_Throws()
            Dim normalized As String = Nothing
            Dim doubleValue As Double
            Dim decimalValue As Decimal

            Assert.ThrowsExactly(Of ArgumentNullException)(Function() NumberTextParser.TryNormalize(Nothing, normalized))
            Assert.ThrowsExactly(Of ArgumentNullException)(Function() NumberTextParser.TryParseDouble(Nothing, doubleValue))
            Assert.ThrowsExactly(Of ArgumentNullException)(Function() NumberTextParser.TryParseDecimal(Nothing, decimalValue))
        End Sub

#End Region

#Region " Preferred Decimal Separator "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that ambiguous texts are resolved with the preferred decimal separator,
        ''' while unambiguous texts are parsed the same way with both preferences.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("1,500", "."c, "1500", DisplayName:="Dot preferred: comma group")>
        <DataRow("1,500", ","c, "1.500", DisplayName:="Comma preferred: comma decimal")>
        <DataRow("1.500", "."c, "1.500", DisplayName:="Dot preferred: dot decimal")>
        <DataRow("1.500", ","c, "1500", DisplayName:="Comma preferred: dot group")>
        <DataRow("1.5", ","c, "1.5", DisplayName:="Comma preferred: dot that is not a group is decimal")>
        <DataRow("1,5", "."c, "1.5", DisplayName:="Dot preferred: comma that is not a group is decimal")>
        <DataRow("1,000,000", ","c, "1000000", DisplayName:="Comma preferred: multiple comma groups")>
        <DataRow("1.000.000", ","c, "1000000", DisplayName:="Comma preferred: multiple dot groups")>
        <DataRow("1.000,50", "."c, "1000.50", DisplayName:="Dot preferred: both separators")>
        <DataRow("1,000.50", ","c, "1000.50", DisplayName:="Comma preferred: both separators")>
        Public Sub TryNormalize_PreferredSeparator_ResolvesAmbiguity(text As String, preferred As Char, expected As String)
            Dim normalized As String = Nothing
            Assert.IsTrue(NumberTextParser.TryNormalize(text, preferred, normalized))
            Assert.AreEqual(expected, normalized)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that a preferred decimal separator other than "." or "," throws <see cref="ArgumentOutOfRangeException"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub TryNormalize_InvalidPreferredSeparator_Throws()
            Dim normalized As String = Nothing
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() NumberTextParser.TryNormalize("1", "'"c, normalized))
        End Sub

#End Region

#Region " TryParseDecimal "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the parsing of decimal values (used by the numeric up/down controls).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("1000,50", "1000.50")>
        <DataRow("1,000.50", "1000.50")>
        <DataRow("1.000,50", "1000.50")>
        <DataRow("1,000", "1000")>
        <DataRow("0,01", "0.01")>
        <DataRow("-25,5", "-25.5")>
        <DataRow("999,999,999,999,999", "999999999999999")>
        Public Sub TryParseDecimal_ValidText_ReturnsExactValue(text As String, expected As String)
            Dim value As Decimal
            Assert.IsTrue(NumberTextParser.TryParseDecimal(text, value))
            Assert.AreEqual(Decimal.Parse(expected, CultureInfo.InvariantCulture), value)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that invalid decimal values are rejected and the output value is reset to zero.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("")>
        <DataRow("abc")>
        <DataRow("1,2,3")>
        <DataRow("$100")>
        Public Sub TryParseDecimal_InvalidText_ReturnsFalseAndZero(text As String)
            Dim value As Decimal = 123D
            Assert.IsFalse(NumberTextParser.TryParseDecimal(text, value))
            Assert.AreEqual(0D, value)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that parsing does not depend on the culture of the current thread.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("de-DE")>
        <DataRow("es-ES")>
        <DataRow("en-US")>
        Public Sub TryParseDecimal_IsCultureIndependent(cultureName As String)
            Using scope As New CultureScope(cultureName)
                Dim value As Decimal
                Assert.IsTrue(NumberTextParser.TryParseDecimal("1,000.50", value))
                Assert.AreEqual(1000.5D, value)
                Assert.IsTrue(NumberTextParser.TryParseDecimal("1000,50", value))
                Assert.AreEqual(1000.5D, value)
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
