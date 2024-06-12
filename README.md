# Project2-TransactionTracker


Console application for tracking transaction data.
Supports adding, editing, sorting and has session-independent data storage.
The transactions have 2 types (Income and Expense) with Date, Name and Value properties.


## Internal things


### Program
Holds the Main menu selection and calls functions.
In case of data management the History object that holds the data is passed as an argument.
Data is saved for each time you pass the Main loop and upon startup the program checks for local data.


#### Methods
- Add: Step-by-step method to add transactions.
- FastAdd: Fast 1-line method to add transactions.
- Edit: Choice between Edit and Remove transaction(s) via index selection.
- Sort: Sort the transactions
	- Sort methods: Date, Name or Value in Ascending or Descending order.
- List: Display the transactions via optional filters.
	- List filters: All, Expense or Income
- Reset: Delete the stored transaction file.
- Quit: Exit program.
- Help: Display help.

The Main menu has access to Quick commands for Sort and List.
- Sort date/name/value a/d
- List all/expense/income

In other places the program will ask what method you want to use for sorting and display.


### TransactionManager
Handles the creation of Transaction objects and interactions with TransactionHistory object.


#### Methods
- New, Fastadd: Construct transactions from new data.
- Edit: Handles selection of transactions to remove or edit.
- EqualStrings, EmptyString: Methods for string verification.


### TransactionHistory
This object holds all the data and is the only Class allowed to modify the data.


#### Methods
- Add: Checks and adds the input transaction object to the data list.
- Edit: Displays and lets user edit the data of input index from TransactionManager: Edit
- Show: Displays the data according to input
- Sort: Sorts the data according to input
- Remove: Removes the data of input index from TransactionManager: Remove
- ReadHistory: Reads local data file if it exists, otherwise initialize an empty set.
- WriteHistory: Saves the current data to file.
- ResetHistory: Deletes local data and saves a new empty one.

- 
### Transaction
#### Properties
- Type (Expense, Income)
- Date
- Name
- Value

![FlowchartWhite.png]