﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing
{
    interface Renderer
    {
        string GenerateHTML(object entity);
    }
}
