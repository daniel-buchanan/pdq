using System;
using System.Linq.Expressions;

namespace pdq.state.Utilities
{
    static class ConvertAccess
    {
        private static Expression GetOperand(Expression expression)
        {
            var unaryExpression = (UnaryExpression)expression;
            return unaryExpression.Operand;
        }

        public static Expression GetExpression(Expression expression)
        {
            return GetOperand(expression);
        }

        public static object GetValue(Expression expression)
        {
            throw new NotImplementedException();
        }

        public static Type GetType(Expression expression, ExpressionHelper helper)
        {
            return helper.GetType(GetOperand(expression));
        }

        public static string GetName(Expression expression, ExpressionHelper helper)
        {
            return helper.GetName(GetOperand(expression));
        }
    }
}
