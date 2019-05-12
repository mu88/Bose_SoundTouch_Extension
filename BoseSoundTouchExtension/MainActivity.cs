using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BoseSoundTouchExtension.BusinessLogic;
using BusinessLogic;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace BoseSoundTouchExtension
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private SpeakerResolver SpeakerResolver { get; set; }

        private List<ISpeaker> Speakers { get; set; }

        private ISpeaker SelectedDestinationSpeaker { get; set; }

        private List<ISpeaker> DestinationSpeakers { get; set; }

        private ISpeaker SelectedSourceSpeaker { get; set; }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SpeakerResolver = new SpeakerResolver(ApplicationContext);
            Speakers = new List<ISpeaker>();

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var switchButton = FindViewById<Button>(Resource.Id.switchButton);
            switchButton.Click += SwitchButtonOnClick;
            switchButton.Visibility = ViewStates.Invisible;

            var loadButton = FindViewById<Button>(Resource.Id.loadButton);
            loadButton.Click += LoadButtonOnClick;

            FindViewById<ListView>(Resource.Id.sourceListView).ItemClick += SourceListViewOnItemClick;
            FindViewById<ListView>(Resource.Id.destinationListView).ItemClick += DestinationListViewOnItemClick;

            FindViewById<TextView>(Resource.Id.sourceTextView).Visibility = ViewStates.Invisible;
            FindViewById<TextView>(Resource.Id.destinationTextView).Visibility = ViewStates.Invisible;
        }

        private void DestinationListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            SelectedDestinationSpeaker = DestinationSpeakers.ElementAt(e.Position);

            FindViewById<TextView>(Resource.Id.switchButton).Visibility = ViewStates.Visible;
        }

        private void SourceListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            DestinationSpeakers = Speakers.Where((device, indexOfCurrentDevice) => indexOfCurrentDevice != e.Position).ToList();

            FindViewById<TextView>(Resource.Id.destinationTextView).Visibility = ViewStates.Visible;

            SelectedSourceSpeaker = Speakers.ElementAt(e.Position);

            var destinationListView = FindViewById<ListView>(Resource.Id.destinationListView);
            destinationListView.Adapter = new ArrayAdapter<ISpeaker>(this, Android.Resource.Layout.SimpleListItem1, DestinationSpeakers);
            destinationListView.Selected = false;
        }

        private async void LoadButtonOnClick(object sender, EventArgs e)
        {
            await LoadSpeakersAsync();

            var sourceListView = FindViewById<ListView>(Resource.Id.sourceListView);
            sourceListView.Adapter = new ArrayAdapter<ISpeaker>(this, Android.Resource.Layout.SimpleListItem1, Speakers);
            sourceListView.Selected = false;

            FindViewById<TextView>(Resource.Id.sourceTextView).Visibility = ViewStates.Visible;

            FindViewById<TextView>(Resource.Id.destinationTextView).Visibility = ViewStates.Invisible;

            FindViewById<TextView>(Resource.Id.switchButton).Visibility = ViewStates.Invisible;

            var destinationListView = FindViewById<ListView>(Resource.Id.destinationListView);
            destinationListView.Adapter = new ArrayAdapter<ISpeaker>(this, Android.Resource.Layout.SimpleListItem1, new List<ISpeaker>());
        }

        private async Task LoadSpeakersAsync()
        {
            Speakers = (await SpeakerResolver.ResolveSpeakersAsync()).ToList();
        }

        private async void SwitchButtonOnClick(object sender, EventArgs e)
        {
            await SelectedSourceSpeaker.ShiftToSpeakerAsync(SelectedDestinationSpeaker);
        }
    }
}