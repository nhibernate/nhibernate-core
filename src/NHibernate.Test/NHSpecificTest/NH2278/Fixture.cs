using System;
using System.Collections;

using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2278
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using( ISession s = sessions.OpenSession() )
			{
				s.Delete( "from CustomA" );
				s.Flush();
			}
		}

		[Test]
		public void CustomIdBag()
		{
			CustomA a = new CustomA();
			a.Name = "first generic type";
			a.Items = new CustomList<string>();
			a.Items.Add( "first string" );
			a.Items.Add( "second string" );

			ISession s = OpenSession();
			s.SaveOrUpdate(a);
			s.Flush();
			s.Close();

			Assert.That(a.Id, Is.Not.Null);
			Assert.That(a.Items[0], Is.StringMatching("first string"));

			s = OpenSession();
			a = s.Load<CustomA>(a.Id);

			Assert.That(a.Items, Is.InstanceOf<CustomPersistentIdentifierBag<string>>());

			Assert.That(a.Items[0], Is.StringMatching("first string"), "first item should be 'first string'");
			Assert.That(a.Items[1], Is.StringMatching("second string"), "second item should be 'second string'");

			// ensuring the correct generic type was constructed
			a.Items.Add("third string");
			Assert.That(a.Items.Count, Is.EqualTo(3), "3 items in the list now");

			a.Items[1] = "new second string";
			s.Flush();
			s.Close();
		}
	}
}