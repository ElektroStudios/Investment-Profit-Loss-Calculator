#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

#End Region

#Region " CustomTextBox "

''' <summary>
''' A <see cref="TextBox"/> that displays a hint (placeholder text) while it is empty.
''' </summary>
Public Class CustomTextBox : Inherits TextBox

#Region " Constants "

    ''' <summary>
    ''' The WM_PAINT window message.
    ''' </summary>
    Private Const WM_PAINT As Integer = &HF

#End Region

#Region " Fields "

    ''' <summary>
    ''' The value of the <see cref="CustomTextBox.HintForeColor"/> property.
    ''' </summary>
    Private hintForeColorField As Color = SystemColors.GrayText

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the hint text that is displayed while the text box is empty.
    ''' </summary>
    <Category("Appearance")>
    <Description("The hint text that is displayed while the text box is empty.")>
    Public Property Hint As String = ""

    ''' <summary>
    ''' Gets or sets the color of the hint text.
    ''' </summary>
    <Category("Appearance")>
    <Description("The color of the hint text.")>
    Public Property HintForeColor As Color
        Get
            Return Me.hintForeColorField
        End Get
        Set(value As Color)
            Me.hintForeColorField = value
            Me.Invalidate()
        End Set
    End Property

#End Region

#Region " Overridden Methods "

    ''' <summary>
    ''' Processes Windows messages, drawing the hint after the text box is painted.
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)

        If m.Msg = WM_PAINT AndAlso String.IsNullOrEmpty(Me.Text) AndAlso Not String.IsNullOrEmpty(Me.Hint) Then
            Using g As Graphics = Me.CreateGraphics()
                TextRenderer.DrawText(g, Me.Hint, Me.Font, Me.ClientRectangle, Me.hintForeColorField, Me.BackColor,
                                      TextFormatFlags.Top Or TextFormatFlags.Left)
            End Using
        End If
    End Sub

#End Region

End Class

#End Region
