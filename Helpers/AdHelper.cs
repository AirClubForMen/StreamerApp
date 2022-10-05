using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;

namespace StreamerApp.Helpers
{
    // Helper class to deal with adds
    public class AdHelper
    {
        #region Local Properties
        // set up the breaks we want to use, would be grabbed from an ad server 
        Uri preRollUri = new Uri("https://static.videezy.com/system/resources/previews/000/054/194/original/wc.mp4");
        Uri adBreakUri1 = new Uri("https://www.kristofcreative.com/wp-content/uploads/citizens-bank-commercial-preparations.mp4");
        Uri adBreakUri2 = new Uri("https://cdn1.extremereach.io/media/107116/145952/21ae9a42-58ac-49dd-b02d-802a4af39dcb/55f5437e-7090-4728-8dea-b9489a41e307.mp4?line_item=15309710&cid=101400&e=e.mp4");
        Uri adBreakUri3 = new Uri("https://cdn1.extremereach.io/media/107116/165625/c7744647-8f62-480b-91ee-1ebe6e51f1de/3e5debee-f4d2-4c6b-943c-4ba624381fc3.mp4?line_item=15490705&cid=188419&e=e.mp4");
        Uri postRollUri = new Uri("https://media.istockphoto.com/videos/sign-in-neon-style-turning-on-video-id522763430");

        static int lastAdIndex = 0;
        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public AdHelper()
        {
            // normally the ads would be grabbed from an ad server but this is just a demo 
            AdList.Add(adBreakUri1);
            AdList.Add(adBreakUri2);
            AdList.Add(adBreakUri3);
        }


        // Set up Singleton
        public static AdHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new AdHelper();
                return instance;
            }
        }
        static AdHelper instance = null;

        #endregion

        #region Methods

        /// <summary>
        /// Insert a certain number of ads at a certain time. This would have more smarts in a real environment
        /// </summary>
        /// <param name="moviePlaybackItem">Reference to the Media Playback Item</param>
        /// <param name="insertAtTime">When to insert the ad(s)</param>
        /// <param name="numberOfAds">Number of ads to insert</param>
        public void InsertAdBreak(MediaPlaybackItem moviePlaybackItem, TimeSpan insertAtTime, int numberOfAds = 1)
        {
            // Set up the break at the time specified
            MediaBreak mediaBreak = new MediaBreak(MediaBreakInsertionMethod.Interrupt, insertAtTime);

            // add a number of ads to the break
            for (int i = 0; i < numberOfAds; i++)
            {
                // If the ad index is >= to the count then start back from the beginning
                if (lastAdIndex >= AdList.Count)
                    lastAdIndex = 0;

                // get the next ad in the list
                MediaPlaybackItem adItem = new MediaPlaybackItem(MediaSource.CreateFromUri(AdList[lastAdIndex]));

                // increment the index of the the last ad
                lastAdIndex++;

                // add the item to the list
                mediaBreak.PlaybackList.Items.Add(adItem);
            }
            // add the items to the mid-roll break
            moviePlaybackItem.BreakSchedule.InsertMidrollBreak(mediaBreak);
        }

        /// <summary>
        /// Take care of the Pre-Roll Break
        /// </summary>
        /// <param name="moviePlaybackItem">Reference to the Media Playback Item</param>
        public void InsertPreRollBreak(MediaPlaybackItem moviePlaybackItem)
        {
            // set up pre-roll break
            MediaBreak preRollMediaBreak = new MediaBreak(MediaBreakInsertionMethod.Interrupt);

            // We would normally get this from a Preview Server
            MediaPlaybackItem prerollAd = new MediaPlaybackItem(MediaSource.CreateFromUri(preRollUri));
            prerollAd.CanSkip = false;
            preRollMediaBreak.PlaybackList.Items.Add(prerollAd);

            // add pre-roll break to the media playback item
            moviePlaybackItem.BreakSchedule.PrerollBreak = preRollMediaBreak;

        }
        /// <summary>
        /// Take care of the Post-Roll Break
        /// </summary>
        /// <param name="moviePlaybackItem">Reerence to the Media Playback Item</param>
        public void InsertPostRollBreak(MediaPlaybackItem moviePlaybackItem)
        {   
            // set up post-roll break
            MediaBreak postRollMediaBreak = new MediaBreak(MediaBreakInsertionMethod.Interrupt);

            // We would normally get this from a Preview Server
            MediaPlaybackItem postrollAd = new MediaPlaybackItem(MediaSource.CreateFromUri(postRollUri));
            postrollAd.CanSkip = false;
            postRollMediaBreak.PlaybackList.Items.Add(postrollAd);

            // add pre-roll break to the media playback item
            moviePlaybackItem.BreakSchedule.PostrollBreak = postRollMediaBreak;
        }


        #endregion

        #region Properties

        // List of Ads, would be grabbed from an ad server in a real environment
        List<Uri> AdList { get; set; } = new List<Uri>();

        #endregion
    }
}
