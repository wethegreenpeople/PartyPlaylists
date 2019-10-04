using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public interface IDataStore<T>
    {
        Task<T> AddItemAsync(T item);
        Task<T> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
