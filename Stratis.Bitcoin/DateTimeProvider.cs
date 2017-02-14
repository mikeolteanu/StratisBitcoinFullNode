﻿using System;
using NBitcoin;

namespace Stratis.Bitcoin
{
	public class DateTimeProvider
	{
		public virtual long GetTime()
		{
			return DateTime.UtcNow.ToUnixTimestamp();
		}

		public virtual DateTimeOffset GetTimeOffset()
		{
			return DateTimeOffset.UtcNow;
		}

		public static DateTimeProvider Default => new DateTimeProvider();
	}
}