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
    /// Interaction logic for reportOptions.xaml
    /// </summary>
    public partial class reportOptions : Window
    {
        /// <summary>
        /// Constructor of the <see cref="reportOptions" /> class.
        /// </summary>
        public reportOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the btnApply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            // *change* here
            // Modify this for *future* options use.
            this.Close();
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
        /// Handles the Loaded event of the rptOpt control.
        /// Loads initial settings
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void rptOpt_Loaded(object sender, RoutedEventArgs e)
        {

            radioButtonUserGenerated.IsChecked = true;
        }
    }
}
