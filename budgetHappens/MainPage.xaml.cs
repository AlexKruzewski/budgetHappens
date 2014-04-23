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

        public static Session currentSession = null;

        #endregion

        #region Constructors

        public MainPage()
        {
            InitializeComponent();

            if (currentSession == null)
                currentSession = new Session();

            Loaded += MainPage_Loaded;
        }
        #endregion

        #region Event Handlers

        void CurrentSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentBudget")
            {
                SetupLists();
                SetUpCurrentBudget();

                HighLightSelectItem(ListBudgets);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (currentSession.CurrentBudget != null)
                currentSession.PropertyChanged += CurrentSession_PropertyChanged;
            else
                SetUpCurrentBudget();

            SetupLists();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (currentSession.Budgets.Count > 0)
                HighLightSelectItem(ListBudgets);
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
            currentSession.DeleteBudget(currentSession.CurrentBudget);
            currentSession.SaveSession();
            currentSession.CurrentBudget = currentSession.GetDefaultOrNextBudget();

            if (currentSession.CurrentBudget == null)
                SetUpCurrentBudget();

            SetupLists();
        }

        private void ListPickerBudgets_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("aselection changed");
            System.Diagnostics.Debug.WriteLine("--------------------------");
            BudgetModel selectedBudget = (BudgetModel)ListPickerBudgets.SelectedItem;
            currentSession.CurrentBudget = selectedBudget;
        }

        private void ListBudgets_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("bselection changed");
            System.Diagnostics.Debug.WriteLine("--------------------------");
            BudgetModel selectedBudget = (BudgetModel)ListBudgets.SelectedItem;
            currentSession.CurrentBudget = selectedBudget;
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
            if (currentSession.CurrentBudget == null)
                currentSession.CurrentBudget = currentSession.GetDefaultOrNextBudget();

            if (currentSession.CurrentBudget != null)
                ShowCaseBugetsAvailable();
            else
                ShowCaseNoBudgets();
            
        }

        private void ShowCaseBugetsAvailable()
        {
            if (!currentSession.CurrentBudget.IsPeriodValid())
            {
                currentSession.CurrentBudget.CurrentPeriod = currentSession.CurrentBudget.StartNewPeriod();
                currentSession.SaveSession();
            }

            TextBlockCurrentAmount.Text = currentSession.CurrentBudget.Currency + currentSession.CurrentBudget.CurrentPeriod.CurrentAmount.ToString("0.00");
            TextBlockPeriodAmount.Text = "of " + currentSession.CurrentBudget.Currency + currentSession.CurrentBudget.CurrentPeriod.PeriodAmount.ToString("0.00") + " left";
            TextBlockDaysLeft.Text = currentSession.CurrentBudget.CurrentPeriod.DaysLeft.ToString("0") + " Days Left";
        }

        private void ShowCaseNoBudgets()
        {
            StackPanelCurrent.Children.Remove(TextBlockDaysLeft);
            StackPanelCurrent.Children.Remove(TextBlockBudgets);
            StackPanelCurrent.Children.Remove(ListPickerBudgets);
            StackPanelCurrent.Children.Remove(TextBlockCurrentAmount);
            StackPanelCurrent.Children.Remove(TextBlockPeriodAmount);
            StackPanelCurrent.Children.Remove(ButtonWithdraw);

            ApplicationBar.IsVisible = false;

            TextBlockNoBudgetSet.Visibility = System.Windows.Visibility.Visible;
            ButtonAddBuget.Visibility = System.Windows.Visibility.Visible;
        }

        private void SetupLists()
        {
            ListBudgets.ItemsSource = currentSession.Budgets;
            ListBudgets.SelectedItem = currentSession.CurrentBudget;
            ListPickerBudgets.ItemsSource = currentSession.Budgets;
            ListPickerBudgets.SelectedItem = currentSession.CurrentBudget;
            if (currentSession.CurrentBudget != null)
            {
                var withdrawalList = (from withdrawal in currentSession.CurrentBudget.CurrentPeriod.Withdrawals
                                     select withdrawal).OrderByDescending(x=>x.WithdrawalDate).ToList();

                ListWithdrawals.ItemsSource = withdrawalList;
            }
            else
                ListWithdrawals.ItemsSource = new List<WithdrawalModel>();

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