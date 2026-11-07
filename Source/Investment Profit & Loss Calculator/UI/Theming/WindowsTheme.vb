#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Runtime.InteropServices

Imports Microsoft.Win32

#End Region

#Region " WindowsTheme "

Namespace Theming

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Provides access to the theme settings of Windows and to the dark mode of window title bars.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    Friend NotInheritable Class WindowsTheme

#Region " Constants "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The registry key (under HKEY_CURRENT_USER) that contains the personalization settings of Windows.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const PersonalizeKeyPath As String = "Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The registry value that specifies whether applications must use the light theme (1) or the dark theme (0).
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const AppsUseLightThemeValueName As String = "AppsUseLightTheme"

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The HRESULT returned by <c>DwmSetWindowAttribute</c> when the attribute is not supported by the OS.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Const E_INVALIDARG As Integer = &H80070057

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The section name sent by Windows in WM_SETTINGCHANGE when the color mode (light/dark) changes.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const ImmersiveColorSetSectionName As String = "ImmersiveColorSet"

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The WM_SETTINGCHANGE window message.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Const WM_SETTINGCHANGE As Integer = NativeMethods.WM_SETTINGCHANGE

#End Region

#Region " Constructors "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Prevents a default instance of the <see cref="WindowsTheme"/> class from being created.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private Sub New()
        End Sub

#End Region

#Region " Friend Methods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the user has selected the dark mode for applications in the Windows settings.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the dark mode is enabled for applications;
        ''' otherwise, <see langword="False"/> (including Windows versions without dark mode support).
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Function IsAppDarkModeEnabled() As Boolean

            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(PersonalizeKeyPath, writable:=False)
                ' The key does not exist in Windows versions without dark mode support.
                If key Is Nothing Then
                    Return False
                End If

                Return WindowsTheme.IsDarkModeValue(key.GetValue(AppsUseLightThemeValueName))
            End Using
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Interprets the data of the "AppsUseLightTheme" registry value.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="value">
        ''' The data of the registry value, or <see langword="Nothing"/> if the value does not exist.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the value indicates dark mode; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="InvalidDataException">
        ''' Thrown when the registry value is not a DWORD.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Function IsDarkModeValue(value As Object) As Boolean

            ' The value does not exist in Windows versions without dark mode support.
            If value Is Nothing Then
                Return False
            End If

            If Not TypeOf value Is Integer Then
                Throw New InvalidDataException(
                    $"The registry value ""{AppsUseLightThemeValueName}"" is not a DWORD (type: {value.GetType().Name}).")
            End If

            Return DirectCast(value, Integer) = 0
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Enables or disables the immersive dark mode of the title bar of the specified window.
        ''' <para></para>
        ''' To take effect when the window is shown, this method must be called when the window handle is created
        ''' (e.g. from <see cref="System.Windows.Forms.Control.OnHandleCreated"/>), not from the Load event.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="windowHandle">
        ''' The handle of the window.
        ''' </param>
        '''
        ''' <param name="enabled">
        ''' <see langword="True"/> to enable the dark mode; <see langword="False"/> to disable it.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the attribute was applied;
        ''' <see langword="False"/> if the OS does not support it (Windows versions prior to Windows 10 1809).
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="windowHandle"/> is <see cref="IntPtr.Zero"/>.
        ''' </exception>
        '''
        ''' <exception cref="COMException">
        ''' Thrown when the DWM API fails for a reason other than an unsupported attribute.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Function SetImmersiveDarkMode(windowHandle As IntPtr, enabled As Boolean) As Boolean

            If windowHandle = IntPtr.Zero Then
                Throw New ArgumentException("The window handle cannot be zero.", NameOf(windowHandle))
            End If

            Dim value As Integer = If(enabled, 1, 0)

            Dim hr As Integer = NativeMethods.DwmSetWindowAttribute(windowHandle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, value, Marshal.SizeOf(value))
            If hr = E_INVALIDARG Then
                ' Windows 10 builds prior to 20H1 use the undocumented attribute value 19.
                hr = NativeMethods.DwmSetWindowAttribute(windowHandle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, value, Marshal.SizeOf(value))
            End If

            If hr = E_INVALIDARG Then
                Return False
            End If

            Marshal.ThrowExceptionForHR(hr)
            Return True
        End Function

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Forces the non-client area (title bar and borders) of the specified window to be redrawn.
        ''' <para></para>
        ''' This is required to reflect a title bar dark mode change on a window that is already visible:
        ''' Windows 11 redraws the title bar immediately, but Windows 10 does not redraw it until 
        ''' the activation state of the window changes, so the activation state is toggled and restored.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="windowHandle">
        ''' The handle of the window.
        ''' </param>
        ''' 
        ''' <param name="isActive">
        ''' <see langword="True"/> if the window is currently the active window; otherwise, <see langword="False"/>.
        ''' The title bar is redrawn in this state.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="windowHandle"/> is <see cref="IntPtr.Zero"/>.
        ''' </exception>
        ''' 
        ''' <exception cref="System.ComponentModel.Win32Exception">
        ''' Thrown when the window frame could not be redrawn.
        ''' </exception>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Sub RedrawNonClientArea(windowHandle As IntPtr, isActive As Boolean)

            If windowHandle = IntPtr.Zero Then
                Throw New ArgumentException("The window handle cannot be zero.", NameOf(windowHandle))
            End If

            Const flags As Integer = NativeMethods.SWP_NOSIZE Or NativeMethods.SWP_NOMOVE Or NativeMethods.SWP_NOZORDER Or
                                     NativeMethods.SWP_NOACTIVATE Or NativeMethods.SWP_FRAMECHANGED

            If Not NativeMethods.SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0, flags) Then
                Throw New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error())
            End If

            ' WM_NCACTIVATE only repaints the non-client area in the specified state; it does not change the focus.
            NativeMethods.SendMessage(windowHandle, NativeMethods.WM_NCACTIVATE, If(isActive, IntPtr.Zero, New IntPtr(1)), IntPtr.Zero)
            NativeMethods.SendMessage(windowHandle, NativeMethods.WM_NCACTIVATE, If(isActive, New IntPtr(1), IntPtr.Zero), IntPtr.Zero)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the specified window message notifies a change of the Windows color mode
        ''' (light/dark), that is, a WM_SETTINGCHANGE message with the "ImmersiveColorSet" parameter.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="msg">
        ''' The message identifier.
        ''' </param>
        ''' 
        ''' <param name="lParam">
        ''' The lParam of the message.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <returns>
        ''' <see langword="True"/> if the message notifies a color mode change; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' ----------------------------------------------------------------------------------------------------
        Friend Shared Function IsColorModeChangeMessage(msg As Integer, lParam As IntPtr) As Boolean

            ' A WM_SETTINGCHANGE without a section name (lParam = NULL) is not a color mode change.
            If msg <> NativeMethods.WM_SETTINGCHANGE OrElse lParam = IntPtr.Zero Then
                Return False
            End If

            Return String.Equals(Marshal.PtrToStringUni(lParam), ImmersiveColorSetSectionName, StringComparison.Ordinal)
        End Function

