using System.Web.Http;
using System;
using Restaurant_WebAPI.Models;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Util;
using System.Security.Claims;
using System.Linq;

namespace Restaurant_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix(Constants.OrderRoutePrefix)]
    public class OrderController : ApiController
    {
        private readonly IOrderServiceAPI orderService;

        public OrderController(IOrderServiceAPI orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateOrder([FromBody] NewOrderRequest request)
        {
            if(request == null)
            {
                return BadRequest("Order Data is required to place Order");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!request.Items.Any())
            {
                return BadRequest("At least One item required to create order");
            }
            try
            {
                // Getting user id from token
                var userIdClaim = (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                    return Unauthorized();

                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized();

                OrderCreationResult result = orderService.PlaceOrder(request,userId);
                return Ok(new { message = "Order created successfully", result });
            }
            catch (OrderCreationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return InternalServerError(new Exception("An error has occured while creating order"));
            }
        }
    }
}
