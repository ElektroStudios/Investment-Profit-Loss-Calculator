Public Class CustomTextBox : Inherits TextBox

    Public Property Hint As String = ""

    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)

        If m.Msg = 15 Then ' WindowMessages.WM_Paint
            If String.IsNullOrEmpty(Me.Text) AndAlso Not String.IsNullOrEmpty(Me.Hint) Then
                Using g As Graphics = Me.CreateGraphics(), fnt As Font = New Font(Me.Font, Me.Font.Style)
                    TextRenderer.DrawText(g, Me.Hint, fnt, Me.ClientRectangle, SystemColors.GrayText, Me.BackColor, TextFormatFlags.Top Or TextFormatFlags.Left)
                End Using
            End If
        End If

    End Sub

End Class