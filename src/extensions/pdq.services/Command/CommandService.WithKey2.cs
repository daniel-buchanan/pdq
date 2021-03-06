using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using pdq.common;
using Dapper.Contrib.Extensions;

namespace pdq.services
{
    public class Command<TEntity, TKey1, TKey2> :
        ServiceBase,
        ICommand<TEntity, TKey1, TKey2>
        where TEntity : class, IEntity<TKey1, TKey2>
    {
        public Command(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        private Command(ITransient transient) : base(transient) { }

        public static ICommand<TEntity, TKey1, TKey2> Create(ITransient transient)
            => new Command<TEntity, TKey1, TKey2>(transient);

        public TEntity Add(TEntity toAdd)
        {
            throw new NotImplementedException();
        }

        public void Delete(TKey1 key1, TKey2 key2)
        {
            throw new NotImplementedException();
        }

        public void Delete(params ICompositeKeyValue<TKey1, TKey2>[] keys)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<ICompositeKeyValue<TKey1, TKey2>> keys)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void Update(dynamic toUpdate, TKey1 key1, TKey2 key2)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity toUpdate, Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void Update(dynamic toUpdate, Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}

