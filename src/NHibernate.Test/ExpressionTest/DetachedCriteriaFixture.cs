using System.Collections;
using NHibernate.DomainModel;
using NHibernate.Expression;
using NExpression = NHibernate.Expression.Expression;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	[TestFixture]
	public class DetachedCriteriaFixture : TestCase
	{

		protected override IList Mappings
		{
			get
			{
				return new string[] { "Componentizable.hbm.xml" };
			}
		}
		
		[Test]
		public void CanUseDetachedCriteriaToQuery()
		{
			using (ISession s = OpenSession())
			{
				Componentizable master = new Componentizable();
				master.NickName = "master";
				s.Save(master);
				s.Flush();
			}

			DetachedCriteria detachedCriteria = DetachedCriteria.For(typeof(Componentizable));
			detachedCriteria.Add(NExpression.Eq("NickName","master"));

			using (ISession s = OpenSession())
			{
				Componentizable componentizable = (Componentizable)detachedCriteria.GetExecutableCriteria(s).UniqueResult();
				Assert.AreEqual("master", componentizable.NickName);
				s.Delete(componentizable);
				s.Flush();
			}
		}
	}
}
