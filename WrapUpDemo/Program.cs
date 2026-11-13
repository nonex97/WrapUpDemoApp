using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapUpDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<PersonModel> people = new List<PersonModel>
            {
                new PersonModel {FirstName = "Neven", LastName = "GrguricDarnit", Email = "neven@dev.me"},
                new PersonModel {FirstName = "Yuki", LastName = "Fox", Email = "yuki@fox.me"},
                new PersonModel {FirstName = "John", LastName = "Float", Email = "john@float.me"},
            };

            List<CarModel> cars = new List<CarModel>
            {
                new CarModel {Manufacturer = "Cupra", Model = "DARNLeon"},
                new CarModel {Manufacturer = "Porsche", Model = "911 GT3"},
                new CarModel {Manufacturer = "Audiheck", Model = "RS6"}
            };

            DataAccess<PersonModel> peopleData = new DataAccess<PersonModel>();
            peopleData.BadEntryFound += PeopleData_BadEntryFound;
            peopleData.SaveToCSV(people, @"C:\temp\csharp_files\people.csv"); // @ means there are no escape characters so we don't need to use \\ or something else

            DataAccess<CarModel> carData = new DataAccess<CarModel>();
            carData.BadEntryFound += CarData_BadEntryFound;
            carData.SaveToCSV(cars, @"C:\temp\csharp_files\cars.csv");

            Console.ReadLine();
        }

        private static void CarData_BadEntryFound(object sender, CarModel e)
        {
            Console.WriteLine($"Bad entry found for {e.Manufacturer} {e.Model}");
        }

        private static void PeopleData_BadEntryFound(object sender, PersonModel e)
        {
            Console.WriteLine($"Bad entry found for {e.FirstName} {e.LastName}");
        }
    }

    public class DataAccess<T> where T: new()  // T has to have an empty constructor (T is not the list, but the PersonModel or CarModel)
    {
        public event EventHandler<T> BadEntryFound;
        public void SaveToCSV(List<T> items, string filePath)
        {
            List<string> rows = new List<string>();
            T entry = new T();
            var cols = entry.GetType().GetProperties(); // for PersonModel that is: FirstName, LastName, Email

            string row = "";
            foreach (var col in cols)
            {
                row += $",{col.Name}"; // it's easier to put the comma at the start of each row since it's easier to delete the first 
            }
            row = row.Substring(1); // deletes the comma from the start (FirstName,LastName,Email), this is our header row
            rows.Add(row);

            foreach (var item in items)
            {
                row = "";
                bool badWordDetected = false;

                foreach (var col in cols)
                {
                    string val = col.GetValue(item, null).ToString();

                    badWordDetected = BadWordDetector(val);
                    if (badWordDetected == true)
                    {
                        BadEntryFound?.Invoke(this, item);
                        break;
                    }

                    row += $",{val}";
                }

                if (badWordDetected == false) // not writing the row to the list if a bad word is detected
                {
                    row = row.Substring(1);
                    rows.Add(row);
                }
            }

            File.WriteAllLines(filePath, rows); // saving the CSV file
        }

        private bool BadWordDetector(string stringToTest)
        {
            bool output = false;
            string lowerCaseTest = stringToTest.ToLower();

            if (lowerCaseTest.Contains("darn") || lowerCaseTest.Contains("heck"))
            {
                output = true;
            }

            return output;
        }
    }
}