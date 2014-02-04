using System;
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

namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for AlertNotificationSettings.xaml
    /// </summary>
    public partial class AlertNotificationSettings : Window
    {
        // Global variables to be used in other windows and objects
        public static bool capAssetsInOut, capAssetTagExpiry, capAssetMaintenanceDue;
        public static bool logsScannedAssets, logsCAP;

        public static bool assetUpdates, assetAdded, assetDeleted;
        public static bool userUpdates, userAdded, userDeleted;
        public static bool readerUpdates, readerAdded, readerDeleted;

        public static bool sendEmailUser, sendEmailAdmin;
        public static bool sendSMSUser, sendSMSAdmin;

        /// <summary>
        /// Constructor of the <see cref="AlertNotificationSettings" /> class.
        /// </summary>
        public AlertNotificationSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the alertNotifyWindow control.
        /// Loads the user set values
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void alertNotifyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Assign set bool values and UI
            //assignDefaultAlertNotifyValues();
            assignUserSetValues();
            
        }

        /// <summary>
        /// Assigns the default alert notify values.
        /// </summary>
        public static void assignDefaultAlertNotifyValues()
        {
            capAssetsInOut = capAssetTagExpiry = capAssetMaintenanceDue = true;
            logsScannedAssets = logsCAP = true;

            assetUpdates = assetAdded = assetDeleted = false;
            userUpdates = userAdded = userDeleted = false;
            readerUpdates = readerAdded = readerDeleted = false;

            sendEmailUser = sendEmailAdmin = false;
            sendSMSUser = sendSMSAdmin = false;
        }

        /// <summary>
        /// Assigns the user set values.
        /// </summary>
        private void assignUserSetValues()
        {
            bool criticalParam = false;
            bool cbLogs = false;
            bool assetOption = false;
            bool userOption = false;
            bool readerOption = false;
            //bool 

            //////Check and set all the values
            checkBoxCriticalAssetParams.IsChecked = false;
            checkBoxCriticalAssetParams.IsChecked = true;

            if (capAssetsInOut == true)
            {
                checkBoxAssetsMovingINOut.IsChecked = true;
                criticalParam = true;
            }
            else
            {
                checkBoxAssetsMovingINOut.IsChecked = false;

            }

            if (capAssetTagExpiry == true)
            {
                checkBoxAssetTagExpiry.IsChecked = true;
                criticalParam = true;

            }
            else
            {
                checkBoxAssetTagExpiry.IsChecked = false;
            }

            if (capAssetMaintenanceDue == true)
            {
                checkBoxAssetMaintDue.IsChecked = true;
                criticalParam = true;
            }
            else
            {
                checkBoxAssetMaintDue.IsChecked = false;
            }

            if (!(criticalParam))
            {
                checkBoxCriticalAssetParams.IsChecked = false;
            }

            /////
            checkBoxLogs.IsChecked = false;
            checkBoxLogs.IsChecked = true;
           
            if (logsScannedAssets == true)
            {
                checkBoxScannedAssets.IsChecked = true;
                cbLogs = true;
                
            }
            else
            {
                checkBoxScannedAssets.IsChecked = false;
            }


            if (logsCAP == true)
            {
                checkBoxCriticalAParams.IsChecked = true;
                cbLogs = true;
                
            }
            else
            {
                checkBoxCriticalAParams.IsChecked = false;
            }

            if (!(cbLogs))
            {
                checkBoxLogs.IsChecked = false;
 
            }

            ///////
            //////
            checkBoxAssetOptions.IsChecked = false;
            checkBoxAssetOptions.IsChecked = true;

            if (assetUpdates == true)
            {
                checkBoxAssetUpdates.IsChecked = true;
                assetOption = true;
            }
            else
            {
                checkBoxAssetUpdates.IsChecked = false;
            }

            if (assetAdded == true)
            {
                checkBoxAssetAdded.IsChecked = true;
                assetOption = true;
            }
            else
            {
                checkBoxAssetAdded.IsChecked = false;
            }

            if (assetDeleted == true)
            {
                checkBoxAssetDeleted.IsChecked = true;
                assetOption = true;
            }
            else
            {
                checkBoxAssetDeleted.IsChecked = false;
            }

            if (!(assetOption))
            {
                checkBoxAssetOptions.IsChecked = false;
            }

            //////
            //////
            checkBoxUserOptions.IsChecked = false;
            checkBoxUserOptions.IsChecked = true;

            if (userUpdates == true)
            {
                checkBoxUserUpdates.IsChecked = true;
                userOption = true;
            }
            else
            {
                checkBoxUserUpdates.IsChecked = false;
            }

            if (userAdded == true)
            {
                checkBoxUserAdded.IsChecked = true;
                userOption = true;
            }
            else
            {
                checkBoxUserAdded.IsChecked = false;
            }

            if (userDeleted == true)
            {
                checkBoxUserDeleted.IsChecked = true;
                userOption = true;
            }
            else
            {
                checkBoxUserDeleted.IsChecked = false;
            }

            if (!(userOption))
            {
                checkBoxUserOptions.IsChecked = false;
 
            }
            /////
            checkBoxReaderOptions.IsChecked = false;
            checkBoxReaderOptions.IsChecked = true;

            if (readerUpdates == true)
            {
                checkBoxReaderLocUpdate.IsChecked = true;
                readerOption = true;
            }
            else
            {
                checkBoxReaderLocUpdate.IsChecked = false;
            }

            if (readerAdded == true)
            {
                checkBoxReaderAdded.IsChecked = true;
                readerOption = true;
            }
            else
            {
                checkBoxReaderAdded.IsChecked = false;
            }

            if (readerDeleted == true)
            {
                checkBoxReaderRemoved.IsChecked = true;
                readerOption = true;
            }
            else
            {
                checkBoxReaderRemoved.IsChecked = false;
            }

            if (!(readerOption))
            {
                checkBoxReaderOptions.IsChecked = false;
                
            }
            /////

            if (sendEmailUser == true)
            {
                checkBoxEmailOptions.IsChecked = true;
            }
            else
            {
                checkBoxEmailOptions.IsChecked = false;
            }

            if (sendEmailAdmin == true)
            {
                checkBoxEmailAdmin.IsChecked = true;
            }
            else
            {
                checkBoxEmailAdmin.IsChecked = false;
            }

            if (sendSMSUser == true)
            {
                checkBoxSendSMSUser.IsChecked = true;
            }
            else
            {
                checkBoxSendSMSUser.IsChecked = false;
            }

            if (sendSMSAdmin == true)
            {
                checkBoxSendSMSAdmin.IsChecked = true;
            }
            else
            {
                checkBoxSendSMSAdmin.IsChecked = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        
        /// <summary>
        /// Handles the Click event of the btnRestoreDefaults control.
        /// Restores the default settings
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnRestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            assignDefaultAlertNotifyValues();

            // Initialize to false
            checkBoxCriticalAssetParams.IsChecked = false;
            checkBoxLogs.IsChecked = false;
            // Trigger the checked event
            checkBoxCriticalAssetParams.IsChecked = true;
            checkBoxLogs.IsChecked = true;
            

            checkBoxEmailOptions.IsChecked = false;
            checkBoxSendSMSUser.IsChecked = false;

            checkBoxAssetOptions.IsChecked = false;
            checkBoxUserOptions.IsChecked = false;
            checkBoxReaderOptions.IsChecked = false;

            textBlockAlertNotifyMessage.Visibility = Visibility.Visible;          
            textBlockAlertNotifyMessage.Text = "Default values restored !";

        }


        /// <summary>
        /// Handles the Checked event of the checkBoxCriticalAssetParams control.
        /// Check box UI options
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxCriticalAssetParams_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxAssetsMovingINOut.IsEnabled = true;
            checkBoxAssetsMovingINOut.IsChecked = true;

            checkBoxAssetTagExpiry.IsEnabled = true;
            checkBoxAssetTagExpiry.IsChecked = true;

            checkBoxAssetMaintDue.IsEnabled = true;
            checkBoxAssetMaintDue.IsChecked = true;

        }

        /// <summary>
        /// Handles the Checked event of the checkBoxAssetOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxAssetOptions_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxAssetUpdates.IsEnabled = true;
            checkBoxAssetUpdates.IsChecked = true;

            checkBoxAssetAdded.IsEnabled = true;
            checkBoxAssetAdded.IsChecked = true;

            checkBoxAssetDeleted.IsEnabled = true;
            checkBoxAssetDeleted.IsChecked = true;
            
        }

        /// <summary>
        /// Handles the Checked event of the checkBoxUserOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxUserOptions_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxUserUpdates.IsEnabled = true;
            checkBoxUserUpdates.IsChecked = true;

            checkBoxUserAdded.IsEnabled = true;
            checkBoxUserAdded.IsChecked = true;

            checkBoxUserDeleted.IsEnabled = true;
            checkBoxUserDeleted.IsChecked = true;
        }

        /// <summary>
        /// Handles the Checked event of the checkBoxReaderOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxReaderOptions_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxReaderLocUpdate.IsEnabled = true;
            checkBoxReaderLocUpdate.IsChecked = true;

            checkBoxReaderAdded.IsEnabled = true;
            checkBoxReaderAdded.IsChecked = true;

            checkBoxReaderRemoved.IsEnabled = true;
            checkBoxReaderRemoved.IsChecked = true;
        }

        /// <summary>
        /// Handles the Checked event of the checkBoxEmailOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxEmailOptions_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxEmailAdmin.IsEnabled = true;
            checkBoxEmailAdmin.IsChecked = true;

        }

        /// <summary>
        /// Handles the Checked event of the checkBoxSendSMSUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxSendSMSUser_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxSendSMSAdmin.IsEnabled = true;
            checkBoxSendSMSAdmin.IsChecked = true;
        }

        /// <summary>
        /// Handles the Checked event of the checkBoxLogs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxLogs_Checked(object sender, RoutedEventArgs e)
        {

            checkBoxScannedAssets.IsEnabled = true;
            checkBoxScannedAssets.IsChecked = true;

            checkBoxCriticalAParams.IsEnabled = true;
            checkBoxCriticalAParams.IsChecked = true;
        }

        /// <summary>
        /// Handles the Unchecked event of the checkBoxCriticalAssetParams control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxCriticalAssetParams_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxAssetsMovingINOut.IsEnabled = false;
            checkBoxAssetsMovingINOut.IsChecked = false;

            checkBoxAssetTagExpiry.IsEnabled = false;
            checkBoxAssetTagExpiry.IsChecked = false;

            checkBoxAssetMaintDue.IsEnabled = false;
            checkBoxAssetMaintDue.IsChecked = false;

        }

        /// <summary>
        /// Handles the Unchecked event of the checkBoxAssetOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxAssetOptions_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxAssetUpdates.IsEnabled = false;
            checkBoxAssetUpdates.IsChecked = false;

            checkBoxAssetAdded.IsEnabled = false;
            checkBoxAssetAdded.IsChecked = false;

            checkBoxAssetDeleted.IsEnabled = false;
            checkBoxAssetDeleted.IsChecked = false;

        }

        /// <summary>
        /// Handles the Unchecked event of the checkBoxUserOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxUserOptions_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxUserUpdates.IsEnabled = false;
            checkBoxUserUpdates.IsChecked = false;

            checkBoxUserAdded.IsEnabled = false;
            checkBoxUserAdded.IsChecked = false;

            checkBoxUserDeleted.IsEnabled = false;
            checkBoxUserDeleted.IsChecked = false;
        }

        /// <summary>
        /// Handles the Unchecked event of the checkBoxReaderOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxReaderOptions_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxReaderLocUpdate.IsEnabled = false;
            checkBoxReaderLocUpdate.IsChecked = false;

            checkBoxReaderAdded.IsEnabled = false;
            checkBoxReaderAdded.IsChecked = false;

            checkBoxReaderRemoved.IsEnabled = false;
            checkBoxReaderRemoved.IsChecked = false;
        }

        /// <summary>
        /// Handles the Unchecked event of the checkBoxEmailOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxEmailOptions_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxEmailAdmin.IsEnabled = false;
            checkBoxEmailAdmin.IsChecked = false;

        }

        /// <summary>
        /// Handles the Unchecked event of the checkBoxSendSMSUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxSendSMSUser_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxSendSMSAdmin.IsEnabled = false;
            checkBoxSendSMSAdmin.IsChecked = false;

        }

        /// <summary>
        /// Handles the Unchecked event of the checkBoxLogs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkBoxLogs_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxScannedAssets.IsEnabled = false;
            checkBoxScannedAssets.IsChecked = false;

            checkBoxCriticalAParams.IsEnabled = false;
            checkBoxCriticalAParams.IsChecked = false;

        }

        /// <summary>
        /// Handles the Click event of the btnSaveSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
           
            textBlockAlertNotifyMessage.Visibility = Visibility.Visible;
            textBlockAlertNotifyMessage.Text = "Settings Saved !";

            if (checkBoxAssetsMovingINOut.IsChecked == true)
            {
                capAssetsInOut = true;
            }
            else
            {
                capAssetsInOut = false;
            }

            if (checkBoxAssetTagExpiry.IsChecked == true)
            {
                capAssetTagExpiry = true;
            }
            else
            {
                capAssetTagExpiry = false;
            }

            if (checkBoxAssetMaintDue.IsChecked == true)
            {
                capAssetMaintenanceDue = true;
            }
            else
            {
                capAssetMaintenanceDue = false;
            }

            if (checkBoxScannedAssets.IsChecked == true)
            {
                logsScannedAssets = true;
            }
            else
            {
                logsScannedAssets = false;
            }


            if (checkBoxCriticalAParams.IsChecked == true)
            {
                logsCAP = true;
            }
            else
            {
                logsCAP = false;
            }


            if (checkBoxAssetUpdates.IsChecked == true)
            {
                assetUpdates = true;
            }
            else
            {
                assetUpdates = false;
            }

            if (checkBoxAssetAdded.IsChecked == true)
            {
                assetAdded = true;
            }
            else
            {
                assetAdded = false;
            }

            if (checkBoxAssetDeleted.IsChecked == true)
            {
                assetDeleted = true;
            }
            else
            {
                assetDeleted = false;
            }

            if (checkBoxUserUpdates.IsChecked == true)
            {
                userUpdates = true;
            }
            else
            {
                userUpdates = false;
            }

            if (checkBoxUserAdded.IsChecked == true)
            {
                userAdded = true;
            }
            else
            {
                userAdded = false;
            }

            if (checkBoxUserDeleted.IsChecked == true)
            {
                userDeleted = true;
            }
            else
            {
                userDeleted = false;
            }

            if (checkBoxReaderLocUpdate.IsChecked == true)
            {
                readerUpdates = true;
            }
            else
            {
                readerUpdates = false;
            }

            if (checkBoxReaderAdded.IsChecked == true)
            {
                readerAdded = true;
            }
            else
            {
                readerAdded = false;
            }

            if (checkBoxReaderRemoved.IsChecked == true)
            {
                readerDeleted = true;
            }
            else
            {
                readerDeleted = false;
            }

            if (checkBoxEmailOptions.IsChecked == true)
            {
                sendEmailUser = true;
            }
            else
            {
                sendEmailUser = false;
            }

            if (checkBoxEmailAdmin.IsChecked == true)
            {
                sendEmailAdmin = true;
            }
            else
            {
                sendEmailAdmin = false;
            }

            if (checkBoxSendSMSUser.IsChecked == true)
            {
                sendSMSUser = true;
            }
            else
            {
                sendSMSUser = false;
            }

            if (checkBoxSendSMSAdmin.IsChecked == true)
            {
                sendSMSAdmin = true;
            }
            else
            {
                sendSMSAdmin = false;
            }


            //Check for valid email and mobile if check boxes are checked for email and SMS
            if (AlertNotificationSettings.sendEmailUser == true)
            {
                if ((!(sendEmail.checkEmail(LoginWindow.user_email))) || (string.IsNullOrEmpty(LoginWindow.user_email)))
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

            this.Close();

        }

    }
}
