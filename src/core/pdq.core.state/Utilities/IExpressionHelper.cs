using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using pdq.common;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("pdq")]
namespace pdq.state.Utilities
{
    internal interface IExpressionHelper
    {
        EqualityOperator ConvertExpressionTypeToEqualityOperator(Expression expression);

        EqualityOperator ConvertExpressionTypeToEqualityOperator(ExpressionType type);

        string GetMethodName(Expression expression);

        /// <summary>
        /// Get the field name from the expression. i.e. p => p.ID (ID would be the field name). Note that this *should* be a SIMPLE expression, as in the previous example.
        /// </summary>
        /// <param name="expression">The expression to get the name from</param>
        /// <returns>The name of the field</returns>
        string GetName(Expression expression);

        /// <summary>
        /// Get the name of the parameter used in an expression. i.e. p => p.ID (p would be the parameter name). Note that this *should* be a SIMPLE expression, as in the previous example.
        /// </summary>
        /// <param name="expr">The expression to get the parameter name from</param>
        /// <returns>The paramter name</returns>
        string GetParameterName(Expression expr);

        Type GetType(Expression expression);

        /// <summary>
        /// Get the table/type name
        /// </summary>
        /// <typeparam name="TObject">The type to get the name for</typeparam>
        /// <returns>The name of the type or table</returns>
        string GetTypeName<TObject>();

        object GetValue(Expression expression);
    }
}