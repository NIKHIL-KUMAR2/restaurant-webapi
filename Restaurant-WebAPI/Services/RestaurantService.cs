using System.Collections.Generic;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository restaurantRepository;

        public RestaurantService(IRestaurantRepository resRepository)
        {
            restaurantRepository = resRepository;
        }

        public List<Restaurant> GetUserRestaurants(int userId)
        {
            return restaurantRepository.GetRestaurantsByUserId(userId);
        }

        public List<Restaurant> GetAllRestaurants()
        {
            return restaurantRepository.GetAllRestaurants();
        }

        public List<MenuItemModel> GetMenuItems(int restaurantId)
        {
            return restaurantRepository.GetMenuItemsByRestaurantId(restaurantId);
        }
    }
}
