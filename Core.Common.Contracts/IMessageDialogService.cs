namespace Core.Common.Contracts
{
    public enum MessageDialogResult
    {
        OK,
        CANCEL
    }

    public interface IMessageDialogService
    {
        MessageDialogResult ShowOkCancelDialog(string text, string title);
        void ShowInfoDialog(string text, string title);
    }
}