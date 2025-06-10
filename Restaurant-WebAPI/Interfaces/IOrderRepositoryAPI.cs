using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Interfaces
{
    public interface IOrderRepositoryAPI
    {
        OrderCreationResult PlaceOrder(NewOrderRequest request, int userId);
    }
}
