using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project2_TransactionTracker
{
    internal class TransactionHistory
    {
        private List<Transaction> _transactions = [];
        private List<Transaction> _sortedTransactions = [];
        private decimal _balance = 0;
        
        internal decimal CalculateBalance()
        {
            _balance = (decimal)_transactions.Sum(value => value.TransactionValue);
            return _balance;
        }
        internal void AddTransaction()
        {
            Regex dateRegex = new Regex(@"^\d{4}-\d{2}-\d{2}$");
            string ?input = null;

            DateOnly dateTime;
            string ?itemType;
            string ?itemName;
            decimal itemValue;

            do
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Date format: YYYY-MM-DD");
                Console.Write("Input transaction date: ");
                input = Console.ReadLine().ToLower().Trim();

                if (dateRegex.Count(input) == 10)
                {
                    string[] dateArray = input.Split('-');
                    int year = Convert.ToInt32(dateArray[0]);
                    int month = Convert.ToInt32(dateArray[1]);
                    int day = Convert.ToInt32(dateArray[2]);

                    dateTime = new DateOnly(year, month, day);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Incorrect date. YYYY-MM-DD required.");
                    continue;
                }

                Console.WriteLine("Types: 'Expense' 'Income' ");
                Console.Write("Input transaction type: ");
                input = Console.ReadLine().ToLower().Trim();

                if(input == "expense" || input == "income")
                {
                    itemType = input;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Incorrect type. 'expense' or 'income' required.");
                    continue;
                }

                Console.Write("Input transaction name: ");
                input = Console.ReadLine().ToLower().Trim();
                if( ! String.IsNullOrEmpty(input))
                {
                    itemName = input;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Name can't be empty");
                    continue;
                }

                Console.Write("Input transaction value: ");
                input = Console.ReadLine().ToLower().Trim();

                bool value = Decimal.TryParse(input, out decimal number);

                if(value == false) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Value needs to be a number.");
                    continue;
                }

                 itemValue = number; 

                this._transactions.Add(new Transaction(dateTime, itemType, itemName, itemValue));

            } while (input != "exit");
        }

        public void RemoveTransaction()
        {
        }

        public void EditTransaction()
        {

        }

        public void Sort(string method)
        {
            switch (method)
            {
                case "name":
                    this._transactions.OrderBy(name => name.TransactionName);
                    break;

                case "cost":
                    this._transactions.OrderBy(value => value.TransactionValue);
                    break;

                case "date":
                    this._transactions.OrderBy(date => date.TransactionTime);
                    break;
            }
        }
        public void ShowTransactions()
        {
            Console.WriteLine("titlebar");
            int index = 0;
            foreach (var item in _transactions)
            {
                Console.WriteLine($"{index} - {item.TransactionTime} {item.TransactionName}");
                index++;
            }
        }

        public void ReadStorage(string filePath)
        {
            List<string> storage = File.ReadAllLines(filePath).ToList();

            foreach (string line in storage)
            {
                string[] items = line.Split(',');
                decimal amount = Convert.ToDecimal(items[3]);

                string[] date = items[0].Split("-"); //Get and split date into array
                int year = Convert.ToInt32(date[0]);
                int month = Convert.ToInt32(date[1]);
                int day = Convert.ToInt32(date[2]);
                DateOnly dateOnly = new DateOnly(year, month, day);

                this._transactions.Add(new Transaction(
                    dateOnly, //Date
                    items[1], //Type
                    items[2], //Name
                    amount //Value
                    )
                );
            }
        }

        public void WriteStorage(string filePath)
        {
            this.Sort("date");
            string[] strings = new string[this._transactions.Count];
            int i = 0;
            foreach (var item in this._transactions)
            {
                strings[i] =
                    $"{item.TransactionTime}," +
                    $"{item.TypeOfTransaction}," +
                    $"{item.TransactionName}," +
                    $"{item.TransactionValue}";
                i++;
            }
            File.WriteAllLines(filePath, strings);
        }
    }
}
