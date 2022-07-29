using System;
using System.Linq.Expressions;

namespace pdq.state.Utilities
{
    static class MemberAccess
    {
        public static object GetValue(Expression expression)
        {
            var objectMember = Expression.Convert(expression, expression.Type);
            var getterLambda = Expression.Lambda(objectMember);

            try
            {
                var getter = getterLambda.Compile();

                return getter.DynamicInvoke();
            }
            catch
            {
                return null;
            }
        }

        public static Type GetType(Expression expression)
        {
            return ((MemberExpression)expression).Type;
        }

        public static string GetName(Expression expression, IReflectionHelper helper)
        {
            var memberExpr = (MemberExpression)expression;
            return helper.GetFieldName(memberExpr.Member);
        }
    }
}
