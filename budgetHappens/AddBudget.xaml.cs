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
        bool _valuesValidate = true;
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
            if(_valuesValidate)
            {
                BudgetModel newBudget = new BudgetModel();

                newBudget.Name = TextBoxName.Text;
                newBudget.AmountPerPeriod = decimal.Parse(TextBoxAmount.Text);
                newBudget.Currency = ListPickerCurrency.SelectedItem.ToString();

                newBudget.PeriodLength = PeriodLength.Weekly;

                newBudget.BudgetStartDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), ListPickerStartDay.SelectedIndex.ToString());
                newBudget.CurrentPeriod = new PeriodModel(newBudget.BudgetStartDay, newBudget.AmountPerPeriod, newBudget.PeriodLength);

                newBudget.Default = (App.CurrentSession.Budgets.Count() == 0) ? true : false;
                App.CurrentSession.Budgets.Add(newBudget);
                App.CurrentSession.CurrentBudget = newBudget;

                App.CurrentSession.SaveSession();

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

        /// <summary>
        /// Validates the amount field to ensure the values added are valid.
        /// </summary>
        private void ValidateAmountField()
        {
            _valuesValidate = GeneralHelpers.ValidateValue(TextBoxAmount.Text, DataType.Decimal);
            if (!_valuesValidate)
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