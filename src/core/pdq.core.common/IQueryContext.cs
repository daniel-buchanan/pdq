﻿using System;
using System.Collections.Generic;

namespace pdq.common
{
    public interface IQueryContext : IDisposable
	{
		/// <summary>
        /// The Id of the <see cref="IQueryContext"/>.
        /// </summary>
		Guid Id { get; }

		/// <summary>
        /// The kind of query that this <see cref="IQueryContext"/> represents.
        /// </summary>
		QueryType Kind { get; }

		/// <summary>
        /// The targets of the <see cref="IQueryContext"/>.
        /// </summary>
		IEnumerable<IQueryTarget> QueryTargets { get; }
	}
}

