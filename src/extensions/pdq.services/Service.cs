﻿using System;
using pdq.common;
using pdq.common.Connections;
using pdq.state;
using pdq.state.Utilities;

namespace pdq.services
{
    internal abstract class Service
    {
        private readonly ITransientInternal transient;
        private readonly IUnitOfWork unitOfWork;
        protected readonly bool disposeOnExit;

        protected readonly IReflectionHelper reflectionHelper;
        protected readonly IExpressionHelper expressionHelper;
        protected readonly IDynamicExpressionHelper dynamicExpressionHelper;

        protected Service()
        {
            reflectionHelper = new ReflectionHelper();
            expressionHelper = new ExpressionHelper(reflectionHelper);
            dynamicExpressionHelper = new DynamicExpressionHelper(expressionHelper, new CallExpressionHelper(expressionHelper));
        }

        protected Service(ITransient transient) : this()
        {
            this.transient = transient as ITransientInternal;
            this.disposeOnExit = false;
        }

        protected Service(IUnitOfWork unitOfWork) : this()
        {
            this.unitOfWork = unitOfWork;
            this.disposeOnExit = true;
        }

        protected event EventHandler<PreExecutionEventArgs> preExecution;

        /// <summary>
        /// Get the Transient required to begin a query.
        /// </summary>
        /// <returns>Retunds a <see cref="ITransientInternal"/>.</returns>
        internal ITransientInternal GetTransient()
        {
            if (this.transient != null) return this.transient;

            return this.unitOfWork.Begin() as ITransientInternal;
        }

        /// <summary>
        /// Execute the provided query.<br/>
        /// Ensuring that everything is disposed properly.
        /// </summary>
        /// <param name="method">The <see cref="Action{IQuery}"/> defining the query.</param>
        protected void ExecuteQuery(Action<IQueryContainer> method)
        {
            var t = this.GetTransient();
            using(var q = t.Query())
            {
                method(q);
            }

            if (this.disposeOnExit) t.Dispose();
        }

        /// <summary>
        /// Execute the provided query.<br/>
        /// Ensuring that everything is disposed properly and the results of the query are returned.
        /// </summary>
        /// <param name="method">The <see cref="Action{IQuery, T}"/> defining the query.</param>
        protected T ExecuteQuery<T>(Func<IQueryContainer, T> method)
        {
            T result;
            var t = this.GetTransient();
            using (var q = t.Query())
            {
                result = method(q);
            }

            if (this.disposeOnExit) t.Dispose();

            return result;
        }

        /// <summary>
        /// Event to notify interested methods about pre-executeion query state.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="query">The <see cref="IQuery"/> containing the context to send.</param>
        protected void NotifyPreExecution(object sender, IQueryContainer query)
        {
            var internalQuery = query as IQueryContainerInternal;
            var args = new PreExecutionEventArgs(internalQuery.Context);
            this.preExecution?.Invoke(sender, args);
        }

        /// <summary>
        /// Geet the name of the table for this query,
        /// </summary>
        /// <typeparam name="TEntity">The type of Entity to work with.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> to use.</param>
        /// <returns>The name of the table for this <see cref="IQueryContext"/>.</returns>
        protected string GetTableInfo<TEntity>(IQueryContainer q)
            where TEntity : new()
        {
            var internalQuery = q as IQueryContainerInternal;
            var internalContext = internalQuery?.Context as IQueryContextInternal;
            return internalContext?.Helpers().GetTableName<TEntity>();
        }

        /// <summary>
        /// Get an individual key column name.
        /// </summary>
        /// <typeparam name="TEntity">The Entity type to work with.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> instance to use.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The SQL name of the Column.</returns>
        protected static string GetKeyColumnName<TEntity>(IQueryContainer q, string name)
        {
            var internalQuery = q as IQueryContainerInternal;
            var internalContext = internalQuery?.Context as IQueryContextInternal;
            var prop = typeof(TEntity).GetProperty(name);
            return internalContext.ReflectionHelper.GetFieldName(prop);
        }

        /// <summary>
        /// Get an individual key column name.
        /// </summary>
        /// <typeparam name="TEntity">The Entity type to work with.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> instance to use.</param>
        /// <param name="key">The key metadata</param>
        /// <returns>The SQL name of the Column.</returns>
        protected static string GetKeyColumnName<TEntity>(IQueryContainer q, IKeyMetadata key)
            => GetKeyColumnName<TEntity>(q, key.Name);

        /// <summary>
        /// Get the column names for an Entity.<br/>
        /// Where it has a single key.
        /// </summary>
        /// <typeparam name="TEntity">The Entity type to work with.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> instance to work with.</param>
        /// <param name="keyName">The name of the Key.</param>
        protected void GetKeyColumnNames<TEntity, TKey>(IQueryContainer q, out string keyName)
            where TEntity : IEntity<TKey>, new()
        {
            var tmp = new TEntity();
            keyName = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata);
        }

        /// <summary>
        /// Get the column names for an Entity.<br/>
        /// Where it has a composite key with two values.
        /// </summary>
        /// <typeparam name="TEntity">The Entity type to work with.</typeparam>
        /// <typeparam name="TKey1">The type of the first component of the key.</typeparam>
        /// <typeparam name="TKey2">The type of the second component of the key.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> instance to work with.</param>
        /// <param name="key1Name">The name of the first Key comoponent.</param>
        /// <param name="key2Name">The name of the second Key compononent.</param>
        protected void GetKeyColumnNames<TEntity, TKey1, TKey2>(IQueryContainer q, out string key1Name, out string key2Name)
            where TEntity : IEntity<TKey1, TKey2>, new()
        {
            var tmp = new TEntity();
            key1Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentOne);
            key2Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentTwo);
        }

        /// <summary>
        /// Get the column names for an Entity.<br/>
        /// Where it has a composite key with two values.
        /// </summary>
        /// <typeparam name="TEntity">The Entity type to work with.</typeparam>
        /// <typeparam name="TKey1">The type of the first component of the key.</typeparam>
        /// <typeparam name="TKey2">The type of the second component of the key.</typeparam>
        /// <typeparam name="TKey3">The type of the third component of the key.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> instance to work with.</param>
        /// <param name="key1Name">The name of the first Key comoponent.</param>
        /// <param name="key2Name">The name of the second Key compononent.</param>
        /// <param name="key3Name">The name of the third Key compononent.</param>
        protected void GetKeyColumnNames<TEntity, TKey1, TKey2, TKey3>(IQueryContainer q, out string key1Name, out string key2Name, out string key3Name)
            where TEntity : IEntity<TKey1, TKey2, TKey3>, new()
        {
            var tmp = new TEntity();
            key1Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentOne);
            key2Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentTwo);
            key3Name = GetKeyColumnName<TEntity>(q, tmp.KeyMetadata.ComponentThree);
        }
    }
}

