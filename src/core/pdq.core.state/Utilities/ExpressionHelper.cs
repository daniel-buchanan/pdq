using System;
using System.Linq.Expressions;
using pdq.common;

namespace pdq.state.Utilities
{
    internal class ExpressionHelper : IExpressionHelper
    {
        private readonly IReflectionHelper reflectionHelper;

        public ExpressionHelper(IReflectionHelper reflectionHelper)
        {
            // setup helpers
            this.reflectionHelper = reflectionHelper;
        }

        /// <summary>
        /// Get the field name from the expression. i.e. p => p.ID (ID would be the field name). Note that this *should* be a SIMPLE expression, as in the previous example.
        /// </summary>
        /// <param name="expression">The expression to get the name from</param>
        /// <returns>The name of the field</returns>
        public string GetName(Expression expression)
        {
            // switch on node type
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    return ConvertAccess.GetName(expression, this);
                case ExpressionType.MemberAccess:
                    return MemberAccess.GetName(expression, this.reflectionHelper);
                case ExpressionType.Constant:
                    return MemberAccess.GetName(expression, this.reflectionHelper);
                case ExpressionType.Call:
                    {
                        var call = (MethodCallExpression)expression;
                        if (call.Object == null)
                        {
                            return GetName(call.Arguments[0]);
                        }
                        return GetName(call.Object);
                    }
                case ExpressionType.Lambda:
                    var body = ((LambdaExpression)expression).Body;
                    return this.GetName(body);
            }

            // if not found, return null
            return null;
        }

        public string GetMethodName(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Lambda) expression = ((LambdaExpression)expression).Body;
            if (expression.NodeType == ExpressionType.Convert) expression = ((UnaryExpression)expression).Operand;

            var methodExpression = expression as MethodCallExpression;
            if (methodExpression == null) return null;

            return methodExpression.Method.Name;
        }

        /// <summary>
        /// Get the name of the parameter used in an expression. i.e. p => p.ID (p would be the parameter name). Note that this *should* be a SIMPLE expression, as in the previous example.
        /// </summary>
        /// <param name="expr">The expression to get the parameter name from</param>
        /// <returns>The paramter name</returns>
        public string GetParameterName(Expression expr)
        {
            // check the expression for null
            if (expr == null)
                throw new ArgumentNullException(nameof(expr), "The provided expression cannot be null.");

            // check for member expressions
            if (expr is MemberExpression)
            {
                // get member expression
                var memberExpression = expr as MemberExpression;

                // check if we have a parameter expression
                if (memberExpression?.Expression is ParameterExpression)
                {
                    var parameterExpression = memberExpression.Expression as ParameterExpression;
                    return parameterExpression?.Name;
                }
            }

            if (expr is MethodCallExpression)
            {
                var call = expr as MethodCallExpression;
                if (call == null) return null;
                if (call.Object == null) return GetParameterName(call.Arguments[0]);

                return GetParameterName(call.Object);
            }

            // if the expression is not a lambda, return nothing
            if (!(expr is LambdaExpression)) return null;

            // get lambda and parameter
            var lambdaExpr = (LambdaExpression)expr;
            var param = lambdaExpr.Parameters[0];

            // if no parameter, return null
            if (param == null)
                return null;

            // return parameter name
            return param.Name;
        }

        /// <summary>
        /// Get the table/type name
        /// </summary>
        /// <typeparam name="TObject">The type to get the name for</typeparam>
        /// <returns>The name of the type or table</returns>
        public string GetTypeName<TObject>()
        {
            // use the reflection helper to get the table name
            // NOTE: this will use the [TableName] attribute if present
            return reflectionHelper.GetTableName(typeof(TObject));
        }

        public EqualityOperator ConvertExpressionTypeToEqualityOperator(ExpressionType type)
        {
            // convert expression EqualityOperator into our EqualityOperator
            switch (type)
            {
                case ExpressionType.Equal:
                    return EqualityOperator.Equals;
                case ExpressionType.GreaterThan:
                    return EqualityOperator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return EqualityOperator.GreaterThanOrEqualTo;
                case ExpressionType.LessThan:
                    return EqualityOperator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return EqualityOperator.LessThanOrEqualTo;
                case ExpressionType.NotEqual:
                    return EqualityOperator.NotEquals;
            }

            // if nothing found, default to equals
            return EqualityOperator.Equals;
        }

        public EqualityOperator ConvertExpressionTypeToEqualityOperator(Expression expression)
        {
            var binaryExpr = expression as BinaryExpression;
            if (binaryExpr == null) return EqualityOperator.Equals;
            return ConvertExpressionTypeToEqualityOperator(binaryExpr.NodeType);
        }

        /// <inheritdoc/>
        public object GetValue(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                try
                {
                    expression = ConvertAccess.GetExpression(expression);
                }
                catch
                {
                    return null;
                }
            }


            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return MemberAccess.GetValue(expression);
            }
            else if (expression.NodeType == ExpressionType.Constant)
            {
                return ConstantAccess.GetValue(expression);
            }
            else if (expression.NodeType == ExpressionType.Lambda)
            {
                var body = ((LambdaExpression)expression).Body;
                return GetValue(body);
            }
            else if (expression.NodeType == ExpressionType.Call)
            {
                try
                {
                    return Expression.Lambda(expression).Compile().DynamicInvoke();
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public Type GetType(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                return ConvertAccess.GetType(expression, this);
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return MemberAccess.GetType(expression);
            }
            else if (expression.NodeType == ExpressionType.Constant)
            {
                return ConstantAccess.GetType(expression);
            }
            else if(expression.NodeType == ExpressionType.Parameter)
            {
                return ParameterAccess.GetType(expression);
            }
            else if (expression.NodeType == ExpressionType.Lambda)
            {
                var body = ((LambdaExpression)expression).Body;
                return GetType(body);
            }

            return null;
        }
    }
}
