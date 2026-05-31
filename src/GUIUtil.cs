using Microsoft.WindowsAPICodePack.Dialogs;

namespace MyraGUIUtil
{
    public partial class GUIUtil
    {
        public static string? SelectFolder()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }

            return null;
        }

        public static IEnumerable<string>? SelectFolderMultiple()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileNames;
            }

            return null;
        }
    }
}
