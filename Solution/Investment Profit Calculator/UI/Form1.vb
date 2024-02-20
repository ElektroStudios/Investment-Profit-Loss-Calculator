
Imports System.Drawing.Imaging
Imports System.Globalization
Imports FormExtensions

Public Class Form1

    Private invested As Double
    Private currentPrice As Double
    Private targetPrice As Double

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Application.CurrentCulture = CultureInfo.InvariantCulture
    End Sub

    Private Sub NumericUpDownInvestedMoney_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDownInvestedMoney.ValueChanged
        Dim num As NumericUpDown = DirectCast(sender, NumericUpDown)
        Me.invested = num.Value
        Me.CalculatePL()
    End Sub

    Private Sub TextBoxcurrentPrice_TextChanged(sender As Object, e As EventArgs) Handles TextBoxCurrentPrice.TextChanged
        Double.TryParse(DirectCast(sender, TextBox).Text.Replace(",", "."), Me.currentPrice)

        If Me.targetPrice <> 0 Then
            Me.UpdateNumericUpDownTargetPrice()
        End If
        Me.CalculatePL()
    End Sub

    Private Sub TextBoxtargetPrice_TextChanged(sender As Object, e As EventArgs) Handles TextBoxTargetPrice.TextChanged
        Dim tb As CustomTextBox = DirectCast(sender, TextBox)
        Double.TryParse(tb.Text.Replace(",", "."), Me.targetPrice)
        If Me.targetPrice < 0 Then
            Me.targetPrice = 0
            tb.Text = 0
        End If

        Me.UpdateNumericUpDownTargetPrice()

        If tb.Enabled Then
            Me.CalculatePL()
        End If
    End Sub

    Private Sub NumericUpDownLeverage_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDownLeverage.ValueChanged
        Me.CalculatePL()
    End Sub

    Private Sub NumericUpDownTargetPrice_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDownTargetPrice.ValueChanged
        Dim num As NumericUpDown = DirectCast(sender, NumericUpDown)
        If num.Enabled Then
            Me.TextBoxTargetPrice.Text = Me.currentPrice + (Me.currentPrice * (num.Value / 100))
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
        Dim change As Double = ((Me.targetPrice - Me.currentPrice) / Math.Abs(Me.currentPrice)) * 100
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

        If (Me.invested = 0) OrElse
            (Me.currentPrice = 0) OrElse
            (String.IsNullOrEmpty(Me.TextBoxTargetPrice.Text)) OrElse
            (String.IsNullOrEmpty(Me.NumericUpDownTargetPrice.Value = 0)) Then

            Me.LabelProfitValue.Text = "N/A"
            Me.LabelProfitValue.ForeColor = Color.LightGray
            Me.ButtonMakeScreenshot.Enabled = False

        Else
            Dim tempInvested As Double = (Me.invested * Me.NumericUpDownLeverage.Value)
            Dim result As Double = ((Me.targetPrice / Me.currentPrice) * tempInvested) - tempInvested
            Dim change As Double = ((Me.targetPrice - Me.currentPrice) / Math.Abs(Me.currentPrice)) * 100

            Select Case result

                Case > 0
                    Me.LabelProfitValue.Text = $"{result.ToString("C", CultureInfo.GetCultureInfo("en-US"))}  (+{change:0.##}%)"
                    Me.LabelProfitValue.ForeColor = Color.GreenYellow

                Case < 0
                    Me.LabelProfitValue.Text = $"{result.ToString("C", CultureInfo.GetCultureInfo("en-US"))}  ({change:0.##}%)"
                    Me.LabelProfitValue.ForeColor = Color.IndianRed

                Case Else ' = 0
                    Me.LabelProfitValue.Text = 0.ToString("C0", CultureInfo.GetCultureInfo("en-US"))
                    Me.LabelProfitValue.ForeColor = Color.LightGray

            End Select

            Me.ButtonMakeScreenshot.Enabled = True

        End If

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
