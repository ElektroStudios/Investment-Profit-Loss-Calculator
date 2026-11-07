# Investment Profit & Loss Calculator Version History 📋

## v2.0 *(current)* 🆕
#### 🎨 New Design:
 - Completely redesigned user interface with a modern, flat look: bordered input fields, a segmented "Price | Percent" selector, a result card and flat buttons.
 - The application now follows the Windows light/dark mode, including a dark title bar in dark mode.
 - The theme switches instantly when the Windows light/dark mode is changed while the application is open.
 - Clearer labels: "Amount invested", "Leverage", "Entry price", "Target price" and "Estimated profit / loss".
#### 🌟 Improvements:
 - Both "." and "," are accepted as decimal separator, with optional thousands separators (e.g. "1,000.50", "1.000,50" or "1000,50").
 - Prices are displayed with thousands separators and decimals (e.g. "1,344.00"), supporting up to 10 decimals for low-priced assets.
 - The invested amount now accepts cents (e.g. "1,000.50").
 - Invalid values are reported with a red field border and an error message, instead of being silently ignored.
 - Price fields no longer accept letters or other invalid characters.
 - Screenshots now capture the window exactly as it is displayed, including the dark title bar.
 - Added controls to select currency (Euro or Dollar) and decimal separator.
#### 🛠️ Fixes:
 - Fixed the text cursor jumping to the beginning while typing decimals in the "Amount invested" and "Percent" fields.
 - Fixed the screenshot showing a light title bar and borders.
 - Fixed the percentage drifting slightly when changing the entry price in "Percent" mode.
#### ⚙️ Internal:
 - Calculation logic extracted from the user interface and covered by a new unit test project.

## v1.3.1 🔄
#### 🛠️ Fixes:
 - Fixed "Current Asset Price" control behavior when "Fixed" checkbox is selected.

## v1.3 🔄
#### 🛠️ Fixes:
 - Minor fixes to some controls that didn't properly refresh the calculation after input.
#### 🌟 Improvements:
 - Minor improvements to some controls, which now will restore their default value when focus is lost and no input is provided.

## v1.2 🔄
#### 🛠️ Fixes:
 - Tab order of the controls is fixed.
 - "Investment" numeric control was not updating the P&L calculation when pressed the up arrow in this control.
#### 🌟 Improvements:
 - Now the screenshot file names includes the current date and time to prevent file name collisions.

## v1.1 🔄
(*Changes not available.*)

## v1.0 🔄
Initial Release.