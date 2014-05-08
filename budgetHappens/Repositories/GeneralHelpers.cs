using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace budgetHappens.Repositories
{
    public static class GeneralHelpers
    {
        public static List<string> GetNames<TEnum>() where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException(String.Format("Type '{0}' is not an enum", type.Name));

            return (
              from field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
              where field.IsLiteral
              select field.Name)
            .ToList<string>();
        }

        /// <summary>
        /// Gets the date of the given day in the week closest to before the currenct date
        /// </summary>
        /// <param name="dt">Todays Date</param>
        /// <param name="startOfWeek">The day of the week</param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Returns the list of currencies available
        /// </summary>
        /// <returns>Returns a list of currencies</returns>
        public static List<string> GetCurrencies()
        {
            List<string> currencies = new List<string>();
            currencies.Add("£");
            currencies.Add("$");
            currencies.Add("€");
            return currencies;
        }

        public static void GetItemsRecursive<T>(DependencyObject parents, ref List<T> objectList) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parents);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parents, i);
                if (child is T)
                {
                    objectList.Add(child as T);
                }
                GetItemsRecursive<T>(child, ref objectList);
            }

            return;
        }

        /// <summary>
        /// Validates the given data type
        /// </summary>
        /// <param name="value">The values to be validated</param>
        /// <param name="dataType">The datatype to validate it agasint.</param>
        /// <returns></returns>
        public static bool ValidateValue(object value, DataType dataType)
        {
            bool validates = true;

            switch (dataType)
            {
                case DataType.Int:
                    int tempInt;
                    validates = int.TryParse(value.ToString(), out tempInt);
                    break;
                case DataType.Decimal:
                    decimal tempDecimal;
                    validates = decimal.TryParse(value.ToString(), out tempDecimal);
                    break;
                case DataType.Date:
                    DateTime tempDate;
                    validates = DateTime.TryParse(value.ToString(), out tempDate);
                    break;
                case DataType.String:
                    validates = !String.IsNullOrEmpty(value.ToString());
                    break;
            }

            return validates;
        }

        /// <summary>
        /// Saves the given data to isolated storage under the given file name
        /// </summary>
        /// <param name="isoFileName">The name of the file to save to</param>
        /// <param name="value">The string value of what you are saving</param>
        public static void SaveDataToIsolatedStorage(string isoFileName, string value)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            StreamWriter sw = new StreamWriter(isoStore.OpenFile(isoFileName, FileMode.OpenOrCreate));
            sw.Write(value);
            sw.Close();
        }

        /// <summary>
        /// Gets the data from isolated storage
        /// </summary>
        /// <param name="isoFileName">Filename to which to get the data from</param>
        /// <returns></returns>
        public static string GetDataFromIsolatedStorage(string isoFileName)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists(isoFileName))
            {
                // This method loads the data from isolated storage, if it is available.
                StreamReader sr = new StreamReader(isoStore.OpenFile(isoFileName, FileMode.Open));
                string data = sr.ReadToEnd();
                sr.Close();

                return data;
            }
            else
                return "";
        }

    }
}
