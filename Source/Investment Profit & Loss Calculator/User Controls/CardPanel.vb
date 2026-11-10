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

#Region " CardPanel "

''' <summary>
''' A flat panel with a thin border, used to group related content as a "card".
''' </summary>
<DesignerCategory("Code")>
Public Class CardPanel : Inherits Panel ' NOSONAR

#Region " Fields "

    ''' <summary>
    ''' The value of the <see cref="CardPanel.BorderColor"/> property.
    ''' </summary>
    Private borderColorField As Color = SystemColors.ControlDark

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the border color.
    ''' </summary>
    <Category("Appearance")>
    <Description("The border color.")>
    Public Property BorderColor As Color
        Get
            Return Me.borderColorField
        End Get
        Set(value As Color)
            Me.borderColorField = value
            Me.Invalidate()
        End Set
    End Property

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="CardPanel"/> class.
    ''' </summary>
    Public Sub New()
        MyBase.New()

        Me.SetStyle(ControlStyles.UserPaint Or
                    ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.ResizeRedraw, True)
    End Sub

#End Region

#Region " Overridden Methods "

    ''' <summary>
    ''' Raises the <see cref="Control.Paint"/> event, drawing the border of the card.
    ''' </summary>
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Using pen As New Pen(Me.borderColorField)
            e.Graphics.DrawRectangle(pen, 0, 0, Me.ClientSize.Width - 1, Me.ClientSize.Height - 1)
        End Using
    End Sub

#End Region

End Class

#End Region
