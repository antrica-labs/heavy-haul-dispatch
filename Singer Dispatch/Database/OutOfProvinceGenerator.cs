using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Database
{
    class OutOfProvinceGenerator
    {
        public static IEnumerable<OutOfProvinceTravel> GetAllOutOfProvince(DateTime start, DateTime end, SingerDispatchDataContext context)
        {
            return from t in context.OutOfProvinceTravels select t;
        }
    }
}
