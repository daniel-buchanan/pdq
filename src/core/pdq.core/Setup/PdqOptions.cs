﻿using pdq.core.common.Logging;

namespace pdq.core
{
	public class PdqOptions
	{
		public LogLevel DefaultLogLevel { get; private set; } = LogLevel.Error;

		public void OverrideDefaultLogLevel(LogLevel level)
        {
			DefaultLogLevel = level;
        }
	}
}

