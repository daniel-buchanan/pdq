﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace pdq.services
{
    /// <summary>
    /// A Service for making modifications to a given <see cref="TEntity"/>. 
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity"/> to work with.</typeparam>
	public interface ICommand<TEntity> where TEntity : IEntity, new()
	{
		/// <summary>
        /// Add an item to the database.
        /// </summary>
        /// <param name="toAdd">The <see cref="TEntity"/> to be added.</param>
        /// <returns>The updated <see cref="TEntity"/> which has been added.</returns>
		TEntity Add(TEntity toAdd);

		/// <summary>
        /// Update the provided item.
        /// </summary>
        /// <param name="toUpdate">The details to update.</param>
        /// <param name="expression">An expression specifying which item(s) to update.</param>
		void Update(TEntity toUpdate, Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Update the provided item.
        /// </summary>
        /// <param name="toUpdate">The details to update.</param>
        /// <param name="expression">An expression specifying which item(s) to update.</param>
        void Update(dynamic toUpdate, Expression<Func<TEntity, bool>> expression);

		/// <summary>
        /// Delete one or more items.
        /// </summary>
        /// <param name="expression">An expression specifying which item(s) should be deleted.</param>
		void Delete(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Event fired before the query is executed.
        /// </summary>
        event EventHandler<PreExecutionEventArgs> PreExecution;
    }
}

