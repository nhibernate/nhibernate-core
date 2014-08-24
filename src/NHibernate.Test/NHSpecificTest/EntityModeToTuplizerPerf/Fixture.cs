using System;
using System.Diagnostics;
using NHibernate.Tuple;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.EntityModeToTuplizerPerf
{
	[TestFixture]
	public class Fixture
	{
		private TargetClazz target;

		[SetUp]
		public void Setup()
		{
			target = new TargetClazz();
		}

		[Test]
		public void VerifyEntityModeNotFound()
		{
			Assert.IsNull(target.GetTuplizerOrNull(EntityMode.Xml));
		}

		[Test]
		public void VerifyEntityModeFound()
		{
			ITuplizer tuplizer = new TuplizerStub();
			target.Add(EntityMode.Map, tuplizer);
			Assert.AreSame(tuplizer, target.GetTuplizerOrNull(EntityMode.Map));
		}

		[Test, Explicit("To the commiter - run before and after")]
		public void RemoveThisTest_JustToShowPerfDifference()
		{
			const int loop = 1000000;
			target.Add(EntityMode.Map, new TuplizerStub());
			target.Add(EntityMode.Poco, new TuplizerStub());
			target.Add(EntityMode.Xml, new TuplizerStub());

			var watch = new Stopwatch();
			watch.Start();
			for (int i = 0; i < loop; i++)
			{
				target.GetTuplizerOrNull(EntityMode.Map);
				target.GetTuplizerOrNull(EntityMode.Poco);
			}
			watch.Stop();
			Console.WriteLine(watch.ElapsedMilliseconds);
		}

		private class TargetClazz : EntityModeToTuplizerMapping
		{
			public void Add(EntityMode eMode, ITuplizer tuplizer)
			{
				AddTuplizer(eMode, tuplizer);
			}
		}

		private class TuplizerStub : ITuplizer
		{
			public System.Type MappedClass
			{
				get { throw new NotImplementedException(); }
			}

			public object[] GetPropertyValues(object entity)
			{
				throw new NotImplementedException();
			}

			public void SetPropertyValues(object entity, object[] values)
			{
				throw new NotImplementedException();
			}

			public object GetPropertyValue(object entity, int i)
			{
				throw new NotImplementedException();
			}

			public object Instantiate()
			{
				throw new NotImplementedException();
			}

			public bool IsInstance(object obj)
			{
				throw new NotImplementedException();
			}
		}
	}
}