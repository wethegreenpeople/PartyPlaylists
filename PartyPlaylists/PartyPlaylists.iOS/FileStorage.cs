using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyPlaylists.Services;

namespace PartyPlaylists.iOS
{
    public class FileStorage : IFileStorage
    {
        public Task<byte[]> ReadAsBytes(string filename)
        {
            var data = File.ReadAllBytes(filename);

            if (data != null)
                data = CleanByteOrderMark(data);

            return Task.FromResult(data);
        }

        public async Task<string> ReadAsString(string filename)
        {
            var data = await ReadAsBytes(filename);

            if (data == null)
                return string.Empty;

            return System.Text.Encoding.UTF8.GetString(data);
        }

        public async Task SaveFile(string filename, string fileContent)
        {
            await File.WriteAllTextAsync(filename, fileContent);
        }

        public byte[] CleanByteOrderMark(byte[] bytes)
        {
            var bom = new byte[] { 0xEF, 0xBB, 0xBF };
            var empty = Enumerable.Empty<byte>();
            if (bytes.Take(3).SequenceEqual(bom))
                return bytes.Skip(3).ToArray();

            return bytes;
        }
    }
}