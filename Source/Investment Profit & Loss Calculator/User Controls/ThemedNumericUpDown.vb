#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

#End Region

#Region " ThemedNumericUpDown "

''' ----------------------------------------------------------------------------------------------------
''' <summary>
''' A <see cref="NumericUpDown"/> whose up/down buttons are drawn flat with customizable colors,
''' instead of the native light-themed buttons, so it fits in dark themes.
''' <para></para>
''' It also accepts both the dot and the comma as decimal separator (see <see cref="ValidateEditText"/>).
''' </summary>
''' ----------------------------------------------------------------------------------------------------
<DesignerCategory("Code")>
Public Class ThemedNumericUpDown : Inherits NumericUpDown

#Region " Enumerations "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Specifies a part of the up/down buttons.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Enum ButtonPart As Integer

        ''' <summary>
        ''' No button.
        ''' </summary>
        None = 0

        ''' <summary>
        ''' The up button.
        ''' </summary>
        Up = 1

        ''' <summary>
        ''' The down button.
        ''' </summary>
        Down = 2

    End Enum

#End Region

#Region " Fields "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The internal control that draws the up/down buttons.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private ReadOnly buttonsControl As Control

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The button part under the mouse pointer the last time it was checked.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private lastHoveredPart As ButtonPart

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Indicates whether <see cref="ThemedNumericUpDown.ValidateEditText"/> is replacing the typed text 
    ''' with its normalized form (used to handle the re-entrant validation of the Text setter).
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private isNormalizingText As Boolean

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="ThemedNumericUpDown.ButtonBackColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private buttonBackColorField As Color = SystemColors.Control

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="ThemedNumericUpDown.ButtonHoverBackColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private buttonHoverBackColorField As Color = SystemColors.ControlLight

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="ThemedNumericUpDown.ButtonPressedBackColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private buttonPressedBackColorField As Color = SystemColors.ControlDark

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="ThemedNumericUpDown.ArrowColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private arrowColorField As Color = SystemColors.ControlText

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' The value of the <see cref="ThemedNumericUpDown.DisabledArrowColor"/> property.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private disabledArrowColorField As Color = SystemColors.GrayText

#End Region

#Region " Properties "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the background color of the up/down buttons.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Category("Appearance")>
    <Description("The background color of the up/down buttons.")>
    Public Property ButtonBackColor As Color
        Get
            Return Me.buttonBackColorField
        End Get
        Set(value As Color)
            Me.buttonBackColorField = value
            Me.buttonsControl.Invalidate()
        End Set
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the background color of the up/down buttons when the mouse is over them.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Category("Appearance")>
    <Description("The background color of the up/down buttons when the mouse is over them.")>
    Public Property ButtonHoverBackColor As Color
        Get
            Return Me.buttonHoverBackColorField
        End Get
        Set(value As Color)
            Me.buttonHoverBackColorField = value
            Me.buttonsControl.Invalidate()
        End Set
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the background color of the up/down buttons when they are pressed.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Category("Appearance")>
    <Description("The background color of the up/down buttons when they are pressed.")>
    Public Property ButtonPressedBackColor As Color
        Get
            Return Me.buttonPressedBackColorField
        End Get
        Set(value As Color)
            Me.buttonPressedBackColorField = value
            Me.buttonsControl.Invalidate()
        End Set
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the color of the arrows of the up/down buttons.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Category("Appearance")>
    <Description("The color of the arrows of the up/down buttons.")>
    Public Property ArrowColor As Color
        Get
            Return Me.arrowColorField
        End Get
        Set(value As Color)
            Me.arrowColorField = value
            Me.buttonsControl.Invalidate()
        End Set
    End Property

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets or sets the color of the arrows of the up/down buttons when the control is disabled.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <Category("Appearance")>
    <Description("The color of the arrows of the up/down buttons when the control is disabled.")>
    Public Property DisabledArrowColor As Color
        Get
            Return Me.disabledArrowColorField
        End Get
        Set(value As Color)
            Me.disabledArrowColorField = value
            Me.buttonsControl.Invalidate()
        End Set
    End Property

#End Region

