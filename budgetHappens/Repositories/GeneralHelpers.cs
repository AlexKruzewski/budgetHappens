using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static List<string> GetCurrencies()
        {
            List<string> currencies = new List<string>();
            currencies.Add("£");
            currencies.Add("$");
            return currencies;
        }

    }
}
