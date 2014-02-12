﻿using budgetHappens.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace budgetHappens.Models
{
    public class Budget
    {

        #region Parameters

        public string Name { get; set; }
        public string Currency { get; set; }
        public DayOfWeek BudgetStartDay { get; set; }
        public PeriodLength PeriodLength { get; set; }
        public Decimal AmountPerPeriod { get; set; }
        public Period CurrentPeriod { get; set; }
        public bool Default { get; set; }

        #endregion

        #region Attributes

        public const string Key = "Budget";
        
        #endregion

        #region Constructors
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        internal bool IsPeriodValid()
        {
            return (DateTime.Now < this.CurrentPeriod.EndDate) ? true : false;
        }

        internal Period StartNewPeriod()
        {
            Period newPeriod = new Period(this.BudgetStartDay, this.AmountPerPeriod, this.PeriodLength);
            return newPeriod;
        }

        #endregion

    }

}
