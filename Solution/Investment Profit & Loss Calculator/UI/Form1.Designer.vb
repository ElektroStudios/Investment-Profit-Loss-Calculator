﻿
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.ButtonClear = New System.Windows.Forms.Button()
        Me.LabelProfitValue = New System.Windows.Forms.Label()
        Me.NumericUpDownLeverage = New System.Windows.Forms.NumericUpDown()
        Me.LabelLeverage = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.NumericUpDownTargetPrice = New System.Windows.Forms.NumericUpDown()
        Me.LabelTargetPricePercent = New System.Windows.Forms.Label()
        Me.RadioButtonTargetPriceFixed = New System.Windows.Forms.RadioButton()
        Me.RadioButtonTargetPricePercent = New System.Windows.Forms.RadioButton()
        Me.PanelTargetPrice = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CheckBoxAlwaysOnTop = New System.Windows.Forms.CheckBox()
        Me.ButtonMakeScreenshot = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.NumericUpDownInvestedMoney = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBoxTargetPrice = New CustomTextBox()
        Me.TextBoxCurrentPrice = New CustomTextBox()
        CType(Me.NumericUpDownLeverage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDownTargetPrice, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelTargetPrice.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDownInvestedMoney, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ButtonClear
        '
        Me.ButtonClear.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonClear.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ButtonClear.Location = New System.Drawing.Point(12, 401)
        Me.ButtonClear.Name = "ButtonClear"
        Me.ButtonClear.Size = New System.Drawing.Size(154, 30)
        Me.ButtonClear.TabIndex = 11
        Me.ButtonClear.Text = "Clear Values"
        Me.ButtonClear.UseVisualStyleBackColor = True
        '
        'LabelProfitValue
        '
        Me.LabelProfitValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelProfitValue.ForeColor = System.Drawing.Color.LightGray
        Me.LabelProfitValue.Location = New System.Drawing.Point(16, 307)
        Me.LabelProfitValue.Name = "LabelProfitValue"
        Me.LabelProfitValue.Size = New System.Drawing.Size(311, 69)
        Me.LabelProfitValue.TabIndex = 10
        Me.LabelProfitValue.Text = "..."
        '
        'NumericUpDownLeverage
        '
        Me.NumericUpDownLeverage.Cursor = System.Windows.Forms.Cursors.Hand
        Me.NumericUpDownLeverage.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!)
        Me.NumericUpDownLeverage.Location = New System.Drawing.Point(221, 43)
        Me.NumericUpDownLeverage.Maximum = New Decimal(New Integer() {5000, 0, 0, 0})
        Me.NumericUpDownLeverage.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericUpDownLeverage.Name = "NumericUpDownLeverage"
        Me.NumericUpDownLeverage.Size = New System.Drawing.Size(106, 31)
        Me.NumericUpDownLeverage.TabIndex = 4
        Me.NumericUpDownLeverage.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'LabelLeverage
        '
        Me.LabelLeverage.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelLeverage.ForeColor = System.Drawing.Color.LightGray
        Me.LabelLeverage.Location = New System.Drawing.Point(216, 9)
        Me.LabelLeverage.Name = "LabelLeverage"
        Me.LabelLeverage.Size = New System.Drawing.Size(111, 31)
        Me.LabelLeverage.TabIndex = 3
        Me.LabelLeverage.Text = "Leverage:"
        Me.LabelLeverage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.LightGray
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(185, 31)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Investment:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.LightGray
        Me.Label2.Location = New System.Drawing.Point(16, 82)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(311, 31)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Current Asset Price:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.LightGray
        Me.Label3.Location = New System.Drawing.Point(16, 276)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(311, 31)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Profit / Loss:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'NumericUpDownTargetPrice
        '
        Me.NumericUpDownTargetPrice.Cursor = System.Windows.Forms.Cursors.Hand
        Me.NumericUpDownTargetPrice.DecimalPlaces = 2
        Me.NumericUpDownTargetPrice.Enabled = False
        Me.NumericUpDownTargetPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!)
        Me.NumericUpDownTargetPrice.Location = New System.Drawing.Point(114, 42)
        Me.NumericUpDownTargetPrice.Maximum = New Decimal(New Integer() {-727379969, 232, 0, 0})
        Me.NumericUpDownTargetPrice.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.NumericUpDownTargetPrice.Name = "NumericUpDownTargetPrice"
        Me.NumericUpDownTargetPrice.Size = New System.Drawing.Size(167, 31)
        Me.NumericUpDownTargetPrice.TabIndex = 3
        '
        'LabelTargetPricePercent
        '
        Me.LabelTargetPricePercent.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelTargetPricePercent.ForeColor = System.Drawing.Color.LightGray
        Me.LabelTargetPricePercent.Location = New System.Drawing.Point(287, 41)
        Me.LabelTargetPricePercent.Name = "LabelTargetPricePercent"
        Me.LabelTargetPricePercent.Size = New System.Drawing.Size(20, 31)
        Me.LabelTargetPricePercent.TabIndex = 4
        Me.LabelTargetPricePercent.Text = "%"
        Me.LabelTargetPricePercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RadioButtonTargetPriceFixed
        '
        Me.RadioButtonTargetPriceFixed.AutoSize = True
        Me.RadioButtonTargetPriceFixed.Checked = True
        Me.RadioButtonTargetPriceFixed.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!)
        Me.RadioButtonTargetPriceFixed.ForeColor = System.Drawing.Color.LightGray
        Me.RadioButtonTargetPriceFixed.Location = New System.Drawing.Point(4, 7)
        Me.RadioButtonTargetPriceFixed.Name = "RadioButtonTargetPriceFixed"
        Me.RadioButtonTargetPriceFixed.Size = New System.Drawing.Size(83, 29)
        Me.RadioButtonTargetPriceFixed.TabIndex = 0
        Me.RadioButtonTargetPriceFixed.TabStop = True
        Me.RadioButtonTargetPriceFixed.Text = "Fixed"
        Me.RadioButtonTargetPriceFixed.UseVisualStyleBackColor = True
        '
        'RadioButtonTargetPricePercent
        '
        Me.RadioButtonTargetPricePercent.AutoSize = True
        Me.RadioButtonTargetPricePercent.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!)
        Me.RadioButtonTargetPricePercent.ForeColor = System.Drawing.Color.LightGray
        Me.RadioButtonTargetPricePercent.Location = New System.Drawing.Point(4, 42)
        Me.RadioButtonTargetPricePercent.Name = "RadioButtonTargetPricePercent"
        Me.RadioButtonTargetPricePercent.Size = New System.Drawing.Size(104, 29)
        Me.RadioButtonTargetPricePercent.TabIndex = 2
        Me.RadioButtonTargetPricePercent.Text = "Percent"
        Me.RadioButtonTargetPricePercent.UseVisualStyleBackColor = True
        '
        'PanelTargetPrice
        '
        Me.PanelTargetPrice.Controls.Add(Me.TextBoxTargetPrice)
        Me.PanelTargetPrice.Controls.Add(Me.RadioButtonTargetPricePercent)
        Me.PanelTargetPrice.Controls.Add(Me.LabelTargetPricePercent)
        Me.PanelTargetPrice.Controls.Add(Me.RadioButtonTargetPriceFixed)
        Me.PanelTargetPrice.Controls.Add(Me.NumericUpDownTargetPrice)
        Me.PanelTargetPrice.Location = New System.Drawing.Point(17, 189)
        Me.PanelTargetPrice.Name = "PanelTargetPrice"
        Me.PanelTargetPrice.Size = New System.Drawing.Size(310, 79)
        Me.PanelTargetPrice.TabIndex = 8
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.LightGray
        Me.Label4.Location = New System.Drawing.Point(16, 155)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(311, 31)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Target Asset Price:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CheckBoxAlwaysOnTop
        '
        Me.CheckBoxAlwaysOnTop.AutoSize = True
        Me.CheckBoxAlwaysOnTop.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CheckBoxAlwaysOnTop.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!)
        Me.CheckBoxAlwaysOnTop.ForeColor = System.Drawing.Color.LightGray
        Me.CheckBoxAlwaysOnTop.Location = New System.Drawing.Point(12, 437)
        Me.CheckBoxAlwaysOnTop.Name = "CheckBoxAlwaysOnTop"
        Me.CheckBoxAlwaysOnTop.Size = New System.Drawing.Size(193, 24)
        Me.CheckBoxAlwaysOnTop.TabIndex = 13
        Me.CheckBoxAlwaysOnTop.Text = "Window Always On Top"
        Me.CheckBoxAlwaysOnTop.UseVisualStyleBackColor = True
        '
        'ButtonMakeScreenshot
        '
        Me.ButtonMakeScreenshot.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonMakeScreenshot.Enabled = False
        Me.ButtonMakeScreenshot.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ButtonMakeScreenshot.Location = New System.Drawing.Point(176, 401)
        Me.ButtonMakeScreenshot.Name = "ButtonMakeScreenshot"
        Me.ButtonMakeScreenshot.Size = New System.Drawing.Size(154, 30)
        Me.ButtonMakeScreenshot.TabIndex = 12
        Me.ButtonMakeScreenshot.Text = "Take Screenshot"
        Me.ButtonMakeScreenshot.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(72, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(72, Byte), Integer))
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PictureBox1.Location = New System.Drawing.Point(-3, 379)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(363, 10)
        Me.PictureBox1.TabIndex = 13
        Me.PictureBox1.TabStop = False
        '
        'NumericUpDownInvestedMoney
        '
        Me.NumericUpDownInvestedMoney.Cursor = System.Windows.Forms.Cursors.Hand
        Me.NumericUpDownInvestedMoney.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!)
        Me.NumericUpDownInvestedMoney.Location = New System.Drawing.Point(17, 43)
        Me.NumericUpDownInvestedMoney.Maximum = New Decimal(New Integer() {-727379969, 232, 0, 0})
        Me.NumericUpDownInvestedMoney.Name = "NumericUpDownInvestedMoney"
        Me.NumericUpDownInvestedMoney.Size = New System.Drawing.Size(165, 31)
        Me.NumericUpDownInvestedMoney.TabIndex = 1
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.LightGray
        Me.Label5.Location = New System.Drawing.Point(184, 43)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(20, 31)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "$"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxTargetPrice
        '
        Me.TextBoxTargetPrice.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.TextBoxTargetPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxTargetPrice.Hint = "Target Price"
        Me.TextBoxTargetPrice.Location = New System.Drawing.Point(114, 5)
        Me.TextBoxTargetPrice.Name = "TextBoxTargetPrice"
        Me.TextBoxTargetPrice.Size = New System.Drawing.Size(193, 31)
        Me.TextBoxTargetPrice.TabIndex = 1
        '
        'TextBoxCurrentPrice
        '
        Me.TextBoxCurrentPrice.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.TextBoxCurrentPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxCurrentPrice.Hint = "Current Price"
        Me.TextBoxCurrentPrice.Location = New System.Drawing.Point(17, 116)
        Me.TextBoxCurrentPrice.Name = "TextBoxCurrentPrice"
        Me.TextBoxCurrentPrice.Size = New System.Drawing.Size(310, 31)
        Me.TextBoxCurrentPrice.TabIndex = 6
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(43, Byte), Integer), CType(CType(43, Byte), Integer), CType(CType(43, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(342, 468)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.NumericUpDownInvestedMoney)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.ButtonMakeScreenshot)
        Me.Controls.Add(Me.CheckBoxAlwaysOnTop)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.PanelTargetPrice)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LabelProfitValue)
        Me.Controls.Add(Me.NumericUpDownLeverage)
        Me.Controls.Add(Me.ButtonClear)
        Me.Controls.Add(Me.TextBoxCurrentPrice)
        Me.Controls.Add(Me.LabelLeverage)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Investment Profit & Loss Calculator"
        CType(Me.NumericUpDownLeverage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDownTargetPrice, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelTargetPrice.ResumeLayout(False)
        Me.PanelTargetPrice.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDownInvestedMoney, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBoxTargetPrice As CustomTextBox
    Friend WithEvents TextBoxCurrentPrice As CustomTextBox
    Friend WithEvents ButtonClear As Button
    Friend WithEvents NumericUpDownLeverage As NumericUpDown
    Friend WithEvents LabelLeverage As Label
    Friend WithEvents LabelProfitValue As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents NumericUpDownTargetPrice As NumericUpDown
    Friend WithEvents LabelTargetPricePercent As Label
    Friend WithEvents RadioButtonTargetPriceFixed As RadioButton
    Friend WithEvents RadioButtonTargetPricePercent As RadioButton
    Friend WithEvents PanelTargetPrice As Panel
    Friend WithEvents Label4 As Label
    Friend WithEvents CheckBoxAlwaysOnTop As CheckBox
    Friend WithEvents ButtonMakeScreenshot As Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents NumericUpDownInvestedMoney As NumericUpDown
    Friend WithEvents Label5 As Label
End Class
