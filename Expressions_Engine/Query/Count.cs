﻿using BH.Engine.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Expressions.Query
{
    public static partial class Query
    {
        public static int Count(this List<object> list, LambdaExpression filter)
        {
            Delegate dlg = filter.Compile();
            return list.Count(obj => (bool)dlg.DynamicInvoke(obj));
        }
    }
}
