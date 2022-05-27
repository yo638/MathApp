using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models.NotificationTypes
{
    public class ErrorNotification : Notification
    {
        ErrorNotification() { }
        ErrorNotification(string message, string title, string provider, string button)
        {
            this.message = message;
            this.title = title;
            this.icon = "error";
            this.type = "error";
            this.provider = provider;
            this.button = button;
        }
        ErrorNotification(string message, string title, string provider)
        {
            this.message = message;
            this.title = title;
            this.icon = "error";
            this.type = "error";
            this.provider = provider;
            this.button = null;
        }
    }
}
