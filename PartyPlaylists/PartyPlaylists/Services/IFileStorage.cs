using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public interface IFileStorage
    {
        Task<string> ReadAsString(string filename);
        Task SaveFile(string filename, string fileContent);
    }
}
