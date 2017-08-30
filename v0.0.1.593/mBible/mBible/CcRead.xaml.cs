﻿using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Tasks;
using mBible.RealData;

namespace mBible
{
    /// <summary>
    /// Sample code for LongListMultiSelector
    /// </summary>
    public partial class CcRead : PhoneApplicationPage
    {
        private AppSettings settings = new AppSettings();
        class PivotCallbacks
        {
            public Action Init { get; set; }
            public Action OnActivated { get; set; }
            public Action<CancelEventArgs> OnBackKeyPress { get; set; }
        }

        //Dictionary<object, PivotCallbacks> _callbacks;

        /// <summary>
        /// Initializes the dictionary of delegates to call when each pivot is selected
        /// </summary>
        public CcRead()
        {
            InitializeComponent();
            this.Loaded += CcRead_Loaded;

            // Init = CreateVerseApplicationBarItems,
            //  OnActivated = OnVersePivotItemActivated,
            //   OnBackKeyPress = OnVerseBackKeyPressed
            CreateVerseApplicationBarItems();
            OnVersePivotItemActivated();
            //OnVerseBackKeyPressed();
        }

        void CcRead_Loaded(object sender, RoutedEventArgs e)
        {
            txtBookName.Text = settings.CurrentEnglishSetting + " " + settings.SelectedChapterSetting;
        }
        /// <summary>
        /// Resets the application bar
        /// </summary>
        void ClearApplicationBar()
        {
            while (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }

            while (ApplicationBar.MenuItems.Count > 0)
            {
                ApplicationBar.MenuItems.RemoveAt(0);
            }
        }

        #region MultiselectListbox item
        ApplicationBarIconButton select;
        ApplicationBarIconButton share;
        ApplicationBarIconButton smaller;
        ApplicationBarIconButton bigger;
        ApplicationBarMenuItem copy;
        ApplicationBarMenuItem appsettings;
        ApplicationBarMenuItem howitworks;
        ApplicationBarMenuItem helpsupport;

        /// <summary>
        /// Creates ApplicationBar items for Verse list
        /// </summary>
        private void CreateVerseApplicationBarItems()
        {
            select = new ApplicationBarIconButton();
            select.IconUri = new Uri("/Assets/AppBar/appbar_list.png", UriKind.RelativeOrAbsolute);
            select.Text = "select";
            select.Click += OnSelectClick;
            
            share = new ApplicationBarIconButton();
            share.IconUri = new Uri("/Assets/AppBar/appbar_share.png", UriKind.RelativeOrAbsolute);
            share.Text = "share";
            share.Click += OnShareClick;

            copy = new ApplicationBarMenuItem();
            copy.Text = "copy selected verses";
            copy.Click += OnCopyClick;

            appsettings = new ApplicationBarMenuItem();
            appsettings.Text = "App Settings";
            appsettings.Click += OnSettingsClick;

            howitworks = new ApplicationBarMenuItem();
            howitworks.Text = "How It Works";

            helpsupport = new ApplicationBarMenuItem();
            helpsupport.Text = "Help and Support";
            
            smaller = new ApplicationBarIconButton();
            smaller.IconUri = new Uri("/Assets/AppBar/appbar_minus.png", UriKind.RelativeOrAbsolute);
            smaller.Text = "smaller";
            smaller.Click += OnSmallerClick;

            bigger = new ApplicationBarIconButton();
            bigger.IconUri = new Uri("/Assets/AppBar/appbar_add.png", UriKind.RelativeOrAbsolute);
            bigger.Text = "bigger";
            bigger.Click += OnBiggerClick;
            
            /*
            markAsRead = new ApplicationBarMenuItem();
            markAsRead.Text = "mark as read";
            markAsRead.Click += OnMarkAsReadClick;

            markAsUnread = new ApplicationBarMenuItem();
            markAsUnread.Text = "mark as unread";
            markAsUnread.Click += OnMarkAsUnreadClick;
             */
        }

        /// <summary>
        /// Called when Verse pivot item is activated : makes sure that selection is disabled and updates the application bar
        /// </summary>
        void OnVersePivotItemActivated()
        {
            if (VerseList.IsSelectionEnabled)
            {
                VerseList.IsSelectionEnabled = false; // Will update the application bar too
            }
            else
            {
                SetupVerseApplicationBar();
            }
        }

