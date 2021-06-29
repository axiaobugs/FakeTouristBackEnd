using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiechengAPI.Helper;
using XiechengAPI.Moldes;

namespace XiechengAPI.Services
{
    public interface ITouristRepository
    {
        Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(
            string keyword,
            string ratingOperator,
            int? ratingValue,
            int pageSize,
            int pageNumber);
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoutePicture>> GetPictureByTouristRouteIdAsync(Guid touristRouteId);
        Task<TouristRoutePicture> GetPictureAsync(int pictureId);
        Task<IEnumerable<TouristRoute>> GeTouristRouteByIdListAsync(IEnumerable<Guid> ids);
        Task<bool> SaveAsync();
        void AddTouristRoute(TouristRoute touristRoute);
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
        Task<ShoppingCart> GetShoppingCartByUserId(string userId);
        Task CreateShoppingCart(ShoppingCart shoppingCart);
        Task AddShoppingCartItem(LineItem lineItem);
        Task<LineItem> GetShoppingCartItemById(int lineItemId);
        void DeleteShoppingCartItem(LineItem lineItem);
        Task<IEnumerable<LineItem>> GetShoppingCartItemsByListAsync(IEnumerable<int> ids);
        void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems);
        Task AddOrderAsync(Order order);
        Task<PaginationList<Order>> GetOrdersByUserID(string userId,int pageNumber,int pageSize);
        Task<Order> GetOrderById(Guid orderId);
    }
}
 