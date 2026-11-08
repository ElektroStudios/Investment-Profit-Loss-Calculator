#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Core

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " DisplayFormatTests "

Namespace Core

    ''' <summary>
    ''' Unit tests for the <see cref="DisplayFormat"/> class.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class DisplayFormatTests

        ''' <summary>
        ''' Verifies that the default format is US dollar with dot as decimal separator.
        ''' </summary>
        <TestMethod>
        Public Sub Default_IsUsDollarWithDot()
            Assert.AreEqual(CurrencyKind.UsDollar, DisplayFormat.Default.Currency)
            Assert.AreEqual(DecimalSeparatorKind.Dot, DisplayFormat.Default.DecimalSeparator)
        End Sub

        ''' <summary>
        ''' Verifies the separators and symbols of every combination of currency and decimal separator.
        ''' </summary>
        <TestMethod>
        <DataRow(CurrencyKind.UsDollar, DecimalSeparatorKind.Dot, "."c, ","c, "$")>
        <DataRow(CurrencyKind.UsDollar, DecimalSeparatorKind.Comma, ","c, "."c, "$")>
        <DataRow(CurrencyKind.Euro, DecimalSeparatorKind.Dot, "."c, ","c, "€")>
        <DataRow(CurrencyKind.Euro, DecimalSeparatorKind.Comma, ","c, "."c, "€")>
        Public Sub Properties_MatchSettings(currency As CurrencyKind, separator As DecimalSeparatorKind, decimalChar As Char, groupChar As Char, symbol As String)
            Dim format As New DisplayFormat(currency, separator)

            Assert.AreEqual(decimalChar, format.DecimalSeparatorChar)
            Assert.AreEqual(groupChar, format.GroupSeparatorChar)
            Assert.AreEqual(symbol, format.CurrencySymbol)
            Assert.AreEqual(decimalChar.ToString(), format.NumberFormat.NumberDecimalSeparator)
            Assert.AreEqual(groupChar.ToString(), format.NumberFormat.NumberGroupSeparator)
            Assert.AreEqual(decimalChar.ToString(), format.NumberFormat.CurrencyDecimalSeparator)
            Assert.AreEqual(symbol, format.NumberFormat.CurrencySymbol)
            Assert.IsTrue(format.NumberFormat.IsReadOnly)
            Assert.IsTrue(format.Culture.IsReadOnly)
            Assert.AreSame(format.NumberFormat, format.Culture.NumberFormat)
        End Sub

        ''' <summary>
        ''' Verifies the currency format: the dollar symbol goes before the amount, the euro symbol after it.
        ''' </summary>
        <TestMethod>
        <DataRow(CurrencyKind.UsDollar, DecimalSeparatorKind.Dot, 1234.5, "$1,234.50")>
        <DataRow(CurrencyKind.UsDollar, DecimalSeparatorKind.Dot, -1234.5, "-$1,234.50")>
        <DataRow(CurrencyKind.UsDollar, DecimalSeparatorKind.Comma, 1234.5, "$1.234,50")>
        <DataRow(CurrencyKind.Euro, DecimalSeparatorKind.Comma, 1234.5, "1.234,50 €")>
        <DataRow(CurrencyKind.Euro, DecimalSeparatorKind.Comma, -1234.5, "-1.234,50 €")>
        <DataRow(CurrencyKind.Euro, DecimalSeparatorKind.Dot, 1234.5, "1,234.50 €")>
        Public Sub FormatCurrency_ReturnsExpectedText(currency As CurrencyKind, separator As DecimalSeparatorKind, amount As Double, expected As String)
            Assert.AreEqual(expected, New DisplayFormat(currency, separator).FormatCurrency(amount, 2))
        End Sub

        ''' <summary>
        ''' Verifies that the format does not depend on the culture of the current thread.
        ''' </summary>
        <TestMethod>
        <DataRow("de-DE")>
        <DataRow("es-ES")>
        <DataRow("en-US")>
        <DataRow("fr-FR")>
        Public Sub FormatCurrency_IsCultureIndependent(cultureName As String)
            Using scope As New Helpers.CultureScope(cultureName)
                Assert.AreEqual("$1,234.50", DisplayFormat.Default.FormatCurrency(1234.5, 2))
                Assert.AreEqual("1.234,50 €", New DisplayFormat(CurrencyKind.Euro, DecimalSeparatorKind.Comma).FormatCurrency(1234.5, 2))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that <see cref="DisplayFormat.WithCurrency"/> and <see cref="DisplayFormat.WithDecimalSeparator"/>
        ''' only change the specified setting.
        ''' </summary>
        <TestMethod>
        Public Sub WithMethods_ChangeOnlyOneSetting()
            Dim euroDot As DisplayFormat = DisplayFormat.Default.WithCurrency(CurrencyKind.Euro)
            Assert.AreEqual(CurrencyKind.Euro, euroDot.Currency)
            Assert.AreEqual(DecimalSeparatorKind.Dot, euroDot.DecimalSeparator)

            Dim euroComma As DisplayFormat = euroDot.WithDecimalSeparator(DecimalSeparatorKind.Comma)
            Assert.AreEqual(CurrencyKind.Euro, euroComma.Currency)
            Assert.AreEqual(DecimalSeparatorKind.Comma, euroComma.DecimalSeparator)
        End Sub

        ''' <summary>
        ''' Verifies the value equality of display formats.
        ''' </summary>
        <TestMethod>
        Public Sub Equals_ComparesSettings()
            Dim a As New DisplayFormat(CurrencyKind.Euro, DecimalSeparatorKind.Comma)
            Dim b As New DisplayFormat(CurrencyKind.Euro, DecimalSeparatorKind.Comma)
            Dim c As New DisplayFormat(CurrencyKind.Euro, DecimalSeparatorKind.Dot)

            Assert.AreEqual(a, b)
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode())
            Assert.AreNotEqual(a, c)
            Assert.AreEqual(DisplayFormat.Default, New DisplayFormat(CurrencyKind.UsDollar, DecimalSeparatorKind.Dot))
        End Sub

        ''' <summary>
        ''' Verifies that undefined enumeration values throw <see cref="ArgumentOutOfRangeException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub Constructor_UndefinedValues_Throw()
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() New DisplayFormat(CType(99, CurrencyKind), DecimalSeparatorKind.Dot))
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() New DisplayFormat(CurrencyKind.Euro, CType(99, DecimalSeparatorKind)))
        End Sub

    End Class

End Namespace

#End Region
