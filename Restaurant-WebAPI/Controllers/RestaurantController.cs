using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Util;
using System;
using System.Linq;
using System.Web.Http;

[Authorize]
[RoutePrefix(Constants.RestaurantRoutePrefix)]
public class RestaurantController : ApiController
{
    private readonly IRestaurantService restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        this.restaurantService = restaurantService;
    }

    [HttpGet]
    [Route("")]
    public IHttpActionResult GetAllRestaurants()
    {
        try
        {
            var restaurants = restaurantService.GetAllRestaurants();
            return Ok(restaurants);
        }
        catch
        {
            return BadRequest("Something went wrong! Please try again later");
        }
    }

    [HttpGet]
    [Route("{resId:int}/menu")]
    public IHttpActionResult GetMenuItemsByRestaurant(int resId)
    {
        try
        {
            var menuItems = restaurantService.GetMenuItems(resId);
            if (menuItems == null || !menuItems.Any())
                return BadRequest("Invalid Data provided");

            return Ok(menuItems);
        }
        catch
        {
            return InternalServerError(new Exception("An unexpected error occurred. Please try again later."));
        }
    }
}
