using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using System.IO;
using Android.Preferences;
using System.Net;

namespace BackupKodiSettings
{
    [Activity(Label = "TestButtonsActivity")]
    public class TestButtonsActivity : Activity
    {
        const string backupFolderName = "MarksKodiBackup";
        string favouritesPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/userdata/favourites.xml";
        string xOnfluencePath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/userdata/addon_data/skin.xonfluence/settings.xml";

        string backupFolder = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/{backupFolderName}/";

        string SportsDonkeyAddonPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/addons/plugin.video.SportsDonkey";
        string SportsDonkeyRepoPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/addons/repository.cdngroup";
        string SportsDonkeyUserDataAddonPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/userdata/addon_data/plugin.video.SportsDonkey";

        LinearLayout linearLayout1;
        Button btnCheckFavourites, btnCheckXonfluence, btnCheckBackupDir, btnCreateBackupDir;
        Button btnDeleteBackupDir, btnBackupSD, btnRestoreSD, btnCheckSDPaths;
        Button btnGetPulseCode;
        Switch swiShowTestButtons;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TestButtons);
            ActionBar.Hide();
            btnCheckFavourites = FindViewById<Button>(Resource.Id.btnCheckFavourites);
            btnCheckXonfluence = FindViewById<Button>(Resource.Id.btnCheckXonfluence);
            btnCheckBackupDir = FindViewById<Button>(Resource.Id.btnCheckBackupDir);
            btnCreateBackupDir = FindViewById<Button>(Resource.Id.btnCreateBackupDir);
            btnDeleteBackupDir = FindViewById<Button>(Resource.Id.btnDeleteBackupDir);
            btnBackupSD = FindViewById<Button>(Resource.Id.btnBackupSD);
            btnRestoreSD = FindViewById<Button>(Resource.Id.btnRestoreSD);
            btnCheckSDPaths = FindViewById<Button>(Resource.Id.btnSDPaths);
            btnGetPulseCode = FindViewById<Button>(Resource.Id.btnGetPulsePin);

   


            swiShowTestButtons = FindViewById<Switch>(Resource.Id.swiShowTestButtons);
            linearLayout1 = FindViewById<LinearLayout>(Resource.Id.linearLayout1);

            btnCheckFavourites.Click += btnCheckFavourites_Click;
            btnCheckXonfluence.Click += btnCheckXonfluence_Click;
            btnCheckBackupDir.Click += btnCheckBackupDir_Click;
            btnCreateBackupDir.Click += btnCreateBackupDir_Click;
            btnDeleteBackupDir.Click += btnDeleteBackupDir_Click;
            btnCheckSDPaths.Click += btnCheckSDPaths_Click;
            btnBackupSD.Click += btnBackupSD_Click;
            btnRestoreSD.Click += btnRestoreSD_Click;
            btnGetPulseCode.Click += btnGetPulseCode_Click;

