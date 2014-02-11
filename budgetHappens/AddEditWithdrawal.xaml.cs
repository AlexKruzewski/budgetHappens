using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using budgetHappens.ViewModels;

namespace budgetHappens
{
    public partial class AddEditWithdrawal : PhoneApplicationPage
    {
        Session currentSession = null;
        Withdrawal selectedWithdrawal = null;
        public AddEditWithdrawal()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            currentSession = (Session)PhoneApplicationService.Current.State["CurrentSession"];
            string value = "";
            if (NavigationContext.QueryString.TryGetValue("Edit",out value))
            {
                System.Diagnostics.Debug.WriteLine("im not null");
                selectedWithdrawal = (Withdrawal)PhoneApplicationService.Current.State["SelectedWithdrawal"];
                selectedWithdrawal = (from w in currentSession.CurrentBudget.CurrentPeriod.Withdrawals
                                          where w.Equals(selectedWithdrawal)
                                          select w).FirstOrDefault();
                TextBoxAmount.Text = selectedWithdrawal.Amount.ToString("0.00");
                TextBoxDescription.Text = selectedWithdrawal.Description;
                ButtonAddEditWithdrawal.Content = "Save";
                PhoneApplicationService.Current.State["SelectedWithdrawal"] = null;
            }
        }

        private void ButtonAddEditWithdrawal_Click_1(object sender, RoutedEventArgs e)
        {
            Period currentPeriod = currentSession.CurrentBudget.CurrentPeriod;
            decimal amount = decimal.Parse(TextBoxAmount.Text);
            if (selectedWithdrawal != null)
            {
                selectedWithdrawal.StringAmount = currentSession.CurrentBudget.Currency + amount.ToString("0.00");
                selectedWithdrawal.Amount = amount;
                selectedWithdrawal.Description = TextBoxDescription.Text;
            }
            else
            {
                currentPeriod.Withdrawals.Add(new Withdrawal(amount, TextBoxDescription.Text, currentSession.CurrentBudget.Currency));
            }
            currentSession.SaveSession();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}