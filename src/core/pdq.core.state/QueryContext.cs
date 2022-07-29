using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using pdq.common;
using pdq.state.Utilities;
using pdq.state.Utilities.Parsers;

namespace pdq.state
{
	internal abstract class QueryContext : IQueryContextInternal
	{
		private readonly IExpressionHelper expressionHelper;
		private readonly IReflectionHelper reflectionHelper;
        private readonly IDynamicExpressionHelper dynamicExpressionHelper;
		private readonly IAliasManager aliasManager;
		private readonly ParserHolder parserHolder;
		private readonly List<IQueryTarget> queryTargets;

		protected QueryContext(
			IAliasManager aliasManager,
			QueryTypes kind)
		{
			Id = Guid.NewGuid();
			Kind = kind;
			this.queryTargets = new List<IQueryTarget>();
			this.reflectionHelper = new ReflectionHelper();
			this.expressionHelper = new ExpressionHelper(this.reflectionHelper);
            var callExpressionHelper = new CallExpressionHelper(expressionHelper);
            this.dynamicExpressionHelper = new DynamicExpressionHelper(expressionHelper, callExpressionHelper);
            this.aliasManager = aliasManager;
            this.parserHolder = new ParserHolder(expressionHelper, reflectionHelper, callExpressionHelper);
		}

		/// <inheritdoc/>
        public Guid Id { get; private set; }

		/// <inheritdoc/>
		public QueryTypes Kind { get; private set; }

		/// <inheritdoc/>
		public IReadOnlyCollection<IQueryTarget> QueryTargets => this.queryTargets;

		/// <inheritdoc/>
		IExpressionHelper IQueryContextInternal.ExpressionHelper => this.expressionHelper;

		/// <inheritdoc/>
		IReflectionHelper IQueryContextInternal.ReflectionHelper => this.reflectionHelper;

        /// <inheritdoc/>
        IQueryParsers IQueryContextInternal.Parsers => this.parserHolder;

        /// <inheritdoc/>
        IAliasManager IQueryContextInternal.AliasManager => this.aliasManager;

        IDynamicExpressionHelper IQueryContextInternal.DynamicExpressionHelper => this.dynamicExpressionHelper;

        /// <inheritdoc/>
        void IQueryContextInternal.AddQueryTarget(IQueryTarget target)
			=> this.queryTargets.Add(target);

		/// <inheritdoc/>
        IQueryTarget IQueryContextInternal.GetQueryTarget(Expression expression)
        {
            GetAliasAndTable(expression, out var alias, out var table);
            var managedAlias = this.aliasManager.FindByAssociation(table)?.FirstOrDefault()?.Name ?? alias;
            managedAlias = this.aliasManager.Add(managedAlias, table);

            return this.queryTargets.FirstOrDefault(t => t.Alias == managedAlias);
        }

        /// <inheritdoc/>
        IQueryTarget IQueryContextInternal.GetQueryTarget(string alias)
        {
			return this.queryTargets.FirstOrDefault(qt => qt.Alias == alias);
        }

        /// <inheritdoc/>
        IQueryTarget IQueryContextInternal.AddQueryTarget(Expression expression)
        {
            var self = this as IQueryContextInternal;
            var existing = self.GetQueryTarget(expression);
            if (existing != null) return existing;

            GetAliasAndTable(expression, out var alias, out var table);
            var target = state.QueryTargets.TableTarget.Create(table, alias);
            self.AddQueryTarget(target);
            return target;
        }

        private void GetAliasAndTable(Expression expression, out string alias, out string table)
        {
            alias = table = null;
            Expression exprAlias, exprTable;
            exprAlias = exprTable = expression;

            if (expression is LambdaExpression)
            {
                var lambda = expression as LambdaExpression;
                if(lambda.Parameters.Count > 1) return;
                exprAlias = lambda.Body;
                exprTable = lambda.Body;
            }

            if (exprAlias is MethodCallExpression)
            {
                var call = exprAlias as MethodCallExpression;
                var obj = call.Object as MemberExpression;
                exprAlias = obj;
                exprTable = obj.Expression;
            }

            if(expression is ParameterExpression)
            {
                var paramExpr = expression as ParameterExpression;
                alias = paramExpr.Name;
                table = this.reflectionHelper.GetTableName(paramExpr.Type);
                return;
            }

            alias = this.expressionHelper.GetParameterName(exprAlias);
            table = this.reflectionHelper.GetTableName(exprTable.Type);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.queryTargets.DisposeAll();
        }
    }
}