#Region " Constructors "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Initializes a new instance of the <see cref="ThemedNumericUpDown"/> class.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <exception cref="InvalidOperationException">
    ''' Thrown when the internal up/down buttons control cannot be found.
    ''' </exception>
    ''' ----------------------------------------------------------------------------------------------------
    Public Sub New()
        MyBase.New()

        Me.buttonsControl = ThemedNumericUpDown.FindButtonsControl(Me)

        AddHandler Me.buttonsControl.Paint, AddressOf Me.ButtonsControl_Paint
        AddHandler Me.buttonsControl.MouseMove, AddressOf Me.ButtonsControl_MouseMove
        AddHandler Me.buttonsControl.MouseLeave, AddressOf Me.ButtonsControl_MouseStateChanged
        AddHandler Me.buttonsControl.MouseDown, AddressOf Me.ButtonsControl_MouseStateChanged
        AddHandler Me.buttonsControl.MouseUp, AddressOf Me.ButtonsControl_MouseStateChanged
    End Sub

#End Region

#Region " Public Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the value that is currently displayed, without validating the text that is being typed.
    ''' <para></para>
    ''' Reading <see cref="NumericUpDown.Value"/> while the user is typing validates and reformats the text 
    ''' (e.g. "1" becomes "1.00"), which moves the caret to the beginning of the text. 
    ''' This method parses the typed text instead (see <see cref="Core.NumberTextParser"/>), 
    ''' constrained to <see cref="NumericUpDown.Minimum"/> and <see cref="NumericUpDown.Maximum"/>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <param name="value">
    ''' When this method returns, contains the current value, or zero if the typed text is not a valid number.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <returns>
    ''' <see langword="True"/> if the current value is valid; 
    ''' <see langword="False"/> if the user is typing a text that is not a valid number (including an empty text).
    ''' </returns>
    ''' ----------------------------------------------------------------------------------------------------
    Public Function TryGetEditValue(ByRef value As Decimal) As Boolean

        If Not Me.UserEdit Then
            ' The text is not being edited, so reading Value does not trigger the validation.
            value = Me.Value
            Return True
        End If

        If Not Core.NumberTextParser.TryParseDecimal(Me.Text, ThemedNumericUpDown.GetPreferredDecimalSeparator(), value) Then
            value = 0D
            Return False
        End If

        value = Math.Max(Me.Minimum, Math.Min(Me.Maximum, value))
        Return True
    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Reformats the displayed text using the current culture of the thread.
    ''' <para></para>
    ''' Call this method after changing the culture of the thread, 
    ''' as the control does not reformat its text by itself.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Public Sub RefreshText()
        Me.UpdateEditText()
    End Sub

#End Region

#Region " Overridden Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Validates and parses the text typed by the user.
    ''' <para></para>
    ''' The text is normalized with <see cref="Core.NumberTextParser"/> before the base parsing, so both the dot (".") 
    ''' and the comma (",") are accepted as decimal separator, with optional thousands separators,
    ''' (e.g. "1000,50", "1,000.50" and "1.000,50" are 1000.50). Ambiguous texts are resolved with the
    ''' decimal separator of the current culture.
    ''' <para></para>
    ''' If the text cannot be parsed, the base behavior applies (the previous value is restored).
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub ValidateEditText()

        ' The Text setter of UpDownBase calls ValidateEditText again while UserEdit is True, 
        ' so the re-entrant call (made while normalizing) must go straight to the base implementation.
        If Me.isNormalizingText OrElse Not Me.UserEdit Then
            MyBase.ValidateEditText()
            Return
        End If

        Dim value As Decimal
        If Not Core.NumberTextParser.TryParseDecimal(Me.Text, ThemedNumericUpDown.GetPreferredDecimalSeparator(), value) Then
            MyBase.ValidateEditText()
            Return
        End If

        Me.isNormalizingText = True
        Try
            ' The base class parses the text using the current culture, without thousands separators.
            ' This assignment validates the text through the re-entrant call above.
            Me.Text = value.ToString(Globalization.CultureInfo.CurrentCulture)
        Finally
            Me.isNormalizingText = False
        End Try

        ' Only needed if the Text setter did not validate the text (it always should, while UserEdit is True).
        If Me.UserEdit Then
            MyBase.ValidateEditText()
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="Control.EnabledChanged"/> event.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        MyBase.OnEnabledChanged(e)
        Me.buttonsControl.Invalidate()
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Releases the unmanaged resources used by the control, and optionally releases the managed resources.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            RemoveHandler Me.buttonsControl.Paint, AddressOf Me.ButtonsControl_Paint
            RemoveHandler Me.buttonsControl.MouseMove, AddressOf Me.ButtonsControl_MouseMove
            RemoveHandler Me.buttonsControl.MouseLeave, AddressOf Me.ButtonsControl_MouseStateChanged
            RemoveHandler Me.buttonsControl.MouseDown, AddressOf Me.ButtonsControl_MouseStateChanged
            RemoveHandler Me.buttonsControl.MouseUp, AddressOf Me.ButtonsControl_MouseStateChanged
        End If
        MyBase.Dispose(disposing)
    End Sub

#End Region

#Region " Event Handlers "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Control.Paint"/> event of the up/down buttons control,
    ''' painting flat buttons over the native ones.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonsControl_Paint(sender As Object, e As PaintEventArgs)

        Dim size As Size = Me.buttonsControl.ClientSize
        Dim upBounds As New Rectangle(0, 0, size.Width, size.Height \ 2)
        Dim downBounds As New Rectangle(0, upBounds.Bottom, size.Width, size.Height - upBounds.Height)

        Dim hoveredPart As ButtonPart = Me.GetHoveredPart()
        Dim isPressed As Boolean = (Control.MouseButtons And MouseButtons.Left) = MouseButtons.Left

        Me.PaintButton(e.Graphics, upBounds, isUp:=True, isHovered:=hoveredPart = ButtonPart.Up, isPressed:=isPressed)
        Me.PaintButton(e.Graphics, downBounds, isUp:=False, isHovered:=hoveredPart = ButtonPart.Down, isPressed:=isPressed)
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the <see cref="Control.MouseMove"/> event of the up/down buttons control.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonsControl_MouseMove(sender As Object, e As MouseEventArgs)
        Dim hoveredPart As ButtonPart = Me.GetHoveredPart()
        If hoveredPart <> Me.lastHoveredPart Then
            Me.lastHoveredPart = hoveredPart
            Me.buttonsControl.Invalidate()
        End If
    End Sub

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Handles the mouse events of the up/down buttons control that change the visual state of the buttons.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub ButtonsControl_MouseStateChanged(sender As Object, e As EventArgs)
        Me.lastHoveredPart = Me.GetHoveredPart()
        Me.buttonsControl.Invalidate()
    End Sub

