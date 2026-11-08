#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

#End Region

#Region " ThemePalette "

Namespace Theming

    ''' <summary>
    ''' Defines the colors of a visual theme (dark or light) of the application.
    ''' <para></para>
    ''' The colors are inspired by the Windows 11 Fluent design, and all the text colors
    ''' meet the WCAG 2.x AA contrast ratio (4.5:1) against the backgrounds they are drawn on.
    ''' </summary>
    Public NotInheritable Class ThemePalette

#Region " Fields "

        ''' <summary>
        ''' The shared instance returned by <see cref="ThemePalette.Dark"/>.
        ''' </summary>
        Private Shared ReadOnly DarkInstance As New ThemePalette(isDark:=True)

        ''' <summary>
        ''' The shared instance returned by <see cref="ThemePalette.Light"/>.
        ''' </summary>
        Private Shared ReadOnly LightInstance As New ThemePalette(isDark:=False)

#End Region

#Region " Shared Properties "

        ''' <summary>
        ''' Gets the dark theme palette.
        ''' </summary>
        Public Shared ReadOnly Property Dark As ThemePalette
            Get
                Return ThemePalette.DarkInstance
            End Get
        End Property

        ''' <summary>
        ''' Gets the light theme palette.
        ''' </summary>
        Public Shared ReadOnly Property Light As ThemePalette
            Get
                Return ThemePalette.LightInstance
            End Get
        End Property

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets a value indicating whether this is a dark theme palette.
        ''' </summary>
        Public ReadOnly Property IsDark As Boolean

        ''' <summary>
        ''' Gets the background color of the window.
        ''' </summary>
        Public ReadOnly Property WindowBackColor As Color

        ''' <summary>
        ''' Gets the background color of cards (grouping surfaces such as the result panel).
        ''' </summary>
        Public ReadOnly Property CardBackColor As Color

        ''' <summary>
        ''' Gets the border color of cards.
        ''' </summary>
        Public ReadOnly Property CardBorderColor As Color

        ''' <summary>
        ''' Gets the background color of input fields.
        ''' </summary>
        Public ReadOnly Property InputBackColor As Color

        ''' <summary>
        ''' Gets the border color of input fields.
        ''' </summary>
        Public ReadOnly Property InputBorderColor As Color

        ''' <summary>
        ''' Gets the border color of disabled input fields.
        ''' </summary>
        Public ReadOnly Property InputDisabledBorderColor As Color

        ''' <summary>
        ''' Gets the border color of the focused input field.
        ''' </summary>
        Public ReadOnly Property FocusBorderColor As Color

        ''' <summary>
        ''' Gets the color of errors: the border of input fields with invalid values and the error messages.
        ''' </summary>
        Public ReadOnly Property ErrorColor As Color

        ''' <summary>
        ''' Gets the primary text color.
        ''' </summary>
        Public ReadOnly Property TextColor As Color

        ''' <summary>
        ''' Gets the secondary text color, used for captions.
        ''' </summary>
        Public ReadOnly Property SecondaryTextColor As Color

        ''' <summary>
        ''' Gets the color of disabled text and input hints.
        ''' </summary>
        Public ReadOnly Property DisabledTextColor As Color

        ''' <summary>
        ''' Gets the accent color, used as background of selected elements.
        ''' </summary>
        Public ReadOnly Property AccentColor As Color

        ''' <summary>
        ''' Gets the color of text drawn on <see cref="ThemePalette.AccentColor"/>.
        ''' </summary>
        Public ReadOnly Property AccentTextColor As Color

        ''' <summary>
        ''' Gets the background color of buttons.
        ''' </summary>
        Public ReadOnly Property ButtonBackColor As Color

        ''' <summary>
        ''' Gets the background color of buttons when the mouse is over them.
        ''' </summary>
        Public ReadOnly Property ButtonHoverBackColor As Color

        ''' <summary>
        ''' Gets the background color of buttons when they are pressed.
        ''' </summary>
        Public ReadOnly Property ButtonPressedBackColor As Color

        ''' <summary>
        ''' Gets the border color of buttons.
        ''' </summary>
        Public ReadOnly Property ButtonBorderColor As Color

        ''' <summary>
        ''' Gets the text color of a profit result.
        ''' </summary>
        Public ReadOnly Property ProfitColor As Color

        ''' <summary>
        ''' Gets the text color of a loss result.
        ''' </summary>
        Public ReadOnly Property LossColor As Color

        ''' <summary>
        ''' Gets the text color of a break-even or not available result.
        ''' </summary>
        Public ReadOnly Property NeutralResultColor As Color

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ThemePalette"/> class.
        ''' </summary>
        ''' <param name="isDark">
        ''' <see langword="True"/> to create the dark theme palette;
        ''' <see langword="False"/> to create the light theme palette.
        ''' </param>
        Private Sub New(isDark As Boolean)

            Me.IsDark = isDark

            If isDark Then
                Me.WindowBackColor = Color.FromArgb(32, 32, 32)
                Me.CardBackColor = Color.FromArgb(43, 43, 43)
                Me.CardBorderColor = Color.FromArgb(58, 58, 58)
                Me.InputBackColor = Color.FromArgb(45, 45, 45)
                Me.InputBorderColor = Color.FromArgb(80, 80, 80)
                Me.InputDisabledBorderColor = Color.FromArgb(58, 58, 58)
                Me.FocusBorderColor = Color.FromArgb(76, 194, 255)
                Me.TextColor = Color.FromArgb(243, 243, 243)
                Me.SecondaryTextColor = Color.FromArgb(184, 184, 184)
                Me.DisabledTextColor = Color.FromArgb(120, 120, 120)
                Me.AccentColor = Color.FromArgb(0, 103, 192)
                Me.AccentTextColor = Color.FromArgb(255, 255, 255)
                Me.ButtonBackColor = Color.FromArgb(55, 55, 55)
                Me.ButtonHoverBackColor = Color.FromArgb(65, 65, 65)
                Me.ButtonPressedBackColor = Color.FromArgb(48, 48, 48)
                Me.ButtonBorderColor = Color.FromArgb(80, 80, 80)
                Me.ProfitColor = Color.FromArgb(108, 203, 95)
                Me.LossColor = Color.FromArgb(255, 153, 164)
                Me.NeutralResultColor = Color.FromArgb(184, 184, 184)
                Me.ErrorColor = Color.FromArgb(255, 153, 164)

            Else
                Me.WindowBackColor = Color.FromArgb(243, 243, 243)
                Me.CardBackColor = Color.FromArgb(251, 251, 251)
                Me.CardBorderColor = Color.FromArgb(229, 229, 229)
                Me.InputBackColor = Color.FromArgb(255, 255, 255)
                Me.InputBorderColor = Color.FromArgb(190, 190, 190)
                Me.InputDisabledBorderColor = Color.FromArgb(225, 225, 225)
                Me.FocusBorderColor = Color.FromArgb(0, 103, 192)
                Me.TextColor = Color.FromArgb(26, 26, 26)
                Me.SecondaryTextColor = Color.FromArgb(92, 92, 92)
                Me.DisabledTextColor = Color.FromArgb(160, 160, 160)
                Me.AccentColor = Color.FromArgb(0, 103, 192)
                Me.AccentTextColor = Color.FromArgb(255, 255, 255)
                Me.ButtonBackColor = Color.FromArgb(251, 251, 251)
                Me.ButtonHoverBackColor = Color.FromArgb(240, 240, 240)
                Me.ButtonPressedBackColor = Color.FromArgb(229, 229, 229)
                Me.ButtonBorderColor = Color.FromArgb(200, 200, 200)
                Me.ProfitColor = Color.FromArgb(15, 123, 15)
                Me.LossColor = Color.FromArgb(196, 43, 28)
                Me.NeutralResultColor = Color.FromArgb(92, 92, 92)
                Me.ErrorColor = Color.FromArgb(196, 43, 28)

            End If
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Gets the palette that corresponds to the specified theme mode.
        ''' </summary>
        ''' <param name="isDarkMode">
        ''' <see langword="True"/> to get the dark theme palette;
        ''' <see langword="False"/> to get the light theme palette.
        ''' </param>
        ''' <returns>
        ''' The <see cref="ThemePalette"/> that corresponds to the specified theme mode.
        ''' </returns>
        Public Shared Function FromDarkMode(isDarkMode As Boolean) As ThemePalette
            Return If(isDarkMode, ThemePalette.Dark, ThemePalette.Light)
        End Function

#End Region

    End Class

End Namespace

#End Region
