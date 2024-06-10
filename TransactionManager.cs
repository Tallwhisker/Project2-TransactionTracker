using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Project2_TransactionTracker
{
    internal static class TransactionManager
    {
        public static TransactionHistory New(TransactionHistory history)
        {
            string? itemType;
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Add transaction type or 'exit' to save.");

                Console.Write("Transaction type: ");
                itemType = Console.ReadLine().Trim();

                if (itemType.ToLower() == "exit")
                {
                    continue;
                }

                //CheckInput returns true if string is empty or null.
                if (CheckInput(itemType))
                {
                    Console.WriteLine("Transaction type can't be empty.");
                    continue;
                }

                //Check if type exists
                switch (itemType.ToLower())
                {
                    case "expense":
                    case "income":

                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Incorrect transaction type.\n" +
                            "Expense\n" +
                            "Income\n");
                        continue;
                }

                DateOnly itemDate;
                try
                {
                    Console.Write("Transaction date YYYY-MM-DD: ");
                    itemDate = DateOnly.Parse(Console.ReadLine().Trim());
                }
                catch (FormatException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Date format incorrect. YYYY-MM-DD required.");
                    continue;
                }
                catch (ArgumentNullException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Date empty. YYYY-MM-DD required.");
                    continue;
                }


                Console.Write("Transaction name: ");
                string itemName = Console.ReadLine().Trim();
                if (CheckInput(itemName))
                {
                    Console.WriteLine("Transaction name can't be empty.");
                    continue;
                }

                decimal itemValue;
                try
                {
                    Console.Write("Transaction value: ");
                    itemValue = Convert.ToDecimal(Console.ReadLine().Trim());
                }
                catch (FormatException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Invalid format. Numbers only");
                    continue;
                }
                catch (OverflowException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Price must be within {Decimal.MinValue} to {Decimal.MaxValue}");
                    continue;
                }


                switch (itemType.ToLower()) 
                {
                    case "expense":
                        if (itemValue > 0)
                        {
                            itemValue *= -1;
                        }

                        history.Add(new Transaction(
                            TransactionTypes.Expense,
                            itemDate,
                            itemName,
                            itemValue
                            ));
                        break;

                    case "income":
                        history.Add(new Transaction(
                            TransactionTypes.Income,
                            itemDate,
                            itemName,
                            itemValue
                            ));
                        break;

                    default:
                        break;
                }
            }
            //End of 'New' loop
            while (itemType != "exit");

            return history;
        }

        public static TransactionHistory Edit(TransactionHistory history) 
        {
            return history;
        }


        internal static bool CheckInput(string input)
        {
            bool isEmpty = String.IsNullOrEmpty(input);

            if (isEmpty)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            return isEmpty;
        }
    }
}
