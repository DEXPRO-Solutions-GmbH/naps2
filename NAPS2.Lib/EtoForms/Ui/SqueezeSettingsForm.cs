using System.Linq.Expressions;
using System.Security;
using Eto.Drawing;
using Eto.Forms;
using NAPS2.EtoForms.Desktop;
using NAPS2.EtoForms.Layout;
using NAPS2.EtoForms.Widgets;

namespace NAPS2.EtoForms.Ui
{
    // TODO: Squeeze Konfigurationsfenster bauen
    internal class SqueezeSettingsForm : EtoDialogBase
    {
        private readonly TextBox _server = new();
        private readonly TextBox _client = new();
        private readonly TextBox _user = new();
        private readonly PasswordBoxWithToggle _ownerPassword = new();
        private readonly TextBox _classID = new();
        private readonly CheckBox _keepSettings = C.CheckBox(UiStrings.KeepSettings);

        public SqueezeSettingsForm(Naps2Config config) : base(config)

        {
        }

        protected override void BuildLayout()
        {
            Title = UiStrings.SqueezeSettingsForm;
            Icon = new Icon(1f, Icons.cog_small.ToEtoImage());

            FormStateController.DefaultExtraLayoutSize = new Size(60, 0);
            FormStateController.FixedHeightLayout = true;

            LayoutController.Content = L.Column(
                           L.Column(
                               C.Label(UiStrings.SQZServerURL),
                               _server,
                               C.Label(UiStrings.SQZClient),
                               _client,
                               C.Label(UiStrings.SQZUserName),
                               _user,
                               C.Label(UiStrings.SQZPassword),
                               _ownerPassword,
                               C.Label(UiStrings.SQZClassID),
                               _classID
                           ),
                                       C.Filler(),
            L.Row(
                _keepSettings.MinWidth(140),
                C.Filler(),
                L.OkCancel(
                    C.OkButton(this, Save),
                    C.CancelButton(this))
            )
                   );
        }

        private void UpdateValues(Naps2Config config)
        {
            void UpdateTextbox(TextBox textBox, Expression<Func<CommonConfig, bool>> accessor)
            {
                
            }

        }

        private void Save()
        {
            var transact = Config.User.BeginTransaction();
            void SetIfChanged<T>(Expression<Func<CommonConfig, T>> accessor, T value)
            {
                var oldValue = Config.Get(accessor);
                if (!Equals(value, oldValue))
                {
                    transact.Set(accessor, value);
                }
            }

        }

        private void RestoreDefaults_Click(object? sender, EventArgs e)
        {
            UpdateValues(Config.DefaultsOnly);
        }
    }
}
