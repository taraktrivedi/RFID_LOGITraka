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
using System.Windows.Navigation;

namespace ZTraka_App
{

    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {


        /// <summary>
        /// Constructor of the <see cref="About" /> class.
        /// </summary>
        public About()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the btnOkAbout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnOkAbout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// Sets the build datetime,version and revision numbers
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var Bversion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            //Formula to get the build DateTime
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
            TimeSpan.TicksPerDay * Bversion.Build + // days since 1 January 2000
            TimeSpan.TicksPerSecond * 2 * Bversion.Revision)); // seconds since midnight, (multiply by 2 to get original)

            lblBuildNumber.Content = Bversion.Build.ToString() + "  Revision: " + Bversion.Revision.ToString();

            lblVersionNumber.Content = Bversion.ToString() + "  Built on: " + buildDateTime.ToString();
            

        }

        /// <summary>
        /// Handles the RequestNavigate event of the Hyperlink control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RequestNavigateEventArgs" /> instance containing the event data.</param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            this.Close();
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