            swiShowTestButtons.Click += swiToggleTestButtons_Click;
            RestoreInstanceState(savedInstanceState);
            ToggleTestButtons();

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            //                                  | SecurityProtocolType.Tls11
            //                                  | SecurityProtocolType.Tls12
            //                                  | SecurityProtocolType.Ssl3;

        }

        private void btnGetPulseCode_Click(object sender, EventArgs e)
        {
            const string pulsePinAddress = "https://ares-project.uk/showpin.php?action=getbuildpin";
          //  const string bitFly = "http://bit.ly/getbuildpin";

            HttpWebRequest webRequest = null;
            WebResponse webResponse;
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(pulsePinAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            try
            {
                webResponse = webRequest.GetResponse();
                var stream = webResponse.GetResponseStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }






            using (WebClient client = new WebClient())
            {
                try
                {
                    var s =  client.DownloadString(new Uri(pulsePinAddress));

                    Console.WriteLine(s);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
        }

        private void btnRestoreSD_Click(object sender, EventArgs e)
        {
            if (Helpers.DirectoryCopy(backupFolder + "SDAddonBackup/", SportsDonkeyAddonPath, true))
                if (Helpers.DirectoryCopy(backupFolder + "SDRepoBackup/", SportsDonkeyRepoPath, true))
                    if (Helpers.DirectoryCopy(backupFolder + "SDUserDataBackup/", SportsDonkeyUserDataAddonPath, true))
                        Toast.MakeText(this, "SportsDonkey successfully restored", ToastLength.Long).Show();
                    else
                        Toast.MakeText(this, "SportsDonkey restore failed", ToastLength.Long).Show();
        }

        private void btnBackupSD_Click(object sender, EventArgs e)
        {
            if (Helpers.DirectoryCopy(SportsDonkeyAddonPath, backupFolder + "SDAddonBackup", true))
                if (Helpers.DirectoryCopy(SportsDonkeyRepoPath, backupFolder + "SDRepoBackup", true))
                    if (Helpers.DirectoryCopy(SportsDonkeyUserDataAddonPath, backupFolder + "SDUserDataBackup", true))
                        Toast.MakeText(this, "SportsDonkey successfully backed up", ToastLength.Long).Show();
                    else
                        Toast.MakeText(this, "SportsDonkey backup failed", ToastLength.Long).Show();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutBoolean("SwitchChecked", swiShowTestButtons.Checked);
            outState.PutBoolean("ButtonsVisible", linearLayout1.Visibility == ViewStates.Visible);
        }

        protected override void OnPause()
        {
            base.OnPause();
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("SwitchChecked", swiShowTestButtons.Checked);
            editor.PutBoolean("ButtonsVisible", linearLayout1.Visibility == ViewStates.Visible);
            // editor.Commit();    // applies changes synchronously on older APIs
            editor.Apply();        // applies changes asynchronously on newer APIs
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            RestoreInstanceState(savedInstanceState);
        }

        private void RestoreInstanceState( Bundle savedInstanceState)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            swiShowTestButtons.Checked = prefs.GetBoolean("SwitchChecked", false);
            linearLayout1.Visibility = prefs.GetBoolean("ButtonsVisible", false)?ViewStates.Visible:ViewStates.Gone;
        }


        /// <summary>
        /// Show/Hide all of the debug Test buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void swiToggleTestButtons_Click(object sender, EventArgs e)
        {
            ToggleTestButtons();
        }

        private void btnCheckSDPaths_Click(object sender, EventArgs e)
        {
            var sharedFolder = Android.OS.Environment.RootDirectory + "/" + "mnt/shared/GenymotionSharedFolder/";
            Toast.MakeText(this, Directory.Exists(sharedFolder) ? "Shared folder found" : "Shared folder not found", ToastLength.Long).Show();

            Toast.MakeText(this, Directory.Exists(SportsDonkeyAddonPath) ? "SportsDonkey Addon found" : "SportsDonkey Addon not found", ToastLength.Short).Show();
            Toast.MakeText(this, Directory.Exists(SportsDonkeyRepoPath) ? "SportsDonkey Repo found" : "SportsDonkey Repo not found", ToastLength.Short).Show();
            Toast.MakeText(this, Directory.Exists(SportsDonkeyUserDataAddonPath) ? "SportsDonkeyUserData Addon found" : "SportsDonkeyUserData Addon not found", ToastLength.Short).Show();
        }


        private void btnDeleteBackupDir_Click(object sender, EventArgs e)
        {
            DeleteBackupFolder();
        }

        private void btnCreateBackupDir_Click(object sender, EventArgs e)
        {
            var success = Helpers.CreateSBackupFolderIfNotExists(this,backupFolder);
            Toast.MakeText(this, $"{(success ? "Backup folder created" : "Failed to create backup folder")}", ToastLength.Short).Show();
        }

        private void btnCheckBackupDir_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(backupFolder))
                Toast.MakeText(this, $"Backup folder found", ToastLength.Short).Show();
        }


        private void btnCheckFavourites_Click(object sender, EventArgs e)
        {
            if (File.Exists(favouritesPath))
                Toast.MakeText(this, $"'favourites.xml' found", ToastLength.Short).Show();
            else
                Toast.MakeText(this, $"'favourites.xml' not found", ToastLength.Short).Show();
        }

        private void btnCheckXonfluence_Click(object sender, EventArgs e)
        {
            if (File.Exists(xOnfluencePath))
                Toast.MakeText(this, $"xonfluence 'settings.xml' found", ToastLength.Short).Show();
            else
                Toast.MakeText(this, $"xonfluence 'settings.xml' not found", ToastLength.Short).Show();
        }

        private void ToggleTestButtons()
        {
            if (swiShowTestButtons.Checked)
            {
                linearLayout1.Visibility = ViewStates.Visible;
                swiShowTestButtons.Text = "Hide Test Buttons";
            }
            else
            {
                linearLayout1.Visibility = ViewStates.Gone;
                swiShowTestButtons.Text = "Show Test Buttons";
            }
        }

        /// <summary>
        /// Deletes the backup folder if it exists
        /// </summary>
        private void DeleteBackupFolder()
        {
            if (!Directory.Exists(backupFolder)) return;
            try
            {
                Directory.Delete(backupFolder);
                Toast.MakeText(this, $"Backup folder deleted", ToastLength.Long).Show();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, $"Error deleting backup folder. {ex.Message}", ToastLength.Long).Show();
            }
        }
    }
}