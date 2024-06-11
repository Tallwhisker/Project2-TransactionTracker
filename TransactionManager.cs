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


                //Check if type is correct
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
                        if (itemValue < 0)
                        {
                            itemValue = Math.Abs(itemValue);
                        }

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
            //End of loop
            while (itemType.ToLower() != "exit");

            return history;
        }

        public static TransactionHistory FastAdd(TransactionHistory history)
        {
            Console.WriteLine("FastAdd Mode. 'exit' when done.");
            string? input;
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\nPositive value for income, negative for expense.\n" +
                    "Input format: YYYY-MM-DD NAME VALUE"
                );
                Console.Write("Input: ");
                input = Console.ReadLine();

                if (input.ToLower().Trim() == "exit")
                { 
                    continue; 
                }
                else if (CheckInput(input))
                {
                    Console.WriteLine("Input can't be empty. Starting over.");
                    continue;
                }

                List<String> strings = input.Split(" ").ToList();

                if (strings.Count < 3)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Minimum 3 values are required.");
                    continue;
                }

                bool isValidDate = DateOnly.TryParse(
                    strings.First().ToString(),
                    out DateOnly newDate
                );
                strings.RemoveAt(0);

                bool isValidValue = Decimal.TryParse(
                    strings.Last().ToString(),
                    out Decimal newDecimal
                );

                strings.RemoveAt(strings.Count - 1);
                //Need to trim to guard against name being spaces.
                string? newName = String.Join<String>(" ", strings).Trim();

                if (!isValidDate)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Date format incorrect.");
                }
                if (!isValidValue)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Value range must be: {Decimal.MinValue} to {Decimal.MaxValue}");
                }
                if (CheckInput(newName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Name can't be empty.");
                }

                if (isValidDate && isValidValue && !CheckInput(newName))
                {
                    if (newDecimal < 0)
                    {
                        history.Add(new Transaction(
                            TransactionTypes.Expense,
                            newDate,
                            newName,
                            newDecimal
                        ));
                    }
                    else if (newDecimal > 0)
                    {
                        history.Add(new Transaction(
                            TransactionTypes.Income,
                            newDate,
                            newName,
                            newDecimal
                        ));
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> Date: {newDate} Value: {newDecimal} Name: {newName}");
                }

            } 
            while (input.ToLower() != "exit");

            return history;
        }

        public static TransactionHistory Edit(TransactionHistory history) 
        {
            Console.WriteLine("Edit or Remove?");
            Console.Write("Input Method: ");
            string editMode = Console.ReadLine().Trim();

            //Check input
            switch (editMode.ToLower()) 
            {
                case "edit":
                    editMode = "Edit";
                    break;

                case "remove":
                    editMode = "Remove";
                    break;

                default:
                    Console.ForegroundColor= ConsoleColor.Red;
                    Console.WriteLine("> Input needs to be 'edit' or 'remove'. Aborting process.");
                    return history;
            }


            //Sort list
            Console.WriteLine("Sort method: Date / Name / Value");
            Console.Write("Sort by: ");
            string? sortInput = Console.ReadLine().Trim();
            history.Sort(sortInput);

            //Display
            Console.WriteLine("List method: All / Expense / Income");
            Console.Write("List by: ");
            string? listInput = Console.ReadLine().Trim();


            string? inputIndex;
            do 
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                history.ShowTransactions(listInput);

                Console.WriteLine("\nInputs: Index / Exit / List");
                Console.Write($"{editMode} Index: ");
                inputIndex = Console.ReadLine();
                bool isIndex = Int32.TryParse(inputIndex, out int itemIndex);

                if ( inputIndex.ToLower() == "exit" )
                {
                    continue;
                }

                if ( isIndex )
                {
                    switch ( editMode.ToLower() )
                    {
                        case "edit":
                            history.EditTransaction(itemIndex); 
                            break;

                        case "remove":
                            history.Remove(itemIndex);
                            break;
                        }
                    }
                else if (! isIndex )
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Index needs to be a number. Aborting.");
                    return history;
                }
            }
            while (inputIndex.ToLower() != "exit");

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
