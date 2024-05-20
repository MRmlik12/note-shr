using System.Threading.Tasks;

namespace NoteSHR.Core.Services;

public interface IFilePicker
{
    Task<string?> GetFileUrl();
}