using System;
using System.Threading;
using System.Globalization;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ZTraka_App
{
    public partial class App : Application
    {
        //private static bool appON = true;   
        //public static HATrakaMain hatraka;
        /// <summary>
        /// Constructor of the <see cref="App" /> class.
        /// </summary>
        public App()
        {
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));


            //var dtFormat = new DateTimeFormatInfo();
            //dtFormat.AMDesignator = "AM";
            //dtFormat.
            //dtFormat.UniversalSortableDateTimePattern;
            Thread.CurrentThread.CurrentCulture.DateTimeFormat = DateTimeFormatInfo.InvariantInfo;
            //Thread.CurrentThread.CurrentCulture.DateTimeFormat = dtFormat;
            
        }

        /// <summary>
        /// Handles the Startup event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupEventArgs" /> instance containing the event data.</param>
        public void App_Startup(object sender, StartupEventArgs e)
        {                
            //hatraka.Hide();
            HATrakaMain hatraka = new HATrakaMain();
            //hatraka.BeginInit();
            //hatraka.Show();
            //hatraka.Hide();
            //hatraka.Visibility = Visibility.Hidden;

            // get login window at start-up
            LoginWindow logon = new LoginWindow();
            logon.ShowDialog();

           // logon.loadTheMainWindow();
            
 
        }

    }
}
