using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data;
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
    /// Interaction logic for UserOptions.xaml
    /// </summary>
    public partial class UserOptions : Window
    {
        short toggleButtonEditProfile = 0;
        short toggleButtonEditPrivileges = 0;
        short toggleButtonAddNewProfile = 0;
        ztATdbLocalDataSet1 userInfoDataSet = new ztATdbLocalDataSet1();
        ztATdbLocalDataSet1TableAdapters.UserInfoDataTableTableAdapter UserInfoTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.UserInfoDataTableTableAdapter();

        // Get all userids from the table for comparison for unique (and no duplicate) otherwise operation will fail unique PK
        ztATdbLocalDataSet1 userIDDataSet = new ztATdbLocalDataSet1();
        ztATdbLocalDataSet1TableAdapters.UserIDDataTableTableAdapter UserIDTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.UserIDDataTableTableAdapter();

        /// <summary>
        /// Constructor of the <see cref="UserOptions" /> class.
        /// </summary>
        public UserOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the userOptionWindow control.
        /// Get user credentials and display user profile
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void userOptionWindow_Loaded(object sender, RoutedEventArgs e)
        {
           showMyProfile();               
        }

        /// <summary>
        /// Shows user profile.
        /// </summary>
        private void showMyProfile()
        {
            //toggleButton = 0;
            int resultFill;
            
            string username = "";
            string[] userNameSplit;
            string userRole = "";
    
            try
            {
                resultFill = UserInfoTA.Fill(userInfoDataSet.UserInfoDataTable, LoginWindow.userID);

                textBoxGPUserID.Text = userInfoDataSet.UserInfoDataTable.Rows[0]["user_id"].ToString();
                username = userInfoDataSet.UserInfoDataTable.Rows[0]["user_name"].ToString();
                userNameSplit = username.Split(' ');
                textBoxGPFirstName.Text = userNameSplit[0];
                textBoxGPLastName.Text = userNameSplit[1];
                textBoxGPContact.Text = userInfoDataSet.UserInfoDataTable.Rows[0]["user_contact"].ToString();
                textBoxGPEmail.Text = userInfoDataSet.UserInfoDataTable.Rows[0]["user_email"].ToString();
                textBoxUserDept.Text = userInfoDataSet.UserInfoDataTable.Rows[0]["user_dept"].ToString();
                textBoxUserIDRights.Text = userInfoDataSet.UserInfoDataTable.Rows[0]["user_id"].ToString();
                userRole = userInfoDataSet.UserInfoDataTable.Rows[0]["user_role"].ToString();

                cmbUserRole.Text = userRole;
            }

            catch (System.Exception ex)
            {
                LogFile.Log("Error: failed to get user profile !");
                MessageBox.Show("Failed to retrieve: " + ex.ToString());
            }
 
        }


        /// <summary>
        /// Handles the Click event of the btnCancelChanges control.
        /// Closes the window without saving and current changes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancelChanges_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbUserRole control.
        /// Changes the role ID according to the role selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void cmbUserRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUserRole.SelectedIndex == 0)
            {
                textBoxRoleID.Text = "AD01";
            }
            else if (cmbUserRole.SelectedIndex == 1)
            {
                textBoxRoleID.Text = "AD02";
            }
            else if (cmbUserRole.SelectedIndex == 2)
            {
                textBoxRoleID.Text = "VORES01";
            }
              
        }

        /// <summary>
        /// Handles the Click event of the btnEdit control.
        /// Update and edit the changes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            string userid, username, usercontact, useremail;
            DateTime userlastupdate;
            textBlockProfileWarning.Background = Brushes.LightPink;

            // CHECK for user ROLE here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockProfileWarning.Visibility = Visibility.Visible;
                textBlockProfileWarning.Text = "Not enough rights to perform this task.";
                return;
            }
            if (toggleButtonEditProfile == 0)
            {
                toggleButtonEditProfile = 1;
                textBlockUserNotifyMessage.Visibility = Visibility.Visible;
                textBlockUserNotifyMessage.Text = "Edit Mode ON";
                btnEdit.Content = "Save Profile";

                //Change to editable mode
                textBoxGPFirstName.IsReadOnly = false;
                textBoxGPLastName.IsReadOnly = false;
                textBoxGPContact.IsReadOnly = false;
                textBoxGPEmail.IsReadOnly = false;
                //


            }
            else if (toggleButtonEditProfile == 1)
            {
                if (!(checkForValidFields()))
                {
                    // There seems to be a problem.
                    return;
                }

                //Change to Read Only mode
                textBoxGPFirstName.IsReadOnly = true;
                textBoxGPLastName.IsReadOnly = true;
                textBoxGPContact.IsReadOnly = true;
                textBoxGPEmail.IsReadOnly = true;

                toggleButtonEditProfile = 0;
                textBlockUserNotifyMessage.Visibility = Visibility.Visible;
                btnEdit.Content = "Edit Profile";

                //Store values
                userid = textBoxGPUserID.Text;

                username = textBoxGPFirstName.Text.Trim();
                username += " ";
                username += textBoxGPLastName.Text;

                usercontact = textBoxGPContact.Text;
                useremail = textBoxGPEmail.Text;

                userlastupdate = DateTime.Now;

                this.Cursor = Cursors.Wait;

                try
                {
                    UserInfoTA.UpdateProfile(username, usercontact, useremail, userlastupdate, userid);
                    textBlockUserNotifyMessage.Text = "Profile saved !";


                    // Log ,email, SMS
                    LogFile.Log("User profile Updated: Record with User ID- " + userid.ToString() + " has been updated !");
                    if (AlertNotificationSettings.userUpdates == true)
                    {
                        this.Cursor = Cursors.Wait;
                        if (AlertNotificationSettings.sendEmailUser == true)
                        {
                            HATrakaMain.emailContent = "User profile Updated: Record with User ID- " + userid.ToString() + " has been updated !";
                            sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                        }
                        if (AlertNotificationSettings.sendSMSUser == true)
                        {
                            SMSClass.sendSMS("User profile Updated: Record with User ID- " + userid.ToString() + " has been updated !", LoginWindow.user_contact);

                            if (AlertNotificationSettings.sendSMSAdmin == true)
                            {
                                SMSClass.sendSMS("User profile Updated: Record with User ID- " + userid.ToString() + " has been updated !", HATrakaMain.superAdminSMS);
                            }
                        }

                    }

                }
                catch (System.Exception ex)
                {
                    textBlockUserNotifyMessage.Text = "Profile update failed !";
                    MessageBox.Show("Update failed: " + ex.ToString());
                    LogFile.Log("Error: Update of user profile failed!");
                }

                finally
                {
                    this.Cursor = Cursors.Arrow;
                }


            }
        }

        /// <summary>
        /// Handles the Click event of the btnChangePassword control.
        /// Changes the password
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            //Check various conditions for password changing procedure
            if (string.IsNullOrEmpty(passwordBoxOldPassword.Password))
            {
                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightPink;
                textBlockPasswordWarning.Text = "Warning ! Old Password cannot be blank.";
                return;
            }
            else if (string.IsNullOrEmpty(passwordBoxNewPass.Password))
            {
                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightPink;
                textBlockPasswordWarning.Text = "Warning ! New Password cannot be blank.";
                return;
            }
            else if (string.IsNullOrEmpty(passwordBoxConfirmNewPass.Password))
            {
                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightPink;
                textBlockPasswordWarning.Text = "Warning ! Please confirm your new password.";
                return;
            }

            //Check old password is correct
            string userPassword = "";
            userPassword = userInfoDataSet.UserInfoDataTable.Rows[0]["user_pass"].ToString();
            userPassword = myEncryption.Decrypt(userPassword,true);
            if (passwordBoxOldPassword.Password.CompareTo(userPassword) != 0)
            {
                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightPink;
                textBlockPasswordWarning.Text = "Error ! Please enter your old password correctly.";
                return;
            }

            if (passwordBoxConfirmNewPass.Password.CompareTo(passwordBoxNewPass.Password) != 0)
            {
                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightPink;
                textBlockPasswordWarning.Text = "Error ! Please match and confirm your new password correctly.";
                return;
            }


            if (passwordBoxOldPassword.Password.CompareTo(passwordBoxNewPass.Password) == 0)
            {
                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightPink;
                textBlockPasswordWarning.Text = "Error ! New password should be different from the old password.";
                return;
            }


            if (passwordBoxNewPass.Password.Length < 8)
            {
                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightPink;
                textBlockPasswordWarning.Text = "Error ! New password should be atleast 8 characters long.";
                return;
            }

            if (!(Regex.IsMatch(passwordBoxNewPass.Password, @"(?=.*[\d])(?=.*[a-z])")))
            {
                //{8,20} # Length atleast 8 characters and maximum of 20	
                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightPink;
                textBlockPasswordWarning.Text = "Error ! New password should contain atleast one alphabet and one number.";
                return;
            }

            // Success
            //Encrypt the new password
            string encryptedPass = myEncryption.Encrypt(passwordBoxNewPass.Password, true);
            try
            {
                UserInfoTA.UpdatePassword(encryptedPass, LoginWindow.userID);

                textBlockPasswordWarning.Visibility = Visibility.Visible;
                textBlockPasswordWarning.Background = Brushes.LightGreen;
                textBlockPasswordWarning.Text = "Success ! Password changed successfully";
                MessageBox.Show("Update successful");
            }
            catch (System.Exception ex)
            {
                LogFile.Log("Error: Password update failed");
                MessageBox.Show("Update failed: " + ex.ToString());
            }

            
            


        }

        /// <summary>
        /// Handles the Click event of the btnEditPrivileges control.
        /// Edit and update user profile priviledges
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnEditPrivileges_Click(object sender, RoutedEventArgs e)
        {
            string userid,userroleid,userrole,userdept;
            DateTime userlastupdate;

            // CHECK for user ROLE here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockRoles.Visibility = Visibility.Visible;
                textBlockRoles.Background = Brushes.LightPink;
                textBlockRoles.Text = "Not enough rights to perform this task.";
                return;
            }

            //By default
            textBoxUserIDRights.IsReadOnly = true;
            btnSearchUserID.IsEnabled = false;
            textBlockSearchUserInfo.Visibility = Visibility.Hidden;
            btnDeleteProfile.IsEnabled = false;

            //Set textBlockRoles
            textBlockRoles.Visibility = Visibility.Visible;
            textBlockRoles.Background = Brushes.LightBlue;
            textBlockRoles.Text = "Note: The roles and privileges of users can only be changed by admins";

            if (toggleButtonEditPrivileges == 0)
            {
                
                    toggleButtonEditPrivileges = 1;
                    textBlockuserRolesNotifyMessage.Text = "Edit Mode ON";
                    textBlockuserRolesNotifyMessage.Visibility = Visibility.Visible;
                    btnEditPrivileges.Content = "Save Changes";

                    //Change to Edit mode
                    textBoxUserDept.IsReadOnly = false;
                    cmbUserRole.IsEnabled = true;
              
            }
            else if (toggleButtonEditPrivileges == 1)
            {
                toggleButtonEditPrivileges = 0;
                textBlockuserRolesNotifyMessage.Visibility = Visibility.Visible;
                btnEditPrivileges.Content = "Edit Privileges";

                userid = textBoxUserIDRights.Text;
                userlastupdate = DateTime.Now;
                userrole = cmbUserRole.Text;
                userroleid = textBoxRoleID.Text;
                userdept = textBoxUserDept.Text;

                //Change to read only mode
                textBoxUserDept.IsReadOnly = true;
                cmbUserRole.IsEnabled = false;

                this.Cursor = Cursors.Wait;

                try
                {
                    UserInfoTA.UpdatePrivilegesACL(userdept, userrole, userroleid, userlastupdate, userid);
                    UserInfoTA.UpdatePrivilegesACLlogin(userroleid, userid);
                    textBlockuserRolesNotifyMessage.Text = "User privileges saved !";

                    // Log ,email, SMS
                    LogFile.Log("User privileges Updated: Record with User ID- " + userid.ToString() + " has been updated !");
                    if (AlertNotificationSettings.userUpdates == true)
                    {
                        this.Cursor = Cursors.Wait;
                        if (AlertNotificationSettings.sendEmailUser == true)
                        {
                            HATrakaMain.emailContent = "User privileges Updated: Record with User ID- " + userid.ToString() + " has been updated !";
                            sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                        }
                        if (AlertNotificationSettings.sendSMSUser == true)
                        {
                            SMSClass.sendSMS("User privileges Updated: Record with User ID- " + userid.ToString() + " has been updated !", LoginWindow.user_contact);

                            if (AlertNotificationSettings.sendSMSAdmin == true)
                            {
                                SMSClass.sendSMS("User privileges Updated: Record with User ID- " + userid.ToString() + " has been updated !", HATrakaMain.superAdminSMS);
                            }
                        }

                    }

                }
                catch (System.Exception ex)
                {
                    LogFile.Log("Error: User Privilege update failed ! ");
                    textBlockuserRolesNotifyMessage.Text = "Privilege update failed !";
                    MessageBox.Show("Update failed: " + ex.ToString());
                }

                finally
                {
                    this.Cursor = Cursors.Arrow;
                }


            }
        }


        /// <summary>
        /// Handles the Click event of the btnManageOtherUsers control.
        /// Manage other users profile
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnManageOtherUsers_Click(object sender, RoutedEventArgs e)
        {
            string userRoleID = "";
            userRoleID = textBoxRoleID.Text;
            //check for ADMIN or ADMIN02 roles
            // CHECK for user ROLE here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockRoles.Visibility = Visibility.Visible;
                textBlockRoles.Background = Brushes.LightPink;
                textBlockRoles.Text = "Not enough rights to perform this task.";
                return;
            }
                btnSearchUserID.IsEnabled = true;
                textBoxUserIDRights.IsReadOnly = false;
                textBlockSearchUserInfo.Visibility = Visibility.Visible;

                textBlockRoles.Visibility = Visibility.Visible;
                textBlockRoles.Background = Brushes.LightBlue;
                textBlockRoles.Text = "Note: The roles and privileges of users can only be changed by admins";
                //btnDeleteProfile.IsEnabled = true;

           
        }

        /// <summary>
        /// Handles the Click event of the btnSearchUserID control.
        /// Serach the user profile by his ID
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnSearchUserID_Click(object sender, RoutedEventArgs e)
        {
            string userID = "";
            userID = textBoxUserIDRights.Text;

            if (string.IsNullOrEmpty(userID))
            {
                textBlockuserRolesNotifyMessage.Visibility = Visibility.Visible;
                textBlockuserRolesNotifyMessage.Text = "Enter a search term !";
                return;

            }

            // Wild char search
            userID = "%"+ userID + "%";
            int rowCount;
            textBlockuserRolesNotifyMessage.Visibility = Visibility.Visible;

            //Temp table to store privilege data of other users
            ztATdbLocalDataSet1.UserInfoDataTableDataTable oUsersDataTable = new ztATdbLocalDataSet1.UserInfoDataTableDataTable();

            this.Cursor = Cursors.Wait;

            try
            {
                rowCount = UserInfoTA.Fill(oUsersDataTable, userID);

                if (rowCount > 0)
                {
                    textBlockuserRolesNotifyMessage.Text = "User found !";

                    textBoxUserDept.Text = oUsersDataTable.Rows[0]["user_dept"].ToString();
                    textBoxUserIDRights.Text = oUsersDataTable.Rows[0]["user_id"].ToString();
                    string userRole = oUsersDataTable.Rows[0]["user_role"].ToString();

                    cmbUserRole.Text = userRole;

                    btnDeleteProfile.IsEnabled = true;


                }
                else
                {
                    textBlockuserRolesNotifyMessage.Text = "No User found !";
                }

            }
            catch (System.Exception ex)
            {
                LogFile.Log("Error: No user profile found for this ID");
                textBlockuserRolesNotifyMessage.Text = "Search Failed!";
                MessageBox.Show("Search failed: " + ex.ToString());
            }
            finally
            {
                oUsersDataTable.Dispose();
                this.Cursor = Cursors.Arrow;
            }

        }

        /// <summary>
        /// Handles the Click event of the btnAddNew control.
        /// Add new user profile
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {

            string idANDpassword = "";
            string euserPassword = "";
            string userid,userpass,username,firstName,lastName,usercontact,useremail,userdept,userrole,userroleid;
            string[] idPassSplit;
            DateTime userlastupdate, userdateCreated;

            textBlockProfileWarning.Background = Brushes.LightPink;

            //check for ADMIN or ADMIN02 roles
            // CHECK for user ROLE here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockProfileWarning.Visibility = Visibility.Visible;
                textBlockProfileWarning.Background = Brushes.LightPink;
                textBlockProfileWarning.Text = "Not enough rights to perform this task.";
                return;
            }


            if (toggleButtonAddNewProfile == 0)
            {
             
                    toggleButtonAddNewProfile = 1;
                    textBlockUserNotifyMessage.Visibility = Visibility.Visible;
                    textBlockUserNotifyMessage.Text = "Add New User";
                    btnAddNew.Content = "Generate New Profile";

                    //Change to editable mode
                    textBoxGPFirstName.IsReadOnly = false;
                    textBoxGPFirstName.Text = "";
                    textBoxGPLastName.IsReadOnly = false;
                    textBoxGPLastName.Text = "";
                    textBoxGPContact.IsReadOnly = false;
                    textBoxGPContact.Text = "";
                    textBoxGPEmail.IsReadOnly = false;
                    textBoxGPEmail.Text = "";

                    //Clear ID
                    textBoxGPUserID.Text = "";
                    textBoxUserIDRights.Text = "";

                    //Change to Edit mode for privileges
                    textBoxUserDept.IsReadOnly = false;
                    cmbUserRole.IsEnabled = true;

                    //Change to Read only mode for password..since password will be auto-generated initially
                    passwordBoxConfirmNewPass.IsEnabled = false;
                    passwordBoxNewPass.IsEnabled = false;
                    passwordBoxOldPassword.IsEnabled = false;
                    btnChangePassword.IsEnabled = false;

                    //Change  privilege buttons to disabled
                    btnEditPrivileges.IsEnabled = false;
                    btnManageOtherUsers.IsEnabled = false;

                    textBlockProfileWarning.Visibility = Visibility.Visible;
                    textBlockProfileWarning.Text = "Note ! User ID and Password will be generated automatically";
              
            }

            else if (toggleButtonAddNewProfile == 1)
            {

                if (!(checkForValidFields()))
                {
                    // There seems to be a problem.
                    return;
                }
                
                //Change to Read Only mode
                textBoxGPFirstName.IsReadOnly = true;
                textBoxGPLastName.IsReadOnly = true;
                textBoxGPContact.IsReadOnly = true;
                textBoxGPEmail.IsReadOnly = true;

                //Change to Read only mode for privileges
                textBoxUserDept.IsReadOnly = true;
                cmbUserRole.IsEnabled = false;

                //Change button content
                toggleButtonAddNewProfile = 0;
                textBlockUserNotifyMessage.Visibility = Visibility.Visible;
                textBlockProfileWarning.Visibility = Visibility.Hidden;
                btnAddNew.Content = "Add New Profile";

                //Change to Edit mode for password..since password will be auto-generated initially
                passwordBoxConfirmNewPass.IsEnabled = true;
                passwordBoxNewPass.IsEnabled = true;
                passwordBoxOldPassword.IsEnabled = true;
                btnChangePassword.IsEnabled = true;

                //Change  privilege buttons to enabled
                btnEditPrivileges.IsEnabled = true;
                btnManageOtherUsers.IsEnabled = true;

                //Store values
                firstName = textBoxGPFirstName.Text.Trim();
                lastName = textBoxGPLastName.Text.Trim();

                username = firstName + " " + lastName;

                usercontact = textBoxGPContact.Text;
                useremail = textBoxGPEmail.Text;
                userrole = cmbUserRole.Text;
                userroleid = textBoxRoleID.Text;
                userdept = textBoxUserDept.Text;

                userlastupdate = DateTime.Now;
                userdateCreated = DateTime.Now;

                // Get all the user ID so that generated ID can be comapred to find if duplicate exists
                try
                {
                    UserIDTA.Fill(userIDDataSet.UserIDDataTable);
                }
                catch
                {
                    LogFile.Log("Error: Could not get all userIDs in table");
                    //Could not get data
                }
                //Generate ID and password

                int rowCount;
                do
                {
                    idANDpassword = generateIDPass(firstName, lastName);
                    idPassSplit = idANDpassword.Split(' ');
                    userid = idPassSplit[0];
                    userpass = idPassSplit[1];

                    DataRow[] resultRow = userIDDataSet.UserIDDataTable.Select("user_id = '" + userid + "'");
                    rowCount = resultRow.Count();
                } while (rowCount != 0);

                //Now encrypt the password here:
                euserPassword = myEncryption.Encrypt(userpass, true);

                this.Cursor = Cursors.Wait;

                try //Insert data in two tables ACL and ACL login
                {
                    UserInfoTA.InsertACL(userid, username, userdept, userrole, userroleid, usercontact, useremail, userdateCreated, userlastupdate);
                    UserInfoTA.InsertACLlogin(userid, userroleid, euserPassword, userdateCreated);

                    textBlockUserNotifyMessage.Text = "Profile Generated !";
                    textBlockProfileWarning.Visibility = Visibility.Visible;
                    textBlockProfileWarning.Text = @"Please note the user ID and Password.         User ID: " + userid + " Password: " + userpass;
                    textBlockProfileWarning.Background = Brushes.LightGreen;
                    textBoxGPUserID.Text = userid;
                    textBoxUserIDRights.Text = userid;


                    // Log ,email, SMS
                    LogFile.Log("User profile Added: Record with User ID- " + userid.ToString() + " has been added !");
                    if (AlertNotificationSettings.userAdded == true)
                    {
                        this.Cursor = Cursors.Wait;
                        if (AlertNotificationSettings.sendEmailUser == true)
                        {
                            HATrakaMain.emailContent = "User profile Added: Record with User ID- " + userid.ToString() + " has been added !";
                            sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                        }
                        if (AlertNotificationSettings.sendSMSUser == true)
                        {
                            SMSClass.sendSMS("User profile Added: Record with User ID- " + userid.ToString() + " has been added !", LoginWindow.user_contact);

                            if (AlertNotificationSettings.sendSMSAdmin == true)
                            {
                                SMSClass.sendSMS("User profile Added: Record with User ID- " + userid.ToString() + " has been added !", HATrakaMain.superAdminSMS);
                            }
                        }

                    }


                }
                catch (System.Exception ex)
                {
                    LogFile.Log("Error: Could not insert new data in the database");
                    textBlockUserNotifyMessage.Text = "Profile generation failed !";
                    MessageBox.Show("Adding failed: " + ex.ToString());
                }

                finally
                {
                    this.Cursor = Cursors.Arrow;
                }

            }
        }

        /// <summary>
        /// Checks the user email according to standard regex pattern
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <returns></returns>
        private bool checkUserEmail(string userEmail)
        {

            // See whether email is correct according to pattern.
            if (Regex.IsMatch(userEmail, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generates the ID pass.
        /// generates a random user ID according to specific rules
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <returns></returns>
        private string generateIDPass(string firstName, string lastName)
        {
            string newID = "";
            string newPass = "";
            string idANDPass = "";
            int rNum;
            char aRnd;

            // Generate ID
            newID = firstName.Substring(0, 1).ToUpper() + lastName.Substring(0, 1).ToUpper();
            Random rnd = new Random();
            
            //Generate two random alphabets
            rNum = rnd.Next(0, 26);
            aRnd = (char)('A' + rNum);
            newID += aRnd.ToString();

            rNum = rnd.Next(0, 26);
            aRnd = (char)('A' + rNum);
            newID += aRnd.ToString();
            //
            //Add a random 4 digit number
            rNum = rnd.Next(1000, 9999);
            newID += rNum.ToString();
            //////////////////////////////////////

            //Generate password
            newPass = firstName.Substring(0, 1).ToLower() + lastName.Substring(0, 1).ToLower();
            newPass += "user12";

            //Now combine
            idANDPass = newID.Trim() + " " + newPass.Trim();
            return idANDPass;
        }


        private bool checkForValidFields()
        {
            //Check for nulls
            //name, role, roleid
            if ((string.IsNullOrEmpty(textBoxGPFirstName.Text)) || (string.IsNullOrEmpty(textBoxGPLastName.Text)))
            {
                textBlockProfileWarning.Visibility = Visibility.Visible;
                textBlockProfileWarning.Text = "Error ! First name and Last name cannot be blank";
                return false;
            }

            if (!Regex.IsMatch(textBoxGPFirstName.Text, @"^[a-zA-Z][\p{L} \.'\-]+$"))
            {
                textBlockProfileWarning.Visibility = Visibility.Visible;
                textBlockProfileWarning.Text = "Error ! First name should contain alphabets (atleast two)";
                return false;
            }

            if (!Regex.IsMatch(textBoxGPLastName.Text, @"^[a-zA-Z][\p{L} \.'\-]+$"))
            {
                textBlockProfileWarning.Visibility = Visibility.Visible;
                textBlockProfileWarning.Text = "Error ! Last name should also contain alphabets (atleast two)";
                return false;
            }

            if (!string.IsNullOrEmpty(textBoxGPEmail.Text))
            {
                if (!(checkUserEmail(textBoxGPEmail.Text.ToString())))
                {
                    textBlockProfileWarning.Visibility = Visibility.Visible;
                    textBlockProfileWarning.Text = "Error ! Email should be in proper format";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(textBoxGPContact.Text))
            {
                if (!(SMSClass.checkMobileNumber(textBoxGPContact.Text.Trim())))
                {
                    textBlockProfileWarning.Visibility = Visibility.Visible;
                    textBlockProfileWarning.Text = "Error ! Email should be in proper format";
                    return false;
                }
            }


            if (string.IsNullOrEmpty(cmbUserRole.Text))
            {
                textBlockProfileWarning.Visibility = Visibility.Visible;
                textBlockProfileWarning.Text = "Error ! User must be assigned a role";
                return false;
            }


            return true;
        }

        /// <summary>
        /// Handles the Click event of the btnDeleteProfile control.
        /// Deletes user profile
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnDeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            

            string original_userid = "";
            original_userid = textBoxUserIDRights.Text;

            textBlockRoles.Visibility = Visibility.Visible;
            textBlockRoles.Background = Brushes.LightBlue;
            textBlockRoles.Text = "Note: The roles and privileges of users can only be changed by admins";

            //check for ADMIN or ADMIN02 roles
            // CHECK for user ROLE here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockRoles.Visibility = Visibility.Visible;
                textBlockRoles.Background = Brushes.LightPink;
                textBlockRoles.Text = "Not enough rights to perform this task.";
                
                return;
            }


            if (original_userid.CompareTo(LoginWindow.userID) == 0)
            {
                textBlockuserRolesNotifyMessage.Text = "Cannot delete profile!"; // Self profile
                btnDeleteProfile.IsEnabled = false;
                return;
            }


            this.Cursor = Cursors.Wait;
            try
            {
                UserIDTA.DeleteRecord(original_userid);
                textBlockuserRolesNotifyMessage.Text = "Record Deleted!";
                btnDeleteProfile.IsEnabled = false;
                textBoxUserIDRights.Text = "";
                cmbUserRole.SelectedIndex = -1;
                textBoxRoleID.Text = "";
                textBoxUserDept.Text = "";

                // Log ,email, SMS
                LogFile.Log("User record Deleted: Record with User ID- " + original_userid.ToString() + " has been deleted !");
                if (AlertNotificationSettings.userDeleted == true)
                {
                    this.Cursor = Cursors.Wait;
                    if (AlertNotificationSettings.sendEmailUser == true)
                    {
                        HATrakaMain.emailContent = "User profile deleted: Record with User ID- " + original_userid.ToString() + " has been deleted !";
                        sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                    }
                    if (AlertNotificationSettings.sendSMSUser == true)
                    {
                        SMSClass.sendSMS("User profile deleted: Record with User ID- " + original_userid.ToString() + " has been deleted !", LoginWindow.user_contact);

                        if (AlertNotificationSettings.sendSMSAdmin == true)
                        {
                            SMSClass.sendSMS("User profile deleted: Record with User ID- " + original_userid.ToString() + " has been deleted !", HATrakaMain.superAdminSMS);
                        }
                    }

                }

            }

            catch (System.Exception ex)
            {
                LogFile.Log("Error: Could not delete user profile");
                btnDeleteProfile.IsEnabled = false;
                textBlockuserRolesNotifyMessage.Text = "Delete operation Failed!";
                MessageBox.Show("Delete failed: " + ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

    }
}
