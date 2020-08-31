using Acr.UserDialogs;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace PeakMVP.Services.Dialog {
    public class DialogService : IDialogService {
        public Task ShowAlertAsync(string message, string title, string buttonLabel) {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public async Task ToastAsync(string message) {
            await Task.Run(() => {
                return UserDialogs.Instance.Toast(new ToastConfig(message)
                    //var icon = await BitmapLoader.Current.LoadFromResource("emoji_cool_small.png", null, null);
                    //.SetIcon(icon)
                    .SetBackgroundColor(Color.FromArgb(12, 168, 244))
                    .SetMessageTextColor(Color.White)
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(ToastPosition.Bottom));
            });
        }

        public async Task ToastAsync(string message, string actionName, Action action) {
            await Task.Run(() => {
                return UserDialogs.Instance.Toast(new ToastConfig(message)
                    .SetBackgroundColor(Color.FromArgb(12, 168, 244))
                    .SetMessageTextColor(Color.White)
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(ToastPosition.Bottom)
                    .SetAction(x => x
                        .SetText(actionName)
                        .SetTextColor(Color.White)
                        .SetAction(action)
                    ));
            });
        }
    }
}
