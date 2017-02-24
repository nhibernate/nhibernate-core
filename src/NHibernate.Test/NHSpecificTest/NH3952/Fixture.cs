using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3952
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally", ParentId = e1.Id, Hobbies = new[] { "Inline skate", "Sailing" } };
				session.Save(e2);

				var e3 = new Entity { Name = "Max", ParentId = e1.Id };
				session.Save(e3);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void SimpleNestedSelect()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(e => new { e.Name, children = e.Children.Select(c => c.Name).ToArray() })
					.OrderBy(e => e.Name)
					.ToList();

				Assert.AreEqual(3, result.Count);
				Assert.AreEqual(2, result[0].children.Length);
				Assert.AreEqual("Bob", result[0].Name);
				Assert.Contains("Max", result[0].children);
				Assert.Contains("Sally", result[0].children);
				Assert.AreEqual(0, result[1].children.Length + result[2].children.Length);
			}
		}

		[Test]
		public void ArraySelect()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(e => new { e.Name, e.Hobbies })
					.OrderBy(e => e.Name)
					.ToList();

				Assert.AreEqual(3, result.Count);
				Assert.AreEqual(2, result[2].Hobbies.Length);
				Assert.AreEqual("Sally", result[2].Name);
				Assert.Contains("Inline skate", result[2].Hobbies);
				Assert.Contains("Sailing", result[2].Hobbies);
				Assert.AreEqual(0, result[0].Hobbies.Length + result[1].Hobbies.Length);
			}
		}

		private static readonly MethodInfo CastMethodDefinition = ReflectionHelper.GetMethodDefinition(
			() => Enumerable.Cast<object>(null));

		private static readonly MethodInfo CastMethod = ReflectionHelper.GetMethod(
			() => Enumerable.Cast<int>(null));

		[Test, Explicit("Just a blunt perf comparison among some reflection patterns used in NH")]
		public void ReflectionBluntPerfCompare()
		{
			var swCached = new Stopwatch();
			swCached.Start();
			for (var i = 0; i < 1000; i++)
			{
				Trace.TraceInformation(CastMethod.ToString());
			}
			swCached.Stop();

			var swCachedDef = new Stopwatch();
			swCachedDef.Start();
			for (var i = 0; i < 1000; i++)
			{
				var cast = CastMethodDefinition.MakeGenericMethod(new[] { typeof(int) });
				Trace.TraceInformation(cast.ToString());
			}
			swCachedDef.Stop();

			var swRefl = new Stopwatch();
			swRefl.Start();
			for (var i = 0; i < 1000; i++)
			{
				var cast = ReflectionHelper.GetMethod(() => Enumerable.Cast<int>(null));
				Trace.TraceInformation(cast.ToString());
			}
			swRefl.Stop();

			var swReflDef = new Stopwatch();
			swReflDef.Start();
			for (var i = 0; i < 1000; i++)
			{
				var cast = ReflectionHelper.GetMethodDefinition(() => Enumerable.Cast<object>(null))
					.MakeGenericMethod(new[] { typeof(int) });
				Trace.TraceInformation(cast.ToString());
			}
			swReflDef.Stop();

			var swEnHlp = new Stopwatch();
			swEnHlp.Start();
			for (var i = 0; i < 1000; i++)
			{
				// Testing the obsolete helper perf. Disable obsolete warning. Remove this swEnHlp part of the test if this helper is to be removed.
#pragma warning disable 0618
				var cast = EnumerableHelper.GetMethod("Cast", new[] { typeof(IEnumerable) }, new[] { typeof(int) });
#pragma warning restore 0618
				Trace.TraceInformation(cast.ToString());
			}
			swEnHlp.Stop();

			Assert.Pass(@"Blunt perf timings:
Cached method: {0}
Cached method definition + make gen: {1}
ReflectionHelper.GetMethod: {2}
ReflectionHelper.GetMethodDefinition + make gen: {3}
EnumerableHelper.GetMethod(generic overload): {4}",
				swCached.Elapsed, swCachedDef.Elapsed, swRefl.Elapsed, swReflDef.Elapsed, swEnHlp.Elapsed);
		}
	}
}