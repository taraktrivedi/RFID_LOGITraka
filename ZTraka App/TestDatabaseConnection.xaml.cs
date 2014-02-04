using System;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for TestDatabaseConnection.xaml
    /// </summary>
    public partial class TestDatabaseConnection : Window
    {
        // Variables private
        private short toggleButtonEditConnection = 0;
        private string dataSource,dataBaseName, userid, passwd;
        private string newConnectionString;


        /// <summary>
        /// Constructor of the <see cref="TestDatabaseConnection" /> class.
        /// </summary>
        public TestDatabaseConnection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the testdbWindow control.
        /// Load the parameters of the connections string in the textboxes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void testdbWindow_Loaded(object sender, RoutedEventArgs e)
        {

            string ConString;
            string[] connArray, connectionProperty;
           
            ConString = ConfigurationManager.ConnectionStrings["ZTraka_App.Properties.Settings.ztATdbLocalConnectionString"].ConnectionString;
            // Get the connection strings section.
            connArray = ConString.Split(';');

            for (int i = 0; i < connArray.Length; i++)
            {
                connectionProperty = connArray[i].Split('=');

                if (connectionProperty[0].CompareTo("Data Source") == 0)
                {
                    textBoxDataSource.Text = connectionProperty[1].ToString();
                }
                else if (connectionProperty[0].CompareTo("Initial Catalog") == 0)
                {
                    textBoxDatabaseName.Text = connectionProperty[1].ToString();
                }

                else if (connectionProperty[0].CompareTo("User ID") == 0)
                {
                    textBoxUserID.Text = connectionProperty[1].ToString();
                }

                else if (connectionProperty[0].CompareTo("Password") == 0)
                {
                    passwordBoxDatabase.Password = connectionProperty[1].ToString();
                }
            }
            
            
                
        }

        /// <summary>
        /// Handles the Click event of the btnTestDatabaseConnection control.
        /// Checks the database connection according to the connection string
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnTestDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            int rowCount;

            // Instantiate datatable and table adapter
            ztATdbLocalDataSet1TableAdapters.assetInfoSearchDBDataTableTableAdapter assetInfoTableAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.assetInfoSearchDBDataTableTableAdapter();
            ztATdbLocalDataSet1.assetInfoSearchDBDataTableDataTable tmpResTableA = new ztATdbLocalDataSet1.assetInfoSearchDBDataTableDataTable();

            textBlockConnStringTestNotifyMessage.Visibility = Visibility.Visible;
            try
            {
                rowCount = assetInfoTableAdpt.TestConnection(tmpResTableA);
                
                //Check whether row count is greater than zero
                if (rowCount > 0)
                {
                    textBlockConnStringTestNotifyMessage.Text = "Success !";
                    textBlockConnectionResults.Visibility = Visibility.Visible;
                    textBlockConnectionResults.Background = Brushes.LightGreen;
                    textBlockConnectionResults.Text = "Connection Successful !";
                }
                else
                {
                    textBlockConnStringTestNotifyMessage.Text = "Failed !";
                }
            }
            catch
            {
                textBlockConnStringTestNotifyMessage.Text = "Connection Failed!";
                textBlockConnectionResults.Visibility = Visibility.Visible;
                textBlockConnectionResults.Background = Brushes.LightPink;
                textBlockConnectionResults.Text = "Connection to database failed: Possible database server issue or connection string problem ";
                LogFile.Log("Error: Test Database connection failed! Check connection string or db server.");
            }

        }

        /// <summary>
        /// Handles the Click event of the btnEditConnection control.
        /// Edit the connection string
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnEditConnection_Click(object sender, RoutedEventArgs e)
        {
            textBlockConnectionResults.Visibility = Visibility.Hidden;
            textBlockConnectionResults.Background = Brushes.LightPink;
            ///////////////////////
            // CHECK for USER rights
            // CHECK for user ROLE ID here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockConnectionResults.Visibility = Visibility.Visible;
                textBlockConnectionResults.Text = "Not enough rights to perform this task.";
                return;
            }
            ////////////////
            if (toggleButtonEditConnection == 0)
            {
                toggleButtonEditConnection = 1;
                //btnTestDatabaseConnection.IsEnabled = false;

                textBlockConnStringTestNotifyMessage.Visibility = Visibility.Visible;
                textBlockConnStringTestNotifyMessage.Text = "Edit Mode ON";
                btnEditConnection.Content = "Save Connection";

                //Change to editable mode
                    
                    textBoxDatabaseName.IsReadOnly = false;
                    textBoxDataSource.IsReadOnly = false;
                    
                    textBoxUserID.IsReadOnly = false;

                    passwordBoxDatabase.IsEnabled = true;
                    btnTestDatabaseConnection.IsEnabled = false;
 
            }
            else if (toggleButtonEditConnection == 1)
            {
                if (!(checkNullsNRegex()))
                {
                    //problem
                    return;

                }

                //Change to Read Only mode
                textBoxDatabaseName.IsReadOnly = true;
                textBoxDataSource.IsReadOnly = true;

                textBoxUserID.IsReadOnly = true;

                passwordBoxDatabase.IsEnabled = false;
                btnTestDatabaseConnection.IsEnabled = true;

                textBlockConnStringTestNotifyMessage.Visibility = Visibility.Visible;
                btnEditConnection.Content = "Edit Connection";

                //Store values
                storeValues();

                //Try
                try
                {
                    //At users risk see *change* here
                    //use reflection to poke values in as a way to get access to the non-public fields (and methods).
                    //ConfigurationManager.ConnectionStrings["ZTraka_App.Properties.Settings.ztATdbLocalConnectionString"].ConnectionString = newConnectionString;
                    var settings = ConfigurationManager.ConnectionStrings["ZTraka_App.Properties.Settings.ztATdbLocalConnectionString"];
                    var fi = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                    fi.SetValue(settings, false);
                    settings.ConnectionString = newConnectionString;


                    textBlockConnStringTestNotifyMessage.Text = "Update done!";
                    textBlockConnectionResults.Background = Brushes.LightGreen;
                    textBlockConnectionResults.Visibility = Visibility.Visible;
                    textBlockConnectionResults.Text = "Update to the connection parameters done!";


                }
                catch //(System.Exception ex)
                {
                    textBlockConnStringTestNotifyMessage.Text = "Update failed !";
                    textBlockConnectionResults.Background = Brushes.LightPink;
                    textBlockConnectionResults.Visibility = Visibility.Visible;
                    textBlockConnectionResults.Text = "Update to the connection parameters failed: Possibly due to connection mode restrictions";
                    LogFile.Log("Error: Failed to update changes in the connection string");
                }
 
            }
        }

        /// <summary>
        /// Checks the nulls using regex.
        /// </summary>
        /// <returns></returns>
        private bool checkNullsNRegex()
        {

            //Check for nulls
            //Data source
            if (string.IsNullOrEmpty(textBoxDataSource.Text))
            {
                textBlockConnectionResults.Visibility = Visibility.Visible;
                textBlockConnectionResults.Text = "Warning ! Data Source cannot be blank";
                return false;
            }

            //Datebase name
            if (string.IsNullOrEmpty(textBoxDatabaseName.Text))
            {
                textBlockConnectionResults.Visibility = Visibility.Visible;
                textBlockConnectionResults.Text = "Warning ! Database name cannot be blank";
                return false;
            }
            
            //User ID
            if (string.IsNullOrEmpty(textBoxUserID.Text))
            {
                textBlockConnectionResults.Visibility = Visibility.Visible;
                textBlockConnectionResults.Text = "Warning ! User ID cannot be blank";
                return false;
            }

            //Password
            if (string.IsNullOrEmpty(passwordBoxDatabase.Password))
            {
                textBlockConnectionResults.Visibility = Visibility.Visible;
                textBlockConnectionResults.Text = "Warning ! Password cannot be blank";
                return false;
            }

            // Check for regex
            //Datasource, database name, userID can be of alphabets and numbers.
            //if (!Regex.IsMatch(textBoxDataSource.Text, @"^[a-zA-Z0-9]+$"))
            //{
            //    textBlockConnectionResults.Visibility = Visibility.Visible;
            //    textBlockConnectionResults.Text = "Warning ! Data Source / IP format";
            //    return false;
            //}
          
            //If reached here then all conditions are good
            return true;

        }

        /// <summary>
        /// Stores the values of connection string and creates a new conn string
        /// </summary>
        private void storeValues()
        {
            
            //Store values
            dataSource = textBoxDataSource.Text.TrimEnd();
            dataBaseName = textBoxDatabaseName.Text.TrimEnd();
            userid = textBoxUserID.Text.TrimEnd();
            passwd = passwordBoxDatabase.Password.TrimEnd();

            newConnectionString = @"Data Source=" + dataSource + ";Initial Catalog=" + dataBaseName + ";Persist Security Info=True;User ID=" + userid + ";Password=" + passwd;

        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// Closes the window, does not save changes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
