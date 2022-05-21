﻿using System;
namespace pdq.core.common
{
	public interface ITransient : IDisposable
	{
		Guid Id { get; }

		IQuery Query();
	}
}