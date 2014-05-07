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
using budgetHappens.Models;
using budgetHappens.Repositories;
using System.Windows.Media;

namespace budgetHappens
{
    public partial class AddEditWithdrawal : PhoneApplicationPage
    {
        #region Parameters
        #endregion

        #region Attributes

        WithdrawalModel _selectedWithdrawal = null;
        bool _valuesValidate = true;
        #endregion

        #region Constructors

        public AddEditWithdrawal()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string value = "";

            if (NavigationContext.QueryString.TryGetValue("Edit", out value))
            {
                _selectedWithdrawal = (WithdrawalModel)PhoneApplicationService.Current.State["SelectedWithdrawal"];
                _selectedWithdrawal = (from w in App.CurrentSession.CurrentBudget.CurrentPeriod.Withdrawals
                                      where w.Equals(_selectedWithdrawal)
                                      select w).FirstOrDefault();

                TextBoxAmount.Text = _selectedWithdrawal.Amount.ToString("0.00");
                TextBoxDescription.Text = _selectedWithdrawal.Description;
                ButtonAddEditWithdrawal.Content = "Save";
                Grid.SetColumnSpan(ButtonAddEditWithdrawal, 1);

                ButtonRemoveWithdrawal.Visibility = System.Windows.Visibility.Visible;

                PhoneApplicationService.Current.State["SelectedWithdrawal"] = null;
            }
        }

        private void ButtonAddEditWithdrawal_Click_1(object sender, RoutedEventArgs e)
        {
            ValidateAmountField();
            if (_valuesValidate)
            {
                PeriodModel currentPeriod = App.CurrentSession.CurrentBudget.CurrentPeriod;
                decimal amount = decimal.Parse(TextBoxAmount.Text);

                if (_selectedWithdrawal != null)
                {
                    _selectedWithdrawal.StringAmount = App.CurrentSession.CurrentBudget.Currency + amount.ToString("0.00");
                    _selectedWithdrawal.Amount = amount;
                    _selectedWithdrawal.Description = TextBoxDescription.Text;
                }
                else
                {
                    currentPeriod.Withdrawals.Add(new WithdrawalModel(amount, TextBoxDescription.Text, App.CurrentSession.CurrentBudget.Currency));
                }

                App.CurrentSession.SaveSession();
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private void ButtonRemoveWithdrawal_Click_1(object sender, RoutedEventArgs e)
        {
            App.CurrentSession.DeleteWithdrawal(App.CurrentSession.CurrentBudget, this._selectedWithdrawal);
            App.CurrentSession.SaveSession();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void TextBoxAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            _valuesValidate = GeneralHelpers.ValidateValue(TextBoxAmount.Text, DataType.Decimal);

            ValidateAmountField();
        }

        private void TextBoxAmount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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