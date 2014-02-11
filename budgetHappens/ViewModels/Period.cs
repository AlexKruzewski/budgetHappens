using budgetHappens.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetHappens.ViewModels
{
    public class Period
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PeriodAmount { get; set; }
        public decimal CurrentAmount 
        { 
            get 
            {
                decimal tempAmount = PeriodAmount;
                foreach (var withdrawal in Withdrawals)
                {
                    tempAmount -= withdrawal.Amount;
                }
                return tempAmount;
            } 
        }
        public ObservableCollection<Withdrawal> Withdrawals { get; set; }

        public Period(DayOfWeek startDay, decimal periodAmount, PeriodLength length)
        {
            switch (length)
            {
                case PeriodLength.Weekly:
                    StartDate = GeneralHelpers.StartOfWeek(DateTime.Now, startDay);
                    break;
                case PeriodLength.Monthly:
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
                case PeriodLength.Yearly:
                    StartDate = new DateTime(DateTime.Now.Year, 1, 1);
                    break;
            }

            switch (length)
            {
                case PeriodLength.Weekly:
                    EndDate = StartDate.AddDays(7);
                    break;
                case PeriodLength.Monthly:
                    EndDate = StartDate.AddMonths(1);
                    break;
                case PeriodLength.Yearly:
                    EndDate = StartDate.AddYears(1);
                    break;
                default:
                    break;
            }
            PeriodAmount = periodAmount;
            Withdrawals = new ObservableCollection<Withdrawal>();
        }
    }
}
