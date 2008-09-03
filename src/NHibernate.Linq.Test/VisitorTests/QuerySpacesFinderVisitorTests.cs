using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq.Test.Model;
using NHibernate.Linq.Visitors;
using NUnit.Framework;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Linq.Test.VisitorTests
{
	[TestFixture]
	public class QuerySpacesFinderVisitorTests:BaseTest
	{

		public override void Setup()
		{
			base.Setup();
			visitor = new QuerySpacesFinderVisitor(session.SessionFactory as ISessionFactoryImplementor);
		}
		private QuerySpacesFinderVisitor visitor;


		[NUnit.Framework.Test]
		public void CanFindRoot()
		{

			var results = from x in session.Linq<Animal>()
						  select x;

			visitor.Visit(results.Expression);
			Assert.Contains(typeof (Animal).FullName, (ICollection)visitor.QuerySpaces);
		}


		[NUnit.Framework.Test]
		public void CanFindProperty()
		{

			var results = from x in session.Linq<Animal>()
						  where x.Zoo.Name=="My Zoo"
						  select x;

			visitor.Visit(results.Expression);
			Assert.Contains(typeof(Animal).FullName, (ICollection)visitor.QuerySpaces);
			Assert.Contains(typeof(Zoo).FullName, (ICollection)visitor.QuerySpaces);
			Assert.That(!visitor.QuerySpaces.Contains(typeof(string).FullName));
		}


		[NUnit.Framework.Test]
		public void CanFindNestedProperty()
		{

			var results = from x in session.Linq<Human>()
						  where x.Name.First=="Osman" && x.Pets.Count>0
						  select x;
			visitor.Visit(results.Expression);
			Assert.Contains(typeof(Human).FullName, (ICollection)visitor.QuerySpaces);
			Assert.Contains(typeof(DomesticAnimal).FullName, (ICollection)visitor.QuerySpaces);
		}
	}
}
