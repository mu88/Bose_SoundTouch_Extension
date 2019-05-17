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

        private List<ISpeaker> PlayingSpeakers { get; set; }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // If the following line is enabled, the 'Options menu indicator' (three-dot indicator) will be shown
            // Since this is not needed yet, it can be disabled.
            //MenuInflater.Inflate(Resource.Menu.menu_main, menu);

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

            FindViewById<Button>(Resource.Id.switchButton).Click += SwitchButtonOnClick;
            FindViewById<ListView>(Resource.Id.sourceListView).ItemClick += SourceListViewOnItemClick;
            FindViewById<ListView>(Resource.Id.destinationListView).ItemClick += DestinationListViewOnItemClick;
            FindViewById<ProgressBar>(Resource.Id.progressBar1).Indeterminate = true;

            MakeControlInvisible(Resource.Id.sourceTextView);
            MakeControlInvisible(Resource.Id.destinationTextView);
            MakeControlInvisible(Resource.Id.switchButton);
            MakeControlInvisible(Resource.Id.progressBar1);

            LoadAsync();
        }

        private void DestinationListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            SelectedDestinationSpeaker = DestinationSpeakers.ElementAt(e.Position);

            MakeControlVisible(Resource.Id.switchButton);
        }

        private void SourceListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedPlayingSpeaker =
                PlayingSpeakers.Where((device, indexOfCurrentDevice) => indexOfCurrentDevice == e.Position).First();

            DestinationSpeakers = Speakers.Where(x => !Equals(x, selectedPlayingSpeaker)).ToList();

            MakeControlVisible(Resource.Id.destinationTextView);

            SelectedSourceSpeaker = Speakers.ElementAt(e.Position);

            SetListViewContent(Resource.Id.destinationListView, DestinationSpeakers);
            UnselectListView(Resource.Id.destinationListView);
        }

        private async Task LoadAsync()
        {
            MakeControlVisible(Resource.Id.progressBar1);

            Speakers = (await SpeakerResolver.ResolveSpeakersAsync()).ToList();

            PlayingSpeakers = new List<ISpeaker>();
            foreach (var speaker in Speakers)
            {
                if (await speaker.IsPlayingAsync())
                {
                    PlayingSpeakers.Add(speaker);
                }
            }

            if (!PlayingSpeakers.Any())
            {
                return;
            }

            SetListViewContent(Resource.Id.sourceListView, PlayingSpeakers);
            UnselectListView(Resource.Id.sourceListView);

            MakeControlVisible(Resource.Id.sourceTextView);
            MakeControlInvisible(Resource.Id.destinationTextView);
            MakeControlInvisible(Resource.Id.switchButton);

            SetListViewContent(Resource.Id.destinationListView, new List<ISpeaker>());

            MakeControlInvisible(Resource.Id.progressBar1);
        }

        private void UnselectListView(int controlId)
        {
            FindViewById<ListView>(controlId).Selected = false;
        }

        private void SetListViewContent(int controlId, IList<ISpeaker> content)
        {
            FindViewById<ListView>(controlId).Adapter = new ArrayAdapter<ISpeaker>(this, Android.Resource.Layout.SimpleListItem1, content);
        }

        private void MakeControlInvisible(int controlId)
        {
            FindViewById(controlId).Visibility = ViewStates.Invisible;
        }

        private void MakeControlVisible(int controlId)
        {
            FindViewById(controlId).Visibility = ViewStates.Visible;
        }

        private async void SwitchButtonOnClick(object sender, EventArgs e)
        {
            MakeControlVisible(Resource.Id.progressBar1);

            await SelectedSourceSpeaker.ShiftToSpeakerAsync(SelectedDestinationSpeaker);

            MakeControlInvisible(Resource.Id.progressBar1);
            
            await LoadAsync();
        }
    }
}