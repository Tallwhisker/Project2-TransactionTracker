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
            this._balance = 0;
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
            if (isExpense && transactionValue + currentBalance < 0) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("> Transaction rejected.\n"+ 
                    "Error: Balance cannot go below 0.\n" +
                    $"Current balance: {this.CalculateBalance()}"
                );
                return;
            }

            //Check that the new income doesn't go above Decimal Max
            if (isIncome && transactionValue + currentBalance > Decimal.MaxValue) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("> Transaction rejected.\n" +
                    $"Error: Balance cannot go above {Decimal.MaxValue}.\n" +
                    $"Current balance: {this.CalculateBalance()}"
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
            catch (ArgumentOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Index [{index}] does not exist.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing device index.");
                Console.WriteLine(e);
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
            catch (ArgumentOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Index [{index}] does not exist.");
                return;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error editing index [{index}].");
                Console.WriteLine(e);
                return;
            }


            Console.WriteLine($"\nCurrent Date: {transaction.TimeString}" + 
                "Input new Date YYYY-MM-DD or leave blank to skip"
            );
            string? itemTime = Console.ReadLine();
            bool isValidDate = DateOnly.TryParse(itemTime, out DateOnly newDate);

            if (String.IsNullOrEmpty(itemTime))
            {
                itemTime = transaction.TimeString;
            }
            else if (!isValidDate && !String.IsNullOrEmpty(itemTime))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Date format incorrect. YYYY-MM-DD. Aborting.");
                return;
            }


            Console.WriteLine($"\nCurrent Name: {transaction.TransactionName}\n" +
                "Input new Name or leave blank to skip."
            );
            Console.Write("Input Name: ");
            string? itemName = Console.ReadLine();

            if (String.IsNullOrEmpty(itemName))
            {
                itemName = transaction.TransactionName;
            }


            Console.WriteLine($"\nCurrent Value: {transaction.TransactionValue}\n" +
                "Input new Value or leave blank to skip."
            );
            Console.Write("Input Value: ");
            string? itemValue = Console.ReadLine();
            bool isValidValue = Decimal.TryParse(itemValue, out Decimal newDecimal);

            if (String.IsNullOrEmpty(itemValue))
            {
                itemValue = transaction.TransactionValue.ToString();
            }
            else if ( ! isValidValue && ! String.IsNullOrEmpty(itemValue))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Value needs to be a number. Aborting.");
                return;
            }

            //If nothing has gone wrong, save the values.
            transaction.TransactionTime = newDate;
            transaction.TransactionName = itemName;
            transaction.TransactionValue = Convert.ToDecimal(itemValue);
            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine("> Transaction Saved.");
        }


        public void Sort(string method)
        {
            switch (method.ToLower())
            {
                case "name":
                    Console.WriteLine("> Sorted by Name\n");
                    this._transactions = this._transactions.OrderBy(name => name.TransactionName).ToList();
                    break;

                case "value":
                    this._transactions = this._transactions.OrderBy(value => value.TransactionValue).ToList();
                    Console.WriteLine("> Sorted by Value\n");

                    break;

                case "date":
                    this._transactions = this._transactions.OrderBy(date => date.TransactionTime).ToList();
                    Console.WriteLine("> Sorted by Date\n");

                    break;

                default:
                    Console.WriteLine("> Input not recognized. Defaulting to Date.\n");
                    this.Sort("date");
                    break;
            }
        }


        public void ShowTransactions(string method)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            switch (method.ToLower())
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"> List method '{method}' not recognized. Defaulting to 'all'");
                    method = "All";
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine($"\nListing {method} transactions.\n" +
                "Index".PadRight(13) +
                "Date".PadRight(17) +
                "Name".PadRight(25) +
                "Value".PadRight(15)
            );


            int index = 0;
            foreach (var item in _transactions)
            {
                string type = item.TransactionType.ToString();
                StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                bool isSameType = String.Equals( type, method, comparison );

                //If List method is "all" let everything through
                if (method.Equals( "all", comparison ))
                {
                    Console.WriteLine($"" +
                        $"{index,-13}" +
                        $"{item.TransactionTime,-17}" +
                        $"{item.TransactionName,-25}" +
                        $"{item.TransactionValue,-15}"
                    );
                }
                //Else only allow matching types
                else if (isSameType)
                {
                    Console.WriteLine($"" +
                        $"{index,-13}" +
                        $"{item.TransactionTime,-17}" +
                        $"{item.TransactionName,-25}" +
                        $"{item.TransactionValue,-15}"
                    );
                }
                index++;
            }
            Console.WriteLine($"Account balance:".PadLeft(46) +
                $"".PadLeft(9) +
                $"{this.CalculateBalance()}");
        }


        //Check FilePath for history, else return empty List.
        public List<Transaction> ReadHistory()
        {
            List<Transaction> transactions = [];

            if (File.Exists(FilePath))
            {
                XmlReader reader = XmlReader.Create(FilePath);
                XmlSerializer serializer = new XmlSerializer(typeof(List<Transaction>));

                try
                {
                    using (reader)
                    {
                        transactions = (List<Transaction>)serializer.Deserialize(reader);
                    }
                }
                catch (Exception e) 
                {
                    reader.Dispose();
                    reader.Close();
                    Console.WriteLine("Error loading history.");
                    File.Delete(FilePath);
                    Console.WriteLine($"Data file at \"{FilePath}\" has been replaced with an empty file due to this error.");
                }
            }
            return transactions;
        }


        internal void WriteHistory()
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(this.FilePath, writerSettings);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(List<Transaction>));

            try
            {
                using (writer)
                {
                    serializer.Serialize(writer, this._transactions, namespaces);
                    writer.Dispose();
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when saving file.\n" + e);
            }
        }


        internal void ResetHistory()
        {
            Console.WriteLine("Reset history, confirm: Y / N");
            Console.Write("Input: ");
            string input = Console.ReadLine();

            switch (input)
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
