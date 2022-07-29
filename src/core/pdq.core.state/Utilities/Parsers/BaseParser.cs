using System;
using System.Linq.Expressions;
using pdq.common;

namespace pdq.state.Utilities
{
	internal abstract class BaseParser : IParser
	{
		protected readonly IExpressionHelper expressionHelper;
        protected readonly IReflectionHelper reflectionHelper;

		protected BaseParser(
            IExpressionHelper expressionHelper,
            IReflectionHelper reflectionHelper)
		{
			this.expressionHelper = expressionHelper;
            this.reflectionHelper = reflectionHelper;
		}

        /// <inheritdoc/>
        public abstract IWhere Parse(Expression expression, IQueryContextInternal context);

        /// <summary>
        /// This is used for the case where we are doing a column.Match<T1,T2>((t1,t2) => t1.someNullable == t2.nonNull)
        /// We check for the null-able and then get the underlying expression to join against
        /// </summary>
        /// <param name="operation">The operation to check for null ables</param>
        /// <returns>A Tuple of Left,Right Expression</returns>
        protected Tuple<Expression, Expression> UpdateNullableExpression(
            BinaryExpression operation)
        {
            var left = operation.Left;
            var right = operation.Right;
            if (left.NodeType == ExpressionType.Convert)
            {
                var temp = (UnaryExpression)left;
                left = temp.Operand;
            }

            if (right.NodeType == ExpressionType.Convert)
            {
                var temp = (UnaryExpression)right;
                right = temp.Operand;
            }

            return new Tuple<Expression, Expression>(left, right);
        }
    }
}

