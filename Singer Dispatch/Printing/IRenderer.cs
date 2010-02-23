using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing
{
    interface IRenderer
    {
        string GenerateHTML(object entity);
    }
}
