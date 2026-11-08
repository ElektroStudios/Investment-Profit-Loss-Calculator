#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization

#End Region

#Region " ProfitLossCalculator "

Namespace Core

    ''' <summary>
    ''' Provides the UI-independent calculation logic of the application:
    ''' price parsing, price change percentages, target prices and profit and loss calculation.
    ''' </summary>
    Public NotInheritable Class ProfitLossCalculator

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="ProfitLossCalculator"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' The format used to display prices: thousands separators, 
        ''' at least two decimals and up to ten decimals (for low-priced assets such as some cryptocurrencies).
        ''' </summary>
        Public Const PriceDisplayFormat As String = "#,##0.00########"

        ''' <summary>
        ''' Parses a price typed by the user, using the dot as preferred decimal separator.
        ''' </summary>
        ''' <inheritdoc cref="TryParsePrice(String, DisplayFormat, ByRef Double)"/>
        Public Shared Function TryParsePrice(text As String, ByRef value As Double) As Boolean
            Return ProfitLossCalculator.TryParsePrice(text, DisplayFormat.Default, value)
        End Function

        ''' <summary>
        ''' Parses a price typed by the user.
        ''' <para></para>
        ''' Both the dot (".") and the comma (",") are accepted as decimal separator, 
        ''' with optional thousands separators; the decimal separator of <paramref name="format"/> 
        ''' resolves ambiguous texts (see <see cref="NumberTextParser"/> for the rules).
        ''' Non-finite values (NaN, infinity or overflowing numbers) are rejected.
        ''' </summary>
        ''' <param name="text">
        ''' The text to parse.
        ''' </param>
        ''' 
        ''' <param name="format">
        ''' The display format.
        ''' </param>
        ''' 
        ''' <param name="value">
        ''' When this method returns, contains the parsed price, or zero if the parsing failed.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="text"/> was parsed successfully; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="text"/> or <paramref name="format"/> is <see langword="Nothing"/>.
        ''' </exception>
        Public Shared Function TryParsePrice(text As String, format As DisplayFormat, ByRef value As Double) As Boolean
            If format Is Nothing Then
                Throw New ArgumentNullException(NameOf(format))
            End If
            Return NumberTextParser.TryParseDouble(text, format.DecimalSeparatorChar, value)
        End Function

        ''' <summary>
        ''' Formats a price with the default display format (e.g. 1344 is formatted as "1,344.00").
        ''' </summary>
        ''' <inheritdoc cref="FormatPrice(Double, DisplayFormat)"/>
        Public Shared Function FormatPrice(price As Double) As String
            Return ProfitLossCalculator.FormatPrice(price, DisplayFormat.Default)
        End Function

        ''' <summary>
        ''' Formats a price to be displayed to the user, using <see cref="PriceDisplayFormat"/> 
        ''' and the separators of the specified display format 
        ''' (e.g. 1344 is formatted as "1,344.00" or "1.344,00").
        ''' <para></para>
        ''' The resulting text can be parsed back with <see cref="ProfitLossCalculator.TryParsePrice(String, DisplayFormat, ByRef Double)"/>
        ''' using the same display format.
        ''' </summary>
        ''' <param name="price">
        ''' The price to format.
        ''' </param>
        ''' 
        ''' <param name="format">
        ''' The display format.
        ''' </param>
        ''' <returns>
        ''' The formatted price.
        ''' </returns>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="format"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' 
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="price"/> is not a finite value.
        ''' </exception>
        Public Shared Function FormatPrice(price As Double, format As DisplayFormat) As String

            If format Is Nothing Then
                Throw New ArgumentNullException(NameOf(format))
            End If
            If Double.IsNaN(price) OrElse Double.IsInfinity(price) Then
                Throw New ArgumentOutOfRangeException(NameOf(price), price, "Price must be a finite value.")
            End If

            Return format.FormatNumber(price, PriceDisplayFormat)
        End Function

        ''' <summary>
        ''' Calculates the target price that results from applying a percentage change to the current price.
        ''' </summary>
        ''' <param name="currentPrice">
        ''' The current asset price.
        ''' </param>
        '''
        ''' <param name="percent">
        ''' The percentage change (e.g. 10 for +10%, -25 for -25%).
        ''' </param>
        ''' <returns>
        ''' The resulting target price.
        ''' </returns>
        Public Shared Function CalculateTargetPrice(currentPrice As Double, percent As Double) As Double

            Return currentPrice + (currentPrice * (percent / 100))
        End Function

        ''' <summary>
        ''' Calculates the percentage change between the current price and the target price.
        ''' </summary>
        ''' <param name="currentPrice">
        ''' The current asset price.
        ''' </param>
        '''
        ''' <param name="targetPrice">
        ''' The target asset price.
        ''' </param>
        ''' <returns>
        ''' The percentage change.
        ''' <para></para>
        ''' Returns <see cref="Double.NaN"/> when both prices are zero,
        ''' and <see cref="Double.PositiveInfinity"/> or <see cref="Double.NegativeInfinity"/>
        ''' when only the current price is zero.
        ''' </returns>
        Public Shared Function CalculatePriceChangePercent(currentPrice As Double, targetPrice As Double) As Double

            Return ((targetPrice - currentPrice) / Math.Abs(currentPrice)) * 100
        End Function

        ''' <summary>
        ''' Calculates the profit (positive value) or loss (negative value) of an investment.
        ''' </summary>
        ''' <param name="invested">
        ''' The invested money (margin). Must be zero or greater.
        ''' </param>
        '''
        ''' <param name="currentPrice">
        ''' The asset price at the moment of the investment. Must be different than zero.
        ''' </param>
        '''
        ''' <param name="targetPrice">
        ''' The asset price at which the position is closed.
        ''' </param>
        '''
        ''' <param name="leverage">
        ''' The leverage multiplier. Must be greater than zero.
        ''' </param>
        ''' <returns>
        ''' The profit or loss amount.
        ''' </returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when any argument is out of its valid range.
        ''' </exception>
        Public Shared Function CalculateProfitLoss(invested As Double, currentPrice As Double, targetPrice As Double, leverage As Double) As Double

            ProfitLossCalculator.ValidateArguments(invested, currentPrice, targetPrice, leverage)
            If currentPrice = 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(currentPrice), currentPrice, "Current price cannot be zero.")
            End If

            Dim position As Double = invested * leverage
            Return ((targetPrice / currentPrice) * position) - position
        End Function

        ''' <summary>
        ''' Calculates the profit and loss of an investment,
        ''' returning a <see cref="ProfitLossResult"/> that describes the outcome.
        ''' <para></para>
        ''' The result is <see cref="ProfitLossResult.NotAvailable"/> when
        ''' <paramref name="invested"/> or <paramref name="currentPrice"/> are zero.
        ''' </summary>
        ''' <param name="invested">
        ''' The invested money (margin). Must be zero or greater.
        ''' </param>
        '''
        ''' <param name="currentPrice">
        ''' The asset price at the moment of the investment.
        ''' </param>
        '''
        ''' <param name="targetPrice">
        ''' The asset price at which the position is closed.
        ''' </param>
        '''
        ''' <param name="leverage">
        ''' The leverage multiplier. Must be greater than zero.
        ''' </param>
        ''' <returns>
        ''' The resulting <see cref="ProfitLossResult"/>.
        ''' </returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when any argument is out of its valid range.
        ''' </exception>
        Public Shared Function Calculate(invested As Double, currentPrice As Double, targetPrice As Double, leverage As Double) As ProfitLossResult

            ProfitLossCalculator.ValidateArguments(invested, currentPrice, targetPrice, leverage)

            If (invested = 0) OrElse (currentPrice = 0) Then
                Return ProfitLossResult.NotAvailable
            End If

            Dim amount As Double = ProfitLossCalculator.CalculateProfitLoss(invested, currentPrice, targetPrice, leverage)
            Dim changePercent As Double = ProfitLossCalculator.CalculatePriceChangePercent(currentPrice, targetPrice)
            Return New ProfitLossResult(amount, changePercent)
        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Validates the common calculation arguments.
        ''' </summary>
        Private Shared Sub ValidateArguments(invested As Double, currentPrice As Double, targetPrice As Double, leverage As Double)

            If Double.IsNaN(invested) OrElse Double.IsInfinity(invested) OrElse invested < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(invested), invested, "Invested money must be a finite value, zero or greater.")
            End If
            If Double.IsNaN(currentPrice) OrElse Double.IsInfinity(currentPrice) Then
                Throw New ArgumentOutOfRangeException(NameOf(currentPrice), currentPrice, "Current price must be a finite value.")
            End If
            If Double.IsNaN(targetPrice) OrElse Double.IsInfinity(targetPrice) Then
                Throw New ArgumentOutOfRangeException(NameOf(targetPrice), targetPrice, "Target price must be a finite value.")
            End If
            If Double.IsNaN(leverage) OrElse Double.IsInfinity(leverage) OrElse leverage <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(leverage), leverage, "Leverage must be a finite value greater than zero.")
            End If
        End Sub

#End Region

    End Class

End Namespace

#End Region
