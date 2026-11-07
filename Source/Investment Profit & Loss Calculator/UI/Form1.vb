
Imports System.Diagnostics
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.Runtime.InteropServices

Imports FormExtensions

Imports Core

Imports Theming

Public Class Form1

    Private invested As Double
    Private currentPrice As Double
    Private targetPrice As Double
    Private leverage As Double = 1

#Region " Theming "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The palette of the theme currently applied to the window, which follows the Windows app mode (dark/light).
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private currentPaletteField As ThemePalette = ThemePalette.FromDarkMode(WindowsTheme.IsAppDarkModeEnabled())

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the palette of the theme currently applied to the window.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend ReadOnly Property CurrentPalette As ThemePalette
        Get
            Return Me.currentPaletteField
        End Get
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new instance of the <see cref="Form1"/> class.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public Sub New()
        Me.InitializeComponent()
        Me.ApplyTheme(Me.currentPaletteField)
        Me.ApplyDisplayFormat(Form1.LoadDisplayFormatFromSettings(), persist:=False)
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="Control.HandleCreated"/> event.
    ''' <para></para>
    ''' The title bar dark mode is applied here (and not in the Load event), 
    ''' because the DWM attribute only takes effect when it is set before the window is shown.
    ''' This method also runs again if the window handle is recreated.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)

        If Me.currentPaletteField.IsDark Then
            WindowsTheme.SetImmersiveDarkMode(Me.Handle, enabled:=True)
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Processes Windows messages.
    ''' <para></para>
    ''' When the user switches between the light and dark modes in the Windows settings, Windows broadcasts
    ''' WM_SETTINGCHANGE with the "ImmersiveColorSet" parameter to all top-level windows, 
    ''' and the theme of the window is updated immediately.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)

        If WindowsTheme.IsColorModeChangeMessage(m.Msg, m.LParam) Then
            Me.ApplyThemeMode(WindowsTheme.IsAppDarkModeEnabled())
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Applies the dark or light theme to the window and its title bar, if it is not already applied.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="isDarkMode">
    ''' <see langword="True"/> to apply the dark theme; <see langword="False"/> to apply the light theme.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Sub ApplyThemeMode(isDarkMode As Boolean)

        If isDarkMode = Me.currentPaletteField.IsDark Then
            Return
        End If

        Me.currentPaletteField = ThemePalette.FromDarkMode(isDarkMode)
        Me.ApplyTheme(Me.currentPaletteField)
        Me.CalculatePL() ' Updates the color of the result.

        If Me.IsHandleCreated AndAlso WindowsTheme.SetImmersiveDarkMode(Me.Handle, isDarkMode) Then
            WindowsTheme.RedrawNonClientArea(Me.Handle, isActive:=Form.ActiveForm Is Me)
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Applies the colors of the specified palette to the window and its controls.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="palette">
    ''' The palette to apply.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ApplyTheme(palette As ThemePalette)

        If palette Is Nothing Then
            Throw New ArgumentNullException(NameOf(palette))
        End If

        Me.SuspendLayout()

        Me.BackColor = palette.WindowBackColor
        Me.ForeColor = palette.TextColor

        For Each caption As Label In New Label() {Me.LabelInvestedMoney, Me.LabelLeverage, Me.LabelCurrentPrice, Me.LabelTargetPrice}
            caption.ForeColor = palette.SecondaryTextColor
        Next

        For Each frame As InputFrame In New InputFrame() {Me.FrameInvestedMoney, Me.FrameLeverage, Me.FrameCurrentPrice, Me.FrameTargetPrice, Me.FrameTargetPercent}
            frame.BackColor = palette.InputBackColor
            frame.BorderColor = palette.InputBorderColor
            frame.FocusedBorderColor = palette.FocusBorderColor
            frame.DisabledBorderColor = palette.InputDisabledBorderColor
            frame.ErrorBorderColor = palette.ErrorColor
        Next

        For Each textBox As CustomTextBox In New CustomTextBox() {Me.TextBoxCurrentPrice, Me.TextBoxTargetPrice}
            textBox.BackColor = palette.InputBackColor
            textBox.ForeColor = palette.TextColor
            textBox.HintForeColor = palette.DisabledTextColor
        Next

        For Each numeric As ThemedNumericUpDown In New ThemedNumericUpDown() {Me.NumericUpDownInvestedMoney, Me.NumericUpDownLeverage, Me.NumericUpDownTargetPrice}
            numeric.BackColor = palette.InputBackColor
            numeric.ForeColor = palette.TextColor
            numeric.ButtonBackColor = palette.InputBackColor
            numeric.ButtonHoverBackColor = palette.ButtonHoverBackColor
            numeric.ButtonPressedBackColor = palette.ButtonPressedBackColor
            numeric.ArrowColor = palette.SecondaryTextColor
            numeric.DisabledArrowColor = palette.DisabledTextColor
        Next

        Me.PanelResult.BackColor = palette.CardBackColor
        Me.PanelResult.BorderColor = palette.CardBorderColor
        Me.LabelProfitCaption.ForeColor = palette.SecondaryTextColor

        For Each button As ButtonBase In New ButtonBase() {Me.ButtonClear, Me.ButtonMakeScreenshot, Me.RadioButtonTargetPriceFixed, Me.RadioButtonTargetPricePercent, Me.ButtonCurrency, Me.ButtonDecimalSeparator, Me.ButtonGitHub}
            Form1.ApplyFlatButtonColors(button, palette)
        Next
        Me.UpdateTargetModeButtonColors()
        Me.UpdateGitHubButtonImage(palette)

        Me.CheckBoxAlwaysOnTop.BackColor = palette.WindowBackColor
        Me.CheckBoxAlwaysOnTop.ForeColor = palette.TextColor
        Me.CheckBoxAlwaysOnTop.FlatAppearance.CheckedBackColor = palette.InputBackColor
        Me.CheckBoxAlwaysOnTop.FlatAppearance.MouseOverBackColor = palette.WindowBackColor
        Me.CheckBoxAlwaysOnTop.FlatAppearance.MouseDownBackColor = palette.WindowBackColor
        Me.UpdateAlwaysOnTopCheckBoxColors()

        Me.ResumeLayout()
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Applies the colors of the specified palette to a flat button.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Shared Sub ApplyFlatButtonColors(button As ButtonBase, palette As ThemePalette)
        button.UseVisualStyleBackColor = False
        button.FlatStyle = FlatStyle.Flat
        button.BackColor = palette.ButtonBackColor
        button.ForeColor = palette.TextColor
        button.FlatAppearance.BorderSize = 1
        button.FlatAppearance.BorderColor = palette.ButtonBorderColor
        button.FlatAppearance.MouseOverBackColor = palette.ButtonHoverBackColor
        button.FlatAppearance.MouseDownBackColor = palette.ButtonPressedBackColor
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Updates the colors of the "Price" / "Percent" segmented buttons, highlighting the selected one.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub UpdateTargetModeButtonColors()
        Dim palette As ThemePalette = Me.currentPaletteField

        For Each segment As RadioButton In New RadioButton() {Me.RadioButtonTargetPriceFixed, Me.RadioButtonTargetPricePercent}
            segment.FlatAppearance.CheckedBackColor = palette.AccentColor
            If segment.Checked Then
                segment.ForeColor = palette.AccentTextColor
                segment.FlatAppearance.BorderColor = palette.AccentColor
                segment.FlatAppearance.MouseOverBackColor = palette.AccentColor
                segment.FlatAppearance.MouseDownBackColor = palette.AccentColor
            Else
                segment.ForeColor = palette.TextColor
                segment.FlatAppearance.BorderColor = palette.ButtonBorderColor
                segment.FlatAppearance.MouseOverBackColor = palette.ButtonHoverBackColor
                segment.FlatAppearance.MouseDownBackColor = palette.ButtonPressedBackColor
            End If
        Next
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Updates the border color of the "Always on top" check box, highlighting it when checked.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub UpdateAlwaysOnTopCheckBoxColors()
        Me.CheckBoxAlwaysOnTop.FlatAppearance.BorderColor =
            If(Me.CheckBoxAlwaysOnTop.Checked, Me.currentPaletteField.FocusBorderColor, Me.currentPaletteField.InputBorderColor)
    End Sub

