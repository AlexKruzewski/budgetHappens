using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace budgetHappens
{
    /// <summary>
    /// This page displays the detail about the apps and how to contact the author.
    /// </summary>
    public partial class About : PhoneApplicationPage
    {
        #region Properties
        #endregion

        #region Attributes
        #endregion

        #region Constructors
        public About()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TextBlockVersion.Text = "Version: " + System.Xml.Linq.XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
            base.OnNavigatedTo(e);
        }

        #endregion

        #region Methods
        #endregion




    }
}