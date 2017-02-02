using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace BackupKodiSettings
{
    [Activity(Label = "View Settings")]
    public class ViewSettingsActivity : Activity
    {

        Button btnViewFavourites, btnViewXonfSettings;
        TextView txtViewSettings;
        string favouritesPath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/userdata/favourites.xml";
        string xOnfluencePath = $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/org.xbmc.kodi/files/.kodi/userdata/addon_data/skin.xonfluence/settings.xml";
        State ourState = State.DisplayingNothing;
        enum State
        {
            DisplayingFavourites,
            DisplayingSettings,
            DisplayingNothing
        }


        #region Lifecycle Methods
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewSettings);
            btnViewFavourites = FindViewById<Button>(Resource.Id.btnViewFavourites);
            btnViewXonfSettings = FindViewById<Button>(Resource.Id.btnViewXonfSettings);
            txtViewSettings = FindViewById<TextView>(Resource.Id.txtSettingsView);

            btnViewFavourites.Click += btnViewFavourites_Click;
            btnViewXonfSettings.Click += btnViewXonfSettings_Click;
            RegisterForContextMenu(txtViewSettings);
        }

        //protected override void OnPause()
        //{
        //    base.OnPause();
        //    SaveInstanceState();
        //}


        #endregion



        //protected override void OnSaveInstanceState(Bundle outState)
        //{
        //    base.OnSaveInstanceState(outState);
        //    outState.PutInt("OurState", (int)ourState);
        //}

        //protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        //{
        //    base.OnRestoreInstanceState(savedInstanceState);
        //    RestoreInstanceState();
        //}

   

        #region Menu Events
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            menu.Add("Clear");
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            txtViewSettings.Text = "";
            ourState = State.DisplayingNothing;
            return base.OnContextItemSelected(item);
        } 
        #endregion

        #region Button Events
        private void btnViewXonfSettings_Click(object sender, EventArgs e)
        {
            txtViewSettings.Text = Helpers.ReadXonfSettings(xOnfluencePath);
            ourState = State.DisplayingSettings;
        }

        private void btnViewFavourites_Click(object sender, EventArgs e)
        {
            txtViewSettings.Text = Helpers.ReadFavourites(favouritesPath) ?? "No favourites have been created yet";
            ourState = State.DisplayingFavourites;
        } 
        #endregion


        private void RestoreInstanceState(Bundle savedInstanceState)
        {
            RestoreInstanceState();
        }

        private void RestoreInstanceState()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ourState = (State)prefs.GetInt("OurState", 2);

            switch (ourState)
            {
                case State.DisplayingFavourites:
                    txtViewSettings.Text = Helpers.ReadFavourites(favouritesPath) ?? "No favourites have been created yet";
                    break;
                case State.DisplayingSettings:
                    txtViewSettings.Text = Helpers.ReadXonfSettings(xOnfluencePath) ?? "Failed to read Xonfluence settings.xml";
                    break;
                case State.DisplayingNothing:
                    txtViewSettings.Text = "";
                    break;
            }
        }

        private void SaveInstanceState()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("OurState", (int)ourState);
            // editor.Commit();    // applies changes synchronously on older APIs
            editor.Apply();        // applies changes asynchronously on newer APIs
        }

    }
}