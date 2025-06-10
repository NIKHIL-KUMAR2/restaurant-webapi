using System.Collections.Generic;
using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Interfaces
{
    public interface IRestaurantRepository
    {
        List<Restaurant> GetRestaurantsByUserId(int userId);

        List<Restaurant> GetAllRestaurants();

        List<MenuItemModel> GetMenuItemsByRestaurantId(int restaurantId);
    }
}
