using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Management;

namespace ZTraka_App
{
    /// <summary>
    /// Class to generate the license key
    /// </summary>
    public class licenseKeyGenerate
    {

        /// <summary>
        /// Constructor of the <see cref="licenseKeyGenerate" /> class.
        /// </summary>
        public licenseKeyGenerate()
        {
 
        }

        /// <summary>
        /// Generates the license key.
        /// </summary>
        /// <returns></returns>
        public static string generateLicenseKey()
        {
            // Intialize variables
            string theKey = "";
            string tmpKey = "";
            string sourceString = "";

            //Get source string from hardware ID and user information provided
            sourceString = getKeyInformationString();

            //Use MD5 algo hash
            using (MD5 md5Hash = MD5.Create())
            {
                theKey = GetMd5Hash(md5Hash, sourceString);
            }

            //Key is 32 character key (HEX). Convert to upper and take any block of 16 digits
            tmpKey = theKey.Substring(9, 16).ToUpper(); //taken from the 9th index
            theKey = tmpKey.Substring(0, 4) + "-" + tmpKey.Substring(4, 4) + "-" + tmpKey.Substring(8, 4) + "-" + tmpKey.Substring(12, 4);

            //Generates the key in blocks of 4 characters followed by a hyphen.
            return theKey;
        }

        /// <summary>
        /// Generates the alternate license key.
        /// </summary>
        /// <returns></returns>
        public static string generateAlternateLicenseKey()
        {
            // Intialize variables
            string theKey = "";
            string tmpKey = "";
            string sourceString = "";

            //Get source string from user information provided
            sourceString = "ZT" + Register.registeredUsername + "ZT" + Register.registeredEmailAddress + "ZT";

            using (MD5 md5Hash = MD5.Create())
            {
                theKey = GetMd5Hash(md5Hash, sourceString);
            }

            //Key is 32 character key (HEX). Convert to upper and take any block of 16 digits
            //Generates the key in blocks of 4 characters followed by a hyphen.
            tmpKey = theKey.Substring(9, 16).ToUpper(); //taken from the 9th index
            theKey = tmpKey.Substring(0, 4) + "-" + tmpKey.Substring(4, 4) + "-" + tmpKey.Substring(8, 4) + "-" + tmpKey.Substring(12, 4);

            return theKey;
        }

        /// <summary>
        /// Gets the key information string.
        /// Gets from hardware sources as well as user information provided in the register form
        /// </summary>
        /// <returns></returns>
        public static string getKeyInformationString()
        {
            string keyString = "";
            //string 
            keyString += Register.registeredEmailAddress;
            keyString += "%";
            keyString += Register.registeredUsername;
            keyString += "%";
            keyString += getProcessorID();
            keyString += "%";
            keyString += getMotherboardID();
            keyString += "%";
            keyString += getHardDiskVolumeSerial();
            
            return keyString;
        }

        /// <summary>
        /// Gets the MD5 hash.
        /// </summary>
        /// <param name="md5Hash">The MD5 hash.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        /// <summary>
        /// Verifies the MD5 hash.
        /// Function only for future use in keygen maybe
        /// </summary>
        /// <param name="md5Hash">The MD5 hash.</param>
        /// <param name="input">The input.</param>
        /// <param name="hash">The hash.</param>
        /// <returns></returns>
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input. 
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the processor ID.
        /// WMI function (using system.management)
        /// </summary>
        /// <returns></returns>
        static string getProcessorID()
        {
            string id = "";
            try
            {
                ManagementObjectCollection mbsList = null;
                ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
                mbsList = mbs.Get();
                
                foreach (ManagementObject mo in mbsList)
                {
                    id = mo["ProcessorID"].ToString();
                }
            }
            catch
            {
 
            }
            return id;
        }

        /// <summary>
        /// Gets the motherboard ID.
        /// WMI function (using system.management)
        /// </summary>
        /// <returns></returns>
        static string getMotherboardID()
        {
            string mBoardSerial = "";
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                ManagementObjectCollection moc = mos.Get();

                foreach (ManagementObject mo in moc)
                {
                    mBoardSerial = (string)mo["SerialNumber"];
                }
            }
            catch
            {
 
            }
            return mBoardSerial;
        }

        /// <summary>
        /// Gets the hard disk volume serial.
        /// WMI function (using system.management)
        /// </summary>
        /// <returns></returns>
        static string getHardDiskVolumeSerial()
        {
            string volumeSerialHD = "";
            try
            {
                ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""c:""");
                dsk.Get();
                volumeSerialHD = dsk["VolumeSerialNumber"].ToString();
            }
            catch
            {
 
            }
            return volumeSerialHD;
        }
    }
}
