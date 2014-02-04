using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
    /// Interaction logic for ReaderMapProfile.xaml
    /// </summary>
    public partial class ReaderMapProfile : Window
    {
        public ReaderMapProfile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the readerMapManageWindow control.
        /// Intializer variables and data connection
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void readerMapManageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Credentials set from the loginWindow 

        }

        //Initialize variables
        short toggleButtonEditReader = 0;
        short toggleButtonAddNewReader = 0;

        private string reader_id, floor_id, location_id, map_floor,asset_info, reader_info, map_comments;

        private short rooms_pfloor, assets_pfloor;
        

        //Instantiate dataset and TA
        ztATdbLocalDataSet1 readerInfoDS = new ztATdbLocalDataSet1();
        ztATdbLocalDataSet1TableAdapters.readerInfoDataTableTableAdapter readerInfoTAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.readerInfoDataTableTableAdapter();

   
        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// Closes the window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the LostFocus event of the textBoxSearchBoxTag control.
        /// Search box UI feature
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchBoxTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSearchBoxTag.Text.ToString()))
            {
                textBoxSearchBoxTag.Background = Brushes.White;
                textBoxSearchBoxTag.Foreground = Brushes.LightSlateGray;
                textBoxSearchBoxTag.BorderBrush = Brushes.LightSlateGray;
                textBoxSearchBoxTag.Text = "<Enter reader ID or floor ID>";

            }
        }

        /// <summary>
        /// Handles the GotFocus event of the textBoxSearchBoxTag control.
        /// Search box UI feature
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxSearchBoxTag_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxSearchBoxTag.Text.CompareTo("<Enter reader ID or floor ID>") == 0)
            {
                textBoxSearchBoxTag.Background = Brushes.LightYellow;
                textBoxSearchBoxTag.Foreground = Brushes.Black;
                textBoxSearchBoxTag.BorderBrush = Brushes.YellowGreen;
                textBoxSearchBoxTag.Text = "";
            }
        }

        /// <summary>
        /// Handles the Click event of the btnsearchBoxTag control.
        /// Search tag ID /userID
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnsearchBoxTag_Click(object sender, RoutedEventArgs e)
        {
            string searchID = "";
            searchID = textBoxSearchBoxTag.Text;

            if ((string.IsNullOrEmpty(searchID)) || (searchID.CompareTo("<Enter reader ID or floor ID>") == 0))
            {
                textBlockReaderMapsInfoNotifyMessage.Visibility = Visibility.Visible;
                textBlockReaderMapsInfoNotifyMessage.Text = "Enter a search term !";
                return;

            }


            // Wild char search
            searchID = "%" + searchID + "%";

            string mapFloors;
            int rowCount =0;
            textBlockReaderMapsInfoNotifyMessage.Visibility = Visibility.Visible;

            //Temp table to store data of reader info
            ztATdbLocalDataSet1TableAdapters.readerInfoDataTableTableAdapter readerInfoTableAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.readerInfoDataTableTableAdapter();
            ztATdbLocalDataSet1.readerInfoDataTableDataTable readerDTableDetail = new ztATdbLocalDataSet1.readerInfoDataTableDataTable();

            this.Cursor = Cursors.Wait;
            try
            {
                rowCount = readerInfoTableAdpt.Fill(readerDTableDetail, searchID);

                if (rowCount > 0)
                {
                    textBlockReaderMapsInfoNotifyMessage.Text = "Reader record found !";

                    textBoxReaderID.Text = readerDTableDetail.Rows[0]["reader_id"].ToString().TrimEnd();
                    textBoxFloorID.Text = readerDTableDetail.Rows[0]["floor_id"].ToString().TrimEnd();
                    textBoxLocationID.Text = readerDTableDetail.Rows[0]["location_id"].ToString().TrimEnd();
                    mapFloors = readerDTableDetail.Rows[0]["map_floor"].ToString().TrimEnd();
                    cmbMapFloors.Text = mapFloors;
                    textBoxAssetInfoDescr.Text = readerDTableDetail.Rows[0]["asset_info"].ToString().TrimEnd();
                    textBoxReaderInfo.Text = readerDTableDetail.Rows[0]["reader_info"].ToString().TrimEnd();
                    textBoxRoomsPerFloor.Text = readerDTableDetail.Rows[0]["rooms_pfloor"].ToString().TrimEnd();
                    textBoxAssetsPerFloor.Text = readerDTableDetail.Rows[0]["assets_pfloor"].ToString().TrimEnd();
                    textBoxMapComments.Text = readerDTableDetail.Rows[0]["map_comments"].ToString().TrimEnd();

                    //Enable Delete button
                    btnDeleteReader.IsEnabled = true;
                }
                else
                {
                    textBlockReaderMapsInfoNotifyMessage.Text = "No record found !";
                }

            }
            catch
            {
                LogFile.Log("Error: Reader and Map Info search failed");
                textBlockReaderMapsInfoNotifyMessage.Text = "Search Failed!";
                textBlockSearchWarning.Visibility = Visibility.Visible; 
                textBlockSearchWarning.Text =  "Search failed: Connection with DB problem ! ";

            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                readerDTableDetail.Dispose();
            }
        }

        /// <summary>
        /// Checks the nulls
        /// </summary>
        /// <returns></returns>
        private bool checkNullsNRegex()
        {
            //Check for nulls

            if (string.IsNullOrEmpty(textBoxFloorID.Text))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Floor ID cannot be blank";
                return false;
            }

            //Location ID
            if (string.IsNullOrEmpty(textBoxLocationID.Text))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Location ID cannot be blank";
                return false;
            }

            if (!Regex.IsMatch(textBoxLocationID.Text, @"^[a-zA-Z0-9]+$"))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Error ! Asset Location cannot contain special characters";
                return false;
            }

            //Map Floor category
            if (cmbMapFloors.SelectedIndex == -1)
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Floor cannot be blank";
                return false;
            }

            //Asset descr
            if (string.IsNullOrEmpty(textBoxAssetInfoDescr.Text))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset Description cannot be blank";
                return false;
            }



            //Assets per floor should be numbers if filled
            if (!(string.IsNullOrEmpty(textBoxAssetsPerFloor.Text)))
            {
                if (!Regex.IsMatch(textBoxAssetsPerFloor.Text, @"^[\d]+$"))
                {
                    textBlockSearchWarning.Visibility = Visibility.Visible;
                    textBlockSearchWarning.Text = "Error ! Assets per floor should contain only numbers";
                    return false;
                }
            }

            if (!(string.IsNullOrEmpty(textBoxRoomsPerFloor.Text)))
            {
                if (!Regex.IsMatch(textBoxRoomsPerFloor.Text, @"^[\d]+$"))
                {
                    textBlockSearchWarning.Visibility = Visibility.Visible;
                    textBlockSearchWarning.Text = "Error ! Rooms per floor should contain only numbers";
                    return false;
                }
            }

            //If reached here then all conditions are good
            return true;

        }

        /// <summary>
        /// Stores the values.
        /// </summary>
        private void storeValues()
        {
            //Store values

            reader_id = textBoxReaderID.Text.ToString();
            floor_id = textBoxFloorID.Text.Trim();
            location_id = textBoxLocationID.Text;
            map_floor = cmbMapFloors.Text;
            asset_info = textBoxAssetInfoDescr.Text;
            reader_info = textBoxReaderInfo.Text;

            short.TryParse(textBoxAssetsPerFloor.Text, out assets_pfloor);
            short.TryParse(textBoxRoomsPerFloor.Text, out rooms_pfloor);
            
            map_comments = textBoxMapComments.Text.ToString();
         
        }

        /// <summary>
        /// Generates the new asset ID.
        /// </summary>
        /// <returns></returns>
        private string generateNewReaderID()
        {
            string newReaderID = "";         
            int rNum;

            //Generate RF (two alphabets)
            newReaderID += "RF";

            //Add a random 2 or 3 digit number
            Random rnd2 = new Random();
            rNum = rnd2.Next(10, 255);

            if (rNum.ToString().Length == 2)
            {
                newReaderID += "0" + rNum.ToString();
            }
            else
            {
                newReaderID += rNum.ToString();
            }

            return newReaderID;

        }

        /// <summary>
        /// Clears all fields.
        /// </summary>
        private void clearAllFields()
        {
            //Change to readonly mode and clear all fields
            //textBoxReaderID.IsReadOnly = true;
            textBoxReaderID.Text = "";

            textBoxFloorID.IsReadOnly = true;
            textBoxFloorID.Text = "";

            textBoxLocationID.IsReadOnly = true;
            textBoxLocationID.Text = "";

            cmbMapFloors.IsEnabled = false;
            cmbMapFloors.SelectedIndex = -1;

            textBoxAssetInfoDescr.IsReadOnly = true;
            textBoxAssetInfoDescr.Text = "";

            textBoxReaderInfo.IsReadOnly = true;
            textBoxReaderInfo.Text = "";

            textBoxRoomsPerFloor.IsReadOnly = true;
            textBoxRoomsPerFloor.Text = "";

            textBoxAssetsPerFloor.IsReadOnly = true;
            textBoxAssetsPerFloor.Text = "";

            textBoxMapComments.IsReadOnly = true;
            textBoxMapComments.Text = "";

            //Buttons
        
            btnDeleteReader.IsEnabled = false;

            //Image:
            imageReaderPic.Source = new BitmapImage(new Uri(@"./IconImages/RFreader.png", UriKind.Relative)); 

        }

        /// <summary>
        /// Handles the Click event of the btnDeleteReader control.
        /// Deletes the reader map record
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnDeleteReader_Click(object sender, RoutedEventArgs e)
        {

            string original_readerID = "";

            textBlockSearchWarning.Background = Brushes.LightPink;
            ///////////////////////
            // CHECK for USER rights
            // CHECK for user ROLE ID here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Not enough rights to perform this task.";

                return;
            }
            ////////////////

            original_readerID = textBoxReaderID.Text;

            this.Cursor = Cursors.Wait;
            try
            {
                readerInfoTAdpt.DeleteReaderT(original_readerID);
                textBlockReaderMapsInfoNotifyMessage.Text = "Record Deleted!";

                //Clear all fields.
                clearAllFields();

                // Log ,email, SMS
                LogFile.Log("Reader record Deleted: Record with Reader ID- " + original_readerID.ToString() + " has been deleted !");
                if (AlertNotificationSettings.readerDeleted == true)
                {
                    this.Cursor = Cursors.Wait;
                    if (AlertNotificationSettings.sendEmailUser == true)
                    {
                        
                        HATrakaMain.emailContent = "Reader record Deleted: Record with Reader ID- " + original_readerID.ToString() + " has been deleted !";
                        sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                    }
                    if (AlertNotificationSettings.sendSMSUser == true)
                    {
                        
                        SMSClass.sendSMS("Reader Deleted: Record with Reader ID- " + original_readerID.ToString() + " has been deleted !", LoginWindow.user_contact);

                        if (AlertNotificationSettings.sendSMSAdmin == true)
                        {
                            SMSClass.sendSMS("Reader Deleted: Record with Reader ID- " + original_readerID.ToString() + " has been deleted !", HATrakaMain.superAdminSMS);
                        }
                    }

                }

            }

            catch (System.Exception ex)
            {
                //btnDeleteReader.IsEnabled = false;
                LogFile.Log("Error: Deleting asset record failed");
                textBlockReaderMapsInfoNotifyMessage.Text = "Delete operation Failed!";
                MessageBox.Show("Delete failed possibly due to database connection issue: " + ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }

        }

        /// <summary>
        /// Handles the Click event of the btnAddNewReader control.
        /// Adds new reader and map record to the database
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddNewReader_Click(object sender, RoutedEventArgs e)
        {
           
            textBlockSearchWarning.Background = Brushes.LightPink;

            // CHECK for user ROLE ID here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Not enough rights to perform this task.";
                return;
            }

            if (toggleButtonAddNewReader == 0)
            {

                toggleButtonAddNewReader = 1;
                btnEditReaderInfo.IsEnabled = false;

                textBlockReaderMapsInfoNotifyMessage.Visibility = Visibility.Visible;
                textBlockReaderMapsInfoNotifyMessage.Text = "Add New Reader";
                btnAddNewReader.Content = "Save New Reader";

                //Change to editable mode
                //textBoxReaderID.IsReadOnly = true;
                textBoxReaderID.Text = "";

                //textBoxFloorID.IsReadOnly = false;
                textBoxFloorID.Text = "";

                textBoxLocationID.IsReadOnly = false;
                textBoxLocationID.Text = "";

                cmbMapFloors.IsEnabled = true;
                cmbMapFloors.SelectedIndex = -1;

                textBoxAssetInfoDescr.IsReadOnly = false;
                textBoxAssetInfoDescr.Text = "";

                textBoxReaderInfo.IsReadOnly = false;
                textBoxReaderInfo.Text = "";

                textBoxRoomsPerFloor.IsReadOnly = false;
                textBoxRoomsPerFloor.Text = "";

                textBoxAssetsPerFloor.IsReadOnly = false;
                textBoxAssetsPerFloor.Text = "";

                textBoxMapComments.IsReadOnly = false;
                textBoxMapComments.Text = "";

                //Buttons
               
                btnDeleteReader.IsEnabled = false;

                //Image:
                imageReaderPic.Source = new BitmapImage(new Uri(@"./IconImages/RFreader.png", UriKind.Relative));

                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Background = Brushes.LightBlue;
                textBlockSearchWarning.Text = "Note ! Reader ID will be generated automatically";

            }
            else if (toggleButtonAddNewReader == 1)
            {

                if (!(checkNullsNRegex()))
                {
                    //problem
                    return;

                }

                //Change to Read Only mode
                textBoxFloorID.IsReadOnly = true;
                textBoxLocationID.IsReadOnly = true;
                cmbMapFloors.IsEnabled = false;
                textBoxAssetInfoDescr.IsReadOnly = true;
                textBoxReaderInfo.IsReadOnly = true;
                textBoxRoomsPerFloor.IsReadOnly = true;
                textBoxAssetsPerFloor.IsReadOnly = true;
                textBoxMapComments.IsReadOnly = true;

                toggleButtonAddNewReader = 0;
                btnEditReaderInfo.IsEnabled = true;

                textBlockReaderMapsInfoNotifyMessage.Visibility = Visibility.Visible;
                btnAddNewReader.Content = "Add New Reader";

                //Store values:
                storeValues();

                // Get all the reader IDs so that generated ID can be comapred to find if duplicate exists
                ztATdbLocalDataSet1.readerInfoDataTableDataTable rdTempT = new ztATdbLocalDataSet1.readerInfoDataTableDataTable();


                this.Cursor = Cursors.Wait;

                string newReader_id;
                

                do
                {
                    newReader_id = generateNewReaderID(); // Eg: RF144
                    //use this in TA

                    // But first check for unique ID as it is a PK
                    try
                    {
                        readerInfoTAdpt.FillReaderID(rdTempT, newReader_id);
                    }
                    catch
                    {
                        LogFile.Log("Error: Could not get all Reader IDs ");
                        //Could not get data
                    }


                } while (rdTempT.Count > 0);

               
                try
                {
                    readerInfoTAdpt.InsertReaderT(newReader_id, location_id, floor_id, map_floor, asset_info, reader_info);
                    //readerInfoTAdpt.InsertMapT(floor_id, map_floor, rooms_pfloor, assets_pfloor, map_comments);

                    textBlockReaderMapsInfoNotifyMessage.Text = "Asset Added !";
                    textBlockSearchWarning.Visibility = Visibility.Hidden;

                    //Enable the delete button
                    btnDeleteReader.IsEnabled = true;

                    textBoxReaderID.Text = newReader_id;

                    // Log ,email, SMS
                    LogFile.Log("Reader Added: Record with Reader ID- " + newReader_id.ToString() + " has been added !");
                    if (AlertNotificationSettings.readerAdded == true)
                    {
                        this.Cursor = Cursors.Wait;
                        if (AlertNotificationSettings.sendEmailUser == true)
                        {
                            HATrakaMain.emailContent = "Reader Added: Record with Reader ID- " + newReader_id.ToString() + " has been added !";
                            sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                        }
                        if (AlertNotificationSettings.sendSMSUser == true)
                        {
                            
                            SMSClass.sendSMS("Reader Added: Record with Reader ID- " + newReader_id.ToString() + " has been added !", LoginWindow.user_contact);

                            if (AlertNotificationSettings.sendSMSAdmin == true)
                            {
                                SMSClass.sendSMS("Reader Added: Record with Reader ID- " + newReader_id.ToString() + " has been added !", HATrakaMain.superAdminSMS);
                            }
                        }

                    }




                }
                catch (System.Exception ex)
                {
                    LogFile.Log("Error: Asset adding failed ");
                    textBlockReaderMapsInfoNotifyMessage.Text = "Asset generation failed !";
                    MessageBox.Show("Adding failed: " + ex.ToString());
                }

                finally
                {
                    this.Cursor = Cursors.Arrow;
                }

            }

        }

        /// <summary>
        /// Handles the Click event of the btnEditReaderInfo control.
        /// Update and Edit reader map information
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnEditReaderInfo_Click(object sender, RoutedEventArgs e)
        {
            textBlockSearchWarning.Visibility = Visibility.Hidden;
            textBlockSearchWarning.Background = Brushes.LightPink;
            ///////////////////////
            // CHECK for USER rights
            // CHECK for user ROLE ID here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Not enough rights to perform this task.";
                return;
            }
            ////////////////

            if (toggleButtonEditReader == 0)
            {
                if (string.IsNullOrEmpty(textBoxReaderID.Text.Trim()))
                {
                    textBlockSearchWarning.Visibility = Visibility.Visible;
                    textBlockSearchWarning.Text = "Find a reader record to edit !";
                    return;

                }
                toggleButtonEditReader = 1;
                btnAddNewReader.IsEnabled = false;

                textBlockReaderMapsInfoNotifyMessage.Visibility = Visibility.Visible;
                textBlockReaderMapsInfoNotifyMessage.Text = "Edit Mode ON";
                btnEditReaderInfo.Content = "Save Reader Info";

                //Change to editable mode
                //textBoxReaderID.IsReadOnly = false;
                //textBoxFloorID.IsReadOnly = false;
                textBoxLocationID.IsReadOnly = false;
                cmbMapFloors.IsEnabled = true;
                textBoxAssetInfoDescr.IsReadOnly = false;
                textBoxReaderInfo.IsReadOnly = false;
                textBoxRoomsPerFloor.IsReadOnly = false;
                textBoxAssetsPerFloor.IsReadOnly = false;
                textBoxMapComments.IsReadOnly = false;
  
                //Buttons
                
                btnDeleteReader.IsEnabled = false;

            }
            else if (toggleButtonEditReader == 1)
            {
                if (!(checkNullsNRegex()))
                {
                    //problem
                    return;

                }

                //Change to Read Only mode
                textBoxFloorID.IsReadOnly = true;
                textBoxLocationID.IsReadOnly = true;
                cmbMapFloors.IsEnabled = false;
                textBoxAssetInfoDescr.IsReadOnly = true;
                textBoxReaderInfo.IsReadOnly = true;
                textBoxRoomsPerFloor.IsReadOnly = true;
                textBoxAssetsPerFloor.IsReadOnly = true;
                textBoxMapComments.IsReadOnly = true;

                toggleButtonEditReader = 0;
                btnAddNewReader.IsEnabled = true;

                textBlockReaderMapsInfoNotifyMessage.Visibility = Visibility.Visible;

                btnEditReaderInfo.Content = "Edit Reader Info";


                // Store values:
                storeValues();
                this.Cursor = Cursors.Wait;
                try
                {
                    readerInfoTAdpt.UpdateReaderT(location_id, floor_id, map_floor, asset_info, reader_info, reader_id);
                    readerInfoTAdpt.UpdateMapT(map_floor, rooms_pfloor, assets_pfloor, map_comments, floor_id);

                    textBlockReaderMapsInfoNotifyMessage.Text = "Reader Info saved !";
                    textBlockSearchWarning.Visibility = Visibility.Hidden;

                    //Enable delete button
                    btnDeleteReader.IsEnabled = true;

                    // Log ,email, SMS
                    LogFile.Log("Reader Updated: Record with Reader ID- " + reader_id.ToString() + " has been updated !");
                    if (AlertNotificationSettings.readerUpdates == true)
                    {
                        this.Cursor = Cursors.Wait;
                        if (AlertNotificationSettings.sendEmailUser == true)
                        {
                            HATrakaMain.emailContent = "Reader Updated: Record with Reader ID- " + reader_id.ToString() + " has been updated !";
                            sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                        }
                        if (AlertNotificationSettings.sendSMSUser == true)
                        {
                            
                            SMSClass.sendSMS("Reader Updated: Record with Reader ID- " + reader_id.ToString() + " has been updated !", LoginWindow.user_contact);

                            if (AlertNotificationSettings.sendSMSAdmin == true)
                            {
                                SMSClass.sendSMS("Reader Updated: Record with Reader ID- " + reader_id.ToString() + " has been updated !", HATrakaMain.superAdminSMS);
                            }
                        }

                    }

                }
                catch (System.Exception ex)
                {
                    LogFile.Log("Error: Reader update failed");
                    textBlockReaderMapsInfoNotifyMessage.Text = "Reader update failed !";
                    MessageBox.Show("Update failed: " + ex.ToString());
                }
                finally
                {
                    this.Cursor = Cursors.Arrow;
                }

            }

        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbMapFloors control.
        /// Populate the floorID text field on selection changes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void cmbMapFloors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string mapFloor = "";
            //mapFloor = cmbMapFloors.Text;

            ztATdbLocalDataSet1.readerInfoDataTableDataTable readerTab = new ztATdbLocalDataSet1.readerInfoDataTableDataTable();

            this.Cursor = Cursors.Wait;

            if (cmbMapFloors.SelectedIndex == -1)
            {
                textBoxFloorID.Text = "";
            }
            else if (cmbMapFloors.SelectedIndex == 0)
            {
                textBoxFloorID.Text = "F1";
                
                mapFloor = "First";
                try
                {
                    readerInfoTAdpt.FillMapFloor(readerTab, mapFloor);
                    textBoxRoomsPerFloor.Text = readerTab.Rows[0]["rooms_pfloor"].ToString().TrimEnd();
                    textBoxAssetsPerFloor.Text = readerTab.Rows[0]["assets_pfloor"].ToString().TrimEnd();
                    textBoxMapComments.Text = readerTab.Rows[0]["map_comments"].ToString().TrimEnd();


                }

                catch
                {

                }
            }
            else if (cmbMapFloors.SelectedIndex == 1)
            {
                textBoxFloorID.Text = "F2";
                mapFloor = "Second";
                try
                {
                    readerInfoTAdpt.FillMapFloor(readerTab, mapFloor);
                    textBoxRoomsPerFloor.Text = readerTab.Rows[0]["rooms_pfloor"].ToString().TrimEnd();
                    textBoxAssetsPerFloor.Text = readerTab.Rows[0]["assets_pfloor"].ToString().TrimEnd();
                    textBoxMapComments.Text = readerTab.Rows[0]["map_comments"].ToString().TrimEnd();


                }

                catch
                {

                }
            }
            else if (cmbMapFloors.SelectedIndex == 2)
            {
                textBoxFloorID.Text = "F3";
                mapFloor = "Third";
                try
                {
                    readerInfoTAdpt.FillMapFloor(readerTab, mapFloor);
                    textBoxRoomsPerFloor.Text = readerTab.Rows[0]["rooms_pfloor"].ToString().TrimEnd();
                    textBoxAssetsPerFloor.Text = readerTab.Rows[0]["assets_pfloor"].ToString().TrimEnd();
                    textBoxMapComments.Text = readerTab.Rows[0]["map_comments"].ToString().TrimEnd();

                }

                catch
                {

                }
            }

            this.Cursor = Cursors.Arrow;
        }


        
    }
}
