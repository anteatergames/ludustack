using LuduStack.Domain.ValueObjects;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<UploadResultVo> StoreMediaAsync(string container, string fileName, string extension, byte[] image, params string[] tags);

        Task<string> DeleteImageAsync(string container, string fileName);
    }
}