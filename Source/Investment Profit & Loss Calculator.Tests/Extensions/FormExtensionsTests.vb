#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Windows.Forms

Imports FormExtensions

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " FormExtensionsTests "

Namespace Extensions

    ''' <summary>
    ''' Unit tests for the <see cref="Global.FormExtensions.GetNextControl"/> extension method.
    ''' <para></para>
    ''' A window is shown off-screen to allow setting its active control,
    ''' so these tests require an interactive Windows session.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class FormExtensionsTests

#Region " Fields "

        ''' <summary>
        ''' The window that hosts the controls.
        ''' </summary>
        Private form As Form

        ''' <summary>
        ''' The first text box in the tab order.
        ''' </summary>
        Private textBox0 As TextBox

        ''' <summary>
        ''' The second text box in the tab order.
        ''' </summary>
        Private textBox1 As TextBox

        ''' <summary>
        ''' The third text box in the tab order.
        ''' </summary>
        Private textBox2 As TextBox

        ''' <summary>
        ''' A button placed between the text boxes in the tab order, which must be ignored by type filtering.
        ''' </summary>
        Private button As Button

#End Region

#Region " Test Initialization / Cleanup "

        ''' <summary>
        ''' Creates and shows the window and its controls.
        ''' <para></para>
        ''' The controls are added in reverse order to verify that the tab order, not the collection order, is used.
        ''' </summary>
        <TestInitialize>
        Public Sub TestInitialize()
            Me.form = New Form() With {
                .StartPosition = FormStartPosition.Manual,
                .Location = New Point(-32000, -32000),
                .ShowInTaskbar = False
            }

            Me.textBox0 = New TextBox() With {.Name = NameOf(Me.textBox0), .TabIndex = 0, .Top = 0}
            Me.button = New Button() With {.Name = NameOf(Me.button), .TabIndex = 1, .Top = 30}
            Me.textBox1 = New TextBox() With {.Name = NameOf(Me.textBox1), .TabIndex = 2, .Top = 60}
            Me.textBox2 = New TextBox() With {.Name = NameOf(Me.textBox2), .TabIndex = 3, .Top = 90}

            Me.form.Controls.AddRange(New Control() {Me.textBox2, Me.textBox1, Me.button, Me.textBox0})
            Me.form.Show()
            Application.DoEvents()
        End Sub

        ''' <summary>
        ''' Closes the window.
        ''' </summary>
        <TestCleanup>
        Public Sub TestCleanup()
            Me.form.Close()
            Me.form.Dispose()
        End Sub

#End Region

#Region " Tests "

        ''' <summary>
        ''' Verifies forward navigation, skipping controls of other types.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_Forward_ReturnsNextControlOfType()
            Me.form.ActiveControl = Me.textBox0

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(True, False, False, Me.form.Controls)

            Assert.AreSame(Me.textBox1, result)
        End Sub

        ''' <summary>
        ''' Verifies backward navigation.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_Backward_ReturnsPreviousControlOfType()
            Me.form.ActiveControl = Me.textBox2

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(False, False, False, Me.form.Controls)

            Assert.AreSame(Me.textBox1, result)
        End Sub

        ''' <summary>
        ''' Verifies that forward navigation wraps to the first control when enabled.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_ForwardFromLastWithWrap_ReturnsFirstControl()
            Me.form.ActiveControl = Me.textBox2

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(True, False, True, Me.form.Controls)

            Assert.AreSame(Me.textBox0, result)
        End Sub

        ''' <summary>
        ''' Verifies that backward navigation wraps to the last control when enabled.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_BackwardFromFirstWithWrap_ReturnsLastControl()
            Me.form.ActiveControl = Me.textBox0

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(False, False, True, Me.form.Controls)

            Assert.AreSame(Me.textBox2, result)
        End Sub

        ''' <summary>
        ''' Verifies that, without wrap, forward navigation from the last control returns the active control.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_ForwardFromLastWithoutWrap_ReturnsActiveControl()
            Me.form.ActiveControl = Me.textBox2

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(True, False, False, Me.form.Controls)

            Assert.AreSame(Me.textBox2, result)
        End Sub

        ''' <summary>
        ''' Verifies that, without wrap, backward navigation from the first control returns the active control.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_BackwardFromFirstWithoutWrap_ReturnsActiveControl()
            Me.form.ActiveControl = Me.textBox0

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(False, False, False, Me.form.Controls)

            Assert.AreSame(Me.textBox0, result)
        End Sub

        ''' <summary>
        ''' Verifies that controls with <see cref="Control.TabStop"/> disabled are skipped when requested.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_TabStopOnly_SkipsControlsWithoutTabStop()
            Me.textBox1.TabStop = False
            Me.form.ActiveControl = Me.textBox0

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(True, True, False, Me.form.Controls)

            Assert.AreSame(Me.textBox2, result)
        End Sub

        ''' <summary>
        ''' Verifies that controls with <see cref="Control.TabStop"/> disabled are included when not requested.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_NotTabStopOnly_IncludesControlsWithoutTabStop()
            Me.textBox1.TabStop = False
            Me.form.ActiveControl = Me.textBox0

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(True, False, False, Me.form.Controls)

            Assert.AreSame(Me.textBox1, result)
        End Sub

        ''' <summary>
        ''' Verifies that a null control collection returns the active control.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_NullCollection_ReturnsActiveControl()
            Me.form.ActiveControl = Me.textBox1

            Dim result As Control = Me.form.GetNextControl(Of TextBox)(True, False, True, Nothing)

            Assert.AreSame(Me.textBox1, result)
        End Sub

        ''' <summary>
        ''' Verifies that a collection without controls of the specified type returns the active control.
        ''' </summary>
        <TestMethod>
        Public Sub GetNextControl_NoControlsOfType_ReturnsActiveControl()
            Using emptyPanel As New Panel()
                Me.form.ActiveControl = Me.textBox1

                Dim result As Control = Me.form.GetNextControl(Of TextBox)(True, False, True, emptyPanel.Controls)

                Assert.AreSame(Me.textBox1, result)
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
