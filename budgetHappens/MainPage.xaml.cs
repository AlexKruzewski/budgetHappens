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
                SetUpCurrentBudget();
                SetupLists();

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

        private void ListBudgets_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
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

            TextBlockBudgetName.Text = currentSession.CurrentBudget.Name;
            TextBlockCurrentAmount.Text = currentSession.CurrentBudget.Currency + currentSession.CurrentBudget.CurrentPeriod.CurrentAmount.ToString("0.00");
            TextBlockPeriodAmount.Text = "of " + currentSession.CurrentBudget.Currency + currentSession.CurrentBudget.CurrentPeriod.PeriodAmount.ToString("0.00") + " left";
            double daysLeft = (currentSession.CurrentBudget.CurrentPeriod.EndDate - DateTime.Now).TotalDays;
            TextBlockDaysLeft.Text = daysLeft.ToString("0") + " Days Left";
        }

        private void ShowCaseNoBudgets()
        {
            StackPanelCurrent.Children.Remove(TextBlockDaysLeft);
            StackPanelCurrent.Children.Remove(TextBlockBudgetName);
            StackPanelCurrent.Children.Remove(TextBlockCurrentAmount);
            StackPanelCurrent.Children.Remove(TextBlockPeriodAmount);

            ApplicationBar.IsVisible = false;

            TextBlockNoBudgetSet.Visibility = System.Windows.Visibility.Visible;
            ButtonAddBuget.Visibility = System.Windows.Visibility.Visible;
        }

        private void SetupLists()
        {
            ListBudgets.ItemsSource = currentSession.Budgets;
            ListBudgets.SelectedItem = currentSession.CurrentBudget;

            if (currentSession.CurrentBudget != null)
                ListWithdrawals.ItemsSource = currentSession.CurrentBudget.CurrentPeriod.Withdrawals;
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