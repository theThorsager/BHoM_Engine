﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsAutoGenerated(this MethodBase method)
        {
            return method.GetCustomAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)) != null;
        }


        /***************************************************/

        public static bool IsAutoGenerated(this Type type) 
        {
            return type.DeclaringType != null;
        }


        /***************************************************/



    }
}
