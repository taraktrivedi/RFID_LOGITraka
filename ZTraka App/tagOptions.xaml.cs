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
using System.IO;
using System.Text.RegularExpressions;



namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for tagOptions.xaml
    /// </summary>
    public partial class tagOptions : Window
    {

        //Initialize variables
        short toggleButtonEditAsset = 0;
        short toggleButtonAddNewAsset = 0;
        private byte[] byteImage, asset_image;

        private string asset_id, tag_id, asset_lastloc, asset_status, asset_alarms, asset_location, asset_category,
                    asset_desc, asset_model, asset_comments;
        private DateTime asset_lastchecked, asset_tag_created, tag_expiry, asset_maint_due;
        private short asset_quantity;
        private decimal asset_value;

        //Instantiate dataset and TA
        ztATdbLocalDataSet1 assetInfoDS = new ztATdbLocalDataSet1();
        ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter assetInfoTAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter();


        /// <summary>
        /// Constructor of the <see cref="tagOptions" /> class.
        /// </summary>
        public tagOptions()
        {

            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the tgOptions control.
        /// Check/Set user credentials
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tgOptions_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

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
                textBoxSearchBoxTag.Text = "<Enter asset ID or tag ID>";

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
            if (textBoxSearchBoxTag.Text.CompareTo("<Enter asset ID or tag ID>") == 0)
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

            if ((string.IsNullOrEmpty(searchID)) || (searchID.CompareTo("<Enter asset ID or tag ID>") == 0))
            {
                textBlockTagNotifyMessage.Visibility = Visibility.Visible;
                textBlockTagNotifyMessage.Text = "Enter a search term !";
                return;
 
            }

            // Wild char search
            searchID = "%" + searchID + "%";

            string cmbAssetStat, cmbAssetCat;
            int rowCount;
            textBlockTagNotifyMessage.Visibility = Visibility.Visible;

            //Temp table to store data of assets
            ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter assetInfoTableAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter();
            ztATdbLocalDataSet1.AssetInfoDataTableDataTable assetDTableDetail = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();

            this.Cursor = Cursors.Wait;
            try
            {
                rowCount = assetInfoTableAdpt.FillBy(assetDTableDetail, searchID);

                if (rowCount > 0)
                {
                    textBlockTagNotifyMessage.Text = "Asset record found !";

                    textBoxAssetID.Text = assetDTableDetail.Rows[0]["asset_id"].ToString().TrimEnd();
                    textBoxTagID.Text = assetDTableDetail.Rows[0]["tag_id"].ToString().TrimEnd();
                    textBoxAssetLocation.Text = assetDTableDetail.Rows[0]["asset_location"].ToString().TrimEnd();
                    cmbAssetCat = assetDTableDetail.Rows[0]["asset_category"].ToString().TrimEnd();
                    cmbAssetCategory.Text = cmbAssetCat;
                    textBoxAssetDescr.Text = assetDTableDetail.Rows[0]["asset_desc"].ToString().TrimEnd();
                    textBoxAssetModel.Text = assetDTableDetail.Rows[0]["asset_model"].ToString().TrimEnd();
                    textBoxAssetTagCreated.Text = assetDTableDetail.Rows[0]["asset_tag_created"].ToString().TrimEnd();
                    textBoxAssetQuantity.Text = assetDTableDetail.Rows[0]["asset_quantity"].ToString().TrimEnd();
                    textBoxAssetValue.Text = assetDTableDetail.Rows[0]["asset_value"].ToString().TrimEnd();
                    cmbAssetStat = assetDTableDetail.Rows[0]["asset_status"].ToString().TrimEnd();
                    cmbAssetStatus.Text = cmbAssetStat;
                    textBoxAssetLastChecked.Text = assetDTableDetail.Rows[0]["asset_lastchecked"].ToString().TrimEnd();
                    textBoxAssetLastLocation.Text = assetDTableDetail.Rows[0]["asset_lastloc"].ToString().TrimEnd();
                    textBoxTagExpiry.Text = assetDTableDetail.Rows[0]["tag_expiry"].ToString().TrimEnd();
                    textBoxAssetMaintDue.Text = assetDTableDetail.Rows[0]["asset_maint_due"].ToString().TrimEnd();
                    textBoxAssetAlarms.Text = assetDTableDetail.Rows[0]["asset_alarms"].ToString().TrimEnd();
                    textBoxAssetComments.Text = assetDTableDetail.Rows[0]["asset_comments"].ToString().TrimEnd();

                    if (assetDTableDetail.Rows[0]["asset_image"] != DBNull.Value)
                    {
                        //Image retreiving
                        byte[] imageBytes = (byte[])assetDTableDetail.Rows[0]["asset_image"];

                        // Assign it to the updating value in case new image is not given
                        byteImage = (byte[])assetDTableDetail.Rows[0]["asset_image"];

                        if (imageBytes.Length > 10)
                        {
                            BitmapImage bitmapimage = new BitmapImage();
                            // Convert byte[] to Image   
                            bitmapimage.BeginInit();
                            bitmapimage.StreamSource = new MemoryStream(imageBytes);
                            bitmapimage.EndInit();
                            imageAssetPic.Source = (System.Windows.Media.ImageSource)bitmapimage;
                        }
                        else
                        {
                            imageAssetPic.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative));

                        }
                    }
                    else
                    {
                        imageAssetPic.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative));

                    }

                    //Enable Delete button
                    btnDeleteAsset.IsEnabled = true;
                }
                else
                {
                    textBlockTagNotifyMessage.Text = "No Asset found !";
                }

            }
            catch
            {
                LogFile.Log("Error: Asset tag search failed");
                textBlockTagNotifyMessage.Text = "Search Failed!";
                textBlockSearchWarning.Visibility = Visibility.Visible; 
                textBlockSearchWarning.Text =  "Search failed: Connection with DB problem ! ";

            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                assetDTableDetail.Dispose();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnEditAssetInfo control.
        /// Update and Edit asset information
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnEditAssetInfo_Click(object sender, RoutedEventArgs e)
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

            if (toggleButtonEditAsset == 0)
            {
                if (string.IsNullOrEmpty(textBoxAssetID.Text.Trim()))
                {
                    textBlockSearchWarning.Visibility = Visibility.Visible;
                    textBlockSearchWarning.Text = "Find an asset record to edit !";
                    return;

                }
                toggleButtonEditAsset = 1;
                btnAddNewAsset.IsEnabled = false;

                textBlockTagNotifyMessage.Visibility = Visibility.Visible;
                textBlockTagNotifyMessage.Text = "Edit Mode ON";
                btnEditAssetInfo.Content = "Save Asset Info";

                //Change to editable mode
                    //textBoxAssetID.IsReadOnly = false;
                    textBoxTagID.IsReadOnly = false;
                    textBoxAssetLocation.IsReadOnly = false;
                    cmbAssetCategory.IsEnabled = true;
                    textBoxAssetDescr.IsReadOnly = false;
                    textBoxAssetModel.IsReadOnly = false;
                    textBoxAssetTagCreated.IsReadOnly = false;
                    textBoxAssetQuantity.IsReadOnly = false;
                    textBoxAssetValue.IsReadOnly = false;
                    cmbAssetStatus.IsEnabled = true;
                    textBoxAssetLastChecked.IsReadOnly = false;
                    textBoxAssetLastLocation.IsReadOnly = false;
                    textBoxTagExpiry.IsReadOnly = false;
                    textBoxAssetMaintDue.IsReadOnly = false;
                    textBoxAssetAlarms.IsReadOnly = false;
                    textBoxAssetComments.IsReadOnly = false;
                //
                //Last checked is datetime.now
                    textBoxAssetLastChecked.Text = DateTime.Now.ToString();
                    textBoxAssetLastChecked.IsReadOnly = true;

                //Buttons
                    btnBrowseImage.IsEnabled = true;
                    btnDeleteAsset.IsEnabled = false;

            }
            else if (toggleButtonEditAsset == 1)
            {
                if (!(checkNullsNRegex()))
                {
                    //problem
                    return;

                }
                
                //Change to Read Only mode
                textBoxTagID.IsReadOnly = true;
                textBoxAssetLocation.IsReadOnly = true;
                cmbAssetCategory.IsEnabled = false;
                textBoxAssetDescr.IsReadOnly = true;
                textBoxAssetModel.IsReadOnly = true;
                textBoxAssetTagCreated.IsReadOnly = true;
                textBoxAssetQuantity.IsReadOnly = true;
                textBoxAssetValue.IsReadOnly = true;
                cmbAssetStatus.IsEnabled = false;
                textBoxAssetLastChecked.IsReadOnly = true;
                textBoxAssetLastLocation.IsReadOnly = true;
                textBoxTagExpiry.IsReadOnly = true;
                textBoxAssetMaintDue.IsReadOnly = true;
                textBoxAssetAlarms.IsReadOnly = true;
                textBoxAssetComments.IsReadOnly = true;

                toggleButtonEditAsset = 0;
                btnAddNewAsset.IsEnabled = true;

                textBlockTagNotifyMessage.Visibility = Visibility.Visible;

                btnEditAssetInfo.Content = "Edit Asset Info";
                

                // Store values:
                storeValues();
                this.Cursor = Cursors.Wait;
                try
                {
                    assetInfoTAdpt.UpdateAssetMain(tag_id, asset_location, asset_category, asset_desc, asset_tag_created, asset_model, asset_quantity, asset_image, asset_comments, asset_id);
                    assetInfoTAdpt.UpdateAssetInfoStat(tag_id, asset_lastchecked, asset_lastloc, asset_maint_due, tag_expiry, asset_status, asset_alarms, asset_value, asset_id);
                    textBlockTagNotifyMessage.Text = "Asset Info saved !";
                    textBlockSearchWarning.Visibility = Visibility.Hidden;

                    // Button enable/disable
                    //Enable delete button
                    btnDeleteAsset.IsEnabled = true;
                    btnBrowseImage.IsEnabled = false;


                    // Log ,email, SMS
                    LogFile.Log("Asset Updated: Record with Asset ID- " + asset_id.ToString() + " has been updated !");
                    if (AlertNotificationSettings.assetUpdates == true)
                    {
                        this.Cursor = Cursors.Wait;
                        if (AlertNotificationSettings.sendEmailUser == true)
                        {
                            HATrakaMain.emailContent = "Asset Updated: Record with Asset ID- " + asset_id.ToString() + " has been updated !";
                            sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                        }
                        if (AlertNotificationSettings.sendSMSUser == true)
                        {
                            SMSClass.sendSMS("Asset Updated: Record with Asset ID- " + asset_id.ToString() + " has been updated !", LoginWindow.user_contact);

                            if (AlertNotificationSettings.sendSMSAdmin == true)
                            {
                                SMSClass.sendSMS("Asset Updated: Record with Asset ID- " + asset_id.ToString() + " has been updated !", HATrakaMain.superAdminSMS);
                            }
                        }

                    }

                }
                catch (System.Exception ex)
                {
                    LogFile.Log("Error: Asset update failed");
                    textBlockTagNotifyMessage.Text = "Asset update failed !";
                    MessageBox.Show("Update failed: " + ex.ToString());
                }
                finally
                {
                    this.Cursor = Cursors.Arrow;
                }

            }

        }

        /// <summary>
        /// Handles the Click event of the btnBrowseImage control.
        /// Browse image for asset
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".png";
            dlg.Filter = "Image Files (JPG,JPEG,PNG,BMP,GIF,TIFF)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff";

            dlg.Title = "Select an image for the asset";
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                //dlg.OpenFile();
                imageAssetPic.Source = new BitmapImage(new Uri(dlg.FileName));
                
                //Other method to convert image into bytes
                //byte[] byteArrayStoredImage = getImageBytes(imageAssetPic.Source as BitmapSource);

                //BitmapImage img = (BitmapImage)imageAssetPic.Source;
                
                System.Drawing.Image image1 = System.Drawing.Image.FromFile(dlg.FileName,true);
                using (MemoryStream m = new MemoryStream())
                {
                    image1.Save(m,image1.RawFormat);
                    byteImage = m.ToArray();
                }
                
                //Stream imgStream = img.StreamSource;
                
                //imgStream.Read(byteImage, 0, (int)imgStream.Length);

                //Image retreiving checking

                //if (!(byteImage.Length == 0))
                //{
                //    BitmapImage bitmapimage1 = new BitmapImage();
                //    // Convert byte[] to Image   
                //    bitmapimage1.BeginInit();
                //    bitmapimage1.StreamSource = new MemoryStream(byteImage);
                //    bitmapimage1.EndInit();
                //    imageTry.Source = (System.Windows.Media.ImageSource)bitmapimage1;
                //}
                //else
                //{
                //    imageTry.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative));

                //}
                
            }
        }

        //Alternative method to get image bytes
        //private static byte[] getImageBytes(BitmapSource source)
        //{
        //    var encoder = new BmpBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(source));
        //    using (var stream = new MemoryStream())
        //    {
        //        encoder.Save(stream);
        //        return stream.ToArray();
        //    }
        //}

        /// <summary>
        /// Handles the Click event of the btnAddNewAsset control.
        /// Adds new asset to the database
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddNewAsset_Click(object sender, RoutedEventArgs e)
        {
           
            textBlockSearchWarning.Background = Brushes.LightPink;

            // CHECK for user ROLE ID here
            if (!((LoginWindow.user_roleid.CompareTo("AD01") == 0) || (LoginWindow.user_roleid.CompareTo("AD02") == 0)))
            {   //Not enough rights
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Not enough rights to perform this task.";
                return;
            }

            if (toggleButtonAddNewAsset == 0)
            {

                toggleButtonAddNewAsset = 1;
                btnEditAssetInfo.IsEnabled = false;

                textBlockTagNotifyMessage.Visibility = Visibility.Visible;
                textBlockTagNotifyMessage.Text = "Add New Asset";
                btnAddNewAsset.Content = "Save New Asset";

                //Change to editable mode
                //textBoxAssetID.IsReadOnly = true;
                textBoxAssetID.Text = "";

                textBoxTagID.IsReadOnly = false;
                textBoxTagID.Text = "";

                textBoxAssetLocation.IsReadOnly = false;
                textBoxAssetLocation.Text = "";
                
                cmbAssetCategory.IsEnabled = true;
                cmbAssetCategory.SelectedIndex = -1;

                textBoxAssetDescr.IsReadOnly = false;
                textBoxAssetDescr.Text = "";

                textBoxAssetModel.IsReadOnly = false;
                textBoxAssetModel.Text = "";

                textBoxAssetTagCreated.IsReadOnly = false;
                textBoxAssetTagCreated.Text = "";

                textBoxAssetQuantity.IsReadOnly = false;
                textBoxAssetQuantity.Text = "";

                textBoxAssetValue.IsReadOnly = false;
                textBoxAssetValue.Text = "";

                cmbAssetStatus.IsEnabled = true;
                cmbAssetStatus.SelectedIndex = -1;

                textBoxAssetLastChecked.IsReadOnly = false;
                textBoxAssetLastChecked.Text = "";

                textBoxAssetLastLocation.IsReadOnly = false;
                textBoxAssetLastLocation.Text = "";

                textBoxTagExpiry.IsReadOnly = false;
                textBoxTagExpiry.Text = "";

                textBoxAssetMaintDue.IsReadOnly = false;
                textBoxAssetMaintDue.Text = "";

                textBoxAssetAlarms.IsReadOnly = false;
                textBoxAssetAlarms.Text = "";

                textBoxAssetComments.IsReadOnly = false;
                textBoxAssetComments.Text = "";

                //
                //Last checked is datetime.now
                textBoxAssetLastChecked.Text = DateTime.Now.ToString();
                textBoxAssetTagCreated.Text = DateTime.Now.ToString(); 

                //Buttons
                btnBrowseImage.IsEnabled = true;
                btnDeleteAsset.IsEnabled = false;

                //Image:
                imageAssetPic.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative)); 

                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Background = Brushes.LightBlue;
                textBlockSearchWarning.Text = "Note ! Asset ID will be generated automatically";

            }
            else if (toggleButtonAddNewAsset == 1)
            {

                if (!(checkNullsNRegex()))
                {
                    //problem
                    return;

                }

                //Change to Read Only mode
                textBoxTagID.IsReadOnly = true;
                textBoxAssetLocation.IsReadOnly = true;
                cmbAssetCategory.IsEnabled = false;
                textBoxAssetDescr.IsReadOnly = true;
                textBoxAssetModel.IsReadOnly = true;
                textBoxAssetTagCreated.IsReadOnly = true;
                textBoxAssetQuantity.IsReadOnly = true;
                textBoxAssetValue.IsReadOnly = true;
                cmbAssetStatus.IsEnabled = false;
                textBoxAssetLastChecked.IsReadOnly = true;
                textBoxAssetLastLocation.IsReadOnly = true;
                textBoxTagExpiry.IsReadOnly = true;
                textBoxAssetMaintDue.IsReadOnly = true;
                textBoxAssetAlarms.IsReadOnly = true;
                textBoxAssetComments.IsReadOnly = true;

                toggleButtonAddNewAsset = 0;
                btnEditAssetInfo.IsEnabled = true;

                textBlockTagNotifyMessage.Visibility = Visibility.Visible;
                btnAddNewAsset.Content = "Add New Asset";
          
                //Store values:
                storeValues();
                this.Cursor = Cursors.Wait;

                //Temp table
                ztATdbLocalDataSet1.AssetInfoDataTableDataTable assetTempT = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();

                string newAsset_id;

                do
                {
                    newAsset_id = generateNewAssetID(); // Eg: SA+19+MV+6712
                    //use this in TA

                    // But first check for unique ID as it is a PK
                    try
                    {
                        assetInfoTAdpt.FillAllAssetID(assetTempT, newAsset_id);
                    }
                    catch
                    {
                        LogFile.Log("Error: Could not get all Reader IDs ");
                        //Could not get data
                    }


                } while (assetTempT.Count > 0);
       
                try
                {
                    assetInfoTAdpt.InsertAssetMain(newAsset_id, tag_id, asset_location, asset_category, asset_desc, asset_tag_created, asset_model, asset_quantity, asset_image, asset_comments);
                    assetInfoTAdpt.InsertAssetInfoStat(newAsset_id, tag_id, asset_lastchecked, asset_lastloc, asset_maint_due, tag_expiry, asset_status, asset_alarms, asset_value);

                    textBlockTagNotifyMessage.Text = "Asset Added !";
                    textBlockSearchWarning.Visibility = Visibility.Hidden;

                    //Enable the delete button
                    btnDeleteAsset.IsEnabled = true;
                    btnBrowseImage.IsEnabled = false;
                    textBoxAssetID.Text = newAsset_id;

                    // Log ,email, SMS
                    LogFile.Log("Asset Added: Record with Asset ID- " + newAsset_id.ToString() + " has been added !");
                    if (AlertNotificationSettings.assetAdded == true)
                    {
                        this.Cursor = Cursors.Wait;
                        if (AlertNotificationSettings.sendEmailUser == true)
                        {
                            HATrakaMain.emailContent = "Asset Added: Record with Asset ID- " + newAsset_id.ToString() + " has been added !";
                            sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                        }
                        if (AlertNotificationSettings.sendSMSUser == true)
                        {
                            SMSClass.sendSMS("Asset Added: Record with Asset ID- " + newAsset_id.ToString() + " has been added !", LoginWindow.user_contact);

                            if (AlertNotificationSettings.sendSMSAdmin == true)
                            {
                                SMSClass.sendSMS("Asset Added: Record with Asset ID- " + newAsset_id.ToString() + " has been added !", HATrakaMain.superAdminSMS);
                            }
                        }

                    }

                }
                catch (System.Exception ex)
                {
                    LogFile.Log("Error: Asset adding failed ");
                    textBlockTagNotifyMessage.Text = "Asset generation failed !";
                    MessageBox.Show("Adding failed: " + ex.ToString());
                }

                finally
                {
                    this.Cursor = Cursors.Arrow;
                }

            }

        }

        /// <summary>
        /// Checks the nulls
        /// </summary>
        /// <returns></returns>
        private bool checkNullsNRegex()
        {
            //Check for nulls
            //tag ID
            if (string.IsNullOrEmpty(textBoxTagID.Text))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Tag ID cannot be blank";
                return false;
            }

            if (!Regex.IsMatch(textBoxTagID.Text, @"^[\d]+$"))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Error ! Tag ID should contain only numbers";
                return false;
            }

            //Asset Location
            if (string.IsNullOrEmpty(textBoxAssetLocation.Text))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset Location cannot be blank";
                return false;
            }

            if (!Regex.IsMatch(textBoxAssetLocation.Text, @"^[a-zA-Z0-9]+$"))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Error ! Asset Location cannot contain special characters";
                return false;
            }

            //Asset category
            if (cmbAssetCategory.SelectedIndex == -1)
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset Category cannot be blank";
                return false;
            }

            //Asset descr
            if (string.IsNullOrEmpty(textBoxAssetDescr.Text))
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset Description cannot be blank";
                return false;
            }

            //Asset category
            if (cmbAssetStatus.SelectedIndex == -1)
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset Status cannot be blank";
                return false;
            }

            //Asset quantity should be numbers if filled
            if (!(string.IsNullOrEmpty(textBoxAssetQuantity.Text)))
            {
                if (!Regex.IsMatch(textBoxAssetQuantity.Text, @"^[\d]+$"))
                {
                    textBlockSearchWarning.Visibility = Visibility.Visible;
                    textBlockSearchWarning.Text = "Error ! Asset Quantity should contain only numbers";
                    return false;
                }
            }

            //Asset value should be numbers (floating) if filled
            if (!(string.IsNullOrEmpty(textBoxAssetValue.Text)))
            {
                if (!Regex.IsMatch(textBoxAssetValue.Text, @"^[0-9]*\.?[0-9]+$"))
                {
                    textBlockSearchWarning.Visibility = Visibility.Visible;
                    textBlockSearchWarning.Text = "Error ! Asset Value should contain only numbers";
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

            asset_id = textBoxAssetID.Text.ToString();
            tag_id = textBoxTagID.Text.Trim();
            asset_location = textBoxAssetLocation.Text;
            asset_category = cmbAssetCategory.Text;
            asset_desc = textBoxAssetDescr.Text;
            asset_model = textBoxAssetModel.Text;

            asset_quantity = short.Parse(textBoxAssetQuantity.Text.ToString());
            asset_value = decimal.Parse( textBoxAssetValue.Text.ToString());
            asset_status = cmbAssetStatus.Text;

            asset_lastloc = textBoxAssetLastLocation.Text;

            asset_alarms = textBoxAssetAlarms.Text;
            asset_comments = textBoxAssetComments.Text;

            if ((DateTime.TryParse(textBoxAssetTagCreated.Text.ToString(), out asset_tag_created)) == true)
            {
                //success
            }
            else
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset tag created date did not match date format and has been changed! ";
                asset_tag_created = DateTime.Now.AddYears(-2); //actually use default date
                textBoxAssetTagCreated.Text = asset_tag_created.ToString();
            }

            if ((DateTime.TryParse(textBoxAssetLastChecked.Text.ToString(), out asset_lastchecked)) == true)
            {
                //success
            }
            else
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset last checked date did not match date format and has been changed! ";
                asset_lastchecked = DateTime.Now; //actually use default date
                textBoxAssetLastChecked.Text = asset_lastchecked.ToString();
            }

            //textBoxTagExpiry
            if ((DateTime.TryParse(textBoxTagExpiry.Text.ToString(), out tag_expiry)) == true)
            {
                //success
            }
            else
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset tag expiry date did not match date format and has been changed! ";
                tag_expiry = DateTime.Now.AddYears(2); //actually use default date
                textBoxTagExpiry.Text = tag_expiry.ToString();
            }

            //textBoxAssetMaintDue
            if ((DateTime.TryParse(textBoxAssetMaintDue.Text.ToString(), out asset_maint_due)) == true)
            {
                //success
            }
            else
            {
                textBlockSearchWarning.Visibility = Visibility.Visible;
                textBlockSearchWarning.Text = "Warning ! Asset maintenance due date did not match date format and has been changed! ";
                asset_maint_due = DateTime.Now.AddYears(2); //actually use default date
                textBoxAssetMaintDue.Text = asset_maint_due.ToString();
            }

            // Asset Image
            // possible *change* here byte image length
            if (byteImage != null)
            {
                if (byteImage.Length > 0)
                {
                    asset_image = byteImage;
                }
                else
                {
                    
                    //asset_image[0] = 0; 
                }
            }
        }

        /// <summary>
        /// Generates the new asset ID.
        /// </summary>
        /// <returns></returns>
        private string generateNewAssetID()
        {
            string newAssetID = "";         
            int rNum;

            //Generate SA (two alphabets)
            newAssetID += "SA";

            //Add a random 2 digit number
            Random rnd2 = new Random();
            rNum = rnd2.Next(10, 99);
            newAssetID += rNum.ToString();
            
            //Generate two alphabets depending on asset category
            if (cmbAssetCategory.Text.CompareTo("Fixed") == 0)
            {
                newAssetID += "FX";
            }
            else if (cmbAssetCategory.Text.CompareTo("Movable") == 0)
            {
                newAssetID += "MV";
            }

            else if (cmbAssetCategory.Text.CompareTo("Portable") == 0)
            {
                newAssetID += "PT";
            }

            //Add a random 4 digit number
            Random rnd4 = new Random();
            rNum = rnd4.Next(1000, 9999);
            newAssetID += rNum.ToString();

            return newAssetID;

        }

        /// <summary>
        /// Clears all fields.
        /// </summary>
        private void clearAllFields()
        {
            //Change to readonly mode and clear all fields
            //textBoxAssetID.IsReadOnly = true;
            textBoxAssetID.Text = "";

            textBoxTagID.IsReadOnly = true;
            textBoxTagID.Text = "";

            textBoxAssetLocation.IsReadOnly = true;
            textBoxAssetLocation.Text = "";

            cmbAssetCategory.IsEnabled = false;
            cmbAssetCategory.SelectedIndex = -1;

            textBoxAssetDescr.IsReadOnly = true;
            textBoxAssetDescr.Text = "";

            textBoxAssetModel.IsReadOnly = true;
            textBoxAssetModel.Text = "";

            textBoxAssetTagCreated.IsReadOnly = true;
            textBoxAssetTagCreated.Text = "";

            textBoxAssetQuantity.IsReadOnly = true;
            textBoxAssetQuantity.Text = "";

            textBoxAssetValue.IsReadOnly = true;
            textBoxAssetValue.Text = "";

            cmbAssetStatus.IsEnabled = false;
            cmbAssetStatus.SelectedIndex = -1;

            textBoxAssetLastChecked.IsReadOnly = true;
            textBoxAssetLastChecked.Text = "";

            textBoxAssetLastLocation.IsReadOnly = true;
            textBoxAssetLastLocation.Text = "";

            textBoxTagExpiry.IsReadOnly = true;
            textBoxTagExpiry.Text = "";

            textBoxAssetMaintDue.IsReadOnly = true;
            textBoxAssetMaintDue.Text = "";

            textBoxAssetAlarms.IsReadOnly = true;
            textBoxAssetAlarms.Text = "";

            textBoxAssetComments.IsReadOnly = true;
            textBoxAssetComments.Text = "";


            //Buttons
            btnBrowseImage.IsEnabled = false;
            btnDeleteAsset.IsEnabled = false;

            //Image:
            imageAssetPic.Source = new BitmapImage(new Uri(@"./IconImages/noImage.png", UriKind.Relative)); 

        }

        /// <summary>
        /// Handles the Click event of the btnDeleteAsset control.
        /// Delete the asset record
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnDeleteAsset_Click(object sender, RoutedEventArgs e)
        {

            string original_assetID = "";

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

            original_assetID = textBoxAssetID.Text;

            this.Cursor = Cursors.Wait;
            try
            {
                assetInfoTAdpt.DeleteAsset(original_assetID);
                textBlockTagNotifyMessage.Text = "Record Deleted!";

                //Clear all fields.
                clearAllFields();

                // Log ,email, SMS
                LogFile.Log("Asset Deleted: Record with Asset ID- " + original_assetID.ToString() + " has been deleted !");
                if (AlertNotificationSettings.assetDeleted == true)
                {
                    this.Cursor = Cursors.Wait;
                    if (AlertNotificationSettings.sendEmailUser == true)
                    {
                        HATrakaMain.emailContent = "Asset Deleted: Record with Asset ID- " + original_assetID.ToString() + " has been deleted !";
                        sendEmail.sendEmailMessage(HATrakaMain.emailContent);

                    }
                    if (AlertNotificationSettings.sendSMSUser == true)
                    {
                        SMSClass.sendSMS("Asset Deleted: Record with Asset ID- " + original_assetID.ToString() + " has been deleted !", LoginWindow.user_contact);

                        if (AlertNotificationSettings.sendSMSAdmin == true)
                        {
                            SMSClass.sendSMS("Asset Deleted: Record with Asset ID- " + original_assetID.ToString() + " has been deleted !", HATrakaMain.superAdminSMS);
                        }
                    }

                }

            }

            catch (System.Exception ex)
            {
                //btnDeleteAsset.IsEnabled = false;
                LogFile.Log("Error: Deleting asset record failed");
                textBlockTagNotifyMessage.Text = "Delete operation Failed!";
                MessageBox.Show("Delete failed possibly due to database connection issue: " + ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        
    }
}
