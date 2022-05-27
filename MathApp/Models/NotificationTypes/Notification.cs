using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models.NotificationTypes
{
    public class Notification
    {
        public string message { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string type { get; set; }
        public string provider { get; set; }
        public string button { get; set; }

        public Notification() {}
        public Notification(string message, string title, string icon, string type, string provider, string button)
        {
            this.message = message;
            this.title = title;
            this.icon = icon;
            this.type = type;
            this.provider = provider;
            this.button = button;
        }
        public Notification(string message, string type, string provider)
        {
            this.message = message;
            this.title = null;
            this.icon = type;
            this.type = type;
            this.provider = provider;
            this.button = null;
        }
    }
}
