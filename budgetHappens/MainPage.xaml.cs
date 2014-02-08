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

namespace budgetHappens
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static Session CurrentSession = null;
        //public List<Withdrawal> Withdrawals;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if(CurrentSession == null)
                CurrentSession = new Session();
        }

        void CurrentSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.PropertyName + " has changed");
            if (e.PropertyName == "CurrentBudget")
            {
                setUpCurrentBudget();
                ListWithdrawals.ItemsSource = CurrentSession.CurrentBudget.CurrentPeriod.Withdrawals;
                HighLightSelectItem(ListBudgets);
            }  
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            foreach (var s in CurrentSession.Budgets)
            {
                System.Diagnostics.Debug.WriteLine(s.ToString());
            }
            if(CurrentSession.CurrentBudget != null)
                CurrentSession.PropertyChanged += CurrentSession_PropertyChanged;
            else
                setUpCurrentBudget();
            SetupLists();
        }

        private void AddBudgetButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddBudget.xaml", UriKind.Relative));
        }

        void appBarButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddEditWithdrawal.xaml", UriKind.Relative));
        }

        void DeleteCurrentBudget_Click(object sender, EventArgs e)
        {
            CurrentSession.DeleteBudget(CurrentSession.CurrentBudget);
            CurrentSession.SaveSession();
            CurrentSession.CurrentBudget = CurrentSession.GetDefaultOrNextBudget();
            if (CurrentSession.CurrentBudget == null)
                setUpCurrentBudget();
        }

        private void NavigateToAddBudgetPage()
        {
            NavigationService.Navigate(new Uri("/AddBudget.xaml", UriKind.Relative));
        }

        private void setUpCurrentBudget()
        {
            bool noBudgetSet = true;
            if (CurrentSession.CurrentBudget == null)
                CurrentSession.CurrentBudget = CurrentSession.GetDefaultOrNextBudget();

            if (CurrentSession.CurrentBudget != null)
            {
                if (!CurrentSession.CurrentBudget.IsPeriodValid())
                {
                    CurrentSession.CurrentBudget.CurrentPeriod = CurrentSession.CurrentBudget.StartNewPeriod();
                    CurrentSession.SaveSession();
                }
                BuildLocalizedApplicationBar();
                TextBlockBudgetName.Text = CurrentSession.CurrentBudget.Name;
                TextBlockCurrentAmount.Text = CurrentSession.CurrentBudget.Currency + CurrentSession.CurrentBudget.CurrentPeriod.CurrentAmount.ToString("0.00");
                TextBlockPeriodAmount.Text = "of " + CurrentSession.CurrentBudget.Currency + CurrentSession.CurrentBudget.CurrentPeriod.PeriodAmount.ToString("0.00") + " left";
                TextBlockDaysLeft.Text = (CurrentSession.CurrentBudget.CurrentPeriod.EndDate.Day - DateTime.Now.Day).ToString() + " Days Left";
                noBudgetSet = false;
            }

            if (noBudgetSet)
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
            ListBudgets.ItemsSource = CurrentSession.Budgets;
            ListBudgets.SelectedItem = CurrentSession.CurrentBudget;
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Add.png", UriKind.Relative));
            appBarButton.Text = "Add Withdrawal";
            appBarButton.Click += appBarButton_Click;
            ApplicationBar.Buttons.Add(appBarButton);

            ApplicationBarMenuItem AddNewBudget = new ApplicationBarMenuItem("Add New Budget");
            AddNewBudget.Click += AddBudgetButton_Click_1;
            ApplicationBar.MenuItems.Add(AddNewBudget);

            ApplicationBarMenuItem DeleteCurrentBudget = new ApplicationBarMenuItem();
            DeleteCurrentBudget.Text = "Delete Current Budget";
            DeleteCurrentBudget.Click += DeleteCurrentBudget_Click;
            ApplicationBar.MenuItems.Add(DeleteCurrentBudget);
        }

        private void ListBudgets_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Budget selectedBudget = (Budget)ListBudgets.SelectedItem;
            CurrentSession.CurrentBudget = selectedBudget;
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
            System.Diagnostics.Debug.WriteLine("selected item: " + ((Budget)selectedList.SelectedItem).Name);
            List<UserControl> listItems = new List<UserControl>();
            GetItemsRecursive<UserControl>(selectedList, ref listItems);
            
            foreach (var item in listItems)
            {
                if (selectedList.SelectedItem.Equals(item.DataContext))
                {
                    System.Diagnostics.Debug.WriteLine("found selected" + ((Budget)selectedList.SelectedItem).Name);
                    VisualStateManager.GoToState(item, "Selected", true);
                }
                else
                    VisualStateManager.GoToState(item, "Normal", true);
            }
        }

        private void ListWithdrawals_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            PhoneApplicationService.Current.State["SelectedWithdrawal"] = (Withdrawal)ListWithdrawals.SelectedItem;
            NavigationService.Navigate(new Uri("/AddEditWithdrawal.xaml?Edit=true", UriKind.Relative));
        }

    }
}