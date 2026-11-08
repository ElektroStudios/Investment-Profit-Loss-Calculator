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

Imports Core

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Theming

#End Region

#Region " Form1Tests "

Namespace UI

    ''' <summary>
    ''' Integration tests for the main window (<see cref="Global.Form1"/>),
    ''' verifying how user input is wired to the profit and loss calculation.
    ''' <para></para>
    ''' The window is shown off-screen, so these tests require an interactive Windows session.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class Form1Tests

#Region " Fields "

        ''' <summary>
        ''' The window under test.
        ''' </summary>
        Private form As Global.Form1

        ''' <summary>
        ''' The culture of the test thread before the test,
        ''' because <see cref="Global.Form1"/> changes it when loaded.
        ''' </summary>
        Private originalCulture As CultureInfo

#End Region

#Region " Test Initialization / Cleanup "

        ''' <summary>
        ''' Creates and shows the window under test.
        ''' </summary>
        <TestInitialize>
        Public Sub TestInitialize()
            Me.originalCulture = Thread.CurrentThread.CurrentCulture

            Me.form = New Global.Form1() With {
                .StartPosition = FormStartPosition.Manual,
                .Location = New Point(-32000, -32000),
                .ShowInTaskbar = False
            }
            ' The display format saved in the settings of the test host is ignored: tests start with the default one.
            Me.form.ApplyDisplayFormat(DisplayFormat.Default, persist:=False)
            Me.form.Show()
            Application.DoEvents()
        End Sub

        ''' <summary>
        ''' Closes the window under test and restores the culture of the test thread.
        ''' </summary>
        <TestCleanup>
        Public Sub TestCleanup()
            Me.form.Close()
            Me.form.Dispose()
            Thread.CurrentThread.CurrentCulture = Me.originalCulture
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Fills the input controls of the window in "Fixed" target price mode.
        ''' </summary>
        Private Sub EnterFixedInput(invested As Decimal, currentPrice As String, targetPrice As String, leverage As Decimal)
            Me.form.RadioButtonTargetPriceFixed.Checked = True
            Me.form.NumericUpDownLeverage.Value = leverage
            Me.form.NumericUpDownInvestedMoney.Value = invested
            Me.form.TextBoxCurrentPrice.Text = currentPrice
            Me.form.TextBoxTargetPrice.Text = targetPrice
        End Sub

        ''' <summary>
        ''' Formats an amount as US dollars with the default display format of the application.
        ''' </summary>
        Private Shared Function FormatUsd(amount As Double) As String
            Return DisplayFormat.Default.FormatCurrency(amount, 2)
        End Function

#End Region

