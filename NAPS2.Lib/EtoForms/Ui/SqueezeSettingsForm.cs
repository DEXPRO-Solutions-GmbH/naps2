using System.Linq.Expressions;
using System.Security;
using Eto.Drawing;
using Eto.Forms;
using NAPS2.EtoForms.Desktop;
using NAPS2.EtoForms.Layout;
using NAPS2.EtoForms.Widgets;
using NAPS2.ImportExport.Squeeze;

namespace NAPS2.EtoForms.Ui
{
    // TODO: Squeeze Konfigurationsfenster bauen
    internal class SqueezeSettingsForm : EtoDialogBase
    {
        private readonly DesktopFormProvider _desktopFormProvider;
        private readonly TextBox _server = new();
        private readonly TextBox _client = new();
        private readonly TextBox _user = new();
        private readonly PasswordBoxWithToggle _ownerPassword = new();
        private readonly TextBox _class = new();
        private readonly CheckBox _keepSettings = new() { Text = UiStrings.RememberTheseSettings };
        private readonly Button _restoreDefaults = new() { Text = UiStrings.RestoreDefaults };

        public SqueezeSettingsForm(Naps2Config config, DesktopSubFormController desktopSubFormController,
        DesktopFormProvider desktopFormProvider) : base(config)

        {
            _desktopFormProvider = desktopFormProvider;
            UpdateValues(Config);

            _restoreDefaults.Click += RestoreDefaults_Click;
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
                               _class
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
            _keepSettings.Checked = config.Get(c => c.RememberSqueezeSettings);
            _server.Text = config.Get(c => c.SqueezeSettings.SQZURL);
            _client.Text = config.Get(c => c.SqueezeSettings.SQZClient);
            _user.Text = config.Get(c => c.SqueezeSettings.SQZUserName);
            _ownerPassword.Text = config.Get(c => c.SqueezeSettings.SQZPassword);
            _class.Text = config.Get(c => c.SqueezeSettings.SQZClassID);


            void UpdateCheckbox(CheckBox checkBox, Expression<Func<CommonConfig, bool>> accessor)
            {
                checkBox.Checked = config.Get(accessor);
                checkBox.Enabled = !config.AppLocked.Has(accessor);
            }

            UpdateCheckbox(_keepSettings, c => c.KeepSettings);

            void UpdateTextbox(TextBox textBox, Expression<Func<CommonConfig, bool>> accessor)
            {
                
            }

        }

        private void Save()
        {
            var squeezeSettings = new SqueezeSettings
            {
                SQZURL = _server.Text ?? "",
                SQZClient = _client.Text ?? "",
                SQZUserName = _user.Text ?? "",
                SQZPassword = _ownerPassword.Text ?? "",
                SQZClassID = _class.Text ?? ""

            };

            var runTransact = Config.Run.BeginTransaction();
            var userTransact = Config.User.BeginTransaction();
            bool remember = _keepSettings.IsChecked();
            var transactToWrite = remember ? userTransact : runTransact;

            // when checkbox is clicked save setting

            if (_keepSettings.Checked == true)
            {
                runTransact.Remove(c => c.SqueezeSettings);
                userTransact.Remove(c => c.SqueezeSettings);
                transactToWrite.Set(c => c.SqueezeSettings, squeezeSettings);
                userTransact.Set(c => c.RememberSqueezeSettings, remember);

                runTransact.Commit();
                userTransact.Commit();
            }


            var transact = Config.User.BeginTransaction();
            void SetIfChanged<T>(Expression<Func<CommonConfig, T>> accessor, T value)
            {
                var oldValue = Config.Get(accessor);
                if (!Equals(value, oldValue))
                {
                    transact.Set(accessor, value);
                }
            }
            SetIfChanged(c => c.KeepSettings, _keepSettings.IsChecked());
            transact.Commit();

            _desktopFormProvider.DesktopForm.Invalidate();
            _desktopFormProvider.DesktopForm.PlaceProfilesToolbar();

        }

        private void RestoreDefaults_Click(object? sender, EventArgs e)
        {
            UpdateValues(Config.DefaultsOnly);
        }
    }
}
