using Disney.Models.Auth;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Disney.Services
{
    public interface IFileService
    {
        public Task<UserManagerResponse> Create(byte[] file, string contentType, string extension, string container, string fileName);
        public UserManagerResponse Delete(string fileRoot, string container);
        public Task<string> SaveImage(IFormFile image, string container);
        public Task<string> UploadEncodedImageAsync(string rawBase64File, string newName, string container);

    }
}
