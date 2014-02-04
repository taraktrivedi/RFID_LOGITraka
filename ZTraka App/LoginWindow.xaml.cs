using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Microsoft.SqlServer.MessageBox;


namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static string user_name ;       
        public static string userID;
        public static string user_roleid;
        public static string user_role;
        public static string user_email;
        public static string user_contact;
       
        bool exceptionOccured = false;
        public static bool loginSuccess = false;

        /// <summary>
        /// Constructor of the <see cref="LoginWindow" /> class.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();    
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the theme
            Application.Current.Resources.Source = new Uri("./WPFThemes/myCustomTheme.xaml", UriKind.RelativeOrAbsolute);
            txtBoxUserID.Focus();
                        
        }


        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="AppUserID">The app user ID.</param>
        /// <param name="encryptedPass">The encrypted pass.</param>
        /// <returns></returns>
        public bool validateUser(string AppUserID, string encryptedPass)
        {
            
            bool checkrows = false; //by default
            string ConString = ConfigurationManager.ConnectionStrings["ZTraka_App.Properties.Settings.ztATdbLocalConnectionString"].ToString();
            string CmdString,CmdUString;
            DateTime tStamp = DateTime.Now;

            using (SqlConnection con = new SqlConnection(ConString))
            {
                try
                {

                    CmdString =  @"SELECT acl.user_name, acl.user_roleid, acl.user_role, acl.user_email, acl.user_contact  
                                   FROM acl INNER JOIN acl_login ON acl.user_id = acl_login.user_id 
                                   WHERE acl_login.user_id ='" + AppUserID + "' and acl_login.user_pass ='" + encryptedPass + "'";
                    
                    CmdUString =  @"UPDATE [acl]
                                    SET [user_last_login] = '" + tStamp + "' " +
                                    "WHERE [user_id] = '" + AppUserID + "'";
                    
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    con.Open();
                    
                                   
                    SqlDataReader dr = (SqlDataReader)cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            // get the results of each column
                            user_name = dr.GetString(0);
                            user_roleid = dr.GetString(1);
                            user_role = dr.GetString(2);
                            user_email = dr.GetString(3);
                            user_contact = dr.GetString(4);
                        }
                        checkrows = true;//success
                        
                    }
                    if (checkrows)
                    {
                        dr.Close();
                        dr.Dispose();
                        cmd.CommandText = CmdUString;
                        SqlDataReader drU = (SqlDataReader)cmd.ExecuteReader();
                        drU.Close();
                        drU.Dispose();
                    }

                    con.Close();
                    return checkrows;
                }

                catch (Exception ex)
                {
                    LogFile.Log("Error: problem connecting to the database !");
                    MessageBox.Show(ex.Message.ToString(),"Error connecting to the database!",MessageBoxButton.OK,MessageBoxImage.Stop);
                    Cursor = Cursors.Arrow;
                    exceptionOccured = true;
                    return checkrows;
                }
                 
            }       

        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// Application shutdown
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            loginSuccess = false;
            this.Close();
       
        }


        //public string Encrypt(string plainText)
        //{
        //    if (plainText == null) throw new ArgumentNullException("plainText");
        //    System.Security.Cryptography.PasswordDeriveBytes
        //    //encrypt data
        //    var data = Encoding.Unicode.GetBytes(plainText);
        //    byte[] encrypted = Protect(data, null, Scope);

        //    //return as base64 string
        //    return Convert.ToBase64String(encrypted);
        //}

        /// <summary>
        /// Handles the Click event of the btnLogin control.
        /// Process the validation and proceed to main window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Set cursor to waiting

            this.Cursor = Cursors.Wait;

            string AppUserID, AppPassword;
            bool valid =false; //by default

            AppUserID = txtBoxUserID.Text.ToString();
            userID = AppUserID; // UserID importantin other forms
            AppPassword = passwordBox1.Password;

            string encryptedPass, decryptedPass;
            encryptedPass = myEncryption.Encrypt(AppPassword, true);
            decryptedPass = myEncryption.Decrypt(encryptedPass, true);

            // Check user authentication directly from the database;
            valid = validateUser(AppUserID, encryptedPass);

            if (checkBoxRemeberMe.IsChecked == true)
            {
                Properties.Settings.Default.userpassW = passwordBox1.Password;
                Properties.Settings.Default.userID = txtBoxUserID.Text.ToUpper().Trim();
                Properties.Settings.Default.Save();
            }


           if (valid) //authentication is good.
           {
               
               // Now setting the application properties with valid username, role and roleid
               Properties.Settings.Default.usernameD = user_name;
                   Properties.Settings.Default.userrole = user_role;
                   Properties.Settings.Default.userroleid = user_roleid;

                   // Variable for future use with App.cs
                   loginSuccess = true;
                this.Close(); //close login window
               // Load the main window
                loadTheMainWindow();
                this.Cursor = Cursors.Arrow;
               
           }

           else if (!exceptionOccured)
           
           {
               loginSuccess = false;
               // Configure the message box to be displayed 
               string messageBoxText = "Please enter username and password correctly !";
               string caption = "Incorrect Login !";
               MessageBoxButton button = MessageBoxButton.OK;
               MessageBoxImage icon = MessageBoxImage.Stop;
               // Display message box
               MessageBox.Show(messageBoxText, caption, button, icon);
               this.Cursor = Cursors.Arrow;
           
           }
        }

        /// <summary>
        /// Handles the GotFocus event of the passwordBox1 control.
        /// Check for username and set background for textboxes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void passwordBox1_GotFocus(object sender, RoutedEventArgs e)
        {

            // if( txtBoxUsername.Text == Properties.Settings.Default.usernameW 
            int compareResult = txtBoxUserID.Text.ToUpper().Trim().CompareTo(Properties.Settings.Default.userID.ToString());
            if (compareResult == 0)
            {
                var bc = new BrushConverter();
                passwordBox1.Background = (Brush)bc.ConvertFrom("#FFF9F9B9");
                txtBoxUserID.Background = (Brush)bc.ConvertFrom("#FFF9F9B9");

                //txtBoxUsername.Background = Brushes.YellowGreen;
                passwordBox1.Password = Properties.Settings.Default.userpassW;
            }
             
        }

        /// <summary>
        /// Loads the main window of the app.
        /// Sets some content for the window.
        /// </summary>
        private void loadTheMainWindow()
        {
            HATrakaMain hatraka = new HATrakaMain();
            //hatraka.Hide();

            hatraka.Show();

            LogFile.Log("User " + LoginWindow.user_name.ToString() + "  has logged in successfully !");
            //Show the main window
            //hatraka.Visibility = Visibility.Visible;
            hatraka.lblWelcomeUsername.Visibility = Visibility.Visible;
            hatraka.lblLogoutExit.Visibility = Visibility.Visible;

            hatraka.lblWelcomeUsername.Content += " " + LoginWindow.user_name + " !";
 
        }

        /// <summary>
        /// Handles the Closed event of the Window control.
        /// Checks for login success, if false then closes and performs necessary actions
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (loginSuccess == false)
            {
                // Log the event and close the window to save resources before shutting down
                loginSuccess = false;
                //Save settings
                Properties.Settings.Default.Save();

                //Check if reader is disconnected before shutting down
                //Always a good idea to disconnect reader before shutdown
                if (HATrakaMain.isReaderDisconnected == false)
                {
                    //disconnectTheReader();
                }

                LogFile.Log("User cancelled login. App not started !");
                LogFile.Log("App Close Event -Done! Time: " + DateTime.Now.ToLongTimeString());

                Application.Current.Shutdown();
            }
        }

       

        

    }
}
