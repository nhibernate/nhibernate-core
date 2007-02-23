using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.JoinedSubclass
{
	/// <summary>
	/// Test the use of <c>&lt;class&gt;</c> and <c>&lt;joined-subclass&gt;</c> mappings
	/// that are in different files through the use of the <c>extends</c> attribute.
	/// </summary>
	/// <remarks>
	/// Inheriting from <see cref="JoinedSubclassFixture"/> because the only thing different
	/// is how the classes are mapped.
	/// </remarks>
	[TestFixture]
	public class JoinedSubclassExtendsFixture : JoinedSubclassFixture
	{
		protected override IList Mappings
		{
			get
			{
				// order is important!  The base classes must be configured before
				// the subclasses.
				return new string[]
					{
						"JoinedSubclass.JoinedSubclass.Person.hbm.xml",
						"JoinedSubclass.JoinedSubclass.Employee.hbm.xml",
						"JoinedSubclass.JoinedSubclass.Customer.hbm.xml"
					};
			}
		}
	}
}