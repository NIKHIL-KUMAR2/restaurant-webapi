using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Interfaces
{
    public interface IOrderServiceAPI
    {
        OrderCreationResult PlaceOrder(NewOrderRequest request, int userId);
    }
}
