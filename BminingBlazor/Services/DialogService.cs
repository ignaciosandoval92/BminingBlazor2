using BminingBlazor.Shared;

namespace BminingBlazor.Services
{
    public class DialogService : IDialogService
    {
        private NotificationMatDialog _notificationMatDialog;




        public void SetNotificationMatDialog(NotificationMatDialog notificationMatDialog)
        {
            _notificationMatDialog = notificationMatDialog;
        }



        public void ShowError(string message)
        {
            _notificationMatDialog.Show(message, "Error");
        }



        public void ShowWarning(string message)
        {
            _notificationMatDialog.Show(message, "Advertencia");
        }



        public void ShowNotification(string message)
        {
            _notificationMatDialog.Show(message, "Notificación");
        }
    }
}