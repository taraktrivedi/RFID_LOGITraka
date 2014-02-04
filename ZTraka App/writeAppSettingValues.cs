using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace ZTraka_App
{
    public class writeAppSettingValues
    {

        public writeAppSettingValues()
        {
 
        }

        public static string writeToFile()
        {
            
            //var text = "Your string";
            //text += "some other text";
            //File.WriteAllText(saveFileDialog1.FileName, text);

            using (var writer = new StringWriter())
            {
                //writer.
                writer.WriteLine("%%%%%%%%%%%%%%%%HATraka app settings file%%%%%%%%%%%");
                writer.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\r\n");
                writer.WriteLine("%% Warning: Do not modify the settings file unless you are sure of what changes are to be done!");
                writer.WriteLine("%% App Settings file created on: " + DateTime.Now.ToString() + " \r\n");
                writer.WriteLine("%%%%%%%%%%%Reader connections%%%%%%%%%%%%%%%%%%%%%%%");
                writer.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\r\n");
                writer.WriteLine("numberOfConnections=" + NewConnection.numberOfConnections.ToString() + ";");

                for (int i = 0; i < NewConnection.numberOfConnections; i++)
                {
                    if (!(string.IsNullOrEmpty(NewConnection.myConn[i])))
                    {
                        writer.WriteLine("readerConnection=" + NewConnection.myConn[i].ToString() + ";");
                    }
                    else
                    {
                        writer.WriteLine("readerConnection=(?);");
                    }
                }
             
                writer.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%5%%%%%%%%%%%\r\n");
                writer.WriteLine("%%%%%%%%%%%Reader settings%%%%%%%%%%%%%%%%%%%%%%%%%%");
                writer.WriteLine("tIntervalSec=" + ReaderSettings.tIntervalSec.ToString() + ";");
                writer.WriteLine("autoStopReadings=" + ReaderSettings.autoStopReadings.ToString() + ";");
                writer.WriteLine("readerMode=" + ReaderSettings.readerMode.ToString() + ";");
                
                
                writer.WriteLine("isAutoStopEnabled=" + ReaderSettings.isAutoStopEnabled.ToString() + ";");
                writer.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\r\n");
                writer.WriteLine("%%%%%%%%%%%Alert Notification settings%%%%%%%%%%%%%%");
                writer.WriteLine("capAssetsInOut=" + AlertNotificationSettings.capAssetsInOut.ToString() + ";");
                writer.WriteLine("capAssetTagExpiry=" + AlertNotificationSettings.capAssetTagExpiry.ToString() + ";");
                writer.WriteLine("capAssetMaintenanceDue=" + AlertNotificationSettings.capAssetMaintenanceDue.ToString() + ";");
                writer.WriteLine("logsScannedAssets=" + AlertNotificationSettings.logsScannedAssets.ToString() + ";");
                writer.WriteLine("logsCAP=" + AlertNotificationSettings.logsCAP.ToString() + ";");
                writer.WriteLine("assetUpdates=" + AlertNotificationSettings.assetUpdates.ToString() + ";");
                writer.WriteLine("assetAdded=" + AlertNotificationSettings.assetAdded.ToString() + ";");
                writer.WriteLine("assetDeleted=" + AlertNotificationSettings.assetDeleted.ToString() + ";");
                writer.WriteLine("userUpdates=" + AlertNotificationSettings.userUpdates.ToString() + ";");
                writer.WriteLine("userAdded=" + AlertNotificationSettings.userAdded.ToString() + ";");
                writer.WriteLine("userDeleted=" + AlertNotificationSettings.userDeleted.ToString() + ";");
                writer.WriteLine("readerUpdates=" + AlertNotificationSettings.readerUpdates.ToString() + ";");
                writer.WriteLine("readerAdded=" + AlertNotificationSettings.readerAdded.ToString() + ";");
                writer.WriteLine("readerDeleted=" + AlertNotificationSettings.readerDeleted.ToString() + ";");
                writer.WriteLine("sendEmailUser=" + AlertNotificationSettings.sendEmailUser.ToString() + ";");
                writer.WriteLine("sendEmailAdmin=" + AlertNotificationSettings.sendEmailAdmin.ToString() + ";");
                writer.WriteLine("sendSMSUser=" + AlertNotificationSettings.sendSMSUser.ToString() + ";");
                writer.WriteLine("sendSMSAdmin=" + AlertNotificationSettings.sendSMSAdmin.ToString() + ";");
                writer.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\r\n");
                writer.WriteLine("%%%%%%%%% App Theme Select %%%%%%%%%%%%%%%%%%%%%%%%%");
                writer.WriteLine("skinSelector=" + SkinsWindow.skinSelect.ToString() + ";");
                writer.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                writer.WriteLine("%%%%%%%%% App System settings %%%%%%%%%%%%%%%%%%%%%%%%%");
                writer.WriteLine("minimizeInTray=" + HATrakaMain.minimizeInTray.ToString() + ";");
                writer.WriteLine("shutdownAppAfterXHours=" + HATrakaMain.shutdownAppAfterXHours.ToString() + ";");
                writer.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                
                return writer.GetStringBuilder().ToString();
            }
        }

    }


}
