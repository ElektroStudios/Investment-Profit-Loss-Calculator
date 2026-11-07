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

#Region " ProfitLossResultTests "

Namespace Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Unit tests for the <see cref="ProfitLossResult"/> class.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <TestClass>
    Public NotInheritable Class ProfitLossResultTests

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Formats an amount as US dollars, the same way the application does.
        ''' <para></para>
        ''' The negative currency pattern of "en-US" depends on the runtime and OS culture data
        ''' (e.g. "($50.00)" vs. "-$50.00"), so tests involving losses build the currency part with this method.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function FormatUsd(amount As Double) As String
            Return amount.ToString("C", CultureInfo.GetCultureInfo("en-US"))
        End Function

#End Region

#Region " NotAvailable "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the state of <see cref="ProfitLossResult.NotAvailable"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub NotAvailable_HasExpectedState()
            Dim result As ProfitLossResult = ProfitLossResult.NotAvailable

            Assert.AreEqual(ProfitLossKind.NotAvailable, result.Kind)
            Assert.IsFalse(result.IsAvailable)
            Assert.AreEqual(0.0, result.Amount)
            Assert.AreEqual(0.0, result.ChangePercent)
            Assert.AreEqual("N/A", result.ToDisplayText())
            Assert.AreEqual(ProfitLossResult.NotAvailableText, result.ToDisplayText())
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the calculator returns the shared <see cref="ProfitLossResult.NotAvailable"/> instance.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub NotAvailable_IsSharedInstance()
            Dim first As ProfitLossResult = ProfitLossCalculator.Calculate(0, 100, 150, 1)
            Dim second As ProfitLossResult = ProfitLossCalculator.Calculate(100, 0, 150, 1)

            Assert.AreSame(first, second)
            Assert.AreSame(ProfitLossResult.NotAvailable, first)
        End Sub

#End Region

#Region " Kind "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the kind is determined by the sign of the amount.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(0.01, ProfitLossKind.Profit)>
        <DataRow(1000000.0, ProfitLossKind.Profit)>
        <DataRow(-0.01, ProfitLossKind.Loss)>
        <DataRow(-1000000.0, ProfitLossKind.Loss)>
        <DataRow(0.0, ProfitLossKind.BreakEven)>
        Public Sub Constructor_KindDependsOnAmountSign(amount As Double, expected As ProfitLossKind)
            Dim result As New ProfitLossResult(amount, 0)

            Assert.AreEqual(expected, result.Kind)
            Assert.IsTrue(result.IsAvailable)
            Assert.AreEqual(amount, result.Amount)
        End Sub

#End Region

#Region " ToDisplayText "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the display text of profitable results.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(50.0, 50.0, "$50.00  (+50%)")>
        <DataRow(1234.5, 12.5, "$1,234.50  (+12.5%)")>
        <DataRow(100.0, 33.333333, "$100.00  (+33.33%)")>
        <DataRow(0.126, 0.004, "$0.13  (+0%)")>
        <DataRow(1000000.0, 1000.0, "$1,000,000.00  (+1000%)")>
        Public Sub ToDisplayText_Profit_ReturnsExpectedText(amount As Double, changePercent As Double, expected As String)
            Dim result As New ProfitLossResult(amount, changePercent)
            Assert.AreEqual(expected, result.ToDisplayText())
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the display text of losing results.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(-50.0, -50.0, "  (-50%)")>
        <DataRow(-1234.5, -12.5, "  (-12.5%)")>
        <DataRow(-100.0, -100.0, "  (-100%)")>
        <DataRow(-10.0, -66.666666, "  (-66.67%)")>
        Public Sub ToDisplayText_Loss_ReturnsExpectedText(amount As Double, changePercent As Double, expectedSuffix As String)
            Dim result As New ProfitLossResult(amount, changePercent)
            Assert.AreEqual($"{FormatUsd(amount)}{expectedSuffix}", result.ToDisplayText())
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the currency part of a losing result is a negative US dollar amount,
        ''' whatever negative currency pattern is used by the runtime.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ToDisplayText_Loss_CurrencyIsNegativeUsd()
            Dim text As String = New ProfitLossResult(-50, -50).ToDisplayText()
            Dim currencyPart As String = text.Substring(0, text.IndexOf("  ", StringComparison.Ordinal))

            Assert.IsTrue(currencyPart = "($50.00)" OrElse currencyPart = "-$50.00",
                          $"Unexpected currency format: '{currencyPart}'")
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that a losing result does not get the "+" sign that profitable results get.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ToDisplayText_Loss_DoesNotContainPlusSign()
            Dim text As String = New ProfitLossResult(-50, -50).ToDisplayText()
            Assert.DoesNotContain("+", text)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the display text of break-even results, which never includes the percentage.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(0.0)>
        <DataRow(15.0)>
        <DataRow(-15.0)>
        Public Sub ToDisplayText_BreakEven_ReturnsZeroDollars(changePercent As Double)
            Dim result As New ProfitLossResult(0, changePercent)
            Assert.AreEqual("$0", result.ToDisplayText())
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the display text does not depend on the culture of the current thread.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("de-DE")>
        <DataRow("es-ES")>
        <DataRow("fr-FR")>
        <DataRow("ja-JP")>
        Public Sub ToDisplayText_IsCultureIndependent(cultureName As String)
            Dim expectedProfit As String
            Dim expectedLoss As String
            Dim expectedBreakEven As String

            Using scope As New CultureScope("en-US")
                expectedProfit = New ProfitLossResult(1234.5, 12.75).ToDisplayText()
                expectedLoss = New ProfitLossResult(-1234.5, -12.75).ToDisplayText()
                expectedBreakEven = New ProfitLossResult(0, 0).ToDisplayText()
            End Using

            Using scope As New CultureScope(cultureName)
                Assert.AreEqual(expectedProfit, New ProfitLossResult(1234.5, 12.75).ToDisplayText())
                Assert.AreEqual(expectedLoss, New ProfitLossResult(-1234.5, -12.75).ToDisplayText())
                Assert.AreEqual(expectedBreakEven, New ProfitLossResult(0, 0).ToDisplayText())
            End Using

            Assert.AreEqual("$1,234.50  (+12.75%)", expectedProfit)
        End Sub

#End Region

#Region " ToString "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that <see cref="ProfitLossResult.ToString"/> returns the display text.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ToString_ReturnsDisplayText()
            Dim profit As New ProfitLossResult(10, 5)
            Dim loss As New ProfitLossResult(-10, -5)

            Assert.AreEqual(profit.ToDisplayText(), profit.ToString())
            Assert.AreEqual(loss.ToDisplayText(), loss.ToString())
            Assert.AreEqual(ProfitLossResult.NotAvailable.ToDisplayText(), ProfitLossResult.NotAvailable.ToString())
        End Sub

#End Region

    End Class

End Namespace

#End Region
