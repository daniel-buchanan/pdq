﻿using System;
using System.Collections.Generic;
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

        public state.IWhere ParseExpression(Expression expr, IQueryContextInternal context)
        {
            MethodCallExpression call;
            if(expr.NodeType == ExpressionType.Lambda)
            {
                var lambda = expr as LambdaExpression;
                call = lambda.Body as MethodCallExpression;
            }
            else
            {
                call = expr as MethodCallExpression;
            }

            var arg = call.Arguments[0];
            var nodeType = arg.NodeType;

            // check for contains
            if (call.Method.Name == "Contains" && nodeType == ExpressionType.MemberAccess)
                return ParseContainsMemberAccessCall(call, context);
            if (call.Method.Name == "Contains" && nodeType == ExpressionType.Constant)
                return ParseContainsConstantCall(call, context);
            
            // otherwise return null
            return null;
        }

        public state.IWhere ParseBinaryExpression(BinaryExpression expr, IQueryContextInternal context)
        {
            var left = expr.Left;
            var right = expr.Right;
            var nodeType = expr.NodeType;

            MethodCallExpression callExpr = null;
            Expression nonCallExpr = null;

            if (left.NodeType == ExpressionType.Call)
            {
                callExpr = (MethodCallExpression)left;
                nonCallExpr = right;
            }
            else if (right.NodeType == ExpressionType.Call)
            {
                callExpr = (MethodCallExpression)right;
                nonCallExpr = left;
            }

            if (callExpr == null) return null;

            if (callExpr.Method.Name == "DatePart") return ParseDatePartCall(nodeType, callExpr, nonCallExpr, context);
            if (callExpr.Method.Name == "ToLower") return ParseCaseCall(nodeType, callExpr, nonCallExpr, context);
            if (callExpr.Method.Name == "ToUpper") return ParseCaseCall(nodeType, callExpr, nonCallExpr, context);
            if (callExpr.Method.Name == "Substring") return ParseSubStringCall(nodeType, callExpr, nonCallExpr, context);
            if (callExpr.Method.Name == "Contains") return ParseContains(callExpr, nonCallExpr, context);

            return null;
        }

        private state.IWhere ParseContains(
            MethodCallExpression callExpr,
            Expression nonCallExpr,
            IQueryContextInternal context)
        {
            var arg = callExpr.Arguments[0];
            var body = callExpr.Object;

            var memberAccessExp = body as MemberExpression;

            // get alias and field name
            var fieldName = this.expressionHelper.GetName(memberAccessExp);
            var value = this.expressionHelper.GetValue(arg) as string;

            if (nonCallExpr.NodeType != ExpressionType.Constant) return null;

            var constValue = (bool)this.expressionHelper.GetValue(nonCallExpr);
            var op = constValue ? EqualityOperator.Like : EqualityOperator.NotLike;

            var target = context.GetQueryTarget(memberAccessExp);
            var col = state.Column.Create(fieldName, target);

            if (value == null && op == EqualityOperator.NotLike)
                return Not.This(Conditionals.Column.Like<string>(col, null, StringContains.Create()));
            if (value == null)
                return Conditionals.Column.Like<string>(col, null, StringContains.Create());
            if (op == EqualityOperator.Like)
                return Conditionals.Column.Like(col, value, StringContains.Create());
            else return Not.This(Conditionals.Column.Like(col, value, StringContains.Create()));
        }

        private state.IWhere ParseDatePartCall(
            ExpressionType nodeType,
            MethodCallExpression callExpr,
            Expression nonCallExpr,
            IQueryContextInternal context)
        {
            var arguments = callExpr.Arguments;
            var objectExpression = arguments[0];
            var datePartExpression = arguments[1];
            var op = this.expressionHelper.ConvertExpressionTypeToEqualityOperator(nodeType);

            var dpField = this.expressionHelper.GetName(objectExpression);
            var dp = (common.DatePart)this.expressionHelper.GetValue(datePartExpression);

            var leftTarget = context.GetQueryTarget(objectExpression);
            var col = state.Column.Create(dpField, leftTarget);

            if (nonCallExpr.NodeType == ExpressionType.Constant)
            {
                var value = (int)this.expressionHelper.GetValue(nonCallExpr);
                return Conditionals.Column.Equals(col, op, value, state.Conditionals.ValueFunctions.DatePart.Create(dp));
            }

            if (nonCallExpr.NodeType == ExpressionType.MemberAccess)
            {
                var member = (MemberExpression) nonCallExpr;
                if (member.Expression.NodeType == ExpressionType.Constant)
                {
                    var value = (int)this.expressionHelper.GetValue(member);
                    return Conditionals.Column.Equals(col, op, value, state.Conditionals.ValueFunctions.DatePart.Create(dp));
                }

                var mField = this.expressionHelper.GetName(nonCallExpr);
                var target = context.GetQueryTarget(nonCallExpr);
                col = state.Column.Create(mField, target);

                return Conditionals.Column.Equals(col, op, 0, state.Conditionals.ValueFunctions.DatePart.Create(dp));
            }

            return null;
        }

        private ValueFunction ConvertMethodNameToValueFunction(string method)
        {
            if (method == "ToLower") return ValueFunction.ToLower;
            if (method == "ToUpper") return ValueFunction.ToUpper;
            if (method == "DatePart") return ValueFunction.DatePart;
            if (method == "Substring") return ValueFunction.Substring;

            return ValueFunction.None;
        }

        private state.IValueFunction ConvertMethodNameToValueFunctionImpl(ValueFunction function)
        {
            switch(function)
            {
                case ValueFunction.ToLower: return ToLower.Create();
                case ValueFunction.ToUpper: return ToUpper.Create();
                default: return null;
            }
        }

        private state.IWhere ParseCaseCall(
            ExpressionType nodeType,
            MethodCallExpression callExpr,
            Expression nonCallExpr,
            IQueryContextInternal context)
        {
            var body = callExpr.Object;
            var field = this.expressionHelper.GetName(body);
            var op = this.expressionHelper.ConvertExpressionTypeToEqualityOperator(nodeType);
            var leftTarget = context.GetQueryTarget(body);
            var col = state.Column.Create(field, leftTarget);

            var func = ConvertMethodNameToValueFunction(callExpr.Method.Name);

            if (func == ValueFunction.None) return null;
            var funcImplementation = ConvertMethodNameToValueFunctionImpl(func);


            if (nonCallExpr.NodeType == ExpressionType.Constant)
            {
                var value = this.expressionHelper.GetValue(nonCallExpr);
                var functionType = typeof(Conditionals.Column<>);
                var implementedType = functionType.MakeGenericType(value.GetType());
                return (state.IWhere)Activator.CreateInstance(implementedType, new object[] { col, op, value, funcImplementation });
            }

            if (nonCallExpr.NodeType == ExpressionType.MemberAccess)
            {
                var fieldB = this.expressionHelper.GetName(nonCallExpr);
                var target = context.GetQueryTarget(nonCallExpr);
                var right = state.Column.Create(fieldB, target);
                return Conditionals.Column.Equals(col, op, funcImplementation, right);
            }

            if (nonCallExpr.NodeType == ExpressionType.Call)
            {
                var rightCallExpr = (MethodCallExpression)nonCallExpr;
                var rightBody = rightCallExpr.Object;
                var fieldB = this.expressionHelper.GetName(rightBody);
                var methodB = ConvertMethodNameToValueFunction(rightCallExpr.Method.Name);
                if (methodB == ValueFunction.None) return null;

                var target = context.GetQueryTarget(rightBody);
                var right = state.Column.Create(fieldB, target);
                funcImplementation = ConvertMethodNameToValueFunctionImpl(methodB);

                return Conditionals.Column.Equals(col, op, funcImplementation, right);
            }

            return null;
        }

        private state.IWhere ParseSubStringCall(
            ExpressionType nodeType,
            MethodCallExpression callExpr,
            Expression nonCallExpr,
            IQueryContextInternal context)
        {
            var arguments = callExpr.Arguments;
            var startExpression = arguments[0];
            Expression lengthExpression = null;
            if (arguments.Count > 1) lengthExpression = arguments[1];

            var op = this.expressionHelper.ConvertExpressionTypeToEqualityOperator(nodeType);
            var field = this.expressionHelper.GetName(callExpr);
            var target = context.GetQueryTarget(callExpr);
            var col = state.Column.Create(field, target);

            var startValue = (int)this.expressionHelper.GetValue(startExpression);

            if (nonCallExpr.NodeType == ExpressionType.Constant)
            {
                var value = (string) this.expressionHelper.GetValue(nonCallExpr);

                if (lengthExpression != null)
                {
                    var lengthValue = (int) this.expressionHelper.GetValue(lengthExpression);
                    return Conditionals.Column.Equals(col, op, value, Substring.Create(startValue, lengthValue));
                }

                return Conditionals.Column.Equals(col, op, value, Substring.Create(startValue));
            }

            return null;
        }

        private state.IWhere ParseContainsMemberAccessCall(MethodCallExpression call, IQueryContextInternal context)
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
                var firstArgument = call.Arguments[0] as MemberExpression;
                if (firstArgument != null &&
                    firstArgument.Expression.NodeType == ExpressionType.Constant)
                {
                    // check if underlying value is a constant
                    return ParseContainsConstantCall(call, context);
                }

                // otherwise default to member access
                memberAccessExp = firstArgument;
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

        private state.IWhere ParseContainsConstantCall(MethodCallExpression call, IQueryContextInternal context)
        {
            var arg = call.Arguments[0];
            var body = call.Object;

            var memberAccessExp = body as MemberExpression;

            // get alias and field name
            var target = context.GetQueryTarget(memberAccessExp);
            var fieldName = this.expressionHelper.GetName(memberAccessExp);
            var value = this.expressionHelper.GetValue(arg);
            var col = state.Column.Create(fieldName, target);

            if (value == null)
                return Conditionals.Column.Like<string>(col, null, StringContains.Create());

            return Conditionals.Column.Like<string>(col, (string)value, StringContains.Create());
        }
    }
}
