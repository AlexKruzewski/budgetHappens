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
            Budget selectedBudget = (Budget)ListBudgets.SelectedItem;
            currentSession.CurrentBudget = selectedBudget;
        }

        private void ListWithdrawals_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            PhoneApplicationService.Current.State["SelectedWithdrawal"] = (Withdrawal)ListWithdrawals.SelectedItem;
            NavigationService.Navigate(new Uri("/AddEditWithdrawal.xaml?Edit=true", UriKind.Relative));
        }

        #endregion

        #region Methods

        private void SetUpCurrentBudget()
        {
            if (currentSession.CurrentBudget == null)
                currentSession.CurrentBudget = currentSession.GetDefaultOrNextBudget();

            if (currentSession.CurrentBudget != null)
            {

                if (!currentSession.CurrentBudget.IsPeriodValid())
                {
                    currentSession.CurrentBudget.CurrentPeriod = currentSession.CurrentBudget.StartNewPeriod();
                    currentSession.SaveSession();
                }

                BuildLocalizedApplicationBar();

                TextBlockBudgetName.Text = currentSession.CurrentBudget.Name;
                TextBlockCurrentAmount.Text = currentSession.CurrentBudget.Currency + currentSession.CurrentBudget.CurrentPeriod.CurrentAmount.ToString("0.00");
                TextBlockPeriodAmount.Text = "of " + currentSession.CurrentBudget.Currency + currentSession.CurrentBudget.CurrentPeriod.PeriodAmount.ToString("0.00") + " left";
                TextBlockDaysLeft.Text = (currentSession.CurrentBudget.CurrentPeriod.EndDate.Day - DateTime.Now.Day).ToString() + " Days Left";

            }
            else
            {

                StackPanelCurrent.Children.Remove(TextBlockDaysLeft);
                StackPanelCurrent.Children.Remove(TextBlockBudgetName);
                StackPanelCurrent.Children.Remove(TextBlockCurrentAmount);
                StackPanelCurrent.Children.Remove(TextBlockPeriodAmount);

                Button addBudgetbutton = new Button();
                addBudgetbutton.Content = "Add Budget";
                addBudgetbutton.Click += AddBudgetButton_Click_1;
                StackPanelCurrent.Children.Add(addBudgetbutton);
            }

        }

        private void SetupLists()
        {
            ListBudgets.ItemsSource = currentSession.Budgets;
            ListBudgets.SelectedItem = currentSession.CurrentBudget;

            if (currentSession.CurrentBudget != null)
                ListWithdrawals.ItemsSource = currentSession.CurrentBudget.CurrentPeriod.Withdrawals;
            else
                ListWithdrawals.ItemsSource = new List<Withdrawal>();

        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton addWithdrawalButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Add.png", UriKind.Relative));
            addWithdrawalButton.Text = "Add Withdrawal";
            addWithdrawalButton.Click += AddWithdrawalButton_Click;
            ApplicationBar.Buttons.Add(addWithdrawalButton);

            ApplicationBarMenuItem addNewBudgetButton = new ApplicationBarMenuItem("Add New Budget");
            addNewBudgetButton.Click += AddBudgetButton_Click_1;
            ApplicationBar.MenuItems.Add(addNewBudgetButton);

            ApplicationBarMenuItem deleteCurrentBudgetButton = new ApplicationBarMenuItem();
            deleteCurrentBudgetButton.Text = "Delete Current Budget";
            deleteCurrentBudgetButton.Click += DeleteCurrentBudget_Click;
            ApplicationBar.MenuItems.Add(deleteCurrentBudgetButton);
        }



        public static void GetItemsRecursive<T>(DependencyObject parents, ref List<T> objectList) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parents);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parents, i);
                if (child is T)
                {
                    objectList.Add(child as T);
                }
                GetItemsRecursive<T>(child, ref objectList);
            }

            return;
        }

        public static void HighLightSelectItem(LongListSelector selectedList)
        {
            List<UserControl> listItems = new List<UserControl>();
            GetItemsRecursive<UserControl>(selectedList, ref listItems);

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