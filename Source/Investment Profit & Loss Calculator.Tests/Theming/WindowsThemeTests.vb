#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Win32

Imports Theming

#End Region

#Region " WindowsThemeTests "

Namespace Theming

    ''' <summary>
    ''' Unit tests for the <see cref="WindowsTheme"/> class.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class WindowsThemeTests

#Region " IsDarkModeValue "

        ''' <summary>
        ''' Verifies the interpretation of the "AppsUseLightTheme" DWORD values.
        ''' </summary>
        <TestMethod>
        <DataRow(0, True, DisplayName:="0 = dark mode")>
        <DataRow(1, False, DisplayName:="1 = light mode")>
        Public Sub IsDarkModeValue_Dword_ReturnsExpected(value As Integer, expected As Boolean)
            Assert.AreEqual(expected, WindowsTheme.IsDarkModeValue(value))
        End Sub

        ''' <summary>
        ''' Verifies that a missing value (Windows versions without dark mode) means light mode.
        ''' </summary>
        <TestMethod>
        Public Sub IsDarkModeValue_MissingValue_ReturnsFalse()
            Assert.IsFalse(WindowsTheme.IsDarkModeValue(Nothing))
        End Sub

        ''' <summary>
        ''' Verifies that a value of an unexpected registry type throws <see cref="InvalidDataException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub IsDarkModeValue_UnexpectedType_Throws()
            Assert.ThrowsExactly(Of InvalidDataException)(Function() WindowsTheme.IsDarkModeValue("0"))
            Assert.ThrowsExactly(Of InvalidDataException)(Function() WindowsTheme.IsDarkModeValue(0L))
            Assert.ThrowsExactly(Of InvalidDataException)(Function() WindowsTheme.IsDarkModeValue(New Byte() {0}))
        End Sub

#End Region

#Region " IsColorModeChangeMessage "

        ''' <summary>
        ''' Verifies the detection of the WM_SETTINGCHANGE "ImmersiveColorSet" message.
        ''' </summary>
        <TestMethod>
        <DataRow(&H1A, "ImmersiveColorSet", True, DisplayName:="Color mode change")>
        <DataRow(&H1A, "Policy", False, DisplayName:="Other setting section")>
        <DataRow(&H1A, "immersivecolorset", False, DisplayName:="Different casing")>
        <DataRow(&HF, "ImmersiveColorSet", False, DisplayName:="Other message")>
        Public Sub IsColorModeChangeMessage_ReturnsExpected(msg As Integer, section As String, expected As Boolean)
            Dim lParam As IntPtr = Marshal.StringToHGlobalUni(section)
            Try
                Assert.AreEqual(expected, WindowsTheme.IsColorModeChangeMessage(msg, lParam))
            Finally
                Marshal.FreeHGlobal(lParam)
            End Try
        End Sub

        ''' <summary>
        ''' Verifies that a WM_SETTINGCHANGE without a section name is not a color mode change.
        ''' </summary>
        <TestMethod>
        Public Sub IsColorModeChangeMessage_NullSection_ReturnsFalse()
            Assert.IsFalse(WindowsTheme.IsColorModeChangeMessage(WindowsTheme.WM_SETTINGCHANGE, IntPtr.Zero))
        End Sub

#End Region

#Region " Argument Validation "

        ''' <summary>
        ''' Verifies that a null window handle throws <see cref="ArgumentException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub WindowMethods_ZeroHandle_Throw()
            Assert.ThrowsExactly(Of ArgumentException)(Function() WindowsTheme.SetImmersiveDarkMode(IntPtr.Zero, True))
            Assert.ThrowsExactly(Of ArgumentException)(Sub() WindowsTheme.RedrawNonClientArea(IntPtr.Zero, isActive:=True))
        End Sub

#End Region

#Region " Windows Integration "

        ''' <summary>
        ''' Verifies that <see cref="WindowsTheme.IsAppDarkModeEnabled"/> reflects the current Windows setting.
        ''' </summary>
        <TestMethod>
        <TestCategory("UI")>
        Public Sub IsAppDarkModeEnabled_ReflectsRegistry()
            Dim expected As Boolean
            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(WindowsTheme.PersonalizeKeyPath)
                expected = key IsNot Nothing AndAlso
                           TypeOf key.GetValue(WindowsTheme.AppsUseLightThemeValueName) Is Integer AndAlso
                           DirectCast(key.GetValue(WindowsTheme.AppsUseLightThemeValueName), Integer) = 0
            End Using

            Assert.AreEqual(expected, WindowsTheme.IsAppDarkModeEnabled())
        End Sub

        ''' <summary>
        ''' Verifies that the title bar dark mode can be toggled on a real window without errors.
        ''' </summary>
        <STATestMethod>
        <TestCategory("UI")>
        Public Sub SetImmersiveDarkMode_RealWindow_DoesNotThrow()
            Using form As New Form()
                Dim handle As IntPtr = form.Handle

                Dim supported As Boolean = WindowsTheme.SetImmersiveDarkMode(handle, True)
                Assert.AreEqual(supported, WindowsTheme.SetImmersiveDarkMode(handle, False))

                WindowsTheme.RedrawNonClientArea(handle, isActive:=False)
                WindowsTheme.RedrawNonClientArea(handle, isActive:=True)
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
