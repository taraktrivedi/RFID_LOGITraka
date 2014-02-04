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

namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for EmailSupport.xaml
    /// </summary>
    public partial class EmailSupport : Window
    {
        /// <summary>
        /// Constructor of the <see cref="EmailSupport" /> class.
        /// </summary>
        public EmailSupport()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the GotFocus event of the textBoxEmailSupportName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxEmailSupportName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxEmailSupportName.Text.Equals("Name"))
            {
                textBoxEmailSupportName.Text = "";
                textBoxEmailSupportName.Foreground = Brushes.Black;
                textBoxEmailSupportName.Background = Brushes.LightYellow;
            }
        }

        /// <summary>
        /// Handles the GotFocus event of the textBoxEmailSupportEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxEmailSupportEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxEmailSupportEmail.Text.Equals("Email"))
            {
                textBoxEmailSupportEmail.Text = "";
                textBoxEmailSupportEmail.Foreground = Brushes.Black;
                textBoxEmailSupportEmail.Background = Brushes.LightYellow;
            }
        }

        /// <summary>
        /// Handles the GotFocus event of the textBoxEmailSupportMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxEmailSupportMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxEmailSupportMessage.Text.Equals("Message"))
            {
                textBoxEmailSupportMessage.Text = "";
                textBoxEmailSupportMessage.Foreground = Brushes.Black;
                textBoxEmailSupportMessage.Background = Brushes.LightYellow;
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the textBoxEmailSupportMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxEmailSupportMessage_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxEmailSupportMessage.Text.Equals(""))
            {
                textBoxEmailSupportMessage.Text = "Message";
                textBoxEmailSupportMessage.Foreground = Brushes.Silver;
                textBoxEmailSupportMessage.Background = Brushes.White;
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the textBoxEmailSupportEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxEmailSupportEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxEmailSupportEmail.Text.Equals(""))
            {
                textBoxEmailSupportEmail.Text = "Email";
                textBoxEmailSupportEmail.Foreground = Brushes.Silver;
                textBoxEmailSupportEmail.Background = Brushes.White;
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the textBoxEmailSupportName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBoxEmailSupportName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxEmailSupportName.Text.Equals(""))
            {
                textBoxEmailSupportName.Text = "Name";
                textBoxEmailSupportName.Foreground = Brushes.Silver;
                textBoxEmailSupportName.Background = Brushes.White;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnEmailSupportCancel control.
        /// Closes the window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnEmailSupportCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnEmailSupportSendMail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnEmailSupportSendMail_Click(object sender, RoutedEventArgs e)
        {
            bool validateSupportEmail = false;
            string checkSuppEmail;
            //set cursor in wait mode
            Cursor = Cursors.Wait;
            checkSuppEmail = textBoxEmailSupportEmail.Text.ToString();


            validateSupportEmail = checkSupportEmail(checkSuppEmail);

            if (validateSupportEmail)
            {
                // trigger an email to info@zigtraka.com/tarak@zigtraka.com and show success in mail sending.
                sendEmailSuppSuccess(checkSuppEmail);

            }
            else
            {
                // Configure the message box to be displayed 
                string messageBoxText = "Invalid email address entered.Please try again !";
                string caption = "Invalid Email !";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Stop;
                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
                Cursor = Cursors.Arrow;
            }



        }



        /// <summary>
        /// Checks the support email.
        /// Validates email address.
        /// </summary>
        /// <param name="suppEmail">The supp email.</param>
        /// <returns></returns>
        private bool checkSupportEmail(string suppEmail)
        {

            // Check email pattern matching standard Regex expression.
            if (Regex.IsMatch(suppEmail, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// Sends the email supp message.
        /// </summary>
        /// <param name="suppEmailAddress">The supp email address.</param>
        private void sendEmailSuppSuccess(string suppEmailAddress)
        {
            string emailSubject, emailBody;
            emailSubject = "Feedback-Comments from " + textBoxEmailSupportName.Text.ToString().Trim();
            emailBody = htmlSupportEmailFormat();

            // Set gmail configuration for a/c info@zigtrka.com. This is sending email address
            MailMessage objMail = new MailMessage();
            var objSmtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("info@zigtraka.com", "adminzig19"),
                EnableSsl = true
            };
           
            //send the message
            try
            {
                objMail.From = new MailAddress(suppEmailAddress);
                objMail.To.Add("tarak@zigtraka.com");
                //objMail.CC.Add("tarak@zigtraka.com");// or info@zigtraka.com

                //set the content
                objMail.Subject = emailSubject;
                objMail.IsBodyHtml = true;
                objMail.Body = emailBody;
                objMail.IsBodyHtml = true;

                objSmtp.Send(objMail);
                LogFile.Log("Support email sent successfully");
                MessageBox.Show("Support email sent successfully!", "Thank you for contacting ZigTraka", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            
            
            }

            catch (Exception ex)
            {
                LogFile.Log("Error: Support Email not sent. Problem with internet connection");
                string fullLengthError = ex.ToString(); //Not used for now
                // Configure the message box to be displayed 
                string messageBoxTextNetworkerror = "There seems to be an issue with your internet connection. \n A Support email could not be sent. \n Please try again later.";
                string captionError = "Email Not Sent !";
                MessageBoxButton buttonError = MessageBoxButton.OK;
                MessageBoxImage iconError = MessageBoxImage.Warning;
                // Display message box
                MessageBox.Show(messageBoxTextNetworkerror, captionError, buttonError, iconError);
            }

            finally
            {
                Cursor = Cursors.Arrow;
                objMail = null;
                objSmtp = null;

            }
        }



        /// <summary>
        /// HTML support email format.
        /// </summary>
        /// <returns></returns>
        private string htmlSupportEmailFormat()
        {
            string emailHTMLstring = @" <html> <head> <title>Contact form for ZigTraka</title> </head> 
<body> <h1> <span style='color:#000080;'><span style='font-size: 24px;
'>Customer Message...</span></span></h1> <p> A customer has a Feedback/Suggestion for us:</p> <p> <strong><em><u>Customer Details </u></em></strong></p> <p> <em>Company Name:<strong>
" + textBoxEmailSupportName.Text.Trim() + @"</strong></em></p> <p> <em>Email Address<strong>: <u>
" + textBoxEmailSupportEmail.Text.Trim() + @"</u></strong></em></p> <p> <em><u>Message Details:</u></em></p> <p> <strong><em>
" + textBoxEmailSupportMessage.Text.Trim() + @"</em></strong></p> <br /> <hr /> <p> From,</p> <p> 
" + textBoxEmailSupportName.Text.Trim() + @"</p> <img alt='' src='http://i.imgur.com/PCL2R.png' /> </body> </html> ";

            return emailHTMLstring;
        }
       
    }
}
