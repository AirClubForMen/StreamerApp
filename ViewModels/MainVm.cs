using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace StreamerApp.ViewModels
{
    /// <summary>
    /// Main View Model for the Streamer Class
    /// </summary>
    public class MainVm: ViewModelBase
    {
        // Download Bit Rate
        public uint DownloadBitRate
        {
            get { return downloadBitRate; }
            set
            {
                downloadBitRate = value;
                PropChanged();
            }
        }
        uint downloadBitRate=0;

        // Current Playback Bit Rate
        public uint PlaybackBitRate
        {
            get { return playbackBitRate; }
            set
            {
                playbackBitRate = value;
                PropChanged();
                PropChanged("FullSpeedBackground");
            }
        }
        uint playbackBitRate = 0;

        // Max Playback Bit Rate
        public uint MaxPlaybackBitRate { get; set; } = 0;

        // Color of the Background of the box that display the bit rates(could use a converter for this)
        public SolidColorBrush FullSpeedBackground
        {
            get {
                // Green = 100%
                var speedColor = new SolidColorBrush(Colors.Green);

                // If the Max Rate is 0 use Red
                if(MaxPlaybackBitRate==0) return new SolidColorBrush(Colors.Red);

                // Use Light Green if over 75% and less than full speed
                if((double) PlaybackBitRate/ MaxPlaybackBitRate < 1) speedColor = new SolidColorBrush(Colors.LightGreen);

                // If less than 75% use Orange
                if ((double)PlaybackBitRate / MaxPlaybackBitRate < .75) speedColor = new SolidColorBrush(Colors.Orange);

                return speedColor; }
            
        }
    }
}
