﻿using budgetHappens.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetHappens.Models
{
    public class Period
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
                foreach (var withdrawal in Withdrawals)
                {
                    tempAmount -= withdrawal.Amount;
                }
                return tempAmount;
            }
        }

        public ObservableCollection<Withdrawal> Withdrawals { get; set; }

        #endregion

        #region Attributes
        #endregion

        #region Constructors

        public Period(DayOfWeek startDay, decimal periodAmount, PeriodLength length)
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
            Withdrawals = new ObservableCollection<Withdrawal>();
        }

        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion
        
    }
}
