using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using pdq.common;
using pdq.state.Conditionals;
using pdq.state.Conditionals.ValueFunctions;

namespace pdq.state.Utilities
{
    class CallExpressionHelper
    {
        private readonly IExpressionHelper expressionHelper;

        public CallExpressionHelper(
            IExpressionHelper expressionHelper)
        {
            this.expressionHelper = expressionHelper;
        }

        public state.IWhere ParseExpression(Expression expression, IQueryContextInternal context)
        {
            Expression expressionToParse;
            if (expression is LambdaExpression)
            {
                var lambda = expression as LambdaExpression;
                var unary = lambda.Body as UnaryExpression;
                if (unary?.NodeType == ExpressionType.Not)
                {
                    expressionToParse = Expression.Equal(unary.Operand, Expression.Constant(false));
                }
                else
                {
                    expressionToParse = lambda.Body;
                }
            }
            else
            {
                expressionToParse = expression;
            }

            if(IsMethodCallOnProperty(expressionToParse))
            {
                var binaryExpression = expressionToParse as BinaryExpression;
                if(binaryExpression == null)
                {
                    var constantExpression = Expression.Constant(true);
                    binaryExpression = Expression.Equal(expressionToParse, constantExpression);
                }
                return ParseBinaryExpression(binaryExpression, context);
            }

            if(IsMethodCallOnConstantOrMemberAccess(expressionToParse))
            {
                var binaryExpression = expressionToParse as BinaryExpression;
                if (binaryExpression == null)
                {
                    return ParseMethodAccessCall(expressionToParse, context);
                }

                var call = binaryExpression.Left as MethodCallExpression ??
                           binaryExpression.Right as MethodCallExpression;
                var constant = binaryExpression.Left as ConstantExpression ??
                               binaryExpression.Right as ConstantExpression;
                var result = ParseMethodAccessCall(call, context);
                var constantValue = (bool)this.expressionHelper.GetValue(constant);

                if (binaryExpression.NodeType == ExpressionType.NotEqual ||
                    !constantValue)
                    return Not.This(result);

                return result;
            }

            return ParseBinaryExpression(expressionToParse, context);
        }

        public IValueFunction ParseFunction(Expression expression)
        {
            var callExpression = expression as MethodCallExpression;
            if (callExpression == null) return null;

            switch(callExpression.Method.Name)
            {
                case SupportedMethods.ToLower:
                    return ToLower.Create();
                case SupportedMethods.ToUpper:
                    return ToUpper.Create();
                case SupportedMethods.DatePart:
                    return ParseDatePart(callExpression);
                case SupportedMethods.Contains:
                    return ParseContains(callExpression);
                case SupportedMethods.Substring:
                    return ParseSubString(callExpression);
            }

            return null;
        }

        private bool IsMethodCallOnProperty(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;
            var binaryExpression = expression as BinaryExpression;
            if(binaryExpression != null)
            {
                var call = binaryExpression.Left as MethodCallExpression ??
                           binaryExpression.Right as MethodCallExpression;
                var constant = binaryExpression.Left as ConstantExpression ??
                               binaryExpression.Right as ConstantExpression;

                if (call is null || constant is null) return false;
                methodCallExpression = call;
            }

            if (methodCallExpression == null) return false;
            if (methodCallExpression.Arguments.Count == 0) return false;

            if (methodCallExpression.Object is MemberExpression &&
                methodCallExpression.Arguments.All(a => a.NodeType == ExpressionType.Constant) ||
                methodCallExpression.Arguments.All(a => a.NodeType == ExpressionType.MemberAccess))
            {
                var memberExpression = methodCallExpression.Object as MemberExpression ??
                    methodCallExpression.Arguments[0] as MemberExpression;
                var innerExpression = memberExpression?.Expression as ConstantExpression;
                return innerExpression is null;
            }

            var lastArgument = methodCallExpression.Arguments.Last();
            return lastArgument.NodeType == ExpressionType.MemberAccess ||
                   lastArgument.NodeType == ExpressionType.Constant;
        }

