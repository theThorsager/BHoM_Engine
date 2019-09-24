﻿using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        [Input("expression","The expression to increment")]
        [Output("A UnaryExpression that increments an expression and returns its previous value")]
        [Description("Creates a UnaryExpression that increments an expression and returns its previous value")]
        public static UnaryExpression PostIncrement(Expression expression)
        {
            return Expression.PostIncrementAssign(expression);
        }
    }
}
