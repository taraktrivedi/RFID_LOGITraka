using System;
using System.Threading;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO.Ports;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DL580_9130;

namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        /// <summary>
        /// Constructor of the <see cref="HomeWindow" /> class.
        /// </summary>
        public HomeWindow()
        {       
            InitializeComponent();
            //Thread.Sleep(5000);
        }


        /// <summary>
        /// Handles the Loaded event of the appHomeWindow control.
        /// Home app window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void appHomeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Flow diagram.  WPF

        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            //sendOutputSignalBeep();
        }

        private void btnStopSending_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Sends the output signal to the serial port
        /// </summary>
        public static void sendOutputSignalBeep()
        {
            // all of the options for a serial device
            // can be sent through the constructor of the SerialPort class
            // PortName = "COM7", Baud Rate = 19200, Parity = None, 
            // Data Bits = 8, Stop Bits = One, Handshake = None
            int flag1 = 0;
            SerialPort sp = new SerialPort("COM7", 2400, Parity.None, 8, StopBits.One);
            //sp.Handshake = Handshake.None;
            sp.Open();
            
                //Thread.Sleep(200);
                if (flag1 == 0)
                {
                    sp.Write(new byte[] { 0xAB, 0xCD, 0xEF }, 0, 3);
                    flag1 = 1;
                }

                else
                {
                    sp.Write(new byte[] { 0x00, 0x02, 0x04 }, 0, 3);
                    flag1 = 0;
                }
            
            sp.Close();
            //HATrakaMain.outputSignal.Stop();

        }

    }


}
