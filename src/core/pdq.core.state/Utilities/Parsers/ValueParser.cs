using System;
using System.Linq.Expressions;
using System.Reflection;
using pdq.common;
using pdq.state.Conditionals;

namespace pdq.state.Utilities.Parsers
{
	internal class ValueParser : BaseParser
	{
        private readonly CallExpressionHelper callExpressionHelper;

        public ValueParser(
            IExpressionHelper expressionHelper,
            CallExpressionHelper callExpressionHelper,
            IReflectionHelper reflectionHelper)
            : base(expressionHelper, reflectionHelper)
        {
            this.callExpressionHelper = callExpressionHelper;
        }

        public override state.IWhere Parse(Expression expression, IQueryContextInternal context)
        {
            var earlyResult = callExpressionHelper.ParseExpression(expression, context);
            if (earlyResult != null) return earlyResult;

            var valueResult = ParseMemberExpression(expression, context);
            if (valueResult == null) valueResult = ParseBinaryExpression(expression);

            var queryTarget = context.GetQueryTarget(valueResult.Alias);
            if (queryTarget == null)
            {
                queryTarget = QueryTargets.TableTarget.Create(valueResult.Table, valueResult.Alias);
                context.AddQueryTarget(queryTarget);
            }
            
            var col = state.Column.Create(valueResult.Field, queryTarget);

            //add the model type for the type def of the repository
            var convertedValue = GetConvertedValue(valueResult.Value, valueResult.ValueType);
            var parameters = new[] { col, valueResult.Operator, convertedValue };
            var parameterTypes = new[] { col.GetType(), typeof(EqualityOperator), valueResult.ValueType };
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            //get the generic type definition for the model
            var genericType = typeof(Column<>);
            var genericTypeArguments = new Type[] { valueResult.ValueType };
            var constructedType = genericType.MakeGenericType(genericTypeArguments);
            var ctor = constructedType.GetConstructor(bindingFlags, null, parameterTypes, null);

            object instance;

            if(ctor != null) instance = ctor.Invoke(parameters);
            else instance = Activator.CreateInstance(constructedType, parameters);

            return (state.IWhere)instance;
        }

        private object GetConvertedValue(object val, Type valType)
        {
            if (valType.IsEnum) return Enum.ToObject(valType, val);
            if (val == null && valType == typeof(bool)) return true;
            if (val == null) return null;

            object convertedValue;
            try
            {
                convertedValue = Convert.ChangeType(val, valType);
            }
            catch
            {
                var underlyingType = reflectionHelper.GetUnderlyingType(valType);
                convertedValue = Convert.ChangeType(val, underlyingType);
            }

            return convertedValue;
        }

        private ValueResult ParseMemberExpression(Expression expression, IQueryContextInternal context)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null) return null;

            var val = Parse(expression, context);
            var valType = this.expressionHelper.GetType(expression);
            var field = this.expressionHelper.GetName(expression);
            var table = this.reflectionHelper.GetTableName(memberExpression.Member.DeclaringType);
            var alias = this.expressionHelper.GetParameterName(expression);
            var op = EqualityOperator.Equals;
            return new ValueResult(field, table, alias, val, valType, op);
        }

        private ValueResult ParseBinaryExpression(Expression expression)
        {
            BinaryExpression operation;
            if (expression.NodeType == ExpressionType.Lambda)
            {
                var lambda = expression as LambdaExpression;
                operation = lambda.Body as BinaryExpression;
            }
            else
            {
                operation = expression as BinaryExpression;
            }

            var op = this.expressionHelper.ConvertExpressionTypeToEqualityOperator(operation.NodeType);
            string field, alias, table;
            Expression valueExpression;

            if(operation.Left is MemberExpression)
            {
                var memberExpression = operation.Left as MemberExpression;
                field = this.expressionHelper.GetName(operation.Left);
                alias = this.expressionHelper.GetParameterName(operation.Left);
                table = this.reflectionHelper.GetTableName(memberExpression.Member.DeclaringType);
                valueExpression = operation.Right;
            }
            else if(operation.Right is MemberExpression)
            {
                var memberExpression = operation.Right as MemberExpression;
                field = this.expressionHelper.GetName(operation.Right);
                alias = this.expressionHelper.GetParameterName(operation.Right);
                table = this.reflectionHelper.GetTableName(memberExpression.Member.DeclaringType);
                valueExpression = operation.Left;
            }
            else
            {
                return null;
            }

            var value = this.expressionHelper.GetValue(valueExpression);
            var valueType = this.expressionHelper.GetType(valueExpression);

            return new ValueResult(field, table, alias, value, valueType, op);
        }

        private sealed class ValueResult
        {
            public ValueResult() { }

            public ValueResult(string field, string table, string alias, object value, Type valueType, EqualityOperator op)
            {
                Field = field;
                Alias = alias;
                Table = table;
                Value = value;
                ValueType = valueType;
                Operator = op;
            }

            public object Value { get; set; }
            public Type ValueType { get; set; }
            public string Field { get; set; }
            public string Table { get; set; }
            public string Alias { get; set; }
            public EqualityOperator Operator { get; set; }
        }
    }
}