#End Region

#Region " NativeMethods "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Platform invocation methods and constants.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private NotInheritable Class NativeMethods

            ''' <summary>
            ''' DWMWINDOWATTRIBUTE: allows the window frame to be drawn in dark mode colors (Windows 10 20H1 and later).
            ''' </summary>
            Friend Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

            ''' <summary>
            ''' Undocumented DWMWINDOWATTRIBUTE used for the same purpose in Windows 10 1809 to 1909.
            ''' </summary>
            Friend Const DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 As Integer = 19

            ''' <summary>
            ''' SetWindowPos flag: retains the current size.
            ''' </summary>
            Friend Const SWP_NOSIZE As Integer = &H1

            ''' <summary>
            ''' SetWindowPos flag: retains the current position.
            ''' </summary>
            Friend Const SWP_NOMOVE As Integer = &H2

            ''' <summary>
            ''' SetWindowPos flag: retains the current Z order.
            ''' </summary>
            Friend Const SWP_NOZORDER As Integer = &H4

            ''' <summary>
            ''' SetWindowPos flag: does not activate the window.
            ''' </summary>
            Friend Const SWP_NOACTIVATE As Integer = &H10

            ''' <summary>
            ''' SetWindowPos flag: sends a WM_NCCALCSIZE message, so the window frame is recalculated and redrawn.
            ''' </summary>
            Friend Const SWP_FRAMECHANGED As Integer = &H20

            ''' <summary>
            ''' Window message sent when a system-wide setting changes (also known as WM_WININICHANGE).
            ''' </summary>
            Friend Const WM_SETTINGCHANGE As Integer = &H1A

            ''' <summary>
            ''' Window message sent to change the active state of the non-client area.
            ''' </summary>
            Friend Const WM_NCACTIVATE As Integer = &H86

            ''' <summary>
            ''' Prevents a default instance of the <see cref="NativeMethods"/> class from being created.
            ''' </summary>
            Private Sub New()
            End Sub

            ''' <summary>
            ''' Sets the value of a Desktop Window Manager (DWM) non-client rendering attribute for a window.
            ''' </summary>
            <DllImport("dwmapi.dll", ExactSpelling:=True)>
            Friend Shared Function DwmSetWindowAttribute(hwnd As IntPtr, attribute As Integer, ByRef value As Integer, valueSize As Integer) As Integer
            End Function

            ''' <summary>
            ''' Changes the size, position, and Z order of a window.
            ''' </summary>
            <DllImport("user32.dll", ExactSpelling:=True, SetLastError:=True)>
            Friend Shared Function SetWindowPos(hwnd As IntPtr, hwndInsertAfter As IntPtr, x As Integer, y As Integer, cx As Integer, cy As Integer, flags As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
            End Function

            ''' <summary>
            ''' Sends the specified message to a window and waits until the window procedure processes it.
            ''' </summary>
            <DllImport("user32.dll", CharSet:=CharSet.Unicode, EntryPoint:="SendMessageW", ExactSpelling:=True)>
            Friend Shared Function SendMessage(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
            End Function

        End Class

#End Region

    End Class

End Namespace

#End Region
