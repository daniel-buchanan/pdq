using System;
using System.Linq;
using System.Linq.Expressions;
using pdq.common;
using pdq.Exceptions;
using pdq.state;

namespace pdq.Implementation
{
	internal abstract class SelectBase : Execute
	{
        protected readonly PdqOptions options;
        protected readonly ISelectQueryContext context;

        protected SelectBase(
            ISelectQueryContext context,
            IQuery query)
            : base((IQueryInternal)query)
        {
            this.options = (query as IQueryInternal).Options;
            this.context = context;
            this.query.SetContext(this.context);
        }

        protected void AddColumns(Expression expression)
        {
            var properties = this.context.Helpers().GetPropertyInformation(expression);
            foreach (var p in properties)
            {
                var target = p.Type is object ?
                    this.context.QueryTargets.FirstOrDefault(t => t.Alias == p.Alias) :
                    GetQueryTarget(p.Type);
                this.context.Select(state.Column.Create(p.Name, target, p.NewName));
            }
        }

        protected IQueryTarget GetQueryTarget(Type type)
        {
            var table = this.context.Helpers().GetTableName(type);
            return GetQueryTarget(table);
        }

        protected IQueryTarget GetQueryTarget(Expression expression)
        {
            var table = this.context.Helpers().GetTableName(expression);
            return GetQueryTarget(table);
        }

        protected IQueryTarget GetQueryTarget(string table)
        {
            var alias = this.query.AliasManager.FindByAssociation(table).FirstOrDefault();
            if (alias == null) throw new TableNotFoundException(alias.Name, table);

            var target = this.context.QueryTargets.FirstOrDefault(t => t.Alias == alias.Name);
            if (target == null) throw new TableNotFoundException(alias.Name, table);
            return target;
        }
    }
}

