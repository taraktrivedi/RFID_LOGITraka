using System;
using System.Net;
using System.Data;
using System.Net.NetworkInformation;
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
    /// Interaction logic for NetworkConnections.xaml
    /// </summary>
    public partial class NetworkConnections : Window
    {
        // *change* here for reader parameters
        public static int totalReadersConnected = 4;
        public static string readerPort = "4001";
        public static string[] readerIPs = new string[totalReadersConnected];

        //Instantiate TA
        
        public static ztATdbLocalDataSet1TableAdapters.readerInfoDataTableTableAdapter readerInfoTabAdpt = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.readerInfoDataTableTableAdapter();
        public static ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter assetInfoTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.AssetInfoDataTableTableAdapter();

        /// <summary>
        /// Constructor of the <see cref="NetworkConnections" /> class.
        /// </summary>
        public NetworkConnections()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the checkNetworkConn control.
        /// Populate the network connections grid on window load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void checkNetworkConn_Loaded(object sender, RoutedEventArgs e)
        {

            populateNetworkConnections();
        }

        /// <summary>
        /// Populates the network connections.
        /// Function to give netstat like results to users
        /// </summary>
        private void populateNetworkConnections()
        {
          
            //string readerIP = "";
            short sr_Count = 0;
            //string remoteIPtext = "";

            DataTable dtNetworkConnections = new DataTable();
            dtNetworkConnections.Clear();
            dtNetworkConnections.Columns.Add("sr_no", typeof(System.Int32));
            dtNetworkConnections.Columns.Add("protocol", typeof(string));
            dtNetworkConnections.Columns.Add("local_address", typeof(string));
            dtNetworkConnections.Columns.Add("remote_address", typeof(string));
            dtNetworkConnections.Columns.Add("state", typeof(string));

            //////////////////////////////////////////////////////////////
            IPGlobalProperties ipPropertiesOne = IPGlobalProperties.GetIPGlobalProperties();

            IPEndPoint[] endPointsOne = ipPropertiesOne.GetActiveTcpListeners();
            TcpConnectionInformation[] tcpConnectionsOne = ipPropertiesOne.GetActiveTcpConnections();

            foreach (TcpConnectionInformation info in tcpConnectionsOne)
            {
                DataRow dRowValues = dtNetworkConnections.NewRow();

                dRowValues["sr_no"] = ++sr_Count;
                dRowValues["protocol"] = "TCP";
                dRowValues["local_address"] = info.LocalEndPoint.Address.ToString() + ":" + info.LocalEndPoint.Port.ToString();
                dRowValues["remote_address"] = info.RemoteEndPoint.Address.ToString() + ":" + info.RemoteEndPoint.Port.ToString();
                dRowValues["state"] = info.State.ToString();

                dtNetworkConnections.Rows.Add(dRowValues);
                
            }

            //sr_Count = 0;
            dgNetworkConnections.DataContext = dtNetworkConnections.DefaultView;

            // *future* USE ONLY
            //for (int j = 0; j < dtNetworkConnections.Rows.Count; j++)
            //{
            //    remoteIPtext += "IPaddress=" + dtNetworkConnections.Rows[j]["remote_address"].ToString() + ";"  + "\r\n";

            //}

            //Port address for Reader IP is 4001 set in reader setup software *change* here if modified
            //int i = 0;
            //var readerIPRegex = new Regex(".*IPaddress=(.*):1433;.*");
            //foreach (Match m in readerIPRegex.Matches(remoteIPtext))
            //{              
            //    readerIPs[i] = m.Groups[1].Value.ToString().TrimEnd();
            //    ++i;             
            //}

            //DataView dvIP = new DataView(dtNetworkConnections);
            ////remote_address
            //dvIP.RowFilter = "remote_address " + "LIKE '%192.168.0.%' ";
            //if (dvIP.Count > 0)
            //{
            //    readerIP = dvIP[0]["remote_address"].ToString();
            //}
        }

        /// <summary>
        /// Gets the reader IP.
        /// Processes the reader IP from the active tcp/ip connections
        /// </summary>
        /// <param name="readerNumber">The reader number.</param>
        /// <returns></returns>
        public static string getReaderIP(int readerNumber)
        {
            string readerIP = "";

            //Enable below to get real-time data (IP) of the readers

            //IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            //IPEndPoint[] endPoints = ipProperties.GetActiveTcpListeners();
            //TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

            //int i = 0;
            //foreach (TcpConnectionInformation info in tcpConnections)
            //{
            //    if (info.RemoteEndPoint.Port.ToString().CompareTo(readerPort) == 0)
            //    {
            //        readerIPs[i] = info.RemoteEndPoint.Address.ToString();
            //        i++;

            //    }

            //}

            // *change* here comment below
            //Manually add the available reader ID's
            readerIPs[0] = "192.168.0.178";
            readerIPs[1] = "192.168.0.178";
            readerIPs[2] = "192.168.0.178";
            readerIPs[3] = "192.168.0.178";

            readerIP = readerIPs[readerNumber];
            return readerIP;

        }

        /// <summary>
        /// Gets the active reader ID.
        /// </summary>
        /// <param name="readerIP">The reader IP.</param>
        /// <returns></returns>
        public static string getActiveReaderID(string readerIP)
        {
            string activeReaderID = "";

            var readerIDRegex = new Regex(".*192.168.0.(.*).*");  
            if (readerIDRegex.IsMatch(readerIP))
            {
                activeReaderID = readerIDRegex.Match(readerIP).Groups[1].Value.ToString().Trim();
                if (activeReaderID.Length == 2)
                {
                    activeReaderID = "0" + activeReaderID;
                }
                activeReaderID = "RF" + activeReaderID;
            }

            return activeReaderID;
        }

        /// <summary>
        /// Updates the asset location.
        /// </summary>
        /// <param name="getTagId">The get tag id.</param>
        /// <param name="locID">The loc ID.</param>
        public static void updateAssetLocation(string getTagId,string locID)
        {
            string assetCurrentLoc = "";

            ztATdbLocalDataSet1.AssetInfoDataTableDataTable assetInfoDT = new ztATdbLocalDataSet1.AssetInfoDataTableDataTable();
            try
            {
                assetInfoTA.Fill(assetInfoDT, getTagId);
                if (assetInfoDT.Count > 0)
                {
                    assetCurrentLoc = assetInfoDT.Rows[0]["asset_location"].ToString();


                    //Asset last location is updated to asset current location
                    assetInfoTA.UpdateAssetLastLoc(assetCurrentLoc, getTagId);
                    //Finally update the asset location (current)
                    assetInfoTA.UpdateAssetLoc(locID, getTagId);
                }
                else
                {
                    //asset tag not found
                }


            }
            catch
            {
 
            }
 
        }

        /// <summary>
        /// Gets the location ID for the given reader ID.
        /// </summary>
        /// <param name="activeReaderID">The active reader ID.</param>
        /// <returns></returns>
        public static string getLocationID(string activeReaderID)
        {
            ztATdbLocalDataSet1.readerInfoDataTableDataTable readerLocTab = new ztATdbLocalDataSet1.readerInfoDataTableDataTable();
            string locID = "";   
            
            try
            {
                //Data adapter method to get locationID based on reader ID
                readerInfoTabAdpt.FillReaderID(readerLocTab, activeReaderID);
                if (readerLocTab.Count > 0)
                {
                    locID = readerLocTab.Rows[0]["location_id"].ToString().Trim();
                }
                else
                {
                    //locID = "";
                }
            }
            catch
            {
                //locID = "";
                // Problem retrieving location ID
                LogFile.Log("Error: Problem retrieving location ID from reader ID");
            }

            return locID;
        }

        /// <summary>
        /// Handles the Click event of the btnExit control.
        /// Closes the window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnRefresh control.
        /// Refresh the network connections.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            populateNetworkConnections();
        }
    }
}
