using System.Collections.Generic;
using System.Threading.Tasks;
using pdq.common;
using Dapper;
using System.Linq;
using pdq.common.Utilities;

namespace pdq.Implementation
{
    internal class Execute : ExecuteBase, IExecute
	{
		protected Execute(IQueryInternal query) : base(query) { }

        /// <inheritdoc/>
        public IExecuteDynamic Dynamic() => ExecuteDynamic.Create(this.query);

        /// <inheritdoc/>
        public IEnumerable<T> AsEnumerable<T>() => AsEnumerableAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> AsEnumerableAsync<T>()
            => await ExecuteAsync((s, p, t) => GetConnection().QueryAsync<T>(s, p, t));

        /// <inheritdoc/>
        public T First<T>() => FirstAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<T> FirstAsync<T>()
            => await ExecuteAsync((s, p, t) => GetConnection().QueryFirstAsync<T>(s, p, t));

        /// <inheritdoc/>
        public T FirstOrDefault<T>() => FirstOrDefaultAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<T> FirstOrDefaultAsync<T>()
            => await ExecuteAsync((s, p, t) => GetConnection().QueryFirstOrDefaultAsync<T>(s, p, t));

        /// <inheritdoc/>
        public T Single<T>() => SingleAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<T> SingleAsync<T>()
            => await ExecuteAsync((s, p, t) => GetConnection().QuerySingleAsync<T>(s, p, t));

        /// <inheritdoc/>
        public T SingleOrDefault<T>() => SingleOrDefaultAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<T> SingleOrDefaultAsync<T>()
            => await ExecuteAsync((s, p, t) => GetConnection().QuerySingleOrDefaultAsync<T>(s, p, t));

        /// <inheritdoc/>
        public IList<T> ToList<T>() => ToListAsync<T>().WaitFor();

        /// <inheritdoc/>
        public async Task<IList<T>> ToListAsync<T>()
        {
            var enumerable = await AsEnumerableAsync<T>();
            return enumerable.ToList();
        }

        /// <inheritdoc/>
        void IExecute.Execute() => ExecuteAsync().WaitFor();

        /// <inheritdoc/>
        public async Task ExecuteAsync()
            => await ExecuteAsync((s, p, t) => GetConnection().ExecuteAsync(s, p, t));

        /// <inheritdoc/>
        public new string GetSql() => base.GetSql();
    }
}

