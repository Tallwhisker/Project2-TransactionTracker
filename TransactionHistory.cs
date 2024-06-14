using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Project2_TransactionTracker
{
    public class TransactionHistory
    {
        public TransactionHistory(string FilePath)
        {
            this.FilePath = FilePath;

            //ReadHistory checks FilePath and reads history, or returns empty List.
            this._transactions = this.ReadHistory();
            this._balance = 0m;
        }

        public string FilePath { get; set; }
        private List<Transaction> _transactions;
        private decimal _balance;


        internal decimal CalculateBalance()
        {
            this._balance = (decimal)this._transactions.Sum(value => value.TransactionValue);
            return this._balance;
        }


        internal void Add(Transaction transaction) 
        {
            decimal currentBalance = this.CalculateBalance();
            decimal transactionValue = transaction.TransactionValue;

            bool isExpense = transaction.TransactionType == TransactionTypes.Expense;
            bool isIncome = transaction.TransactionType == TransactionTypes.Income;

            //Check that the new expense doesn't cause negative balance.
            if ( isExpense && transactionValue + currentBalance < 0 ) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("> Transaction rejected.\n"+ 
                    "Error: Balance would go below 0.\n" +
                    $"Current balance: {this.CalculateBalance()}\n"
                );
                return;
            }

            //Check that the new income doesn't go above Decimal Max
            if ( isIncome && transactionValue + currentBalance > Decimal.MaxValue ) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("> Transaction rejected.\n" +
                    $"Error: Balance cannot go above {Decimal.MaxValue}.\n" +
                    $"Current balance: {this.CalculateBalance()}\n"
                );
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("> Added transaction.");
            this._transactions.Add(transaction);
        }


        public void Remove(int index)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                this._transactions.RemoveAt(index);
                Console.WriteLine($"> Removed transaction at index [{index}].");
            }
            catch ( ArgumentOutOfRangeException )
            {
                TransactionManager.PrintError($"Error: Index [{index}] does not exist.");
            }
            catch ( Exception e )
            {
                TransactionManager.PrintError($"Error removing at index [{index}]:\n" + e.GetType());
            }
        }


        public void EditTransaction(int index)
        {
            Transaction transaction;
            try
            {
                transaction = this._transactions[index];
                Console.WriteLine($"> Editing Index [{index}]");
            }
            catch ( ArgumentOutOfRangeException )
            {
                TransactionManager.PrintError($"Error: Index [{index}] does not exist.");
                return;
            }
            catch ( Exception e )
            {
                TransactionManager.PrintError($"Error editing index [{index}].\n" + e.GetType());
                return;
            }


            Console.WriteLine($"\nCurrent Date: {transaction.TimeString}\n" + 
                            "Input new Date YYYY-MM-DD or leave blank to skip"
            );
            Console.Write("Input Date: ");
            string? itemTime = Console.ReadLine().Trim();
            bool isValidDate = DateOnly.TryParse(itemTime, out _);

            if ( String.IsNullOrEmpty(itemTime) )
            {
                itemTime = transaction.TimeString;
            }
            else if ( ! isValidDate && ! String.IsNullOrEmpty(itemTime))
            {
                TransactionManager.PrintError("Date format incorrect. YYYY-MM-DD.");
                return;
            }


            Console.WriteLine($"\nCurrent Name: {transaction.TransactionName}\n" +
                "Input new Name or leave blank to skip."
            );
            Console.Write("Input Name: ");
            string? itemName = Console.ReadLine().Trim();

            if ( String.IsNullOrEmpty(itemName) )
            {
                itemName = transaction.TransactionName;
            }


            Console.WriteLine($"\nCurrent Value: {transaction.TransactionValue}\n" +
                "Input new Value or leave blank to skip."
            );
            Console.Write("Input Value: ");
            string? itemValue = Console.ReadLine().Trim();
            bool isValidValue = Decimal.TryParse(itemValue, out _);

            if ( String.IsNullOrEmpty(itemValue) )
            {
                itemValue = transaction.TransactionValue.ToString();
            }
            else if ( ! isValidValue && ! String.IsNullOrEmpty(itemValue) )
            {
                TransactionManager.PrintError("Value needs to be a number. Aborting.");
                return;
            }

            //If nothing has gone wrong, save the values.
            transaction.TimeString = itemTime;
            transaction.TransactionName = itemName;
            transaction.TransactionValue = Convert.ToDecimal(itemValue);

            if (transaction.TransactionValue < 0) 
            {
                transaction.TransactionType = TransactionTypes.Expense; 
            }
            else if (transaction.TransactionValue > 0) 
            {
                transaction.TransactionType = TransactionTypes.Income; 
            }

            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine("> Transaction Saved.");
        }


        public void Sort(string method, string methodTwo = "")
        {
            //If optional argument methodTwo isn't provided, ask user for it.
            if ( String.IsNullOrEmpty( methodTwo) ) 
            {
                Console.WriteLine("Sort by Ascending 'a' or Descending 'd'?");
                Console.Write("Input: ");
                methodTwo = Console.ReadLine().Trim();
            }
            method = String.Concat(method, $" {methodTwo}");

            switch ( method.ToLower() )
            {
                case "name a":
                    Console.WriteLine("> Sorted by Name Ascending");
                    this._transactions = [.. this._transactions.OrderBy(name => name.TransactionName)];
                    break; 

                case "name d":
                    Console.WriteLine("> Sorted by Name Descending");
                    this._transactions = [.. this._transactions.OrderByDescending(name => name.TransactionName)];
                    break;

                case "value a":
                    this._transactions = [.. this._transactions.OrderBy(value => value.TransactionValue)];
                    Console.WriteLine("> Sorted by Value Ascending");
                    break;

                case "value d":
                    this._transactions = [.. this._transactions.OrderByDescending(value => value.TransactionValue)];
                    Console.WriteLine("> Sorted by Value Descending");
                    break;

                case "date a":
                    this._transactions = [.. this._transactions.OrderBy(date => date.TransactionTime)];
                    Console.WriteLine("> Sorted by Date Ascending");
                    break;

                case "date d":
                    this._transactions = [.. this._transactions.OrderByDescending(date => date.TransactionTime)];
                    Console.WriteLine("> Sorted by Date Descending");
                    break;

                default:
                    Console.WriteLine("> Input not recognized. Defaulting to Date Ascending.");
                    this._transactions = [.. this._transactions.OrderBy(date => date.TransactionTime)];
                    break;
            }
        }


        public void ShowTransactions(string method)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            switch ( method.ToLower() )
            {
                case "all":
                    method = "All";
                    break;

                case "income":
                    method = "Income";
                    break;

                case "expense":
                    method = "Expense";
                    break;

                default:
                    TransactionManager.PrintError($"> List method '{method}' not recognized. Defaulting to 'all'");
                    method = "All";
                    break;
            }

            Console.WriteLine($"\nListing {method} transactions.\n" +
                "Index".PadRight(13) +
                "Date".PadRight(17) +
                "Name".PadRight(25) +
                "Value".PadRight(15)
            );


            int index = 0;
            foreach ( var item in _transactions )
            {
                string type = item.TransactionType.ToString();
                StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                bool isSameType = String.Equals( type, method, comparison );

                //If List method is "all" let everything through
                if ( method.Equals( "all", comparison ) )
                {
                    Console.WriteLine($"" +
                        $"{index,-13}" +
                        $"{item.TransactionTime,-17}" +
                        $"{item.TransactionName,-25}" +
                        $"{item.TransactionValue,-15:C2}"
                    );
                }
                //Else only allow matching types
                else if ( isSameType )
                {
                    Console.WriteLine($"" +
                        $"{index,-13}" +
                        $"{item.TransactionTime,-17}" +
                        $"{item.TransactionName,-25}" +
                        $"{item.TransactionValue,-15:C2}"
                    );
                }
                index++;
            }
            Console.WriteLine($"Account balance:".PadLeft(46) +
                $"".PadLeft(9) +
                $"{this.CalculateBalance():C}");
        }


        //Check FilePath for history, else return empty List.
        internal List<Transaction> ReadHistory()
        {
            List<Transaction> transactions = new List<Transaction>();

            if ( File.Exists(FilePath) )
            {
                XmlReader reader = XmlReader.Create(FilePath);
                XmlSerializer serializer = new(typeof(List<Transaction>));

                try
                {
                    using ( reader )
                    {
                        transactions = serializer.Deserialize(reader) as List<Transaction>;
                    }
                }
                catch ( Exception ) 
                {
                    reader.Dispose();
                    reader.Close();
                    File.Delete(FilePath);
                    TransactionManager.PrintError("Error loading history.\n" +
                    $"Data file at \"{FilePath}\" has been replaced with an empty file due to this error.");
                }
            }
            return transactions;
        }


        internal void WriteHistory()
        {
            XmlWriterSettings writerSettings = new()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            XmlWriter writer = XmlWriter.Create(this.FilePath, writerSettings);

            XmlSerializerNamespaces namespaces = new(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new(typeof(List<Transaction>));

            try
            {
                using ( writer )
                {
                    serializer.Serialize(writer, this._transactions, namespaces);
                    writer.Dispose();
                    writer.Close();
                }
            }
            catch ( Exception e )
            {
                TransactionManager.PrintError("Error when saving file." + e);
            }
        }


        internal void ResetHistory()
        {
            Console.WriteLine("Reset history, confirm: Y / N");
            Console.Write("Input: ");
            string? input = Console.ReadLine();

            switch ( input )
            {
                case "y":
                    if (File.Exists(FilePath)) 
                    {
                        this._transactions.Clear();
                        Console.WriteLine($"Deleting history at {FilePath}");
                        File.Delete(FilePath);
                    }
                    break;
            }
        }
    }
}