        private bool IsMethodCallOnConstantOrMemberAccess(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null)
            {
                var binaryExpression = expression as BinaryExpression;
                methodCallExpression = binaryExpression.Left as MethodCallExpression ??
                                       binaryExpression.Right as MethodCallExpression;

                if (methodCallExpression is null) return false;
            }

            if (methodCallExpression.Arguments.Count == 0) return false;

            var firstArgument = methodCallExpression.Arguments.First();
            return firstArgument.NodeType == ExpressionType.MemberAccess ||
                   firstArgument.NodeType == ExpressionType.Constant;
        }

        private state.IWhere ParseBinaryExpression(
            Expression expression,
            IQueryContextInternal context)
        {
            GetCallAndNonCallExpressions(
                expression,
                out var callExpr,
                out var nonCallExpr);

            if (callExpr == null) return null;

            var valueFunction = ParseFunction(callExpr);
            if (valueFunction == null) return null;

            var memberExpression = callExpr.Object as MemberExpression ??
                                   callExpr.Arguments.First() as MemberExpression;

            var op = this.expressionHelper.ConvertExpressionTypeToEqualityOperator(expression);
            var fieldName = this.expressionHelper.GetName(memberExpression);
            var target = context.GetQueryTarget(memberExpression);
            var col = state.Column.Create(fieldName, target);

            IWhere result = null;
            var invertResult = op == EqualityOperator.NotEquals;

            if (nonCallExpr.NodeType == ExpressionType.Constant)
            {
                var value = this.expressionHelper.GetValue(nonCallExpr);

                if(value is bool)
                {
                    var boolValue = (bool)value;
                    if(!boolValue && op == EqualityOperator.Equals)
                    {
                        value = invertResult = true;
                    }
                }

                var functionType = typeof(Column<>);
                var implementedType = functionType.MakeGenericType(value.GetType());
                var parameters = new object[] { col, op, valueFunction, value };
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                result = (state.IWhere)Activator.CreateInstance(
                    implementedType,
                    bindingFlags,
                    null,
                    parameters,
                    System.Globalization.CultureInfo.CurrentCulture);
            }

            if (nonCallExpr.NodeType == ExpressionType.MemberAccess)
            {
                var fieldB = this.expressionHelper.GetName(nonCallExpr);
                var targetB = context.GetQueryTarget(nonCallExpr);
                var colB = state.Column.Create(fieldB, targetB);
                result = Conditionals.Column.Equals(col, op, valueFunction, colB);
            }

            if (nonCallExpr.NodeType == ExpressionType.Call)
            {
                var rightCallExpr = nonCallExpr as MethodCallExpression;
                var rightBody = rightCallExpr.Object;
                var fieldB = this.expressionHelper.GetName(rightBody);
                var targetB = context.GetQueryTarget(rightBody);
                var colB = state.Column.Create(fieldB, targetB);
                valueFunction = ParseFunction(rightCallExpr);

                result = Conditionals.Column.Equals(col, op, valueFunction, colB);
            }

            if (invertResult) return Not.This(result);

            return result;
        }

        private void GetCallAndNonCallExpressions(
            Expression expression,
            out MethodCallExpression callExpr,
            out Expression nonCallExpr)
        {
            callExpr = null;
            nonCallExpr = null;

            var binaryExpr = expression as BinaryExpression;
            if (binaryExpr == null)
            {
                var lambda = expression as LambdaExpression;
                binaryExpr = lambda.Body as BinaryExpression;
            }

            if (binaryExpr == null) return;

            var left = binaryExpr.Left;
            var right = binaryExpr.Right;

            if (left.NodeType == ExpressionType.Call)
            {
                callExpr = left as MethodCallExpression;
                nonCallExpr = right;
            }
            else if (right.NodeType == ExpressionType.Call)
            {
                callExpr = right as MethodCallExpression;
                nonCallExpr = left;
            }
        }

