#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Globalization
Imports System.Threading
Imports System.Windows.Forms

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " Form1Tests "

Namespace UI

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Integration tests for the main window (<see cref="Global.Form1"/>),
    ''' verifying how user input is wired to the profit and loss calculation.
    ''' <para></para>
    ''' The window is shown off-screen, so these tests require an interactive Windows session.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class Form1Tests

#Region " Fields "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The window under test.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private form As Global.Form1

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The culture of the test thread before the test,
        ''' because <see cref="Global.Form1"/> changes it when loaded.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private originalCulture As CultureInfo

#End Region

#Region " Test Initialization / Cleanup "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Creates and shows the window under test.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestInitialize>
        Public Sub TestInitialize()
            Me.originalCulture = Thread.CurrentThread.CurrentCulture

            Me.form = New Global.Form1() With {
                .StartPosition = FormStartPosition.Manual,
                .Location = New Point(-32000, -32000),
                .ShowInTaskbar = False
            }
            Me.form.Show()
            Application.DoEvents()
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Closes the window under test and restores the culture of the test thread.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestCleanup>
        Public Sub TestCleanup()
            Me.form.Close()
            Me.form.Dispose()
            Thread.CurrentThread.CurrentCulture = Me.originalCulture
        End Sub

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Fills the input controls of the window in "Fixed" target price mode.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub EnterFixedInput(invested As Decimal, currentPrice As String, targetPrice As String, leverage As Decimal)
            Me.form.RadioButtonTargetPriceFixed.Checked = True
            Me.form.NumericUpDownLeverage.Value = leverage
            Me.form.NumericUpDownInvestedMoney.Value = invested
            Me.form.TextBoxCurrentPrice.Text = currentPrice
            Me.form.TextBoxTargetPrice.Text = targetPrice
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Formats an amount as US dollars, the same way the application does.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function FormatUsd(amount As Double) As String
            Return amount.ToString("C", CultureInfo.GetCultureInfo("en-US"))
        End Function

#End Region

