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
    public partial class AddEditTransaction : PhoneApplicationPage
    {
        #region Parameters

        #endregion

        #region Attributes

        TransactionModel _selectedTransaction = null;
        bool _valuesValidate = true;
        private string _action = "";

        #endregion

        #region Constructors

        public AddEditTransaction()
        {
            InitializeComponent();
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
                    _selectedTransaction = (TransactionModel)PhoneApplicationService.Current.State["SelectedWithdrawal"];
                    _selectedTransaction = (from w in App.CurrentSession.CurrentBudget.CurrentPeriod.Transactions
                                           where w.Equals(_selectedTransaction)
                                           select w).FirstOrDefault();

                    TextBoxAmount.Text = _selectedTransaction.Amount.ToString("0.00");
                    TextBoxDescription.Text = _selectedTransaction.Description;

                    if(_selectedTransaction.TransactionType == TransactionType.Deposit)
                        TextBlockTitle.Text = "EditDeposit";
                    else
                        TextBlockTitle.Text = "EditWithdrawal";

                    btn.Text = "Save";
                    btn.IconUri = new Uri("/Assets/icons/save.png", UriKind.Relative);
                    PhoneApplicationService.Current.State["SelectedWithdrawal"] = null;
                    break;
                case "Deposit":
                    TextBlockTitle.Text = "Add Funds";
                    btn.Text = "Add Funds";
                    ApplicationBar.Buttons.RemoveAt(1);
                    break;
                default:
                    ApplicationBar.Buttons.RemoveAt(1);
                    break;
            }

        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            ValidateAmountField();
            if (_valuesValidate)
            {
                PeriodModel currentPeriod = App.CurrentSession.CurrentBudget.CurrentPeriod;
                decimal amount = decimal.Parse(TextBoxAmount.Text);

                switch(_action)
                {
                    case "Edit":
                        if (_selectedTransaction != null)
                        {
                            if(_selectedTransaction.TransactionType == TransactionType.Deposit)
                                _selectedTransaction.StringAmount = String.Format("{0}{1}",App.CurrentSession.CurrentBudget.Currency, amount.ToString("0.00"));
                            else
                                _selectedTransaction.StringAmount = String.Format("{0}{1}{2}","-",App.CurrentSession.CurrentBudget.Currency,amount.ToString("0.00"));

                            _selectedTransaction.Amount = amount;
                            _selectedTransaction.Description = TextBoxDescription.Text;
                        }
                        break;
                    case "Deposit":
                        currentPeriod.Transactions.Add(new TransactionModel(amount, TextBoxDescription.Text, App.CurrentSession.CurrentBudget.Currency, TransactionType.Deposit));
                        break;
                    default:
                        currentPeriod.Transactions.Add(new TransactionModel(amount, TextBoxDescription.Text, App.CurrentSession.CurrentBudget.Currency, TransactionType.Withdrawal));
                        break;

                }

                App.CurrentSession.SaveSession();
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            App.CurrentSession.DeleteTransaction(App.CurrentSession.CurrentBudget, this._selectedTransaction);
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

        private void TextBoxDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxDescription.Text == "Quick Withdrawal")
                TextBoxDescription.Text = "";
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