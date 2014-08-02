using budgetHappens.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetHappens.Models
{
    public class PeriodModel
    {
        #region Parameters

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PeriodAmount { get; set; }

        public decimal CurrentAmount
        {
            get
            {
                decimal tempAmount = PeriodAmount;
                foreach (var withdrawal in Transactions)
                {
                    if(withdrawal.TransactionType == TransactionType.Deposit)
                        tempAmount += withdrawal.Amount;
                    else if(withdrawal.TransactionType == TransactionType.Withdrawal)
                        tempAmount -= withdrawal.Amount;
                }
                return tempAmount;
            }
        }

        public double DaysLeft
        {
            get
            {
                return (this.EndDate - DateTime.Now).TotalDays;
            }
        }

        public ObservableCollection<TransactionModel> Transactions { get; set; }

        #endregion

        #region Attributes
        #endregion

        #region Constructors
        /// <summary>
        /// Sets up a budget period.
        /// </summary>
        /// <param name="startDay">Day of Week</param>
        /// <param name="periodAmount">Decimal value of the budget amount</param>
        /// <param name="length">How long this budget period will last.</param>
        public PeriodModel(DayOfWeek startDay, decimal periodAmount, PeriodLength length)
        {
            switch (length)
            {
                case PeriodLength.Weekly:
                    StartDate = GeneralHelpers.StartOfWeek(DateTime.Now, startDay);
                    EndDate = StartDate.AddDays(7);
                    break;
                case PeriodLength.Monthly:
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    EndDate = StartDate.AddMonths(1);
                    break;
                case PeriodLength.Yearly:
                    StartDate = new DateTime(DateTime.Now.Year, 1, 1);
                    EndDate = StartDate.AddYears(1);
                    break;
            }

            PeriodAmount = periodAmount;
            Transactions = new ObservableCollection<TransactionModel>();
        }

        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion
        
    }
}