        /// <summary>
        /// Configure ApplicationBar items for Verse list
        /// </summary>
        private void SetupVerseApplicationBar()
        {
            ClearApplicationBar();

            if (VerseList.IsSelectionEnabled)
            {
                ApplicationBar.Buttons.Add(share);
                ApplicationBar.MenuItems.Add(copy);
                UpdateVerseApplicationBar();
            }
            else
            {
                ApplicationBar.Buttons.Add(select);
                ApplicationBar.Buttons.Add(smaller);
                ApplicationBar.Buttons.Add(bigger);
                ApplicationBar.MenuItems.Add(appsettings);
                ApplicationBar.MenuItems.Add(howitworks);
                ApplicationBar.MenuItems.Add(helpsupport);
            }
            ApplicationBar.IsVisible = true;
        }

        /// <summary>
        /// Updates the Verse Application bar items depending on selection
        /// </summary>
        private void UpdateVerseApplicationBar()
        {
            if (VerseList.IsSelectionEnabled)
            {
                bool hasSelection = ((VerseList.SelectedItems != null) && (VerseList.SelectedItems.Count > 0));
                share.IsEnabled = hasSelection;
                copy.IsEnabled = hasSelection;
                //markAsUnread.IsEnabled = hasSelection;
            }
        }

        /// <summary>
        /// Back Key Pressed = leaves the selection mode
        /// </summary>
        /// <param name="e"></param>
        protected void OnVerseBackKeyPressed(CancelEventArgs e)
        {
            if (VerseList.IsSelectionEnabled)
            {
                VerseList.IsSelectionEnabled = false;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Passes the Verse list in selection mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSelectClick(object sender, EventArgs e)
        {
            VerseList.IsSelectionEnabled = true;
        }

        /// <summary>
        /// Deletes selected items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnShareClick(object sender, EventArgs e)
        {
            String shareStr;
            shareStr = settings.CurrentEnglishSetting + " " + settings.SelectedChapterSetting + "\n\n";
            
            foreach (VerseObject verse_obj in VerseList.SelectedItems)
            {
                shareStr = shareStr + verse_obj.Verse + "\n";
            }
            
            ShareStatusTask shareStatusTask = new ShareStatusTask();
            shareStatusTask.Status = shareStr + "\n\n Via mBible";
            shareStatusTask.Show();
        }
        
        void OnCopyClick(object sender, EventArgs e)
        {
            String copyStr;
            copyStr = settings.CurrentEnglishSetting + " " + settings.SelectedChapterSetting + "\n\n";

            foreach (VerseObject verse_obj in VerseList.SelectedItems)
            {
                copyStr = copyStr + verse_obj.Verse + "\n";
            }
            
            Clipboard.SetText(copyStr + "\n\n Via mBible"); 
        }
        
        void OnDeleteClick(object sender, EventArgs e)
        {
            IList source = VerseList.ItemsSource as IList;
            while (VerseList.SelectedItems.Count > 0)
            {
                source.Remove((VerseObject)VerseList.SelectedItems[0]);
            }
        }

        /// <summary>
        /// Mark all items as read
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMarkAsReadClick(object sender, EventArgs e)
        {
            foreach (VerseObject obj in VerseList.SelectedItems)
            {
                obj.Unread = false;
            }

            VerseList.IsSelectionEnabled = false;
        }

        /// <summary>
        /// Mark all items as unread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMarkAsUnreadClick(object sender, EventArgs e)
        {
            foreach (VerseObject obj in VerseList.SelectedItems)
            {
                obj.Unread = true;
            }

            VerseList.IsSelectionEnabled = false;
        }

        /// <summary>
        /// Adjusts the user interface according to the number of selected Verses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVerseListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateVerseApplicationBar();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVerseListIsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetupVerseApplicationBar();
        }



        /// <summary>
        /// Tap on an item : depending on the selection state, either unselect it or consider it as read
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemContentTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            VerseObject item = ((FrameworkElement)sender).DataContext as VerseObject;
            if (item != null)
            {
                item.Unread = false;
            }
        }

        #endregion


        #region Databinding
        /// <summary>
        /// Hide the application bar
        /// </summary>
        void SetupBoundBuddiesApplicationBar()
        {
            ApplicationBar.IsVisible = false;
        }


        #endregion

        private void OnSettingsClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CcSettings.xaml", UriKind.Relative));
        }

        public void OnSmallerClick(object sender, EventArgs e)
        {
            int newfonts = settings.FontSizeSetting - 2;
            settings.FontSizeSetting = newfonts;
        }

        public void OnBiggerClick(object sender, EventArgs e)
        {
            int newfonts = settings.FontSizeSetting + 2;
            settings.FontSizeSetting = newfonts;
        }

    }
}