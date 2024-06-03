using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.EtoForms.Notifications;

public class UploadedNotification : NotificationModel
{
    public UploadedNotification(string title)
    {
        Title = title;
    }

    public string Title { get; }

    public override NotificationView CreateView()
    {
        return new UploadedNotificationView(this);
    }
}
