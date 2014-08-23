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
    public partial class AddEditBudget : PhoneApplicationPage
    {
        #region Parameters
        #endregion

        #region Attributes
        BudgetModel _currentBudget = null;
        private string _action = "";
        #endregion

        #region Constructors

        public AddEditBudget()
        {
            InitializeComponent();

            ListPickerCurrency.ItemsSource = GeneralHelpers.GetCurrencies();
            ListPickerLength.ItemsSource = GeneralHelpers.GetNames<PeriodLength>();
            ListPickerStartDay.ItemsSource = GeneralHelpers.GetNames<DayOfWeek>();
        }

        #endregion

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var gotQuery = NavigationContext.QueryString.TryGetValue("Action", out _action);
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            switch (_action)
            {
                case "Edit":
                    _currentBudget = App.CurrentSession.CurrentBudget;

                    TextBlockTitle.Text = "EditBudget";

                    TextBoxName.Text = _currentBudget.Name;
                    TextBoxAmount.Text = _currentBudget.AmountPerPeriod.ToString();
                    ListPickerCurrency.SelectedItem = _currentBudget.Currency;
                    ListPickerLength.SelectedItem = _currentBudget.PeriodLength.ToString();
                    ListPickerStartDay.SelectedItem = _currentBudget.BudgetStartDay.ToString();
                    CheckboxDefault.IsChecked = _currentBudget.Default;
                    break;
                case "Create":
                    ApplicationBar.Buttons.RemoveAt(1);
                    break;
                default:
                    ApplicationBar.Buttons.RemoveAt(1);
                    break;
            }

        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (ValidateFields())
            {
                if(_currentBudget == null)
                    _currentBudget = new BudgetModel();

                _currentBudget.Name = TextBoxName.Text;
                _currentBudget.AmountPerPeriod = decimal.Parse(TextBoxAmount.Text);
                _currentBudget.Currency = ListPickerCurrency.SelectedItem.ToString();

                _currentBudget.PeriodLength = (PeriodLength)Enum.Parse(typeof(PeriodLength), ListPickerLength.SelectedIndex.ToString());

                _currentBudget.BudgetStartDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), ListPickerStartDay.SelectedIndex.ToString());
                
                if (_action == "Create")
                    _currentBudget.CurrentPeriod = new PeriodModel(_currentBudget.BudgetStartDay, _currentBudget.AmountPerPeriod, _currentBudget.PeriodLength);

                //If the current budget is set as default we want to find the previous default budget and clear it default status.
                if (App.CurrentSession.Budgets.Count() == 0 | CheckboxDefault.IsChecked == true)
                {
                    if (App.CurrentSession.Budgets.Count() > 0) 
                    {
                        BudgetModel tempBudget = App.CurrentSession.GetDefaultOrNextBudget();
                        tempBudget.Default = false;
                    }

                    _currentBudget.Default = true;
                }
                else
                {
                    _currentBudget.Default = false;
                }

                if (_action == "Create")
                    App.CurrentSession.Budgets.Add(_currentBudget);
                App.CurrentSession.CurrentBudget = _currentBudget;

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

        private void ListPickerLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PeriodLength periodLength = (PeriodLength)Enum.Parse(typeof(PeriodLength), ListPickerLength.SelectedIndex.ToString());
            switch (periodLength)
            {
                case PeriodLength.Yearly:
                    TextblockStartDay.Visibility = Visibility.Collapsed;
                    ListPickerStartDay.Visibility = Visibility.Collapsed;
                    TextBlockDefault.SetValue(Grid.RowProperty, 4);
                    CheckboxDefault.SetValue(Grid.RowProperty, 4);
                    break;
                case PeriodLength.Monthly:
                    TextblockStartDay.Visibility = Visibility.Collapsed;
                    ListPickerStartDay.Visibility = Visibility.Collapsed;
                    TextBlockDefault.SetValue(Grid.RowProperty, 4);
                    CheckboxDefault.SetValue(Grid.RowProperty, 4);
                    break;
                case PeriodLength.Weekly:
                    TextblockStartDay.Visibility = Visibility.Visible;
                    ListPickerStartDay.Visibility = Visibility.Visible;
                    TextBlockDefault.SetValue(Grid.RowProperty, 5);
                    CheckboxDefault.SetValue(Grid.RowProperty, 5);
                    break;
            }
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            App.CurrentSession.DeleteBudget(_currentBudget);
            App.CurrentSession.SaveSession();

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
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