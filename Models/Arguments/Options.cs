using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CommandLine;

namespace Mipup.Models.Arguments
{
    public class Options
    {
        private readonly Regex youtubeRegex = new(@"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$");
        private readonly Regex startDurationRegex = new(@"^(\d+:)([0-5]?\d):([0-5]?\d)|([0-5]?\d):([0-5]?\d)|([0-5]?\d)$");

        private string _url = String.Empty;
        private string _startDuration = String.Empty;

        [Option('n', "name", Required = true, HelpText = "The name you want the audio to be saved as.")]
        public string Name { get; set; } = String.Empty;

        [Option('u', "url", Required = true, HelpText = "URL of the youtube video for audio to cut from.")]
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (!youtubeRegex.IsMatch(value))
                {
                    throw new ArgumentException("Youtube Link: Not valid.");
                }
                _url = value.Trim();
            }
        }

        [Option('s', "start", Required = true, HelpText = "Starting duration of the audio.")]
        public string StartDuration
        {
            get
            {
                return _startDuration;
            }
            set
            {
                if (!startDurationRegex.IsMatch(value))
                {
                    throw new ArgumentException("Start Duration: Wrong duration format. (hh:mm:ss)");
                }
                _startDuration = value.Trim();
            }
        }

        [Option('t', "time", Required = true, HelpText = "Length of the audio. (Seconds)")]
        public int Time { get; set; }
    }
}
