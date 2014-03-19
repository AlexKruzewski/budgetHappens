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

        List<QuickWithdrawModel> Amounts = new List<QuickWithdrawModel>();
        Session currentSession = null;

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
            string currencyIcon = currentSession.CurrentBudget.Currency;
            Amounts.Add(new QuickWithdrawModel() { Amount = 1.00M, StringAmount = currencyIcon + "1.00" });
            Amounts.Add(new QuickWithdrawModel() { Amount = 1.50M, StringAmount = currencyIcon + "1.50" });
            Amounts.Add(new QuickWithdrawModel() { Amount = 2.00M, StringAmount = currencyIcon + "2.00" });
            Amounts.Add(new QuickWithdrawModel() { Amount = 2.50M, StringAmount = currencyIcon + "2.50" });
            Amounts.Add(new QuickWithdrawModel() { Amount = 5.00M, StringAmount = currencyIcon + "5.00" });
            Amounts.Add(new QuickWithdrawModel() { Amount = 10.00M, StringAmount = currencyIcon + "10.00" });
            Amounts.Add(new QuickWithdrawModel() { Amount = 15.00M, StringAmount = currencyIcon + "15.00" });
            Amounts.Add(new QuickWithdrawModel() { Amount = 20.00M, StringAmount = currencyIcon + "20.00" });
            Amounts.Add(new QuickWithdrawModel() { Amount = 25.00M, StringAmount = currencyIcon + "25.00" });
        }
        #endregion

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            currentSession = (Session)PhoneApplicationService.Current.State["CurrentSession"];

            if(Amounts.Count == 0)
                PopulateData();

            ListQuickWithdraws.ItemsSource = Amounts;
        }

        private void LongListSelector_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector list = sender as LongListSelector;

            if (list == null)
                return;

            QuickWithdrawModel selectedModel = list.SelectedItem as QuickWithdrawModel;

            currentSession.CurrentBudget.CurrentPeriod.Withdrawals.Add(new WithdrawalModel(selectedModel.Amount, "Quick Withdrawal", currentSession.CurrentBudget.Currency));

            currentSession.SaveSession();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        }

        private void ButtonOtherAmount_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddEditWithdrawal.xaml", UriKind.Relative));
        }

        #endregion

    }
}