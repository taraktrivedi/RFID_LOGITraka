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
    /// Interaction logic for SkinsWindow.xaml
    /// </summary>
    public partial class SkinsWindow : Window
    {
        // Global variable for theme selection settings
        public static int skinSelect = 1;

        /// <summary>
        /// Constructor of the <see cref="SkinsWindow" /> class.
        /// </summary>
        public SkinsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the btnApply control.
        /// Applies the theme as selected by the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            // Save logic here:..
            //Application.Current.Resources.Source = new Uri("./WPFThemes/ShinyRed.xaml", UriKind.RelativeOrAbsolute);
          
            if (radioButtonDefaultTheme.IsChecked == true)
            {
                
                //Set new themes here
                // This is a theme with default wpf setting
                Application.Current.Resources.Source = new Uri("./WPFThemes/DefaultTheme.xaml", UriKind.RelativeOrAbsolute);
                skinSelect = 0;
            }
            else if (radioButtonTheme1.IsChecked == true)
            {
                // This is the theme selected for the application 
                Application.Current.Resources.Source = new Uri("./WPFThemes/myCustomTheme.xaml", UriKind.RelativeOrAbsolute);
                skinSelect = 1;
            }
            else if (radioButtonTheme2.IsChecked == true)
            {
                Application.Current.Resources.Source = new Uri("./WPFThemes/ExpressionLight.xaml", UriKind.RelativeOrAbsolute);
                skinSelect = 2;
            }
            else if (radioButtonTheme3.IsChecked == true)
            {
                Application.Current.Resources.Source = new Uri("./WPFThemes/BureauBlue.xaml", UriKind.RelativeOrAbsolute);
                skinSelect = 3;

            }
            else if (radioButtonTheme4.IsChecked == true)
            {
                Application.Current.Resources.Source = new Uri("./WPFThemes/WhistlerBlue.xaml", UriKind.RelativeOrAbsolute);
                skinSelect = 4;

            }
            else if (radioButtonTheme5.IsChecked == true)
            {
                Application.Current.Resources.Source = new Uri("./WPFThemes/ExpressionDark.xaml", UriKind.RelativeOrAbsolute);
                skinSelect = 5;
            }
            
            this.Close();
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
        /// Handles the Loaded event of the themeWindow control.
        /// Load the theme according to the theme selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void themeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (skinSelect == 0)
            {
                //Set radio button accordingly
                radioButtonDefaultTheme.IsChecked = true;

            }
            else if (skinSelect == 1)
            {

                radioButtonTheme1.IsChecked = true;

            }
            else if (skinSelect == 2)
            {

                radioButtonTheme2.IsChecked = true;

            }
            else if (skinSelect == 3)
            {

                radioButtonTheme3.IsChecked = true;


            }
            else if (skinSelect == 4)
            {

                radioButtonTheme4.IsChecked = true;


            }
            else if (skinSelect == 5)
            {

                radioButtonTheme5.IsChecked = true;

            }
        }

        /// <summary>
        /// Function which Selects the theme for app.
        /// </summary>
        public static void selectThemeforApp()
        {

            if (skinSelect == 0)
            {
                Application.Current.Resources.Source = new Uri("./WPFThemes/DefaultTheme.xaml", UriKind.RelativeOrAbsolute);

            }
            else if (skinSelect == 1)
            {
                // This is the selected theme for the application
                Application.Current.Resources.Source = new Uri("./WPFThemes/myCustomTheme.xaml", UriKind.RelativeOrAbsolute);

            }
            else if (skinSelect == 2)
            {

                Application.Current.Resources.Source = new Uri("./WPFThemes/ExpressionLight.xaml", UriKind.RelativeOrAbsolute);

            }
            else if (skinSelect == 3)
            {

                Application.Current.Resources.Source = new Uri("./WPFThemes/BureauBlue.xaml", UriKind.RelativeOrAbsolute);


            }
            else if (skinSelect == 4)
            {

                Application.Current.Resources.Source = new Uri("./WPFThemes/WhistlerBlue.xaml", UriKind.RelativeOrAbsolute);


            }
            else if (skinSelect == 5)
            {

                Application.Current.Resources.Source = new Uri("./WPFThemes/ExpressionDark.xaml", UriKind.RelativeOrAbsolute);

            }

        }

    }
}
