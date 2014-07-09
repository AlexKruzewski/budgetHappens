using budgetHappens.Models;
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

        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }

        private void populateBudgets()
        {
            string dataFromStorage = "";
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("Budgets", out dataFromStorage))
            {
                Budgets = JsonConvert.DeserializeObject<ObservableCollection<BudgetModel>>(dataFromStorage);
            }
            else
                Budgets = new ObservableCollection<BudgetModel>();
        }

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

        public void SaveSession()
        {

            var data = JsonConvert.SerializeObject(this.Budgets);
           
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["Budgets"] = data;
            settings.Save();

            //PhoneApplicationService.Current.State["CurrentSession"] = this;
        }

        internal void AddBudget(BudgetModel newBudget)
        {
            this.Budgets.Add(newBudget);
        }

        internal void DeleteBudget(BudgetModel budget)
        {
            this.Budgets.Remove(budget);
        }


        internal void ChangeCurrentBudget(BudgetModel selectedBudget)
        {
            this.CurrentBudget = selectedBudget;
        }

        internal void DeleteTransaction(BudgetModel budgetModel, TransactionModel withdrawalModel)
        {
            budgetModel.CurrentPeriod.Transactions.Remove(withdrawalModel);
        }

        #endregion
    }
}