#Region " Tests "

        ''' <summary>
        ''' Verifies the initial state of the window.
        ''' </summary>
        <TestMethod>
        Public Sub InitialState_IsFixedModeWithoutResult()
            Assert.IsTrue(Me.form.RadioButtonTargetPriceFixed.Checked)
            Assert.IsTrue(Me.form.TextBoxTargetPrice.Enabled)
            Assert.IsFalse(Me.form.NumericUpDownTargetPrice.Enabled)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
            Assert.AreEqual(1D, Me.form.NumericUpDownLeverage.Value)
        End Sub

        ''' <summary>
        ''' Verifies that the culture of the UI thread uses the number format of the current display format.
        ''' </summary>
        <TestMethod>
        Public Sub Load_SetsDisplayFormatCulture()
            Assert.AreSame(Me.form.CurrentDisplayFormat.Culture, Thread.CurrentThread.CurrentCulture)
            Assert.AreEqual(".", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
        End Sub

        ''' <summary>
        ''' Verifies a profitable calculation in "Fixed" target price mode.
        ''' </summary>
        <TestMethod>
        Public Sub FixedMode_Profit_ShowsProfitInGreen()
            Me.EnterFixedInput(100D, "100", "150", 1D)

            Assert.AreEqual("$50.00  (+50%)", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Me.form.CurrentPalette.ProfitColor, Me.form.LabelProfitValue.ForeColor)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies a losing calculation in "Fixed" target price mode.
        ''' </summary>
        <TestMethod>
        Public Sub FixedMode_Loss_ShowsLossInRed()
            Me.EnterFixedInput(100D, "100", "50", 1D)

            Assert.AreEqual($"{FormatUsd(-50)}  (-50%)", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Me.form.CurrentPalette.LossColor, Me.form.LabelProfitValue.ForeColor)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies a break-even calculation in "Fixed" target price mode.
        ''' </summary>
        <TestMethod>
        Public Sub FixedMode_BreakEven_ShowsZeroInGray()
            Me.EnterFixedInput(100D, "100", "100", 1D)

            Assert.AreEqual("$0", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Me.form.CurrentPalette.NeutralResultColor, Me.form.LabelProfitValue.ForeColor)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies that the leverage multiplies the profit and loss amount.
        ''' </summary>
        <TestMethod>
        Public Sub FixedMode_WithLeverage_MultipliesAmount()
            Me.EnterFixedInput(100D, "100", "110", 10D)

            Assert.AreEqual("$100.00  (+10%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that changing the leverage after the input recalculates the result.
        ''' </summary>
        <TestMethod>
        Public Sub FixedMode_ChangingLeverage_Recalculates()
            Me.EnterFixedInput(100D, "100", "110", 1D)
            Assert.AreEqual("$10.00  (+10%)", Me.form.LabelProfitValue.Text)

            Me.form.NumericUpDownLeverage.Value = 5D
            Assert.AreEqual("$50.00  (+10%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that the comma is accepted as decimal separator in price text boxes.
        ''' </summary>
        <TestMethod>
        Public Sub FixedMode_CommaDecimalSeparator_IsAccepted()
            Me.EnterFixedInput(100D, "1,5", "3", 1D)

            Assert.AreEqual("$100.00  (+100%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that in "Fixed" mode the percentage control reflects the price change.
        ''' </summary>
        <TestMethod>
        Public Sub FixedMode_UpdatesPercentControl()
            Me.EnterFixedInput(100D, "200", "250", 1D)

            Assert.AreEqual(25D, Me.form.NumericUpDownTargetPrice.Value)
        End Sub

        ''' <summary>
        ''' Verifies that a negative target price is reset to zero.
        ''' </summary>
        <TestMethod>
        Public Sub FixedMode_NegativeTargetPrice_IsResetToZero()
            Me.EnterFixedInput(100D, "100", "-5", 1D)

            Assert.AreEqual("0.00", Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual($"{FormatUsd(-100)}  (-100%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that the result is not available without invested money.
        ''' </summary>
        <TestMethod>
        Public Sub ZeroInvested_ShowsNotAvailable()
            Me.EnterFixedInput(0D, "100", "150", 1D)

            Assert.AreEqual("N/A", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Me.form.CurrentPalette.NeutralResultColor, Me.form.LabelProfitValue.ForeColor)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies that the result is not available without an entry price (empty or zero).
        ''' <para></para>
        ''' A non-numeric entry price is not "not available" but an input error: see <see cref="InvalidEntryPrice_IsReported"/>.
        ''' </summary>
        <TestMethod>
        <DataRow("", DisplayName:="Empty")>
        <DataRow("0", DisplayName:="Zero")>
        Public Sub MissingCurrentPrice_ShowsNotAvailable(currentPrice As String)
            Me.EnterFixedInput(100D, currentPrice, "150", 1D)

            Assert.AreEqual("N/A", Me.form.LabelProfitValue.Text)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies that the result is not available without a target price.
        ''' </summary>
        <TestMethod>
        Public Sub EmptyTargetPrice_ShowsNotAvailable()
            Me.EnterFixedInput(100D, "100", "150", 1D)
            Me.form.TextBoxTargetPrice.Text = String.Empty
            Me.form.NumericUpDownLeverage.Value = 2D ' Forces a recalculation.

            Assert.AreEqual("N/A", Me.form.LabelProfitValue.Text)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies that selecting "Percent" mode swaps the enabled input controls.
        ''' </summary>
        <TestMethod>
        Public Sub PercentMode_SwapsEnabledControls()
            Me.form.RadioButtonTargetPricePercent.Checked = True

            Assert.IsFalse(Me.form.TextBoxTargetPrice.Enabled)
            Assert.IsTrue(Me.form.NumericUpDownTargetPrice.Enabled)

            Me.form.RadioButtonTargetPriceFixed.Checked = True

            Assert.IsTrue(Me.form.TextBoxTargetPrice.Enabled)
            Assert.IsFalse(Me.form.NumericUpDownTargetPrice.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies that in "Percent" mode the target price is calculated from the percentage.
        ''' </summary>
        <TestMethod>
        Public Sub PercentMode_CalculatesTargetPriceFromPercent()
            Me.form.RadioButtonTargetPricePercent.Checked = True
            Me.form.NumericUpDownInvestedMoney.Value = 100D
            Me.form.TextBoxCurrentPrice.Text = "200"
            Me.form.NumericUpDownTargetPrice.Value = 25D

            Assert.AreEqual("250.00", Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual("$25.00  (+25%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that in "Percent" mode, changing the current price keeps the percentage
        ''' and recalculates the target price.
        ''' </summary>
        <TestMethod>
        Public Sub PercentMode_ChangingCurrentPrice_RecalculatesTargetPrice()
            Me.form.RadioButtonTargetPricePercent.Checked = True
            Me.form.NumericUpDownInvestedMoney.Value = 100D
            Me.form.TextBoxCurrentPrice.Text = "200"
            Me.form.NumericUpDownTargetPrice.Value = -10D

            Me.form.TextBoxCurrentPrice.Text = "50"

            Assert.AreEqual("45.00", Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual(-10D, Me.form.NumericUpDownTargetPrice.Value)
            Assert.AreEqual($"{FormatUsd(-10)}  (-10%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that the "Clear" button resets the input and the result.
        ''' </summary>
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

        ''' <summary>
        ''' Verifies that the "Always on top" check box toggles <see cref="Form.TopMost"/>.
        ''' </summary>
        <TestMethod>
        Public Sub AlwaysOnTopCheckBox_TogglesTopMost()
            Me.form.CheckBoxAlwaysOnTop.Checked = True
            Assert.IsTrue(Me.form.TopMost)

            Me.form.CheckBoxAlwaysOnTop.Checked = False
            Assert.IsFalse(Me.form.TopMost)
        End Sub

        ''' <summary>
        ''' Verifies that the palette applied to the window corresponds to the Windows app mode.
        ''' </summary>
        <TestMethod>
        Public Sub Theme_FollowsWindowsAppMode()
            Dim expected As ThemePalette = ThemePalette.FromDarkMode(WindowsTheme.IsAppDarkModeEnabled())

            Assert.AreSame(expected, Me.form.CurrentPalette)
            Assert.AreEqual(expected.WindowBackColor, Me.form.BackColor)
            Assert.AreEqual(expected.InputBackColor, Me.form.TextBoxCurrentPrice.BackColor)
            Assert.AreEqual(expected.TextColor, Me.form.TextBoxCurrentPrice.ForeColor)
            Assert.AreEqual(expected.InputBackColor, Me.form.NumericUpDownInvestedMoney.BackColor)
            Assert.AreEqual(expected.CardBackColor, Me.form.PanelResult.BackColor)
        End Sub

        ''' <summary>
        ''' Verifies that the selected "Price" / "Percent" segment is highlighted with the accent color.
        ''' </summary>
        <TestMethod>
        Public Sub TargetModeButtons_HighlightSelectedSegment()
            Dim palette As ThemePalette = Me.form.CurrentPalette

            Assert.AreEqual(palette.AccentTextColor, Me.form.RadioButtonTargetPriceFixed.ForeColor)
            Assert.AreEqual(palette.TextColor, Me.form.RadioButtonTargetPricePercent.ForeColor)

            Me.form.RadioButtonTargetPricePercent.Checked = True

            Assert.AreEqual(palette.TextColor, Me.form.RadioButtonTargetPriceFixed.ForeColor)
            Assert.AreEqual(palette.AccentTextColor, Me.form.RadioButtonTargetPricePercent.ForeColor)
            Assert.AreEqual(palette.AccentColor, Me.form.RadioButtonTargetPricePercent.FlatAppearance.CheckedBackColor)
        End Sub

        ''' <summary>
        ''' Verifies that prices are formatted with thousands separators and decimals when the text box loses the focus,
        ''' without changing the calculated result.
        ''' </summary>
        <TestMethod>
        <DataRow("42000.5", "42,000.50")>
        <DataRow("1000,5", "1,000.50")>
        <DataRow("1.000,5", "1,000.50")>
        <DataRow("1,000", "1,000.00")>
        <DataRow("0.00001234", "0.00001234")>
        Public Sub FormatPriceText_FormatsValidPrices(text As String, expected As String)
            Me.EnterFixedInput(100D, "100", text, 1D)
            Dim resultBefore As String = Me.form.LabelProfitValue.Text

            Me.form.FormatPriceText(Me.form.TextBoxTargetPrice)

            Assert.AreEqual(expected, Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual(resultBefore, Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that empty or invalid prices are not modified when formatted.
        ''' </summary>
        <TestMethod>
        <DataRow("")>
        <DataRow("abc")>
        <DataRow("1,2,3")>
        Public Sub FormatPriceText_LeavesInvalidTextUnchanged(text As String)
            Me.form.TextBoxCurrentPrice.Text = text

            Me.form.FormatPriceText(Me.form.TextBoxCurrentPrice)

            Assert.AreEqual(text, Me.form.TextBoxCurrentPrice.Text)
        End Sub

        ''' <summary>
        ''' Verifies that the invested amount accepts cents.
        ''' </summary>
        <TestMethod>
        Public Sub InvestedMoney_AcceptsDecimals()
            Assert.AreEqual(2, Me.form.NumericUpDownInvestedMoney.DecimalPlaces)

            Me.EnterFixedInput(1000.5D, "100", "150", 1D)

            Assert.AreEqual("$500.25  (+50%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that the window is captured from the screen with its visible bounds 
        ''' (including the title bar rendered by DWM), and that the client area has the theme background.
        ''' </summary>
        <TestMethod>
        Public Sub CaptureFromScreen_CapturesVisibleWindow()
            Me.form.Location = New Point(100, 100)
            Me.form.TopMost = True
            Me.form.Activate()
            Me.form.Refresh()
            Application.DoEvents()

            Dim visibleBounds As Rectangle = WindowCapture.GetVisibleBounds(Me.form.Handle)
            Using screenshot As Bitmap = WindowCapture.CaptureFromScreen(Me.form.Handle)
                Assert.IsTrue(screenshot.Width > 0 AndAlso screenshot.Height > 0)
                Assert.IsTrue(screenshot.Width <= visibleBounds.Width + 1 AndAlso screenshot.Height <= visibleBounds.Height + 1,
                              $"Screenshot {screenshot.Size} larger than visible bounds {visibleBounds.Size}.")
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that switching between the dark and light themes while the window is open 
        ''' updates the colors of the window, the controls and the current result.
        ''' </summary>
        <TestMethod>
        Public Sub ApplyThemeMode_SwitchesThemeWhileOpen()
            Dim original As ThemePalette = Me.form.CurrentPalette
            Me.EnterFixedInput(100D, "100", "150", 1D)

            For Each isDark As Boolean In New Boolean() {Not original.IsDark, original.IsDark}
                Me.form.ApplyThemeMode(isDark)
                Dim expected As ThemePalette = ThemePalette.FromDarkMode(isDark)

                Assert.AreSame(expected, Me.form.CurrentPalette)
                Assert.AreEqual(expected.WindowBackColor, Me.form.BackColor)
                Assert.AreEqual(expected.InputBackColor, Me.form.TextBoxTargetPrice.BackColor)
                Assert.AreEqual(expected.InputBackColor, Me.form.NumericUpDownLeverage.BackColor)
                Assert.AreEqual(expected.CardBackColor, Me.form.PanelResult.BackColor)
                Assert.AreEqual(expected.ProfitColor, Me.form.LabelProfitValue.ForeColor)
                Assert.AreEqual(expected.AccentTextColor, Me.form.RadioButtonTargetPriceFixed.ForeColor)
            Next
        End Sub

        ''' <summary>
        ''' Verifies that the window processes a real WM_SETTINGCHANGE "ImmersiveColorSet" message, 
        ''' keeping the theme that corresponds to the current Windows setting.
        ''' </summary>
        <TestMethod>
        Public Sub WndProc_ColorModeChangeMessage_FollowsWindowsSetting()
            ' Starts from the opposite theme, so the message must restore the theme of the Windows setting.
            Dim systemIsDark As Boolean = WindowsTheme.IsAppDarkModeEnabled()
            Me.form.ApplyThemeMode(Not systemIsDark)

            Dim lParam As IntPtr = Runtime.InteropServices.Marshal.StringToHGlobalUni(WindowsTheme.ImmersiveColorSetSectionName)
            Try
                NativeMethods.SendMessage(Me.form.Handle, WindowsTheme.WM_SETTINGCHANGE, IntPtr.Zero, lParam)
            Finally
                Runtime.InteropServices.Marshal.FreeHGlobal(lParam)
            End Try

            Assert.AreSame(ThemePalette.FromDarkMode(systemIsDark), Me.form.CurrentPalette)
        End Sub

        ''' <summary>
        ''' Gets the internal edit box of a <see cref="NumericUpDown"/>, 
        ''' used to simulate the user typing without the validation performed by the Text setter.
        ''' </summary>
        Private Shared Function GetEditBox(numeric As NumericUpDown) As TextBox
            For Each child As Control In numeric.Controls
                If TypeOf child Is TextBox Then
                    Return DirectCast(child, TextBox)
                End If
            Next
            Throw New InvalidOperationException("The edit box of the NumericUpDown was not found.")
        End Function

        ''' <summary>
        ''' Verifies that typing a decimal amount does not reformat the text being typed 
        ''' (which moved the caret to the beginning), while the result is updated live.
        ''' </summary>
        <TestMethod>
        <DataRow("1", "$0.50  (+50%)")>
        <DataRow("1000", "$500.00  (+50%)")>
        <DataRow("1000,", "$500.00  (+50%)")>
        <DataRow("1000,5", "$500.25  (+50%)")>
        <DataRow("1000.5", "$500.25  (+50%)")>
        <DataRow("1,000.5", "$500.25  (+50%)")>
        Public Sub TypingInvestedMoney_DoesNotReformatText(typedText As String, expectedResult As String)
            Me.EnterFixedInput(0D, "100", "150", 1D)
            Dim editBox As TextBox = GetEditBox(Me.form.NumericUpDownInvestedMoney)

            editBox.Text = typedText

            Assert.AreEqual(typedText, editBox.Text)
            Assert.AreEqual(expectedResult, Me.form.LabelProfitValue.Text)
            Assert.IsFalse(Me.form.FrameInvestedMoney.HasError)
        End Sub

        ''' <summary>
        ''' Verifies that typing a decimal percentage does not reformat the text being typed, 
        ''' while the target price and the result are updated live.
        ''' </summary>
        <TestMethod>
        <DataRow("12", "224.00", "$12.00  (+12%)")>
        <DataRow("12,", "224.00", "$12.00  (+12%)")>
        <DataRow("12,5", "225.00", "$12.50  (+12.5%)")>
        <DataRow("12.5", "225.00", "$12.50  (+12.5%)")>
        Public Sub TypingPercent_DoesNotReformatText(typedText As String, expectedTargetPrice As String, expectedResult As String)
            Me.form.RadioButtonTargetPricePercent.Checked = True
            Me.form.NumericUpDownInvestedMoney.Value = 100D
            Me.form.TextBoxCurrentPrice.Text = "200"
            Dim editBox As TextBox = GetEditBox(Me.form.NumericUpDownTargetPrice)

            editBox.Text = typedText

            Assert.AreEqual(typedText, editBox.Text)
            Assert.AreEqual(expectedTargetPrice, Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual(expectedResult, Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that typing a leverage does not reformat the text being typed.
        ''' </summary>
        <TestMethod>
        Public Sub TypingLeverage_UpdatesResultLive()
            Me.EnterFixedInput(100D, "100", "110", 1D)
            Dim editBox As TextBox = GetEditBox(Me.form.NumericUpDownLeverage)

            editBox.Text = "20"

            Assert.AreEqual("20", editBox.Text)
            Assert.AreEqual("$200.00  (+10%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that invalid prices are reported: the field border and the result show the error, 
        ''' and the screenshot is disabled until the price is fixed.
        ''' </summary>
        <TestMethod>
        <DataRow("3.00.33.3.88")>
        <DataRow("1,2,3")>
        <DataRow("12abc")>
        Public Sub InvalidEntryPrice_IsReported(invalidText As String)
            Me.EnterFixedInput(100D, "100", "150", 1D)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)

            Me.form.TextBoxCurrentPrice.Text = invalidText

            Assert.IsTrue(Me.form.FrameCurrentPrice.HasError)
            Assert.AreEqual("Invalid entry price", Me.form.LabelProfitValue.Text)
            Assert.AreEqual(Me.form.CurrentPalette.ErrorColor, Me.form.LabelProfitValue.ForeColor)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)

            Me.form.TextBoxCurrentPrice.Text = "100"

            Assert.IsFalse(Me.form.FrameCurrentPrice.HasError)
            Assert.AreEqual("$50.00  (+50%)", Me.form.LabelProfitValue.Text)
            Assert.IsTrue(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies that an invalid target price is reported.
        ''' </summary>
        <TestMethod>
        Public Sub InvalidTargetPrice_IsReported()
            Me.EnterFixedInput(100D, "100", "1.2.3", 1D)

            Assert.IsTrue(Me.form.FrameTargetPrice.HasError)
            Assert.AreEqual("Invalid target price", Me.form.LabelProfitValue.Text)
            Assert.IsFalse(Me.form.ButtonMakeScreenshot.Enabled)
        End Sub

        ''' <summary>
        ''' Verifies that an invalid typed amount is reported.
        ''' </summary>
        <TestMethod>
        Public Sub InvalidInvestedMoney_IsReported()
            Me.EnterFixedInput(100D, "100", "150", 1D)

            GetEditBox(Me.form.NumericUpDownInvestedMoney).Text = "1,2,3"

            Assert.IsTrue(Me.form.FrameInvestedMoney.HasError)
            Assert.AreEqual("Invalid amount invested", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that empty fields are not reported as errors (the result is just not available).
        ''' </summary>
        <TestMethod>
        Public Sub EmptyFields_AreNotErrors()
            Me.EnterFixedInput(100D, "100", "150", 1D)

            Me.form.TextBoxCurrentPrice.Text = String.Empty
            GetEditBox(Me.form.NumericUpDownInvestedMoney).Text = String.Empty

            Assert.IsFalse(Me.form.FrameCurrentPrice.HasError)
            Assert.IsFalse(Me.form.FrameInvestedMoney.HasError)
            Assert.IsNull(Me.form.GetInputErrorMessage())
            Assert.AreEqual("N/A", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies the characters that can be typed in the price text boxes.
        ''' </summary>
        <TestMethod>
        <DataRow("0"c, True)>
        <DataRow("9"c, True)>
        <DataRow("."c, True)>
        <DataRow(","c, True)>
        <DataRow(ChrW(8), True, DisplayName:="Backspace")>
        <DataRow(ChrW(22), True, DisplayName:="Ctrl+V")>
        <DataRow("a"c, False)>
        <DataRow("-"c, False)>
        <DataRow(" "c, False)>
        <DataRow("$"c, False)>
        <DataRow(ChrW(&H663), False, DisplayName:="Arabic-Indic digit")>
        Public Sub IsAllowedPriceChar_ReturnsExpected(c As Char, expected As Boolean)
            Assert.AreEqual(expected, Global.Form1.IsAllowedPriceChar(c))
        End Sub

        ''' <summary>
        ''' Verifies that switching the decimal separator reformats the prices, the numeric fields and the result,
        ''' keeping their values.
        ''' </summary>
        <TestMethod>
        Public Sub ApplyDisplayFormat_DecimalSeparator_ReformatsValues()
            Me.EnterFixedInput(1000.5D, "1344", "1500", 1D)
            Dim comma As DisplayFormat = DisplayFormat.Default.WithDecimalSeparator(DecimalSeparatorKind.Comma)

            Me.form.ApplyDisplayFormat(comma, persist:=False)

            Assert.AreEqual("1.344,00", Me.form.TextBoxCurrentPrice.Text)
            Assert.AreEqual("1.500,00", Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual("1.000,50", Me.form.NumericUpDownInvestedMoney.Text)
            Assert.AreEqual(1000.5D, Me.form.NumericUpDownInvestedMoney.Value)
            Assert.AreEqual("$116,13  (+11,61%)", Me.form.LabelProfitValue.Text)
            Assert.AreEqual("1.000,00", Me.form.ButtonDecimalSeparator.Text)
            Assert.AreEqual("0,00", Me.form.TextBoxCurrentPrice.Hint)

            Me.form.ApplyDisplayFormat(DisplayFormat.Default, persist:=False)

            Assert.AreEqual("1,344.00", Me.form.TextBoxCurrentPrice.Text)
            Assert.AreEqual("1,500.00", Me.form.TextBoxTargetPrice.Text)
            Assert.AreEqual("1,000.50", Me.form.NumericUpDownInvestedMoney.Text)
            Assert.AreEqual("$116.13  (+11.61%)", Me.form.LabelProfitValue.Text)
            Assert.AreEqual("1,000.00", Me.form.ButtonDecimalSeparator.Text)
        End Sub

        ''' <summary>
        ''' Verifies that switching the currency updates the caption, the button and the result.
        ''' </summary>
        <TestMethod>
        Public Sub ApplyDisplayFormat_Currency_UpdatesCaptionAndResult()
            Me.EnterFixedInput(100D, "100", "150", 1D)

            Me.form.ApplyDisplayFormat(New DisplayFormat(CurrencyKind.Euro, DecimalSeparatorKind.Comma), persist:=False)

            Assert.AreEqual("Amount invested (€)", Me.form.LabelInvestedMoney.Text)
            Assert.AreEqual("€", Me.form.ButtonCurrency.Text)
            Assert.AreEqual("50,00 €  (+50%)", Me.form.LabelProfitValue.Text)

            Me.form.ApplyDisplayFormat(DisplayFormat.Default, persist:=False)

            Assert.AreEqual("Amount invested ($)", Me.form.LabelInvestedMoney.Text)
            Assert.AreEqual("$", Me.form.ButtonCurrency.Text)
            Assert.AreEqual("$50.00  (+50%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that, with the comma as decimal separator, ambiguous prices are read the European way.
        ''' </summary>
        <TestMethod>
        Public Sub CommaDecimalSeparator_ParsesAmbiguousPrices()
            Me.form.ApplyDisplayFormat(DisplayFormat.Default.WithDecimalSeparator(DecimalSeparatorKind.Comma), persist:=False)

            ' "1.500" is 1500 and "1,5" is 1.5: the price is multiplied by 1000.
            Me.EnterFixedInput(100D, "1,5", "1.500", 1D)

            Assert.AreEqual("$99.900,00  (+99900%)", Me.form.LabelProfitValue.Text)
        End Sub

        ''' <summary>
        ''' Verifies that the currency and decimal separator buttons toggle the format and save it in the settings.
        ''' </summary>
        <TestMethod>
        Public Sub DisplayFormatButtons_ToggleAndPersist()
            Dim originalSettings As DisplayFormat = Global.Form1.LoadDisplayFormatFromSettings()
            Try
                Me.form.ButtonCurrency.PerformClick()
                Assert.AreEqual(CurrencyKind.Euro, Me.form.CurrentDisplayFormat.Currency)
                Assert.AreEqual(Me.form.CurrentDisplayFormat, Global.Form1.LoadDisplayFormatFromSettings())

                Me.form.ButtonDecimalSeparator.PerformClick()
                Assert.AreEqual(DecimalSeparatorKind.Comma, Me.form.CurrentDisplayFormat.DecimalSeparator)
                Assert.AreEqual(Me.form.CurrentDisplayFormat, Global.Form1.LoadDisplayFormatFromSettings())

                Me.form.ButtonCurrency.PerformClick()
                Me.form.ButtonDecimalSeparator.PerformClick()
                Assert.AreEqual(DisplayFormat.Default, Me.form.CurrentDisplayFormat)
            Finally
                Global.Form1.SaveDisplayFormatToSettings(originalSettings)
            End Try
        End Sub

        ''' <summary>
        ''' Verifies the parsing of the display format saved in the settings.
        ''' </summary>
        <TestMethod>
        <DataRow("UsDollar", "Dot", CurrencyKind.UsDollar, DecimalSeparatorKind.Dot)>
        <DataRow("Euro", "Comma", CurrencyKind.Euro, DecimalSeparatorKind.Comma)>
        <DataRow("Euro", "Dot", CurrencyKind.Euro, DecimalSeparatorKind.Dot)>
        Public Sub ParseDisplayFormat_ValidNames(currencyName As String, separatorName As String, currency As CurrencyKind, separator As DecimalSeparatorKind)
            Assert.AreEqual(New DisplayFormat(currency, separator), Global.Form1.ParseDisplayFormat(currencyName, separatorName))
        End Sub

        ''' <summary>
        ''' Verifies that invalid saved values throw <see cref="InvalidOperationException"/>.
        ''' </summary>
        <TestMethod>
        <DataRow("euro", "Dot", DisplayName:="Different casing")>
        <DataRow("1", "Dot", DisplayName:="Numeric value")>
        <DataRow("Yen", "Dot", DisplayName:="Unknown currency")>
        <DataRow("", "Dot", DisplayName:="Empty currency")>
        <DataRow("Euro", "Semicolon", DisplayName:="Unknown separator")>
        <DataRow("Euro", "", DisplayName:="Empty separator")>
        Public Sub ParseDisplayFormat_InvalidNames_Throw(currencyName As String, separatorName As String)
            Assert.ThrowsExactly(Of InvalidOperationException)(Function() Global.Form1.ParseDisplayFormat(currencyName, separatorName))
        End Sub

        ''' <summary>
        ''' Verifies the conversion of price texts between display formats.
        ''' </summary>
        <TestMethod>
        <DataRow("1,344.00", "1.344,00")>
        <DataRow("1344", "1.344,00")>
        <DataRow("", "")>
        <DataRow("abc", "abc")>
        Public Sub ConvertPriceText_ConvertsBetweenFormats(text As String, expected As String)
            Dim comma As DisplayFormat = DisplayFormat.Default.WithDecimalSeparator(DecimalSeparatorKind.Comma)
            Assert.AreEqual(expected, Global.Form1.ConvertPriceText(text, DisplayFormat.Default, comma))
        End Sub

        ''' <summary>
        ''' Verifies that the GitHub button has a scaled image that is replaced when the theme changes, 
        ''' and that it points to the repository.
        ''' </summary>
        <TestMethod>
        Public Sub GitHubButton_HasThemedImage()
            Dim image As Image = Me.form.ButtonGitHub.Image
            Assert.IsNotNull(image)
            Assert.IsTrue(image.Width <= Me.form.ButtonGitHub.ClientSize.Height AndAlso image.Width = image.Height)
            Assert.AreEqual("View source code on GitHub", Me.form.ToolTipMain.GetToolTip(Me.form.ButtonGitHub))
            StringAssert.StartsWith(Global.Form1.RepositoryUrl, "https://github.com/ElektroStudios/")

            Dim isDark As Boolean = Me.form.CurrentPalette.IsDark
            Me.form.ApplyThemeMode(Not isDark)
            Assert.AreNotSame(image, Me.form.ButtonGitHub.Image)
            Me.form.ApplyThemeMode(isDark)
        End Sub

        ''' <summary>
        ''' Verifies that images are scaled to a square, keeping their aspect ratio and centered.
        ''' </summary>
        <TestMethod>
        Public Sub ScaleImage_FitsInSquare()
            Using source As New Bitmap(294, 288), g As Graphics = Graphics.FromImage(source)
                g.Clear(Color.Red)
                Using scaled As Bitmap = Global.Form1.ScaleImage(source, 20)
                    Assert.AreEqual(New Size(20, 20), scaled.Size)
                    Assert.AreEqual(Color.Red.ToArgb(), scaled.GetPixel(10, 10).ToArgb())
                End Using
            End Using
            Using source As New Bitmap(10, 10)
                Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() Global.Form1.ScaleImage(source, 0))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the input frames stretch and vertically center the hosted controls.
        ''' </summary>
        <TestMethod>
        Public Sub InputFrames_LayOutHostedControls()
            Dim frame As InputFrame = Me.form.FrameCurrentPrice
            Dim textBox As CustomTextBox = Me.form.TextBoxCurrentPrice

            Assert.AreSame(frame, textBox.Parent)
            Assert.AreEqual(frame.ClientSize.Width - frame.Padding.Horizontal, textBox.Width)
            Assert.IsTrue(textBox.Top >= 0 AndAlso textBox.Bottom <= frame.ClientSize.Height)

            For Each numeric As ThemedNumericUpDown In New ThemedNumericUpDown() {Me.form.NumericUpDownInvestedMoney, Me.form.NumericUpDownLeverage, Me.form.NumericUpDownTargetPrice}
                Assert.IsInstanceOfType(Of InputFrame)(numeric.Parent)
                Assert.IsTrue(numeric.Top >= 0 AndAlso numeric.Bottom <= numeric.Parent.ClientSize.Height, numeric.Name)
            Next
        End Sub

#End Region

#Region " NativeMethods "

        ''' <summary>
        ''' Platform invocation methods.
        ''' </summary>
        Private NotInheritable Class NativeMethods

            ''' <summary>
            ''' Prevents a default instance of the <see cref="NativeMethods"/> class from being created.
            ''' </summary>
            Private Sub New()
            End Sub

            ''' <summary>
            ''' Sends the specified message to a window and waits until the window procedure processes it.
            ''' </summary>
            <Runtime.InteropServices.DllImport("user32.dll", CharSet:=Runtime.InteropServices.CharSet.Unicode, EntryPoint:="SendMessageW", ExactSpelling:=True)>
            Friend Shared Function SendMessage(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
            End Function

        End Class

#End Region

    End Class

End Namespace

#End Region
