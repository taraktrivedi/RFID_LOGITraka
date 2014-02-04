using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace ZTraka_App
{
    /// <summary>
    /// Class SMSClass for delivering SMS
    /// </summary>
    public class SMSClass
    {

        /// <summary>
        /// Constructor of the <see cref="SMSClass" /> class.
        /// </summary>
        public SMSClass()
        {
 
        }

        /// <summary>
        /// Sends the SMS.
        /// SMS sent to the user with valid mobile number.
        /// </summary>
        /// <param name="alertMessage">The alert message.</param>
        /// <param name="cellNumber">The cell number.</param>
        public static void sendSMS(string alertMessage, string cellNumber)
        {
            
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(@"http://ubaid.tk/sms/sms.aspx?uid=" +
            "9765419066" + "&pwd=" + "NUrhR2R84J" +
            "&msg=" + alertMessage + "&phone=" + cellNumber + "&provider=fullonsms");

            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
            string responseString = respStreamReader.ReadToEnd();
            respStreamReader.Close();
            myResp.Close();

            if (responseString.CompareTo("1") == 0)
            {
                LogFile.Log("SMS sent to user successfully ! Message: " + alertMessage);
                // Write SMS sent successfully
            }
            else
            {
                LogFile.Log("Error: SMS could not be sent to user. Reason: Busy server or no internet connection.");
                // Problem. Server or any other..SMS NOT sent
            }
        }

        public static bool checkMobileNumber(string mobileNumber)
        {
            // Check email pattern matching standard Regex expression.
            if (Regex.IsMatch(mobileNumber, @"^([1-9]{1})([0-9]{9})$"))
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
