using ETicaretAPI.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvionment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvionment = webHostEnvironment;
        }

        public async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);
                await fileStream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Task<string> FileRenameAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<(string fileName, string path)>> UploadAsync(string filePath, IFormFileCollection files)
        {
            string uploadPath = Path.Combine(_webHostEnvionment.ContentRootPath, filePath)
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> data = new();
            List<bool> results = new();
            foreach (IFormFile file in files)
            {
                string fileNewName = await FileRenameAsync(file.FileName);

                bool result = await CopyFileAsync($"{uploadPath}\\{fileNewName}", file);
                data.Add((fileNewName, $"{uploadPath}\\{fileNewName}"));
                await CopyFileAsync($"{uploadPath}\\{fileNewName}", file)
            }
            if(results.TrueForAll(r => r.Equals(true)))
                return data;
        }
    }
}
