### Product checker

The Product parser application is a windows Forms application that allows users to parse and validate product codes, calculate their weights, and keep track of the total weight of scanned products. It provides settings options for configuring the time interval and code format.

#### Usage:

* Launch the ParseProductCode application.
* Enter a product code in the provided TextBox.
* The code will be validated against the current code format. If valid, the TextBox background will turn green.
* The timer will start or reset based on the time settings.
* If the product weight exceeds the maximum weight limit (specified in the second TextBox), a warning message will be displayed.
* Valid product codes and their weights will be displayed in the ListBox.
* The total weight of scanned products will be updated accordingly.
* Click the "Настройки" (Settings) button to open the settings menu.
* From the settings menu, select "Настроить время" (Configure Time) to open the time settings form. Set the desired time interval and click "OK."
* Select "Настроить формат кода" (Configure Code Format) to open the code format settings form. Enter the desired code format and click "OK."
* Reset the application values by clicking the "Reset" button.
* Close the application when finished.
