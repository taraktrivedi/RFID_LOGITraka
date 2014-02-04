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
    /// Interaction logic for ReadMeSystemReq.xaml
    /// </summary>
    public partial class ReadMeSystemReq : Window
        {

            /// <summary>
            /// Constructor of the <see cref="ReadMeSystemReq" /> class.
            /// </summary>
            public ReadMeSystemReq()
            {
                InitializeComponent();
            }

            /// <summary>
            /// Handles the Click event of the btnReadMeOK control.
            /// Closes the window
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
            private void btnReadMeOK_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
            }

       
       

        }
    }
