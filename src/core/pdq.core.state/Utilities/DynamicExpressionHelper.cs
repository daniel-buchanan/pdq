using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace pdq.state.Utilities
{
    internal class DynamicExpressionHelper : IDynamicExpressionHelper
    {
        private readonly IExpressionHelper expressionHelper;
        private readonly CallExpressionHelper callExpressionHelper;

        public DynamicExpressionHelper(
            IExpressionHelper expressionHelper,
            CallExpressionHelper callExpressionHelper)
        {
            this.expressionHelper = expressionHelper;
            this.callExpressionHelper = callExpressionHelper;
        }

        /// <inheritdoc/>
        public IEnumerable<DynamicPropertyInfo> GetProperties(Expression expr, IQueryContextInternal context)
        {
            var expression = (LambdaExpression)expr;

            if (expression.Body is MemberInitExpression)
                return GetPropertiesForMemberInit(expression, context);

            return GetPropertiesForNew(expression, context);
        }

        private List<DynamicPropertyInfo> GetPropertiesForMemberInit(
            LambdaExpression expression,
            IQueryContextInternal context)
        {
            var initExpr = (MemberInitExpression)expression.Body;

            var numBindings = initExpr.Bindings.Count;
            var properties = new DynamicPropertyInfo[numBindings];

            var index = 0;
            foreach (var b in initExpr.Bindings)
            {
                var memberBinding = b as MemberAssignment;
                if (!TryGetColumnDetails(memberBinding.Expression, context, out var info))
                    continue;

                var newName = memberBinding.Member.Name;
                info.SetNewName(newName);

                if (memberBinding.Member.MemberType == MemberTypes.Field)
                    info.SetType((memberBinding.Member as FieldInfo).FieldType);

                if (memberBinding.Member.MemberType == MemberTypes.Property)
                    info.SetType((memberBinding.Member as PropertyInfo).PropertyType);

                properties[index] = info;
                index += 1;
            }

            return properties.ToList();
        }

        private List<DynamicPropertyInfo> GetPropertiesForNew(
            LambdaExpression expression,
            IQueryContextInternal context)
        {
            var body = (NewExpression)expression.Body;

            var countArguments = body.Arguments.Count;
            var properties = new DynamicPropertyInfo[countArguments];

            for (var i = 0; i < countArguments; i += 1)
            {
                if (!TryGetColumnDetails(body.Arguments[i], context, out var info))
                    continue;

                properties[i] = info;
            }

            for (var i = 0; i < body.Members.Count; i += 1)
            {
                var m = body.Members[i];
                if (m.Name == properties[i].Name) continue;

                properties[i].SetNewName(m.Name);
            }

            return properties.ToList();
        }

        private bool TryGetColumnDetails(
            Expression expression,
            IQueryContextInternal context,
            out DynamicPropertyInfo info)
        {
            info = null;
            string column = null;
            string alias = null;
            IValueFunction function = null;

            MemberExpression memberExpression = null;
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                if (TryGetColumnDetailsDynamic(methodCallExpression, out info))
                    return true;

                memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
            }

            if(expression is UnaryExpression)
            {
                var unaryExpression = expression as UnaryExpression;
                methodCallExpression = unaryExpression.Operand as MethodCallExpression;
            }

            if(memberExpression == null)
                memberExpression = expression as MemberExpression;

            if (methodCallExpression != null)
            {
                Expression argument;
                if (methodCallExpression.Arguments.Count == 1)
                    argument = methodCallExpression.Arguments[0];
                else if (methodCallExpression.Arguments.Count > 1)
                    argument = methodCallExpression.Arguments[0];
                else
                    argument = methodCallExpression.Arguments.FirstOrDefault();

                memberExpression = argument as MemberExpression;

                var defaultValue = methodCallExpression.Method.ReturnType.IsValueType ?
                    Activator.CreateInstance(methodCallExpression.Method.ReturnType) :
                    null;
                var tempExpression = Expression.Equal(methodCallExpression, Expression.Constant(defaultValue));
                var parsed = this.callExpressionHelper.ParseExpression(tempExpression, context);
                if (parsed is Conditionals.IColumn)
                {
                    var parsedColumn = parsed as Conditionals.IColumn;
                    function = parsedColumn.ValueFunction;
                }
            }

            if (memberExpression == null) return false;

            var parameterExpression = memberExpression.Expression as ParameterExpression;
            column = this.expressionHelper.GetName(memberExpression);
            alias = this.expressionHelper.GetParameterName(memberExpression);
            var type = parameterExpression.Type;
            info = DynamicPropertyInfo.Create(name: column, alias: alias, type: type, function: function);

            return true;
        }

        private bool TryGetColumnDetailsDynamic(MethodCallExpression expression, out DynamicPropertyInfo info)
        {
            info = null;
            string column, alias = null;

            if (expression.Method.DeclaringType != typeof(ISelectColumnBuilder))
                return false;

            if (expression.Arguments.Count == 1)
            {
                var arg = expression.Arguments[0] as ConstantExpression;
                column = arg.Value as string;
            }
            else
            {
                var arg = expression.Arguments[0] as ConstantExpression;
                column = arg.Value as string;

                arg = expression.Arguments[1] as ConstantExpression;
                alias = arg.Value as string;
            }

            info = DynamicPropertyInfo.Create(name: column, alias: alias, type: typeof(object));

            return true;
        }
    }
}