#End Region

#Region " Display Format "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The URL of the source code repository of the application.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Const RepositoryUrl As String = "https://github.com/ElektroStudios/Investment-Profit-Loss-Calculator"

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The display format (currency and decimal separator) currently applied to the window.
    ''' <para></para>
    ''' It is initialized with the default format, because <see cref="Form1.InitializeComponent"/> already raises
    ''' events that format values; the saved format is applied right after it, in the constructor.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private displayFormatField As DisplayFormat = DisplayFormat.Default

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the display format (currency and decimal separator) currently applied to the window.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend ReadOnly Property CurrentDisplayFormat As DisplayFormat
        Get
            Return Me.displayFormatField
        End Get
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Applies the specified display format (currency and decimal separator) to the window:
    ''' the displayed prices, amounts and numeric controls are reformatted, keeping their values.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="format">
    ''' The display format to apply.
    ''' </param>
    ''' 
    ''' <param name="persist">
    ''' <see langword="True"/> to save the format in the user settings, so it is restored the next time.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="format"/> is <see langword="Nothing"/>.
    ''' </exception>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Sub ApplyDisplayFormat(format As DisplayFormat, persist As Boolean)

        If format Is Nothing Then
            Throw New ArgumentNullException(NameOf(format))
        End If

        ' The prices are parsed with the previous format, before switching to the new one.
        Dim previousFormat As DisplayFormat = Me.displayFormatField
        Dim currentPriceText As String = Form1.ConvertPriceText(Me.TextBoxCurrentPrice.Text, previousFormat, format)
        Dim targetPriceText As String = Form1.ConvertPriceText(Me.TextBoxTargetPrice.Text, previousFormat, format)

        Me.displayFormatField = format

        ' The numeric up/down controls display and parse numbers using the culture of the thread.
        Application.CurrentCulture = format.Culture
        For Each numeric As ThemedNumericUpDown In New ThemedNumericUpDown() {Me.NumericUpDownInvestedMoney, Me.NumericUpDownLeverage, Me.NumericUpDownTargetPrice}
            numeric.RefreshText()
        Next

        Me.TextBoxCurrentPrice.Text = currentPriceText
        If Me.TextBoxTargetPrice.Enabled Then
            ' In "Percent" mode the target price was already regenerated from the entry price.
            Me.TextBoxTargetPrice.Text = targetPriceText
        End If

        Dim zeroHint As String = ProfitLossCalculator.FormatPrice(0, format)
        Me.TextBoxCurrentPrice.Hint = zeroHint
        Me.TextBoxTargetPrice.Hint = zeroHint

        Me.LabelInvestedMoney.Text = $"Amount invested ({format.CurrencySymbol})"
        Me.UpdateDisplayFormatButtons()
        Me.CalculatePL()

        If persist Then
            Form1.SaveDisplayFormatToSettings(format)
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Converts a price text from a display format to another (e.g. "1,344.00" to "1.344,00").
    ''' <para></para>
    ''' Empty or invalid text is returned unchanged.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Shared Function ConvertPriceText(text As String, sourceFormat As DisplayFormat, targetFormat As DisplayFormat) As String
        Dim price As Double
        If ProfitLossCalculator.TryParsePrice(text, sourceFormat, price) Then
            Return ProfitLossCalculator.FormatPrice(price, targetFormat)
        End If
        Return text
    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Updates the text and tooltips of the currency and decimal separator buttons.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub UpdateDisplayFormatButtons()
        Dim format As DisplayFormat = Me.displayFormatField

        Me.ButtonCurrency.Text = format.CurrencySymbol
        Me.ToolTipMain.SetToolTip(Me.ButtonCurrency,
            If(format.Currency = CurrencyKind.Euro,
               "Currency: euro (€). Click to switch to US dollar ($).",
               "Currency: US dollar ($). Click to switch to euro (€)."))

        Me.ButtonDecimalSeparator.Text = format.FormatNumber(1000, "#,##0.00")
        Me.ToolTipMain.SetToolTip(Me.ButtonDecimalSeparator,
            If(format.DecimalSeparator = DecimalSeparatorKind.Comma,
               "Decimal separator: comma (1.000,00). Click to switch to dot (1,000.00).",
               "Decimal separator: dot (1,000.00). Click to switch to comma (1.000,00)."))
    End Sub

    Private Sub ButtonCurrency_Click(sender As Object, e As EventArgs) Handles ButtonCurrency.Click
        Dim currency As CurrencyKind = If(Me.displayFormatField.Currency = CurrencyKind.Euro, CurrencyKind.UsDollar, CurrencyKind.Euro)
        Me.ApplyDisplayFormat(Me.displayFormatField.WithCurrency(currency), persist:=True)
    End Sub

    Private Sub ButtonDecimalSeparator_Click(sender As Object, e As EventArgs) Handles ButtonDecimalSeparator.Click
        Dim separator As DecimalSeparatorKind = If(Me.displayFormatField.DecimalSeparator = DecimalSeparatorKind.Comma, DecimalSeparatorKind.Dot, DecimalSeparatorKind.Comma)
        Me.ApplyDisplayFormat(Me.displayFormatField.WithDecimalSeparator(separator), persist:=True)
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Loads the display format saved in the user settings.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <exception cref="InvalidOperationException">
    ''' Thrown when the saved settings contain an invalid value.
    ''' </exception>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Shared Function LoadDisplayFormatFromSettings() As DisplayFormat
        Return Form1.ParseDisplayFormat(My.Settings.Currency, My.Settings.DecimalSeparator)
    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Saves the specified display format in the user settings.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Shared Sub SaveDisplayFormatToSettings(format As DisplayFormat)
        If format Is Nothing Then
            Throw New ArgumentNullException(NameOf(format))
        End If

        My.Settings.Currency = format.Currency.ToString()
        My.Settings.DecimalSeparator = format.DecimalSeparator.ToString()
        My.Settings.Save()
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Parses a display format from the names of its currency and decimal separator, as saved in the settings.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="currencyName">
    ''' The name of a <see cref="CurrencyKind"/> value (e.g. "Euro").
    ''' </param>
    ''' 
    ''' <param name="decimalSeparatorName">
    ''' The name of a <see cref="DecimalSeparatorKind"/> value (e.g. "Comma").
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <exception cref="InvalidOperationException">
    ''' Thrown when a name is not a valid value.
    ''' </exception>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Shared Function ParseDisplayFormat(currencyName As String, decimalSeparatorName As String) As DisplayFormat

        Dim currency As CurrencyKind
        If Not [Enum].TryParse(currencyName, ignoreCase:=False, result:=currency) OrElse
           Not [Enum].IsDefined(GetType(CurrencyKind), currency) OrElse
           Not String.Equals(currency.ToString(), currencyName, StringComparison.Ordinal) Then
            Throw New InvalidOperationException($"Invalid currency in the user settings: ""{currencyName}"".")
        End If

        Dim decimalSeparator As DecimalSeparatorKind
        If Not [Enum].TryParse(decimalSeparatorName, ignoreCase:=False, result:=decimalSeparator) OrElse
           Not [Enum].IsDefined(GetType(DecimalSeparatorKind), decimalSeparator) OrElse
           Not String.Equals(decimalSeparator.ToString(), decimalSeparatorName, StringComparison.Ordinal) Then
            Throw New InvalidOperationException($"Invalid decimal separator in the user settings: ""{decimalSeparatorName}"".")
        End If

        Return New DisplayFormat(currency, decimalSeparator)
    End Function

#End Region

#Region " GitHub "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Updates the GitHub button image: the white mark for the dark theme and the black mark for the light theme,
    ''' scaled to the size of the button.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub UpdateGitHubButtonImage(palette As ThemePalette)

        Dim side As Integer = Math.Max(8, CInt(Math.Round(Me.ButtonGitHub.ClientSize.Height * 0.62)))
        Dim scaled As Bitmap

        ' My.Resources returns a new Bitmap on each call, so it is disposed after scaling it.
        Using source As Bitmap = If(palette.IsDark, My.Resources.GitHub_Invertocat_White, My.Resources.GitHub_Invertocat_Black)
            scaled = Form1.ScaleImage(source, side)
        End Using

        Dim previous As Image = Me.ButtonGitHub.Image
        Me.ButtonGitHub.Image = scaled
        previous?.Dispose()
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Scales an image to fit in a square of the specified side, keeping its aspect ratio, with high quality.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Shared Function ScaleImage(source As Image, side As Integer) As Bitmap

        If source Is Nothing Then
            Throw New ArgumentNullException(NameOf(source))
        End If
        If side <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(side), side, "The side must be greater than zero.")
        End If

        Dim ratio As Double = Math.Min(side / source.Width, side / source.Height)
        Dim width As Integer = Math.Max(1, CInt(Math.Round(source.Width * ratio)))
        Dim height As Integer = Math.Max(1, CInt(Math.Round(source.Height * ratio)))

        Dim result As New Bitmap(side, side, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(result)
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            g.DrawImage(source, New Rectangle((side - width) \ 2, (side - height) \ 2, width, height))
        End Using
        Return result
    End Function

    Private Sub ButtonGitHub_Click(sender As Object, e As EventArgs) Handles ButtonGitHub.Click
        ' UseShellExecute is required to open the URL with the default web browser.
        Using Process.Start(New ProcessStartInfo(RepositoryUrl) With {.UseShellExecute = True})
        End Using
    End Sub

    Private Sub Form1_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        ' Button does not dispose its Image, and the scaled images are owned by this window.
        Me.ButtonGitHub.Image?.Dispose()
    End Sub

#End Region

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' The numeric up/down controls display and parse numbers using the culture of the thread.
        Application.CurrentCulture = Me.displayFormatField.Culture
    End Sub

    Private Sub NumericUpDownInvestedMoney_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDownInvestedMoney.TextChanged, NumericUpDownInvestedMoney.ValueChanged
        ' Value is not read here: reading it while typing reformats the text and moves the caret to the beginning.
        Dim value As Decimal
        Dim isValid As Boolean = Me.NumericUpDownInvestedMoney.TryGetEditValue(value)
        Me.invested = CDbl(value)
        Me.FrameInvestedMoney.HasError = Form1.IsInvalidInput(Me.NumericUpDownInvestedMoney.Text, isValid)
        Me.CalculatePL()
    End Sub

    Private Sub NumericUpDownInvestedMoney_LostFocus(sender As Object, e As EventArgs) Handles NumericUpDownInvestedMoney.LostFocus
        Dim num As NumericUpDown = DirectCast(sender, NumericUpDown)
        If String.IsNullOrWhiteSpace(num.Text) Then
            num.Text = num.Minimum
        End If
    End Sub

    Private Sub TextBoxcurrentPrice_TextChanged(sender As Object, e As EventArgs) Handles TextBoxCurrentPrice.TextChanged
        Dim isValid As Boolean = ProfitLossCalculator.TryParsePrice(Me.TextBoxCurrentPrice.Text, Me.displayFormatField, Me.currentPrice)
        Me.FrameCurrentPrice.HasError = Form1.IsInvalidInput(Me.TextBoxCurrentPrice.Text, isValid)

        If Me.RadioButtonTargetPricePercent.Checked Then
            ' In "Percent" mode the percentage is the source of truth: only the target price is recalculated.
            Me.UpdateTargetPriceFromPercent()
        ElseIf Me.targetPrice <> 0 Then
            Me.UpdateNumericUpDownTargetPrice()
        End If
        Me.CalculatePL()
    End Sub

    Private Sub TextBoxtargetPrice_TextChanged(sender As Object, e As EventArgs) Handles TextBoxTargetPrice.TextChanged
        Dim tb As CustomTextBox = DirectCast(sender, TextBox)
        Dim isValid As Boolean = ProfitLossCalculator.TryParsePrice(tb.Text, Me.displayFormatField, Me.targetPrice)
        If Me.targetPrice < 0 Then
            Me.targetPrice = 0
            tb.Text = ProfitLossCalculator.FormatPrice(0, Me.displayFormatField)
            Return ' The assignment above raised this event again with the corrected text.
        End If
        Me.FrameTargetPrice.HasError = Form1.IsInvalidInput(tb.Text, isValid)

        ' In "Percent" mode the target price is calculated from the percentage, so the percentage is not recalculated
        ' from the (rounded) target price.
        If tb.Enabled Then
            Me.UpdateNumericUpDownTargetPrice()
            Me.CalculatePL()
        End If
    End Sub

    Private Sub PriceTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxCurrentPrice.KeyPress, TextBoxTargetPrice.KeyPress
        If Not Form1.IsAllowedPriceChar(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub PriceTextBox_Leave(sender As Object, e As EventArgs) Handles TextBoxCurrentPrice.Leave, TextBoxTargetPrice.Leave
        Me.FormatPriceText(DirectCast(sender, TextBox))
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Determines whether the specified character can be typed in a price text box: 
    ''' digits, the dot and the comma (decimal and thousands separators) and control characters 
    ''' (e.g. backspace, or Ctrl+C / Ctrl+V / Ctrl+X / Ctrl+Z / Ctrl+A).
    ''' <para></para>
    ''' This only prevents obviously wrong characters: a text made of allowed characters can still be invalid 
    ''' (e.g. "3.00.33.3.88"), and a pasted text is not filtered; those cases are reported as input errors.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="c">
    ''' The typed character.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <returns>
    ''' <see langword="True"/> if the character can be typed; otherwise, <see langword="False"/>.
    ''' </returns>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Shared Function IsAllowedPriceChar(c As Char) As Boolean
        Return Char.IsControl(c) OrElse (c >= "0"c AndAlso c <= "9"c) OrElse c = "."c OrElse c = ","c
    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Determines whether the text of an input field must be reported as invalid.
    ''' <para></para>
    ''' An empty field is not an error (the result is just not available).
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="text">
    ''' The text of the input field.
    ''' </param>
    ''' 
    ''' <param name="isParsed">
    ''' <see langword="True"/> if the text was parsed successfully; otherwise, <see langword="False"/>.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <returns>
    ''' <see langword="True"/> if the text is not empty and could not be parsed; otherwise, <see langword="False"/>.
    ''' </returns>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Shared Function IsInvalidInput(text As String, isParsed As Boolean) As Boolean
        Return Not isParsed AndAlso Not String.IsNullOrWhiteSpace(text)
    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Formats the price of the specified text box with thousands separators and decimals (e.g. "1,344.00").
    ''' <para></para>
    ''' Empty or invalid text is left unchanged, so the user can see and fix it.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="textBox">
    ''' The text box that contains the price.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Sub FormatPriceText(textBox As TextBox)

        If textBox Is Nothing Then
            Throw New ArgumentNullException(NameOf(textBox))
        End If

        Dim price As Double
        If ProfitLossCalculator.TryParsePrice(textBox.Text, Me.displayFormatField, price) Then
            Dim formatted As String = ProfitLossCalculator.FormatPrice(price, Me.displayFormatField)
            If Not String.Equals(textBox.Text, formatted, StringComparison.Ordinal) Then
                textBox.Text = formatted
            End If
        End If
    End Sub

    Private Sub NumericUpDownLeverage_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDownLeverage.TextChanged, NumericUpDownLeverage.ValueChanged
        ' Value is not read here: reading it while typing reformats the text and moves the caret to the beginning.
        Dim value As Decimal
        Dim isValid As Boolean = Me.NumericUpDownLeverage.TryGetEditValue(value)
        Me.leverage = CDbl(value)
        Me.FrameLeverage.HasError = Form1.IsInvalidInput(Me.NumericUpDownLeverage.Text, isValid)
        Me.CalculatePL()
    End Sub

    Private Sub NumericUpDownLeverage_LostFocus(sender As Object, e As EventArgs) Handles NumericUpDownLeverage.LostFocus
        Dim num As NumericUpDown = DirectCast(sender, NumericUpDown)
        If String.IsNullOrWhiteSpace(num.Text) Then
            num.Text = num.Minimum
        End If
    End Sub

    Private Sub NumericUpDownTargetPrice_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDownTargetPrice.TextChanged, NumericUpDownTargetPrice.ValueChanged
        If Me.NumericUpDownTargetPrice.Enabled Then
            Me.UpdateTargetPriceFromPercent()
            Me.CalculatePL()
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' In "Percent" mode, calculates the target price from the entry price and the percentage typed by the user.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub UpdateTargetPriceFromPercent()
        ' Value is not read here: reading it while typing reformats the text and moves the caret to the beginning.
        Dim percent As Decimal
        Dim isValid As Boolean = Me.NumericUpDownTargetPrice.TryGetEditValue(percent)
        Me.FrameTargetPercent.HasError = Form1.IsInvalidInput(Me.NumericUpDownTargetPrice.Text, isValid)

        If isValid Then
            Me.TextBoxTargetPrice.Text = ProfitLossCalculator.FormatPrice(ProfitLossCalculator.CalculateTargetPrice(Me.currentPrice, CDbl(percent)), Me.displayFormatField)
        End If
    End Sub

    Private Sub RadioButtonTargetPriceFixed_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonTargetPriceFixed.CheckedChanged
        Me.TextBoxTargetPrice.Enabled = True
        Me.NumericUpDownTargetPrice.Enabled = False
        Me.UpdateTargetModeButtonColors()
        Me.CalculatePL()
    End Sub

    Private Sub RadioButtonTargetPricePercent_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonTargetPricePercent.CheckedChanged
        Me.TextBoxTargetPrice.Enabled = False
        Me.NumericUpDownTargetPrice.Enabled = True
        Me.UpdateTargetModeButtonColors()
        Me.CalculatePL()
    End Sub

    Private Sub ButtonClear_Click(sender As Object, e As EventArgs) Handles ButtonClear.Click
        Me.NumericUpDownInvestedMoney.Value = 0
        Me.TextBoxCurrentPrice.Clear()
        Me.TextBoxTargetPrice.Clear()
        Me.NumericUpDownLeverage.Value = 1
        Me.ButtonMakeScreenshot.Enabled = False
    End Sub

    Private Sub CheckBoxAlwaysOnTop_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAlwaysOnTop.CheckedChanged
        Me.TopMost = DirectCast(sender, CheckBox).Checked
        Me.UpdateAlwaysOnTopCheckBoxColors()
    End Sub

    Private Sub UpdateNumericUpDownTargetPrice()
        Dim change As Double = ProfitLossCalculator.CalculatePriceChangePercent(Me.currentPrice, Me.targetPrice)
        If Double.IsNaN(change) Then
            Me.NumericUpDownTargetPrice.Value = 0
        Else
            Try
                Me.NumericUpDownTargetPrice.Value = change
            Catch ex As Exception
            End Try
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the message that describes the first input field with an invalid value, if any.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <returns>
    ''' The error message, or <see langword="Nothing"/> if all the input fields are valid.
    ''' </returns>
    ''' ----------------------------------------------------------------------------------------------------
    Friend Function GetInputErrorMessage() As String

        If Me.FrameInvestedMoney.HasError Then
            Return "Invalid amount invested"
        End If
        If Me.FrameLeverage.HasError Then
            Return "Invalid leverage"
        End If
        If Me.FrameCurrentPrice.HasError Then
            Return "Invalid entry price"
        End If
        If Me.RadioButtonTargetPriceFixed.Checked AndAlso Me.FrameTargetPrice.HasError Then
            Return "Invalid target price"
        End If
        If Me.RadioButtonTargetPricePercent.Checked AndAlso Me.FrameTargetPercent.HasError Then
            Return "Invalid percentage"
        End If

        Return Nothing
    End Function

    Private Sub CalculatePL()

        Dim errorMessage As String = Me.GetInputErrorMessage()
        If errorMessage IsNot Nothing Then
            Me.LabelProfitValue.Text = errorMessage
            Me.LabelProfitValue.ForeColor = Me.currentPaletteField.ErrorColor
            Me.ButtonMakeScreenshot.Enabled = False
            Return
        End If

        Dim result As ProfitLossResult
        If String.IsNullOrEmpty(Me.TextBoxTargetPrice.Text) OrElse Me.leverage <= 0 Then
            result = ProfitLossResult.NotAvailable
        Else
            result = ProfitLossCalculator.Calculate(Me.invested, Me.currentPrice, Me.targetPrice, Me.leverage)
        End If

        Me.LabelProfitValue.Text = result.ToDisplayText(Me.displayFormatField)

        Select Case result.Kind
            Case ProfitLossKind.Profit
                Me.LabelProfitValue.ForeColor = Me.currentPaletteField.ProfitColor

            Case ProfitLossKind.Loss
                Me.LabelProfitValue.ForeColor = Me.currentPaletteField.LossColor

            Case Else ' NotAvailable, BreakEven
                Me.LabelProfitValue.ForeColor = Me.currentPaletteField.NeutralResultColor

        End Select

        Me.ButtonMakeScreenshot.Enabled = result.IsAvailable

    End Sub

    Private Sub ButtonMakeScreenshot_Click(sender As Object, e As EventArgs) Handles ButtonMakeScreenshot.Click

        ' The window is captured from the screen (with its real, themed title bar) before showing the dialog,
        ' because the dialog would cover the window.
        Me.Update()
        Using screenshot As Bitmap = WindowCapture.CaptureFromScreen(Me.Handle),
              dlg As New SaveFileDialog With {
                .DefaultExt = "jpg",
                .FileName = $"P&L {Date.Now:dd-MMM-yyyy hh꞉mm꞉ss}",
                .Title = "Select where to save the screenshot file...",
                .AddExtension = True,
                .CheckPathExists = True,
                .OverwritePrompt = True,
                .SupportMultiDottedExtensions = True,
                .Filter = "Jpg Files (*.jpg)|*.jpg",
                .RestoreDirectory = True
            }

            If dlg.ShowDialog(Me) = DialogResult.OK Then
                screenshot.Save(dlg.FileName, ImageFormat.Jpeg)
            End If
        End Using

    End Sub

End Class
