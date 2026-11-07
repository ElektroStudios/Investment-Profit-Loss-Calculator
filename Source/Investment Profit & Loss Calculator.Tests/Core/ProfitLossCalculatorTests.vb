#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Core

Imports InvestmentProfitLossCalculator.Tests.Helpers

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " ProfitLossCalculatorTests "

Namespace Core

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Unit tests for the <see cref="ProfitLossCalculator"/> class.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <TestClass>
    Public NotInheritable Class ProfitLossCalculatorTests

#Region " Constants "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The tolerance used to compare floating point values.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Const Tolerance As Double = 0.000000001

#End Region

#Region " TryParsePrice "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that valid prices are parsed, accepting both dot and comma as decimal separator.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("123.45", 123.45, DisplayName:="Dot decimal separator")>
        <DataRow("123,45", 123.45, DisplayName:="Comma decimal separator")>
        <DataRow("0", 0.0, DisplayName:="Zero")>
        <DataRow("42", 42.0, DisplayName:="Integer")>
        <DataRow("  42  ", 42.0, DisplayName:="Leading and trailing white spaces")>
        <DataRow("-5.5", -5.5, DisplayName:="Negative value")>
        <DataRow(".5", 0.5, DisplayName:="Missing integer part")>
        <DataRow("1e3", 1000.0, DisplayName:="Scientific notation")>
        <DataRow("0.00000123", 0.00000123, DisplayName:="Very small value")>
        Public Sub TryParsePrice_ValidText_ReturnsTrueAndValue(text As String, expected As Double)
            Dim value As Double
            Dim success As Boolean = ProfitLossCalculator.TryParsePrice(text, value)

            Assert.IsTrue(success)
            Assert.AreEqual(expected, value, Tolerance)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that invalid prices are rejected and the output value is reset to zero.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("", DisplayName:="Empty")>
        <DataRow("   ", DisplayName:="White spaces")>
        <DataRow("abc", DisplayName:="Letters")>
        <DataRow("12abc", DisplayName:="Trailing letters")>
        <DataRow("$100", DisplayName:="Currency symbol")>
        <DataRow("1,000.50", DisplayName:="Thousands separator and decimal separator")>
        <DataRow("1.2.3", DisplayName:="Multiple decimal separators")>
        <DataRow("NaN", DisplayName:="NaN")>
        <DataRow("Infinity", DisplayName:="Infinity")>
        <DataRow("-Infinity", DisplayName:="Negative infinity")>
        <DataRow("1e400", DisplayName:="Overflow")>
        Public Sub TryParsePrice_InvalidText_ReturnsFalseAndZero(text As String)
            Dim value As Double = 999
            Dim success As Boolean = ProfitLossCalculator.TryParsePrice(text, value)

            Assert.IsFalse(success)
            Assert.AreEqual(0.0, value)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that parsing does not depend on the culture of the current thread.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("de-DE")>
        <DataRow("es-ES")>
        <DataRow("fr-FR")>
        <DataRow("en-US")>
        <DataRow("ja-JP")>
        Public Sub TryParsePrice_IsCultureIndependent(cultureName As String)
            Using scope As New CultureScope(cultureName)
                Dim dotValue As Double
                Dim commaValue As Double

                Assert.IsTrue(ProfitLossCalculator.TryParsePrice("1.5", dotValue))
                Assert.IsTrue(ProfitLossCalculator.TryParsePrice("1,5", commaValue))
                Assert.AreEqual(1.5, dotValue, Tolerance)
                Assert.AreEqual(1.5, commaValue, Tolerance)
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that a null text throws <see cref="ArgumentNullException"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub TryParsePrice_NullText_ThrowsArgumentNullException()
            Dim value As Double
            Assert.ThrowsExactly(Of ArgumentNullException)(
                Sub() ProfitLossCalculator.TryParsePrice(Nothing, value))
        End Sub

#End Region

