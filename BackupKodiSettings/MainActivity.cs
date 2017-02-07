using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using System.Threading;
using Android.Content.PM;
using Android.Graphics.Drawables;

namespace BackupKodiSettings
{
    public class ImageButtonScaled:Button
    {
        protected ImageButtonScaled(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ImageButtonScaled(Context context) : base(context)
        {
        }

        public ImageButtonScaled(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ImageButtonScaled(Context context, Android.Util.IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ImageButtonScaled(Context context, Android.Util.IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public override void SetCompoundDrawables(Drawable left, Drawable top, Drawable right, Drawable bottom)
        {
            if (right != null)
            {
                var bounds = right.Bounds;
                right.SetBounds(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom - 10);
            }

            if (left != null)
            {
                var bounds = left.Bounds;
                left.SetBounds(bounds.Left, bounds.Top, bounds.Right-30, bounds.Bottom - 30);
            }

            base.SetCompoundDrawables(left, top, right, bottom);
        }
    }


    [Activity(Label = "Backup Kodi Settings", MainLauncher = true, Icon = "@drawable/kodi")]
    public class MainActivity : Activity
    {
        const bool USE_ALERT_DIALOG = true;
        Button btnBackup, btnRestore, btnLaunchKodi, btnBackupSD, btnRestoreSD;
        Button btnGetPulsePin;
        TextView txtPulsePin;
        ProgressBar pbarPulsePin;

        const string backupFolderName = "MarksKodiBackup";
        string favouritesPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/userdata/favourites.xml";
        string xOnfluencePath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/userdata/addon_data/skin.xonfluence/settings.xml";
        string backupFolder = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/{backupFolderName}/";
        string SportsDonkeyAddonPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/addons/plugin.video.SportsDonkey";
        string SportsDonkeyRepoPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/addons/repository.cdngroup";
        string SportsDonkeyUserDataAddonPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/userdata/addon_data/plugin.video.SportsDonkey";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get references to Controls
            btnBackup = FindViewById<Button>(Resource.Id.btnBackup);
            btnRestore = FindViewById<Button>(Resource.Id.btnRestore);
            btnLaunchKodi = FindViewById<Button>(Resource.Id.btnLaunchKodi);

            btnBackupSD = FindViewById<Button>(Resource.Id.btnBackupSD);
            btnRestoreSD = FindViewById<Button>(Resource.Id.btnRestoreSD);
            btnGetPulsePin = FindViewById<Button>(Resource.Id.btnGetPulsePin);

            txtPulsePin = FindViewById<TextView>(Resource.Id.txtPulsePin);
            pbarPulsePin = FindViewById<ProgressBar>(Resource.Id.pbarPulsePin);

            btnBackup.Click += btnBackup_Click;
            btnRestore.Click += btnRestore_Click;
            btnLaunchKodi.Click += btnLaunchKodi_Click;
            btnBackupSD.Click += btnBackupSD_Click;
            btnRestoreSD.Click += btnRestoreSD_Click;
            btnGetPulsePin.Click += btnGetPulsePin_Click;

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        private async void btnGetPulsePin_Click(object sender, EventArgs e)
        {
            pbarPulsePin.Visibility = ViewStates.Visible;
            btnGetPulsePin.Enabled = false;
            string pin=null;
            try
            {
                pin = await Helpers.GetPulsePin();
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, $"Failed to get Pulse Pin. ex.Message={ex.Message}", ToastLength.Long).Show();
            }
            txtPulsePin.Text = pin ?? "Fail";
            btnGetPulsePin.Enabled = true;
            pbarPulsePin.Visibility = ViewStates.Invisible;
        }

        private void btnRestoreSD_Click(object sender, EventArgs e)
        {
            if (Helpers.DirectoryCopy(backupFolder + "SDAddonBackup/", SportsDonkeyAddonPath, true))
                if (Helpers.DirectoryCopy(backupFolder + "SDRepoBackup/", SportsDonkeyRepoPath, true))
                    if (Helpers.DirectoryCopy(backupFolder + "SDUserDataBackup/", SportsDonkeyUserDataAddonPath, true))
                        //Toast.MakeText(this, "SportsDonkey successfully restored", ToastLength.Long).Show();
                        Helpers.ShowDialog(this, "SportsDonkey successfully restored");
                    else
                        //Toast.MakeText(this, "SportsDonkey restore failed", ToastLength.Long).Show();
                        Helpers.ShowDialog(this, "SportsDonkey restore failed");
        }

        private void btnBackupSD_Click(object sender, EventArgs e)
        {
            if (Helpers.DirectoryCopy(SportsDonkeyAddonPath, backupFolder + "SDAddonBackup", true))
                if (Helpers.DirectoryCopy(SportsDonkeyRepoPath, backupFolder + "SDRepoBackup", true))
                    if (Helpers.DirectoryCopy(SportsDonkeyUserDataAddonPath, backupFolder + "SDUserDataBackup", true))
                        //Toast.MakeText(this, "SportsDonkey successfully backed up", ToastLength.Long).Show();
                        Helpers.ShowDialog(this, "SportsDonkey successfully backed up");
                    else
                        //Toast.MakeText(this, "SportsDonkey backup failed", ToastLength.Long).Show();
                        Helpers.ShowDialog(this, "SportsDonkey backup failed");
        }


        /// <summary>
        /// Called once
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DebugMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
      

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Note: When you successfully handle a menu item, return true. 
            // If you don't handle the menu item, you should call the superclass
            // implementation of onOptionsItemSelected() (the default implementation
            // returns false).
            switch (item.ItemId)
            {
                case Resource.Id.test_buttons:
                    StartActivity(typeof(TestButtonsActivity));
                    return true;
                case Resource.Id.view_settings_files:
                    StartActivity(typeof(ViewSettingsActivity));
                    return true;
                //case Resource.Id.get_pulse_pin:
                //    var pulsePin = await Helpers.GetPulsePin();
                //    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void btnLaunchKodi_Click(object sender, EventArgs e)
        {
            btnLaunchKodi.Enabled = false;
            if (Helpers.IsAppInstalled(this,"org.xbmc.kodi"))
            {
                Intent intent = PackageManager.GetLaunchIntentForPackage("org.xbmc.kodi");
                StartActivity(intent);
            }
            else
                Toast.MakeText(this, "Kodi is not installed on this device", ToastLength.Long).Show();
            btnLaunchKodi.Enabled = true;
        }


        /// <summary>
        /// Backup favourites.xml and skin settings.xml files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackup_Click(object sender, EventArgs e)
        {
            btnBackup.Enabled = false;
            bool successFav, successSett = false;
            Toast.MakeText(this, (successFav = BackupFile(favouritesPath)) ? "favourites.xml backed up successfully" : "Failed to backup favourites.xml", ToastLength.Short).Show();
            Toast.MakeText(this, (successSett = BackupFile(xOnfluencePath)) ? "xonfluence backed up successfully" : "Failed to backup xonfluence", ToastLength.Short).Show();

            if (USE_ALERT_DIALOG && (successFav && successSett))
            {
                Helpers.ShowDialog(this, "Backup completed successfully");
            }
            btnBackup.Enabled = true;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            btnRestore.Enabled = false;
            bool successFav, successSett = false;
            Toast.MakeText(this, (successFav = RestoreFile(backupFolder + "favourites.xml", "faves")) ? "favourites.xml successfully restored" : "favourites.xml failed to restore. Make sure Kodi is closed and try again", ToastLength.Short).Show();
            Toast.MakeText(this, (successSett = RestoreFile(backupFolder + "settings.xml", "xonf")) ? "xonfluence successfully restored" : "xonfluence failed to restore. Make sure Kodi is closed and try again", ToastLength.Short).Show();

            if (USE_ALERT_DIALOG && (successFav && successSett))
            {
                Helpers.ShowDialog(this, "Restore completed successfully");
            }
            btnRestore.Enabled = true;
        }
   
        /// <summary>
        /// Create a copy of 'file' to the backup folder
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool BackupFile(string file)
        {
            Helpers.CreateSBackupFolderIfNotExists(this,backupFolder);

            bool success = false;
         
            if (File.Exists(file))
            {
                try
                {
                    var dest = backupFolder + Path.GetFileName(file);
                    File.Copy(file, backupFolder + Path.GetFileName(file), true);
                    success = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return success;
        }

        /// <summary>
        /// Try to restore 'file' to the correct folder
        /// </summary>
        /// <param name="file"></param>
        /// <param name="settingsFile"></param>
        /// <returns>bool</returns>
        private bool RestoreFile(string file, string settingsFile)
        {
            if (File.Exists(file))
            {
                //settingFile can be either "faves" or "xonf"
                var dest = settingsFile == "xonf" ? xOnfluencePath : favouritesPath;
                try
                {
                    File.Copy(file, dest, true);
                    return true;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, $"Failed to restore {Path.GetFileName(file)}", ToastLength.Long).Show();
                    Toast.MakeText(this, $"{ex.Message}", ToastLength.Long).Show();
                    return false;
                }
            }
            return false;
        }
    }
}

