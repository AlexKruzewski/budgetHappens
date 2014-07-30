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
            if (ValidateFields())
            {
                BudgetModel newBudget = new BudgetModel();

                newBudget.Name = TextBoxName.Text;
                newBudget.AmountPerPeriod = decimal.Parse(TextBoxAmount.Text);
                newBudget.Currency = ListPickerCurrency.SelectedItem.ToString();

                newBudget.PeriodLength = PeriodLength.Weekly;

                newBudget.BudgetStartDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), ListPickerStartDay.SelectedIndex.ToString());
                newBudget.CurrentPeriod = new PeriodModel(newBudget.BudgetStartDay, newBudget.AmountPerPeriod, newBudget.PeriodLength);
                System.Diagnostics.Debug.WriteLine(CheckboxDefault.IsChecked.ToString());
                //If the current budget is set as default we want to find the previous default budget and clear it default status.
                if (App.CurrentSession.Budgets.Count() == 0 | CheckboxDefault.IsChecked == true)
                {
                    System.Diagnostics.Debug.WriteLine("in here");
                    if (App.CurrentSession.Budgets.Count() > 0) 
                    {
                        BudgetModel tempBudget = App.CurrentSession.GetDefaultOrNextBudget();
                        System.Diagnostics.Debug.WriteLine(tempBudget.Default.ToString());
                        tempBudget.Default = false;
                    }
                   
                    newBudget.Default = true;
                }
                else
                {
                    newBudget.Default = false;
                }

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

        private void TextBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateNameField();
        }

        private void TextBoxName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ValidateNameField();
        }

        #endregion

        #region Methods

        /// <summary>
        /// In place so we can get around only one side
        /// of the if statement from being verfied (was in
        /// save statement)
        /// </summary>
        /// <returns>A boolean</returns>
        private bool ValidateFields()
        {
            if (!ValidateNameField() | !ValidateAmountField())
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates the amount field
        /// </summary>
        /// <returns>A boolean</returns>
        private bool ValidateAmountField()
        {
            bool amountValidates = GeneralHelpers.ValidateValue(TextBoxAmount.Text, DataType.Decimal);

            if(amountValidates)
            {
                TextBlockValidationAmount.Visibility = Visibility.Collapsed;
                TextBoxAmount.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TextBlockValidationAmount.Visibility = Visibility.Visible;
                TextBoxAmount.Background = new SolidColorBrush(Colors.Red);  
            }

            return amountValidates;
        }

        /// <summary>
        /// Validates the name field
        /// </summary>
        /// <returns>A boolean</returns>
        private bool ValidateNameField()
        {
            bool nameValidates = GeneralHelpers.ValidateValue(TextBoxName.Text, DataType.String);

            if (nameValidates)
            {
                TextBlockValidationName.Visibility = Visibility.Collapsed;
                TextBoxName.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TextBlockValidationName.Visibility = Visibility.Visible;
                TextBoxName.Background = new SolidColorBrush(Colors.Red);
            }

            return nameValidates;
        }
        #endregion

    }
}