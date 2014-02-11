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
using budgetHappens.Repositories;
using budgetHappens.Models;
namespace budgetHappens
{
    public partial class AddBudget : PhoneApplicationPage
    {
        public AddBudget()
        {
            InitializeComponent();
            ListPickerPeriod.ItemsSource = GeneralHelpers.GetNames<PeriodLength>();
            ListPickerCurrency.ItemsSource = GeneralHelpers.GetCurrencies();
            ListPickerStartDay.ItemsSource = GeneralHelpers.GetNames<DayOfWeek>();
        }

        private void ButtonSave_Click_1(object sender, RoutedEventArgs e)
        {
            Budget newBudget = new Budget();
            newBudget.Name = TextBoxName.Text;
            newBudget.AmountPerPeriod = decimal.Parse(TextBoxAmount.Text);
            newBudget.Currency = ListPickerCurrency.SelectedItem.ToString();
            PeriodLength periodLength = (PeriodLength)Enum.Parse(typeof(PeriodLength), ListPickerPeriod.SelectedItem.ToString());
            newBudget.PeriodLength = periodLength;
            newBudget.BudgetStartDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), ListPickerStartDay.SelectedIndex.ToString());
            newBudget.CurrentPeriod = new Period(newBudget.BudgetStartDay, newBudget.AmountPerPeriod, newBudget.PeriodLength);
            Session currentSession = (Session)PhoneApplicationService.Current.State["CurrentSession"];
            newBudget.Default = (currentSession.Budgets.Count() == 0) ? true : false;
            currentSession.Budgets.Add(newBudget);
            currentSession.CurrentBudget = newBudget;
            currentSession.SaveSession();
           
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        }
    }
}