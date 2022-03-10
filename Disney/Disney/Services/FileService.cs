using Disney.Models.Auth;
using Disney.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Disney.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FileService(IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccesor)
        {
            _webHostEnviroment = webHostEnviroment;
            _httpContextAccessor = httpContextAccesor;
        }

        public async Task<UserManagerResponse> Create(byte[] file, string contentType, string extension, string container, string fileName)
        {
            string wwwrootPath = _webHostEnviroment.WebRootPath;

            if (string.IsNullOrEmpty(wwwrootPath))
                return new UserManagerResponse
                {
                    Message = "No existe la ruta",
                    IsSuccess = false
                };

            string fileFolder = Path.Combine(wwwrootPath, container);
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);    
            }

            string fileNameFinal = $"{fileName}{extension}";
            
            string fileFolderFinal = Path.Combine(fileFolder, fileNameFinal);

            await File.WriteAllBytesAsync(fileFolderFinal, file);

            string currentUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            
            string dbUrl = Path.Combine(currentUrl, container, fileNameFinal).Replace("\\", "/");

            return new UserManagerResponse
            {
                Message = dbUrl,
                IsSuccess = true
            };
        }

        public UserManagerResponse Delete(string fileRoot, string container)
        {
            string wwwrootPath = _webHostEnviroment.WebRootPath;

            if (string.IsNullOrEmpty(wwwrootPath))
                return new UserManagerResponse
                {
                    Message = "No existe la ruta",
                    IsSuccess = false
                };

            var fileName = Path.GetFileName(fileRoot);

            string finalPath = Path.Combine(wwwrootPath, container, fileName);

            if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
            }

            return new UserManagerResponse
            {
                Message = "El archivo fue eliminado correctamente",
                IsSuccess = true
            };
        }

        public async Task<string> SaveImage(IFormFile image, string container)
        {
            using var stream = new MemoryStream();

            await image.CopyToAsync(stream);

            var fileBytes = stream.ToArray();

            var result = await Create(fileBytes, image.ContentType, Path.GetExtension(image.FileName), container, Guid.NewGuid().ToString());

            string stringurl = result.Message;

            if (result.IsSuccess)
                return result.Message.ToString();

            return "";

        }

        public static void GetFileData(string base64String, out string contentType, out string imageType)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    contentType = "image/png";
                    imageType = "png";
                    break;
                case "/9J/4":
                    contentType = "image/jpeg";
                    imageType = "jpg";
                    break;
                case "AAAAF":
                    contentType = "audio/mp4";
                    imageType = "mp4";
                    break;
                case "JVBER":
                    contentType = "application/pdf";
                    imageType = "pdf";
                    break;
                case "AAABA":
                    contentType = "image/vnd.microsoft.icon";
                    imageType = "ico";
                    break;
                case "UMFYI":
                    contentType = "application/vnd.rar";
                    imageType = "rar";
                    break;
                case "E1XYD":
                    contentType = "application/rtf";
                    imageType = "rtf";
                    break;
                case "U1PKC":
                    contentType = "text/plain";
                    imageType = "txt";
                    break;
                case "MQOWM":
                case "77U/M":
                    contentType = "application/x-subrip";
                    imageType = "srt";
                    break;
                default:
                    contentType = string.Empty;
                    imageType = string.Empty;
                    break;
            }
        }

        public async Task<string> UploadEncodedImageAsync(string rawBase64File, string newName, string container)
        {
            GetFileData(rawBase64File, out string contentType, out string imageType);

            var fileName = newName.Replace(" ", "_") + "." + imageType;

            var formFileData = new FormFileData()
            {
                FileName = fileName,
                ContentType = contentType,
                Name = newName
            };

            byte[] imageBinaryFile = Convert.FromBase64String(rawBase64File);
            IFormFile newFile = ConvertFile.BinaryToFormFile(imageBinaryFile, formFileData);
            return await SaveImage(newFile, container); 
        }
    }
}
