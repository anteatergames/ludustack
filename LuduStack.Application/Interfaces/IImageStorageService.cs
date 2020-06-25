using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> StoreImageAsync(string container, string fileName, byte[] image);

        Task<string> DeleteImageAsync(string container, string fileName);
    }
}