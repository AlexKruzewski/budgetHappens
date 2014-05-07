using budgetHappens.Repositories;
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
    public class BudgetModel
    {

        #region Parameters

        public string Name { get; set; }
        public string Currency { get; set; }
        public DayOfWeek BudgetStartDay { get; set; }
        public PeriodLength PeriodLength { get; set; }
        public Decimal AmountPerPeriod { get; set; }
        public PeriodModel CurrentPeriod { get; set; }
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

        /// <summary>
        /// Checks to see if the current budget period is valid
        /// (has not extended beyond its end date)
        /// </summary>
        /// <returns>Returns bool</returns>
        internal bool IsPeriodValid()
        {
            return (DateTime.Now < this.CurrentPeriod.EndDate) ? true : false;
        }

        /// <summary>
        /// Starts a new period for the current budget.
        /// </summary>
        /// <returns>Returns a new budget period</returns>
        internal PeriodModel StartNewPeriod()
        {
            PeriodModel newPeriod = new PeriodModel(this.BudgetStartDay, this.AmountPerPeriod, this.PeriodLength);
            return newPeriod;
        }

        #endregion

    }

}
