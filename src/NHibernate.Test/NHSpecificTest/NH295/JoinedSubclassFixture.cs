using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH295
{
	/// <summary>
	/// Runs the same tests as <see cref="SubclassFixture" />, but uses joined-subclass
	/// mapping instead of subclass.
	/// </summary>
	[TestFixture]
	public class JoinedSubclassFixture : SubclassFixture
	{
		protected override string[] Mappings
		{
			get { return new string[] {"NHSpecificTest.NH295.JoinedSubclass.hbm.xml"}; }
		}
	}
}