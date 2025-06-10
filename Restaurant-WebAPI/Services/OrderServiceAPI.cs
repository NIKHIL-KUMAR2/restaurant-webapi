using System.Collections.Generic;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Models;
using Restaurant_WebAPI.Util;

namespace Restaurant_WebAPI.Services
{
    public class OrderServiceAPI:IOrderServiceAPI
    {
        private readonly IOrderRepositoryAPI orderRepository;

        public OrderServiceAPI(IOrderRepositoryAPI orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public OrderCreationResult PlaceOrder(NewOrderRequest request, int userId)
        {
            // Checking duplicate menu Id
            HashSet<int> itemIds = new HashSet<int>();

            foreach (var item in request.Items)
            {
                if (itemIds.Contains(item.MenuItemId))
                {
                    throw new OrderCreationException("Order creation failed. Duplicate menu items found!");
                }
                else
                {
                    itemIds.Add(item.MenuItemId);
                }
            }

            return orderRepository.PlaceOrder(request, userId);
        }

    }
}
