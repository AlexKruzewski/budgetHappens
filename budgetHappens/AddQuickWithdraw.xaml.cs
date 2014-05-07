using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using budgetHappens.Models;
using budgetHappens.ViewModels;

namespace budgetHappens
{
    public partial class AddQuickWithdraw : PhoneApplicationPage
    {
        #region Attributes

        private List<QuickWithdrawModel> _amounts = new List<QuickWithdrawModel>();

        #endregion

        #region Constructors
        public AddQuickWithdraw()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void PopulateData()
        {
            string currencyIcon = App.CurrentSession.CurrentBudget.Currency;
            _amounts.Add(new QuickWithdrawModel() { Amount = 1.00M, StringAmount = currencyIcon + "1.00" });
            _amounts.Add(new QuickWithdrawModel() { Amount = 1.50M, StringAmount = currencyIcon + "1.50" });
            _amounts.Add(new QuickWithdrawModel() { Amount = 2.00M, StringAmount = currencyIcon + "2.00" });
            _amounts.Add(new QuickWithdrawModel() { Amount = 2.50M, StringAmount = currencyIcon + "2.50" });
            _amounts.Add(new QuickWithdrawModel() { Amount = 5.00M, StringAmount = currencyIcon + "5.00" });
            _amounts.Add(new QuickWithdrawModel() { Amount = 10.00M, StringAmount = currencyIcon + "10.00" });
            _amounts.Add(new QuickWithdrawModel() { Amount = 15.00M, StringAmount = currencyIcon + "15.00" });
            _amounts.Add(new QuickWithdrawModel() { Amount = 20.00M, StringAmount = currencyIcon + "20.00" });
            _amounts.Add(new QuickWithdrawModel() { Amount = 25.00M, StringAmount = currencyIcon + "25.00" });
        }
        #endregion

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          
            if(_amounts.Count == 0)
                PopulateData();

            ListQuickWithdraws.ItemsSource = _amounts;
        }

        private void LongListSelector_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector list = sender as LongListSelector;

            if (list == null)
                return;

            QuickWithdrawModel selectedModel = list.SelectedItem as QuickWithdrawModel;

            App.CurrentSession.CurrentBudget.CurrentPeriod.Withdrawals.Add(new WithdrawalModel(selectedModel.Amount, "Quick Withdrawal", App.CurrentSession.CurrentBudget.Currency));

            App.CurrentSession.SaveSession();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        }

        private void ButtonOtherAmount_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddEditWithdrawal.xaml", UriKind.Relative));
        }

        #endregion

    }
}