using System;
using System.Collections.Generic;
using System.Data;
using Restaurant_WebAPI.Util;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Repository
{
    public class OrderRepositoryAPI : IOrderRepositoryAPI
    {
        public OrderCreationResult PlaceOrder(NewOrderRequest request, int userId)
        {
            return DBUtil.ExecuteTransaction((conn, transaction) =>
            {
                // Creating a list of ids of menu items
                var itemIds = new List<int>();
                foreach (var item in request.Items)
                {
                    itemIds.Add(item.MenuItemId);
                }

                // Creating Param names for IN operator
                var paramNames = new List<string>();
                for (int i = 0; i < itemIds.Count; i++)
                {
                    paramNames.Add($"@id{i}");
                }

                string inOperatorQuery = string.Join(",", paramNames);

                // Query to get the price of menu items which are available and belong to provided restaurant
                string priceQuery = $@"
                    SELECT id, price FROM MenuItem
                    WHERE id IN ({inOperatorQuery})
                    AND isAvailable = TRUE
                    AND menuId IN (SELECT id FROM Menu WHERE restaurantId = @restaurantId)";

                var priceCmd = conn.CreateCommand();
                priceCmd.Transaction = transaction;
                priceCmd.CommandText = priceQuery;

                for (int i = 0; i < itemIds.Count; i++)
                {
                    var param = DBUtil.CreateParameter($"@id{i}", DbType.Int32, itemIds[i]);
                    priceCmd.Parameters.Add(param);
                }

                priceCmd.Parameters.Add(DBUtil.CreateParameter("@restaurantId", DbType.Int32, request.RestaurantId));

                // Storing current prices of items in a Dictionary
                var priceMap = new Dictionary<int, decimal>();
                using (var reader = priceCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id"]);
                        decimal price = Convert.ToDecimal(reader["price"]);
                        priceMap[id] = price;
                    }
                }

                // Checking if items are available and belongs to a single restaurant
                if (priceMap.Count != request.Items.Count)
                    throw new OrderCreationException("One or more items do not belong to the selected restaurant or are unavailable.");

                decimal total = 0;
                var itemDetails = new List<(int MenuItemId, decimal Price, int Quantity)>();

                foreach (var item in request.Items)
                {
                    var price = priceMap[item.MenuItemId];
                    total += price * item.Quantity;
                    itemDetails.Add((item.MenuItemId, price, item.Quantity));
                }

                // Creating Order to get order Id (PostgreSQL uses RETURNING)
                string insertOrderSql = @"
                    INSERT INTO AppOrder (userId, restaurantId, orderValue)
                    VALUES (@userId, @restaurantId, @orderValue)
                    RETURNING id";

                int orderId;
                using (var orderCmd = conn.CreateCommand())
                {
                    orderCmd.Transaction = transaction;
                    orderCmd.CommandText = insertOrderSql;
                    orderCmd.Parameters.Add(DBUtil.CreateParameter("@userId", DbType.Int32, userId));
                    orderCmd.Parameters.Add(DBUtil.CreateParameter("@restaurantId", DbType.Int32, request.RestaurantId));
                    orderCmd.Parameters.Add(DBUtil.CreateParameter("@orderValue", DbType.Decimal, total));

                    orderId = Convert.ToInt32(orderCmd.ExecuteScalar());
                }

                // Inserting Order Items
                foreach (var (menuItemId, price, quantity) in itemDetails)
                {
                    string insertItemSql = @"
                        INSERT INTO OrderItem (orderId, menuItemId, itemPrice, quantity)
                        VALUES (@orderId, @menuItemId, @itemPrice, @quantity)";

                    using (var itemCmd = conn.CreateCommand())
                    {
                        itemCmd.Transaction = transaction;
                        itemCmd.CommandText = insertItemSql;
                        itemCmd.Parameters.Add(DBUtil.CreateParameter("@orderId", DbType.Int32, orderId));
                        itemCmd.Parameters.Add(DBUtil.CreateParameter("@menuItemId", DbType.Int32, menuItemId));
                        itemCmd.Parameters.Add(DBUtil.CreateParameter("@itemPrice", DbType.Decimal, price));
                        itemCmd.Parameters.Add(DBUtil.CreateParameter("@quantity", DbType.Int32, quantity));

                        itemCmd.ExecuteNonQuery();
                    }
                }

                return new OrderCreationResult
                {
                    OrderId = orderId,
                    OrderAmount = total
                };
            });
        }
    }
}
