using Core.Common.Contracts;
using System.ComponentModel.Composition;
using System.Windows;
using System;

namespace EchoDesertTrips.Desktop.Support
{
    [Export(typeof(IMessageDialogService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MessageDialogService : IMessageDialogService
    {
        public void ShowInfoDialog(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK);
        }

        public MessageDialogResult ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK ?
                MessageDialogResult.OK :
                MessageDialogResult.CANCEL;
        }

        public MessageDialogResult ShowYesNoCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
                return MessageDialogResult.YES;
            if (result == MessageBoxResult.No)
                return MessageDialogResult.NO;
            return MessageDialogResult.CANCEL;
        }
    }
}
