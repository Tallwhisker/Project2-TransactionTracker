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

                if (EqualStrings(itemType, "exit"))
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
                string? itemName = Console.ReadLine().Trim();
                if (EmptyString(itemName))
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
            while ( ! EqualStrings(itemType, "exit"));      
            
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

                if ( EqualStrings(input.Trim(), "exit") )
                { 
                    continue; 
                }
                else if (EmptyString(input))
                {
                    Console.WriteLine("Input can't be empty. Starting over.");
                    continue;
                }


                List<String> strings = [.. input.Split(" ")];
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

                if ( ! isValidDate)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Date format incorrect.");
                }
                if ( ! isValidValue)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Value range must be: {Decimal.MinValue} to {Decimal.MaxValue}");
                }
                if (EmptyString(newName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Name can't be empty.");
                }

                if (isValidDate && isValidValue && ! EmptyString(newName))
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
            while ( ! EqualStrings(input, "exit"));

            return history;
        }

        public static TransactionHistory Edit(TransactionHistory history) 
        {
            Console.WriteLine("Edit or Remove?");
            Console.Write("Method: ");
            string? editMode = Console.ReadLine().Trim();

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
            Console.WriteLine("\nSort method: Date / Name / Value");
            Console.Write("Sort by: ");
            string? sortInput = Console.ReadLine().Trim();
            history.Sort(sortInput);

            //Display
            Console.WriteLine("\nList method: All / Expense / Income");
            Console.Write("List: ");
            string? listInput = Console.ReadLine().Trim();


            string? inputIndex;
            do 
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                history.ShowTransactions(listInput);

                Console.WriteLine("\nInputs: Index / Exit / List");
                Console.Write($"{editMode} Index: ");
                inputIndex = Console.ReadLine().Trim();
                bool isIndex = Int32.TryParse(inputIndex, out int itemIndex);

                if (EqualStrings(inputIndex, "exit") )
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
            while ( ! EqualStrings(inputIndex, "exit"));

            return history;
        }


        internal static bool EmptyString(string input)
        {
            bool isEmpty = String.IsNullOrEmpty(input);

            if (isEmpty)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            return isEmpty;
        }

        internal static bool EqualStrings(string input1, string input2)
        {
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            return String.Equals(input1, input2, comparison);
        }

    }
}
