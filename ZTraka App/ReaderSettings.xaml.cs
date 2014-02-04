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
    /// Interaction logic for ReaderSettings.xaml
    /// </summary>
    public partial class ReaderSettings : Window
    {

        public static int tIntervalSec = 20;
        public static int autoStopReadings = 100;
        public static int readerMode = 2;
        public static int beepControl = 2;
        
        public static bool isAutoStopEnabled = false;

        /// <summary>
        /// Constructor of the <see cref="ReaderSettings" /> class.
        /// </summary>
        public ReaderSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// Loads the saved or the default values
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            textBoxtimePeriod.Text = tIntervalSec.ToString();
            textBoxAutoStopReadings.Text = autoStopReadings.ToString();
            //////////////////////////////////////////
            if (readerMode == 2)
            {
                radioButtonMultipleRead.IsChecked = true;
            }
            else 
            {
                radioButtonSingleRead.IsChecked = true;
            }

            //////////////////////////////////////////

            if (beepControl == 1)
            {
                radioButtonYes.IsChecked = true;
            }

            else if (beepControl == 2)
            {
                radioButtonNo.IsChecked = true;
            }
            else
            {
                radioButtonAuto.IsChecked = true;
            }
            //////////////////////////////////////////
            if (isAutoStopEnabled)
            {
                checkBoxAutoStop.IsChecked = true;
            }
            else
            {
                checkBoxAutoStop.IsChecked = false;
            }


        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// Closes the window without saving and changes
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// Saves the changes and closes the window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            
            tIntervalSec = Int32.Parse(textBoxtimePeriod.Text);
            autoStopReadings = Int32.Parse(textBoxAutoStopReadings.Text);

            /////////////////////////////////////////////////
            if (radioButtonSingleRead.IsChecked == true)
            {
                readerMode = 1;
            }

            else //Multiple Reads
            {
                readerMode = 2;
            }
            /////////////////////////////////////////////////

            if (radioButtonYes.IsChecked == true)
            {
                beepControl = 1;
                
            }
            else if (radioButtonNo.IsChecked == true)
            {
                beepControl = 2;
                
            }
            else
            {
                beepControl = 3;
                
            }
            /////////////////////////////////////////////////

            
            if (checkBoxAutoStop.IsChecked == true)
            {
                isAutoStopEnabled = true;
            }
            else
            {
                isAutoStopEnabled = false;
            }

            this.Close();
        }

        /// <summary>
        /// Handles the PreviewTextInput event of the textBoxtimePeriod control.
        /// Make sure only numbers are input in the textbox time period
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs" /> instance containing the event data.</param>
        private void textBoxtimePeriod_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the PreviewTextInput event of the textBoxAutoStopReadings control.
        /// Make sure only numbers are input in the textbox AutoStop Readings
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs" /> instance containing the event data.</param>
        private void textBoxAutoStopReadings_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        
    }
}
