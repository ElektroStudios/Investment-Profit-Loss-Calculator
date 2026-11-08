#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization
Imports System.Threading

#End Region

#Region " CultureScope "

Namespace Helpers

    ''' <summary>
    ''' Temporarily changes the culture of the current thread,
    ''' restoring the original culture when the instance is disposed.
    ''' </summary>
    Friend NotInheritable Class CultureScope : Implements IDisposable

#Region " Fields "

        ''' <summary>
        ''' The original culture of the current thread.
        ''' </summary>
        Private ReadOnly originalCulture As CultureInfo

        ''' <summary>
        ''' The original UI culture of the current thread.
        ''' </summary>
        Private ReadOnly originalUICulture As CultureInfo

        ''' <summary>
        ''' Indicates whether this instance has been disposed.
        ''' </summary>
        Private isDisposed As Boolean

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="CultureScope"/> class.
        ''' </summary>
        ''' <param name="cultureName">
        ''' The name of the culture to apply to the current thread (e.g. "de-DE").
        ''' </param>
        Friend Sub New(cultureName As String)
            Me.New(CultureInfo.GetCultureInfo(cultureName))
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="CultureScope"/> class.
        ''' </summary>
        ''' <param name="culture">
        ''' The culture to apply to the current thread.
        ''' </param>
        Friend Sub New(culture As CultureInfo)
            Me.originalCulture = Thread.CurrentThread.CurrentCulture
            Me.originalUICulture = Thread.CurrentThread.CurrentUICulture

            Thread.CurrentThread.CurrentCulture = culture
            Thread.CurrentThread.CurrentUICulture = culture
        End Sub

#End Region

#Region " IDisposable Implementation "

        ''' <summary>
        ''' Restores the original culture of the current thread.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If Me.isDisposed Then
                Return
            End If

            Thread.CurrentThread.CurrentCulture = Me.originalCulture
            Thread.CurrentThread.CurrentUICulture = Me.originalUICulture
            Me.isDisposed = True
        End Sub

#End Region

    End Class

End Namespace

#End Region
