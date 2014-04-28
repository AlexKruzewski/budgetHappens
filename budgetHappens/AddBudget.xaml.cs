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
using System.Windows.Media;
namespace budgetHappens
{
    public partial class AddBudget : PhoneApplicationPage
    {
        #region Parameters
        #endregion

        #region Attributes
        bool valuesValidate = true;
        #endregion

        #region Constructors

        public AddBudget()
        {
            InitializeComponent();

            ListPickerCurrency.ItemsSource = GeneralHelpers.GetCurrencies();
            ListPickerStartDay.ItemsSource = GeneralHelpers.GetNames<DayOfWeek>();
        }

        #endregion

        #region Event Handlers

        private void ButtonSave_Click_1(object sender, RoutedEventArgs e)
        {
            ValidateAmountField();
            if(valuesValidate)
            {
                BudgetModel newBudget = new BudgetModel();

                newBudget.Name = TextBoxName.Text;
                newBudget.AmountPerPeriod = decimal.Parse(TextBoxAmount.Text);
                newBudget.Currency = ListPickerCurrency.SelectedItem.ToString();

                newBudget.PeriodLength = PeriodLength.Weekly;

                newBudget.BudgetStartDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), ListPickerStartDay.SelectedIndex.ToString());
                newBudget.CurrentPeriod = new PeriodModel(newBudget.BudgetStartDay, newBudget.AmountPerPeriod, newBudget.PeriodLength);

                Session currentSession = (Session)PhoneApplicationService.Current.State["CurrentSession"];

                newBudget.Default = (currentSession.Budgets.Count() == 0) ? true : false;
                currentSession.Budgets.Add(newBudget);
                currentSession.CurrentBudget = newBudget;

                currentSession.SaveSession();

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

        }

        private void TextBoxAmount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ValidateAmountField();
        }

        private void TextBoxAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateAmountField();
        }

        #endregion

        #region Methods
        private void ValidateAmountField()
        {
            valuesValidate = GeneralHelpers.ValidateValue(TextBoxAmount.Text, DataType.Decimal);
            if (!valuesValidate)
            {
                TextBlockValidationAmount.Visibility = Visibility.Visible;
                TextBoxAmount.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                TextBlockValidationAmount.Visibility = Visibility.Collapsed;
                TextBoxAmount.Background = new SolidColorBrush(Colors.White);
            }

        }
        #endregion

    }
}