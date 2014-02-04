using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.SqlServer.MessageBox;
using System.Windows.Threading;

namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public static string registeredEmailAddress = "";
        public static string registeredUsername = "";
        public static string Ihazthekeys = "";
        public static string Ihazanotherkeys = "";

        /// <summary>
        /// Constructor of the <see cref="Register" /> class.
        /// </summary>
        public Register()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// Sets the focus on Username textbox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxRegistrationUsername.Focus();
            if (Properties.Settings.Default.isProductActivated == true)
            {
                MessageBox.Show("Product has already been activated !", "Activation already done", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            this.Cursor = Cursors.Wait;
            if (Properties.Settings.Default.keysDB.Count == 1) // By default it is one.
            {
                //Store the keys in string collection DB
                Ihazthekeys = licenseKeyGenerate.generateLicenseKey();
                Properties.Settings.Default.keysDB.Add(Ihazthekeys);

                Ihazanotherkeys = licenseKeyGenerate.generateAlternateLicenseKey();
                Properties.Settings.Default.keysDB.Add(Ihazanotherkeys);
            }
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the Click event of the btnCancelRegistration control.
        /// Closes the window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancelRegistration_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnRegister control.
        /// Checks registration code and valid email address before registering user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.isProductActivated == true)
            {
                MessageBox.Show("Product has already been activated !", "Activation already done",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }

            // Initialize local variables
            bool validateRegistrationCode = false;
            bool validateRegistrationEmail = false;
            string sixteendigitcode, checkRegEmail;

            // Get the textbox values into string variables
            checkRegEmail = textBoxRegistrationEmail.Text.ToString().Trim();
            sixteendigitcode = textBoxRegistrationCode.Text.ToString().Trim();

            //Set cursor in wait mode
            this.Cursor = Cursors.Wait;

            //Get bool values for valid email and 16-digit reg code
            validateRegistrationEmail = checkRegistrationEmail(checkRegEmail);
            validateRegistrationCode = checkRegistrationCode(sixteendigitcode);
            
            if (validateRegistrationCode && validateRegistrationEmail)
            {
                //Set variable to true for product registeration.
                Properties.Settings.Default.isProductActivated = true;
                Properties.Settings.Default.Save();

                // Dispatcher timer to control UI and enable full feature
               HATrakaMain.productTimer.Start();

                // actually trigger an email to info@zigtraka.com and show success in registration and trigger success mail as well to company.
                //sendEmailRegSuccess(checkRegEmail);


                // Configure the message box to be displayed 
                string messageBoxText = "Your product has now been activated successfully !";
                string caption = "Activation Success !";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Exclamation;
                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
                this.Close();
            }
            else
            {
                
                // Configure the message box to be displayed 
                string messageBoxText = "Incorrect registration key or invalid email address entered....               Please try again !";
                string caption = "Activation Failed !";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Stop;
                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
                Cursor = Cursors.Arrow;
                // ADD code to clear text boxes etc...
            }

            
        }

        /// <summary>
        /// Checks the registration code.
        /// </summary>
        /// <param name="regCode">The reg code.</param>
        /// <returns></returns>
        private bool checkRegistrationCode(string regKeys)
        {
            // See whether collection contains this key code.
            if (Properties.Settings.Default.keysDB.Contains(regKeys))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the registration email.
        /// Regex pattern matching.
        /// </summary>
        /// <param name="regEmail">The reg email.</param>
        /// <returns></returns>
        private bool checkRegistrationEmail(string regEmail)
        {
            
            // See whether pattern of regex email check matches
            if (Regex.IsMatch(regEmail, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sends the email of registration success.
        /// </summary>
        /// <param name="regEmailAddress">The reg email address.</param>
        private void sendEmailRegSuccess(string regEmailAddress)
        {
            string emailSubject, emailBody;
            emailSubject = "Registration Success for " + textBoxRegistrationUsername.Text.Trim();
            emailBody = htmlEmailFormat();

            // Gmail configuration: Send mail through email info@zigtraka.com
            MailMessage objMail = new MailMessage();
            var objSmtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("info@zigtraka.com", "adminzig19"),
                EnableSsl = true
            };
            
            //send the message
            try
            {
                objMail.From = new MailAddress("info@zigtraka.com");
                objMail.To.Add(regEmailAddress);
                //objMail.CC.Add("tarak@zigtraka.com");

                //set the content
                objMail.Subject = emailSubject;
                objMail.IsBodyHtml = true;
                objMail.Body = emailBody;
                objMail.IsBodyHtml = true;

                objSmtp.Send(objMail);
                LogFile.Log("Registration email sent successfully");
                MessageBox.Show("Registration Mail sent successfully! You will shortly receive an email with the product activation details","Mail Sent",MessageBoxButton.OK,MessageBoxImage.Information);
            }

            catch (Exception ex)
            {
                LogFile.Log("Error: Registration Email not sent. Problem with internet connection");
                string fullLengthError = ex.ToString(); //Not used for now
                // Configure the message box to be displayed 
                string messageBoxTextNetworkerror = "There seems to be an issue with your internet connection. \n A Registration eMail could not be sent. \n Please try again later.";
                string captionError = "Email Not Sent !";
                MessageBoxButton buttonError = MessageBoxButton.OK;
                MessageBoxImage iconError = MessageBoxImage.Warning;
                // Display message box
                MessageBox.Show(messageBoxTextNetworkerror, captionError, buttonError, iconError);

            }

            finally
            {
                // Set the cursor look to default and dispose objects
                Cursor = Cursors.Arrow;
                objMail = null;
                objSmtp = null;
                
            }
        }

        
        /// <summary>
        /// HTMLs the email format.
        /// This function contains the HTML string based on the HTML page
        /// </summary>
        /// <returns></returns>
        private string htmlEmailFormat()
        {

            string emailHTMLstring = @" <html> <head> <title></title> </head> <body> <div> <span style='font-size: 22px;'> <span style='color:#0000cd;'> Registration success for
" + textBoxRegistrationUsername.Text.ToString() + @" 
                                             !</span></span></div> <div> <span style='color:#0000cd;'> <span style='font-size: 22px;'>Thank 
                                        you for registering our product HATraka !</span></span></div> <div> &nbsp;
                                    </div> <div> Dear Customer,</div> <div> Welcome to ZigTraka Solutions and 
                                    the world of unlimited opportunites with RFID asset tracking.</div> <div>
                                HATraka is a product specially designed for Hospital Asset Tracking management 
system.</div> <div> All your asset tracking needs can be solved with this application software.</div> <div> 
For use of the software please refer to the references and documentation section of the application.</div>
Please use the following details to activate the product.<div> &nbsp;</div> <div> <strong>Registration details:</strong></div> <div> &nbsp;</div> <div> <em>Username: 
" + textBoxRegistrationUsername.Text.ToString() + @"</em></div> <div> <em>Company: 
" + textBoxRegistrationCompany.Text.ToString() + @"</em></div> <div> <em>Email: 
" + textBoxRegistrationEmail.Text.ToString() + @"</em></div> <div> <em>Registration Keys: 
" + Ihazthekeys + @"</em></div> <div> &nbsp;</div> <div> Please feel free to ask any question or 
suggestions related to the product.&nbsp;</div> <div> If you need assistance about the use of software please 
call us on +919765419066 or email us 
at <a href='mailto:info@zigtraka.com?subject=Feedback%20regarding%20software%20product%20HATraka%20!&amp;body=%3CDear%20Customer%2CPlease%20send%20us%20you%20valuable%20feedback%20here%3E'>info@zigtraka.com</a></div> <div> 
Visit our website at <a href='http://www.zigtraka.com'>www.zigtraka.com</a></div> <div> &nbsp;
</div> <div> Best Regards,</div> <div> Team ZigTraka&nbsp;</div> <div> <hr /> <p> 
<span style='color: rgb(105, 105, 105); '>This is an auto generated email please do not reply to this 
email&nbsp;</span></p> </div>
<img alt='' src='http://i.imgur.com/ePaoZ.png' style='width: 100px; height: 107px;' />
</body> </html> ";

            return emailHTMLstring;
        }

        /// <summary>
        /// Handles the Click event of the btnRequestKeys control.
        /// Sends email to zigtraka id..with the keys. ZigTraka will fwd the email to customer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnRequestKeys_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            //string getTheKey = "";
            registeredEmailAddress = textBoxRegistrationEmail.Text.Trim().ToString();
            registeredUsername = textBoxRegistrationUsername.Text.Trim().ToString();

            if (checkRegistrationEmail(registeredEmailAddress) && (!string.IsNullOrEmpty(registeredUsername)))
            {
                //getTheKey = licenseKeyGenerate.generateLicenseKey();
                // Send to the company
                sendEmailRegSuccess("tarak@zigtraka.com");
            }
            else
            {
                //Please enter email and username correctly
                // Configure the message box to be displayed 
                string messageBoxText = "Please enter a valid email address and a username....               Please try again !";
                string caption = "Invalid or blank fields !";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Stop;
                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }

            this.Cursor = Cursors.Arrow;

        }

    

    }
}
