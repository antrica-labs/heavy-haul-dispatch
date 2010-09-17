using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Importer
{
    class TestDataBuilder
    {
        public SingerDispatchDataContext Database { get; set; }

        public TestDataBuilder(SingerDispatchDataContext context)
        {
            Database = context;
        }

        public void FillDatabaseWithStuff()
        {

        }
    }
}
