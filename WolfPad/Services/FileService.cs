using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace WolfPad.Services
{
    public abstract class FileService
    {

        public static async Task SaveFile(string path, string data)
        {
            if (string.IsNullOrWhiteSpace(path))
                MessageBoxService.GetStandartMessageBox("Path can't be empty !", "Error").ShowDialog();


            if (data == null)
                data = string.Empty;

            try
            {
                await File.WriteAllTextAsync(path, data);
            }
            catch (Exception ex)
            {
                MessageBoxService.GetStandartMessageBox(ex.Message, "Error").ShowDialog();
            }
        }

    }
}
