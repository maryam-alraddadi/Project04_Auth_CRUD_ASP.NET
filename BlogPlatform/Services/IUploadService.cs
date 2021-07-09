using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BlogPlatform.Services
{
    public interface IUploadService
    {
        Task<string> UploadImage(IFormFile image);
    }
}