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
using System.Management;
using System.Net;


namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for NewConnection.xaml
    /// </summary>
    public partial class NewConnection : Window
    {
        public static int numberOfConnections = 3;
        //define the number of connections
        public static int countConnections = 0;
        public static string[] myConn = new string[numberOfConnections];
        public static Dictionary<int, string> connStringDB = new Dictionary<int, string>();

        /// <summary>
        /// Initializes constructor of the <see cref="NewConnection" /> class.
        /// </summary>
        public NewConnection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the btnExitConnection control.
        /// Exits the window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnExitConnection_Click(object sender, RoutedEventArgs e)
        {
          
            this.Close();
        }

        /// <summary>
        /// Handles the GotFocus event of the textBoxNewConnectionName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxNewConnectionName_GotFocus(object sender, RoutedEventArgs e)
        {
            lblOR.IsEnabled = false;
            comboBoxSelectExistingConnection.SelectedIndex = -1;
            comboBoxSelectExistingConnection.IsEnabled = false;
            textBoxNewConnectionName.IsEnabled = true;
            radioButtonCreateNewConnection.IsChecked = true;

            if (textBoxNewConnectionName.Text.Equals("<Connection Name>"))
            {
                textBoxNewConnectionName.Text = "";
                textBoxNewConnectionName.Foreground = Brushes.Black;
                textBoxNewConnectionName.Background = Brushes.LightYellow;
            }


            
        }

        /// <summary>
        /// Handles the LostFocus event of the textBoxNewConnectionName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxNewConnectionName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxNewConnectionName.Text.Equals(""))
            {
                textBoxNewConnectionName.Text = "<Connection Name>";
                textBoxNewConnectionName.Foreground = Brushes.Silver;
                textBoxNewConnectionName.Background = Brushes.White;
            }
        }

        /// <summary>
        /// Handles the GotMouseCapture event of the comboBoxSelectExistingConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void comboBoxSelectExistingConnection_GotMouseCapture(object sender, MouseEventArgs e)
        {
            textBoxNewConnectionName.Text = "<Connection Name>";
            textBoxNewConnectionName.Foreground = Brushes.Silver;
            textBoxNewConnectionName.Background = Brushes.White;

            textBoxNewConnectionName.IsEnabled = false;
            comboBoxSelectExistingConnection.IsEnabled = true;
            radioButtonSelectExistingConnection.IsChecked = true;
        }

        /// <summary>
        /// Handles the Checked event of the radioButtonCreateNewConnection control.
        /// Radio button checked to start creating new connection
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void radioButtonCreateNewConnection_Checked(object sender, RoutedEventArgs e)
        {
            lblOR.IsEnabled = false;

            btnAddSave.IsEnabled = true;
            btnUpdateConnection.IsEnabled = false;
            btnDeleteConnection.IsEnabled = false;

            comboBoxBaudRate.Visibility = Visibility.Hidden;
            textBoxIPBaudRateUSB.Visibility = Visibility.Visible;
            
            comboBoxSelectExistingConnection.SelectedIndex = -1;
            comboBoxSelectExistingConnection.IsEnabled = false;
            textBoxNewConnectionName.IsEnabled = true;
            //radioButtonCreateNewConnection.IsChecked = true;
            textBoxNewConnectionName.Focus();

            if (textBoxNewConnectionName.Text.Equals("<Connection Name>"))
            {
                textBoxNewConnectionName.Text = "";
                textBoxNewConnectionName.Foreground = Brushes.Black;
                textBoxNewConnectionName.Background = Brushes.LightYellow;
            }
        }

        /// <summary>
        /// Handles the Checked event of the radioButtonSelectExistingConnection control.
        /// To select existing connection
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void radioButtonSelectExistingConnection_Checked(object sender, RoutedEventArgs e)
        {
            btnAddSave.IsEnabled = false;
            btnUpdateConnection.IsEnabled = true;
            btnDeleteConnection.IsEnabled = true;

            textBoxNewConnectionName.Text = "<Connection Name>";
            textBoxNewConnectionName.Foreground = Brushes.Silver;
            textBoxNewConnectionName.Background = Brushes.White;

            textBoxNewConnectionName.IsEnabled = false;
            comboBoxSelectExistingConnection.IsEnabled = true;

        }

        /// <summary>
        /// Handles the Selected event of the cmbItemEthernetTCPIP control.
        /// Ethernet LAN connection selected
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void cmbItemEthernetTCPIP_Selected(object sender, RoutedEventArgs e)
        {
            comboBoxBaudRate.Visibility = Visibility.Hidden;
            textBoxIPBaudRateUSB.Visibility = Visibility.Visible;
            textBoxIPBaudRateUSB.IsReadOnly = false;
            textBoxIPBaudRateUSB.Text = "192.168.0.178";
            textBoxIPBaudRateUSB.Focus();
            textBoxIPBaudRateUSB.SelectAll();
            
            lblIPBaudRateUSB.Content = "IP address of the reader";
            textBoxImportantNote.Text = @"Note: For an Ethernet (TCP/IP) connection 
the default IP address of the reader is 
192.168.0.178 unless manually changed.";

        }

        /// <summary>
        /// Handles the Selected event of the cmbItemUSB control.
        /// USB connection selected
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void cmbItemUSB_Selected(object sender, RoutedEventArgs e)
        {
            comboBoxBaudRate.Visibility = Visibility.Hidden;
            textBoxIPBaudRateUSB.Visibility = Visibility.Visible;
            textBoxIPBaudRateUSB.Text = "USB 2.0 High Speed";
            textBoxIPBaudRateUSB.IsReadOnly = true;
            lblIPBaudRateUSB.Content = "USB connection properties";
            textBoxImportantNote.Text = @"Note: For a high speed USB connection 
ensure that the USB drivers are installed 
correctly and that there are no errors.";


        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// On window load, populate existing connections and active ports
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxIPBaudRateUSB.IsReadOnly = true;
            //add the connections here
            int totalCount = connStringDB.Count;
            for (int i = 0; i < totalCount; i++)
            {
                if (connStringDB.ContainsKey(i))
                {
                    string localConnName = connStringDB[i];
                    comboBoxSelectExistingConnection.Items.Add(localConnName);
                }
            }
            //string a = myConn[0];
            //int c = countConnections;
            string pNames;
            string[] portNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string s in portNames)
            {
                pNames = s;
                pNames += "ports"; //Just additional fluff to clearly seperate port name
                string portN = pNames.Remove(1 + (pNames.LastIndexOfAny("0123456789".ToCharArray())));
                comboBoxConnectionType.Items.Add(portN);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboBoxConnectionType control.
        /// Combo box has many items that can be selected
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void comboBoxConnectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string checkCOMstring = null;

            //string sdsd = comboBoxConnectionType.SelectedValue.ToString();
            if (comboBoxConnectionType.SelectedIndex == -1)
            {
                textBoxImportantNote.Text = "";
                textBoxIPBaudRateUSB.Text = "";
                lblIPBaudRateUSB.Content = "Connection Properties";
                checkBoxDefaultConnection.IsChecked = false;
                return;

            }
            else
            {
                checkCOMstring = comboBoxConnectionType.SelectedItem.ToString();
            }

            if ((checkCOMstring.Substring(0, 3).CompareTo("COM")) == 0)
            {
                comboBoxBaudRate.Visibility = Visibility.Visible;
                textBoxIPBaudRateUSB.Visibility = Visibility.Hidden;
                //textBoxIPBaudRateUSB.Text = "";
                lblIPBaudRateUSB.Content = "Baud Rate (bps)";
                textBoxImportantNote.Text = @"Note: For serial COM connections ensure 
that the correct COM Port number is 
selected by checking with device manager.";
            
            }


            

        }

        /// <summary>
        /// Handles the Click event of the btnAddSave control.
        /// Add and save a new connection to the system
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnAddSave_Click(object sender, RoutedEventArgs e)
        {

            string conName, conType, conProperties;
            bool isDefault = false;
            


            if (textBoxNewConnectionName.Text.CompareTo("<Connection Name>") == 0)
            {
                MessageBox.Show("Please name your connection!", "Error-No name provided", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (comboBoxConnectionType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a connection method!", "Error-No connection type provided", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (checkBoxDefaultConnection.IsChecked == true)
            { 
                conName = textBoxNewConnectionName.Text.ToString() + " [default]";
                isDefault = true;
            }
            else
            {
                conName = textBoxNewConnectionName.Text.ToString();
            }

            if (connStringDB.ContainsValue(conName))
            {
                MessageBox.Show("Please use a different connection name!", "Duplicate name found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            conType = comboBoxConnectionType.SelectionBoxItem.ToString();
            conProperties = textBoxIPBaudRateUSB.Text.ToString();
            //IsIPv4
            if (comboBoxConnectionType.Text.CompareTo("Ethernet (TCP/IP)") == 0)
            {
                string ipaddress = textBoxIPBaudRateUSB.Text.ToString();

                if (!(IsIPv4(ipaddress)))
                {
                    MessageBox.Show("IP adddress entered is not valid!", "Invalid IP address", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

            }

            
            // Check for COM
            else if ((conType.Substring(0, 3).CompareTo("COM")) == 0)
            {

                conProperties = comboBoxBaudRate.Text; 
            }

            for (countConnections = 0; countConnections < myConn.Count(); countConnections++)
            {
                if (String.IsNullOrEmpty(myConn[countConnections]))
                {

                    //Check for default connection...
                    if (isDefault)
                    {
                        checkUpdateDefaultConn();
                    }

                    //Custom connection string for reader connection
                    myConn[countConnections] = conName + "#" + conType + "#" + conProperties + "#" + isDefault.ToString();
                    connStringDB.Add(countConnections, conName);
                    comboBoxSelectExistingConnection.Items.Add(conName);

                    //countConnections++;
                    MessageBox.Show("Connection added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    return;
                }
            }
            if(countConnections >= myConn.Count()) 
            {
                MessageBox.Show("Please limit the number of connections !", "Maximum Connections exceeded", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Handles the Click event of the btnDeleteConnection control.
        /// Delete an existing connection from the system
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnDeleteConnection_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxSelectExistingConnection.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the connection name to delete!", "Error-No connection name provided", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            string connDeleted = comboBoxSelectExistingConnection.SelectionBoxItem.ToString();
            int countKey = connStringDB.SingleOrDefault(x => x.Value == connDeleted).Key;
            //issue here

            connStringDB.Remove(countKey);
            myConn[countKey] = null;
            //match with the string
            comboBoxSelectExistingConnection.Items.Remove(comboBoxSelectExistingConnection.SelectedItem);
            comboBoxConnectionType.SelectedIndex = -1;
            countConnections = 0;// not countKey reset to the top for scanning empty
            MessageBox.Show("Record deleted!", "Deletion of record complete", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboBoxSelectExistingConnection control.
        /// Select a different existing connection
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void comboBoxSelectExistingConnection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string connUpdateDisplay;
            string connStringDisplay;
            string[] connStringArray = new string[4];
            int getDisplayKey;

            if (comboBoxSelectExistingConnection.SelectedIndex == -1)
            {
                comboBoxConnectionType.SelectedIndex = -1;
                comboBoxBaudRate.Visibility = Visibility.Hidden;
                textBoxIPBaudRateUSB.Visibility = Visibility.Visible;
                return;
            }

            connUpdateDisplay = comboBoxSelectExistingConnection.SelectedItem.ToString();
            getDisplayKey = connStringDB.SingleOrDefault(x => x.Value == connUpdateDisplay).Key;
            connStringDisplay = myConn[getDisplayKey];
            connStringArray = connStringDisplay.Split('#');
            //check issue here connStringArray = null issue

            // connection name : connStringArray[0]
            //connection type: connStringArray[1]
            // connection type properties: connStringArray[2]
            // Is default connection bool: connStringArray[3]
            comboBoxConnectionType.Text = connStringArray[1];

            if (comboBoxConnectionType.Text.CompareTo("Ethernet (TCP/IP)") == 0)
            {
                textBoxIPBaudRateUSB.Text = connStringArray[2];  //IP address set
                textBoxIPBaudRateUSB.Focus();
                textBoxIPBaudRateUSB.SelectAll();
            }

            else if ((connStringArray[1].Substring(0, 3).CompareTo("COM")) == 0)
            {
                comboBoxBaudRate.Text = connStringArray[2]; //baud rate set

            }

            if (connStringArray[3].CompareTo("True") == 0)
            {
                checkBoxDefaultConnection.IsChecked = true;
            }
            else
            {
                checkBoxDefaultConnection.IsChecked = false;
            }


        }


        /// <summary>
        /// Determines whether IP address is IPv4 standard
        /// </summary>
        /// <param name="IPvalue">The I pvalue.</param>
        /// <returns>
        ///   <c>true</c> if [is I PV4] [the specified I pvalue]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIPv4(string IPvalue)
        {
            IPAddress address;
            if (String.IsNullOrEmpty(IPvalue))
            {
                return false;
                //No IPvalue present
            }
            else if (IPAddress.TryParse(IPvalue, out address))
            { 
                    return true;
               
            }

            return false;
        }

        /// <summary>
        /// Handles the Click event of the btnUpdateConnection control.
        /// Update an existing connection in the system.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnUpdateConnection_Click(object sender, RoutedEventArgs e)
        {
            string conNameU, conTypeU, conPropertiesU;
            bool isDefaultU = false;


            if (comboBoxSelectExistingConnection.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an existing connection!", "Error-No selection provided", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

          
            string conNameTemp = comboBoxSelectExistingConnection.SelectionBoxItem.ToString();
            int selectedInd = comboBoxSelectExistingConnection.SelectedIndex;
            int findIndexdef = conNameTemp.IndexOf(" [default]");
            if (findIndexdef != -1)
            {
                conNameTemp = conNameTemp.Remove(findIndexdef);
            }

            if (checkBoxDefaultConnection.IsChecked == true)
            {
                conNameU = conNameTemp + " [default]";
                isDefaultU = true;
            }
            else
            {
                conNameU = conNameTemp;
            }


            conTypeU = comboBoxConnectionType.SelectionBoxItem.ToString();
            conPropertiesU = textBoxIPBaudRateUSB.Text.ToString();
            //IsIPv4
            if (comboBoxConnectionType.Text.CompareTo("Ethernet (TCP/IP)") == 0)
            {
                string ipaddress = textBoxIPBaudRateUSB.Text.ToString();

                if (!(IsIPv4(ipaddress)))
                {
                    MessageBox.Show("IP adddress entered is not valid!", "Invalid IP address", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

            }

            // Check for COM
            else if ((conTypeU.Substring(0, 3).CompareTo("COM")) == 0)
            {

                conPropertiesU = comboBoxBaudRate.Text;
            }

            string connUpdated = comboBoxSelectExistingConnection.SelectionBoxItem.ToString();
            int countKeyU = connStringDB.SingleOrDefault(x => x.Value == connUpdated).Key;

            if (isDefaultU)
            {
                checkUpdateDefaultConn();
            }

            //Update connection string for reader connection
            myConn[countKeyU] = conNameU + "#" + conTypeU + "#" + conPropertiesU + "#" + isDefaultU.ToString();
            connStringDB[countKeyU] = conNameU;
            comboBoxSelectExistingConnection.Items[selectedInd] = conNameU;

            MessageBox.Show("Connection updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            return;



        }

        /// <summary>
        /// Loads the default connection.
        /// Check whether default connection exists
        /// </summary>
        public static void loadDefaultConnection()
        {
            string readerDefaultConnection = "";
            string connectionName = "connectOne [default]";
            readerDefaultConnection = connectionName + "#Ethernet (TCP/IP)#192.168.0.178#True";
            myConn[0] = readerDefaultConnection;
            if (!(connStringDB.ContainsKey(0)))
            {
                connStringDB.Add(0, connectionName);
            }
        }

        /// <summary>
        /// Updates the default connection for the system.
        /// </summary>
      private void checkUpdateDefaultConn()
      {
        //update combo Box
          // update Dictionary DB
          //Update string
          string comboboxStr = string.Empty;
          string newString;
          int findIndex;

          //string try1 = "COM5 [default]";
          //comboboxStr = try1;


          //findIndex = comboboxStr.IndexOf(" [default]");
          //newString = comboboxStr.Remove(findIndex);
         
          for (int i = 0; i < comboBoxSelectExistingConnection.Items.Count; i++)
          {
              comboboxStr = comboBoxSelectExistingConnection.Items[i].ToString();
              findIndex = comboboxStr.IndexOf(" [default]");
              if (findIndex != -1)
              {
                  newString = comboboxStr.Remove(findIndex);
                  
                  comboBoxSelectExistingConnection.Items.Insert(i,newString);
                  comboBoxSelectExistingConnection.Items.RemoveAt(i+1);
              }

          }

          // Update dictionary Db

          for (int j = 0; j < connStringDB.Count; j++)
          {
              string stringdb;
              connStringDB.TryGetValue(j,out stringdb);
              if (!(String.IsNullOrEmpty(stringdb)))
              {
                  //string stringdb = connStringDB[j];
                  int findInd = stringdb.IndexOf(" [default]");
                  if (findInd != -1)
                  {
                      string updatedString = stringdb.Remove(findInd);
                      connStringDB[j] = updatedString;
                  }
              }
          }

          //remove from string array as well.

          for (int k = 0; k < myConn.Count(); k++)
          {
              
              string stringConn = myConn[k];

              if (!(String.IsNullOrEmpty(stringConn)))
              {

                  int searchInd = stringConn.IndexOf(" [default]");
                  if (searchInd != -1)
                  {
                      //string updatedConString = stringConn.Remove(searchInd,10);
                      //int defaultIndex = updatedConString.IndexOf("True");
                      myConn[k] = myConn[k].Replace("True", "False");
                      myConn[k] = myConn[k].Replace(" [default]", "");
                  }
              }

          }




      }



    }
}
