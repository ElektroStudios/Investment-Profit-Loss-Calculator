#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Windows.Forms

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " UserControlsTests "

Namespace UserControls

    ''' ----------------------------------------------------------------------------------------------------
    ''' <summary>
    ''' Tests for the custom controls of the application 
    ''' (<see cref="InputFrame"/>, <see cref="CardPanel"/>, <see cref="ThemedNumericUpDown"/> and <see cref="CustomTextBox"/>).
    ''' </summary>
    ''' ----------------------------------------------------------------------------------------------------
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class UserControlsTests

#Region " Test Initialization / Cleanup "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' The culture applied to the test thread, restored after each test.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private cultureScope As Helpers.CultureScope

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Applies the culture of the default display format to the test thread, as the application does,
        ''' because the numeric up/down controls format and parse numbers with the culture of the thread.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestInitialize>
        Public Sub TestInitialize()
            Me.cultureScope = New Helpers.CultureScope(Global.Core.DisplayFormat.Default.Culture)
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Restores the culture of the test thread.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestCleanup>
        Public Sub TestCleanup()
            Me.cultureScope.Dispose()
        End Sub

#End Region

#Region " Helpers "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Exposes the protected validation of <see cref="ThemedNumericUpDown"/> for testing.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Private NotInheritable Class TestableNumericUpDown : Inherits ThemedNumericUpDown

            ''' <summary>
            ''' Calls <see cref="ThemedNumericUpDown.ValidateEditText"/>.
            ''' </summary>
            Public Sub ValidateText()
                Me.ValidateEditText()
            End Sub

        End Class

#End Region

#Region " InputFrame "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the hosted control is stretched horizontally (respecting the padding) and centered vertically.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub InputFrame_LaysOutHostedControl()
            Using frame As New InputFrame() With {.Size = New Size(200, 42), .Padding = New Padding(10, 0, 4, 0)},
                  textBox As New TextBox() With {.BorderStyle = BorderStyle.None, .Height = 20}

                frame.Controls.Add(textBox)
                frame.PerformLayout()

                Assert.AreEqual(10, textBox.Left)
                Assert.AreEqual(200 - 10 - 4, textBox.Width)
                Assert.AreEqual((42 - textBox.Height) \ 2, textBox.Top)
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the color properties round-trip and the frame is not focused by default.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub InputFrame_ColorProperties_RoundTrip()
            Using frame As New InputFrame() With {
                .BorderColor = Color.Red,
                .FocusedBorderColor = Color.Green,
                .DisabledBorderColor = Color.Blue
            }
                Assert.AreEqual(Color.Red, frame.BorderColor)
                Assert.AreEqual(Color.Green, frame.FocusedBorderColor)
                Assert.AreEqual(Color.Blue, frame.DisabledBorderColor)
                Assert.IsFalse(frame.IsFocused)
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the frame draws its border with <see cref="InputFrame.BorderColor"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub InputFrame_DrawsBorder()
            Using frame As New InputFrame() With {.Size = New Size(60, 30), .BackColor = Color.Black, .BorderColor = Color.Red},
                  bitmap As New Bitmap(60, 30)

                frame.DrawToBitmap(bitmap, New Rectangle(0, 0, 60, 30))

                Assert.AreEqual(Color.Red.ToArgb(), bitmap.GetPixel(0, 15).ToArgb())
                Assert.AreEqual(Color.Red.ToArgb(), bitmap.GetPixel(30, 0).ToArgb())
                Assert.AreEqual(Color.Black.ToArgb(), bitmap.GetPixel(30, 15).ToArgb())
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the frame draws its border with <see cref="InputFrame.ErrorBorderColor"/> when it has an error.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub InputFrame_HasError_DrawsErrorBorder()
            Using frame As New InputFrame() With {.Size = New Size(60, 30), .BackColor = Color.Black, .BorderColor = Color.Red, .ErrorBorderColor = Color.Yellow},
                  bitmap As New Bitmap(60, 30)

                frame.HasError = True
                frame.DrawToBitmap(bitmap, New Rectangle(0, 0, 60, 30))

                Assert.AreEqual(Color.Yellow.ToArgb(), bitmap.GetPixel(0, 15).ToArgb())
            End Using
        End Sub