        private IValueFunction ParseContains(MethodCallExpression expression)
        {
            var arg = expression.Arguments[0];
            var value = this.expressionHelper.GetValue(arg) as string;
            return StringContains.Create(value);
        }

        private IValueFunction ParseDatePart(MethodCallExpression expression)
        {
            var arguments = expression.Arguments;
            var datePartExpression = arguments[1];
            var dp = (common.DatePart)this.expressionHelper.GetValue(datePartExpression);
            return Conditionals.ValueFunctions.DatePart.Create(dp);
        }

        private IValueFunction ParseSubString(MethodCallExpression expression)
        {
            var arguments = expression.Arguments;
            var startExpression = arguments[0];
            Expression lengthExpression = null;
            if (arguments.Count > 1) lengthExpression = arguments[1];

            var startValue = (int)this.expressionHelper.GetValue(startExpression);
            if (lengthExpression != null)
            {
                var lengthValue = (int)this.expressionHelper.GetValue(lengthExpression);
                return Substring.Create(startValue, lengthValue);
            }

            return Substring.Create(startValue);
        }

        private state.IWhere ParseMethodAccessCall(Expression expression, IQueryContextInternal context)
        {
            var callExpression = expression as MethodCallExpression;
            if (callExpression == null) return null;

            IWhere result = null;
            switch(callExpression.Method.Name)
            {
                case SupportedMethods.Contains:
                    result = ParseVariableContainsAsValuesIn(callExpression, context);
                    break;
            }

            return result;
        }

        private state.IWhere ParseVariableContainsAsValuesIn(MethodCallExpression call, IQueryContextInternal context)
        {
            var arg = call.Arguments[0];
            if (arg.NodeType != ExpressionType.MemberAccess) return null;

            MemberExpression memberAccessExp;
            Expression valueAccessExp;

            // check if this is a list or contains with variable
            if (call.Arguments.Count != 1)
            {
                // if the underlying value is an array
                memberAccessExp = call.Arguments[1] as MemberExpression;
                valueAccessExp = call.Arguments[0];
            }
            else
            {
                // check for passed in variable
                memberAccessExp = call.Arguments[0] as MemberExpression;
                valueAccessExp = call.Object;
            }

            if (memberAccessExp == null) return null;

            // get values
            var valueMember = valueAccessExp as MemberExpression;
            object values = this.expressionHelper.GetValue(valueMember);

            // get alias and field name
            var fieldName = this.expressionHelper.GetName(memberAccessExp);

            // setup arguments
            var typeArgs = new[] { memberAccessExp.Type };

            var inputGenericType = typeof(List<>);
            var inputType = inputGenericType.MakeGenericType(typeArgs);
            var input = Activator.CreateInstance(inputType, new object[] { values });
            var target = context.GetQueryTarget(memberAccessExp);
            var col = state.Column.Create(fieldName, target);

            var parameters = new object[] { col, input };
            var enumerableGenericType = typeof(IEnumerable<>);
            var enumerableType = enumerableGenericType.MakeGenericType(typeArgs);

            // get generic type
            var toCreate = typeof(InValues<>);
            var genericToCreate = toCreate.MakeGenericType(typeArgs);
            var parameterTypes = new[] { col.GetType(), enumerableType };
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            var ctor = genericToCreate.GetConstructor(bindingFlags, null, parameterTypes, null);

            if (ctor == null) return null;

            // return instance
            return (state.IWhere)ctor.Invoke(parameters);
        }

        public static class SupportedMethods
        {
            public const string DatePart = "DatePart";
            public const string ToLower = "ToLower";
            public const string ToUpper = "ToUpper";
            public const string Contains = "Contains";
            public const string Substring = "Substring";
        }
    }
}
