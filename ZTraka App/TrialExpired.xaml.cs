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
    /// Interaction logic for TrialExpired.xaml
    /// </summary>
    public partial class TrialExpired : Window
    {
        /// <summary>
        /// Constructor of the <see cref="TrialExpired" /> class.
        /// </summary>
        public TrialExpired()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the buttonExit control.
        /// Save app settings and shutdown
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        /// <summary>
        /// Handles the Click event of the buttonRegisterHere control.
        /// Jump to register screen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonRegisterHere_Click(object sender, RoutedEventArgs e)
        {
            Register reg = new Register();
            reg.Show();
            //Check for product activated,..if yes close this window..return to normal
            if (Properties.Settings.Default.isProductActivated == true)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Handles the Closed event of the appExpiredWindow control.
        /// Shuts down the app if product is not activated
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void appExpiredWindow_Closed(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.isProductActivated == false)
            {
                //Save settings
                Properties.Settings.Default.Save();

                LogFile.Log("User cancelled registration of product. App not started !");
                LogFile.Log("App Close Event -Done! Time: " + DateTime.Now.ToLongTimeString());
                this.Close();

                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Handles the Activated event of the appExpiredWindow control.
        /// Checks whether the product has been activated
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void appExpiredWindow_Activated(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.isProductActivated == true)
            {
                LogFile.Log("The product has now been activated successfully !");
                this.Close();
            }

        }

        
    }
}
