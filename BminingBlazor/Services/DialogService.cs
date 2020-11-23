﻿using BminingBlazor.Shared;

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
            _notificationMatDialog.ShowNotification(message, "Error");
        }



        public void ShowWarning(string message)
        {
            _notificationMatDialog.ShowNotification(message, "Advertencia");
        }



        public void ShowNotification(string message)
        {
            _notificationMatDialog.ShowNotification(message, "Notificación");
        }
    }
}