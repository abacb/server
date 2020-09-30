using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;

namespace Rms.Server.Test
{
    public class TestLog
    {
        private static readonly string MessagePattern = @"^\[.*?\(.*?\)\]\[.*?\](?<message>.*?)$";

        public LogLevel LogLevel { get; set; }

        public EventId EventId { get; set; }

        public object State { get; set; }

        public Exception Exception { get; set; }

        public string GetSimpleText()
        {
            Regex regex = new Regex(MessagePattern, RegexOptions.Singleline);
            Match match = regex.Match(State.ToString());
            string message = match.Groups["message"].Value;
            return message.TrimStart();
        }
    }
}
