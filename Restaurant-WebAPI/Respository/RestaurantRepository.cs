using System;
using System.Collections.Generic;
using System.Data;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Models;
using Restaurant_WebAPI.Util;

namespace Restaurant_WebAPI.Repository
{
    public class RestaurantRepository : IRestaurantRepository
    {
        public List<Restaurant> GetRestaurantsByUserId(int userId)
        {
            List<Restaurant> restaurants = new List<Restaurant>();

            string query = @"SELECT id, name, description FROM Restaurant WHERE userId = @id";
            var userIdParam = DBUtil.CreateParameter("@id", DbType.Int32, userId);

            using (var reader = DBUtil.ExecuteReader(query, userIdParam))
            {
                while (reader.Read())
                {
                    restaurants.Add(new Restaurant
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Description = reader["description"].ToString()
                    });
                }
            }
            return restaurants;
        }

        public List<Restaurant> GetAllRestaurants()
        {
            string query = @"SELECT * FROM Restaurant";
            var restaurants = new List<Restaurant>();

            using (var reader = DBUtil.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    restaurants.Add(new Restaurant
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Description = reader["description"].ToString()
                    });
                }
            }
            return restaurants;
        }

        public List<MenuItemModel> GetMenuItemsByRestaurantId(int restaurantId)
        {
            string query = @"
                SELECT mi.*, m.menuName FROM MenuItem mi
                INNER JOIN Menu m ON mi.menuId = m.id
                WHERE m.restaurantId = @RestaurantId";

            var param = DBUtil.CreateParameter("@RestaurantId", DbType.Int32, restaurantId);
            var items = new List<MenuItemModel>();

            using (var reader = DBUtil.ExecuteReader(query, param))
            {
                while (reader.Read())
                {
                    items.Add(new MenuItemModel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        MenuId = Convert.ToInt32(reader["menuId"]),
                        ItemName = reader["itemName"].ToString(),
                        ItemType = reader["menuName"].ToString(),
                        IsAvailable = Convert.ToBoolean(reader["isAvailable"]),
                        Price = Convert.ToDecimal(reader["price"])
                    });
                }
            }
            return items;
        }
    }
}
