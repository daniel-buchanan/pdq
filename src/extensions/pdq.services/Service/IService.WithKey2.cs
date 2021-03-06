using System;
namespace pdq.services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    public interface IService<TEntity, TKey1, TKey2> :
		IQuery<TEntity, TKey1, TKey2>,
		ICommand<TEntity, TKey1, TKey2>
		where TEntity: IEntity<TKey1, TKey2>
    {

    }
}

