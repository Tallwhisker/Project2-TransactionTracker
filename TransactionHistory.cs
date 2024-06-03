using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_TransactionTracker
{
    internal class TransactionHistory
    {
        internal List<Transaction> _transactions = new List<Transaction>();
        internal decimal _balance = 0;
        
        public decimal CalculateBalance()
        {
            _balance = (decimal)_transactions.Sum(value => value.TransactionValue);
            return _balance;
        }
        public void AddTransaction()
        {
            switch (item.TypeOfTransaction)
            {
                case "expense":
                    if(item.TransactionValue > CalculateBalance())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Expense exceeds balance.");
                        Console.WriteLine($"Current balance: {this._balance}.");
                    } else
                    {
                        this._transactions.Add(item);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Transaction added.");
                    }
                    break;

                case "income":
                    if (item.TransactionValue + CalculateBalance() < decimal.MaxValue)
                    {
                        this._transactions.Add(item);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Transaction added.");
                    }
                    break;
            }
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
