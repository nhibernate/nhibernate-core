﻿using System;

namespace NHibernate.Test.NHSpecificTest.GH0829
{
	[Flags]
	public enum TestEnum
	{
		A = 1 << 0,
		B = 1 << 1,
		C = 1 << 2,
		D = 1 << 3
	}
}
