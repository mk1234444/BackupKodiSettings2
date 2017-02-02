using System;
using Android.Content;
using Android.Widget;
using System.IO;
using Android.Content.PM;
using Android.App;
using System.Threading.Tasks;
using Java.Net;
//using Java.IO;

namespace BackupKodiSettings
{
    public static class Helpers
    {
        /// <summary>
        /// Creates the backup folder if it doesn't already exist
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool CreateSBackupFolderIfNotExists(Context context,string fullPath)
        {
            if (!Directory.Exists(fullPath))
            {
                // var path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                DirectoryInfo dirInfo;
                try
                {
                    dirInfo = Directory.CreateDirectory(fullPath);
                    return dirInfo!=null;
                }
                catch (Exception)
                {
                    Toast.MakeText(context, $"An Exception was thrown when trying to create Save Folder", ToastLength.Short).Show();
                }
            }

            // Save folder already exists
            return true;
        }

        public static bool IsAppInstalled(Context context,string packageName)
        {
            PackageManager pm = context.PackageManager;
            bool installed = false;
            try
            {
                pm.GetPackageInfo(packageName, PackageInfoFlags.Activities);
                installed = true;
            }
            catch (PackageManager.NameNotFoundException)
            {
                installed = false;
            }
            return installed;
        }

        public static string ReadFavourites(string favouritesPath)
        {
            if(!File.Exists(favouritesPath)) return null;
            return File.ReadAllText(favouritesPath);
        }

        public static string ReadXonfSettings(string xonfluencePath)
        {
            if (!File.Exists(xonfluencePath)) return null;
            return File.ReadAllText(xonfluencePath);
        }


        /// <summary>
        /// This example demonstrates how to use I/O classes to synchronously copy the contents of a
        /// directory to another location. In this example, the user can specify whether to also copy
        /// the subdirectories. If the subdirectories are copied, the method in this example recursively
        /// copies them by calling itself on each subsequent subdirectory until there are no more to copy.
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) return false;

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
            return true;
        }

        public static void ShowDialog(Context context, string message)
        {
            AlertDialog alertDialog = new AlertDialog.Builder(context).Create();
            alertDialog.SetMessage(message);
            alertDialog.SetButton("OK", (s, e) =>
            {
                alertDialog.Dismiss();
            });
            alertDialog.Show();
        }

        public static async Task<string> GetPulsePin()
        {
                string pin = await Task.Run(async () => {
                string pulsePin = null;
                   
                // Create a URL for the desired page
                URL url = new Java.Net.URL("https://ares-project.uk/showpin.php?action=getbuildpin");

                // Read all the text returned by the server
                using (Java.IO.BufferedReader input = new Java.IO.BufferedReader(new Java.IO.InputStreamReader(url.OpenStream())))
                {
                    string s1 = null;
                    while ((s1 = await input.ReadLineAsync()) != null)
                    {
                        if (s1.Contains("Pin = "))
                        {
                            pulsePin = s1.Substring(s1.IndexOf("Pin = "), 10).Substring(6, 4);
                            break;
                        }
                    }
                }
                return pulsePin;
            });
            return pin;
        }
    }
}