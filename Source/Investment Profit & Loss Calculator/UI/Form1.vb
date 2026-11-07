
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.Runtime.InteropServices

Imports FormExtensions

Imports Core

Public Class Form1

    Private invested As Double
    Private currentPrice As Double
    Private targetPrice As Double

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Application.CurrentCulture = CultureInfo.InvariantCulture
    End Sub

    Private Sub NumericUpDownInvestedMoney_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDownInvestedMoney.TextChanged
        Dim num As NumericUpDown = DirectCast(sender, NumericUpDown)
        Me.invested = num.Value
        Me.CalculatePL()
    End Sub

    Private Sub NumericUpDownInvestedMoney_LostFocus(sender As Object, e As EventArgs) Handles NumericUpDownInvestedMoney.LostFocus
        Dim num As NumericUpDown = DirectCast(sender, NumericUpDown)
        If String.IsNullOrWhiteSpace(num.Text) Then
            num.Text = num.Minimum
        End If
    End Sub

    Private Sub TextBoxcurrentPrice_TextChanged(sender As Object, e As EventArgs) Handles TextBoxCurrentPrice.TextChanged
        ProfitLossCalculator.TryParsePrice(DirectCast(sender, TextBox).Text, Me.currentPrice)

        If Me.RadioButtonTargetPricePercent.Checked Then
            Me.TextBoxTargetPrice.Text = CStr(ProfitLossCalculator.CalculateTargetPrice(Me.currentPrice, CDbl(Me.NumericUpDownTargetPrice.Value)))
        End If
        If Me.targetPrice <> 0 Then
            Me.UpdateNumericUpDownTargetPrice()
        End If
        Me.CalculatePL()
    End Sub

    Private Sub TextBoxtargetPrice_TextChanged(sender As Object, e As EventArgs) Handles TextBoxTargetPrice.TextChanged
        Dim tb As CustomTextBox = DirectCast(sender, TextBox)
        ProfitLossCalculator.TryParsePrice(tb.Text, Me.targetPrice)
        If Me.targetPrice < 0 Then
            Me.targetPrice = 0
            tb.Text = 0
        End If

        Me.UpdateNumericUpDownTargetPrice()

        If tb.Enabled Then
            Me.CalculatePL()
        End If
    End Sub

    Private Sub NumericUpDownLeverage_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDownLeverage.TextChanged
        Me.CalculatePL()
    End Sub

    Private Sub NumericUpDownLeverage_LostFocus(sender As Object, e As EventArgs) Handles NumericUpDownLeverage.LostFocus
        Dim num As NumericUpDown = DirectCast(sender, NumericUpDown)
        If String.IsNullOrWhiteSpace(num.Text) Then
            num.Text = num.Minimum
        End If
    End Sub

    Private Sub NumericUpDownTargetPrice_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDownTargetPrice.TextChanged
        Dim num As NumericUpDown = DirectCast(sender, NumericUpDown)
        If num.Enabled Then
            Me.TextBoxTargetPrice.Text = CStr(ProfitLossCalculator.CalculateTargetPrice(Me.currentPrice, CDbl(num.Value)))
            Me.CalculatePL()
        End If
    End Sub

    Private Sub RadioButtonTargetPriceFixed_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonTargetPriceFixed.CheckedChanged
        Me.TextBoxTargetPrice.Enabled = True
        Me.NumericUpDownTargetPrice.Enabled = False
        Me.CalculatePL()
    End Sub

    Private Sub RadioButtonTargetPricePercent_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonTargetPricePercent.CheckedChanged
        Me.TextBoxTargetPrice.Enabled = False
        Me.NumericUpDownTargetPrice.Enabled = True
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

    Private Sub CalculatePL()

        Dim result As ProfitLossResult
        If String.IsNullOrEmpty(Me.TextBoxTargetPrice.Text) Then
            result = ProfitLossResult.NotAvailable
        Else
            result = ProfitLossCalculator.Calculate(Me.invested, Me.currentPrice, Me.targetPrice, CDbl(Me.NumericUpDownLeverage.Value))
        End If

        Me.LabelProfitValue.Text = result.ToDisplayText()

        Select Case result.Kind
            Case ProfitLossKind.Profit
                Me.LabelProfitValue.ForeColor = Color.GreenYellow

            Case ProfitLossKind.Loss
                Me.LabelProfitValue.ForeColor = Color.IndianRed

            Case Else ' NotAvailable, BreakEven
                Me.LabelProfitValue.ForeColor = Color.LightGray

        End Select

        Me.ButtonMakeScreenshot.Enabled = result.IsAvailable

    End Sub

    Private Sub ButtonMakeScreenshot_Click(sender As Object, e As EventArgs) Handles ButtonMakeScreenshot.Click

        Dim dlg As New SaveFileDialog With {
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

        If dlg.ShowDialog = DialogResult.OK Then
            Using bmp As New Bitmap(Me.Bounds.Width, Me.DesktopBounds.Height, PixelFormat.Format24bppRgb)
                Me.DrawToBitmap(bmp, New Rectangle(Point.Empty, bmp.Size))
                bmp.Save(dlg.FileName, ImageFormat.Jpeg)
            End Using
        End If

    End Sub

End Class
