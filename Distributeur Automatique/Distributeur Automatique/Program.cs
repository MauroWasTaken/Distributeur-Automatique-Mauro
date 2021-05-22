using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributeur_Automatique
{
    class Program
    {
        static DateTime manualTime;
        static decimal currentBalance,storedBalance;
        static List<Item> items = new List<Item>();
        static decimal[] purcheaseHistory = new decimal[24];
        static void Main(string[] args)
        {
            Setup();
            do
            {
                GetCommand();
            } while (true);
        }
        /// <summary>
        /// mise en place des differents items dans le selecta
        /// </summary>
        static void Setup()
        {
            items.Add(new Item("Smarlies", "A01", 10, (decimal)1.60));
            items.Add(new Item("Carampar", "A02", 5, (decimal)0.60));
            items.Add(new Item("Avril", "A03", 2, (decimal)2.10));
            items.Add(new Item("KokoKola", "A04", 1, (decimal)2.95));
        }
        /// <summary>
        /// lis la commande inserée par l'utilisateur et lance la bonne fonction
        /// </summary>
        static void GetCommand()
        {
            string command;
            command = Console.ReadLine();
            string[] splittedStrings = command.Split('(');

            switch (splittedStrings[0])
            {
                case "Insert":
                    Insert(decimal.Parse(splittedStrings[1].Remove(splittedStrings[1].Length-1)));
                    break;
                case "Choose":
                    Choose(splittedStrings[1].Split('"')[1]);
                    break;
                case "GetChange":
                    GetChange();
                    break;
                case "GetBalance":
                    GetBalance();
                    break;
                case "SetTime":
                    string[] seperatedDate = splittedStrings[1].Split('"')[1].Split('T');
                    SetTime(DateTime.Parse(seperatedDate[0]+" "+ seperatedDate[1]));
                    break;
                case "Validation":
                    Validation();
                    break;
                default:
                    break;
            }
            
        }
        /// <summary>
        /// ajoute une valeur à la machine
        /// </summary>
        /// <param name="amount">valeur à ajouter</param>
        static void Insert(decimal amount)
        {
            currentBalance += amount;
        }
        /// <summary>
        /// choisit et achete un produit
        /// </summary>
        /// <param name="code">produit choisit</param>
        static void Choose(string code)
        {
            bool foundItem = false;
            foreach(Item item in items)
            {
                if (item.Code == code)
                {
                    if (currentBalance >= item.Price) {
                        if (item.Purchease())
                        {
                            if (manualTime != null)
                            {
                                purcheaseHistory[manualTime.Hour]+= item.Price;
                            }
                            else
                            {
                                purcheaseHistory[DateTime.Now.Hour] += item.Price;
                            }
                            Console.WriteLine("Vending "+item.Name);
                            storedBalance += item.Price;
                            currentBalance -= item.Price;
                        }
                        else
                        {
                            Console.WriteLine("Item " + item.Name+ ": Out of stock!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not enough money!");
                    }
                    foundItem = true;
                }
            }
            if(!foundItem) Console.WriteLine("Invalid selection!");
        }
        /// <summary>
        /// retourne l'argent pas utilisé au client
        /// </summary>
        static void GetChange()
        {
            Console.WriteLine(currentBalance);
            currentBalance = 0;
        }
        /// <summary>
        /// montre l'argent dans la machine
        /// </summary>
        static void GetBalance()
        {
            Console.WriteLine(storedBalance);
        }
        /// <summary>
        /// permets de changer l'heure du selecta
        /// </summary>
        /// <param name="time">heure à changer</param>
        static void SetTime(DateTime time)
        {
            manualTime = time;
        }
        /// <summary>
        /// montre l'historique d'achats faits dans la machine
        /// </summary>
        static void Validation()
        {
            int hourCounter = 0;
            List<List<decimal>> sortedList = new List<List<decimal>>();
            foreach (decimal value in purcheaseHistory)
            {
                if (value != 0)
                {
                    int listCounter = 0;
                    int indexToChange=-1;
                    foreach (List<decimal> values in sortedList)
                    {
                        if (value > values[1] & indexToChange==-1)
                        {
                            indexToChange = listCounter;
                        }
                        listCounter++;
                    }

                    if (indexToChange == -1)
                    {
                        sortedList.Add(new List<decimal> { hourCounter, value });
                    }
                    else
                    {
                        sortedList.Insert(indexToChange, new List<decimal> { hourCounter, value });
                    }
                }
                hourCounter++;
            }
            foreach (List<decimal> values in sortedList)
            {
                Console.WriteLine("Hour "+values[0]+" generated a revenue of "+values[1].ToString("0.00"));
            }

        }
    }
    /// <summary>
    /// classe contenant toutes les information d'un item
    /// </summary>
    class Item
    {
        string name;
        string code;
        int quantity;
        decimal price;

        public string Name { get => name; }
        public string Code { get => code; }
        public int Quantity { get => quantity;}
        public decimal Price { get => price;}

        public Item(string name, string code,int quantity,decimal price)
        {
            this.name = name;
            this.code = code;
            this.quantity = quantity;
            this.price = price;
        }

        public bool Purchease()
        {
            if (quantity >0)
            {
                quantity--;
                return true;
            }
            return false;
        }

    }
}
