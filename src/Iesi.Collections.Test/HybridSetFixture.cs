using System;
using System.Collections;

using Iesi.Collections;

using NUnit.Framework;

namespace Iesi.Collections.Test
{
	/// <summary>
	/// Summary description for HybridSetFixture.
	/// </summary>
	[TestFixture]
	public class HybridSetFixture : SetFixture
	{

		protected override ISet CreateInstance()
		{
			return new HybridSet();
		}

		protected override ISet CreateInstance(ICollection init)
		{
			return new HybridSet( init );
		}

		protected override Type ExpectedType
		{
			get { return typeof(HybridSet); }
		}

	}
}
