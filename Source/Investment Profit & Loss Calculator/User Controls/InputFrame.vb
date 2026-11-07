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

#Region " InputFrame "

''' ----------------------------------------------------------------------------------------------------
''' <summary>
''' A flat, bordered container that hosts a single borderless input control
''' (e.g. a <see cref="TextBox"/> or a <see cref="NumericUpDown"/>),
''' drawing a modern border that is highlighted when the hosted control has the focus.
''' <para></para>
''' The hosted control is stretched horizontally and centered vertically.
''' </summary>
''' ----------------------------------------------------------------------------------------------------
<DesignerCategory("Code")>
Public Class InputFrame : Inherits Panel

#Region " Constants "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The height, in pixels (at 96 DPI), of the line drawn at the bottom of the frame when it has the focus.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Const FocusLineHeight As Integer = 2

#End Region

#Region " Fields "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="InputFrame.BorderColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private borderColorField As Color = SystemColors.ControlDark

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="InputFrame.FocusedBorderColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private focusedBorderColorField As Color = SystemColors.Highlight

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="InputFrame.DisabledBorderColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private disabledBorderColorField As Color = SystemColors.ControlLight

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Indicates whether the hosted control has the focus.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private isHostedControlFocused As Boolean

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="InputFrame.ErrorBorderColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private errorBorderColorField As Color = Color.Firebrick

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="InputFrame.HasError"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private hasErrorField As Boolean

#End Region

#Region " Properties "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the border color.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
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

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the border color when the hosted control has the focus.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Category("Appearance")>
    <Description("The border color when the hosted control has the focus.")>
    Public Property FocusedBorderColor As Color
        Get
            Return Me.focusedBorderColorField
        End Get
        Set(value As Color)
            Me.focusedBorderColorField = value
            Me.Invalidate()
        End Set
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the border color when the hosted control is disabled.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Category("Appearance")>
    <Description("The border color when the hosted control is disabled.")>
    Public Property DisabledBorderColor As Color
        Get
            Return Me.disabledBorderColorField
        End Get
        Set(value As Color)
            Me.disabledBorderColorField = value
            Me.Invalidate()
        End Set
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the border color when the hosted control contains an invalid value.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Category("Appearance")>
    <Description("The border color when the hosted control contains an invalid value.")>
    Public Property ErrorBorderColor As Color
        Get
            Return Me.errorBorderColorField
        End Get
        Set(value As Color)
            Me.errorBorderColorField = value
            Me.Invalidate()
        End Set
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets a value indicating whether the hosted control contains an invalid value.
    ''' <para></para>
    ''' When <see langword="True"/>, the border is drawn with <see cref="InputFrame.ErrorBorderColor"/>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property HasError As Boolean
        Get
            Return Me.hasErrorField
        End Get
        Set(value As Boolean)
            If Me.hasErrorField <> value Then
                Me.hasErrorField = value
                Me.Invalidate()
            End If
        End Set
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets a value indicating whether the hosted control has the focus.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property IsFocused As Boolean
        Get
            Return Me.isHostedControlFocused
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new instance of the <see cref="InputFrame"/> class.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public Sub New()
        MyBase.New()

        Me.SetStyle(ControlStyles.UserPaint Or
                    ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.ResizeRedraw, True)

        Me.Padding = New Padding(10, 0, 4, 0)
    End Sub

#End Region

#Region " Overridden Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="Control.ControlAdded"/> event.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnControlAdded(e As ControlEventArgs)
        AddHandler e.Control.Enter, AddressOf Me.HostedControl_Enter
        AddHandler e.Control.Leave, AddressOf Me.HostedControl_Leave
        AddHandler e.Control.EnabledChanged, AddressOf Me.HostedControl_EnabledChanged
        MyBase.OnControlAdded(e)
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="Control.ControlRemoved"/> event.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnControlRemoved(e As ControlEventArgs)
        RemoveHandler e.Control.Enter, AddressOf Me.HostedControl_Enter
        RemoveHandler e.Control.Leave, AddressOf Me.HostedControl_Leave
        RemoveHandler e.Control.EnabledChanged, AddressOf Me.HostedControl_EnabledChanged
        MyBase.OnControlRemoved(e)
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="Control.Layout"/> event,
    ''' stretching the hosted controls horizontally and centering them vertically.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnLayout(e As LayoutEventArgs)
        MyBase.OnLayout(e)

        Dim width As Integer = Math.Max(0, Me.ClientSize.Width - Me.Padding.Horizontal)
        For Each hostedControl As Control In Me.Controls
            Dim top As Integer = Math.Max(0, (Me.ClientSize.Height - hostedControl.Height) \ 2)
            hostedControl.SetBounds(Me.Padding.Left, top, width, hostedControl.Height)
        Next
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="Control.Paint"/> event, drawing the border of the frame.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim color As Color
        If Not Me.IsHostedControlEnabled() Then
            color = Me.disabledBorderColorField
        ElseIf Me.hasErrorField Then
            color = Me.errorBorderColorField
        ElseIf Me.isHostedControlFocused Then
            color = Me.focusedBorderColorField
        Else
            color = Me.borderColorField
        End If

        Dim bounds As New Rectangle(0, 0, Me.ClientSize.Width - 1, Me.ClientSize.Height - 1)
        Using pen As New Pen(color)
            e.Graphics.DrawRectangle(pen, bounds)
        End Using

        If Me.isHostedControlFocused Then
            Dim lineHeight As Integer = Math.Max(1, CInt(Math.Round(FocusLineHeight * e.Graphics.DpiY / 96.0F)))
            Using brush As New SolidBrush(color)
                e.Graphics.FillRectangle(brush, 0, Me.ClientSize.Height - lineHeight, Me.ClientSize.Width, lineHeight)
            End Using
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="Control.MouseDown"/> event, giving the focus to the hosted control
    ''' when the padding area of the frame is clicked.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)

        If Me.Controls.Count <> 0 AndAlso Me.Controls(0).CanFocus Then
            Me.Controls(0).Focus()
        End If
    End Sub

#End Region

#Region " Event Handlers "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Control.Enter"/> event of the hosted control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub HostedControl_Enter(sender As Object, e As EventArgs)
        Me.isHostedControlFocused = True
        Me.Invalidate()
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Control.Leave"/> event of the hosted control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub HostedControl_Leave(sender As Object, e As EventArgs)
        Me.isHostedControlFocused = False
        Me.Invalidate()
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Control.EnabledChanged"/> event of the hosted control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub HostedControl_EnabledChanged(sender As Object, e As EventArgs)
        Me.Invalidate()
    End Sub

#End Region

#Region " Private Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Determines whether the hosted control is enabled.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <returns>
    ''' <see langword="True"/> if there is no hosted control, or if it is enabled; otherwise, <see langword="False"/>.
    ''' </returns>
    ''' ----------------------------------------------------------------------------------------------------
    Private Function IsHostedControlEnabled() As Boolean
        Return Me.Enabled AndAlso (Me.Controls.Count = 0 OrElse Me.Controls(0).Enabled)
    End Function

#End Region

End Class

#End Region