#End Region

#Region " Private Methods "

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the decimal separator of the current culture, used to resolve ambiguous typed texts 
    ''' (e.g. "1,500" is 1.5 when the decimal separator is the comma, and 1500 otherwise).
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Shared Function GetPreferredDecimalSeparator() As Char
        Dim separator As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
        Return If(separator = ",", ","c, "."c)
    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Finds the internal control that draws the up/down buttons of a <see cref="NumericUpDown"/>.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <exception cref="InvalidOperationException">
    ''' Thrown when the control cannot be found.
    ''' </exception>
    ''' ----------------------------------------------------------------------------------------------------
    Private Shared Function FindButtonsControl(numericUpDown As NumericUpDown) As Control

        ' UpDownBase hosts two child controls: the edit box (derived from TextBox) and the buttons.
        For Each child As Control In numericUpDown.Controls
            If Not TypeOf child Is TextBox Then
                Return child
            End If
        Next

        Throw New InvalidOperationException("The up/down buttons control of the NumericUpDown was not found.")
    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Gets the button part that is currently under the mouse pointer.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Function GetHoveredPart() As ButtonPart

        If Not Me.Enabled OrElse Not Me.buttonsControl.IsHandleCreated Then
            Return ButtonPart.None
        End If

        Dim location As Point = Me.buttonsControl.PointToClient(Control.MousePosition)
        Dim size As Size = Me.buttonsControl.ClientSize

        If location.X < 0 OrElse location.Y < 0 OrElse location.X >= size.Width OrElse location.Y >= size.Height Then
            Return ButtonPart.None
        End If

        Return If(location.Y < size.Height \ 2, ButtonPart.Up, ButtonPart.Down)
    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Paints a flat up or down button.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Private Sub PaintButton(g As Graphics, bounds As Rectangle, isUp As Boolean, isHovered As Boolean, isPressed As Boolean)

        If bounds.Width <= 0 OrElse bounds.Height <= 0 Then
            Return
        End If

        Dim backColor As Color
        If isHovered AndAlso isPressed Then
            backColor = Me.buttonPressedBackColorField
        ElseIf isHovered Then
            backColor = Me.buttonHoverBackColorField
        Else
            backColor = Me.buttonBackColorField
        End If

        Using brush As New SolidBrush(backColor)
            g.FillRectangle(brush, bounds)
        End Using

        ' Chevron arrow.
        Dim halfWidth As Single = Math.Max(3.0F, bounds.Width / 5.0F)
        Dim halfHeight As Single = halfWidth / 2.0F
        Dim centerX As Single = bounds.Left + (bounds.Width / 2.0F)
        Dim centerY As Single = bounds.Top + (bounds.Height / 2.0F)
        Dim direction As Single = If(isUp, 1.0F, -1.0F)

        Dim points As PointF() = {
            New PointF(centerX - halfWidth, centerY + (halfHeight * direction)),
            New PointF(centerX, centerY - (halfHeight * direction)),
            New PointF(centerX + halfWidth, centerY + (halfHeight * direction))
        }

        Dim arrowColor As Color = If(Me.Enabled, Me.arrowColorField, Me.disabledArrowColorField)
        Dim previousSmoothingMode As SmoothingMode = g.SmoothingMode
        g.SmoothingMode = SmoothingMode.AntiAlias
        Using pen As New Pen(arrowColor, Math.Max(1.5F, g.DpiX / 64.0F))
            pen.StartCap = LineCap.Round
            pen.EndCap = LineCap.Round
            pen.LineJoin = LineJoin.Round
            g.DrawLines(pen, points)
        End Using
        g.SmoothingMode = previousSmoothingMode
    End Sub

#End Region

End Class

#End Region
