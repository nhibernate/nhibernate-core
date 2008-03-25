using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Subclass
{
	/// <summary>
	/// Test the use of <c>&lt;class&gt;</c> and <c>&lt;subclass&gt;</c> mappings
	/// that are in different files through the use of the <c>extends</c> attribute.
	/// </summary>
	/// <remarks>
	/// Inheriting from <see cref="SubclassFixture"/> because the only thing different
	/// is how the classes are mapped.
	/// </remarks>
	[TestFixture]
	public class SubclassExtendsFixture : SubclassFixture
	{
		protected override IList Mappings
		{
			get
			{
				// order is important!  The base classes must be configured before
				// the subclasses.
				return new string[]
					{
						"Subclass.Subclass.Base.hbm.xml",
						"Subclass.Subclass.One.hbm.xml"
					};
			}
		}
	}
}