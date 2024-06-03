using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.EtoForms.Notifications;

public class UploadedNotificationView : LinkNotificationView
{
    public UploadedNotificationView(UploadedNotification model)
        : base(model, model.Title, "", "", "")
    {
        HideTimeout = HIDE_SHORT;
    }
}
