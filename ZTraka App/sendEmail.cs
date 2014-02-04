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
    /// Send Email Class. Interaction logic here.
    /// </summary>
    public class sendEmail
    {
        /// <summary>
        /// Constructor of the <see cref="sendEmail" /> class.
        /// </summary>
        public sendEmail()
        { 
            
        }

        /// <summary>
        /// Sends the email message.
        /// </summary>
        /// <param name="emailContent">The email Content.</param>
        public static void sendEmailMessage(string emailContent)
        {
            string toEmailAdd = "";

            toEmailAdd = LoginWindow.user_email;
            // toEmailAdd = LoginWindow.email //The user email

            string fromEmailAddress = "ApplicationAlerts@HATraka.com"; //From the application or info@zigtraka.com
                        
            string emailSubject, emailBody;
            emailSubject = "Alert Notifications: " + "Auto generated";
            emailBody = htmlEmailFormat(emailContent);

            //Set the gmail configuration for a/c info@zigtraka.com
            MailMessage objMail = new MailMessage();
            var objSmtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("info@zigtraka.com", "adminzig19"),
                EnableSsl = true
            };
            
            //send the message
            try
            {
                objMail.From = new MailAddress(fromEmailAddress);
                objMail.To.Add(toEmailAdd);
                if (AlertNotificationSettings.sendEmailAdmin == true)
                {
                    objMail.CC.Add(HATrakaMain.superAdminEmail); //
                }
                

                //set the content
                objMail.Subject = emailSubject;
                objMail.IsBodyHtml = true;
                objMail.Body = emailBody;
                objMail.IsBodyHtml = true;

                objSmtp.Send(objMail);
                
                LogFile.Log("Alert email sent successfully");
            }

            catch
            {
                LogFile.Log("Error: Email not sent. Problem with internet connection");
                
            }

            finally
            {
               
                objMail = null;
                objSmtp = null;

            }
        }

        /// <summary>
        /// HTMLs the email format.
        /// </summary>
        /// <param name="emailContent">Content or body of the email.</param>
        /// <returns></returns>
        public static string htmlEmailFormat(string emailContent)
        {
            string emailMessage = emailContent;
            string emailHTMLstring = @" <html> <head> <title>Alert email</title> </head> 
<body> <h1> <span style='color:#000080;'><span style='font-size: 22px;
'>Alert Message...</span></span></h1> <p> There is an alert notification for you:</p> <p> 
&nbsp;</p> <p> <strong><em><u>Message Details: </u></em></strong></p> <p> <em><strong>
" + emailMessage + " ::Timestamp - " + DateTime.Now.ToString() + @"</strong></em></p> <div> </div> <div> &nbsp;
</div> <div> From,</div> <div> Team ZigTraka - Application HATraka&nbsp;</div> <div> <hr /> <p> 
<span style='color: rgb(105, 105, 105); '>This is an auto generated email please do not reply to this 
email&nbsp;</span></p> </div> 
<img alt='' src='http://i.imgur.com/ePaoZ.png' style='width: 100px; height: 107px;' />
</body> </html> ";

            return emailHTMLstring;
        }


        /// <summary>
        /// Checks the email.
        /// Validates email address.
        /// </summary>
        /// <param name="suppEmail">The supp email.</param>
        /// <returns></returns>
        public static bool checkEmail(string emailID)
        {

            // Check email pattern matching standard Regex expression.
            if (Regex.IsMatch(emailID, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
