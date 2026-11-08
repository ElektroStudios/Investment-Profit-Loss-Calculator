#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "


#End Region

#Region " ProfitLossResult "

Namespace Core

    ''' <summary>
    ''' Specifies the kind of outcome of a profit and loss calculation.
    ''' </summary>
    Public Enum ProfitLossKind As Integer

        ''' <summary>
        ''' The calculation could not be performed due to missing input.
        ''' </summary>
        NotAvailable = 0

        ''' <summary>
        ''' The investment produces a profit.
        ''' </summary>
        Profit = 1

        ''' <summary>
        ''' The investment produces a loss.
        ''' </summary>
        Loss = 2

        ''' <summary>
        ''' The investment produces neither profit nor loss.
        ''' </summary>
        BreakEven = 3

    End Enum

    ''' <summary>
    ''' Represents the result of a profit and loss calculation.
    ''' </summary>
    Public NotInheritable Class ProfitLossResult

#Region " Constants "

        ''' <summary>
        ''' The text displayed when the result is not available.
        ''' </summary>
        Public Const NotAvailableText As String = "N/A"

#End Region

#Region " Fields "

        ''' <summary>
        ''' The shared instance returned by <see cref="ProfitLossResult.NotAvailable"/>.
        ''' </summary>
        Private Shared ReadOnly NotAvailableInstance As New ProfitLossResult()

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets a <see cref="ProfitLossResult"/> that represents a calculation that could not be performed.
        ''' </summary>
        Public Shared ReadOnly Property NotAvailable As ProfitLossResult
            Get
                Return ProfitLossResult.NotAvailableInstance
            End Get
        End Property

        ''' <summary>
        ''' Gets the kind of outcome.
        ''' </summary>
        Public ReadOnly Property Kind As ProfitLossKind

        ''' <summary>
        ''' Gets the profit (positive value) or loss (negative value) amount.
        ''' </summary>
        Public ReadOnly Property Amount As Double

        ''' <summary>
        ''' Gets the percentage change between the current price and the target price.
        ''' </summary>
        Public ReadOnly Property ChangePercent As Double

        ''' <summary>
        ''' Gets a value indicating whether the result is available.
        ''' </summary>
        Public ReadOnly Property IsAvailable As Boolean
            Get
                Return Me.Kind <> ProfitLossKind.NotAvailable
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ProfitLossResult"/> class
        ''' that represents a result that is not available.
        ''' </summary>
        Private Sub New()
            Me.Kind = ProfitLossKind.NotAvailable
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ProfitLossResult"/> class.
        ''' </summary>
        ''' <param name="amount">
        ''' The profit (positive value) or loss (negative value) amount.
        ''' </param>
        '''
        ''' <param name="changePercent">
        ''' The percentage change between the current price and the target price.
        ''' </param>
        Public Sub New(amount As Double, changePercent As Double)
            Me.Amount = amount
            Me.ChangePercent = changePercent

            Select Case amount
                Case > 0
                    Me.Kind = ProfitLossKind.Profit
                Case < 0
                    Me.Kind = ProfitLossKind.Loss
                Case Else
                    Me.Kind = ProfitLossKind.BreakEven
            End Select
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Returns the text that is displayed to the user for this result, using the default display format
        ''' (e.g. "$1,000.50  (+12.5%)").
        ''' </summary>
        ''' <returns>
        ''' The text that is displayed to the user.
        ''' </returns>
        Public Function ToDisplayText() As String
            Return Me.ToDisplayText(DisplayFormat.Default)
        End Function

        ''' <summary>
        ''' Returns the text that is displayed to the user for this result, using the specified display format
        ''' (e.g. "$1,000.50  (+12.5%)" or "1.000,50 €  (+12,5%)").
        ''' <para></para>
        ''' The percentage change is formatted with up to two decimals.
        ''' </summary>
        ''' <param name="format">
        ''' The display format.
        ''' </param>
        ''' <returns>
        ''' The text that is displayed to the user.
        ''' </returns>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="format"/> is <see langword="Nothing"/>.
        ''' </exception>
        Public Function ToDisplayText(format As DisplayFormat) As String

            If format Is Nothing Then
                Throw New ArgumentNullException(NameOf(format))
            End If

            Select Case Me.Kind

                Case ProfitLossKind.Profit
                    Return $"{format.FormatCurrency(Me.Amount, 2)}  (+{format.FormatNumber(Me.ChangePercent, "0.##")}%)"

                Case ProfitLossKind.Loss
                    Return $"{format.FormatCurrency(Me.Amount, 2)}  ({format.FormatNumber(Me.ChangePercent, "0.##")}%)"

                Case ProfitLossKind.BreakEven
                    Return format.FormatCurrency(0, 0)

                Case Else
                    Return ProfitLossResult.NotAvailableText

            End Select
        End Function

        ''' <summary>
        ''' Returns a <see cref="String"/> that represents this instance.
        ''' </summary>
        ''' <returns>
        ''' A <see cref="String"/> that represents this instance.
        ''' </returns>
        Public Overrides Function ToString() As String
            Return Me.ToDisplayText()
        End Function

#End Region

    End Class

End Namespace

#End Region
