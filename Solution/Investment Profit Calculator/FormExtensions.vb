
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Module FormExtensions

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Retrieves the next control of the specified type forward or back in the tab order of child controls.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <typeparam name="T">
    ''' The type of the <see cref="System.Windows.Forms.Control"/> to search.
    ''' </typeparam>
    ''' 
    ''' <param name="form">
    ''' The <see cref="System.Windows.Forms.Form"/> to start the search with.
    ''' </param>
    ''' 
    ''' <param name="forward">
    ''' True to search forward in the tab order; False to search backward.
    ''' </param>
    ''' 
    ''' <param name="tabStopOnly">
    ''' True to ignore the controls with the <see cref="System.Windows.Forms.Control.TabStop"/> property set to false; 
    ''' otherwise, False.
    ''' </param>
    ''' 
    ''' <param name="wrap">
    ''' True to continue searching from the first control in the tab order after the last control has been reached; 
    ''' otherwise, False.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <returns>
    ''' The next <see cref="System.Windows.Forms.Control"/> in the tab order.
    ''' </returns>
    ''' ----------------------------------------------------------------------------------------------------
    <DebuggerStepThrough>
    <Extension>
    <EditorBrowsable(EditorBrowsableState.Always)>
    Public Function GetNextControl(Of T As System.Windows.Forms.Control)(form As System.Windows.Forms.Form, forward As Boolean, tabStopOnly As Boolean, wrap As Boolean) As System.Windows.Forms.Control

        Return FormExtensions.GetNextControl(Of T)(form, forward, tabStopOnly, wrap, form.Controls)

    End Function

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Retrieves the next control of the specified type forward or back in the tab order of child controls.
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <typeparam name="T">
    ''' The type of the <see cref="System.Windows.Forms.Control"/> to search.
    ''' </typeparam>
    ''' 
    ''' <param name="form">
    ''' The <see cref="System.Windows.Forms.Form"/> to start the search with.
    ''' </param>
    ''' 
    ''' <param name="forward">
    ''' True to search forward in the tab order; False to search backward.
    ''' </param>
    ''' 
    ''' <param name="tabStopOnly">
    ''' True to ignore the controls with the <see cref="System.Windows.Forms.Control.TabStop"/> property set to false; 
    ''' otherwise, False.
    ''' </param>
    ''' 
    ''' <param name="wrap">
    ''' True to continue searching from the first control in the tab order after the last control has been reached; 
    ''' otherwise, False.
    ''' </param>
    ''' 
    ''' <param name="controlCollection">
    ''' The <see cref="System.Windows.Forms.Form.ControlCollection"/> to search within.
    ''' </param>
    ''' ----------------------------------------------------------------------------------------------------
    ''' <returns>
    ''' The next <see cref="System.Windows.Forms.Control"/> in the tab order.
    ''' </returns>
    ''' ----------------------------------------------------------------------------------------------------
    <DebuggerStepThrough>
    <Extension>
    <EditorBrowsable(EditorBrowsableState.Always)>
    Public Function GetNextControl(Of T As System.Windows.Forms.Control)(form As System.Windows.Forms.Form, forward As Boolean, tabStopOnly As Boolean, wrap As Boolean, controlCollection As System.Windows.Forms.Control.ControlCollection) As System.Windows.Forms.Control

        Dim defaultControl As T = DirectCast(form.ActiveControl, T)
        If controlCollection Is Nothing Then
            Return defaultControl
        End If

        Dim startTabIndex As Integer = form.ActiveControl.TabIndex
        Dim nextCtrl As T = Nothing
        Dim firstCtrl As T = Nothing
        Dim lastCtrl As T = Nothing

        Dim controls As List(Of T)
        If tabStopOnly Then
            controls = (From c As T In controlCollection.OfType(Of T) Order By c.TabIndex Where c.TabStop).ToList()
        Else
            controls = (From c As T In controlCollection.OfType(Of T) Order By c.TabIndex).ToList()
        End If

        If Not controls.Any() Then
            Return defaultControl
        End If

        Select Case wrap
            Case True
                If forward Then
                    nextCtrl = (From c As T In controls Where c.TabIndex > startTabIndex).FirstOrDefault()
                    If (nextCtrl Is Nothing) Then
                        nextCtrl = controls.DefaultIfEmpty(defaultControl).FirstOrDefault()
                    End If
                Else
                    nextCtrl = (From c As T In controls Where c.TabIndex < startTabIndex).Reverse().FirstOrDefault()
                    If (nextCtrl Is Nothing) Then
                        nextCtrl = controls.DefaultIfEmpty(defaultControl).LastOrDefault()
                    End If
                End If

            Case Else
                If forward Then
                    nextCtrl = (From c As T In controls Where c.TabIndex > startTabIndex).DefaultIfEmpty(defaultControl).FirstOrDefault()
                Else
                    nextCtrl = (From c As T In controls Where c.TabIndex < startTabIndex).Reverse().DefaultIfEmpty(defaultControl).FirstOrDefault()
                End If

        End Select

        Return nextCtrl

    End Function

End Module
