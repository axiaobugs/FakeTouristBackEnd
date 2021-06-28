using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiechengAPI.Moldes;

namespace XiechengAPI.Services
{
    public interface ITouristRepository
    {
        IEnumerable<TouristRoute> GetTouristRoutes(string keyword,string ratingOperator,int? ratingValue);
        TouristRoute GetTouristRoute(Guid touristRouteId);
        bool TouristRouteExists(Guid touristRouteId);
        IEnumerable<TouristRoutePicture> GetPictureByTouristRouteId(Guid touristRouteId);
        TouristRoutePicture GetPicture(int pictureId);
        void AddTouristRoute(TouristRoute touristRoute);
        bool Save();
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
        IEnumerable<TouristRoute> GeTouristRouteByIdList(IEnumerable<Guid> ids);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
    }
}
 