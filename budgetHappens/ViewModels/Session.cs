using budgetHappens.Models;
using budgetHappens.Repositories;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace budgetHappens.ViewModels
{
    public class Session
    {
        #region Parameters

        public ObservableCollection<BudgetModel> Budgets
        {
            get
            {
                return _budgets;
            }
            set
            {
                _budgets = value;
            }
        }

        public BudgetModel CurrentBudget
        {
            get
            {
                return _currentBudget;
            }
            set
            {
                _currentBudget = value;
                if (_currentBudget != null)
                    RaisePropertyChanged("CurrentBudget");
            }
        }

        #endregion

        #region Attributes

        private ObservableCollection<BudgetModel> _budgets;
        private BudgetModel _currentBudget;
        private string _fileName = "Budgets.xml";
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Session()
        {
            this.populateBudgets();
        }

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// Is called when a property is changed.
        /// Sets up a new event with the property that has changed,
        /// which notifies any listeners
        /// </summary>
        /// <param name="name">Name of the property that has changed</param>
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Populates the budget list from the
        /// data given in isolated storage
        /// </summary>
        private void populateBudgets()
        {
            string dataFromStorage = GeneralHelpers.GetDataFromIsolatedStorage(_fileName);
            if (!String.IsNullOrEmpty(dataFromStorage))
            {
                Budgets = JsonConvert.DeserializeObject<ObservableCollection<BudgetModel>>(dataFromStorage);
            }
            else
                Budgets = new ObservableCollection<BudgetModel>();
        }

        /// <summary>
        /// Returns the Default budget, if there
        /// is no default budget we find the next available
        /// budget and set that to the new default
        /// </summary>
        /// <returns>A budget</returns>
        public BudgetModel GetDefaultOrNextBudget()
        {
            BudgetModel defaultBudget = null;

            if (Budgets.Count != 0)
            {
                defaultBudget = (BudgetModel)(from b in Budgets
                                         where b.Default == true
                                         select b).FirstOrDefault();

                if (defaultBudget == null)
                {
                    defaultBudget = this.Budgets[0];
                    defaultBudget.Default = (defaultBudget.Default) ? true : false;

                    this.SaveSession();
                }
            }

            return defaultBudget;
        }

        /// <summary>
        /// Sends the budget data to isolated storage
        /// </summary>
        public void SaveSession()
        {
            var data = JsonConvert.SerializeObject(this.Budgets);

            GeneralHelpers.SaveDataToIsolatedStorage(_fileName, data);
        }

        /// <summary>
        /// Adds a new budget to the budget list
        /// </summary>
        /// <param name="newBudget">Budget to be added</param>
        internal void AddBudget(BudgetModel newBudget)
        {
            this.Budgets.Add(newBudget);
        }

        /// <summary>
        /// Deletes a given budget from the budget list
        /// </summary>
        /// <param name="budget">Budget to be removed</param>
        internal void DeleteBudget(BudgetModel budget)
        {
            this.Budgets.Remove(budget);
        }

        /// <summary>
        /// Changes the currently selected budget
        /// </summary>
        /// <param name="selectedBudget">The budget to change to</param>
        internal void ChangeCurrentBudget(BudgetModel selectedBudget)
        {
            this.CurrentBudget = selectedBudget;
        }

        /// <summary>
        /// Handles the removal of a withdrawal
        /// </summary>
        /// <param name="budgetModel">The budget to remove the withdrawalfrom</param>
        /// <param name="withdrawalModel">The withdrawal to remove</param>
        internal void DeleteWithdrawal(BudgetModel budgetModel, WithdrawalModel withdrawalModel)
        {
            budgetModel.CurrentPeriod.Withdrawals.Remove(withdrawalModel);
        }

        #endregion
    }
}
