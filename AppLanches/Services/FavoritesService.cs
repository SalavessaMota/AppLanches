using AppLanches.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Services
{
    public class FavoritesService
    {
        private readonly SQLiteAsyncConnection _database;

        public FavoritesService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Favorites.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<FavoriteProduct>().Wait();
        }

        public async Task<FavoriteProduct> ReadAsync(int id)
        {
            try
            {
                return await _database.Table<FavoriteProduct>().Where(x => x.ProductId == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FavoriteProduct>> ReadAllAsync()
        {
            try
            {
                return await _database.Table<FavoriteProduct>().ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }            
        }


        public async Task<int> CreateAsync(FavoriteProduct favoriteProduct)
        {
            try
            {
                return await _database.InsertAsync(favoriteProduct);
            }
            catch (Exception)
            {

                throw;
            }            
        }


        public async Task DeleteAsync(FavoriteProduct favoriteProduct)
        {
            try
            {
                await _database.DeleteAsync(favoriteProduct);
            }
            catch (Exception)
            {

                throw;
            }            
        }
    }
}
