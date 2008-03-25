using System;
using System.Collections;

using NUnit.Framework;

namespace Iesi.Collections.Test
{
	/// <summary>
	/// Summary description for ListSetFixture.
	/// </summary>
	[TestFixture]
	public class ListSetFixture : SetFixture
	{
		protected override ISet CreateInstance()
		{
			return new ListSet();
		}

		protected override ISet CreateInstance(ICollection init)
		{
			return new ListSet(init);
		}

		protected override Type ExpectedType
		{
			get { return typeof(ListSet); }
		}
	}
}