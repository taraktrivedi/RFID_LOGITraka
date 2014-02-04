using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data;
using System.Windows;
using System.Threading;
using Microsoft.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using System.Collections;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
//using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using DL580_9130;



namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for HATrakaMain.xaml
    /// </summary>
    public partial class HATrakaMain : Window
    {
        // data grid and misc variables
        
        public static int autoStopCount = 1;
        public static int countToolTip = 1;
        public static int masterCount = 0;
        public static int srCount = 1;
        public static bool isReaderDisconnected = true;
        static int maxDuplicateCount = 4; // *change* here Adjustable value based on number of tags
        static int sendCount = 0; // serial port output timer var

        //Timers
        public static DispatcherTimer masterTimer = new DispatcherTimer();
        public static DispatcherTimer cTimer = new DispatcherTimer();
        public static DispatcherTimer productTimer = new DispatcherTimer();
        public static DispatcherTimer allScanTimer = new DispatcherTimer();
        public static DispatcherTimer outputSignal = new DispatcherTimer();

        // Focus elements within the parent
        public static IInputElement getFocussedControl;

        //Create a dictionary 
        Dictionary<int, string> masterTagIDdB = new Dictionary<int, string>();

        // Instantiate dataset and TA
        ztATdbLocalDataSet1 assetInfoDataSet = new ztATdbLocalDataSet1();
        ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter AssetInfoTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter();

        private DataTable dataGridDBTable = new DataTable();

        // Search list DB dictionaries for auto complete
        public static List<string> searchList = new List<string>();
        public static List<string> searchListDB = new List<string>();

        // Create and add to the dictionary important tips and notes that may be useful to the user
        Dictionary<int, string> toolTipDB = new Dictionary<int, string>();

        // Map Image points
        private Point origin;
        private Point start;
        private Point p;
        private Matrix mOriginal;
        private bool flagSliderZoom = true;
        private bool flagSliderUNZoom = true;
        private int maxRange;
        public static string mapEle;

        //Alert Notifications emails and SMS.
        public static string emailContent; 
        public static string superAdminEmail;
        public static string superAdminSMS;
        
        //Notify icon and other app settings
        private System.Windows.Forms.NotifyIcon myNotifyIcon;
        public static bool minimizeInTray = true;
        public static int shutdownAppAfterXHours = 0;
        private bool isLogout = false;

        //Chart parameters
        private int totalMovingAssets = 0;

        //Reader parameters
        private string readerIP = "";
        public static int readerNumber = 0;
        private bool pingResult = false;

        // *change* here
        private int totalReadersConnected = 4;
        private string activeReaderID = "";
        private string locID = "";
        public static bool isTCPIP = false;
        private bool isAllReaderScanEnabled = false;
        private bool connectionSuccessfull = false;

        //Trial period days *change* here
        public static int trialPeriod = 14;

        /// <summary>
        /// Constructor of the <see cref="HATrakaMain" /> class.
        /// </summary>
        public HATrakaMain()
        {
            this.Cursor = Cursors.Wait;
            InitializeComponent();
            initializeNotifyIcon();
            //assetInfoDataSet.AssetInfoDataTable.Columns.Add("sr_no", typeof(int));
            //assetInfoDataSet.AssetInfoDataTable.Columns.Add("timestamp", typeof(DateTime));
            //Thread.Sleep(1000);

            productTimer.Tick += new EventHandler(productTimer_Tick);
            productTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            outputSignal.Tick += new EventHandler(outputSignal_Tick);
            outputSignal.Interval = new TimeSpan(0, 0, 0, 0, 200);

            this.Cursor = Cursors.Arrow;
        }

        /****************************************************************************/

        //Window loaded activated and notify icon events processing 
        //along with a reserved AppHome shortcut

 
        /// <summary>
        /// Handles the Loaded event of the theMainWindow control.
        /// Loads the settings of the app and initializes some variables
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void theMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
            
            // *change* here. Comment the line below to ensure normal product trial
            Properties.Settings.Default.isProductActivated = true;

            //Check for app expiry
            checkAppExpiry();

            //Get tool tip
            loadToolTipDB();
            countToolTip = new Random().Next(11);
            lblToolTip.Content = toolTipDB[countToolTip];

            // Initialize two global dictionaries
            if (Properties.Settings.Default.scannedAssetUdb == null)
            {
                Properties.Settings.Default.scannedAssetUdb = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.scannedAssetUdb.Clear();
            }
            if (Properties.Settings.Default.assetMovementDB == null)
            {
                Properties.Settings.Default.assetMovementDB = new System.Collections.Specialized.OrderedDictionary();
                Properties.Settings.Default.assetMovementDB.Clear();
            }
            if (Properties.Settings.Default.refDate.Year == 2000)
            {
                //First run date is the ref date.
                Properties.Settings.Default.refDate = DateTime.Now;

            }
            //User information already set from LoginWindow

            // *change* here
            // Super admin contacts
            superAdminEmail = "info@zigtraka.com";
            superAdminSMS = "8237871233";

            //Initialize alert notification settings to default values
            AlertNotificationSettings.assignDefaultAlertNotifyValues();

            //Initialize a log file
            //LogFile eventLogFile = new LogFile();
       
            // Image original height and width values stored in matrix
            maxRange = (int)sliderMapZoom.Maximum;
            mOriginal = imageMap.RenderTransform.Value;

            //Initialize timer for allScan
            //Timer settings
            allScanTimer.Tick += new EventHandler(allScanTimer_Tick);
            allScanTimer.Interval = new TimeSpan(0, 0, 12);

            //Load dashboard and reports
            loadDashboardReports();

            // UI features
            hideDGscannedAssetFields();
            hideBrowseDBFields();
            hideAlertDBFields();
            hideScannedAlertFields();

            // Skin select
            SkinsWindow.selectThemeforApp();
            //bool one = FocusManager.GetIsFocusScope(theMainWindow);
            //lblWelcomeUsername.Content = Properties.Settings.Default.usernameD.ToString();
            
            //Finally load up a default connection
            NewConnection.loadDefaultConnection();

            this.Cursor = Cursors.Arrow;

        }

        /// <summary>
        /// Handles the Activated event of the theMainWindow control.
        /// Sets the username of the user
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void theMainWindow_Activated(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Handles the Click event of the appHome control.
        /// Gets to the tutorial or flow doc home
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void appHome_Click(object sender, RoutedEventArgs e)
        {
            dataGridBrowseDatabase.Visibility = Visibility.Hidden;
            dgDisplayScannedAssets.Visibility = Visibility.Hidden;

            hideDGscannedAssetFields();
            hideBrowseDBFields();
            hideAlertDBFields();
            hideScannedAlertFields();

            tabItemScannedAsset.IsSelected = true;

            textBoxSearchID.Text = "";
            
            //textBoxSearchDB.Visibility = Visibility.Hidden;

            textBlockSearchResult.Visibility = Visibility.Hidden;
            cmbEventsTime.SelectedIndex = 0;
            lblTagsNote.Visibility = Visibility.Hidden;

            //var sortedOrder = od.Values.Cast<int>().OrderByDescending(i => i);  // enumerate through ordered by the value
            //var sortedDictionary = od.Cast<DictionaryEntry>().OrderByDescending(r => r.Value).ToDictionary(c => c.Key, d => d.Value);
            //string asd = sortedDictionary.Sum(x => (int)x.Value).ToString();

             // *change* here for home page
            //HomeWindow appHomewin = new HomeWindow();
            //appHomewin.Show();
            
        }

        /// <summary>
        /// Handles the StateChanged event of the theMainWindow control.
        /// Handles the notify icon event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void theMainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                windowInSysTray(true);
            }
            else
            {
                myNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }

        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the myNotifyIcon control.
        /// Call function to process
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs" /> instance containing the event data.</param>
        private void myNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            windowInSysTray(false);
        }

        /// <summary>
        /// Initializes the notify icon with settings
        /// </summary>
        private void initializeNotifyIcon()
        {
            myNotifyIcon = new System.Windows.Forms.NotifyIcon();
            //Uri iconUri = new Uri(@"./IconImages/StopDisabled.png", UriKind.Relative);      
            //var iconFile = new System.IO.MemoryStream(ZTraka_App.Properties.Resources.ZTicon);
            //myNotifyIcon.Icon = (System.Drawing.Icon) BitmapFrame.Create(iconUri);
            myNotifyIcon.Icon = ZTraka_App.Properties.Resources.ZTicon;

            myNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(myNotifyIcon_MouseDoubleClick);

        }

        /// <summary>
        /// App in the sys tray.
        /// Function to process app in system tray
        /// </summary>
        /// <param name="inTray">if set to <c>true</c> [in tray].</param>
        private void windowInSysTray(bool inTray)
        {
            if (minimizeInTray == false)
            {
                return;
            }

            if (inTray)
            {
                //Hide the window
                this.ShowInTaskbar = false;
                this.WindowState = WindowState.Minimized;

                if (myNotifyIcon != null)
                {
                    //Show the icon
                    myNotifyIcon.Visible = true;
                    //Show the balloon tip
                    myNotifyIcon.BalloonTipTitle = "Application minimized";
                    myNotifyIcon.BalloonTipText = "The HATraka app is still running! \r\n Double click the tray icon to show the app.";
                    myNotifyIcon.ShowBalloonTip(3000);
                    myNotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
                    

                }
            }
            else
            {
                //Show the window
                this.ShowInTaskbar = true;
                //Restore it
                this.WindowState = WindowState.Normal;
                //Bring it to the front
                this.Activate();
                //Remove the sys tray icon
                if (myNotifyIcon != null)
                {
                    myNotifyIcon.Visible = false;
                }
            }

        }

        /// <summary>
        /// Handles the Closed event of the theMainWindow control.
        /// Performs necessary operations when window is closed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void theMainWindow_Closed(object sender, EventArgs e)
        {
            if (isLogout == false)
            {
                //  Save the user settings for the application
                Properties.Settings.Default.Save();

                //Dispose object and shutdown

                myNotifyIcon.Dispose();

                //Check for events that are going on and close them properly
                if ((cTimer.IsEnabled == true) || (masterTimer.IsEnabled == true))
                {
                    stopReader();
                }
                //Check if reader is disconnected before shutting down
                //Always a good idea to disconnect reader before shutdown
                if (isReaderDisconnected == false)
                {
                    disconnectTheReader();
                }


                LogFile.Log("App Close Event -Done! Time: " + DateTime.Now.ToLongTimeString());
                Application.Current.Shutdown();

            }

        }

        /****************************************************************************/

        //Help menu options
        // Contents References, Register product, About, Read ME, Email Support, App Logout Exit

        /// <summary>
        /// Handles the Click event of the menuAbout control.
        /// Help->About
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            About aboutWindow = new About();
            aboutWindow.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuRegister control.
        /// Register window to register the product for the user
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuRegister_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuEmailSupport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuEmailSupport_Click(object sender, RoutedEventArgs e)
        {
            EmailSupport emailSupportOption = new EmailSupport();
            emailSupportOption.Show();

        }

        /// <summary>
        /// Handles the Click event of the menuLogoutExit control.
        /// Logout and Exit from the app
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuLogoutExit_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            //Logging the even of app close done in window closing
            Properties.Settings.Default.Save();
            
            // Dispose notify icon
            myNotifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
            
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the Click event of the menuHelpUserDocumentation control.
        /// Opens the help file .chm for the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuHelpUserDocumentation_Click(object sender, RoutedEventArgs e)
        {
            
            string directory = Environment.CurrentDirectory;
            var fileLoc = Path.Combine(directory, @"./HelpDocumentation/HATraka Help Documentation.chm");
            Process.Start(fileLoc);
            
            //var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //var directory1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //string directory = Environment.CurrentDirectory;
            //var file = Path.Combine(directory, @"\Documentation.chm");
            //System.AppDomain.CurrentDomain.BaseDirectory
            //System.IO.Directory.GetCurrentDirectory();
            
            
        }

        /// <summary>
        /// Handles the Click event of the menuReadMe control.
        /// System requirements for the app to run correctly
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuReadMe_Click(object sender, RoutedEventArgs e)
            {
                ReadMeSystemReq readmewindow = new ReadMeSystemReq();
                readmewindow.Show();
                
            }

        /// <summary>
        /// Handles the Click event of the menuCheckUpdates control.
        /// Open web browser and directs to zigtraka pages.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
       
            //Use a product updates webpage..
            // Opens default webbrowser
            System.Diagnostics.Process.Start("http://www.zigtraka.com/hospital-equipment-tracking.html");
        }

        /****************************************************************************/


        // Edit menu functions
        // Special Cut,xCopy, Paste, xSelect All, Invert Select for grid

        /// <summary>
        /// Handles the Click event of the editMenuCut control.
        /// Cuts the selected text from the control (textbox) to clipboard
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void editMenuCut_Click(object sender, RoutedEventArgs e)
        {
            // Get the control which is focussed
            getFocussedControl = FocusManager.GetFocusedElement(this);
            if (getFocussedControl != null)
            {

                if (getFocussedControl.GetType().Name.CompareTo("TextBox") == 0)
                {

                    System.Windows.Clipboard.SetData("Text", ((System.Windows.Controls.TextBox)(getFocussedControl)).SelectedText.ToString());
                    ((System.Windows.Controls.TextBox)(getFocussedControl)).SelectedText = "";

                }
            }          
        }

        /// <summary>
        /// Handles the Click event of the editMenuCopy control.
        /// Copies the data or content from the control (textbox or datagrid) to clipboard
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void editMenuCopy_Click(object sender, RoutedEventArgs e)
        {
            //Window aWin = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.ShowInTaskbar);

            // Get the focussed control
            getFocussedControl = FocusManager.GetFocusedElement(this);
            if (getFocussedControl != null)
            {
                // Check if the control is a textbox
                if (getFocussedControl.GetType().Name.CompareTo("TextBox") == 0)
                {
                    if (((System.Windows.Controls.TextBox)(getFocussedControl)).SelectedText.CompareTo("") == 0) // if no selection made copy entire text
                    {
                        System.Windows.Clipboard.SetData("Text", ((System.Windows.Controls.TextBox)(getFocussedControl)).Text.ToString());
                    }
                    else // if text is selected to copy then copy only the selected text
                    {
                        System.Windows.Clipboard.SetData("Text", ((System.Windows.Controls.TextBox)(getFocussedControl)).SelectedText.ToString());
                    }
                }

                    //Check is the control is a datagrid
                else if ((getFocussedControl.GetType().Name.CompareTo("DataGrid") == 0) || (getFocussedControl.GetType().Name.CompareTo("DataGridCell") == 0))
                {
               
                    //System.Windows.Forms.SendKeys.SendWait("^(C)");
                    string stringCopied = "";
                    if ((getFocussedControl.GetType().Name.CompareTo("DataGrid") == 0))
                    {
                        DataGrid getFocusDG = ((Microsoft.Windows.Controls.DataGrid)(getFocussedControl));

                        if (getFocusDG.SelectedItems.Count > 0)
                        {
                            string cName, cVal;
                            
                            int selCount = getFocusDG.SelectedItems.Count;

                            for (int i = 0; i < selCount; i++)
                            {

                                //IList rowsM  = getFocusDG.SelectedItems;
                                DataRowView rowSelectedVRecord = (DataRowView)getFocusDG.SelectedItems[i];
                                int colCount = rowSelectedVRecord.Row.Table.Columns.Count;

                                stringCopied += "\r\n Selected Row: ";
                                for (int j = 0; j < colCount; j++)
                                {
                                    cName = rowSelectedVRecord.Row.Table.Columns[j].ToString();
                                    cVal = rowSelectedVRecord.Row.ItemArray[j].ToString().TrimEnd();
                                    stringCopied += cName + ":=" + cVal + "   ";

                                }

                                //var objItems = (new System.Collections.ArrayList(((rowSelectedVRecord.Row.Table).Columns._list)));
                                //var itemObj = ((Microsoft.Windows.Controls.DataGrid)(getFocussedControl)).SelectedCells.ToList();
                                //var itemObj = ((System.Data.DataRowView)(((Microsoft.Windows.Controls.DataGrid)(getFocussedControl)).SelectedItem)).Row.ItemArray;

                                //foreach (var itemVar in itemObj)
                                //{
                                //    stringCopied += itemVar. + ":= " + itemVar + "  ";
                                //}
                            }

                        }

                        System.Windows.Clipboard.SetData("Text", stringCopied);
                    }

                    else if ((getFocussedControl.GetType().Name.CompareTo("DataGridCell") == 0))
                    {
                        string lVal, lName;
                        string stringCopiedDG = "";
                        DataGridCell dgCell = ((Microsoft.Windows.Controls.DataGridCell)(getFocussedControl));
                        lName = ((Microsoft.Windows.Controls.DataGridTextColumn)dgCell.Column).Header.ToString();
                        lVal = ((System.Windows.Controls.TextBlock)(((System.Windows.Controls.ContentControl)(dgCell)).Content)).Text.TrimEnd();
                        stringCopiedDG = lVal;

                        System.Windows.Clipboard.SetData("Text", stringCopiedDG);
                    }

                }

            }
        }

        /// <summary>
        /// Handles the Click event of the editMenuPaste control.
        /// Get the data from the clipboard and pastes it into the control (textbox only)
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void editMenuPaste_Click(object sender, RoutedEventArgs e)
        {
            int caretIndexValue;
            string clipboardDataString = "";

            getFocussedControl = FocusManager.GetFocusedElement(this);
            if (getFocussedControl != null)
            {

                if (getFocussedControl.GetType().Name.CompareTo("TextBox") == 0)
                {
                    caretIndexValue = ((System.Windows.Controls.TextBox)(getFocussedControl)).CaretIndex;
                    clipboardDataString = (string)System.Windows.Clipboard.GetData("Text");
                    ((System.Windows.Controls.TextBox)(getFocussedControl)).Text = ((System.Windows.Controls.TextBox)(getFocussedControl)).Text.Insert(caretIndexValue, clipboardDataString);
                    ((System.Windows.Controls.TextBox)(getFocussedControl)).CaretIndex = caretIndexValue + clipboardDataString.Length;


                }
            }
            
        }

        /// <summary>
        /// Handles the Click event of the editMenuSelectAll control.
        /// Selects all the elemets or data from the control (textbox or datagrid)
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void editMenuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            // Call the selectall function to do the job
            if (selectAllFunction())
            {
                //System.Windows.Forms.SendKeys.SendWait("^A"); //Does not work
                //Thread.Sleep(5000);
                //selectAllFunction();

            }
            else // If the select all function does not work for datagrid sort the columns,
                //so that the focus is now on datagrid instead of datagrid Cell.
            {
                //do nothing
            }
            
        }

        /// <summary>
        /// Select all function.
        /// Selects all the data content from the control (datagrid or textbox)
        /// Note: Sort any column to get datagrid control instead of datagrid cell control.
        /// </summary>
        /// <returns></returns>
        private bool selectAllFunction()
        {
            //Get the control
            getFocussedControl = FocusManager.GetFocusedElement(this);

            if (getFocussedControl != null)
            {
                //Check for textbox
                if (getFocussedControl.GetType().Name.CompareTo("TextBox") == 0)
                {
                    ((System.Windows.Controls.TextBox)(getFocussedControl)).SelectAll();
                    return false;
                }
                else if ((getFocussedControl.GetType().Name.CompareTo("DataGrid") == 0) || (getFocussedControl.GetType().Name.CompareTo("DataGridCell") == 0))
                {

                    if ((getFocussedControl.GetType().Name.CompareTo("DataGrid") == 0))
                    {
                        DataGrid getFocusDG = ((Microsoft.Windows.Controls.DataGrid)(getFocussedControl));
                        getFocusDG.SelectAll();
                        return false;

                    }

                        //Check if the control is datagrid cell
                    else if ((getFocussedControl.GetType().Name.CompareTo("DataGridCell") == 0))
                    {
                        //TraversalRequest tvr = new TraversalRequest(FocusNavigationDirection.Next);

                        DataGridCell dgCell = ((Microsoft.Windows.Controls.DataGridCell)(getFocussedControl));
                        //dgCell.IsTabStop = false;
                        //dgCell.Focusable = false;
                        //dgCell.MoveFocus(tvr);
                        //dgCell.Focusable = true;
                        return true;
                        //selectAllFunction();
                    }
                }


                return false;
            }
            return false;
        }

        /// <summary>
        /// Handles the Click event of the editMenuInvertSelect control.
        /// Inverts the selection of the datagrid or textbox control.
        /// Found usefull in many cases
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void editMenuInvertSelect_Click(object sender, RoutedEventArgs e)
        {
            //Get the control
            getFocussedControl = FocusManager.GetFocusedElement(this);
            if (getFocussedControl != null)
            {
                //Check for textbox control
                if (getFocussedControl.GetType().Name.CompareTo("TextBox") == 0)
                {
                    TextBox tbxInvert = (TextBox)((System.Windows.Controls.TextBox)(getFocussedControl));
                    int sIndex = tbxInvert.SelectionStart;
                    int sLength = tbxInvert.SelectionLength;
                    int tLength = tbxInvert.Text.Length;

                    if ((sIndex + sLength) == tLength)
                    {
                        tbxInvert.Select(0, sIndex);
                    }
                    else
                    {
                        tbxInvert.Select(sIndex + sLength, tLength - 1);
                    }
                }

                //Check for datagrid or datagrid cell control.
                else if ((getFocussedControl.GetType().Name.CompareTo("DataGrid") == 0) || (getFocussedControl.GetType().Name.CompareTo("DataGridCell") == 0))
                {

                    //System.Windows.Forms.SendKeys.SendWait("^(C)");
                    //string stringCopied = "";
                    if ((getFocussedControl.GetType().Name.CompareTo("DataGrid") == 0))
                    {
                        DataGrid dataGrid1 = ((Microsoft.Windows.Controls.DataGrid)(getFocussedControl));
                        for (int i = 0; i < dataGrid1.Items.Count; i++)
                        {
                            DataGridRow row = (DataGridRow)dataGrid1.ItemContainerGenerator.ContainerFromIndex(i);

                            if (row.IsSelected == true)
                            {
                                row.IsSelected = false;
                            }
                            else
                            {
                                row.IsSelected = true;
                            }
                        }


                    }
                }


            }
        }

        /****************************************************************************/

        // File Menu Options
        // File menu-> import and export app settings, logout, exit from app

        /// <summary>
        /// Handles the Click event of the fileMenuLogout control.
        /// Logout fromthe app and display the login window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void fileMenuLogout_Click(object sender, RoutedEventArgs e)
        {
            isLogout = true;
            LogFile.Log("User has logged out of the App !");
            myNotifyIcon.Dispose();
            this.Close();
            isLogout = false;
            LoginWindow loginWin = new LoginWindow();
            loginWin.ShowDialog();

        }

        /// <summary>
        /// Handles the Click event of the fileMenuNewConnection control.
        /// New connection, connection properties, add edit update and delete
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void fileMenuNewConnection_Click(object sender, RoutedEventArgs e)
        {
            NewConnection readerConnection = new NewConnection();
            readerConnection.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the fileMenuImportSetings control.
        /// Load the settings file from local disk
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void fileMenuImportSetings_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog OpenFileDialog1 = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            OpenFileDialog1.DefaultExt = ".zts";
            OpenFileDialog1.Filter = "ZT Settings File(*.zts)|*.zts";

            OpenFileDialog1.Title = "Import Settings file...";
            //OpenFileDialog1.
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = OpenFileDialog1.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                if (OpenFileDialog1.FileName != "")
                {
                    
                    this.Cursor = Cursors.Wait;
                    string fileText = File.ReadAllText(OpenFileDialog1.FileName);
                    readAppSettingValues.readFromFile(fileText);
                    SkinsWindow.selectThemeforApp();
                    // Display message box
                    MessageBox.Show("App settings loaded successfully for user:" + LoginWindow.user_name + " !" ,"Settings loaded from file", MessageBoxButton.OK, MessageBoxImage.Information);

                    //Check for valid email and mobile if check boxes are checked for email and SMS
                    if (AlertNotificationSettings.sendEmailUser == true)
                    {
                        if ((!(sendEmail.checkEmail(LoginWindow.user_email))) || (string.IsNullOrEmpty(LoginWindow.user_email)) )
                        {
                            // Display message box
                            MessageBox.Show("Please check for valid email ID in user profile !", "Invalid or missing email ID!", MessageBoxButton.OK, MessageBoxImage.Stop);
                            AlertNotificationSettings.sendEmailUser = false;
                            AlertNotificationSettings.sendEmailAdmin = false;
                        }
                    }

                    if (AlertNotificationSettings.sendSMSUser == true)
                    {
                        if ((!(SMSClass.checkMobileNumber(LoginWindow.user_contact))) || (string.IsNullOrEmpty(LoginWindow.user_contact)))
                        {
                            // Display message box
                            MessageBox.Show("Please check for valid mobile number in user profile !", "Invalid or missing mobile number!", MessageBoxButton.OK, MessageBoxImage.Stop);
                            AlertNotificationSettings.sendSMSUser = false;
                            AlertNotificationSettings.sendSMSAdmin = false;
                        }
                    }



                    this.Cursor = Cursors.Arrow;
                }
                else
                {
                    // Display message box
                    MessageBox.Show("Unable to find file.Please try again !", "File not loaded correctly !", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }


        }

        /// <summary>
        /// Handles the Click event of the fileMenuSaveSettings control.
        /// Export the settings file and save it on local disk
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void fileMenuSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog1.Filter = "ZT Settings File(*.zts)|*.zts";
            saveFileDialog1.Title = "Save a zts file";
            saveFileDialog1.FileName = "myZTAppSettings";
            saveFileDialog1.ValidateNames = true;
            saveFileDialog1.OverwritePrompt = true;

            Nullable<bool> result = saveFileDialog1.ShowDialog();
            //DialogResult.Value = true;

            if (result == true)
            {
                if (saveFileDialog1.FileName != "")
                {
                    this.Cursor = Cursors.Wait;
                    string fileString = writeAppSettingValues.writeToFile();
                    File.WriteAllText(saveFileDialog1.FileName, fileString);

                    // Display message box
                    MessageBox.Show("App settings saved successfully for user:" + LoginWindow.user_name + " !", "Settings saved to file", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Cursor = Cursors.Arrow;
                }
                else
                {
                    // Configure the message box to be displayed 
                    string messageBoxText = "File name is either invalid or blank !";
                    string caption = "Invalid File Name !";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Stop;
                    // Display message box
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }

            }
            else
            {
                //Invalid file name or Cancel operation
            }
        }

        /****************************************************************************/

        // Reader Operation and functions
        // Scanning, resetting, stop, ping etc

        /// <summary>
        /// Handles the Click event of the MenuItemConnectReader control.
        /// Connection to the reader.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuItemConnectReader_Click(object sender, RoutedEventArgs e)
        {
            checkReaderConnection();     
        }

        /// <summary>
        /// Checks the reader connection.
        /// Gets the default connection and connects to the reader.
        /// Checks whether the connection was successful.
        /// </summary>
        private void checkReaderConnection()
        {
            // Intialize local variables
            int connectionIndex;
            int indexC;
            bool defaultConnFound = false;
            string connStringDisplay = null;
            string[] connStringA = new string[4];


            //NewConnection.connStringDB.Values.Where(val => val.Contains("[default]")).Select(val => val.ToString());
            // Get the array string containing the word default.

            for (indexC = 0; indexC < NewConnection.numberOfConnections; indexC++)
            {
                //Check for null or empty
                if (!(string.IsNullOrEmpty(NewConnection.myConn[indexC])))
                {

                    connectionIndex = NewConnection.myConn[indexC].IndexOf("[default]");
                    if (connectionIndex != -1)
                    {
                        defaultConnFound = true;
                        break;
                    }
                }
            }

            if (defaultConnFound) // If a default connection is found process further
            {
                connStringDisplay = NewConnection.myConn[indexC];
                connStringA = connStringDisplay.Split('#');
                //connection type: connStringA[1]
                // connection type properties: connStringA[2]
                if (isAllReaderScanEnabled == true)
                {
                    connStringA[2] = NetworkConnections.getReaderIP(readerNumber);
                }

                //Reset connectionsuccessfull bool value
                connectionSuccessfull = false;
                //Call the main function connectTheReader and pass the required parameters
                connectionSuccessfull = connectTheReader(connStringA[1], connStringA[2]);

                if (connectionSuccessfull)
                {
                    statusBarReaderStatus.Content = "Reader Connected";
                    imageReaderConnected.Visibility = Visibility.Visible;
                    imageReaderDisconnected.Visibility = Visibility.Hidden;
                    imageReaderReset.Visibility = Visibility.Hidden;

                    ////////////////////////////////////////////////
                    menuDisconnectReader.IsEnabled = true;
                    imgDeleteDisconnect.Source = new BitmapImage(new Uri(@"./IconImages/deletedisconnect.png", UriKind.Relative));
                    fileMenuDisconnectReader.IsEnabled = true;
                    imgMenuDeleteDisconnect.Source = new BitmapImage(new Uri(@"./IconImages/deletedisconnect.png", UriKind.Relative));
                    /////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////
                    menuResetReader.IsEnabled = true;
                    imgReset.Source = new BitmapImage(new Uri(@"./IconImages/reset.png", UriKind.Relative));
                    fileMenuResetReader.IsEnabled = true;
                    imgMenuResetReader.Source = new BitmapImage(new Uri(@"./IconImages/reset.png", UriKind.Relative));
                    /////////////////////////////////////////////////////////////////


                    ////////////////////////////////////////////////
                    menuConnectReader.IsEnabled = false;
                    imgConnectReader.Source = new BitmapImage(new Uri(@"./IconImages/connectReaderDisabled.png", UriKind.Relative));
                    fileMenuConnectReader.IsEnabled = false;
                    imgMenuConnectReader.Source = new BitmapImage(new Uri(@"./IconImages/connectReaderDisabled.png", UriKind.Relative));
                    /////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////
                    menuScanSingle.IsEnabled = true;
                    imgScanOnce.Source = new BitmapImage(new Uri(@"./IconImages/RunSingle.png", UriKind.Relative));
                    fileMenuRunSingle.IsEnabled = true;
                    imgMenuScanOnce.Source = new BitmapImage(new Uri(@"./IconImages/RunSingle.png", UriKind.Relative));
                    /////////////////////////////////////////////////////////////////

                    ////////////////////////////////////////////////
                    menuRunMultipleCont.IsEnabled = true;
                    imgRunContinously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultiple.png", UriKind.Relative));
                    fileMenuRunMultipleCont.IsEnabled = true;
                    imgMenuRunContinuously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultiple.png", UriKind.Relative));
                    /////////////////////////////////////////////////////////////////

                    

                    LogFile.Log("Success: Reader connected !");
                    //MessageBox.Show("Reader is now successfully connected!", "Connection Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    connectionSuccessfull = false;
                    LogFile.Log("Error: There seems to be a problem connecting with the reader!");
                    MessageBox.Show("There seems to be a problem connecting with the reader! Check your connection properties and make sure reader is connected to the PC. Try again", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
            }
            else
            {
                connectionSuccessfull = false;
                MessageBox.Show("Please create a new connection or set an existing connection to default!", "No default connection found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// Connects the reader.
        /// Checks the connection parameters and connects the reader according to the API function
        /// </summary>
        /// <param name="connType">Type of the conn.</param>
        /// <param name="connProperty">The conn property.</param>
        /// <returns></returns>
        private bool connectTheReader(string connType, string connProperty)
        {
            byte linktype = 1;      //1 represents the serial port, 2 on behalf of the network interface(TCP/IP), 3 on behalf of the USB interface.
            string linkProperty = null;
            bool returnResult = false;

            // Generate a new dictionary DB for storing baud rate values
            Dictionary<string, int> baudRateDB = new Dictionary<string, int>()
            {
                {"2400",0},
                {"4800",1},
                {"9600 (default)",2},
                {"19200",3},
                {"38400",4},
                {"57600",5},
                {"115200",6},
                {"230400",7}
            };

            if (connType.CompareTo("Ethernet (TCP/IP)") == 0)       //TCP/IP
            {
                linktype = 2;
                linkProperty = connProperty; //IP address set in the default connection.
                
                // *change* here
                if (isAllReaderScanEnabled == true)
                {
                    linkProperty = NetworkConnections.getReaderIP(readerNumber); //IP address obtained from network connection
                }
                readerIP = linkProperty;
                textBlockReaderInfo.Visibility = Visibility.Visible;
                //Call ping and then connect
                pingTheReader();
            }
            
            else if(connType.Substring(0, 3).CompareTo("COM") == 0)
            {
                linktype = 1;
                linkProperty = connType; // set the COM port Eg: COM7
                textBlockReaderInfo.Visibility = Visibility.Hidden;
            }

            else if (connType.CompareTo("USB") == 0)   //USB
            {
                linktype = 3; 
                textBlockReaderInfo.Visibility = Visibility.Hidden;
                // No linkProperty needed for USB connection. Assumed to be USB 2.0 High Speed
            }

            else
            {
                returnResult = false;
                //There is problem: Program should never be here..
            }

            this.Cursor = Cursors.Wait; 

                ///Call the all important function OpenReader from DLL
                short getResult = readerDLL.OpenReader(ref readerDLL.hComm, linktype, linkProperty);
                if (getResult == 0)
                {
                    returnResult = true;
                    // Success connecting to the reader...
                    isReaderDisconnected = false;
                    if (connType.Substring(0, 3).CompareTo("COM") == 0)  //If com port set the baud rate
                    {
                        isTCPIP = false;
                        activeReaderID = "";
                        int BRindex = baudRateDB[connProperty];
                        getResult = readerDLL.SetBaudRate(readerDLL.hComm, BRindex);     //Set the baud rate for serial communication
                        if (getResult == 0)
                        {
                            returnResult = true; //Success for baud rate setting
                        }
                        else
                        {
                            //Failure

                            returnResult = false;

                        }
                    }
                    else if (connType.CompareTo("Ethernet (TCP/IP)") == 0)
                    {
                        isTCPIP = true;
                        activeReaderID = NetworkConnections.getActiveReaderID(readerIP); //Eg: RF178 from IP address 192.168.0.178
                        locID = NetworkConnections.getLocationID(activeReaderID);
                        textBlockReaderInfo.Text = "Reader ID:" + activeReaderID + " connected with IP:" + readerIP.ToString();
                    }

                    //beep control
                    //short getResult2 = readerDLL.BeepControl(readerDLL.hComm, ReaderSettings.ControlType);
                    //if (getResult2 == 0)
                    //{
                    //    // Beep feature setting
                    //    //ControlType— controlling type of buzzer, defined as: 1 start buzzer; 2, stop the buzzer; 3. start the buzzer and automatically stop the buzzer
                    //}
                    //else
                    //{
                    //    //issue
                    //}

                    // Optional check for getting firmaware version...
                    if (!Getversions())
                    {
                        // Communication failure
                        LogFile.Log("Error: Failed to connect to the reader and get firmware version");
                        returnResult = false;

                    }

                
            }

            else
            {

            }
            this.Cursor = Cursors.Arrow;
            return returnResult;


        }

        /// <summary>
        /// Get version of the reader firmware.
        /// </summary>
        /// <returns></returns>
        private bool Getversions()
        {
            byte flag = 0, major = 0, minor = 0;
            bool retResult = false;
            short getVerResult = readerDLL.GetFirmwareVersion(readerDLL.hComm, ref flag, ref major, ref minor);
            if (getVerResult == 0)
            {
                retResult = true;
            }
            return retResult;
        }

        /// <summary>
        /// Loads the tool tip DB.
        /// </summary>
        private void loadToolTipDB()
        {
            toolTipDB.Add(0, "It is always advisable to check your reader connection by pinging the reader before connecting or scanning for the reading of tags...");
            toolTipDB.Add(1, "Make sure one of the connections stored is a default connection. The reader always uses default connection settings for connecting and pinging purposes.");
            toolTipDB.Add(2, "While logging in, only one user can make his/her password remember to the application !");
            toolTipDB.Add(3, "Note that only the user with ADMIN role has all the rights for deleting and modifying records and information from the database");
            toolTipDB.Add(4, "Check the reader settings before processing the tag scan operation");
            toolTipDB.Add(5, "It is always a good idea to disconnect the reader after completing the operations.");
            toolTipDB.Add(6, "Check and test your internet connection as some of the features like email and SMS will not work !");
            toolTipDB.Add(7, "Please see the settings for alert notifications");
            toolTipDB.Add(8, "For using edit operations with datagrid, it is better to sort one of the colums to enable datagrid focus");
            toolTipDB.Add(9, "It is always advisable to check database connection before searching for records in the database through quick browse tab");
            toolTipDB.Add(10, "Check out the dashboard and Reports for alerts and analysis");

        }

        /// <summary>
        /// Handles the Click event of the menuToolTip control.
        /// Tool tip provides useful information to the user
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuToolTip_Click(object sender, RoutedEventArgs e)
        {
            //Increment the count
            countToolTip++;

            //Display the tool tip
            lblToolTip.Content = toolTipDB[countToolTip];

            //Check end of tool tip count
            if (countToolTip > 10)
            {
                //Restart
                countToolTip = 0;
            }

        }

        /// <summary>
        /// Handles the Click event of the menuDisconnectReader control.
        /// Disconnects the reader from the app
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuDisconnectReader_Click(object sender, RoutedEventArgs e)
        {
            disconnectTheReader();
        }

        /// <summary>
        /// Disconnects the reader.
        /// </summary>
        private void disconnectTheReader()
        {
            textBlockReaderInfo.Visibility = Visibility.Hidden;

            if ((cTimer.IsEnabled == true) || (masterTimer.IsEnabled == true))
            {
                stopReader();
            }

            short getResult = readerDLL.CloseReader(readerDLL.hComm);
            if (getResult == 0)
            {
                isReaderDisconnected = true;
                //btnStart.IsEnabled = true;
                //MessageBox.Show("Reader disconnected!", "Disconnect Message", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                
                statusBarReaderStatus.Content = "Reader Disconnected";
                imageReaderDisconnected.Visibility = Visibility.Visible;
                imageReaderConnected.Visibility = Visibility.Hidden;

                imageReaderReset.Visibility = Visibility.Hidden;

                ////////////////////////////////////////////////
                menuDisconnectReader.IsEnabled = false;
                imgDeleteDisconnect.Source = new BitmapImage(new Uri(@"./IconImages/deletedisconnectDisabled.png", UriKind.Relative));
                fileMenuDisconnectReader.IsEnabled = false;
                imgMenuDeleteDisconnect.Source = new BitmapImage(new Uri(@"./IconImages/deletedisconnectDisabled.png", UriKind.Relative));
                /////////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////
                menuConnectReader.IsEnabled = true;
                imgConnectReader.Source = new BitmapImage(new Uri(@"./IconImages/connectReader.png", UriKind.Relative));
                fileMenuConnectReader.IsEnabled = true;
                imgMenuConnectReader.Source = new BitmapImage(new Uri(@"./IconImages/connectReader.png", UriKind.Relative));
                /////////////////////////////////////////////////////////////////


                ////////////////////////////////////////////////
                menuResetReader.IsEnabled = false;
                imgReset.Source = new BitmapImage(new Uri(@"./IconImages/resetDisabled.png", UriKind.Relative));
                fileMenuResetReader.IsEnabled = false;
                imgMenuResetReader.Source = new BitmapImage(new Uri(@"./IconImages/resetDisabled.png", UriKind.Relative));
                /////////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////
                menuScanSingle.IsEnabled = false;
                imgScanOnce.Source = new BitmapImage(new Uri(@"./IconImages/RunSingleDisabled.png", UriKind.Relative));
                fileMenuRunSingle.IsEnabled = false;
                imgMenuScanOnce.Source = new BitmapImage(new Uri(@"./IconImages/RunSingleDisabled.png", UriKind.Relative));
                /////////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////
                menuRunMultipleCont.IsEnabled = false;
                imgRunContinously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultipleDisabled.png", UriKind.Relative));
                fileMenuRunMultipleCont.IsEnabled = false;
                imgMenuRunContinuously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultipleDisabled.png", UriKind.Relative));
                /////////////////////////////////////////////////////////////////

                ////////////////////////////////////////////////
                //fileMenuRunAllMultiple.IsEnabled = false;
                //imgMenuRunAllMultiple.Source = new BitmapImage(new Uri(@"./IconImages/RunAllMultipleDisabled1.png", UriKind.Relative));
                /////////////////////////////////////////////////////////////////

            }
            else
            {
                MessageBox.Show("Reader could not be disconnected! Please try again", "Disconnect Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the menuPingReader control.
        /// calls the Ping function
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuPingReader_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;

            pingTheReader();

            this.Cursor = Cursors.Arrow;

        }

        /// <summary>
        /// Pings the reader.
        /// </summary>
        private void pingTheReader()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            byte[] buffer = new byte[32];
            string pingurl = "";
            int timeout = 120;
            int pingAttmepts = 10; //Practical value seems 7

            int connectionIndex;
            int indexC;
            bool defaultConnFound = false;
         
            string connStringDisplay = null;
            string[] connStringA = new string[4];

            
            for (indexC = 0; indexC < NewConnection.numberOfConnections; indexC++)
            {
                if (!(String.IsNullOrEmpty(NewConnection.myConn[indexC])))
                {

                    connectionIndex = NewConnection.myConn[indexC].IndexOf("[default]");
                    if (connectionIndex != -1)
                    {
                        defaultConnFound = true;
                        break;
                    }
                }
            }

            int i = 1;
            if (defaultConnFound)
            {
                connStringDisplay = NewConnection.myConn[indexC];
                connStringA = connStringDisplay.Split('#');

                if (isAllReaderScanEnabled == true)
                {
                    connStringA[2] = NetworkConnections.getReaderIP(readerNumber);
                }
                //connection type: connStringA[1]
                // connection type properties: connStringA[2]
                if (NewConnection.IsIPv4(connStringA[2]))
                {
                    pingurl = connStringA[2]; // ping IP of the reader as entered by the client in the default connection.

                    for (i = 1; i <= pingAttmepts; i++)
                    {
                        try
                        {

                            PingReply reply = pingSender.Send(pingurl, timeout, buffer, options);
                            if (reply.Status == IPStatus.Success)
                            {
                                pingResult = true;
                                break;
                            }
                            else
                            {    
                                pingResult = false;

                            }
                        }
                        catch
                        {
                                pingResult = false;
                                // ping checking issue..pinging itself has failed

                        }
                    }

                    if (pingResult) //This checks the last ping result
                    {
                        textBlockReaderInfo.Visibility = Visibility.Visible;
                        textBlockReaderInfo.Text = "Pinging success!";
                        //Ping success
                        //MessageBox.Show("Pinging reader success!", "Ping Success", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    }
                    else
                    {
                        textBlockReaderInfo.Visibility = Visibility.Visible;
                        textBlockReaderInfo.Text = "Pinging the reader failed!";
                        LogFile.Log("Error: Ping failed.Check the TCP/IP connection with reader");
                        //MessageBox.Show("Pinging to the reader failed! Reader may not be connected.", "Ping Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else
                {
                    textBlockReaderInfo.Visibility = Visibility.Hidden;
                    //Choose a tcp/IP connection
                    MessageBox.Show("Choose a default TCP/IP connection first.", "Ping Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            else
            {
                textBlockReaderInfo.Visibility = Visibility.Hidden;
                MessageBox.Show("Your default connection does not seem to be an Ethernet TCP/IP connection!. Only use the ping feature if you connect your reader through ethernet TCP/IP connection.", "Ping Error Message", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }

        /// <summary>
        /// Handles the Click event of the menuResetReader control.
        /// Reset the reader
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuResetReader_Click(object sender, RoutedEventArgs e)
        {
            if ((cTimer.IsEnabled == true) || (masterTimer.IsEnabled == true))
            {
                stopReader();
            }

            short getResult = readerDLL.ResetReader(readerDLL.hComm);
            if (getResult == 0)
            {
                MessageBox.Show("Reader reset!", "Reset Message", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                statusBarReaderStatus.Content = "Reader Reset";
                imageReaderConnected.Visibility = Visibility.Hidden;
                imageReaderDisconnected.Visibility = Visibility.Hidden;
                imageReaderReset.Visibility = Visibility.Visible;

                ////////////////////////////////////////////////
                menuResetReader.IsEnabled = false;
                imgReset.Source = new BitmapImage(new Uri(@"./IconImages/resetDisabled.png", UriKind.Relative));
                fileMenuResetReader.IsEnabled = false;
                imgMenuResetReader.Source = new BitmapImage(new Uri(@"./IconImages/resetDisabled.png", UriKind.Relative));
                /////////////////////////////////////////////////////////////////

                disconnectTheReader();
            }
            else
            {
                LogFile.Log("Error: Reader could not be reset");
                MessageBox.Show("Reader could not be reset! Please try again", "Reset Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        /// <summary>
        /// Handles the Click event of the menuScanSingle control.
        /// Scan ONCE according to the reader mode (whether single or multiple) 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuScanSingle_Click(object sender, RoutedEventArgs e)
        {
            string getTagId = "";
            
            srCount = 1;
            // Clears all the data in the table..start fresh
            assetInfoDataSet.AssetInfoDataTable.Clear();


            this.Cursor = Cursors.Wait;

           //Scan once depending upon single or multiple mode
            if (ReaderSettings.readerMode == 1) // 1 = single tag read
            {
                getTagId = getSingleTagID();
                if (!(string.IsNullOrEmpty(getTagId)))
                {
                    //Update asset location before populating datagrid
                    if (isTCPIP == true)
                    {                      
                        NetworkConnections.updateAssetLocation(getTagId,locID);
                    }
                    populateDataGridAssetInfo(getTagId);
                    //Properties.Settings.Default.scannedAssetsNum += 1;
                    if(!(Properties.Settings.Default.scannedAssetUdb.Contains(getTagId)))
                    {
                        Properties.Settings.Default.scannedAssetUdb.Add(getTagId);
                    }

                    // *change* here for specific tags or remove this feature
                    if (getTagId.CompareTo("") != 0)
                    {
                        //HomeWindow.sendOutputSignalBeep();
                        //Start the outputSignal timer
                        //outputSignal.Start();
                    }
                    // Now log the asset
                    if (AlertNotificationSettings.logsScannedAssets == true)
                    {
                        LogFile.Log("1 Asset Scanned:- Tag ID- " + getTagId.ToString());
                    }
                }
            }

            else //Else readermode = 2 i.e. multiple tag reads
            {
                masterTagIDdB.Clear();
                scanMultipleTags();
 
            }

            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the Click event of the menuRunMultipleCont control.
        /// Scan CONTINUOUSLY according to the reader mode (whether single or multiple)
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuRunMultipleCont_Click(object sender, RoutedEventArgs e)
        {
            
            this.Cursor = Cursors.Wait;

            string getTagInfo = "";
            srCount = 1;
            // Clears all the data in the table..start fresh
            assetInfoDataSet.AssetInfoDataTable.Clear();

            ///Now set the timer for continuous mode of operation
            ///ReaderSettings.tIntervalSec = 20; //20 sec timer
            //Timer settings
            masterTimer.Tick += new EventHandler(masterTimer_Tick);
            masterTimer.Interval = new TimeSpan(0, 0, ReaderSettings.tIntervalSec);

            //Timer settings
            cTimer.Tick += new EventHandler(cTimer_Tick);
            cTimer.Interval = new TimeSpan(0, 0, ReaderSettings.tIntervalSec);

            ////////////////////////////////////////////////
            menuRunMultipleCont.IsEnabled = false;
            imgRunContinously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultipleDisabled.png", UriKind.Relative));
            fileMenuRunMultipleCont.IsEnabled = false;
            imgMenuRunContinuously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultipleDisabled.png", UriKind.Relative));
            /////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////
            menuScanSingle.IsEnabled = false;
            imgScanOnce.Source = new BitmapImage(new Uri(@"./IconImages/RunSingleDisabled.png", UriKind.Relative));
            fileMenuRunSingle.IsEnabled = false;
            imgMenuScanOnce.Source = new BitmapImage(new Uri(@"./IconImages/RunSingleDisabled.png", UriKind.Relative));
            /////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////
            menuStopReader.IsEnabled = true;
            imageStop.Source = new BitmapImage(new Uri(@"./IconImages/Stop.png", UriKind.Relative));
            fileMenuStopReader.IsEnabled = true;
            fileMenuImageStop.Source = new BitmapImage(new Uri(@"./IconImages/Stop.png", UriKind.Relative));

            /////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////
            fileMenuRunAllMultiple.IsEnabled = false;
            imgMenuRunAllMultiple.Source = new BitmapImage(new Uri(@"./IconImages/RunAllMultipleDisabled1.png", UriKind.Relative));
            ///////////////////////////////////////////////////////////////


            //Scan continuously depending upon single or multiple mode
            if (ReaderSettings.readerMode == 1) // 1 = single tag read and continuously
            {

                getTagInfo = getSingleTagID();
                cTimer.Start();
                
                //Check for tag ID is not null
                if (!(string.IsNullOrEmpty(getTagInfo)))
                {
                    
                   //Also update the asset location
                    if (isTCPIP == true)
                    {
                        NetworkConnections.updateAssetLocation(getTagInfo, locID);
                    }
  
                    // Now log the asset
                    if (AlertNotificationSettings.logsScannedAssets == true)
                    {
                        LogFile.Log("1 Asset Scanned:- Tag ID- " + getTagInfo.ToString());
                    }
                    //Properties.Settings.Default.scannedAssetsNum += 1;
                    populateDataGridAssetInfo(getTagInfo);

                    //Now store in scanned assets DB
                    if (!(Properties.Settings.Default.scannedAssetUdb.Contains(getTagInfo)))
                    {
                        Properties.Settings.Default.scannedAssetUdb.Add(getTagInfo);
                    }

                }
            }

            else //Else readermode = 2 i.e. multiple tag read and continuously
            {

                masterTagIDdB.Clear();
                scanMultipleTags();
                masterTimer.Start();
            }

            this.Cursor = Cursors.Arrow;

        }

        /// <summary>
        /// Gets the length of the tag.
        /// </summary>
        /// <param name="TagType">Type of the tag.</param>
        /// <returns></returns>
        private int GetTagLength(int TagType)
        {
            // According to API documentation
            int Length = 0;
            if (TagType == 8)
            {
                Length = 8;
            }
            else if (TagType == 9)
            {
                Length = 4;
            }
            else if (TagType == 10)
            {
                Length = 6;
            }
            return Length;
        }

        /// <summary>
        /// Handles the Click event of the menuStopReader control.
        /// Stop the reader from scanning the tags
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuStopReader_Click(object sender, RoutedEventArgs e)
        {  
            // Call stop reader function
            stopReader();
        }

        /// <summary>
        /// Stops the reader from scanning the assets
        /// Stops the timers as well
        /// </summary>
        private void stopReader()
        {
            ////////////////////////////////////////////////
            menuStopReader.IsEnabled = false;
            imageStop.Source = new BitmapImage(new Uri(@"./IconImages/StopDisabled.png", UriKind.Relative));
            fileMenuStopReader.IsEnabled = false;
            fileMenuImageStop.Source = new BitmapImage(new Uri(@"./IconImages/StopDisabled.png", UriKind.Relative));
            /////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////
            menuScanSingle.IsEnabled = true;
            imgScanOnce.Source = new BitmapImage(new Uri(@"./IconImages/RunSingle.png", UriKind.Relative));
            fileMenuRunSingle.IsEnabled = true;
            imgMenuScanOnce.Source = new BitmapImage(new Uri(@"./IconImages/RunSingle.png", UriKind.Relative));
            /////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////
            menuRunMultipleCont.IsEnabled = true;
            imgRunContinously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultiple.png", UriKind.Relative));
            fileMenuRunMultipleCont.IsEnabled = true;
            imgMenuRunContinuously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultiple.png", UriKind.Relative));
            /////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////
            fileMenuRunAllMultiple.IsEnabled = true;
            imgMenuRunAllMultiple.Source = new BitmapImage(new Uri(@"./IconImages/RunAllMultiple.png", UriKind.Relative));
            ///////////////////////////////////////////////////////////////

            // Stop the timers from running and reset autoStopCount variable
            autoStopCount = 1;
            masterTimer.Stop();
            cTimer.Stop();
        }

        /// <summary>
        /// Scans the multiple tags.
        /// </summary>
        private void scanMultipleTags()
        {

            string mtagID = "";
            int mkeyCount = 0;
            int mduplicateCount = 0;     
             
                do
                {
                    mtagID = getSingleTagID();
                    // Check for null or empty
                    if (!(string.IsNullOrEmpty(mtagID)))
                    {

                        if (!(masterTagIDdB.ContainsValue(mtagID)))
                        {
                            masterTagIDdB.Add(mkeyCount, mtagID);
                            //populateDataGridAssetInfo(mtagID);
                            mkeyCount++;

                        }
                        else
                        {
                            mduplicateCount++;
                            // We got duplicate tagid
                            // use duplicateCount as safety for detecting/stopping the process.
                        }
                    }

                } while (mduplicateCount < maxDuplicateCount);

                // First populate datagrid
                for (int i = 0; i < masterTagIDdB.Count; i++)
                {
                    //Also update the asset location
                    if (isTCPIP == true)
                    {
                        NetworkConnections.updateAssetLocation(masterTagIDdB[i], locID);
                    }
                    populateDataGridAssetInfo(masterTagIDdB[i]);
                    //Store unique asset ID in DB variable
                    if (!(Properties.Settings.Default.scannedAssetUdb.Contains(masterTagIDdB[i])))
                    {
                        Properties.Settings.Default.scannedAssetUdb.Add(masterTagIDdB[i]);
                    }
                    
                }
       
                masterCount = masterTagIDdB.Count; // Number of entries in data dictionary
                //Properties.Settings.Default.scannedAssetsNum += (int)masterCount;

                lblTagsNote.Visibility = Visibility.Visible;
                lblTagsNote.Content = /* masterCount.ToString()*/ "Tagged" + " assets scanned !";
                
                if (AlertNotificationSettings.logsScannedAssets == true)
                {
                    // Log the scannned assets in log file
                    string logScannedAssets = "Scanned Assets: \r\n";

                    for (int k = 0; k < masterCount; k++)
                    {
                        logScannedAssets += "Tag Id:-" + masterTagIDdB[k].ToString() + "\r\n";
                    }

                    LogFile.Log(logScannedAssets);
                }

        }

        /// <summary>
        /// Handles the Tick event of the cTimer control.
        /// Timer Tick for continuous scan for single tag
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void cTimer_Tick(object sender, EventArgs e)
        {
            string getTagId = "";
            srCount = 1;
            // Clears all the data in the table..start fresh
            assetInfoDataSet.AssetInfoDataTable.Clear();

            this.Cursor = Cursors.Wait;
            getTagId = getSingleTagID();
            if (!(string.IsNullOrEmpty(getTagId)))
            {
               
                //Also update the asset location
                if (isTCPIP == true)
                {
                    NetworkConnections.updateAssetLocation(getTagId, locID);
                }
                
                //Log the asset scan event
                if (AlertNotificationSettings.logsScannedAssets == true)
                {
                    LogFile.Log("1 Asset Scanned:- Tag ID- " + getTagId.ToString());
                }

                //Finally populate the grid
                populateDataGridAssetInfo(getTagId);
                //Properties.Settings.Default.scannedAssetsNum += 1;

                //Now store in scanned assets DB
                if (!(Properties.Settings.Default.scannedAssetUdb.Contains(getTagId)))
                {
                    Properties.Settings.Default.scannedAssetUdb.Add(getTagId);
                }

                if (ReaderSettings.isAutoStopEnabled == true)
                {
                    autoStopCount++;
                    if (autoStopCount > ReaderSettings.autoStopReadings)
                    {
                        stopReader();
                    }

                }
            }
            this.Cursor = Cursors.Arrow;
            return;

        }

        /// <summary>
        /// Handles the Tick event of the masterTimer control.
        /// Timer tick for multiple continuous tag scan
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void masterTimer_Tick(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Wait;
            lblTagsNote.Content = "";
            srCount = 1;
            string reftagID = "";
            int refkeyCount = 0;
            int refduplicateCount = 0;
            int getScannedCount;
            /////////////////////////////////////////////////////////////////
            // Clears all the data in the table..start fresh
            assetInfoDataSet.AssetInfoDataTable.Clear();
            //Create a dictionary 
            Dictionary<int, string> refTagIDdB = new Dictionary<int, string>();

              do
                {
                    reftagID = getSingleTagID();

                    // Check for null or empty
                    if (!(string.IsNullOrEmpty(reftagID)))
                    {

                        if (!(refTagIDdB.ContainsValue(reftagID)))
                        {
                            refTagIDdB.Add(refkeyCount, reftagID);
                            //populateDataGridAssetInfo(reftagID);
                            refkeyCount++;

                        }
                        else
                        {
                            refduplicateCount++;
                            // We got duplicate tagid
                            // use duplicateCount as safety for detecting/stopping the process.
                        }
                    }

                } while (refduplicateCount < maxDuplicateCount);

              // First populate datagrid
              for (int i = 0; i < refTagIDdB.Count; i++)
              {
                  //Also update the asset location before populating
                  if (isTCPIP == true)
                  {
                      NetworkConnections.updateAssetLocation(refTagIDdB[i], locID);
                  }
                  populateDataGridAssetInfo(refTagIDdB[i]);

                  //Now store in scanned assets DB
                  if (!(Properties.Settings.Default.scannedAssetUdb.Contains(refTagIDdB[i])))
                  {
                      Properties.Settings.Default.scannedAssetUdb.Add(refTagIDdB[i]);
                  }

              }
            
                getScannedCount = refTagIDdB.Count;
                //Properties.Settings.Default.scannedAssetsNum += (int)getScannedCount;
                
            // Check for logging feature turned ON
              if (AlertNotificationSettings.logsScannedAssets == true)
              {
                  // Log the scannned assets in log file
                  string logScannedAssets = "Scanned Assets: \r\n";

                  for (int k = 0; k < getScannedCount; k++)
                  {
                      logScannedAssets += "Tag Id:-" + refTagIDdB[k].ToString() + "\r\n";
                  }

                  LogFile.Log(logScannedAssets);
              }


            /////Detection of missing or added tags
            // Total tags missing or newly added
            //var tagDiff = masterTagIDdB.Values.Except(refTagIDdB.Values).Concat(refTagIDdB.Values.Except(masterTagIDdB.Values));
            
            // Missing tags
            var missingTags = masterTagIDdB.Values.Except(refTagIDdB.Values);

            // Newly added tags
            var newAddedTags = refTagIDdB.Values.Except(masterTagIDdB.Values);

            if (newAddedTags.Count() > 0)
            {
                // New tags found
                lblTagsNote.Visibility = Visibility.Visible;

                lblTagsNote.Content = newAddedTags.Count().ToString() + " new tag(s) found!  ";

                string logMovingAssetsInOut = "";
                // Log the scannned assets in log file
                logMovingAssetsInOut = "Assets moving In Range: \r\n";
                int freqCount = 0;
                // Count the collection of newly added tags
                for (int k = 0; k < newAddedTags.Count(); k++)
                {
                    if (!(Properties.Settings.Default.assetMovementDB.Contains(newAddedTags.ElementAt(k))))
                    {
                        Properties.Settings.Default.assetMovementDB.Add(newAddedTags.ElementAt(k), (int)1);
                    }
                    else
                    {
                        freqCount = (int)Properties.Settings.Default.assetMovementDB[newAddedTags.ElementAt(k)];
                        ++freqCount;
                        Properties.Settings.Default.assetMovementDB[newAddedTags.ElementAt(k)] = freqCount;
                    }
                    logMovingAssetsInOut += "Tag Id:-" + newAddedTags.ElementAt(k) + "\r\n";
                }

                // Check for logging feature turned ON
                if (AlertNotificationSettings.logsCAP == true)
                {   
                    LogFile.Log(logMovingAssetsInOut);
                }          
                if (AlertNotificationSettings.sendEmailUser == true)
                {
                    HATrakaMain.emailContent = logMovingAssetsInOut;
                    sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                }
                if (AlertNotificationSettings.sendSMSUser == true)
                {
                    SMSClass.sendSMS(logMovingAssetsInOut, LoginWindow.user_contact);

                    if (AlertNotificationSettings.sendSMSAdmin == true)
                    {
                        SMSClass.sendSMS(logMovingAssetsInOut, HATrakaMain.superAdminSMS);
                    }
                }


            }
            if (missingTags.Count() > 0)
            {
                // Some tags are missing..alert
                
                //int abstagCountDiff = Math.Abs(tagCountDiff) ;
                lblTagsNote.Visibility = Visibility.Visible;

                lblTagsNote.Content += "Alert: " + missingTags.Count().ToString() + " tag(s) missing!";

                string logMovingAssetsInOut = "";

                // Log the scannned assets in log file
                logMovingAssetsInOut = "Assets moving Out of Range: \r\n";
                // Count the collection of out-of range or missing tags
                int freqCount = 0;

                for (int k = 0; k < missingTags.Count(); k++)
                {
                    if (!(Properties.Settings.Default.assetMovementDB.Contains(missingTags.ElementAt(k))))
                    {
                        Properties.Settings.Default.assetMovementDB.Add(missingTags.ElementAt(k), (int)1);
                    }
                    else
                    {
                        freqCount = (int)Properties.Settings.Default.assetMovementDB[missingTags.ElementAt(k)];
                        ++freqCount;
                        Properties.Settings.Default.assetMovementDB[missingTags.ElementAt(k)] = freqCount;
                    }
                    logMovingAssetsInOut += "Tag Id:-" + missingTags.ElementAt(k) + "\r\n";
                }

                // Check for logging feature turned ON
                if (AlertNotificationSettings.logsCAP == true)
                {
                    LogFile.Log(logMovingAssetsInOut);
                }

                if (AlertNotificationSettings.sendEmailUser == true)
                {
                    HATrakaMain.emailContent = logMovingAssetsInOut;
                    sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                }
                if (AlertNotificationSettings.sendSMSUser == true)
                {
                    SMSClass.sendSMS(logMovingAssetsInOut, LoginWindow.user_contact);

                    if (AlertNotificationSettings.sendSMSAdmin == true)
                    {
                        SMSClass.sendSMS(logMovingAssetsInOut, HATrakaMain.superAdminSMS);
                    }
                }

            }

            if((missingTags.Count() == 0) && (newAddedTags.Count() == 0)) //no change
            {
                lblTagsNote.Content = "";
                lblTagsNote.Visibility = Visibility.Hidden;
            }

            if (ReaderSettings.isAutoStopEnabled == true)
            {
                autoStopCount++;
                if (autoStopCount > ReaderSettings.autoStopReadings)
                {
                    stopReader();
                }
            }

            // Finally replace the Master tag DB with the ref TagID db. So master updates everytime
            masterTagIDdB = refTagIDdB;

            this.Cursor = Cursors.Arrow;
            return;
        }

        /// <summary>
        /// Gets the single tag ID.
        /// Function according to API documentation
        /// </summary>
        /// <returns></returns>
        private string getSingleTagID()
        {
            
            //long tk1, tk2,tkDiff;
            byte TagType = 9;
            byte[] valueRead = new byte[12];
            string CardId = "";
           
            short getResult = readerDLL.SingleTagIdentifyEX(readerDLL.hComm, ref TagType, valueRead);
            

            if (getResult == 0)
            {

                // Tag obtained
                if (TagType > 0)
                {
                    
                    int Length = GetTagLength(TagType);
                    for (int tmp = 0; tmp < Length; tmp++)
                    {
                        CardId = CardId + valueRead[tmp].ToString("X2");    //X 16 hex 2: two bit less than the front zeros
                    }

                }
                else
                {
                    //No tags got
                    lblTagsNote.Visibility = Visibility.Visible;
                     
                    lblTagsNote.Content = "No tags found!";
                    //MessageBox.Show("Reader could not detect any tags! Please try again", "No tags found", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
                
            }
            else
            {
                lblTagsNote.Visibility = Visibility.Visible;
                 
                lblTagsNote.Content = "Reader failed to perform this operation !";
                //Failed to perform this operation
                //MessageBox.Show("Reader failed to perform this operation! Please try again", "Scan Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return CardId;
        }

        /// <summary>
        /// Handles the Tick event of the allScanTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void allScanTimer_Tick(object sender, EventArgs e)
        {
            readerNumber++;
            if (readerNumber >= totalReadersConnected)
            {
                readerNumber = 0;
                isAllReaderScanEnabled = false;
                textBlockReaderInfo.Visibility = Visibility.Visible;
                textBlockReaderInfo.Text = "All Reader Scan completed successfully !";
                allScanTimer.IsEnabled = false;
                allScanTimer.Stop();
            }
            else
            {
                masterTagIDdB.Clear();
                scanMultipleAllReaders();
            }
            return;

        }

        /// <summary>
        /// Handles the Click event of the fileMenuRunAllMultiple control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void fileMenuRunAllMultiple_Click(object sender, RoutedEventArgs e)
        {
            // Reset reader number
            readerNumber = 0;
            isAllReaderScanEnabled = true;

            ////////////////////////////////////////////////
            menuRunMultipleCont.IsEnabled = false;
            imgRunContinously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultipleDisabled.png", UriKind.Relative));
            fileMenuRunMultipleCont.IsEnabled = false;
            imgMenuRunContinuously.Source = new BitmapImage(new Uri(@"./IconImages/RunMultipleDisabled.png", UriKind.Relative));
            /////////////////////////////////////////////////////////////////
            if (isReaderDisconnected == false)
            {
                disconnectTheReader();
            }

            textBlockReaderInfo.Visibility = Visibility.Visible;
            textBlockReaderInfo.Text = "Starting reader:#0 scan!";
    
            masterTagIDdB.Clear(); //Clear the db

            scanMultipleAllReaders();
            if (connectionSuccessfull == true)
            {
                allScanTimer.Start();
            }
           
        }

        /// <summary>
        /// Scans the multiple tags for all readers one by one
        /// </summary>
        private void scanMultipleAllReaders()
        {
                
                this.Cursor = Cursors.Wait;

                //Check for events that are going on and close them properly
                if ((cTimer.IsEnabled == true) || (masterTimer.IsEnabled == true))
                {
                    stopReader();
                }
                if (isReaderDisconnected == false)
                {
                    disconnectTheReader();
                }

                // Start. Based on reader number get the IP (check condition in ping and connect)
                pingTheReader();
                checkReaderConnection(); //Check the readerIP and connect the reader.
                // Perform scan once;
                //
                if (connectionSuccessfull == true)
                {
                    //srCount = 1;
                    scanMultipleTags();

                    //Disconnect the reader
                    disconnectTheReader();
                    textBlockReaderInfo.Visibility = Visibility.Visible;
                    textBlockReaderInfo.Text = "Resuming next reader:#" + (readerNumber + 1).ToString() + " scan in 10 sec !";
                }
                else
                {
                    textBlockReaderInfo.Visibility = Visibility.Visible;
                    textBlockReaderInfo.Text = "Reader Scan abrupted !";
                    readerNumber = 0;
                    isAllReaderScanEnabled = false;
                    if (allScanTimer.IsEnabled == true)
                    {
                        allScanTimer.Stop();
                    }

                }
                this.Cursor = Cursors.Arrow;

                return;

        }   

        //private bool GetMultiID()
        //{
        //used only for getting the scanned count
        //    uint TagType = 9;
        //    //byte[] value = new byte[12];
        //    //int Addr = 0;
        //    //byte ParaCount = 120;
        //    //short Result = readerDLL.GetParameter(readerDLL.hComm, Addr, ParaCount, value);  //To get parameter values.
    
        //    readerDLL.TagIds value = new readerDLL.TagIds();
        //    short Result = readerDLL.MultipleTagIdentifyEX(readerDLL.hComm, TagType, ref getScannedCount, ref value);         // Description count: the number of tags. value: read the label, the format for the AX B. .. XXXXAX B. .. XXXXAX B. ..........
             
        //    if (Result == 0)                                                                            // Wherein A is the length of the label to 8:00 length of a length of 8, 9, 4,10 length of 6, and B for the label, each label length (number of bits) by A impact
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /****************************************************************************/

        //Scanned Assets Grid functions
        //Populate,Filter grid data etc

        /// <summary>
        /// Populates the data grid with the given asset record
        /// </summary>
        /// <param name="oTagId">The o tag id.</param>
        private void populateDataGridAssetInfo(string oTagId)
        {
            dgDisplayScannedAssets.Visibility = Visibility.Visible;
            //ztATdbLocalDataSet1.AssetInfoDataTableRow rowAssetInfoRecord = new ztATdbLocalDataSet1.AssetInfoDataTableRow( rb);

            ztATdbLocalDataSet1.AssetInfoDataTableDataTable tempDTable = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();

            try
            {
                AssetInfoTA.Fill(tempDTable, oTagId); //this table may have only one row

                // create columns for the DataTable

                tempDTable.Columns.Add("sr_no", typeof(System.Int32));
                tempDTable.Columns.Add("timestamp", typeof(DateTime));

                //// specify it as auto increment field
                //sr_no.AutoIncrement = true;
                //sr_no.AutoIncrementSeed = 1;
                //sr_no.ReadOnly = true;

                if (tempDTable.Rows.Count > 0)
                {
                    tempDTable.Rows[0]["sr_no"] = srCount++;
                    tempDTable.Rows[0]["timestamp"] = DateTime.Now;
                }

                // Merge the temp table into the main dataset datatable
                assetInfoDataSet.AssetInfoDataTable.Merge(tempDTable);

            }

            catch
            {
                LogFile.Log("Error: Failed to retrieve records from the database !");
            }
            ///////////////////////////////////////////////////////////////////////
            //AssetInfoTA.Fill(assetInfoDataSet.AssetInfoDataTable, oTagId);
            //dgDisplayScannedAssets.DataContext
            // use the AssetInfoDataTable table as the DataContext for this datagrid
            dgDisplayScannedAssets.DataContext = assetInfoDataSet.AssetInfoDataTable.DefaultView;
                     
        }

        /// <summary>
        /// Handles the SelectionChanged event of the dgDisplayScannedAssets control.
        /// On selection of record from the datagrid populate the textfields
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void dgDisplayScannedAssets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDisplayScannedAssets.SelectedItems.Count == 1)
            {
                string mapAssetDesc = "";
                string mapAssetID = "";
                string mapAssetLoc = "";

                lblTagsNote.Visibility = Visibility.Hidden;

                showDGscannedAssetFields();
                
                showScannedAlertFields();
                
                DataRowView rowSelectedRecord = (DataRowView)dgDisplayScannedAssets.SelectedItems[0];
                lblAssetID.Content = rowSelectedRecord["asset_id"].ToString().TrimEnd();
                lblTagID.Content = rowSelectedRecord["tag_id"].ToString().TrimEnd();
                lblAssetLocation.Content = rowSelectedRecord["asset_location"].ToString().TrimEnd();
                lblAssetCategory.Content = rowSelectedRecord["asset_category"].ToString().TrimEnd();
                textBlockAssetDesc.Text = rowSelectedRecord["asset_desc"].ToString().TrimEnd();
                lblAssetStatusMain.Content = rowSelectedRecord["asset_status"].ToString().TrimEnd();
                lblAssetModel.Content = rowSelectedRecord["asset_model"].ToString().TrimEnd();
                lblAssetQuantity.Content = rowSelectedRecord["asset_quantity"].ToString().TrimEnd();
                lblAssetValue.Content = rowSelectedRecord["asset_value"].ToString().TrimEnd();
                textBlockAssetComments.Text = rowSelectedRecord["asset_comments"].ToString().TrimEnd();

                //Alerts
               

                lblAssetIDAlert.Content = rowSelectedRecord["asset_id"].ToString().TrimEnd();
                lblTagExpiryAlert.Content = rowSelectedRecord["tag_expiry"].ToString().TrimEnd();
                lblAssetMaintAlert.Content = rowSelectedRecord["asset_maint_due"].ToString().TrimEnd();

                //Maps
                mapAssetID = rowSelectedRecord["asset_id"].ToString().TrimEnd();
                mapAssetDesc = rowSelectedRecord["asset_desc"].ToString().Trim();
                mapAssetLoc = rowSelectedRecord["asset_location"].ToString().Trim();
                populateMap(mapAssetID, mapAssetDesc, mapAssetLoc);

                //string getImageType = rowSelectedRecord["asset_image"].GetType().Name.ToString();

                if (rowSelectedRecord["asset_image"] != DBNull.Value)
                {
                    //Image retreiving
                    byte[] imageBytes = (byte[])rowSelectedRecord["asset_image"];


                    if (imageBytes.Length > 0)
                    {
                        BitmapImage bitmapimage = new BitmapImage();
                        // Convert byte[] to Image   
                        bitmapimage.BeginInit();
                        bitmapimage.StreamSource = new MemoryStream(imageBytes);
                        bitmapimage.EndInit();
                        imageAssetImage.Source = (System.Windows.Media.ImageSource)bitmapimage;
                    }
                    else
                    {
                        imageAssetImage.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative));
                    }
                    
                }
                else
                {
                    imageAssetImage.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative));

                }
            }
            else
            {
                lblTagsNote.Visibility = Visibility.Visible;
                lblTagsNote.Content = "Please select one record to its view details";
                hideDGscannedAssetFields();
                
                hideScannedAlertFields();
            }

        }

        /// <summary>
        /// Handles the Selected event of the TreeItemRemoveAndClear control.
        /// This will remove and clear the data source for datagrid
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void TreeItemRemoveAndClear_Selected(object sender, RoutedEventArgs e)
        {
            // Set UI elements
            lblTagsNote.Visibility = Visibility.Hidden;
            srCount = 1;
            // Clears all the data in the table..start fresh
            assetInfoDataSet.AssetInfoDataTable.Clear();

            // use the AssetInfoDataTable table as the DataContext for this datagrid
            dgDisplayScannedAssets.DataContext = assetInfoDataSet.AssetInfoDataTable.DefaultView;
        }

        /// <summary>
        /// Handles the Selected event of the TreeItemClearDisplay control.
        /// Simply clear the data grid but keep the source
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void TreeItemClearDisplay_Selected(object sender, RoutedEventArgs e)
        {
            // Set UI elements
            lblTagsNote.Visibility = Visibility.Hidden;

            srCount = 1;
            ztATdbLocalDataSet1.AssetInfoDataTableDataTable tmpTable = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();
            // this table is empty..
            dgDisplayScannedAssets.DataContext = tmpTable.DefaultView;
        }

        /// <summary>
        /// Handles the Selected event of the TreeItemRefreshDisplay control.
        /// Populate the datagrid with the original source
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void TreeItemRefreshDisplay_Selected(object sender, RoutedEventArgs e)
        {
            // use the AssetInfoDataTable table as the DataContext for this datagrid
            dgDisplayScannedAssets.DataContext = assetInfoDataSet.AssetInfoDataTable.DefaultView;
        }

        /// <summary>
        /// Handles the LostFocus event of the textBoxSearchID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSearchID.Text.ToString()))
            {
                textBoxSearchID.Background = Brushes.White;
                textBoxSearchID.Foreground = Brushes.LightSlateGray;
                textBoxSearchID.BorderBrush = Brushes.LightSlateGray;
                textBoxSearchID.Text = "<Search the grid>";

            }

            
        }

        /// <summary>
        /// Handles the GotFocus event of the textBoxSearchID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchID_GotFocus(object sender, RoutedEventArgs e)
        {
            // Change control graphics
            if (textBoxSearchID.Text.CompareTo("<Search the grid>") == 0)
            {
                textBoxSearchID.Background = Brushes.LightYellow;
                textBoxSearchID.Foreground = Brushes.Black;
                textBoxSearchID.BorderBrush = Brushes.YellowGreen;
                textBoxSearchID.Text = "";
            }
           
        }

        /// <summary>
        /// Handles the Click event of the btnSearchtag control.
        /// Search feature to search the datagrid results
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnSearchtag_Click(object sender, RoutedEventArgs e)
        {
            listBoxSuggestion.Visibility = Visibility.Collapsed;
            // *change* here if necessary
            int maxNumberOfSearchWords = 25;
            //srCount = 1;
            string wChar = "";
            wChar = textBoxSearchID.Text.Trim().ToString();
            // Compute and add search list items and history

            if ((string.IsNullOrEmpty(wChar)) || (wChar.CompareTo("<Search the grid>") == 0))
            {
                dgDisplayScannedAssets.Visibility = Visibility.Hidden;
                
                hideDGscannedAssetFields();
                hideScannedAlertFields();

                lblTagsNote.Visibility = Visibility.Visible;
                lblTagsNote.Content = "Please enter a search term!";
                return;
            }

            else if (!(string.IsNullOrEmpty(wChar)))
            {
                lblTagsNote.Visibility = Visibility.Hidden;
                //Filter out duplicate search terms
                if (!(searchList.Contains(wChar)))
                {
                    searchList.Add(wChar);

                }
                if (!(listBoxHistory.Items.Contains(wChar)))
                {
                    listBoxHistory.Items.Add(wChar);
                }

                // Check for itemms count for search list and History
                if (listBoxHistory.Items.Count > maxNumberOfSearchWords)
                {
                    listBoxHistory.Items.RemoveAt(0); // Remove the oldest search term
                }

                if (searchList.Count > maxNumberOfSearchWords)
                {
                    searchList.RemoveAt(0);
                }
            }
                      

            ztATdbLocalDataSet1.AssetInfoDataTableDataTable searchTable = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();
            //ztATdbLocalDataSet1.AssetInfoDataTableDataTable tmpResTable = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();
            searchTable = assetInfoDataSet.AssetInfoDataTable;
            DataView dv = new DataView(searchTable);

            dv.RowFilter = @"[asset_id] LIKE '%" + wChar + "%'" + " OR " +
                            "[tag_id] LIKE '%" + wChar + "%'" + " OR " +
                            "[asset_location] LIKE '%" + wChar + "%'" + " OR " +
                            "[asset_category] LIKE '%" + wChar + "%'";

            
                lblTagsNote.Visibility = Visibility.Visible;
                 
                lblTagsNote.Content = dv.Count.ToString() + " records found!";
            
            //tmpResTable.AddAssetInfoDataTableRow(searchTable.FindByasset_id(textBoxSearchID.Text));
            //dgDisplayScannedAssets.FindName("a");
            dgDisplayScannedAssets.DataContext = dv;
        }


        // DATA GRID Filter scanned Assets
        // Filter categories
        
        /// <summary>
        /// Handles the Selected event of the treeViewFilterACFixed control.
        /// Filter category: Fixed Assets
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewFilterACFixed_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_category]", "Fixed");
        }

        /// <summary>
        /// Handles the Selected event of the treeViewFilterACPortable control.
        /// Filter category: Portable Assets
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewFilterACPortable_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_category]", "Portable");
        }

        /// <summary>
        /// Handles the Selected event of the treeViewFilterACMovable control.
        /// Filter category: Movable Assets
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewFilterACMovable_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_category]", "Movable");
        }

        /// <summary>
        /// Handles the Selected event of the tviASok control.
        /// Filter category: Asset Status- OK
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviASok_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_status]", "OK");
        }

        /// <summary>
        /// Handles the Selected event of the tviASnew control.
        /// Filter category: Asset Status- NEW
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviASnew_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_status]", "NEW");
        }

        /// <summary>
        /// Handles the Selected event of the tviASrepaired control.
        /// Filter category: Asset Status- REPAIR
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviASrepaired_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_status]", "REPAIR");
        }

        /// <summary>
        /// Handles the Selected event of the tviAStransit control.
        /// Filter category: Asset Status- TRANSIT
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviAStransit_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_status]", "TRANSIT");
        }

        /// <summary>
        /// Handles the Selected event of the tviASserviced control.
        /// Filter category: Asset Status- SERVICED
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviASserviced_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_status]", "SERVICED");
        }

        /// <summary>
        /// Handles the Selected event of the tviASfault control.
        /// Filter category: Asset Status- FAULT
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviASfault_Selected(object sender, RoutedEventArgs e)
        {
            scannedAssetsFilter("[asset_status]", "FAULT");

        }

        /// <summary>
        /// Filter the scanned assets according to the filter category and parameters
        /// </summary>
        /// <param name="mainFilter">The main filter.</param>
        /// <param name="subFilter">The sub filter.</param>
        private void scannedAssetsFilter(string mainFilter, string subFilter)
        {

            if (assetInfoDataSet.AssetInfoDataTable.Rows.Count > 0)
            {

                DataView dv = new DataView(assetInfoDataSet.AssetInfoDataTable);

                //dv.RowFilter = "[asset_category] = '" + "Fixed" + "'";
                dv.RowFilter = mainFilter + "= '" + subFilter + "'";

                if (dv.Count > 0)
                {
                    lblTagsNote.Visibility = Visibility.Visible;
                     
                    lblTagsNote.Content = dv.Count.ToString() + " records found!";
                }
                else
                {
                    lblTagsNote.Visibility = Visibility.Visible;
                     
                    lblTagsNote.Content = "No records found for this filter!";

                }
                dgDisplayScannedAssets.DataContext = dv;
            
 
            }
 
        }

 
        //Auto complete search box functionality and search history.
        // Functions and events for autocomplete list box and history

        /// <summary>
        /// Handles the SelectionChanged event of the listBoxSuggestion control.
        /// UI handled to make it easier
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void listBoxSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxSuggestion.ItemsSource != null)
            {
                //listBoxSuggestion.Visibility = Visibility.Collapsed;
                textBoxSearchID.TextChanged -= new TextChangedEventHandler(textBoxSearchID_TextChanged);

                if (listBoxSuggestion.SelectedIndex != -1)
                {
                    textBoxSearchID.Focus();
                    textBoxSearchID.Text = listBoxSuggestion.SelectedItem.ToString();
                    textBoxSearchID.SelectAll();
                    
                }
                textBoxSearchID.TextChanged += new TextChangedEventHandler(textBoxSearchID_TextChanged);
            }

        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxSearchID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string typedString = textBoxSearchID.Text;
            List<string> autoList = new List<string>();
            autoList.Clear();

            foreach (string item in searchList)
            {
                if (!string.IsNullOrEmpty(textBoxSearchID.Text))
                {
                    if (item.StartsWith(typedString))
                    {
                        autoList.Add(item);
                    }
                }

            }

            if (autoList.Count > 0)
            {
                listBoxSuggestion.ItemsSource = autoList;
                listBoxSuggestion.Visibility = Visibility.Visible;
                
            }

            else if (textBoxSearchID.Text.Equals(""))
            {
                listBoxSuggestion.ItemsSource = null;
                listBoxSuggestion.Visibility = Visibility.Collapsed;

            }
            //else
            //{
            //    listBoxSuggestion.ItemsSource = null;
            //    listBoxSuggestion.Visibility = Visibility.Collapsed;
            //}
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the textBoxSearchID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {          
                listBoxSuggestion.Focus();
                //textBoxSearchID.Text = listBoxSuggestion.SelectedItem.ToString();
            }
            
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the listBoxSuggestion control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void listBoxSuggestion_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                textBoxSearchID.Focus();
                textBoxSearchID.SelectAll();
                listBoxSuggestion.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the GotMouseCapture event of the listBoxSuggestion control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void listBoxSuggestion_GotMouseCapture(object sender, MouseEventArgs e)
        {
            textBoxSearchID.Focus();
            textBoxSearchID.SelectAll();
            listBoxSuggestion.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Selected event of the treeViewHistoryToday control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewHistoryToday_Selected(object sender, RoutedEventArgs e)
        {
            if (listBoxHistory.Items.Count > 0)
            {
                listBoxHistory.Visibility = Visibility.Visible;
            }
            else
            {
                listBoxHistory.Visibility = Visibility.Hidden;
            }

            
        }

        /// <summary>
        /// Handles the SelectionChanged event of the listBoxHistory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void listBoxHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxHistory.SelectedItem != null)
            {
                textBoxSearchID.Text = listBoxHistory.SelectedItem.ToString();
                listBoxSuggestion.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the Selected event of the treeViewClearHistory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewClearHistory_Selected(object sender, RoutedEventArgs e)
        {
            listBoxHistory.Items.Clear();
            listBoxHistory.Visibility = Visibility.Hidden;
            //listBoxHistory.Items.Add("<no history>");
        }

        // End of auto complete search and search history...

        // START Tools Menu Options
        // User and Tag options

        /***************************************************************/

        //Menu Tools Options
        // Reader settings, Asset Tag and User profile management, alerts,skins and maps

        /// <summary>
        /// Handles the Click event of the menuReaderSettings control.
        /// Reader settings for the application
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuReaderSettings_Click(object sender, RoutedEventArgs e)
        {
            ReaderSettings readerset = new ReaderSettings();
            readerset.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuToolsUserOptions control.
        /// User profile management
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuToolsUserOptions_Click(object sender, RoutedEventArgs e)
        {
            UserOptions uoptions = new UserOptions();
            uoptions.Show();

        }

        /// <summary>
        /// Handles the Click event of the menuToolsAssetTagOptions control.
        /// Asset tag management
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuToolsAssetTagOptions_Click(object sender, RoutedEventArgs e)
        {
            tagOptions tgopt = new tagOptions();
            tgopt.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuToolsReaderMapManage control.
        /// Manages the reader and map info.
        /// Create, edit, update add and delete
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuToolsReaderMapManage_Click(object sender, RoutedEventArgs e)
        {
            ReaderMapProfile readermapP = new ReaderMapProfile();
            readermapP.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuToolsTestDatabase control.
        /// Tool for testing the database
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuToolsTestDatabase_Click(object sender, RoutedEventArgs e)
        {
            TestDatabaseConnection testdb = new TestDatabaseConnection();
            testdb.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuToolsTestNetwork control.
        /// Tool for checking the network connection properties. Netstat functionality implemented
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuToolsTestNetwork_Click(object sender, RoutedEventArgs e)
        {
            NetworkConnections nc = new NetworkConnections();
            nc.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuToolsMaps control.
        /// Map view
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuToolsMaps_Click(object sender, RoutedEventArgs e)
        {
            MapWindow map = new MapWindow();
            map.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuToolsSkins control.
        /// Tool for changing the skin of the app.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuToolsSkins_Click(object sender, RoutedEventArgs e)
        {
            SkinsWindow skw = new SkinsWindow();
            skw.Show();
        }

        /***************************************************************/

        // Maps -panning and zooming features
        // Floor map


        /// <summary>
        /// Handles the MouseLeftButtonUp event of the imageMap control.
        /// Release the mouse from map panning
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor cursorOpenHand = new Cursor(new System.IO.MemoryStream(ZTraka_App.Properties.Resources.openhand));
            this.Cursor = cursorOpenHand;
            imageMap.ReleaseMouseCapture();
        }

        /// <summary>
        /// Handles the MouseEnter event of the imageMap control.
        /// Gets the open hand mouse shape
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor cursorOpenHand = new Cursor(new System.IO.MemoryStream(ZTraka_App.Properties.Resources.openhand));
            this.Cursor = cursorOpenHand;

        }

        /// <summary>
        /// Handles the MouseLeave event of the imageMap control.
        /// Restore default mouse arrow
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the imageMap control.
        /// MAP panning feature activated on left button down
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Cursor cursorClosedHand = new Cursor(new System.IO.MemoryStream(ZTraka_App.Properties.Resources.closedhand));
            //Changes the cursor figure to indicate drag on map avaiable
            this.Cursor = cursorClosedHand;

            if (imageMap.IsMouseCaptured) return;
            imageMap.CaptureMouse();

            start = e.GetPosition(imageMap);
            //origin.X = imageMap.RenderTransform.Value.OffsetX;
            //origin.Y = imageMap.RenderTransform.Value.OffsetY;
        }

        /// <summary>
        /// Handles the MouseMove event of the imageMap control.
        /// Map panning feature
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (!imageMap.IsMouseCaptured) return;
            p = e.MouseDevice.GetPosition(borderMap);

            Matrix m = imageMap.RenderTransform.Value;
            m.OffsetX = origin.X + (p.X - start.X);
            m.OffsetY = origin.Y + (p.Y - start.Y);

            imageMap.RenderTransform = new MatrixTransform(m);
        }

        /// <summary>
        /// Handles the MouseWheel event of the imageMap control.
        /// Zoom feature also available through mouse wheel scroll
        /// Subject to the slider max and min value
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseWheelEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            p = e.MouseDevice.GetPosition(imageMap);

            Matrix m = imageMap.RenderTransform.Value;
            if (e.Delta > 0)
            {
                sliderMapZoom.Value++;
                if ((sliderMapZoom.Value <= maxRange) && (flagSliderZoom))
                {
                    if (sliderMapZoom.Value == maxRange)
                    {
                        flagSliderZoom = false;
                    }

                    m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
                    flagSliderUNZoom = true;
                }
            }
            else
            {
                sliderMapZoom.Value--;
                if ((sliderMapZoom.Value >= 0) && (flagSliderUNZoom))
                {
                    if (sliderMapZoom.Value == 0)
                    {
                        flagSliderUNZoom = false;
                    }
                    m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);
                }
                flagSliderZoom = true;
            }
            imageMap.RenderTransform = new MatrixTransform(m);
        }

        /// <summary>
        /// map zoom_ value changed.
        /// Zooming feature on slider
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderMapZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double xcenter = 333;
            double ycenter = 107;
            p.X = xcenter;
            p.Y = ycenter;

            Matrix m = imageMap.RenderTransform.Value;
            if (e.NewValue > e.OldValue) //then zoom
                m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
            else // UnZoom
                m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);

            imageMap.RenderTransform = new MatrixTransform(m);
        }

        /// <summary>
        /// Handles the Click event of the defaultImageView control.
        /// Resets the map to original view/position
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void defaultImageView_Click(object sender, RoutedEventArgs e)
        {
            sliderMapZoom.Value = 0;
            imageMap.RenderTransform = new MatrixTransform(mOriginal);
        }

        /// <summary>
        /// Populates the map.
        /// A function to highlight the selected asset in the floor map
        /// </summary>
        /// <param name="mapAID">The map AID.</param>
        /// <param name="mapADesc">The map A desc.</param>
        private void populateMap(string mapAID, string mapADesc, string mapALoc)
        {
            try
            {
                System.Windows.Shapes.Ellipse assettoRemove = (System.Windows.Shapes.Ellipse)imageMap.FindName(mapEle);
                if (assettoRemove != null)
                {
                    assettoRemove.Visibility = Visibility.Hidden;
                }
              
            }
            catch
            {
 
            }

            //mapALoc = "DOC";
            string findElementAsset = "";
            findElementAsset = "a1" + mapALoc;
            mapEle = findElementAsset;
            System.Windows.Shapes.Ellipse assetName = (System.Windows.Shapes.Ellipse)imageMap.FindName(findElementAsset);

            if (assetName != null)
            {
                // Asset Location
                assetName.Visibility = Visibility.Visible;
                assetName.ToolTip = "Asset ID: " + mapAID + " Description: " + mapADesc;
                //evt1.SourceName = "asset1";
            }
            try
            {
                Storyboard storyboard = Resources["AssetLoaded"] as Storyboard;
                Storyboard.SetTargetName(storyboard, assetName.Name.ToString());
                storyboard.Begin();
            }
            catch
            {
                //LogFile.Log("Error: Issues with Map display for assets ");
            }
        }

        /***************************************************************/

        // Quick Browse Database search functions and features
        // Browse DB, filters and other features

        /// <summary>
        /// Handles the GotFocus event of the textBoxSearchDB control.
        /// Search BOX GUI feature
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchDB_GotFocus(object sender, RoutedEventArgs e)
        {
            // Change control graphics
            if (textBoxSearchDB.Text.CompareTo("<Enter the search term>") == 0)
            {
                textBoxSearchDB.Background = Brushes.LightYellow;
                textBoxSearchDB.Foreground = Brushes.Black;
                textBoxSearchDB.BorderBrush = Brushes.YellowGreen;
                textBoxSearchDB.Text = "";
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the textBoxSearchDB control.
        /// Handles the UI part
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchDB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSearchDB.Text.ToString()))
            {
                textBoxSearchDB.Background = Brushes.White;
                textBoxSearchDB.Foreground = Brushes.LightSlateGray;
                textBoxSearchDB.BorderBrush = Brushes.LightSlateGray;
                textBoxSearchDB.Text = "<Enter the search term>";
 
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbSearchCategory control.
        /// Select the category to search Assets,users or Map Reader info
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void cmbSearchCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblTagsNote.Visibility = Visibility.Hidden;
            dataGridBrowseDatabase.Visibility = Visibility.Hidden;
            textBlockSearchResult.Visibility = Visibility.Hidden;

            hideBrowseDBFields();
            hideAlertDBFields();
            
            //Check for selected category
            if (cmbSearchCategory.SelectedIndex == -1)
            {
                return;
            }
            else if (((System.Windows.Controls.ContentControl)(cmbSearchCategory.SelectedItem)).Content.ToString().CompareTo("Assets") == 0)
            {      
                //columnOne.Header = "Assets";
                gridDynamicColumns(0);
                tviDBFilterCat1.Header = "By Asset Category";
                treeViewDBFilterFixed.Header = "Fixed";
                treeViewDBFilterPortable.Header = "Portable";
                treeViewDBFilterMovable.Header = "Movable";

                tviDBFilterCat2.Visibility = Visibility.Visible;

            }            
            else if (cmbSearchCategory.SelectedIndex == 1)
            {
                //columnOne.Header = "Users";
                gridDynamicColumns(1);
                tviDBFilterCat1.Header = "By User Roles";
                treeViewDBFilterFixed.Header = "ADMIN";
                treeViewDBFilterPortable.Header = "ADMIN2";
                treeViewDBFilterMovable.Header = "Restricted";

                tviDBFilterCat2.Visibility = Visibility.Hidden;

            }

            else if (cmbSearchCategory.SelectedIndex == 2)
            {
                //columnOne.Header = "Reader Info";
                gridDynamicColumns(2);
                tviDBFilterCat1.Header = "By Floor";
                treeViewDBFilterFixed.Header = "First";
                treeViewDBFilterPortable.Header = "Second";
                treeViewDBFilterMovable.Header = "Third";

                tviDBFilterCat2.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Sets the dynamic columns for the datagrid according to the selection
        /// </summary>
        /// <param name="selectCategory">The select category.</param>
        private void gridDynamicColumns(int selectCategory)
        {
            if (selectCategory == 0) //Assets
            {
                columnOne.Header = "Asset ID";
                columnOne.Binding = new Binding("asset_id");

                columnTwo.Header = "Tag ID";
                columnTwo.Binding = new Binding("tag_id");

                columnThree.Header = "Asset Location";
                columnThree.Binding = new Binding("asset_location");

                columnFour.Header = "Asset Category";
                columnFour.Binding = new Binding("asset_category");

                columnFive.Header = "Asset Status";
                columnFive.Binding = new Binding("asset_status");

                
 
            }

            else if (selectCategory == 1) //Users
            {
                columnOne.Header = "User ID";
                columnOne.Binding = new Binding("user_id");

                columnTwo.Header = "User Name";
                columnTwo.Binding = new Binding("user_name");

                columnThree.Header = "User Dept";
                columnThree.Binding = new Binding("user_dept");

                columnFour.Header = "User Role";
                columnFour.Binding = new Binding("user_role");

                columnFive.Visibility = Visibility.Hidden;
 
            }

            else if (selectCategory == 2) //Reader Info
            {
                columnOne.Header = "Reader ID";
                columnOne.Binding = new Binding("reader_id");

                columnTwo.Header = "Location ID";
                columnTwo.Binding = new Binding("location_id");

                columnThree.Header = "Map Floor";
                columnThree.Binding = new Binding("map_floor");

                columnFour.Header = "Reader Info";
                columnFour.Binding = new Binding("reader_info");

                columnFive.Visibility = Visibility.Hidden;
            }
            //var col = new DataGridTextColumn();
            //col.Header = "dnew";
            //dataGridBrowseDatabase.Columns.Add(col);
        }

        /// <summary>
        /// Populates the Datagrid according to the selection.
        /// </summary>
        /// <param name="selectedCategory">The selected category.</param>
        /// <param name="wSearchChar">The w search char.</param>
        private void populateGridDynamicColumns(int selectedCategory, string wSearchChar)
        {
            int matchesFound = 0;
            // Process the search term
            wSearchChar = "%" + wSearchChar.Trim() + "%";
            dataGridBrowseDatabase.Visibility = Visibility.Visible;
            textBlockSearchResult.Visibility = Visibility.Visible;

            //Check for search category
            if (selectedCategory == 0)  //Assets
            {
                //wSearchChar = wSearchChar.Replace('%', ' ').Trim();

                //ztATdbLocalDataSet1TableAdapters.assetInfoSearchDBDataTableTableAdapter assetInfoTableAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.assetInfoSearchDBDataTableTableAdapter();
                //ztATdbLocalDataSet1.assetInfoSearchDBDataTableDataTable tmpResTableA = new ztATdbLocalDataSet1.assetInfoSearchDBDataTableDataTable();
                //assetInfoTableAdpt.Fill(tmpResTableA, wSearchChar);

                ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter assetInfoTableAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter();
                ztATdbLocalDataSet1.AssetInfoDataTableDataTable tmpResTableA = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();
                try
                {
                    assetInfoTableAdpt.FillBy(tmpResTableA, wSearchChar);
                    dataGridBrowseDatabase.DataContext = tmpResTableA.DefaultView;
                
                

                    if (tmpResTableA.Count == 0)
                    {
                        dataGridBrowseDatabase.Visibility = Visibility.Hidden;
                        lblTagsNote.Visibility = Visibility.Hidden;
                        textBlockSearchResult.Background = Brushes.LightPink;
                        textBlockSearchResult.Text = "No result found !";
                    }
                    else if (tmpResTableA.Count > 0)
                    {
                        // Copy into grid function table
                        dataGridDBTable = tmpResTableA;

                        // create Serial No. column for the DataTable
                        tmpResTableA.Columns.Add("sr_no1", typeof(System.Int32));


                        for (int i = 0; i < tmpResTableA.Rows.Count; i++)
                        {
                            matchesFound = i + 1;
                            tmpResTableA.Rows[i]["sr_no1"] = i + 1;
                        }

                        //tmpResTableA.Rows[0]["sr_no1"] = srCount++;

                        //// specify it as auto increment field
                        //tmpResTableA.Columns["sr_no1"].AutoIncrement = true;
                        //tmpResTableA.Columns["sr_no1"].AutoIncrementSeed = 1;
                        //tmpResTableA.Columns["sr_no1"].AutoIncrementStep = 1;             
                        //tmpResTableA.Columns["sr_no1"].ReadOnly = true;                  

                        textBlockSearchResult.Background = Brushes.LightCyan;
                        textBlockSearchResult.Text = matchesFound.ToString() + " Result(s) found !";

                    }

                }

                catch
                {
                    textBlockSearchResult.Visibility = Visibility.Visible;
                    textBlockSearchResult.Background = Brushes.LightPink;
                    textBlockSearchResult.Text = "Failed to get result due to connection issues.";
                    LogFile.Log("Error: Failed to retrieve record from DB");
                    return;
                }

            }
            else if (selectedCategory == 1) //Users
            {
                ztATdbLocalDataSet1TableAdapters.UserIDDataTableTableAdapter userIDTableAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.UserIDDataTableTableAdapter();
                ztATdbLocalDataSet1.UserIDDataTableDataTable tmpResTableU = new ztATdbLocalDataSet1.UserIDDataTableDataTable();

                try
                {
                    userIDTableAdpt.FillSearchDB(tmpResTableU, wSearchChar);
                    dataGridBrowseDatabase.DataContext = tmpResTableU.DefaultView;
                

                    if (tmpResTableU.Count == 0)
                    {

                        textBlockSearchResult.Background = Brushes.LightPink;
                        textBlockSearchResult.Text = "No result found !";
                        dataGridBrowseDatabase.Visibility = Visibility.Hidden;
                        lblTagsNote.Visibility = Visibility.Hidden;
                    }
                    else if (tmpResTableU.Count > 0)
                    {
                        // Save data into grid function table
                        dataGridDBTable = tmpResTableU;

                        // create Serial No. column for the DataTable
                        tmpResTableU.Columns.Add("sr_no1", typeof(System.Int32));


                        for (int i = 0; i < tmpResTableU.Rows.Count; i++)
                        {
                            matchesFound = i + 1;
                            tmpResTableU.Rows[i]["sr_no1"] = i + 1;
                        }

                        textBlockSearchResult.Background = Brushes.LightCyan;
                        textBlockSearchResult.Text = matchesFound.ToString() + " Result(s) found !";

                    }
                }
                catch
                {
                    textBlockSearchResult.Visibility = Visibility.Visible;
                    textBlockSearchResult.Background = Brushes.LightPink;
                    textBlockSearchResult.Text = "Failed to get result due to connection issues.";
                    LogFile.Log("Error: Failed to retrieve record from DB");
                    return;
                }

            }
            else if (selectedCategory == 2) //Maps and Reader Info
            {
                ztATdbLocalDataSet1TableAdapters.readerInfoDataTableTableAdapter readerInfoTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.readerInfoDataTableTableAdapter();
                ztATdbLocalDataSet1.readerInfoDataTableDataTable tmpResTableR = new ztATdbLocalDataSet1.readerInfoDataTableDataTable();

                try
                {
                    readerInfoTA.Fill(tmpResTableR, wSearchChar);
                    dataGridBrowseDatabase.DataContext = tmpResTableR.DefaultView;

                    if (tmpResTableR.Count == 0)
                    {

                        textBlockSearchResult.Background = Brushes.LightPink;
                        textBlockSearchResult.Text = "No result found !";
                        dataGridBrowseDatabase.Visibility = Visibility.Hidden;
                        lblTagsNote.Visibility = Visibility.Hidden;
                    }
                    else if (tmpResTableR.Count > 0)
                    {
                        // Grid table saved
                        dataGridDBTable = tmpResTableR;

                        // create Serial No. column for the DataTable
                        tmpResTableR.Columns.Add("sr_no1", typeof(System.Int32));


                        for (int i = 0; i < tmpResTableR.Rows.Count; i++)
                        {
                            matchesFound = i + 1;
                            tmpResTableR.Rows[i]["sr_no1"] = i + 1;
                        }


                        textBlockSearchResult.Background = Brushes.LightCyan;
                        textBlockSearchResult.Text = matchesFound.ToString() + " Result(s) found !";

                    }


                }

                catch
                {
                    textBlockSearchResult.Visibility = Visibility.Visible;
                    textBlockSearchResult.Background = Brushes.LightPink;
                    textBlockSearchResult.Text = "Failed to get result due to connection issues.";
                    LogFile.Log("Error: Failed to retrieve record from DB");
                    return;
                }

               

            }



        }

        /// <summary>
        /// Handles the SelectionChanged event of the dataGridBrowseDatabase control.
        /// POpulates the fields according to the selection
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void dataGridBrowseDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Set UI elements
            lblTagsNote.Visibility = Visibility.Hidden;
            tbTextBoxSix.BorderBrush = Brushes.Transparent;

            if (cmbSearchCategory.SelectedIndex == 0) // Assets
            {
                lblLabelD1.Content = "Asset Description";
                lblLabelD2.Content = "Asset Comments";

                lblLabelOne.Content = "Asset ID";
                lblLabelTwo.Content = "Tag ID";
                lblLabelThree.Content = "Asset Location";
                lblLabelFour.Content = "Asset Category";
                lblLabelFive.Content = "Asset Model";
                lblLabelSix.Content = "Asset Status";
                lblLabelSeven.Content = "Asset Quantity";
                lblLabelEight.Content = "Asset Value";


                if (dataGridBrowseDatabase.SelectedItems.Count == 1)
                {
                    string mapAssetDesc = "";
                    string mapAssetID = "";
                    string mapAssetLoc = "";
                    showBrowseDBFields();
                    showAlertDBFields();
                    

                    DataRowView rowSelectedRecord = (DataRowView)dataGridBrowseDatabase.SelectedItems[0];
                    textBlockD1.Text = rowSelectedRecord["asset_desc"].ToString().Trim();
                    textBlockD2.Text = rowSelectedRecord["asset_comments"].ToString().Trim();

                    tbTextBoxOne.Text = rowSelectedRecord["asset_id"].ToString().Trim();
                    tbTextBoxTwo.Text = rowSelectedRecord["tag_id"].ToString().Trim();
                    tbTextBoxThree.Text = rowSelectedRecord["asset_location"].ToString().Trim();
                    tbTextBoxFour.Text = rowSelectedRecord["asset_category"].ToString().Trim();
                    tbTextBoxFive.Text = rowSelectedRecord["asset_model"].ToString().Trim();
                    tbTextBoxSix.Text = rowSelectedRecord["asset_status"].ToString().Trim();
                    tbTextBoxSeven.Text = rowSelectedRecord["asset_quantity"].ToString().Trim();
                    tbTextBoxEight.Text = rowSelectedRecord["asset_value"].ToString().Trim();

                    // UI changes to highlight fields
                    tbTextBoxSix.BorderBrush = Brushes.DarkCyan;
                    //tbTextBoxSix.BorderThickness = System.Windows.Thickness;

                    //Alerts
                   

                    lblAlertForAssetID1.Content = rowSelectedRecord["asset_id"].ToString().TrimEnd();
                    lblAlertTagExp1.Content = rowSelectedRecord["tag_expiry"].ToString().TrimEnd();
                    lblAlertAssetMaintDue1.Content = rowSelectedRecord["asset_maint_due"].ToString().TrimEnd();


                    //Maps
                    mapAssetID = rowSelectedRecord["asset_id"].ToString().TrimEnd();
                    mapAssetDesc = rowSelectedRecord["asset_desc"].ToString().Trim();
                    mapAssetLoc = rowSelectedRecord["asset_location"].ToString().Trim();
                    populateMap(mapAssetID, mapAssetDesc, mapAssetLoc);

                    if (rowSelectedRecord["asset_image"] != DBNull.Value)
                    {
                        //Image retreiving
                        byte[] imageBytes = (byte[])rowSelectedRecord["asset_image"];

                        if (!(imageBytes.Length == 0))
                        {
                            BitmapImage bitmapimage = new BitmapImage();
                            // Convert byte[] to Image   
                            bitmapimage.BeginInit();
                            bitmapimage.StreamSource = new MemoryStream(imageBytes);
                            bitmapimage.EndInit();
                            imageSearchDB.Source = (System.Windows.Media.ImageSource)bitmapimage;
                        }
                        else
                        {
                            imageSearchDB.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative));
                        }
                    }
                    else
                    {
                        imageSearchDB.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative));

                    }
                }
 
            }
            else if (cmbSearchCategory.SelectedIndex == 1) //Users
            {
                lblLabelD1.Content = "User Contact";
                lblLabelD2.Content = "Comments";

                lblLabelOne.Content = "User ID";
                lblLabelTwo.Content = "User name";
                lblLabelThree.Content = "User Dept ";
                lblLabelFour.Content = "Uer Role";
                lblLabelFive.Content = "Role ID";
                lblLabelSix.Content = "User Email";
                lblLabelSeven.Content = "User last login";
                lblLabelEight.Content = "User last update";


                if (dataGridBrowseDatabase.SelectedItems.Count == 1)
                {
                    showBrowseDBFields();
                    
                    
                    DataRowView rowSelectedRecord = (DataRowView)dataGridBrowseDatabase.SelectedItems[0];
                    textBlockD1.Text = rowSelectedRecord["user_contact"].ToString().Trim();
                    //textBlockD2.Text = rowSelectedRecord["comments"].ToString().Trim();

                    tbTextBoxOne.Text = rowSelectedRecord["user_id"].ToString().Trim();
                    tbTextBoxTwo.Text = rowSelectedRecord["user_name"].ToString().Trim();
                    tbTextBoxThree.Text = rowSelectedRecord["user_dept"].ToString().Trim();
                    tbTextBoxFour.Text = rowSelectedRecord["user_role"].ToString().Trim();
                    tbTextBoxFive.Text = rowSelectedRecord["user_roleid"].ToString().Trim();
                    tbTextBoxSix.Text = rowSelectedRecord["user_email"].ToString().Trim();
                    tbTextBoxSeven.Text = rowSelectedRecord["user_last_login"].ToString().Trim();
                    tbTextBoxEight.Text = rowSelectedRecord["user_last_update"].ToString().Trim();

                    lblAssetImg.Content = "User Profile";
                    imageSearchDB.Source = new BitmapImage(new Uri(@"./IconImages/userProfile.png", UriKind.Relative));
                    
                }

            }
            else if (cmbSearchCategory.SelectedIndex == 2) // Reader Info
            {
                lblLabelD1.Content = "Reader Info";
                lblLabelD2.Content = "Asset Info";

                lblLabelOne.Content = "Reader ID";
                lblLabelTwo.Content = "Location ID";
                lblLabelThree.Content = "Floor ID";
                lblLabelFour.Content = "Map Floor";
                lblLabelFive.Content = "Rooms per floor";
                lblLabelSix.Content = "Connection type";
                lblLabelSeven.Content = "Assets per floor";
                lblLabelEight.Content = "Map Comments";


                if (dataGridBrowseDatabase.SelectedItems.Count == 1)
                {
                    showBrowseDBFields();
                    
                    
                    DataRowView rowSelectedRecord = (DataRowView)dataGridBrowseDatabase.SelectedItems[0];
                    textBlockD1.Text = rowSelectedRecord["reader_info"].ToString().Trim();
                    textBlockD2.Text = rowSelectedRecord["asset_info"].ToString().Trim();

                    tbTextBoxOne.Text = rowSelectedRecord["reader_id"].ToString().Trim();
                    tbTextBoxTwo.Text = rowSelectedRecord["location_id"].ToString().Trim();
                    tbTextBoxThree.Text = rowSelectedRecord["floor_id"].ToString().Trim();
                    tbTextBoxFour.Text = rowSelectedRecord["map_floor"].ToString().Trim();
                    tbTextBoxFive.Text = rowSelectedRecord["rooms_pfloor"].ToString().Trim(); //Rooms per floor
                    tbTextBoxSix.Text = ""; // Reader connection
                    tbTextBoxSeven.Text = rowSelectedRecord["assets_pfloor"].ToString().Trim(); // Assets per floor
                    tbTextBoxEight.Text = rowSelectedRecord["map_comments"].ToString().Trim(); // Map comments

                    lblAssetImg.Content = "Reader Image";
                    imageSearchDB.Source = new BitmapImage(new Uri(@"./IconImages/RFreader.png", UriKind.Relative));

                }

            }
        }


        // Quick Browse Database Auto complete search and history
        // Browse DB autocomplete UI feature


        /// <summary>
        /// Handles the Click event of the btnSearchOne control.
        /// Main serach function to search DB
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnSearchOne_Click(object sender, RoutedEventArgs e)
        {
            listBoxSearchDBSuggestion.Visibility = Visibility.Collapsed;
            int maxNumberOfSearchWords = 25;
            //srCount = 1;
            string wChar = "";
            wChar = textBoxSearchDB.Text.ToString().Trim();

            if ( (string.IsNullOrEmpty(wChar)) || (wChar.CompareTo("<Enter the search term>") == 0))
            {
                dataGridBrowseDatabase.Visibility = Visibility.Hidden;
                lblTagsNote.Visibility = Visibility.Hidden;
                hideBrowseDBFields();
                hideAlertDBFields();
                
                textBlockSearchResult.Visibility = Visibility.Visible;
                textBlockSearchResult.Background = Brushes.LightPink;
                textBlockSearchResult.Text = "Enter a search term !";
                return;
            }
            else if (cmbSearchCategory.SelectedIndex == -1) // No Selection
            {
                lblTagsNote.Visibility = Visibility.Hidden;
                dataGridBrowseDatabase.Visibility = Visibility.Hidden;
                textBlockSearchResult.Visibility = Visibility.Visible;
                hideBrowseDBFields();
                hideAlertDBFields();
                
                textBlockSearchResult.Text = "Enter a search category !";
                return;
            }

            if (!(string.IsNullOrEmpty(wChar)))
            {
                //Filter out duplicate search terms
                if (!(searchListDB.Contains(wChar)))
                {
                    searchListDB.Add(wChar);

                }
                if (!(listBoxHistoryDB.Items.Contains(wChar)))
                {
                    listBoxHistoryDB.Items.Add(wChar);
                }


                if (listBoxHistoryDB.Items.Count > maxNumberOfSearchWords)
                {
                    listBoxHistoryDB.Items.RemoveAt(0); // Remove the oldest
                }

                if (searchListDB.Count > maxNumberOfSearchWords)
                {
                    searchListDB.RemoveAt(0);
                }
            }

            if (cmbSearchCategory.SelectedIndex == 0) // Assets
            {
                populateGridDynamicColumns(cmbSearchCategory.SelectedIndex, wChar);

            }
            else if (cmbSearchCategory.SelectedIndex == 1) //Users
            {
                populateGridDynamicColumns(cmbSearchCategory.SelectedIndex, wChar);

            }
            else if (cmbSearchCategory.SelectedIndex == 2) //Reader Info
            {
                populateGridDynamicColumns(cmbSearchCategory.SelectedIndex, wChar);

            }

        }


        /// <summary>
        /// Handles the SelectionChanged event of the listBoxSearchDBSuggestion control.
        /// Scrolling through the list copies the term in search box
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void listBoxSearchDBSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxSearchDBSuggestion.ItemsSource != null)
            {
                //listBoxSearchDBSuggestion.Visibility = Visibility.Collapsed;
                textBoxSearchDB.TextChanged -= new TextChangedEventHandler(textBoxSearchDB_TextChanged);

                if (listBoxSearchDBSuggestion.SelectedIndex != -1)
                {
                    textBoxSearchDB.Focus();
                    textBoxSearchDB.Text = listBoxSearchDBSuggestion.SelectedItem.ToString();
                    textBoxSearchDB.SelectAll();

                }
                textBoxSearchDB.TextChanged += new TextChangedEventHandler(textBoxSearchDB_TextChanged);
            }
        }

        /// <summary>
        /// Handles the GotMouseCapture event of the listBoxSearchDBSuggestion control.
        /// UI for search box
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void listBoxSearchDBSuggestion_GotMouseCapture(object sender, MouseEventArgs e)
        {
            textBoxSearchDB.Focus();
            textBoxSearchDB.SelectAll();
            listBoxSearchDBSuggestion.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the listBoxSearchDBSuggestion control.
        /// Select the suggested words during type of search term
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void listBoxSearchDBSuggestion_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                textBoxSearchDB.Focus();
                textBoxSearchDB.SelectAll();
                listBoxSearchDBSuggestion.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxSearchDB control.
        /// Pop up the search list during typing
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchDB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string typedString = textBoxSearchDB.Text;
            List<string> autoList = new List<string>();
            autoList.Clear();

            foreach (string item in searchListDB)
            {
                if (!string.IsNullOrEmpty(textBoxSearchDB.Text))
                {
                    if (item.StartsWith(typedString))
                    {
                        autoList.Add(item);
                    }
                }

            }

            if (autoList.Count > 0)
            {
                listBoxSearchDBSuggestion.ItemsSource = autoList;
                listBoxSearchDBSuggestion.Visibility = Visibility.Visible;

            }

            else if (textBoxSearchDB.Text.Equals(""))
            {
                listBoxSearchDBSuggestion.ItemsSource = null;
                listBoxSearchDBSuggestion.Visibility = Visibility.Collapsed;

            }
            //else
            //{
            //    listBoxSearchDBSuggestion.ItemsSource = null;
            //    listBoxSearchDBSuggestion.Visibility = Visibility.Collapsed;
            //}

        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the textBoxSearchDB control.
        /// UI feature for the user
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchDB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                listBoxSearchDBSuggestion.Focus();
                //textBoxSearchID.Text = listBoxSuggestion.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Handles the Selected event of the treeViewHistoryDB control.
        /// UI feature to handle history list
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewHistoryDB_Selected(object sender, RoutedEventArgs e)
        {
            if (listBoxHistoryDB.Items.Count > 0)
            {
                listBoxHistoryDB.Visibility = Visibility.Visible;
            }
            else
            {
                listBoxHistoryDB.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Handles the Selected event of the treeViewClearHistoryDB control.
        /// Clears the search history
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewClearHistoryDB_Selected(object sender, RoutedEventArgs e)
        {
            listBoxHistoryDB.Items.Clear();
            listBoxHistoryDB.Visibility = Visibility.Hidden;
            //listBoxHistoryDB.Items.Add("<no history>");
        }

        /// <summary>
        /// Handles the SelectionChanged event of the listBoxHistoryDB control.
        /// Copy the search item to serach textbox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void listBoxHistoryDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxHistoryDB.SelectedItem != null)
            {
                textBoxSearchDB.Text = listBoxHistoryDB.SelectedItem.ToString();
                listBoxSearchDBSuggestion.Visibility = Visibility.Collapsed;
            }
        }


        // Browse DB DataGrid Filter functions
         //  Grid functions to filter,clear and refresh grid data


        /// <summary>
        /// Handles the Selected event of the treeViewItemDBClearDisplay control.
        /// Clear display -datagrid
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewItemDBClearDisplay_Selected(object sender, RoutedEventArgs e)
        {
            // Set UI elements
            lblTagsNote.Visibility = Visibility.Hidden;
            dataGridBrowseDatabase.DataContext = null;
            
        }

        /// <summary>
        /// Handles the Selected event of the treeViewItemDBRefreshDisplay control.
        /// Refresh the datagrid display
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewItemDBRefreshDisplay_Selected(object sender, RoutedEventArgs e)
        {
            int rcount;

            rcount = dataGridDBTable.Rows.Count;
            dataGridBrowseDatabase.DataContext = dataGridDBTable.DefaultView;

        }

        /// <summary>
        /// Handles the Selected event of the treeViewItemDBRemoveAndClear control.
        /// Remove and clear the datagrid source
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewItemDBRemoveAndClear_Selected(object sender, RoutedEventArgs e)
        {
            lblTagsNote.Visibility = Visibility.Hidden;
            textBlockSearchResult.Visibility = Visibility.Hidden;
            textBoxSearchDB.Text = "";
            dataGridDBTable.Clear();
            dataGridBrowseDatabase.DataContext = dataGridDBTable.DefaultView;

        }

        /// <summary>
        /// Handles the Selected event of the treeViewDBFilterFixed control.
        /// Data grid Filter category: Fixed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewDBFilterFixed_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_category]", "Fixed");
            }
            else if (cmbSearchCategory.SelectedIndex == 1)
            {
                quickBrowseDBFilter("[user_role]", "ADMIN");
            }
            else if (cmbSearchCategory.SelectedIndex == 2)
            {
                quickBrowseDBFilter("[map_floor]", "First");
            }
        }

        /// <summary>
        /// Handles the Selected event of the treeViewDBFilterPortable control.
        /// Data grid Filter category: Portable
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewDBFilterPortable_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_category]", "Portable");

            }
            else if (cmbSearchCategory.SelectedIndex == 1)
            {
                quickBrowseDBFilter("[user_role]", "ADMIN2");
            }
            else if (cmbSearchCategory.SelectedIndex == 2)
            {
                quickBrowseDBFilter("[map_floor]", "Second");
            }
        }

        /// <summary>
        /// Handles the Selected event of the treeViewDBFilterMovable control.
        /// Data grid Filter category: Movable
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void treeViewDBFilterMovable_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_category]", "Movable");
            }
            else if (cmbSearchCategory.SelectedIndex == 1)
            {
                quickBrowseDBFilter("[user_role]", "Restricted");
            }
            else if (cmbSearchCategory.SelectedIndex == 2)
            {
                quickBrowseDBFilter("[map_floor]", "Third");
            }
        }

        /// <summary>
        /// Handles the Selected event of the tviDBASok control.
        /// Data grid Filter category: Asset Status- OK
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviDBASok_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_status]", "OK");
            }
        }

        /// <summary>
        /// Handles the Selected event of the tviDBASnew control.
        /// Data grid Filter category: Asset Status- NEW
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviDBASnew_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_status]", "NEW");
            }
        }

        /// <summary>
        /// Handles the Selected event of the tviDBASrepaired control.
        /// Data grid Filter category: Asset Status- REPAIR
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviDBASrepaired_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_status]", "REPAIR");
            }
        }

        /// <summary>
        /// Handles the Selected event of the tviDBAStransit control.
        /// Data grid Filter category: Asset Status- TRANSIT
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviDBAStransit_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_status]", "TRANSIT");
            }
        }

        /// <summary>
        /// Handles the Selected event of the tviDBASserviced control.
        /// Data grid Filter category: Asset Status- SERVICED
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviDBASserviced_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_status]", "SERVICED");
            }
        }

        /// <summary>
        /// Handles the Selected event of the tviDBASfault control.
        /// Data grid Filter category: Asset Status- FAULT
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviDBASfault_Selected(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex == 0)
            {
                quickBrowseDBFilter("[asset_status]", "FAULT");
            }
        }

        /// <summary>
        /// Quick browse DB filter.
        /// Filters the data according to the selection
        /// </summary>
        /// <param name="mainFilter">The main filter.</param>
        /// <param name="subFilter">The sub filter.</param>
        private void quickBrowseDBFilter(string mainFilter, string subFilter)
        {
            if (dataGridDBTable.Rows.Count > 0)
            {

                DataView dv = new DataView(dataGridDBTable);

                //dv.RowFilter = "[asset_category] = '" + "Fixed" + "'";
                dv.RowFilter = mainFilter + "= '" + subFilter + "'";

                if (dv.Count > 0)
                {
                    lblTagsNote.Visibility = Visibility.Visible;

                    lblTagsNote.Content = dv.Count.ToString() + " records found!";
                }
                else
                {
                    lblTagsNote.Visibility = Visibility.Visible;

                    lblTagsNote.Content = "No records found for this filter!";

                }
                dataGridBrowseDatabase.DataContext = dv;
            }
            else
            {
                lblTagsNote.Visibility = Visibility.Visible;
                lblTagsNote.Content = "No results found!";
            }
        }

        /**********************************************************************/

        // Report menu options
        // Alert and notify settings, reports, charts and report option features

        /// <summary>
        /// Handles the Click event of the menuCharts control.
        /// Charts window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        /// End of GRid functions
        private void menuCharts_Click(object sender, RoutedEventArgs e)
        {
            ChartWindow cwin = new ChartWindow();
            cwin.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuReportsAutoGen control.
        /// Report generation window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuReportsAutoGen_Click(object sender, RoutedEventArgs e)
        {
            ReportsAutoGen rag = new ReportsAutoGen();
            rag.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuReportsNotificationAssets control.
        /// Alert notifications window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuReportsNotificationAssets_Click(object sender, RoutedEventArgs e)
        {
            AlertNotificationSettings alertnotify = new AlertNotificationSettings();
            alertnotify.Show();
        }

        /// <summary>
        /// Handles the Click event of the menuReportsOpt control.
        /// The report options window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void menuReportsOpt_Click(object sender, RoutedEventArgs e)
        {
            reportOptions rptOne = new reportOptions();
            rptOne.Show();
            
        }

        /*************************************************************************/

        //Dashboard and Reports TAB loading
        //Function to load the dashboard and critical asset parameters

        /// <summary>
        /// Handles the Loaded event of the tabItemDashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tabItemDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            cmbEventsTime.SelectedIndex = 0;
        }

        /// <summary>
        /// Loads the dashboard reports.
        /// </summary>
        private void loadDashboardReports()
        {
            lblSummaryDate.Content = DateTime.Now.ToLongDateString();
            loadCharts();
            //loadDataGridCriticalEvents(); //Function called on selection changed
            loadKeyMetrics();

        }

        /// <summary>
        /// Loads the charts for the dashboard
        /// </summary>
        private void loadCharts()
        {
            ztATdbLocalDataSet1TableAdapters.ChartingQueryTableAdapter chartingTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.ChartingQueryTableAdapter();
            ztATdbLocalDataSet1.ChartingQueryDataTable chartingDataTable = new ztATdbLocalDataSet1.ChartingQueryDataTable();

            int totalPortableAssets = 0;
            int totalMovableAssets = 0;

            int rowCount;
            chartingDataTable.Clear();
            //pChart.Visibility = Visibility.Visible;
            //nEw keyvalue pair
            Dictionary<string, int> dataPieChart = new Dictionary<string, int>();
            Dictionary<string, int> dataPieChart2 = new Dictionary<string, int>();

            //PieSeries pi = new PieSeries();
            //reportChart.Series.Add(pi);

            try
            {
                rowCount = chartingTA.ChartingACategory(chartingDataTable);
                for (int i = 0; i < chartingDataTable.Rows.Count; i++)
                {

                    dataPieChart.Add(chartingDataTable.Rows[i]["aType"].ToString().TrimEnd(),
                    int.Parse(chartingDataTable.Rows[i]["aCount"].ToString().TrimEnd()));

                    if (chartingDataTable.Rows[i]["aType"].ToString().TrimEnd().CompareTo("Movable") == 0)
                    {
                        //Store number of movable assets
                        totalMovableAssets = int.Parse(chartingDataTable.Rows[i]["aCount"].ToString().TrimEnd());
                    }
                    else if (chartingDataTable.Rows[i]["aType"].ToString().TrimEnd().CompareTo("Portable") == 0)
                    {
                        //Store number of movable assets
                        totalPortableAssets = int.Parse(chartingDataTable.Rows[i]["aCount"].ToString().TrimEnd());
                    }

                }

                // *change here*
                totalMovingAssets = totalMovableAssets + totalPortableAssets;

                dBoardChart1.Title = "Asset Category";
                //dBoardChart1.LegendStyle
                dBoardChart1.DataContext = dataPieChart;
                //dataPieChart.Clear();
                chartingDataTable.Clear();


                rowCount = chartingTA.ChartingAStatus(chartingDataTable);
                for (int i = 0; i < chartingDataTable.Rows.Count; i++)
                {

                    dataPieChart2.Add(chartingDataTable.Rows[i]["aType"].ToString().TrimEnd(),
                    int.Parse(chartingDataTable.Rows[i]["aCount"].ToString().TrimEnd()));

                }

                dBoardChart2.Title = "Asset Status";
                dBoardChart2.DataContext = dataPieChart2;



            }
            catch
            {
                //Issues with datatable
            }
        }

        /// <summary>
        /// Loads the data grid critical events.
        /// </summary>
        private void loadDataGridCriticalEvents()
        {
            //Set the the date range
            DateTime texpiryLdate,texpiryHdate,mainLdate,maintHdate;
            texpiryLdate = texpiryHdate = mainLdate = maintHdate = default(DateTime);

            if(cmbEventsTime.SelectedIndex == 0) //7 days
            {
                texpiryLdate = DateTime.Now.AddDays(-7);
                texpiryHdate = DateTime.Now.AddDays(7);
                mainLdate = DateTime.Now.AddDays(-7);
                maintHdate = DateTime.Now.AddDays(7);

            }
            else if(cmbEventsTime.SelectedIndex == 1) // 2 weeks
            {
                texpiryLdate = DateTime.Now.AddDays(-14);
                texpiryHdate = DateTime.Now.AddDays(14);
                mainLdate = DateTime.Now.AddDays(-14);
                maintHdate = DateTime.Now.AddDays(14);

            }

            else if(cmbEventsTime.SelectedIndex == 2) //1 month
            {
                texpiryLdate = DateTime.Now.AddMonths(-1);
                texpiryHdate = DateTime.Now.AddMonths(1);
                mainLdate = DateTime.Now.AddMonths(-1);
                maintHdate = DateTime.Now.AddMonths(1);

            }

            else if (cmbEventsTime.SelectedIndex == 3) // 3 months
            {
                texpiryLdate = DateTime.Now.AddMonths(-3);
                texpiryHdate = DateTime.Now.AddMonths(3);
                mainLdate = DateTime.Now.AddMonths(-3);
                maintHdate = DateTime.Now.AddMonths(3);

            }

            else
            {
                ///selected index = -1  code should not reach here
            }

            ztATdbLocalDSReports1 ds = new ztATdbLocalDSReports1();
            ztATdbLocalDSReports1TableAdapters.asset_mainDTTableAdapter alertRecordTA = new ZTraka_App.ztATdbLocalDSReports1TableAdapters.asset_mainDTTableAdapter();
            ztATdbLocalDSReports1.asset_mainDTDataTable alertRecordDataTable = new ztATdbLocalDSReports1.asset_mainDTDataTable();

            this.Cursor = Cursors.Wait;

            try
            {
                alertRecordTA.FillAlertDateRecords(alertRecordDataTable, mainLdate, maintHdate, texpiryLdate, texpiryHdate);


                if (alertRecordDataTable.Count == 0)
                {
                    //No rows found
                    dataGridDashboard.Visibility = Visibility.Hidden;
                    hideDashboardAlerts();

                }
                else if (alertRecordDataTable.Count > 0)
                {
                    //There are rows available to be processed.
                    dataGridDashboard.Visibility = Visibility.Visible;
                    // create Serial No. column for the DataTable
                    alertRecordDataTable.Columns.Add("sr_no2", typeof(System.Int32));


                    for (int i = 0; i < alertRecordDataTable.Rows.Count; i++)
                    {

                        alertRecordDataTable.Rows[i]["sr_no2"] = i + 1;
                    }

                    DataTable dtCloned = alertRecordDataTable.Clone();
                    dtCloned.Columns["tag_expiry"].DataType = typeof(string);
                    dtCloned.Columns["asset_maint_due"].DataType = typeof(string);
                    foreach (DataRow row in alertRecordDataTable.Rows)
                    {
                        dtCloned.ImportRow(row);
                    }

                    //DataSetDateTime dt = new DataSetDateTime();
                    //alertRecordDataTable.Columns["tag_expiry"].
                    HATrakaMain.emailContent = "";
                    string assetIDCollection = "";
                    foreach (DataRow row in dtCloned.Rows)
                    {
                        DateTime dt = DateTime.Parse(row["tag_expiry"].ToString());
                        row["tag_expiry"] = dt.ToShortDateString();
                        dt = DateTime.Parse(row["asset_maint_due"].ToString());
                        row["asset_maint_due"] = dt.ToShortDateString();
                        assetIDCollection += row["asset_id"].ToString() + ";";

                        //Logging
                        HATrakaMain.emailContent += "Asset ID- " + row["asset_id"].ToString() + "::" + "Tag Expiry-" + row["tag_expiry"] + "::" + "Asset Maint Due-" + row["asset_maint_due"] + "\r\n";                   

                    }

                    // Log ,email, SMS
                    if (AlertNotificationSettings.logsCAP == true)
                    {
                        LogFile.Log(HATrakaMain.emailContent);
                    }
                    
                    if ( (AlertNotificationSettings.capAssetTagExpiry == true) || (AlertNotificationSettings.capAssetMaintenanceDue == true))
                    {
                        if (AlertNotificationSettings.sendEmailUser == true)
                        {
                            
                            sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                        }
                        if (AlertNotificationSettings.sendSMSUser == true)
                        {
                            SMSClass.sendSMS("Alerts:Tag Expiry,Asset Maintenance Due for AssetIDs:: " + assetIDCollection, LoginWindow.user_contact);

                            if (AlertNotificationSettings.sendSMSAdmin == true)
                            {
                                SMSClass.sendSMS("Alerts:Tag Expiry,Asset Maintenance Due for AssetIDs:: " + assetIDCollection, HATrakaMain.superAdminSMS);
                            }
                        }

                    }


                    //Finally set the data context
                    dataGridDashboard.DataContext = dtCloned.DefaultView;

                }

            }

            catch
            {
                LogFile.Log("Error: Problem retrieving alert date records for assets");
            }

            finally
            {
                this.Cursor = Cursors.Arrow;
            }

        }

        /// <summary>
        /// Handles the SelectionChanged event of the dataGridDashboard control.
        /// Populates the alerts based on selection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void dataGridDashboard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            string assetID, tagExpire, assetMaintdue, fp;
            assetID = tagExpire = assetMaintdue = fp = string.Empty;

            DateTime teDate, amdDate;
            teDate = amdDate = default(DateTime);

            hideDashboardAlerts();

            if (dataGridDashboard.SelectedItems.Count == 1)
            {
                lblAlertForAssetID.Visibility = Visibility.Visible;
                lblalertaid.Visibility = Visibility.Visible;

                borderAlerts.Visibility = Visibility.Visible;

                DataRowView rowSelectedRecord = (DataRowView)dataGridDashboard.SelectedItems[0];
                assetID = rowSelectedRecord["asset_id"].ToString().Trim();
                tagExpire = rowSelectedRecord["tag_expiry"].ToString().Trim();
                assetMaintdue = rowSelectedRecord["asset_maint_due"].ToString().Trim();

                //Display Asset ID
                lblAlertForAssetID.Content = assetID;

                // Process day calculation

                amdDate = DateTime.Parse(assetMaintdue);
                teDate = DateTime.Parse(tagExpire);

                processDayRange(teDate, amdDate);
             
            }


                
        }

        /// <summary>
        /// Processes the day ranges for crtical parameters: Tag Expiry and
        /// Asset maintenance due
        /// </summary>
        /// <param name="teDate">The te date.</param>
        /// <param name="amdDate">The amd date.</param>
        private void processDayRange(DateTime teDate, DateTime amdDate)
        {
            int tdays;
            DateTime rangeL, rangeH, dtToday;
            rangeL = rangeH = dtToday = default(DateTime);
            dtToday = DateTime.Today;

            //Check selection for time period
            if (cmbEventsTime.SelectedIndex == 0) //7 days
            {
                rangeL = DateTime.Now.AddDays(-7);
                rangeH = DateTime.Now.AddDays(7);
 
            }
            else if (cmbEventsTime.SelectedIndex == 1) //2 weeks
            {
                rangeL = DateTime.Now.AddDays(-14);
                rangeH = DateTime.Now.AddDays(14);
 
            }

            else if (cmbEventsTime.SelectedIndex == 2) //1 month
            {
                rangeL = DateTime.Now.AddMonths(-1);
                rangeH = DateTime.Now.AddMonths(1);
 
            }
            
            else if (cmbEventsTime.SelectedIndex == 3) //3 months
            {
                rangeL = DateTime.Now.AddMonths(-3);
                rangeH = DateTime.Now.AddMonths(3);
 
            }
        
            //Process tag expiry

            if ((DateTime.Compare(teDate, rangeH) <= 0) && (DateTime.Compare(teDate, rangeL) >= 0))
            {
                lblAlertTagExp.Visibility = Visibility.Visible;
                lblAlertte.Visibility = Visibility.Visible;
                tdays = Math.Abs((int)Math.Ceiling((teDate - dtToday).TotalDays));

                    if (DateTime.Compare(teDate, dtToday) < 0) //already past the due date or today
                    {
                        
                        lblAlertTagExp.Content = "already past " + tdays.ToString() + " day(s)";

                    }

                    else if (DateTime.Compare(teDate, dtToday) == 0) // //due today 
                    {
                        
                        lblAlertTagExp.Content = "due TODAY ! ";


                    }

                    else  // //due in x days 
                    {
                       
                        lblAlertTagExp.Content = "due in " + tdays.ToString() + " day(s)";

                    }                
            }

            //Process maintenance due date

            if ((DateTime.Compare(amdDate, rangeH) <= 0) && (DateTime.Compare(amdDate, rangeL) >= 0))
            {
                lblAlertAssetMaintDue.Visibility = Visibility.Visible;
                lblalertamd.Visibility = Visibility.Visible;
                tdays = Math.Abs((int)Math.Ceiling((amdDate - dtToday).TotalDays));

                if (DateTime.Compare(amdDate, dtToday) < 0) //already past the due date or today
                {
                    
                    lblAlertAssetMaintDue.Content = "already past " + tdays.ToString() + " day(s)";

                }

                else if (DateTime.Compare(amdDate, dtToday) == 0) // //due today 
                {
                    
                    lblAlertAssetMaintDue.Content = "due TODAY ! ";


                }

                else  // //due in x days 
                {
                    
                    lblAlertAssetMaintDue.Content = "due in " + tdays.ToString() + " day(s)";

                }

            }

        }

        /// <summary>
        /// Loads the key metrics.
        /// </summary>
        private void loadKeyMetrics()
        {
            //lblKeyMetricsValue2
            //lblKeyMetricsValue1
            //lblKeyMetricsValue3
            int totalRooms, totalAssetCapacity, totalAssetValue, totalAssetCount;
            float movingAssetValuePercent;
            float averageAssetsPerRoom, averageAssetMovement;
            int assetMovements = 0;
            int topN = 5; //Top 5 frequently moved assets

            int portableAssetValueRs = 0;
            int movableAssetValueRs = 0;
            int totalMovingAssetValueRs = 0;
            
            //Sort db in descending order
            var tmpOD = new System.Collections.Specialized.OrderedDictionary();
            tmpOD = Properties.Settings.Default.assetMovementDB;

            if (tmpOD.Count > 0)
            {
                var tmpDictionary = tmpOD.Cast<DictionaryEntry>().OrderByDescending(r => r.Value).ToDictionary(c => c.Key, d => d.Value);
                ztATdbLocalDataSet1.AssetInfoDataTableDataTable tmpAssetDT = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();

                int k = 1;
                foreach (var item in tmpDictionary)
                {
                    try
                    {
                        AssetInfoTA.Fill(tmpAssetDT, item.Key.ToString());
                        if (tmpAssetDT.Count > 0)
                        {
                            if (!(listBoxFrequentlyMovedAssets.Items.Contains(tmpAssetDT.Rows[0]["asset_id"].ToString() + " Freq: " + item.Value.ToString())))
                            {
                                listBoxFrequentlyMovedAssets.Items.Add(tmpAssetDT.Rows[0]["asset_id"].ToString() + " Freq: " + item.Value.ToString());
                            }
                        }
                    }
                    catch
                    {
                        LogFile.Log("Error: Failed to get data from DB for key metrics in dashboard");
                        //Problem retriving data
                    }
                    //listBoxFrequentlyMovedAssets.Items.Add(item.Key.ToString());
                    ++k;
                    if (k > topN)
                    {
                        break;
                    }
                }

                assetMovements = tmpDictionary.Sum(x => (int)x.Value);
                lblTotalAssetMovementVal.Content = assetMovements.ToString();
                averageAssetMovement = ((float)assetMovements / (float)totalMovingAssets);
                lblAvgMovementValue.Content = averageAssetMovement.ToString();
            }
            else
            {
                
                lblTotalAssetMovementVal.Content = "NA";
                lblAvgMovementValue.Content = "NA";
                listBoxFrequentlyMovedAssets.Items.Clear();
            }
            
            //int sumCount = 0;
            //for (int j = 0; j < Properties.Settings.Default.assetMovementDB.Count; j++)
            //{
            //    sumCount += (int)Properties.Settings.Default.assetMovementDB[j];
            //}
            //// Sum of assetMovementDB values;
            if (!(Properties.Settings.Default.scannedAssetUdb == null))
            {
                lblKeyMetricsTotalUniqueSA.Content = Properties.Settings.Default.scannedAssetUdb.Count.ToString();
            }

            ztATdbLocalDataSet1TableAdapters.ChartingQueryTableAdapter chartingTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.ChartingQueryTableAdapter();
            ztATdbLocalDataSet1.ChartingQueryDataTable chartingDataTable = new ztATdbLocalDataSet1.ChartingQueryDataTable();
            
            try
            {
                // Load the data and save it.
                chartingDataTable.Clear();
                chartingTA.FillRAPF(chartingDataTable);
                totalRooms = int.Parse(chartingDataTable.Rows[0]["aType"].ToString().TrimEnd()); //Total Rooms
                totalAssetCapacity = int.Parse(chartingDataTable.Rows[0]["aCount"].ToString().TrimEnd()); //Total Asset Capacity

                chartingDataTable.Clear();
                chartingTA.FillAVACount(chartingDataTable);
                totalAssetValue = (int)(Convert.ToDecimal( chartingDataTable.Rows[0]["aType"].ToString().TrimEnd())); //Total Asset Value
                totalAssetCount = int.Parse(chartingDataTable.Rows[0]["aCount"].ToString().TrimEnd()); //Total (actual) Asset Count

                chartingDataTable.Clear();
                chartingTA.ChartingATotalValue(chartingDataTable);       
                for(int i = 0; i < chartingDataTable.Count; i++)
                {
                    if(chartingDataTable.Rows[i]["aType"].ToString().TrimEnd().CompareTo("Portable") == 0)
                    {
                        portableAssetValueRs = int.Parse(chartingDataTable.Rows[i]["aCount"].ToString().TrimEnd());
                    }
                    else if (chartingDataTable.Rows[i]["aType"].ToString().TrimEnd().CompareTo("Movable") == 0)
                    {
                        movableAssetValueRs = int.Parse(chartingDataTable.Rows[i]["aCount"].ToString().TrimEnd());
                    }
                }
                totalMovingAssetValueRs = portableAssetValueRs + movableAssetValueRs;
                
                
                // Now process and display

                lblKeyMetricsValue2.Content = totalAssetValue.ToString();

                movingAssetValuePercent = ((float)totalMovingAssetValueRs / (float)totalAssetValue) * 100;

                lblKeyMetricsValue3.Content = Math.Round(movingAssetValuePercent).ToString() + " %"; //Percentage
                progressBarUtilization.Value = Math.Round(movingAssetValuePercent);

                averageAssetsPerRoom = (totalAssetCount / totalRooms);
            }
            catch
            {
                LogFile.Log("Error: Failed to get data from DB for key metrics in dashboard");
                //Some problem with connecting to DB
            }


        }

        /// <summary>
        /// Handles the Click event of the hplResetMetrics control.
        /// Resets the key metrics to initial state
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void hplResetMetrics_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgResult = MessageBox.Show("Are you sure you want to reset the metrics", "Metrics Reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgResult == MessageBoxResult.Yes)
            {
                lblKeyMetricsTotalUniqueSA.Content = "0";
                Properties.Settings.Default.scannedAssetUdb.Clear();
                Properties.Settings.Default.assetMovementDB.Clear();

                lblTotalAssetMovementVal.Content = "NA";
                lblAvgMovementValue.Content = "NA";
                listBoxFrequentlyMovedAssets.Items.Clear();
            }
            else
            {
                //Cancel operation
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbKeyMetrics control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void cmbKeyMetrics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadKeyMetrics();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbEventsTime control.
        /// Loads the critical events once selection changes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void cmbEventsTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            loadDataGridCriticalEvents();

        }

        /*************************************************************************/

        // UI hide/show fields functions
        // UI feat

        /// <summary>
        /// Hides the Datagrid scanned asset fields.
        /// </summary>
        private void hideDGscannedAssetFields()
        {
            lblAssetIDdesc.Visibility = Visibility.Hidden;
            lblAssetID.Visibility = Visibility.Hidden;
            lblTagIDdesc.Visibility = Visibility.Hidden;
            lblTagID.Visibility = Visibility.Hidden;
            lblAssetLocdesc.Visibility = Visibility.Hidden;
            lblAssetLocation.Visibility = Visibility.Hidden;
            lblAssetCatdesc.Visibility = Visibility.Hidden;
            lblAssetCategory.Visibility = Visibility.Hidden;
            lblAssetModelDesc.Visibility = Visibility.Hidden;
            lblAssetModel.Visibility = Visibility.Hidden;
            lblAssetdescdesc.Visibility = Visibility.Hidden;
            textBlockAssetDesc.Visibility = Visibility.Hidden;
            lblAssetStatusDesc.Visibility = Visibility.Hidden;
            lblAssetStatusMain.Visibility = Visibility.Hidden;
            lblAssetQuantdesc.Visibility = Visibility.Hidden;
            lblAssetQuantity.Visibility = Visibility.Hidden;
            lblAssetValDesc.Visibility = Visibility.Hidden;
            lblAssetValue.Visibility = Visibility.Hidden;
            lblComments.Visibility = Visibility.Hidden;
            textBlockAssetComments.Visibility = Visibility.Hidden;
            lblAssetImage.Visibility = Visibility.Hidden;
            stackPanelImage.Visibility = Visibility.Hidden;
            imageAssetImage.Visibility = Visibility.Hidden;
 
        }

        /// <summary>
        /// Shows the Data grid scanned asset fields.
        /// </summary>
        private void showDGscannedAssetFields()
        {
            lblAssetIDdesc.Visibility = Visibility.Visible;
            lblAssetID.Visibility = Visibility.Visible;
            lblTagIDdesc.Visibility = Visibility.Visible;
            lblTagID.Visibility = Visibility.Visible;
            lblAssetLocdesc.Visibility = Visibility.Visible;
            lblAssetLocation.Visibility = Visibility.Visible;
            lblAssetCatdesc.Visibility = Visibility.Visible;
            lblAssetCategory.Visibility = Visibility.Visible;
            lblAssetModelDesc.Visibility = Visibility.Visible;
            lblAssetModel.Visibility = Visibility.Visible;
            lblAssetdescdesc.Visibility = Visibility.Visible;
            textBlockAssetDesc.Visibility = Visibility.Visible;
            lblAssetStatusDesc.Visibility = Visibility.Visible;
            lblAssetStatusMain.Visibility = Visibility.Visible;
            lblAssetQuantdesc.Visibility = Visibility.Visible;
            lblAssetQuantity.Visibility = Visibility.Visible;
            lblAssetValDesc.Visibility = Visibility.Visible;
            lblAssetValue.Visibility = Visibility.Visible;
            lblComments.Visibility = Visibility.Visible;
            textBlockAssetComments.Visibility = Visibility.Visible;
            lblAssetImage.Visibility = Visibility.Visible;
            stackPanelImage.Visibility = Visibility.Visible;
            imageAssetImage.Visibility = Visibility.Visible;
 
        }

        /// <summary>
        /// Hides the browse DB fields.
        /// </summary>
        private void hideBrowseDBFields()
        {
            lblLabelOne.Visibility = Visibility.Hidden;
            tbTextBoxOne.Visibility = Visibility.Hidden;
            lblLabelTwo.Visibility = Visibility.Hidden;
            tbTextBoxTwo.Visibility = Visibility.Hidden;
            lblLabelThree.Visibility = Visibility.Hidden;
            tbTextBoxThree.Visibility = Visibility.Hidden;
            lblLabelFour.Visibility = Visibility.Hidden;
            tbTextBoxFour.Visibility = Visibility.Hidden;
            lblLabelFive.Visibility = Visibility.Hidden;
            tbTextBoxFive.Visibility = Visibility.Hidden;
            lblLabelD1.Visibility = Visibility.Hidden;
            textBlockD1.Visibility = Visibility.Hidden;
            lblLabelSix.Visibility = Visibility.Hidden;
            tbTextBoxSix.Visibility = Visibility.Hidden;
            lblLabelSeven.Visibility = Visibility.Hidden;
            tbTextBoxSeven.Visibility = Visibility.Hidden;
            lblLabelEight.Visibility = Visibility.Hidden;
            tbTextBoxEight.Visibility = Visibility.Hidden;
            lblLabelD2.Visibility = Visibility.Hidden;
            textBlockD2.Visibility = Visibility.Hidden;
            lblAssetImg.Visibility = Visibility.Hidden;
            imageSearchDB.Visibility = Visibility.Hidden;
            borderAssetPic.Visibility = Visibility.Hidden;
 
        }

        /// <summary>
        /// Shows the browse DB fields.
        /// </summary>
        private void showBrowseDBFields()
        {
            lblLabelOne.Visibility = Visibility.Visible;
            tbTextBoxOne.Visibility = Visibility.Visible;
            lblLabelTwo.Visibility = Visibility.Visible;
            tbTextBoxTwo.Visibility = Visibility.Visible;
            lblLabelThree.Visibility = Visibility.Visible;
            tbTextBoxThree.Visibility = Visibility.Visible;
            lblLabelFour.Visibility = Visibility.Visible;
            tbTextBoxFour.Visibility = Visibility.Visible;
            lblLabelFive.Visibility = Visibility.Visible;
            tbTextBoxFive.Visibility = Visibility.Visible;
            lblLabelD1.Visibility = Visibility.Visible;
            textBlockD1.Visibility = Visibility.Visible;
            lblLabelSix.Visibility = Visibility.Visible;
            tbTextBoxSix.Visibility = Visibility.Visible;
            lblLabelSeven.Visibility = Visibility.Visible;
            tbTextBoxSeven.Visibility = Visibility.Visible;
            lblLabelEight.Visibility = Visibility.Visible;
            tbTextBoxEight.Visibility = Visibility.Visible;
            lblLabelD2.Visibility = Visibility.Visible;
            textBlockD2.Visibility = Visibility.Visible;
            lblAssetImg.Visibility = Visibility.Visible;
            imageSearchDB.Visibility = Visibility.Visible;
            borderAssetPic.Visibility = Visibility.Visible;
 
        }

        /// <summary>
        /// Shows the alert DB fields.
        /// </summary>
        private void showAlertDBFields()
        {
            borderAlertOne.Visibility = Visibility.Visible;
            lblAlertForAssetID1.Visibility = Visibility.Visible;
            lblalertid1.Visibility = Visibility.Visible;
            lblAlertTagExp1.Visibility = Visibility.Visible;
            lblAlertte1.Visibility = Visibility.Visible;
            lblAlertAssetMaintDue1.Visibility = Visibility.Visible;
            lblalertamd1.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the alert DB fields.
        /// </summary>
        private void hideAlertDBFields()
        {
            borderAlertOne.Visibility = Visibility.Hidden;
            lblAlertForAssetID1.Visibility = Visibility.Hidden;
            lblalertid1.Visibility = Visibility.Hidden;
            lblAlertTagExp1.Visibility = Visibility.Hidden;
            lblAlertte1.Visibility = Visibility.Hidden;
            lblAlertAssetMaintDue1.Visibility = Visibility.Hidden;
            lblalertamd1.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows the scanned alert fields.
        /// </summary>
        private void showScannedAlertFields()
        {
            borderAlertSA.Visibility = Visibility.Visible;
            lblAssetIDAlert.Visibility = Visibility.Visible;
            lblAssetIDAlertdesc.Visibility = Visibility.Visible;
            lblTagExpiryAlert.Visibility = Visibility.Visible;
            lblTagExpiryAlertdesc.Visibility = Visibility.Visible;
            lblAssetMaintAlert.Visibility = Visibility.Visible;
            lblAssetMaintAlertdesc.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the scanned alert fields.
        /// </summary>
        private void hideScannedAlertFields()
        {
            borderAlertSA.Visibility = Visibility.Hidden;
            lblAssetIDAlert.Visibility = Visibility.Hidden;
            lblAssetIDAlertdesc.Visibility = Visibility.Hidden;
            lblTagExpiryAlert.Visibility = Visibility.Hidden;
            lblTagExpiryAlertdesc.Visibility = Visibility.Hidden;
            lblAssetMaintAlert.Visibility = Visibility.Hidden;
            lblAssetMaintAlertdesc.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Hides the dashboard alerts.
        /// </summary>
        private void hideDashboardAlerts()
        {
            lblAlertForAssetID.Visibility = Visibility.Hidden;
            lblAlertTagExp.Visibility = Visibility.Hidden;
            lblAlertAssetMaintDue.Visibility = Visibility.Hidden;

            lblalertaid.Visibility = Visibility.Hidden;
            lblalertamd.Visibility = Visibility.Hidden;
            lblAlertte.Visibility = Visibility.Hidden;

            borderAlerts.Visibility = Visibility.Hidden;
 
        }

        /*************************************************************************/

        /// <summary>
        /// Demo mode for some of the features. i.e. Disabled
        /// </summary>
        private void trialVersionFeatures()
        {
            //Help options
            menuCheckUpdates.IsEnabled = false;
            imgcheckUpdates.Source = new BitmapImage(new Uri(@"./DemoFeatures/checkUpdatesDisabled.png", UriKind.Relative));
            
            //Tools options
            menuToolsUserOptions.IsEnabled = false;
            imguserSettings.Source = new BitmapImage(new Uri(@"./DemoFeatures/userSettingsDisabled.png", UriKind.Relative));

            menuToolsAssetTagOptions.IsEnabled = false;
            imgtagSettings.Source = new BitmapImage(new Uri(@"./DemoFeatures/tagSettingsDisabled.png", UriKind.Relative));

            menuToolsReaderMapManage.IsEnabled = false;
            imgRFreader.Source = new BitmapImage(new Uri(@"./DemoFeatures/RFreaderDisabled.png", UriKind.Relative));

            menuToolsReaderSettings.IsEnabled = false;
            imgsettings.Source = new BitmapImage(new Uri(@"./DemoFeatures/settingsDisabled.png", UriKind.Relative));

            menuToolsTestNetwork.IsEnabled = false;
            imgtestNetwork.Source = new BitmapImage(new Uri(@"./DemoFeatures/testNetworkDisabled.png", UriKind.Relative));

            menuToolsTestDatabase.IsEnabled = false;
            imgtestDatabase.Source = new BitmapImage(new Uri(@"./DemoFeatures/testDatabaseDisabled.png", UriKind.Relative));

            menuToolsMaps.IsEnabled = false;
            imgmaps.Source = new BitmapImage(new Uri(@"./DemoFeatures/mapsDisabled.png", UriKind.Relative));

            menuToolsSkins.IsEnabled = false;
            imgskins.Source = new BitmapImage(new Uri(@"./DemoFeatures/skinsDisabled.png", UriKind.Relative));
         

            //Report options
            menuReportsOpt.IsEnabled = false;
            imgreportSetting.Source = new BitmapImage(new Uri(@"./DemoFeatures/reportSettingDisabled.png", UriKind.Relative));

            menuCharts.IsEnabled = false;
            imgchartsImg.Source = new BitmapImage(new Uri(@"./DemoFeatures/chartsImgDisabled.png", UriKind.Relative));

            menuReportsAutoGen.IsEnabled = false;
            imgreportsAutoGen.Source = new BitmapImage(new Uri(@"./DemoFeatures/reportsAutoGenDisabled.png", UriKind.Relative));

            menuReportsNotificationAssets.IsEnabled = false;
            imgreportNotify.Source = new BitmapImage(new Uri(@"./DemoFeatures/reportNotifyDisabled.png", UriKind.Relative));

            //Reader connection options

            fileMenuRunAllMultiple.IsEnabled = false;
            imgMenuRunAllMultiple.Source = new BitmapImage(new Uri(@"./IconImages/RunAllMultipleDisabled1.png", UriKind.Relative));
            
            //File options

            fileMenuImportSetings.IsEnabled = false;
            imgimportFile.Source = new BitmapImage(new Uri(@"./DemoFeatures/importFileDisabled.png", UriKind.Relative));

            fileMenuSaveSettings.IsEnabled = false;
            imgexportFile.Source = new BitmapImage(new Uri(@"./DemoFeatures/exportFileDisabled.png", UriKind.Relative));


            //Edit options
            editMenuCopy.IsEnabled = false;
            imgeditCopy.Source = new BitmapImage(new Uri(@"./DemoFeatures/editCopyDisabled.png", UriKind.Relative));

            editMenuSelectAll.IsEnabled = false;
            imgeditSelectAll.Source = new BitmapImage(new Uri(@"./DemoFeatures/editSelectAllDisabled.png", UriKind.Relative));

            editMenuInvertSelect.IsEnabled = false;
            imgeditInvertSelect.Source = new BitmapImage(new Uri(@"./DemoFeatures/editInvertSelectDisabled.png", UriKind.Relative));

            //Toolbar items

            menuReaderSettings.IsEnabled = false;
            imgTBsettings.Source = new BitmapImage(new Uri(@"./DemoFeatures/settingsDisabled.png", UriKind.Relative));

           
            //Warning note visible

            textBlockTrialVersionWarning.Visibility = Visibility.Visible;
            
            borderExpireAlert.Visibility = Visibility.Visible;

            //Get the days left:
            if ((getDaysRemaining() <= 0) || (Properties.Settings.Default.globalRemainingDays <= 0))
            {
                textBlockTrialVersionWarning.Text = "The trial version has expired !";
                // Show window...the product has expired...Please register.
                TrialExpired appExpired = new TrialExpired();
                appExpired.ShowDialog(); 
            }
            else if(getDaysRemaining() == 1)
            {
                textBlockTrialVersionWarning.Text += " " + getDaysRemaining().ToString() + " day !";
            }
            else
            {
                textBlockTrialVersionWarning.Text += " " + getDaysRemaining().ToString() + " days !";
            }
        }

        /// <summary>
        /// Full version features available
        /// </summary>
        public void fullVersionFeatures()
        {

            //Help options
            menuCheckUpdates.IsEnabled = true;
            imgcheckUpdates.Source = new BitmapImage(new Uri(@"./IconImages/checkUpdates.png", UriKind.Relative));


            //Tools options
            menuToolsUserOptions.IsEnabled = true;
            imguserSettings.Source = new BitmapImage(new Uri(@"./IconImages/userSettings.png", UriKind.Relative));

            menuToolsAssetTagOptions.IsEnabled = true;
            imgtagSettings.Source = new BitmapImage(new Uri(@"./IconImages/tagSettings.png", UriKind.Relative));

            menuToolsReaderMapManage.IsEnabled = true;
            imgRFreader.Source = new BitmapImage(new Uri(@"./IconImages/RFreader.png", UriKind.Relative));

            menuToolsReaderSettings.IsEnabled = true;
            imgsettings.Source = new BitmapImage(new Uri(@"./IconImages/settings.png", UriKind.Relative));

            menuToolsTestNetwork.IsEnabled = true;
            imgtestNetwork.Source = new BitmapImage(new Uri(@"./IconImages/testNetwork.png", UriKind.Relative));

            menuToolsTestDatabase.IsEnabled = true;
            imgtestDatabase.Source = new BitmapImage(new Uri(@"./IconImages/testDatabase.png", UriKind.Relative));

            menuToolsMaps.IsEnabled = true;
            imgmaps.Source = new BitmapImage(new Uri(@"./IconImages/maps.png", UriKind.Relative));

            menuToolsSkins.IsEnabled = true;
            imgskins.Source = new BitmapImage(new Uri(@"./IconImages/skins.png", UriKind.Relative));
         
            //Report options
            menuReportsOpt.IsEnabled = true;
            imgreportSetting.Source = new BitmapImage(new Uri(@"./IconImages/reportSetting.png", UriKind.Relative));

            menuCharts.IsEnabled = true;
            imgchartsImg.Source = new BitmapImage(new Uri(@"./IconImages/chartsImg.png", UriKind.Relative));

            menuReportsAutoGen.IsEnabled = true;
            imgreportsAutoGen.Source = new BitmapImage(new Uri(@"./IconImages/reportsAutoGen.png", UriKind.Relative));

            menuReportsNotificationAssets.IsEnabled = true;
            imgreportNotify.Source = new BitmapImage(new Uri(@"./IconImages/reportNotify.png", UriKind.Relative));

            //Reader connection options 
            fileMenuRunAllMultiple.IsEnabled = true;
            imgMenuRunAllMultiple.Source = new BitmapImage(new Uri(@"./IconImages/RunAllMultiple.png", UriKind.Relative));
            
            //File options

            fileMenuImportSetings.IsEnabled = true;
            imgimportFile.Source = new BitmapImage(new Uri(@"./IconImages/importFile.png", UriKind.Relative));

            fileMenuSaveSettings.IsEnabled = true;
            imgexportFile.Source = new BitmapImage(new Uri(@"./IconImages/exportFile.png", UriKind.Relative));


            //Edit options
            editMenuCopy.IsEnabled = true;
            imgeditCopy.Source = new BitmapImage(new Uri(@"./IconImages/editCopy.png", UriKind.Relative));

            editMenuSelectAll.IsEnabled = true;
            imgeditSelectAll.Source = new BitmapImage(new Uri(@"./IconImages/editSelectAll.png", UriKind.Relative));

            editMenuInvertSelect.IsEnabled = true;
            imgeditInvertSelect.Source = new BitmapImage(new Uri(@"./IconImages/editInvertSelect.png", UriKind.Relative));

            //Toolbar items

            menuReaderSettings.IsEnabled = true;
            imgTBsettings.Source = new BitmapImage(new Uri(@"./IconImages/settings.png", UriKind.Relative));

           
            //Warning note visible

            textBlockTrialVersionWarning.Visibility = Visibility.Hidden;
            
            borderExpireAlert.Visibility = Visibility.Hidden;
            
 
        }

        /// <summary>
        /// Gets the days remaining.
        /// Logic to calculate and find out time period passed.
        /// </summary>
        /// <returns></returns>
        private int getDaysRemaining()
        {
            
            int daysPassed;
            int remainingDays = 0;

            DateTime installedDate, todaysDate;
            
            if (Properties.Settings.Default.installedDate.Year == 2000)
            {
                //First run date is the installed date.. So app expire time starts here.
                Properties.Settings.Default.installedDate = DateTime.Now;
                
            }

            installedDate = Properties.Settings.Default.installedDate;

            todaysDate = DateTime.Now;
            //installedDate = DateTime.Now.AddDays(-4);

            daysPassed = (int)(todaysDate - installedDate).TotalDays;
            
            remainingDays = trialPeriod - daysPassed;

            
            // Initial default to 100;. THis value can only decrease with time
            if (Properties.Settings.Default.globalRemainingDays >= remainingDays)
            {
                Properties.Settings.Default.globalRemainingDays = remainingDays;
            }
            else if (Properties.Settings.Default.globalRemainingDays < remainingDays)
            {
                // Somebody has changed system datetime settings.
                //Tried to hack the software using change of datetime.
                Properties.Settings.Default.globalRemainingDays = -1;
                remainingDays = -1;

                string messageStr = @"It seems the system datetime settings were changed or a trial version hack intended to remove the expiry period was detected by the application. Please note that the application will expire with immediate effect or be locked. Contact the system administrator.";
                LogFile.Log("Error - Hack detected: " + messageStr);
                
                MessageBox.Show(messageStr, "Application hack detected", MessageBoxButton.OK, MessageBoxImage.Stop);
            }


            Properties.Settings.Default.Save();
            return remainingDays;
        }

        /// <summary>
        /// Checks the app expiry.
        /// </summary>
        private void checkAppExpiry()
        {
            if (Properties.Settings.Default.isProductActivated == false)
            {
                trialVersionFeatures();
            }
            else
            {
                fullVersionFeatures();
            }
        }

        /// <summary>
        /// Handles the Tick event of the productTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void productTimer_Tick(object sender, EventArgs e)
        {
            checkAppExpiry();

            productTimer.Stop();
            
        }

        /// <summary>
        /// Handles the Tick event of the outputSignal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void outputSignal_Tick(object sender, EventArgs e)
        {
            //outputSignal.Stop();
            sendCount++;
            HomeWindow.sendOutputSignalBeep();
            if (sendCount > 20)
            {
                outputSignal.Stop();
            }
            //Beep here or change the serial port output here etc

        }

        

       

        
       
    }
}