#Region " CalculateTargetPrice "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the target price calculated from a percentage change.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(100.0, 10.0, 110.0, DisplayName:="+10%")>
        <DataRow(100.0, -10.0, 90.0, DisplayName:="-10%")>
        <DataRow(100.0, 0.0, 100.0, DisplayName:="0%")>
        <DataRow(100.0, -100.0, 0.0, DisplayName:="-100%")>
        <DataRow(100.0, 250.0, 350.0, DisplayName:="+250%")>
        <DataRow(50.0, 0.5, 50.25, DisplayName:="Fractional percent")>
        <DataRow(0.0, 50.0, 0.0, DisplayName:="Zero current price")>
        <DataRow(42000.0, 5.0, 44100.0, DisplayName:="BTC-like price")>
        Public Sub CalculateTargetPrice_ReturnsExpectedPrice(currentPrice As Double, percent As Double, expected As Double)
            Dim actual As Double = ProfitLossCalculator.CalculateTargetPrice(currentPrice, percent)
            Assert.AreEqual(expected, actual, Tolerance)
        End Sub

#End Region

#Region " CalculatePriceChangePercent "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the percentage change between two prices.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(100.0, 110.0, 10.0, DisplayName:="Price up")>
        <DataRow(100.0, 90.0, -10.0, DisplayName:="Price down")>
        <DataRow(100.0, 100.0, 0.0, DisplayName:="Unchanged")>
        <DataRow(200.0, 0.0, -100.0, DisplayName:="Price drops to zero")>
        <DataRow(100.0, 300.0, 200.0, DisplayName:="Price triples")>
        <DataRow(-100.0, -50.0, 50.0, DisplayName:="Negative current price uses absolute value")>
        Public Sub CalculatePriceChangePercent_ReturnsExpectedPercent(currentPrice As Double, targetPrice As Double, expected As Double)
            Dim actual As Double = ProfitLossCalculator.CalculatePriceChangePercent(currentPrice, targetPrice)
            Assert.AreEqual(expected, actual, Tolerance)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the percentage change is NaN when both prices are zero.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub CalculatePriceChangePercent_BothPricesZero_ReturnsNaN()
            Dim actual As Double = ProfitLossCalculator.CalculatePriceChangePercent(0, 0)
            Assert.IsTrue(Double.IsNaN(actual))
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the percentage change is infinite when only the current price is zero.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub CalculatePriceChangePercent_ZeroCurrentPrice_ReturnsInfinity()
            Assert.IsTrue(Double.IsPositiveInfinity(ProfitLossCalculator.CalculatePriceChangePercent(0, 10)))
            Assert.IsTrue(Double.IsNegativeInfinity(ProfitLossCalculator.CalculatePriceChangePercent(0, -10)))
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that <see cref="ProfitLossCalculator.CalculatePriceChangePercent"/> is the inverse of
        ''' <see cref="ProfitLossCalculator.CalculateTargetPrice"/> for positive prices.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(100.0, 12.5)>
        <DataRow(0.0001, -37.25)>
        <DataRow(65432.1, 999.99)>
        <DataRow(1.0, -100.0)>
        <DataRow(3.14159, 0.01)>
        Public Sub CalculatePriceChangePercent_IsInverseOfCalculateTargetPrice(currentPrice As Double, percent As Double)
            Dim targetPrice As Double = ProfitLossCalculator.CalculateTargetPrice(currentPrice, percent)
            Dim actual As Double = ProfitLossCalculator.CalculatePriceChangePercent(currentPrice, targetPrice)
            Assert.AreEqual(percent, actual, 0.0000001)
        End Sub

#End Region

#Region " CalculateProfitLoss "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the profit and loss amount for different scenarios.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(100.0, 100.0, 150.0, 1.0, 50.0, DisplayName:="Profit without leverage")>
        <DataRow(100.0, 100.0, 50.0, 1.0, -50.0, DisplayName:="Loss without leverage")>
        <DataRow(100.0, 100.0, 100.0, 1.0, 0.0, DisplayName:="Break-even")>
        <DataRow(100.0, 100.0, 110.0, 10.0, 100.0, DisplayName:="Profit with x10 leverage")>
        <DataRow(100.0, 100.0, 95.0, 10.0, -50.0, DisplayName:="Loss with x10 leverage")>
        <DataRow(100.0, 100.0, 0.0, 1.0, -100.0, DisplayName:="Total loss when price drops to zero")>
        <DataRow(250.0, 40000.0, 50000.0, 2.0, 125.0, DisplayName:="BTC-like scenario with x2 leverage")>
        <DataRow(1000.0, 0.5, 0.75, 1.0, 500.0, DisplayName:="Fractional prices")>
        <DataRow(0.0, 100.0, 150.0, 1.0, 0.0, DisplayName:="Zero invested")>
        Public Sub CalculateProfitLoss_ReturnsExpectedAmount(invested As Double, currentPrice As Double, targetPrice As Double, leverage As Double, expected As Double)
            Dim actual As Double = ProfitLossCalculator.CalculateProfitLoss(invested, currentPrice, targetPrice, leverage)
            Assert.AreEqual(expected, actual, 0.0000001)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Documents that the calculator does not model liquidations:
        ''' with leverage, the calculated loss can exceed the invested money.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub CalculateProfitLoss_LeveragedLoss_IsNotCappedToInvestedMoney()
            Dim actual As Double = ProfitLossCalculator.CalculateProfitLoss(100, 100, 0, 5)
            Assert.AreEqual(-500.0, actual, Tolerance)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the profit and loss amount scales linearly with the leverage.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(2.0)>
        <DataRow(10.0)>
        <DataRow(125.0)>
        <DataRow(5000.0)>
        Public Sub CalculateProfitLoss_ScalesLinearlyWithLeverage(leverage As Double)
            Dim baseAmount As Double = ProfitLossCalculator.CalculateProfitLoss(100, 80, 92, 1)
            Dim leveragedAmount As Double = ProfitLossCalculator.CalculateProfitLoss(100, 80, 92, leverage)
            Assert.AreEqual(baseAmount * leverage, leveragedAmount, 0.0000001)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that a zero current price throws <see cref="ArgumentOutOfRangeException"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub CalculateProfitLoss_ZeroCurrentPrice_Throws()
            Dim ex As ArgumentOutOfRangeException = Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
                Function() ProfitLossCalculator.CalculateProfitLoss(100, 0, 150, 1))
            Assert.AreEqual("currentPrice", ex.ParamName)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that out of range arguments throw <see cref="ArgumentOutOfRangeException"/>
        ''' naming the offending parameter.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(-1.0, 100.0, 150.0, 1.0, "invested", DisplayName:="Negative invested")>
        <DataRow(Double.NaN, 100.0, 150.0, 1.0, "invested", DisplayName:="NaN invested")>
        <DataRow(Double.PositiveInfinity, 100.0, 150.0, 1.0, "invested", DisplayName:="Infinite invested")>
        <DataRow(100.0, Double.NaN, 150.0, 1.0, "currentPrice", DisplayName:="NaN current price")>
        <DataRow(100.0, Double.NegativeInfinity, 150.0, 1.0, "currentPrice", DisplayName:="Infinite current price")>
        <DataRow(100.0, 100.0, Double.NaN, 1.0, "targetPrice", DisplayName:="NaN target price")>
        <DataRow(100.0, 100.0, Double.PositiveInfinity, 1.0, "targetPrice", DisplayName:="Infinite target price")>
        <DataRow(100.0, 100.0, 150.0, 0.0, "leverage", DisplayName:="Zero leverage")>
        <DataRow(100.0, 100.0, 150.0, -2.0, "leverage", DisplayName:="Negative leverage")>
        <DataRow(100.0, 100.0, 150.0, Double.NaN, "leverage", DisplayName:="NaN leverage")>
        Public Sub CalculateProfitLoss_InvalidArgument_Throws(invested As Double, currentPrice As Double, targetPrice As Double, leverage As Double, expectedParamName As String)
            Dim ex As ArgumentOutOfRangeException = Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
                Function() ProfitLossCalculator.CalculateProfitLoss(invested, currentPrice, targetPrice, leverage))
            Assert.AreEqual(expectedParamName, ex.ParamName)
        End Sub

#End Region

#Region " Calculate "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the result is not available when there is no invested money.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_ZeroInvested_ReturnsNotAvailable()
            Dim result As ProfitLossResult = ProfitLossCalculator.Calculate(0, 100, 150, 1)
            Assert.AreSame(ProfitLossResult.NotAvailable, result)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the result is not available when the current price is zero.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_ZeroCurrentPrice_ReturnsNotAvailable()
            Dim result As ProfitLossResult = ProfitLossCalculator.Calculate(100, 0, 150, 1)
            Assert.AreSame(ProfitLossResult.NotAvailable, result)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the kind, amount and percentage change of a profitable result.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_Profit_ReturnsProfitResult()
            Dim result As ProfitLossResult = ProfitLossCalculator.Calculate(100, 100, 150, 1)

            Assert.AreEqual(ProfitLossKind.Profit, result.Kind)
            Assert.IsTrue(result.IsAvailable)
            Assert.AreEqual(50.0, result.Amount, Tolerance)
            Assert.AreEqual(50.0, result.ChangePercent, Tolerance)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the kind, amount and percentage change of a losing result.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_Loss_ReturnsLossResult()
            Dim result As ProfitLossResult = ProfitLossCalculator.Calculate(200, 50, 40, 1)

            Assert.AreEqual(ProfitLossKind.Loss, result.Kind)
            Assert.IsTrue(result.IsAvailable)
            Assert.AreEqual(-40.0, result.Amount, Tolerance)
            Assert.AreEqual(-20.0, result.ChangePercent, Tolerance)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the kind, amount and percentage change of a break-even result.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_BreakEven_ReturnsBreakEvenResult()
            Dim result As ProfitLossResult = ProfitLossCalculator.Calculate(100, 75, 75, 3)

            Assert.AreEqual(ProfitLossKind.BreakEven, result.Kind)
            Assert.IsTrue(result.IsAvailable)
            Assert.AreEqual(0.0, result.Amount)
            Assert.AreEqual(0.0, result.ChangePercent)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the leverage affects the amount,
        ''' but not the percentage change (which is the asset price change).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_Leverage_AffectsAmountButNotChangePercent()
            Dim result As ProfitLossResult = ProfitLossCalculator.Calculate(100, 100, 110, 20)

            Assert.AreEqual(200.0, result.Amount, Tolerance)
            Assert.AreEqual(10.0, result.ChangePercent, Tolerance)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that a drop of the target price to zero results in a total loss.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_ZeroTargetPrice_ReturnsTotalLoss()
            Dim result As ProfitLossResult = ProfitLossCalculator.Calculate(100, 100, 0, 1)

            Assert.AreEqual(ProfitLossKind.Loss, result.Kind)
            Assert.AreEqual(-100.0, result.Amount, Tolerance)
            Assert.AreEqual(-100.0, result.ChangePercent, Tolerance)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the arguments are validated even when the result would not be available.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_InvalidLeverage_ThrowsEvenWhenNotAvailable()
            Dim ex As ArgumentOutOfRangeException = Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
                Function() ProfitLossCalculator.Calculate(0, 0, 0, 0))
            Assert.AreEqual("leverage", ex.ParamName)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that a negative invested amount throws <see cref="ArgumentOutOfRangeException"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Calculate_NegativeInvested_Throws()
            Dim ex As ArgumentOutOfRangeException = Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
                Function() ProfitLossCalculator.Calculate(-100, 100, 150, 1))
            Assert.AreEqual("invested", ex.ParamName)
        End Sub

#End Region

    End Class

End Namespace

#End Region
