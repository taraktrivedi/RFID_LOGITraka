using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ZTraka_App
{
    public class readAppSettingValues
    {

        /// <summary>
        /// Constructor of the <see cref="readAppSettingValues" /> class.
        /// </summary>
        public readAppSettingValues()
        {
 
        }
        

        public static void readFromFile(string fileTextString)
        {
            string readerConnText;
            
            // Reader connections
            ////////////////////////////////////////////////////////////
            var readerConnRegex = new Regex(".*numberOfConnections=(.*);.*");
            if (readerConnRegex.IsMatch(fileTextString))
            {
                readerConnText = readerConnRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                int.TryParse(readerConnText, out NewConnection.numberOfConnections);
                if ((NewConnection.numberOfConnections <= 0) || (NewConnection.numberOfConnections >= 10) )
                {
                    NewConnection.numberOfConnections = 3; //default value
                }
            }
            else
            {
                NewConnection.numberOfConnections = 3; //default value
                
            }

            ///////////////////////////////////////////////////////////////////
            // Resize array
            Array.Resize(ref NewConnection.myConn, NewConnection.numberOfConnections);
            NewConnection.connStringDB.Clear();

            string[] rConnections = {};
            string[] connName = new string[4];
            int countTheConnections = NewConnection.numberOfConnections;
            

            readerConnRegex = new Regex(".*readerConnection=(.*);.*");

            Array.Resize(ref rConnections, readerConnRegex.Matches(fileTextString).Count);

            int cCount = 0;
            ////////////////////////////////////////////////////////////
            foreach(Match m in readerConnRegex.Matches(fileTextString))
            {
                
                readerConnText = m.Groups[1].Value.ToString().TrimEnd();
                // Check for valid connection string of reader
                if (!(readerConnText.CompareTo("(?)") == 0 || (string.IsNullOrEmpty(readerConnText))))
                {
                    rConnections[cCount] = readerConnText;
                    connName = readerConnText.Split('#');
                    NewConnection.connStringDB.Add(cCount, connName[0].TrimEnd());
                    cCount++;
                }
                
            }

            // finally copy the connections subject to a maximum of first N valid reader connections. (N= countTheConnections)
            if (countTheConnections > cCount)
            {
                countTheConnections = cCount;
            }

            for (int i = 0; i < countTheConnections; i++)
            {
                NewConnection.myConn[i] = rConnections[i]; 
            }
    
            ////////////////////////////////////////////////////////////

            string capturedText = "";

            // Reader setting parameters
            ////////////////////////////////////////////////////////////
            var patternRegex = new Regex(".*autoStopReadings=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                int.TryParse(capturedText, out ReaderSettings.autoStopReadings);
                if (ReaderSettings.autoStopReadings <= 0)
                {
                    ReaderSettings.autoStopReadings = 100; //default value
                }
            }
            else
            {
                ReaderSettings.autoStopReadings = 100; //default value
            }

            ////////////////////////////////////////////////////////////
            patternRegex = new Regex(".*tIntervalSec=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                int.TryParse(capturedText, out ReaderSettings.tIntervalSec);
                if (ReaderSettings.tIntervalSec == 0)
                {
                    ReaderSettings.tIntervalSec = 20; //default value
                }
            }
            else
            {
                ReaderSettings.tIntervalSec = 20; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*readerMode=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                int.TryParse(capturedText, out ReaderSettings.readerMode);
                if ( (ReaderSettings.readerMode <= 0) || (ReaderSettings.readerMode > 2))
                {
                    ReaderSettings.readerMode = 2; //default value
                }
            }
            else
            {
                ReaderSettings.readerMode = 2; //default value
            }
            ////////////////////////////////////////////////////////////

            //patternRegex = new Regex(".*beepControl=(.*);.*");
            //if (patternRegex.IsMatch(fileTextString))
            //{
            //    capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
            //    int.TryParse(capturedText, out ReaderSettings.beepControl);
            //    if ((ReaderSettings.beepControl <= 0) || (ReaderSettings.beepControl > 3))
            //    {
            //        ReaderSettings.beepControl = 2; //default value
            //    }
            //}
            //else
            //{
            //    ReaderSettings.beepControl = 2; //default value
            //}
            ////////////////////////////////////////////////////////////

            
            ////////////////////////////////////////////////////////////


            patternRegex = new Regex(".*isAutoStopEnabled=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out ReaderSettings.isAutoStopEnabled);

            }
            else
            {
                ReaderSettings.isAutoStopEnabled = false; //default value
            }
            ////////////////////////////////////////////////////////////
            // Alert notification settings 
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*capAssetsInOut=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.capAssetsInOut);
            }
            else
            {
                AlertNotificationSettings.capAssetsInOut = true; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*capAssetTagExpiry=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.capAssetTagExpiry);
            }
            else
            {
                AlertNotificationSettings.capAssetTagExpiry = true; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*capAssetMaintenanceDue=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.capAssetMaintenanceDue);
            }
            else
            {
                AlertNotificationSettings.capAssetMaintenanceDue = true; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*logsScannedAssets=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.logsScannedAssets);
            }
            else
            {
                AlertNotificationSettings.logsScannedAssets = true; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*logsCAP=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.logsCAP);
            }
            else
            {
                AlertNotificationSettings.logsCAP = true; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*assetUpdates=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.assetUpdates);
            }
            else
            {
                AlertNotificationSettings.assetUpdates = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*assetAdded=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.assetAdded);
            }
            else
            {
                AlertNotificationSettings.assetAdded = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*assetDeleted=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.assetDeleted);
            }
            else
            {
                AlertNotificationSettings.assetDeleted = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*userUpdates=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.userUpdates);
            }
            else
            {
                AlertNotificationSettings.userUpdates = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*userAdded=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.userAdded);
            }
            else
            {
                AlertNotificationSettings.userAdded = false; //default value
            }
            ////////////////////////////////////////////////////////////


            patternRegex = new Regex(".*userDeleted=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.userDeleted);
            }
            else
            {
                AlertNotificationSettings.userDeleted = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*readerUpdates=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.readerUpdates);
            }
            else
            {
                AlertNotificationSettings.readerUpdates = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*readerAdded=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.readerAdded);
            }
            else
            {
                AlertNotificationSettings.readerAdded = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*readerDeleted=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.readerDeleted);
            }
            else
            {
                AlertNotificationSettings.readerDeleted = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*sendEmailUser=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.sendEmailUser);
            }
            else
            {
                AlertNotificationSettings.sendEmailUser = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*userAdded=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.userAdded);
            }
            else
            {
                AlertNotificationSettings.userAdded = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*sendEmailAdmin=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.sendEmailAdmin);
            }
            else
            {
                AlertNotificationSettings.sendEmailAdmin = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*sendSMSUser=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.sendSMSUser);
            }
            else
            {
                AlertNotificationSettings.sendSMSUser = false; //default value
            }
            ////////////////////////////////////////////////////////////

            patternRegex = new Regex(".*sendSMSAdmin=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out AlertNotificationSettings.sendSMSAdmin);
            }
            else
            {
                AlertNotificationSettings.sendSMSAdmin = false; //default value
            }
            ////////////////////////////////////////////////////////////
            // Skin selection- Theme for the App
            patternRegex = new Regex(".*skinSelector=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                int.TryParse(capturedText, out SkinsWindow.skinSelect);
                if ((SkinsWindow.skinSelect < 0) || (SkinsWindow.skinSelect > 5))
                {
                    SkinsWindow.skinSelect = 1; //default value
                }
            }
            else
            {
                SkinsWindow.skinSelect = 1; //default value
            }
            ////////////////////////////////////////////////////////////
            // App system setting
            patternRegex = new Regex(".*minimizeInTray=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                bool.TryParse(capturedText, out HATrakaMain.minimizeInTray);
            }
            else
            {
                HATrakaMain.minimizeInTray = false; //default value
            }
            ////////////////////////////////////////////////////////////
            patternRegex = new Regex(".*shutdownAppAfterXHours=(.*);.*");
            if (patternRegex.IsMatch(fileTextString))
            {
                capturedText = patternRegex.Match(fileTextString).Groups[1].Value.ToString().Trim();
                int.TryParse(capturedText, out HATrakaMain.shutdownAppAfterXHours);
            }
            else
            {
                HATrakaMain.shutdownAppAfterXHours = 0; //default value
            }
            ////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////

        }



    }
}
