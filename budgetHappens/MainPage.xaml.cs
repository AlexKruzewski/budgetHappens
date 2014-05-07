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

        void CurrentSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentBudget")
            {
                System.Diagnostics.Debug.WriteLine("Budget has changed");
                SetUpCurrentBudget();
                SetupWithdrawalList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetUpCurrentBudget();
            SetupBudgetList();
            SetupWithdrawalList();
        }

        private void AddBudgetButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddBudget.xaml", UriKind.Relative));
        }

        void AddWithdrawalButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddQuickWithdraw.xaml", UriKind.Relative));
        }

        void addNewWithdrawalButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddEditWithdrawal.xaml", UriKind.Relative));
        }

        void DeleteCurrentBudget_Click(object sender, EventArgs e)
        {
            App.CurrentSession.DeleteBudget(App.CurrentSession.CurrentBudget);
            App.CurrentSession.SaveSession();
            App.CurrentSession.CurrentBudget = App.CurrentSession.GetDefaultOrNextBudget();

            if (App.CurrentSession.CurrentBudget == null)
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

        #endregion

        #region Methods

        private void SetUpCurrentBudget()
        {

            if (App.CurrentSession.CurrentBudget == null)
                App.CurrentSession.CurrentBudget = App.CurrentSession.GetDefaultOrNextBudget();

            if (App.CurrentSession.CurrentBudget != null)
                ShowCaseBugetsAvailable();
            else
                ShowCaseNoBudgets();
            
        }

        private void ShowCaseBugetsAvailable()
        {
            if (!App.CurrentSession.CurrentBudget.IsPeriodValid())
            {
                App.CurrentSession.CurrentBudget.CurrentPeriod = App.CurrentSession.CurrentBudget.StartNewPeriod();
                App.CurrentSession.SaveSession();
            }

            TextBlockCurrentAmount.Text = App.CurrentSession.CurrentBudget.Currency + App.CurrentSession.CurrentBudget.CurrentPeriod.CurrentAmount.ToString("0.00");
            TextBlockPeriodAmount.Text = "of " + App.CurrentSession.CurrentBudget.Currency + App.CurrentSession.CurrentBudget.CurrentPeriod.PeriodAmount.ToString("0.00") + " left";
            TextBlockDaysLeft.Text = App.CurrentSession.CurrentBudget.CurrentPeriod.DaysLeft.ToString("0") + " Days Left";
            if (App.CurrentSession.CurrentBudget.CurrentPeriod.CurrentAmount < 0)
                TextBlockCurrentAmount.Foreground = new SolidColorBrush(Colors.Red);
        }

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

        private void SetupBudgetList()
        {
            ListPickerBudgets.ItemsSource = App.CurrentSession.Budgets;
            ListPickerBudgets.SelectedItem = App.CurrentSession.CurrentBudget;
            ListPickerBudgets.SelectionChanged += ListPickerBudgets_SelectionChanged_1;
        }

        public void HighLightSelectItem(LongListSelector selectedList)
        {
            List<UserControl> listItems = new List<UserControl>();
            GeneralHelpers.GetItemsRecursive<UserControl>(selectedList, ref listItems);

            foreach (var item in listItems)
            {
                if (selectedList.SelectedItem.Equals(item.DataContext))
                    VisualStateManager.GoToState(item, "Selected", true);
                else
                    VisualStateManager.GoToState(item, "Normal", true);
            }
        }

        #endregion

    }
}