using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace pdq.services
{
    /// <summary>
    /// A Service for making modifications to a given <see cref="TEntity"/>. 
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity{TKey}"/> to work with.</typeparam>
    /// <typeparam name="TKey">The data type of the primary key.</typeparam>
	public interface ICommand<TEntity, TKey> :
        ICommand<TEntity>
		where TEntity: IEntity<TKey>
    {

        /// <summary>
        /// Update only select fields for a specific item.
        /// </summary>
        /// <param name="toUpdate">The fields/columns to update.</param>
        /// <param name="key">The <see cref="TKey"/> of the item to update.</param>
		void Update(dynamic toUpdate, TKey key);

        /// <summary>
        /// Delete a specific item.
        /// </summary>
        /// <param name="key">The <see cref="TKey"/> of the item to delete.</param>
		void Delete(TKey key);

        /// <summary>
        /// Delete a number of specified items.
        /// </summary>
        /// <param name="keys">The set of <see cref="TKey"/>s to delete. </param>
		void Delete(params TKey[] keys);

        /// <summary>
        /// Delete a number of specified items.
        /// </summary>
        /// <param name="keys">The set of <see cref="TKey"/>s to delete. </param>
		void Delete(IEnumerable<TKey> keys);
    }
}

