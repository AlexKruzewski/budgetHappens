using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using budgetHappens.Resources;
using budgetHappens.ViewModels;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using budgetHappens.Models;
using budgetHappens.Repositories;

namespace budgetHappens
{
    /// <summary>
    /// This is the main page of the application.
    /// It is loaded as the first page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        #region Properties
        #endregion

        #region Attributes

        #endregion

        #region Constructors
        public MainPage()
        {
            InitializeComponent();
            App.CurrentSession.PropertyChanged += CurrentSession_PropertyChanged;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Is ran when the A property change event is called
        /// in the session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentBudget")
            {
                SetUpCurrentBudget();
                SetupWithdrawalList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // I want to clear the nav backstack as there shouldnt
            // be a reason for a person wanting to go back to any
            // previous pages.
            while (this.NavigationService.BackStack.Any())
                this.NavigationService.RemoveBackEntry();

            SetUpCurrentBudget();
            SetupBudgetList();
            SetupWithdrawalList();
        }

        private void AddBudgetButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddBudget.xaml", UriKind.Relative));
        }

        private void AddWithdrawalButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddQuickWithdraw.xaml", UriKind.Relative));
        }

        private void DeleteCurrentBudget_Click(object sender, EventArgs e)
        {
            App.CurrentSession.DeleteBudget(App.CurrentSession.CurrentBudget);
            App.CurrentSession.SaveSession();
            App.CurrentSession.CurrentBudget = App.CurrentSession.GetDefaultOrNextBudget();

            if(App.CurrentSession.CurrentBudget == null)
                SetUpCurrentBudget();

            SetupBudgetList();
            SetupWithdrawalList();
        }

        private void ListPickerBudgets_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            BudgetModel selectedBudget = (BudgetModel)ListPickerBudgets.SelectedItem;
            App.CurrentSession.CurrentBudget = selectedBudget;
        }

        private void ListWithdrawals_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            PhoneApplicationService.Current.State["SelectedWithdrawal"] = (WithdrawalModel)ListWithdrawals.SelectedItem;
            NavigationService.Navigate(new Uri("/AddEditWithdrawal.xaml?Edit=true", UriKind.Relative));
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        #endregion

        #region Methods
        /// <summary>
        /// Get the current budget or makes a call
        /// for one if no current budget is set.
        /// Then shows determines if fields need to be hidden
        /// or displayed.
        /// </summary>
        private void SetUpCurrentBudget()
        {

            if (App.CurrentSession.CurrentBudget == null)
                App.CurrentSession.CurrentBudget = App.CurrentSession.GetDefaultOrNextBudget();

            if (App.CurrentSession.CurrentBudget != null)
                ShowCaseBugetsAvailable();
            else
                ShowCaseNoBudgets();
            
        }

        /// <summary>
        /// Shows the fields that need to be displayed if
        /// there is a budget available. 
        /// Also handles some string formatting.
        /// If the current budget period is no longer valid, a new one is setup.
        /// </summary>
        private void ShowCaseBugetsAvailable()
        {
            if (!App.CurrentSession.CurrentBudget.IsPeriodValid())
            {
                App.CurrentSession.CurrentBudget.CurrentPeriod = App.CurrentSession.CurrentBudget.StartNewPeriod();
                App.CurrentSession.SaveSession();
            }

            TextBlockCurrentAmount.Text = App.CurrentSession.CurrentBudget.Currency + App.CurrentSession.CurrentBudget.CurrentPeriod.CurrentAmount.ToString("0.00");
            TextBlockPeriodAmount.Text = String.Format(
                                            "of {0}{1} left", 
                                            App.CurrentSession.CurrentBudget.Currency, 
                                            App.CurrentSession.CurrentBudget.CurrentPeriod.PeriodAmount.ToString("0.00"));
            TextBlockDaysLeft.Text = String.Format("{0} Days Left", App.CurrentSession.CurrentBudget.CurrentPeriod.DaysLeft.ToString("0"));

            if (App.CurrentSession.CurrentBudget.CurrentPeriod.CurrentAmount < 0)
                TextBlockCurrentAmount.Foreground = new SolidColorBrush(Colors.Red);
        }

        /// <summary>
        /// Hides fields if no budgets are set up.
        /// </summary>
        private void ShowCaseNoBudgets()
        {
            StackPanelCurrent.Children.Remove(TextBlockDaysLeft);
            StackPanelCurrent.Children.Remove(GridBudgets);
            StackPanelCurrent.Children.Remove(TextBlockCurrentAmount);
            StackPanelCurrent.Children.Remove(TextBlockPeriodAmount);
            StackPanelCurrent.Children.Remove(ButtonWithdraw);

            ApplicationBar.IsVisible = false;

            TextBlockNoBudgetSet.Visibility = System.Windows.Visibility.Visible;
            ButtonAddBuget.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Sets up the list of withdrawals based on the current budget
        /// </summary>
        private void SetupWithdrawalList()
        {
            if (App.CurrentSession.CurrentBudget != null)
            {
                var withdrawalList = (from withdrawal in App.CurrentSession.CurrentBudget.CurrentPeriod.Withdrawals
                                     select withdrawal).OrderByDescending(x=>x.WithdrawalDate).ToList();

                ListWithdrawals.ItemsSource = withdrawalList;
            }
            else
                ListWithdrawals.ItemsSource = new List<WithdrawalModel>();

        }

        /// <summary>
        /// Sets up the budget listpicker.
        /// </summary>
        private void SetupBudgetList()
        {
            ListPickerBudgets.ItemsSource = App.CurrentSession.Budgets;
            ListPickerBudgets.SelectedItem = App.CurrentSession.CurrentBudget;
            ListPickerBudgets.SelectionChanged += ListPickerBudgets_SelectionChanged_1;
        }

        #endregion


    }
}