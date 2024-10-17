﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Services
{
    public static class ServiceFactory
    {
        public static FavoritesService CreateFavoritesService()
        {
            return new FavoritesService();
        }
    }
}