#End Region

#Region " CardPanel "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the card draws its border with <see cref="CardPanel.BorderColor"/>.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub CardPanel_DrawsBorder()
            Using card As New CardPanel() With {.Size = New Size(60, 30), .BackColor = Color.White, .BorderColor = Color.Blue},
                  bitmap As New Bitmap(60, 30)

                card.DrawToBitmap(bitmap, New Rectangle(0, 0, 60, 30))

                Assert.AreEqual(Color.Blue, card.BorderColor)
                Assert.AreEqual(Color.Blue.ToArgb(), bitmap.GetPixel(59, 15).ToArgb())
                Assert.AreEqual(Color.White.ToArgb(), bitmap.GetPixel(30, 15).ToArgb())
            End Using
        End Sub

#End Region

#Region " ThemedNumericUpDown "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the control can be created (the internal buttons control is found) 
        ''' and that the color properties round-trip.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ThemedNumericUpDown_ColorProperties_RoundTrip()
            Using numeric As New ThemedNumericUpDown() With {
                .ButtonBackColor = Color.Red,
                .ButtonHoverBackColor = Color.Green,
                .ButtonPressedBackColor = Color.Blue,
                .ArrowColor = Color.Yellow,
                .DisabledArrowColor = Color.Gray
            }
                Assert.AreEqual(Color.Red, numeric.ButtonBackColor)
                Assert.AreEqual(Color.Green, numeric.ButtonHoverBackColor)
                Assert.AreEqual(Color.Blue, numeric.ButtonPressedBackColor)
                Assert.AreEqual(Color.Yellow, numeric.ArrowColor)
                Assert.AreEqual(Color.Gray, numeric.DisabledArrowColor)
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the up/down buttons are painted flat with <see cref="ThemedNumericUpDown.ButtonBackColor"/>
        ''' instead of the native buttons.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ThemedNumericUpDown_PaintsFlatButtons()
            Using numeric As New ThemedNumericUpDown() With {.BorderStyle = BorderStyle.None, .Width = 120, .ButtonBackColor = Color.Magenta}
                Dim buttons As Control = Nothing
                For Each child As Control In numeric.Controls
                    If Not TypeOf child Is TextBox Then
                        buttons = child
                    End If
                Next
                Assert.IsNotNull(buttons)

                Using bitmap As New Bitmap(numeric.Width, numeric.Height)
                    numeric.DrawToBitmap(bitmap, New Rectangle(Point.Empty, numeric.Size))

                    ' Top-left corner of the up button: far from the arrow glyph.
                    Dim pixel As Color = bitmap.GetPixel(buttons.Left + 1, buttons.Top + 1)
                    Assert.AreEqual(Color.Magenta.ToArgb(), pixel.ToArgb())
                End Using
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the typed text accepts both the dot and the comma as decimal separator,
        ''' with optional thousands separators.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("1000,50", "1000.50")>
        <DataRow("1,000.50", "1000.50")>
        <DataRow("1.000,50", "1000.50")>
        <DataRow("1,000", "1000")>
        <DataRow("25.5", "25.5")>
        Public Sub ThemedNumericUpDown_ValidateEditText_AcceptsBothDecimalSeparators(text As String, expected As String)
            Using numeric As New TestableNumericUpDown() With {.DecimalPlaces = 2, .Maximum = 1000000D}
                numeric.Text = text
                numeric.ValidateText()

                Assert.AreEqual(Decimal.Parse(expected, Globalization.CultureInfo.InvariantCulture), numeric.Value)
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that invalid typed text restores the previous value.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ThemedNumericUpDown_ValidateEditText_InvalidText_KeepsPreviousValue()
            Using numeric As New TestableNumericUpDown() With {.DecimalPlaces = 2, .Maximum = 1000000D, .Value = 42D}
                numeric.Text = "abc"
                numeric.ValidateText()

                Assert.AreEqual(42D, numeric.Value)
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that <see cref="ThemedNumericUpDown.TryGetEditValue"/> parses the text being typed 
        ''' without validating (reformatting) it, constrained to the minimum and maximum.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        <DataRow("1000,5", "1000.5", True)>
        <DataRow("1,000.5", "1000.5", True)>
        <DataRow("12,", "12", True)>
        <DataRow("5000000", "1000000", True, DisplayName:="Constrained to maximum")>
        <DataRow("-5", "0", True, DisplayName:="Constrained to minimum")>
        <DataRow("1,2,3", "0", False, DisplayName:="Invalid")>
        <DataRow("", "0", False, DisplayName:="Empty")>
        Public Sub ThemedNumericUpDown_TryGetEditValue_ParsesTypedText(typedText As String, expected As String, expectedResult As Boolean)
            Using numeric As New ThemedNumericUpDown() With {.DecimalPlaces = 2, .Maximum = 1000000D, .Value = 42D}
                Dim editBox As TextBox = Nothing
                For Each child As Control In numeric.Controls
                    If TypeOf child Is TextBox Then
                        editBox = DirectCast(child, TextBox)
                    End If
                Next
                Assert.IsNotNull(editBox)

                editBox.Text = typedText ' Simulates typing (no validation).

                Dim value As Decimal
                Assert.AreEqual(expectedResult, numeric.TryGetEditValue(value))
                Assert.AreEqual(Decimal.Parse(expected, Globalization.CultureInfo.InvariantCulture), value)
                Assert.AreEqual(typedText, editBox.Text, "The typed text must not be reformatted.")
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that <see cref="ThemedNumericUpDown.TryGetEditValue"/> returns the value when the text is not being edited.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ThemedNumericUpDown_TryGetEditValue_NotEditing_ReturnsValue()
            Using numeric As New ThemedNumericUpDown() With {.DecimalPlaces = 2, .Maximum = 1000000D, .Value = 42.5D}
                Dim value As Decimal
                Assert.IsTrue(numeric.TryGetEditValue(value))
                Assert.AreEqual(42.5D, value)
            End Using
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that, when the culture uses the comma as decimal separator, ambiguous typed texts 
        ''' are read the European way, and that <see cref="ThemedNumericUpDown.RefreshText"/> reformats the text.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub ThemedNumericUpDown_CommaCulture_ParsesAndFormats()
            Dim comma As Global.Core.DisplayFormat = Global.Core.DisplayFormat.Default.WithDecimalSeparator(Global.Core.DecimalSeparatorKind.Comma)

            Using numeric As New TestableNumericUpDown() With {.DecimalPlaces = 2, .Maximum = 1000000D, .ThousandsSeparator = True, .Value = 1234.5D}
                Assert.AreEqual("1,234.50", numeric.Text)

                Using scope As New Helpers.CultureScope(comma.Culture)
                    numeric.RefreshText()
                    Assert.AreEqual("1.234,50", numeric.Text)

                    numeric.Text = "1,500"
                    Assert.AreEqual(1.5D, numeric.Value)

                    numeric.Text = "1.500"
                    Assert.AreEqual(1500D, numeric.Value)
                    Assert.AreEqual("1.500,00", numeric.Text)
                End Using
            End Using
        End Sub

#End Region

#Region " CustomTextBox "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies that the hint properties round-trip.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        <TestMethod>
        Public Sub CustomTextBox_HintProperties_RoundTrip()
            Using textBox As New CustomTextBox() With {.Hint = "0.00", .HintForeColor = Color.Orange}
                Assert.AreEqual("0.00", textBox.Hint)
                Assert.AreEqual(Color.Orange, textBox.HintForeColor)
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
