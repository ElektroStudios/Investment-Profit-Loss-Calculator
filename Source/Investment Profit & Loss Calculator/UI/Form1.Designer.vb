
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.LabelInvestedMoney = New System.Windows.Forms.Label()
        Me.FrameInvestedMoney = New InputFrame()
        Me.NumericUpDownInvestedMoney = New ThemedNumericUpDown()
        Me.LabelLeverage = New System.Windows.Forms.Label()
        Me.FrameLeverage = New InputFrame()
        Me.NumericUpDownLeverage = New ThemedNumericUpDown()
        Me.LabelCurrentPrice = New System.Windows.Forms.Label()
        Me.FrameCurrentPrice = New InputFrame()
        Me.TextBoxCurrentPrice = New CustomTextBox()
        Me.LabelTargetPrice = New System.Windows.Forms.Label()
        Me.RadioButtonTargetPriceFixed = New System.Windows.Forms.RadioButton()
        Me.RadioButtonTargetPricePercent = New System.Windows.Forms.RadioButton()
        Me.FrameTargetPrice = New InputFrame()
        Me.TextBoxTargetPrice = New CustomTextBox()
        Me.FrameTargetPercent = New InputFrame()
        Me.NumericUpDownTargetPrice = New ThemedNumericUpDown()
        Me.PanelResult = New CardPanel()
        Me.LabelProfitCaption = New System.Windows.Forms.Label()
        Me.LabelProfitValue = New System.Windows.Forms.Label()
        Me.ButtonClear = New System.Windows.Forms.Button()
        Me.ButtonMakeScreenshot = New System.Windows.Forms.Button()
        Me.CheckBoxAlwaysOnTop = New System.Windows.Forms.CheckBox()
        Me.ButtonCurrency = New System.Windows.Forms.Button()
        Me.ButtonDecimalSeparator = New System.Windows.Forms.Button()
        Me.ButtonGitHub = New System.Windows.Forms.Button()
        Me.ToolTipMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.FrameInvestedMoney.SuspendLayout()
        CType(Me.NumericUpDownInvestedMoney, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FrameLeverage.SuspendLayout()
        CType(Me.NumericUpDownLeverage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FrameCurrentPrice.SuspendLayout()
        Me.FrameTargetPrice.SuspendLayout()
        Me.FrameTargetPercent.SuspendLayout()
        CType(Me.NumericUpDownTargetPrice, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelResult.SuspendLayout()
        Me.SuspendLayout()
        '
        'LabelInvestedMoney
        '
        Me.LabelInvestedMoney.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.LabelInvestedMoney.Location = New System.Drawing.Point(20, 16)
        Me.LabelInvestedMoney.Name = "LabelInvestedMoney"
        Me.LabelInvestedMoney.Size = New System.Drawing.Size(200, 24)
        Me.LabelInvestedMoney.TabIndex = 0
        Me.LabelInvestedMoney.Text = "Amount invested ($)"
        Me.LabelInvestedMoney.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'FrameInvestedMoney
        '
        Me.FrameInvestedMoney.BorderColor = System.Drawing.SystemColors.ControlDark
        Me.FrameInvestedMoney.Controls.Add(Me.NumericUpDownInvestedMoney)
        Me.FrameInvestedMoney.DisabledBorderColor = System.Drawing.SystemColors.ControlLight
        Me.FrameInvestedMoney.ErrorBorderColor = System.Drawing.Color.Firebrick
        Me.FrameInvestedMoney.FocusedBorderColor = System.Drawing.SystemColors.Highlight
        Me.FrameInvestedMoney.Location = New System.Drawing.Point(20, 42)
        Me.FrameInvestedMoney.Name = "FrameInvestedMoney"
        Me.FrameInvestedMoney.Padding = New System.Windows.Forms.Padding(10, 0, 4, 0)
        Me.FrameInvestedMoney.Size = New System.Drawing.Size(200, 42)
        Me.FrameInvestedMoney.TabIndex = 1
        '
        'NumericUpDownInvestedMoney
        '
        Me.NumericUpDownInvestedMoney.ArrowColor = System.Drawing.SystemColors.ControlText
        Me.NumericUpDownInvestedMoney.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.NumericUpDownInvestedMoney.ButtonBackColor = System.Drawing.SystemColors.Control
        Me.NumericUpDownInvestedMoney.ButtonHoverBackColor = System.Drawing.SystemColors.ControlLight
        Me.NumericUpDownInvestedMoney.ButtonPressedBackColor = System.Drawing.SystemColors.ControlDark
        Me.NumericUpDownInvestedMoney.DecimalPlaces = 2
        Me.NumericUpDownInvestedMoney.DisabledArrowColor = System.Drawing.SystemColors.GrayText
        Me.NumericUpDownInvestedMoney.Font = New System.Drawing.Font("Segoe UI", 15.75!)
        Me.NumericUpDownInvestedMoney.Location = New System.Drawing.Point(10, 5)
        Me.NumericUpDownInvestedMoney.Maximum = New Decimal(New Integer() {-727379969, 232, 0, 0})
        Me.NumericUpDownInvestedMoney.Name = "NumericUpDownInvestedMoney"
        Me.NumericUpDownInvestedMoney.Size = New System.Drawing.Size(186, 31)
        Me.NumericUpDownInvestedMoney.TabIndex = 0
        Me.NumericUpDownInvestedMoney.ThousandsSeparator = True
        Me.NumericUpDownInvestedMoney.Value = New Decimal(New Integer() {4, 0, 0, 0})
        '
        'LabelLeverage
        '
        Me.LabelLeverage.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.LabelLeverage.Location = New System.Drawing.Point(236, 16)
        Me.LabelLeverage.Name = "LabelLeverage"
        Me.LabelLeverage.Size = New System.Drawing.Size(124, 24)
        Me.LabelLeverage.TabIndex = 2
        Me.LabelLeverage.Text = "Leverage (×)"
        Me.LabelLeverage.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'FrameLeverage
        '
        Me.FrameLeverage.BorderColor = System.Drawing.SystemColors.ControlDark
        Me.FrameLeverage.Controls.Add(Me.NumericUpDownLeverage)
        Me.FrameLeverage.DisabledBorderColor = System.Drawing.SystemColors.ControlLight
        Me.FrameLeverage.ErrorBorderColor = System.Drawing.Color.Firebrick
        Me.FrameLeverage.FocusedBorderColor = System.Drawing.SystemColors.Highlight
        Me.FrameLeverage.Location = New System.Drawing.Point(236, 42)
        Me.FrameLeverage.Name = "FrameLeverage"
        Me.FrameLeverage.Padding = New System.Windows.Forms.Padding(10, 0, 4, 0)
        Me.FrameLeverage.Size = New System.Drawing.Size(124, 42)
        Me.FrameLeverage.TabIndex = 3
        '
        'NumericUpDownLeverage
        '
        Me.NumericUpDownLeverage.ArrowColor = System.Drawing.SystemColors.ControlText
        Me.NumericUpDownLeverage.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.NumericUpDownLeverage.ButtonBackColor = System.Drawing.SystemColors.Control
        Me.NumericUpDownLeverage.ButtonHoverBackColor = System.Drawing.SystemColors.ControlLight
        Me.NumericUpDownLeverage.ButtonPressedBackColor = System.Drawing.SystemColors.ControlDark
        Me.NumericUpDownLeverage.DisabledArrowColor = System.Drawing.SystemColors.GrayText
        Me.NumericUpDownLeverage.Font = New System.Drawing.Font("Segoe UI", 15.75!)
        Me.NumericUpDownLeverage.Location = New System.Drawing.Point(10, 5)
        Me.NumericUpDownLeverage.Maximum = New Decimal(New Integer() {5000, 0, 0, 0})
        Me.NumericUpDownLeverage.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericUpDownLeverage.Name = "NumericUpDownLeverage"
        Me.NumericUpDownLeverage.Size = New System.Drawing.Size(110, 31)
        Me.NumericUpDownLeverage.TabIndex = 0
        Me.NumericUpDownLeverage.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'LabelCurrentPrice
        '
        Me.LabelCurrentPrice.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.LabelCurrentPrice.Location = New System.Drawing.Point(20, 100)
        Me.LabelCurrentPrice.Name = "LabelCurrentPrice"
        Me.LabelCurrentPrice.Size = New System.Drawing.Size(340, 24)
        Me.LabelCurrentPrice.TabIndex = 4
        Me.LabelCurrentPrice.Text = "Entry price"
        Me.LabelCurrentPrice.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'FrameCurrentPrice
        '
        Me.FrameCurrentPrice.BorderColor = System.Drawing.SystemColors.ControlDark
        Me.FrameCurrentPrice.Controls.Add(Me.TextBoxCurrentPrice)
        Me.FrameCurrentPrice.DisabledBorderColor = System.Drawing.SystemColors.ControlLight
        Me.FrameCurrentPrice.ErrorBorderColor = System.Drawing.Color.Firebrick
        Me.FrameCurrentPrice.FocusedBorderColor = System.Drawing.SystemColors.Highlight
        Me.FrameCurrentPrice.Location = New System.Drawing.Point(20, 126)
        Me.FrameCurrentPrice.Name = "FrameCurrentPrice"
        Me.FrameCurrentPrice.Padding = New System.Windows.Forms.Padding(10, 0, 4, 0)
        Me.FrameCurrentPrice.Size = New System.Drawing.Size(340, 42)
        Me.FrameCurrentPrice.TabIndex = 5
        '
        'TextBoxCurrentPrice
        '
        Me.TextBoxCurrentPrice.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBoxCurrentPrice.Font = New System.Drawing.Font("Segoe UI", 15.75!)
        Me.TextBoxCurrentPrice.Hint = "0.00"
        Me.TextBoxCurrentPrice.HintForeColor = System.Drawing.SystemColors.GrayText
        Me.TextBoxCurrentPrice.Location = New System.Drawing.Point(10, 7)
        Me.TextBoxCurrentPrice.Name = "TextBoxCurrentPrice"
        Me.TextBoxCurrentPrice.Size = New System.Drawing.Size(326, 28)
        Me.TextBoxCurrentPrice.TabIndex = 0
        '
        'LabelTargetPrice
        '
        Me.LabelTargetPrice.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.LabelTargetPrice.Location = New System.Drawing.Point(20, 184)
        Me.LabelTargetPrice.Name = "LabelTargetPrice"
        Me.LabelTargetPrice.Size = New System.Drawing.Size(170, 24)
        Me.LabelTargetPrice.TabIndex = 6
        Me.LabelTargetPrice.Text = "Target price"
        Me.LabelTargetPrice.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'RadioButtonTargetPriceFixed
        '
        Me.RadioButtonTargetPriceFixed.Appearance = System.Windows.Forms.Appearance.Button
        Me.RadioButtonTargetPriceFixed.Checked = True
        Me.RadioButtonTargetPriceFixed.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButtonTargetPriceFixed.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.RadioButtonTargetPriceFixed.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!)
        Me.RadioButtonTargetPriceFixed.Location = New System.Drawing.Point(204, 182)
        Me.RadioButtonTargetPriceFixed.Name = "RadioButtonTargetPriceFixed"
        Me.RadioButtonTargetPriceFixed.Size = New System.Drawing.Size(72, 28)
        Me.RadioButtonTargetPriceFixed.TabIndex = 7
        Me.RadioButtonTargetPriceFixed.TabStop = True
        Me.RadioButtonTargetPriceFixed.Text = "Price"
        Me.RadioButtonTargetPriceFixed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.RadioButtonTargetPriceFixed.UseVisualStyleBackColor = False
        '
        'RadioButtonTargetPricePercent
        '
        Me.RadioButtonTargetPricePercent.Appearance = System.Windows.Forms.Appearance.Button
        Me.RadioButtonTargetPricePercent.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButtonTargetPricePercent.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.RadioButtonTargetPricePercent.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!)
        Me.RadioButtonTargetPricePercent.Location = New System.Drawing.Point(276, 182)
        Me.RadioButtonTargetPricePercent.Name = "RadioButtonTargetPricePercent"
        Me.RadioButtonTargetPricePercent.Size = New System.Drawing.Size(84, 28)
        Me.RadioButtonTargetPricePercent.TabIndex = 8
        Me.RadioButtonTargetPricePercent.Text = "Percent"
        Me.RadioButtonTargetPricePercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.RadioButtonTargetPricePercent.UseVisualStyleBackColor = False
        '
        'FrameTargetPrice
        '
        Me.FrameTargetPrice.BorderColor = System.Drawing.SystemColors.ControlDark
        Me.FrameTargetPrice.Controls.Add(Me.TextBoxTargetPrice)
        Me.FrameTargetPrice.DisabledBorderColor = System.Drawing.SystemColors.ControlLight
        Me.FrameTargetPrice.ErrorBorderColor = System.Drawing.Color.Firebrick
        Me.FrameTargetPrice.FocusedBorderColor = System.Drawing.SystemColors.Highlight
        Me.FrameTargetPrice.Location = New System.Drawing.Point(20, 214)
        Me.FrameTargetPrice.Name = "FrameTargetPrice"
        Me.FrameTargetPrice.Padding = New System.Windows.Forms.Padding(10, 0, 4, 0)
        Me.FrameTargetPrice.Size = New System.Drawing.Size(200, 42)
        Me.FrameTargetPrice.TabIndex = 9
        '
        'TextBoxTargetPrice
        '
        Me.TextBoxTargetPrice.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBoxTargetPrice.Font = New System.Drawing.Font("Segoe UI", 15.75!)
        Me.TextBoxTargetPrice.Hint = "0.00"
        Me.TextBoxTargetPrice.HintForeColor = System.Drawing.SystemColors.GrayText
        Me.TextBoxTargetPrice.Location = New System.Drawing.Point(10, 7)
        Me.TextBoxTargetPrice.Name = "TextBoxTargetPrice"
        Me.TextBoxTargetPrice.Size = New System.Drawing.Size(186, 28)
        Me.TextBoxTargetPrice.TabIndex = 0
        '
        'FrameTargetPercent
        '
        Me.FrameTargetPercent.BorderColor = System.Drawing.SystemColors.ControlDark
        Me.FrameTargetPercent.Controls.Add(Me.NumericUpDownTargetPrice)
        Me.FrameTargetPercent.DisabledBorderColor = System.Drawing.SystemColors.ControlLight
        Me.FrameTargetPercent.ErrorBorderColor = System.Drawing.Color.Firebrick
        Me.FrameTargetPercent.FocusedBorderColor = System.Drawing.SystemColors.Highlight
        Me.FrameTargetPercent.Location = New System.Drawing.Point(236, 214)
        Me.FrameTargetPercent.Name = "FrameTargetPercent"
        Me.FrameTargetPercent.Padding = New System.Windows.Forms.Padding(10, 0, 4, 0)
        Me.FrameTargetPercent.Size = New System.Drawing.Size(124, 42)
        Me.FrameTargetPercent.TabIndex = 10
        '
        'NumericUpDownTargetPrice
        '
        Me.NumericUpDownTargetPrice.ArrowColor = System.Drawing.SystemColors.ControlText
        Me.NumericUpDownTargetPrice.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.NumericUpDownTargetPrice.ButtonBackColor = System.Drawing.SystemColors.Control
        Me.NumericUpDownTargetPrice.ButtonHoverBackColor = System.Drawing.SystemColors.ControlLight
        Me.NumericUpDownTargetPrice.ButtonPressedBackColor = System.Drawing.SystemColors.ControlDark
        Me.NumericUpDownTargetPrice.DecimalPlaces = 2
        Me.NumericUpDownTargetPrice.DisabledArrowColor = System.Drawing.SystemColors.GrayText
        Me.NumericUpDownTargetPrice.Enabled = False
        Me.NumericUpDownTargetPrice.Font = New System.Drawing.Font("Segoe UI", 15.75!)
        Me.NumericUpDownTargetPrice.Location = New System.Drawing.Point(10, 5)
        Me.NumericUpDownTargetPrice.Maximum = New Decimal(New Integer() {-727379969, 232, 0, 0})
        Me.NumericUpDownTargetPrice.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.NumericUpDownTargetPrice.Name = "NumericUpDownTargetPrice"
        Me.NumericUpDownTargetPrice.Size = New System.Drawing.Size(110, 31)
        Me.NumericUpDownTargetPrice.TabIndex = 0
        Me.NumericUpDownTargetPrice.Value = New Decimal(New Integer() {4, 0, 0, 0})
        '
        'PanelResult
        '
        Me.PanelResult.BorderColor = System.Drawing.SystemColors.ControlDark
        Me.PanelResult.Controls.Add(Me.LabelProfitCaption)
        Me.PanelResult.Controls.Add(Me.LabelProfitValue)
        Me.PanelResult.Location = New System.Drawing.Point(20, 274)
        Me.PanelResult.Name = "PanelResult"
        Me.PanelResult.Size = New System.Drawing.Size(340, 108)
        Me.PanelResult.TabIndex = 11
        '
        'LabelProfitCaption
        '
        Me.LabelProfitCaption.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.LabelProfitCaption.Location = New System.Drawing.Point(14, 10)
        Me.LabelProfitCaption.Name = "LabelProfitCaption"
        Me.LabelProfitCaption.Size = New System.Drawing.Size(312, 24)
        Me.LabelProfitCaption.TabIndex = 0
        Me.LabelProfitCaption.Text = "Estimated profit / loss"
        Me.LabelProfitCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LabelProfitValue
        '
        Me.LabelProfitValue.Font = New System.Drawing.Font("Segoe UI Semibold", 18.0!)
        Me.LabelProfitValue.Location = New System.Drawing.Point(12, 36)
        Me.LabelProfitValue.Name = "LabelProfitValue"
        Me.LabelProfitValue.Size = New System.Drawing.Size(316, 64)
        Me.LabelProfitValue.TabIndex = 1
        Me.LabelProfitValue.Text = "N/A"
        '
        'ButtonClear
        '
        Me.ButtonClear.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonClear.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!)
        Me.ButtonClear.Location = New System.Drawing.Point(20, 400)
        Me.ButtonClear.Name = "ButtonClear"
        Me.ButtonClear.Size = New System.Drawing.Size(165, 40)
        Me.ButtonClear.TabIndex = 12
        Me.ButtonClear.Text = "Reset"
        Me.ButtonClear.UseVisualStyleBackColor = False
        '
        'ButtonMakeScreenshot
        '
        Me.ButtonMakeScreenshot.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonMakeScreenshot.Enabled = False
        Me.ButtonMakeScreenshot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonMakeScreenshot.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!)
        Me.ButtonMakeScreenshot.Location = New System.Drawing.Point(195, 400)
        Me.ButtonMakeScreenshot.Name = "ButtonMakeScreenshot"
        Me.ButtonMakeScreenshot.Size = New System.Drawing.Size(165, 40)
        Me.ButtonMakeScreenshot.TabIndex = 13
        Me.ButtonMakeScreenshot.Text = "Save screenshot..."
        Me.ButtonMakeScreenshot.UseVisualStyleBackColor = False
        '
        'CheckBoxAlwaysOnTop
        '
        Me.CheckBoxAlwaysOnTop.AutoSize = True
        Me.CheckBoxAlwaysOnTop.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxAlwaysOnTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.CheckBoxAlwaysOnTop.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.CheckBoxAlwaysOnTop.Location = New System.Drawing.Point(20, 454)
        Me.CheckBoxAlwaysOnTop.Name = "CheckBoxAlwaysOnTop"
        Me.CheckBoxAlwaysOnTop.Size = New System.Drawing.Size(124, 25)
        Me.CheckBoxAlwaysOnTop.TabIndex = 14
        Me.CheckBoxAlwaysOnTop.Text = "Always on top"
        Me.CheckBoxAlwaysOnTop.UseVisualStyleBackColor = False
        '
        'ButtonCurrency
        '
        Me.ButtonCurrency.AccessibleName = "Currency"
        Me.ButtonCurrency.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonCurrency.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonCurrency.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!)
        Me.ButtonCurrency.Location = New System.Drawing.Point(176, 450)
        Me.ButtonCurrency.Name = "ButtonCurrency"
        Me.ButtonCurrency.Size = New System.Drawing.Size(36, 32)
        Me.ButtonCurrency.TabIndex = 15
        Me.ButtonCurrency.Text = "$"
        Me.ButtonCurrency.UseVisualStyleBackColor = False
        '
        'ButtonDecimalSeparator
        '
        Me.ButtonDecimalSeparator.AccessibleName = "Decimal separator"
        Me.ButtonDecimalSeparator.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonDecimalSeparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonDecimalSeparator.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!)
        Me.ButtonDecimalSeparator.Location = New System.Drawing.Point(220, 450)
        Me.ButtonDecimalSeparator.Name = "ButtonDecimalSeparator"
        Me.ButtonDecimalSeparator.Size = New System.Drawing.Size(96, 32)
        Me.ButtonDecimalSeparator.TabIndex = 16
        Me.ButtonDecimalSeparator.Text = "1,000.00"
        Me.ButtonDecimalSeparator.UseVisualStyleBackColor = False
        '
        'ButtonGitHub
        '
        Me.ButtonGitHub.AccessibleName = "GitHub repository"
        Me.ButtonGitHub.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonGitHub.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonGitHub.Location = New System.Drawing.Point(324, 450)
        Me.ButtonGitHub.Name = "ButtonGitHub"
        Me.ButtonGitHub.Size = New System.Drawing.Size(36, 32)
        Me.ButtonGitHub.TabIndex = 17
        Me.ToolTipMain.SetToolTip(Me.ButtonGitHub, "View source code on GitHub")
        Me.ButtonGitHub.UseVisualStyleBackColor = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(380, 496)
        Me.Controls.Add(Me.LabelInvestedMoney)
        Me.Controls.Add(Me.FrameInvestedMoney)
        Me.Controls.Add(Me.LabelLeverage)
        Me.Controls.Add(Me.FrameLeverage)
        Me.Controls.Add(Me.LabelCurrentPrice)
        Me.Controls.Add(Me.FrameCurrentPrice)
        Me.Controls.Add(Me.LabelTargetPrice)
        Me.Controls.Add(Me.RadioButtonTargetPriceFixed)
        Me.Controls.Add(Me.RadioButtonTargetPricePercent)
        Me.Controls.Add(Me.FrameTargetPrice)
        Me.Controls.Add(Me.FrameTargetPercent)
        Me.Controls.Add(Me.PanelResult)
        Me.Controls.Add(Me.ButtonClear)
        Me.Controls.Add(Me.ButtonMakeScreenshot)
        Me.Controls.Add(Me.CheckBoxAlwaysOnTop)
        Me.Controls.Add(Me.ButtonCurrency)
        Me.Controls.Add(Me.ButtonDecimalSeparator)
        Me.Controls.Add(Me.ButtonGitHub)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Investment Profit & Loss Calculator"
        Me.FrameInvestedMoney.ResumeLayout(False)
        CType(Me.NumericUpDownInvestedMoney, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FrameLeverage.ResumeLayout(False)
        CType(Me.NumericUpDownLeverage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FrameCurrentPrice.ResumeLayout(False)
        Me.FrameCurrentPrice.PerformLayout()
        Me.FrameTargetPrice.ResumeLayout(False)
        Me.FrameTargetPrice.PerformLayout()
        Me.FrameTargetPercent.ResumeLayout(False)
        CType(Me.NumericUpDownTargetPrice, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelResult.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LabelInvestedMoney As Label
    Friend WithEvents FrameInvestedMoney As InputFrame
    Friend WithEvents NumericUpDownInvestedMoney As ThemedNumericUpDown
    Friend WithEvents LabelLeverage As Label
    Friend WithEvents FrameLeverage As InputFrame
    Friend WithEvents NumericUpDownLeverage As ThemedNumericUpDown
    Friend WithEvents LabelCurrentPrice As Label
    Friend WithEvents FrameCurrentPrice As InputFrame
    Friend WithEvents TextBoxCurrentPrice As CustomTextBox
    Friend WithEvents LabelTargetPrice As Label
    Friend WithEvents RadioButtonTargetPriceFixed As RadioButton
    Friend WithEvents RadioButtonTargetPricePercent As RadioButton
    Friend WithEvents FrameTargetPrice As InputFrame
    Friend WithEvents TextBoxTargetPrice As CustomTextBox
    Friend WithEvents FrameTargetPercent As InputFrame
    Friend WithEvents NumericUpDownTargetPrice As ThemedNumericUpDown
    Friend WithEvents PanelResult As CardPanel
    Friend WithEvents LabelProfitCaption As Label
    Friend WithEvents LabelProfitValue As Label
    Friend WithEvents ButtonClear As Button
    Friend WithEvents ButtonMakeScreenshot As Button
    Friend WithEvents CheckBoxAlwaysOnTop As CheckBox
    Friend WithEvents ButtonCurrency As Button
    Friend WithEvents ButtonDecimalSeparator As Button
    Friend WithEvents ButtonGitHub As Button
    Friend WithEvents ToolTipMain As ToolTip
End Class
