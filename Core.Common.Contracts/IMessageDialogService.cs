namespace Core.Common.Contracts
{
    public enum MessageDialogResult
    {
        OK,
        CANCEL,
        YES,
        NO
    }

    public interface IMessageDialogService
    {
        MessageDialogResult ShowOkCancelDialog(string text, string title);
        MessageDialogResult ShowYesNoCancelDialog(string text, string title);
        void ShowInfoDialog(string text, string title);
    }
}