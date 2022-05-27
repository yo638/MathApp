using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathApp.Models.NotificationTypes
{
    public class SuccessNotification : Notification
    {
        SuccessNotification() {}
        SuccessNotification(string message, string title, string provider, string button)
        {
            this.message = message;
            this.title = title;
            this.icon = "success";
            this.type = "success";
            this.provider = provider;
            this.button = button;
        }
        SuccessNotification(string message, string title, string provider)
        {
            this.message = message;
            this.title = title;
            this.icon = "success";
            this.type = "success";
            this.provider = provider;
            this.button = null;
        }
    }
}