#Region " Tests "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies the initial state of the window.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub InitialState_IsFixedModeWithoutResult()
            Assert.IsTrue(Me.form.RadioButtonTargetPriceFixed.Checked)
            Assert.IsTrue(Me.form.TextBoxTargetPrice.Enabled)
            Assert.IsFalse(Me.form.NumericUpDownTargetPrice.Enabled)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
            Assert.AreEqual(1D, Me.form.NumericUpDownLeverage.Value)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that loading the window sets the invariant culture to the UI thread.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Load_SetsInvariantCulture()
            Assert.AreEqual(CultureInfo.InvariantCulture, Thread.CurrentThread.CurrentCulture)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies a profitable calculation in "Fixed" target price mode.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FixedMode_Profit_ShowsProfitInGreen()
            Me.EnterFixedInput(100D, "100", "150", 1D)

            Assert.AreEqual("$50.00  (+50%)", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Color.GreenYellow, Me.form.LabelProfitValue.ForeColor)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies a losing calculation in "Fixed" target price mode.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FixedMode_Loss_ShowsLossInRed()
            Me.EnterFixedInput(100D, "100", "50", 1D)

            Assert.AreEqual($"{FormatUsd(-50)}  (-50%)", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Color.IndianRed, Me.form.LabelProfitValue.ForeColor)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies a break-even calculation in "Fixed" target price mode.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FixedMode_BreakEven_ShowsZeroInGray()
            Me.EnterFixedInput(100D, "100", "100", 1D)

            Assert.AreEqual("$0", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Color.LightGray, Me.form.LabelProfitValue.ForeColor)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the leverage multiplies the profit and loss amount.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FixedMode_WithLeverage_MultipliesAmount()
            Me.EnterFixedInput(100D, "100", "110", 10D)

            Assert.AreEqual("$100.00  (+10%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that changing the leverage after the input recalculates the result.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FixedMode_ChangingLeverage_Recalculates()
            Me.EnterFixedInput(100D, "100", "110", 1D)
            Assert.AreEqual("$10.00  (+10%)", Me.form.LabelProfitValue.Text)

            Me.form.NumericUpDownLeverage.Value = 5D
            Assert.AreEqual("$50.00  (+10%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the comma is accepted as decimal separator in price text boxes.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FixedMode_CommaDecimalSeparator_IsAccepted()
            Me.EnterFixedInput(100D, "1,5", "3", 1D)

            Assert.AreEqual("$100.00  (+100%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that in "Fixed" mode the percentage control reflects the price change.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FixedMode_UpdatesPercentControl()
            Me.EnterFixedInput(100D, "200", "250", 1D)

            Assert.AreEqual(25D, Me.form.NumericUpDownTargetPrice.Value)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that a negative target price is reset to zero.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FixedMode_NegativeTargetPrice_IsResetToZero()
            Me.EnterFixedInput(100D, "100", "-5", 1D)

            Assert.AreEqual("0", Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual($"{FormatUsd(-100)}  (-100%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the result is not available without invested money.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ZeroInvested_ShowsNotAvailable()
            Me.EnterFixedInput(0D, "100", "150", 1D)

            Assert.AreEqual("N/A", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Color.LightGray, Me.form.LabelProfitValue.ForeColor)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the result is not available without a valid current price.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("")>
        <DataRow("0")>
        <DataRow("abc")>
        Public Sub InvalidCurrentPrice_ShowsNotAvailable(currentPrice As String)
            Me.EnterFixedInput(100D, currentPrice, "150", 1D)

            Assert.AreEqual("N/A", Me.form.LabelProfitValue.Text)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the result is not available without a target price.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub EmptyTargetPrice_ShowsNotAvailable()
            Me.EnterFixedInput(100D, "100", "150", 1D)
            Me.form.TextBoxTargetPrice.Text = String.Empty
            Me.form.NumericUpDownLeverage.Value = 2D ' Forces a recalculation.

            Assert.AreEqual("N/A", Me.form.LabelProfitValue.Text)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that selecting "Percent" mode swaps the enabled input controls.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub PercentMode_SwapsEnabledControls()
            Me.form.RadioButtonTargetPricePercent.Checked = True

            Assert.IsFalse(Me.form.TextBoxTargetPrice.Enabled)
            Assert.IsTrue(Me.form.NumericUpDownTargetPrice.Enabled)

            Me.form.RadioButtonTargetPriceFixed.Checked = True

            Assert.IsTrue(Me.form.TextBoxTargetPrice.Enabled)
            Assert.IsFalse(Me.form.NumericUpDownTargetPrice.Enabled)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that in "Percent" mode the target price is calculated from the percentage.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub PercentMode_CalculatesTargetPriceFromPercent()
            Me.form.RadioButtonTargetPricePercent.Checked = True
            Me.form.NumericUpDownInvestedMoney.Value = 100D
            Me.form.TextBoxCurrentPrice.Text = "200"
            Me.form.NumericUpDownTargetPrice.Value = 25D

            Assert.AreEqual("250", Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual("$25.00  (+25%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that in "Percent" mode, changing the current price keeps the percentage
        ''' and recalculates the target price.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub PercentMode_ChangingCurrentPrice_RecalculatesTargetPrice()
            Me.form.RadioButtonTargetPricePercent.Checked = True
            Me.form.NumericUpDownInvestedMoney.Value = 100D
            Me.form.TextBoxCurrentPrice.Text = "200"
            Me.form.NumericUpDownTargetPrice.Value = -10D

            Me.form.TextBoxCurrentPrice.Text = "50"

            Assert.AreEqual("45", Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual(-10D, Me.form.NumericUpDownTargetPrice.Value)
            Assert.AreEqual($"{FormatUsd(-10)}  (-10%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the "Clear" button resets the input and the result.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ClearButton_ResetsInputAndResult()
            Me.EnterFixedInput(100D, "100", "150", 10D)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)

            Me.form.ButtonClear.PerformClick()

            Assert.AreEqual(0D, Me.form.NumericUpDownInvestedMoney.Value)
            Assert.AreEqual(String.Empty, Me.form.TextBoxCurrentPrice.Text)
            Assert.AreEqual(String.Empty, Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual(1D, Me.form.NumericUpDownLeverage.Value)
            Assert.AreEqual("N/A", Me.form.LabelProfitValue.Text)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the "Always on top" check box toggles <see cref="Form.TopMost"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub AlwaysOnTopCheckBox_TogglesTopMost()
            Me.form.CheckBoxAlwaysOnTop.Checked = True
            Assert.IsTrue(Me.form.TopMost)

            Me.form.CheckBoxAlwaysOnTop.Checked = False
            Assert.IsFalse(Me.form.TopMost)
        End Sub

#End Region

    End Class

End Namespace

#End Region
