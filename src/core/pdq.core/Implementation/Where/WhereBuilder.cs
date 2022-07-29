using System;
using System.Collections.Generic;
using System.Linq;
using pdq.common;
using pdq.Exceptions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("pdq.core-tests")]
namespace pdq.Implementation
{
    public class WhereBuilder : IWhereBuilder
	{
        private readonly List<state.IWhere> clauses;
        private readonly IQueryContext queryContext;
        private readonly IClauseHandlingBehaviour clauseHandlingBehaviour;
        private ClauseHandling defaultClauseHandling;

		private WhereBuilder(ClauseHandling clauseHandling, IQueryContext queryContext)
		{
            this.clauseHandlingBehaviour = ClauseHandlingBehaviour.CreateClauseHandler(this);
            this.defaultClauseHandling = clauseHandling;
            this.queryContext = queryContext;
            this.clauses = new List<state.IWhere>();
		}

        internal static IWhereBuilder Create(PdqOptions options, IQueryContext queryContext)
            => new WhereBuilder(options.DefaultClauseHandling, queryContext);

        private static IWhereBuilder Create(ClauseHandling clauseHandling, IQueryContext queryContext)
            => new WhereBuilder(clauseHandling, queryContext);

        /// <inheritdoc />
        public IWhereBuilder And(Action<IWhereBuilder> builder)
        {
            var b = Create(this.defaultClauseHandling, this.queryContext);
            b.ClauseHandling.DefaultToAnd();
            builder(b);

            this.clauses.Add(b.GetClauses().First());
            return this;
        }

        /// <inheritdoc />
        public IClauseHandlingBehaviour ClauseHandling => this.clauseHandlingBehaviour;

        ClauseHandling IWhereBuilder.DefaultClauseHandling => this.defaultClauseHandling;

        /// <inheritdoc />
        public IColumnWhereBuilder Column(string name, string targetAlias = null)
        {
            var target = string.IsNullOrWhiteSpace(targetAlias) ?
                null :
                this.queryContext.QueryTargets.FirstOrDefault(t => t.Alias == targetAlias);
            var c = state.Column.Create(name, target);
            return ColumnWhereBuilder.Create(this, c);
        }

        /// <inheritdoc />
        public IWhereBuilder Or(Action<IWhereBuilder> builder)
        {
            var b = Create(this.defaultClauseHandling, this.queryContext);
            b.ClauseHandling.DefaultToOr();
            builder(b);

            this.clauses.Add(b.GetClauses().First());
            return this;
        }

        /// <inheritdoc />
        void IWhereBuilder.AddClause(state.IWhere item) => this.clauses.Add(item);

        /// <inheritdoc />
        IEnumerable<state.IWhere> IWhereBuilder.GetClauses()
        {
            if (this.defaultClauseHandling == common.ClauseHandling.And)
                return new[] { state.Conditionals.And.Where(this.clauses) };

            if (this.defaultClauseHandling == common.ClauseHandling.Or)
                return new[] { state.Conditionals.Or.Where(this.clauses) };

            throw new WhereBuildFailedException();
        }

        private sealed class ClauseHandlingBehaviour : IClauseHandlingBehaviour
        {
            private readonly WhereBuilder builder;

            private ClauseHandlingBehaviour(WhereBuilder builder)
                => this.builder = builder;

            public static IClauseHandlingBehaviour CreateClauseHandler(WhereBuilder builder)
                => new ClauseHandlingBehaviour(builder);

            /// <inheritdoc />
            public void DefaultToAnd()
                => this.builder.defaultClauseHandling = common.ClauseHandling.And;

            /// <inheritdoc />
            public void DefaultToOr()
                => this.builder.defaultClauseHandling = common.ClauseHandling.Or;
        }
    }
}

