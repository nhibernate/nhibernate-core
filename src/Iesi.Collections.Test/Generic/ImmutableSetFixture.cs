using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NUnit.Framework;

namespace Iesi.Collections.Test.Generic
{
	/// <summary>
	/// Summary description for HashedSetFixture.
	/// </summary>
	[TestFixture]
	public class ImmutableSetFixture : GenericSetFixture
	{
		protected override ISet<string> CreateInstance()
		{
			return new ImmutableSet<string>(new HashedSet<string>());
		}

		protected override ISet<string> CreateInstance(ICollection<string> init)
		{
			return new ImmutableSet<string>(new HashedSet<string>(init));
		}

		protected override Type ExpectedType
		{
			get { return typeof(ImmutableSet<string>); }
		}
	}
}