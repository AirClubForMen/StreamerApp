using StreamerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Gaming.Input;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StreamerApp.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StreamerApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaPlayerPage : Page
    {
        #region Local Variables

        // Manifest URI of Movie to play
        System.Uri manifestUri = new Uri("http://amssamples.streaming.mediaservices.windows.net/49b57c87-f5f3-48b3-ba22-c55cfdffa9cb/Sintel.ism/manifest(format=m3u8-aapl)");

        #endregion

        #region Constuctor and initialization
        /// <summary>
        /// Contructor 
        /// </summary>
        public MediaPlayerPage()
        {
            this.InitializeComponent();
            vm = new MainVm();
            this.DataContext = vm;
        }

        /// <summary>
        /// Initial setup when Page is navigated to
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // set up the media with the main movie manifest, manifestUri would normally be set in the contructor
            await InitializeAdaptiveMediaSource(manifestUri);
        }
        #endregion

        #region Properties
        // Reference to the View Model
        MainVm vm { get; set; }

        // Reference to the adaptive Media Source
        private AdaptiveMediaSource AMS { get; set; }

        // Quick reference to the AdHelper Instance
        private AdHelper AH { get { return AdHelper.Instance; } }

        #endregion

        #region Methods

        /// <summary>
        /// InitializeAdaptiveMediaSource - Initialize and start Media Source
        /// </summary>
        /// <param name="uri">URI of Movie to Play</param>
        /// <returns></returns>
        async private Task InitializeAdaptiveMediaSource(System.Uri uri)
        {
            // try to create the Adaptive MEdia Source from the URI
            AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(uri);

            // set up th adaptive media source
            AMS = result.MediaSource;
            AMS.InitialBitrate = AMS.AvailableBitrates.Max<uint>();
            vm.MaxPlaybackBitRate = AMS.InitialBitrate;

            //Register for download requests
            AMS.DownloadRequested += DownloadRequested;

            //Register for download failure and completion events
            AMS.DownloadCompleted += DownloadCompleted;
            AMS.DownloadFailed += DownloadFailed;

            //Register for bitrate change events
            AMS.DownloadBitrateChanged += DownloadBitrateChanged;
            AMS.PlaybackBitrateChanged += PlaybackBitrateChanged;

            //Register for diagnostic event
            AMS.Diagnostics.DiagnosticAvailable += DiagnosticAvailable;

            // Set the Bit Rates in the View Model
            vm.DownloadBitRate = AMS.InitialBitrate;
            vm.PlaybackBitRate = AMS.CurrentPlaybackBitrate;

            // Set the source from the Adaptive Media Source
            var source = MediaSource.CreateFromAdaptiveMediaSource(AMS);

            // If successfully set, Load and start the media source
            if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
            {
                // set up the initial movie 
                var moviePlaybackItem = new MediaPlaybackItem(source);

                // set up the pre and post roll breaks, Handled in the Ad Helper Class
                AH.InsertPreRollBreak(moviePlaybackItem);
                AH.InsertPostRollBreak(moviePlaybackItem);

                // Set up some Ads, also handled in the Ad Helper class
                AH.InsertAdBreak(moviePlaybackItem, TimeSpan.FromSeconds(148), 2);
                AH.InsertAdBreak(moviePlaybackItem, TimeSpan.FromSeconds(436),2);

                // set up the media player
                MedElement.SetMediaPlayer(new MediaPlayer());
                MedElement.MediaPlayer.AutoPlay = true;
                MedElement.MediaPlayer.Source = moviePlaybackItem;
                
                // Start the Media Player
                MedElement.MediaPlayer.Play();
            }
            else
            {
                // Handle failure to create the adaptive media source
                System.Console.WriteLine($"Adaptive source creation failed: {uri} - {result.ExtendedError}");
            }
        }
        #region Event Handlers

        /// <summary>
        /// Register the Diagnostics 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DiagnosticAvailable(AdaptiveMediaSourceDiagnostics sender, AdaptiveMediaSourceDiagnosticAvailableEventArgs args)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Event that fires when the Download Bit Rate Changes
        /// </summary>
        /// <param name="sender">Adaptive Media Source</param>
        /// <param name="args">New Bit Rate</param>
        private async void DownloadBitrateChanged(AdaptiveMediaSource sender, AdaptiveMediaSourceDownloadBitrateChangedEventArgs args)
        {
            // make sure you are using the UI thread and set the Download Bit Rate in the View Model
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                vm.DownloadBitRate = args.NewValue;
            }));
        }

        /// <summary>
        /// Event that fires when the Playback Bit Rate Changes
        /// </summary>
        /// <param name="sender">Adaptive Media Source</param>
        /// <param name="args">New Bit Rate</param>
        private async void PlaybackBitrateChanged(AdaptiveMediaSource sender, AdaptiveMediaSourcePlaybackBitrateChangedEventArgs args)
        {
            // make sure you are using the UI thread and set the Playback Bit Rate in the View Model
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                vm.PlaybackBitRate = args.NewValue;
            }));
        }

        /// <summary>
        /// Do some logging or send a message if the download fails
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DownloadFailed(AdaptiveMediaSource sender, AdaptiveMediaSourceDownloadFailedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Do something when the download completes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DownloadCompleted(AdaptiveMediaSource sender, AdaptiveMediaSourceDownloadCompletedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Do some processing to get things ready to play
        /// </summary>
        /// <param name="sender">Adaptive Media Source</param>
        /// <param name="args">Download Request Arguments</param>
        private void DownloadRequested(AdaptiveMediaSource sender, AdaptiveMediaSourceDownloadRequestedEventArgs args)
        {

            // rewrite key URIs to replace http:// with https://
            if (args.ResourceType == AdaptiveMediaSourceResourceType.Key)
            {
                string originalUri = args.ResourceUri.ToString();
                string secureUri = originalUri.Replace("http:", "https:");

                // override the URI by setting property on the result sub object
                args.Result.ResourceUri = new Uri(secureUri);
            }

            // If a media segment set up the beginning and end 
            if (args.ResourceType == AdaptiveMediaSourceResourceType.MediaSegment)
            {
                var resourceUri = args.ResourceUri.ToString();
                if (args.ResourceByteRangeOffset != null)
                {
                    resourceUri +="?range=" + args.ResourceByteRangeOffset + "-" + (args.ResourceByteRangeLength - 1);
                }
                // override the URI by setting a property on the result sub object
                args.Result.ResourceUri = new Uri(resourceUri);

                // clear the byte range properties on the result sub object
                args.Result.ResourceByteRangeOffset = null;
                args.Result.ResourceByteRangeLength = null;
            }
        }
        #endregion

        #endregion

    }
}
