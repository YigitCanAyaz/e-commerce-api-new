using ECommerceAPI.Infrastructure.Operations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure.Services
{
    public class FileService
    {
        private async Task<string> FileRenameAsync(string path, string fileName, int num = 1)
        {
            return await Task.Run(async () =>
            {
                string extension = Path.GetExtension(fileName);
                string oldName = $"{Path.GetFileNameWithoutExtension(fileName)}-{num}";
                string newFileName = $"{NameOperation.CharacterRegulatory(oldName)}{extension}";

                if (File.Exists($"{path}\\{newFileName}"))
                {
                    return await FileRenameAsync(path, $"{newFileName.Split($"-{num}")[0]}{extension}", ++num);
                }
                return newFileName;
            });
        }

    }
}
