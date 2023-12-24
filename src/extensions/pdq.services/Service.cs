﻿using System;
using System.Threading;
using System.Threading.Tasks;
using pdq.common;
using pdq.common.Connections;
using pdq.common.Utilities;
using pdq.state;

namespace pdq.services
{
    internal abstract class Service
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPdq pdq;
        protected readonly bool disposeOnExit;

        protected Service(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.disposeOnExit = false;
        }

        protected Service(IPdq pdq)
        {
            this.pdq = pdq;
            this.disposeOnExit = true;
        }

        protected event EventHandler<PreExecutionEventArgs> PreExecution;

        /// <summary>
        /// Get the Transient required to begin a query.
        /// </summary>
        /// <returns>Returns a <see cref="IUnitOfWork"/>.</returns>
        internal IUnitOfWork GetTransient() => this.unitOfWork ?? this.pdq.Begin();

        /// <summary>
        /// Execute the provided query.<br/>
        /// Ensuring that everything is disposed properly.
        /// </summary>
        /// <param name="method">The <see cref="Action{IQuery}"/> defining the query.</param>
        protected void ExecuteQuery(Action<IQueryContainer> method)
            => ExecuteQueryAsync((q, c) => { method(q); return Task.CompletedTask; }).WaitFor();

        /// <summary>
        /// Execute the provided query.<br/>
        /// Ensuring that everything is disposed properly and the results of the query are returned.
        /// </summary>
        /// <param name="method">The <see cref="Action{IQuery, T}"/> defining the query.</param>
        protected T ExecuteQuery<T>(Func<IQueryContainer, T> method)
            => ExecuteQueryAsync((q, c) => Task.FromResult(method(q))).WaitFor();

        protected async Task ExecuteQueryAsync(Func<IQueryContainer, CancellationToken, Task> method, CancellationToken cancellationToken = default)
        {
            var t = this.GetTransient();
            using (var q = await t.QueryAsync(cancellationToken))
            {
                await method(q, cancellationToken);
            }

            if (this.disposeOnExit) t.Dispose();
        }
        
        protected async Task<T> ExecuteQueryAsync<T>(Func<IQueryContainer, CancellationToken, Task<T>> method, CancellationToken cancellationToken = default)
        {
            T result;
            var t = this.GetTransient();
            using (var q = await t.QueryAsync(cancellationToken))
            {
                result = await method(q, cancellationToken);
            }

            if (this.disposeOnExit) t.Dispose();

            return result;
        }

        /// <summary>
        /// Event to notify interested methods about pre-execution query state.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="query">The <see cref="IQuery"/> containing the context to send.</param>
        protected void NotifyPreExecution(object sender, IQueryContainer query)
        {
            var args = new PreExecutionEventArgs(query.Context);
            this.PreExecution?.Invoke(sender, args);
        }

        /// <summary>
        /// Get the name of the table for this query,
        /// </summary>
        /// <typeparam name="TEntity">The type of Entity to work with.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> to use.</param>
        /// <returns>The name of the table for this <see cref="IQueryContext"/>.</returns>
        protected string GetTableInfo<TEntity>(IQueryContainer q)
            where TEntity : new()
            => q.Context.Helpers().GetTableName<TEntity>();

        /// <summary>
        /// Get an individual key column name.
        /// </summary>
        /// <typeparam name="TEntity">The Entity type to work with.</typeparam>
        /// <param name="q">The <see cref="IQuery"/> instance to use.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The SQL name of the Column.</returns>
        protected static string GetKeyColumnName<TEntity>(IQueryContainer q, string name)
        {
            var prop = typeof(TEntity).GetProperty(name);
            return q.Context.Helpers().GetFieldName(prop);
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
    }
}

