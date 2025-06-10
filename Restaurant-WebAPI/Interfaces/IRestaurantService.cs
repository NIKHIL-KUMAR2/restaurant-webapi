using System.Collections.Generic;
using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Interfaces
{
    public interface IRestaurantService
    {
        List<Restaurant> GetUserRestaurants(int userId);

        List<Restaurant> GetAllRestaurants();

        List<MenuItemModel> GetMenuItems(int restaurantId);
    }
}
