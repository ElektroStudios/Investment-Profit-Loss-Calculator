#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Drawing

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Theming

#End Region

#Region " ThemePaletteTests "

Namespace Theming

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Unit tests for the <see cref="ThemePalette"/> class.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <TestClass>
    Public NotInheritable Class ThemePaletteTests

#Region " Constants "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The minimum contrast ratio for normal text required by WCAG 2.x level AA.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Const MinimumTextContrastRatio As Double = 4.5

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The minimum contrast ratio for user interface components (e.g. focus indicators) required by WCAG 2.1.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Const MinimumComponentContrastRatio As Double = 3.0

#End Region

#Region " Private Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Calculates the WCAG 2.x relative luminance of a color.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function GetRelativeLuminance(color As Color) As Double
            Dim linearize As Func(Of Byte, Double) =
                Function(channel As Byte) As Double
                    Dim value As Double = channel / 255.0
                    Return If(value <= 0.03928, value / 12.92, Math.Pow((value + 0.055) / 1.055, 2.4))
                End Function

            Return (0.2126 * linearize(color.R)) + (0.7152 * linearize(color.G)) + (0.0722 * linearize(color.B))
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Calculates the WCAG 2.x contrast ratio between two colors.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function GetContrastRatio(color1 As Color, color2 As Color) As Double
            Dim luminance1 As Double = GetRelativeLuminance(color1)
            Dim luminance2 As Double = GetRelativeLuminance(color2)
            Return (Math.Max(luminance1, luminance2) + 0.05) / (Math.Min(luminance1, luminance2) + 0.05)
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Asserts that all the specified foreground/background pairs meet the specified contrast ratio.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Sub AssertContrast(pairs As IEnumerable(Of (Name As String, Foreground As Color, Background As Color)), minimumRatio As Double)
            Dim failures As New List(Of String)

            For Each pair As (Name As String, Foreground As Color, Background As Color) In pairs
                Dim ratio As Double = GetContrastRatio(pair.Foreground, pair.Background)
                If ratio < minimumRatio Then
                    failures.Add($"{pair.Name}: {ratio:0.00}")
                End If
            Next

            Assert.IsEmpty(failures, $"Contrast ratio below {minimumRatio}: {String.Join(", ", failures)}")
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the palette for a data row.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Function GetPalette(isDark As Boolean) As ThemePalette
            Return If(isDark, ThemePalette.Dark, ThemePalette.Light)
        End Function

#End Region

#Region " Tests "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that <see cref="ThemePalette.FromDarkMode"/> returns the expected palette.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub FromDarkMode_ReturnsExpectedPalette()
            Assert.AreSame(ThemePalette.Dark, ThemePalette.FromDarkMode(True))
            Assert.AreSame(ThemePalette.Light, ThemePalette.FromDarkMode(False))
            Assert.IsTrue(ThemePalette.Dark.IsDark)
            Assert.IsFalse(ThemePalette.Light.IsDark)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the dark palette has dark backgrounds and the light palette has light backgrounds.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub Backgrounds_MatchThemeMode()
            Assert.IsTrue(GetRelativeLuminance(ThemePalette.Dark.WindowBackColor) < 0.1)
            Assert.IsTrue(GetRelativeLuminance(ThemePalette.Light.WindowBackColor) > 0.8)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that all the text colors meet the WCAG AA contrast ratio against the backgrounds they are drawn on.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(True, DisplayName:="Dark")>
        <DataRow(False, DisplayName:="Light")>
        Public Sub TextColors_MeetWcagAaContrast(isDark As Boolean)
            Dim p As ThemePalette = GetPalette(isDark)

            AssertContrast({
                ("Text on window", p.TextColor, p.WindowBackColor),
                ("Secondary text on window", p.SecondaryTextColor, p.WindowBackColor),
                ("Text on input", p.TextColor, p.InputBackColor),
                ("Text on button", p.TextColor, p.ButtonBackColor),
                ("Text on hovered button", p.TextColor, p.ButtonHoverBackColor),
                ("Text on pressed button", p.TextColor, p.ButtonPressedBackColor),
                ("Text on card", p.TextColor, p.CardBackColor),
                ("Secondary text on card", p.SecondaryTextColor, p.CardBackColor),
                ("Profit on card", p.ProfitColor, p.CardBackColor),
                ("Loss on card", p.LossColor, p.CardBackColor),
                ("Neutral result on card", p.NeutralResultColor, p.CardBackColor),
                ("Accent text on accent", p.AccentTextColor, p.AccentColor),
                ("Error message on card", p.ErrorColor, p.CardBackColor)
            }, MinimumTextContrastRatio)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the focus and error indicators meet the WCAG contrast ratio for user interface components.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(True, DisplayName:="Dark")>
        <DataRow(False, DisplayName:="Light")>
        Public Sub BorderIndicators_MeetWcagComponentContrast(isDark As Boolean)
            Dim p As ThemePalette = GetPalette(isDark)

            AssertContrast({
                ("Focus border on window", p.FocusBorderColor, p.WindowBackColor),
                ("Focus border on input", p.FocusBorderColor, p.InputBackColor),
                ("Error border on window", p.ErrorColor, p.WindowBackColor),
                ("Error border on input", p.ErrorColor, p.InputBackColor)
            }, MinimumComponentContrastRatio)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that profit and loss colors are distinguishable from each other and from the neutral color.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(True, DisplayName:="Dark")>
        <DataRow(False, DisplayName:="Light")>
        Public Sub ResultColors_AreDistinct(isDark As Boolean)
            Dim p As ThemePalette = GetPalette(isDark)

            Assert.AreNotEqual(p.ProfitColor, p.LossColor)
            Assert.AreNotEqual(p.ProfitColor, p.NeutralResultColor)
            Assert.AreNotEqual(p.LossColor, p.NeutralResultColor)
            Assert.IsTrue(p.ProfitColor.G > p.ProfitColor.R, "Profit color must be greenish.")
            Assert.IsTrue(p.LossColor.R > p.LossColor.G, "Loss color must be reddish.")
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that all the colors of the palettes are opaque, as transparent colors are not supported by most controls.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow(True, DisplayName:="Dark")>
        <DataRow(False, DisplayName:="Light")>
        Public Sub AllColors_AreOpaque(isDark As Boolean)
            Dim p As ThemePalette = GetPalette(isDark)
            Dim failures As New List(Of String)

            For Each prop As Reflection.PropertyInfo In GetType(ThemePalette).GetProperties()
                If prop.PropertyType Is GetType(Color) AndAlso prop.GetIndexParameters().Length = 0 Then
                    Dim color As Color = DirectCast(prop.GetValue(p), Color)
                    If color.A <> 255 OrElse color.IsEmpty Then
                        failures.Add(prop.Name)
                    End If
                End If
            Next

            Assert.IsEmpty(failures, $"Non-opaque or empty colors: {String.Join(", ", failures)}")
        End Sub

#End Region

    End Class

End Namespace

#End Region
