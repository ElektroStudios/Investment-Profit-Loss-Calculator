#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

#End Region

#Region " WindowCapture "

Namespace Theming

    ''' <summary>
    ''' Captures the image of a window exactly as it is displayed on screen,
    ''' including the non-client area rendered by the Desktop Window Manager (e.g. a dark title bar).
    ''' <para></para>
    ''' <see cref="System.Windows.Forms.Control.DrawToBitmap"/> is not used because it draws the non-client area
    ''' with the classic (light) theme, ignoring the DWM attributes of the window.
    ''' </summary>
    Friend NotInheritable Class WindowCapture

#Region " Constants "

        ''' <summary>
        ''' The HRESULT returned by DWM functions when desktop composition is disabled (Windows 7 basic theme).
        ''' </summary>
        Private Const DWM_E_COMPOSITIONDISABLED As Integer = &H80263001

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="WindowCapture"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Friend Methods "

        ''' <summary>
        ''' Captures the visible bounds of the specified window from the screen.
        ''' <para></para>
        ''' The window must be visible and not covered by other windows (e.g. capture it before showing a dialog).
        ''' The image has the physical resolution of the screen, even if the application is not DPI aware.
        ''' </summary>
        ''' <param name="windowHandle">
        ''' The handle of the window to capture.
        ''' </param>
        ''' <returns>
        ''' A 24-bit RGB <see cref="Bitmap"/> with the image of the window. The caller must dispose it.
        ''' </returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="windowHandle"/> is <see cref="IntPtr.Zero"/>.
        ''' </exception>
        '''
        ''' <exception cref="Win32Exception">
        ''' Thrown when the window bounds cannot be retrieved.
        ''' </exception>
        Friend Shared Function CaptureFromScreen(windowHandle As IntPtr) As Bitmap

            If windowHandle = IntPtr.Zero Then
                Throw New ArgumentException("The window handle cannot be zero.", NameOf(windowHandle))
            End If

            ' Switches the thread to per-monitor DPI awareness during the capture, so the window bounds,
            ' the DWM bounds and the screen pixels all use physical coordinates (no DPI virtualization).
            Dim previousContext As IntPtr = IntPtr.Zero
            If WindowCapture.IsThreadDpiAwarenessSupported() Then
                previousContext = NativeMethods.SetThreadDpiAwarenessContext(NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE)
            End If

            Try
                Dim bounds As Rectangle = WindowCapture.GetVisibleBounds(windowHandle)
                If bounds.Width <= 0 OrElse bounds.Height <= 0 Then
                    Throw New InvalidOperationException($"The window has empty bounds: {bounds}.")
                End If

                Dim bitmap As New Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb)
                Try
                    Using g As Graphics = Graphics.FromImage(bitmap)
                        g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size, CopyPixelOperation.SourceCopy)
                    End Using
                    Return bitmap

                Catch
                    bitmap.Dispose()
                    Throw
                End Try

            Finally
                If previousContext <> IntPtr.Zero Then
                    NativeMethods.SetThreadDpiAwarenessContext(previousContext)
                End If
            End Try
        End Function

        ''' <summary>
        ''' Gets the visible bounds of the specified window, in screen coordinates,
        ''' excluding the invisible resize borders that Windows 10 and later add around windows.
        ''' </summary>
        ''' <param name="windowHandle">
        ''' The handle of the window.
        ''' </param>
        ''' <returns>
        ''' The visible bounds of the window.
        ''' </returns>
        ''' <exception cref="Win32Exception">
        ''' Thrown when the window bounds cannot be retrieved.
        ''' </exception>
        Friend Shared Function GetVisibleBounds(windowHandle As IntPtr) As Rectangle

            Dim rect As NativeMethods.RECT
            Dim hr As Integer = NativeMethods.DwmGetWindowAttribute(windowHandle, NativeMethods.DWMWA_EXTENDED_FRAME_BOUNDS, rect, Marshal.SizeOf(GetType(NativeMethods.RECT)))

            If hr = DWM_E_COMPOSITIONDISABLED Then
                ' Without desktop composition there are no invisible borders: the window rectangle is the visible one.
                If Not NativeMethods.GetWindowRect(windowHandle, rect) Then
                    Throw New Win32Exception(Marshal.GetLastWin32Error())
                End If
            Else
                Marshal.ThrowExceptionForHR(hr)
            End If

            Return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom)
        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Determines whether the OS supports changing the DPI awareness of a thread (Windows 10 1607 and later).
        ''' </summary>
        Private Shared Function IsThreadDpiAwarenessSupported() As Boolean
            Dim user32 As IntPtr = NativeMethods.GetModuleHandle("user32.dll")
            Return user32 <> IntPtr.Zero AndAlso NativeMethods.GetProcAddress(user32, "SetThreadDpiAwarenessContext") <> IntPtr.Zero
        End Function

#End Region

#Region " NativeMethods "

        ''' <summary>
        ''' Platform invocation methods, structures and constants.
        ''' </summary>
        Private NotInheritable Class NativeMethods

            ''' <summary>
            ''' DWMWINDOWATTRIBUTE: retrieves the extended frame bounds rectangle (the visible window bounds), in screen space.
            ''' </summary>
            Friend Const DWMWA_EXTENDED_FRAME_BOUNDS As Integer = 9

            ''' <summary>
            ''' DPI_AWARENESS_CONTEXT: per-monitor DPI awareness.
            ''' </summary>
            Friend Shared ReadOnly DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE As New IntPtr(-3)

            ''' <summary>
            ''' Defines a rectangle by the coordinates of its upper-left and lower-right corners.
            ''' </summary>
            <StructLayout(LayoutKind.Sequential)>
            Friend Structure RECT

                ''' <summary>
                ''' The x-coordinate of the upper-left corner.
                ''' </summary>
                Friend Left As Integer

                ''' <summary>
                ''' The y-coordinate of the upper-left corner.
                ''' </summary>
                Friend Top As Integer

                ''' <summary>
                ''' The x-coordinate of the lower-right corner.
                ''' </summary>
                Friend Right As Integer

                ''' <summary>
                ''' The y-coordinate of the lower-right corner.
                ''' </summary>
                Friend Bottom As Integer

            End Structure

            ''' <summary>
            ''' Prevents a default instance of the <see cref="NativeMethods"/> class from being created.
            ''' </summary>
            Private Sub New()
            End Sub

            ''' <summary>
            ''' Retrieves the current value of a Desktop Window Manager (DWM) attribute of a window.
            ''' </summary>
            <DllImport("dwmapi.dll", ExactSpelling:=True)>
            Friend Shared Function DwmGetWindowAttribute(hwnd As IntPtr, attribute As Integer, ByRef value As RECT, valueSize As Integer) As Integer
            End Function

            ''' <summary>
            ''' Retrieves the bounding rectangle of a window, in screen coordinates.
            ''' </summary>
            <DllImport("user32.dll", ExactSpelling:=True, SetLastError:=True)>
            Friend Shared Function GetWindowRect(hwnd As IntPtr, ByRef rect As RECT) As <MarshalAs(UnmanagedType.Bool)> Boolean
            End Function

            ''' <summary>
            ''' Sets the DPI awareness of the current thread, returning the previous DPI awareness context.
            ''' </summary>
            <DllImport("user32.dll", ExactSpelling:=True)>
            Friend Shared Function SetThreadDpiAwarenessContext(dpiContext As IntPtr) As IntPtr
            End Function

            ''' <summary>
            ''' Retrieves a module handle for the specified module, which must have been loaded by the calling process.
            ''' </summary>
            <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, EntryPoint:="GetModuleHandleW", ExactSpelling:=True)>
            Friend Shared Function GetModuleHandle(moduleName As String) As IntPtr
            End Function

            ''' <summary>
            ''' Retrieves the address of an exported function from the specified module.
            ''' </summary>
            <DllImport("kernel32.dll", CharSet:=CharSet.Ansi, ExactSpelling:=True, BestFitMapping:=False, ThrowOnUnmappableChar:=True)>
            Friend Shared Function GetProcAddress(moduleHandle As IntPtr, procedureName As String) As IntPtr
            End Function

        End Class

#End Region

    End Class

End Namespace

#End Region
