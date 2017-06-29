using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2043
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
        public class Namer : EmptyInterceptor
        {
            public override string GetEntityName(object entity)
            {
                if (entity.GetType().Name.EndsWith("Impl"))
                    return entity.GetType().BaseType.FullName;

                return base.GetEntityName(entity);
            }
        }

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetInterceptor(new Namer());
		}

        [Test]
		public void Test()
		{
			try
			{
				using (ISession s = OpenSession())
				{
					var a = new AImpl {Id = 1, Name = "A1"};
					var b = new BImpl {Id = 1, Name = "B1", A = a };
				    a.B = b;

					s.Save(a);
					s.Save(b);
					s.Flush();
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				{
					s.Delete("from B");
					s.Delete("from A");
					s.Flush();
				}
			}
		}
	}
}
