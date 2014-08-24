using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Transform;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class DetachedQueryFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "QueryTest.DetachedQueryTest.hbm.xml" }; }
		}

		public const int totalFoo = 15;
		private const string MyComment = "My Comment";

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using(var tx = s.BeginTransaction())
			{
				for (int i = 0; i < totalFoo; i++)
				{
					Foo f = new Foo("N" + i, "D" + i, i);
					s.Save(f);
				}
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using(var tx = s.BeginTransaction())
			{
				s.Delete("from Foo");
				tx.Commit();
			}
		}

		[Test]
		public void PropertiesSet()
		{
			TestDetachedQuery tdq = new TestDetachedQuery();
			tdq.SetMaxResults(10).SetFirstResult(5).SetCacheable(true).SetReadOnly(true).SetTimeout(444).SetFlushMode(
				FlushMode.Auto).SetCacheRegion("A_REGION").SetResultTransformer(new AliasToBeanResultTransformer(typeof(NoFoo))).
				SetIgnoreUknownNamedParameters(true).SetComment(MyComment);
			Assert.AreEqual(10, tdq.Selection.MaxRows);
			Assert.AreEqual(5, tdq.Selection.FirstRow);
			Assert.AreEqual(444, tdq.Selection.Timeout);
			Assert.IsTrue(tdq.Cacheable);
			Assert.IsTrue(tdq.ReadOnly);
			Assert.AreEqual(FlushMode.Auto, tdq.FlushMode);
			Assert.AreEqual("A_REGION", tdq.CacheRegion);
			Assert.IsNotNull(tdq.ResultTransformer);
			Assert.IsTrue(tdq.ShouldIgnoredUnknownNamedParameters);
			Assert.That(tdq.Comment, Is.EqualTo(MyComment));

			tdq.SetLockMode("LM1", LockMode.Upgrade);
			tdq.SetLockMode("LM2", LockMode.Write);
			Assert.AreEqual(2, tdq.LockModes.Count);
			Assert.IsTrue(tdq.LockModes.ContainsKey("LM1"));
			Assert.AreEqual(LockMode.Upgrade, tdq.LockModes["LM1"]);
			Assert.IsTrue(tdq.LockModes.ContainsKey("LM2"));
			Assert.AreEqual(LockMode.Write, tdq.LockModes["LM2"]);

			tdq.SetProperties(new Foo("Pallino", "Pinco"));
			tdq.SetProperties(new Foo("Fulano", "De Tal"));
			Assert.AreEqual(2, tdq.OptionalUntypeParams.Count);
			Assert.IsTrue(tdq.OptionalUntypeParams[0].Equals(new Foo("Pallino", "Pinco")));
			Assert.IsTrue(tdq.OptionalUntypeParams[1].Equals(new Foo("Fulano", "De Tal")));

			tdq.SetAnsiString(1, "");
			tdq.SetBinary(2, new byte[] { });
			tdq.SetBoolean(3, false);
			tdq.SetByte(4, 255);
			tdq.SetCharacter(5, 'A');
			tdq.SetDateTime(6, DateTime.MaxValue);
			tdq.SetDecimal(7, 10.15m);
			tdq.SetDouble(8, 8.1f);
			tdq.SetEntity(9, new Foo("Fulano", "De Tal"));
			tdq.SetEnum(10, FlushMode.Auto);
			tdq.SetInt16(11, 1);
			tdq.SetInt32(12, 1);
			tdq.SetInt64(13, 1);
			tdq.SetSingle(14, 1.1f);
			tdq.SetString(15, "");
			tdq.SetTime(16, DateTime.Now);
			tdq.SetTimestamp(17, DateTime.Now);
			tdq.SetGuid(18, Guid.Empty);
			Assert.IsTrue(tdq.PosParams[1].Type.Equals(NHibernateUtil.AnsiString));
			Assert.IsTrue(tdq.PosParams[2].Type.Equals(NHibernateUtil.Binary));
			Assert.IsTrue(tdq.PosParams[3].Type.Equals(NHibernateUtil.Boolean));
			Assert.IsTrue(tdq.PosParams[4].Type.Equals(NHibernateUtil.Byte));
			Assert.IsTrue(tdq.PosParams[5].Type.Equals(NHibernateUtil.Character));
			Assert.IsTrue(tdq.PosParams[6].Type.Equals(NHibernateUtil.DateTime));
			Assert.IsTrue(tdq.PosParams[7].Type.Equals(NHibernateUtil.Decimal));
			Assert.IsTrue(tdq.PosParams[8].Type.Equals(NHibernateUtil.Double));
			Assert.IsTrue(tdq.PosParams[9].Type.Equals(NHibernateUtil.Entity(typeof(Foo))));
			Assert.IsTrue(tdq.PosParams[10].Type.Equals(NHibernateUtil.Enum(typeof(FlushMode))));
			Assert.IsTrue(tdq.PosParams[11].Type.Equals(NHibernateUtil.Int16));
			Assert.IsTrue(tdq.PosParams[12].Type.Equals(NHibernateUtil.Int32));
			Assert.IsTrue(tdq.PosParams[13].Type.Equals(NHibernateUtil.Int64));
			Assert.IsTrue(tdq.PosParams[14].Type.Equals(NHibernateUtil.Single));
			Assert.IsTrue(tdq.PosParams[15].Type.Equals(NHibernateUtil.String));
			Assert.IsTrue(tdq.PosParams[16].Type.Equals(NHibernateUtil.Time));
			Assert.IsTrue(tdq.PosParams[17].Type.Equals(NHibernateUtil.Timestamp));
			Assert.IsTrue(tdq.PosParams[18].Type.Equals(NHibernateUtil.Guid));

			tdq.SetAnsiString("1", "");
			tdq.SetBinary("2", new byte[] { });
			tdq.SetBoolean("3", false);
			tdq.SetByte("4", 255);
			tdq.SetCharacter("5", 'A');
			tdq.SetDateTime("6", DateTime.MaxValue);
			tdq.SetDecimal("7", 10.15m);
			tdq.SetDouble("8", 8.1f);
			tdq.SetEntity("9", new Foo("Fulano", "De Tal"));
			tdq.SetEnum("10", FlushMode.Auto);
			tdq.SetInt16("11", 1);
			tdq.SetInt32("12", 1);
			tdq.SetInt64("13", 1);
			tdq.SetSingle("14", 1.1f);
			tdq.SetString("15", "");
			tdq.SetTime("16", DateTime.Now);
			tdq.SetTimestamp("17", DateTime.Now);
			tdq.SetGuid("18", Guid.Empty);
			Assert.IsTrue(tdq.NamedParams["1"].Type.Equals(NHibernateUtil.AnsiString));
			Assert.IsTrue(tdq.NamedParams["2"].Type.Equals(NHibernateUtil.Binary));
			Assert.IsTrue(tdq.NamedParams["3"].Type.Equals(NHibernateUtil.Boolean));
			Assert.IsTrue(tdq.NamedParams["4"].Type.Equals(NHibernateUtil.Byte));
			Assert.IsTrue(tdq.NamedParams["5"].Type.Equals(NHibernateUtil.Character));
			Assert.IsTrue(tdq.NamedParams["6"].Type.Equals(NHibernateUtil.DateTime));
			Assert.IsTrue(tdq.NamedParams["7"].Type.Equals(NHibernateUtil.Decimal));
			Assert.IsTrue(tdq.NamedParams["8"].Type.Equals(NHibernateUtil.Double));
			Assert.IsTrue(tdq.NamedParams["9"].Type.Equals(NHibernateUtil.Entity(typeof(Foo))));
			Assert.IsTrue(tdq.NamedParams["10"].Type.Equals(NHibernateUtil.Enum(typeof(FlushMode))));
			Assert.IsTrue(tdq.NamedParams["11"].Type.Equals(NHibernateUtil.Int16));
			Assert.IsTrue(tdq.NamedParams["12"].Type.Equals(NHibernateUtil.Int32));
			Assert.IsTrue(tdq.NamedParams["13"].Type.Equals(NHibernateUtil.Int64));
			Assert.IsTrue(tdq.NamedParams["14"].Type.Equals(NHibernateUtil.Single));
			Assert.IsTrue(tdq.NamedParams["15"].Type.Equals(NHibernateUtil.String));
			Assert.IsTrue(tdq.NamedParams["16"].Type.Equals(NHibernateUtil.Time));
			Assert.IsTrue(tdq.NamedParams["17"].Type.Equals(NHibernateUtil.Timestamp));
			Assert.IsTrue(tdq.NamedParams["18"].Type.Equals(NHibernateUtil.Guid));

			tdq.SetParameter(10, 123456m);
			Assert.AreEqual(1, tdq.PosUntypeParams.Count);
			Assert.IsTrue(tdq.PosUntypeParams.ContainsKey(10));

			tdq.SetParameter("Any", 123456m);
			Assert.AreEqual(1, tdq.NamedUntypeParams.Count);
			Assert.IsTrue(tdq.NamedUntypeParams.ContainsKey("Any"));

			tdq.SetParameterList("UntypedList", new int[] { 1, 2, 3 });
			Assert.IsTrue(tdq.NamedUntypeListParams.ContainsKey("UntypedList"));

			tdq.SetParameterList("TypedList", new Int64[] { 1, 2, 3 }, NHibernateUtil.Int64);
			Assert.IsTrue(tdq.NamedListParams.ContainsKey("TypedList"));
			Assert.IsNotNull((tdq.NamedListParams["TypedList"].Value as IEnumerable));
		}

		[Test]
		public void CopyToTest()
		{
			TestDetachedQuery origin = new TestDetachedQuery();
			origin.SetMaxResults(10).SetFirstResult(5).SetCacheable(true).SetReadOnly(true).SetTimeout(444).SetFlushMode
				(FlushMode.Auto).SetCacheRegion("A_REGION").SetResultTransformer(new AliasToBeanResultTransformer(typeof(NoFoo)));
			origin.SetComment(MyComment);
			origin.SetLockMode("LM1", LockMode.Upgrade);
			origin.SetProperties(new Foo("Pallino", "Pinco"));
			origin.SetInt64(1, 1);
			origin.SetBinary(2, new byte[] { });
			origin.SetBoolean(3, false);
			origin.SetDateTime(6, DateTime.MaxValue);
			origin.SetCharacter("5", 'A');
			origin.SetDateTime("6", DateTime.MaxValue);
			origin.SetDecimal("7", 10.15m);
			origin.SetParameterList("UntypedList", new int[] { 1, 2, 3 });
			origin.SetParameterList("TypedList", new Int64[] { 1, 2, 3 }, NHibernateUtil.Int64);

			TestDetachedQuery tdq = new TestDetachedQuery();
			tdq.SetLockMode("LM1", LockMode.Read);
			tdq.SetLockMode("LM2", LockMode.Write);
			tdq.SetProperties(new Foo("Fulano", "De Tal"));
			tdq.SetAnsiString(1, ""); //will be override
			tdq.SetByte(4, 255);
			tdq.SetCharacter(5, 'A');
			tdq.SetDateTime(6, DateTime.MinValue); // will be override
			tdq.SetDateTime("6", DateTime.MinValue); // will be override
			tdq.SetDouble("8", 8.1f);
			tdq.SetEntity("9", new Foo("Fulano", "De Tal"));
			tdq.SetParameterList("UntypedList", new int[] { 5, 6, 7, 8 }); // will be override
			tdq.SetParameterList("TypedList", new Int64[] { 5, 6, 7, 8 }, NHibernateUtil.Int64); // will be override
			tdq.SetComment("other comment"); // will be override
			origin.CopyTo(tdq);

			Assert.AreEqual(5, tdq.Selection.FirstRow);
			Assert.AreEqual(444, tdq.Selection.Timeout);
			Assert.IsTrue(tdq.Cacheable);
			Assert.IsTrue(tdq.ReadOnly);
			Assert.AreEqual(FlushMode.Auto, tdq.FlushMode);
			Assert.AreEqual("A_REGION", tdq.CacheRegion);
			Assert.IsNotNull(tdq.ResultTransformer);
			Assert.That(tdq.Comment, Is.EqualTo(MyComment));

			// merge/override of LockModes
			Assert.AreEqual(2, tdq.LockModes.Count);
			Assert.IsTrue(tdq.LockModes.ContainsKey("LM1"));
			Assert.AreEqual(LockMode.Upgrade, tdq.LockModes["LM1"]);
			Assert.IsTrue(tdq.LockModes.ContainsKey("LM2"));
			Assert.AreEqual(LockMode.Write, tdq.LockModes["LM2"]);

			// merge of OptionalUntypeParams (eventually override by IQuery)
			Assert.AreEqual(2, tdq.OptionalUntypeParams.Count);
			Assert.IsTrue(tdq.OptionalUntypeParams[0].Equals(new Foo("Fulano", "De Tal")));
			Assert.IsTrue(tdq.OptionalUntypeParams[1].Equals(new Foo("Pallino", "Pinco")));

			// merge/override positional parameters
			Assert.IsTrue(tdq.PosParams[1].Type.Equals(NHibernateUtil.Int64));
			Assert.IsTrue(tdq.PosParams[2].Type.Equals(NHibernateUtil.Binary));
			Assert.IsTrue(tdq.PosParams[3].Type.Equals(NHibernateUtil.Boolean));
			Assert.IsTrue(tdq.PosParams[4].Type.Equals(NHibernateUtil.Byte));
			Assert.IsTrue(tdq.PosParams[5].Type.Equals(NHibernateUtil.Character));
			Assert.IsTrue(tdq.PosParams[6].Type.Equals(NHibernateUtil.DateTime));
			Assert.IsTrue(tdq.PosParams[6].Value.Equals(DateTime.MaxValue));

			// merge/override named parameters
			Assert.IsTrue(tdq.NamedParams["5"].Type.Equals(NHibernateUtil.Character));
			Assert.IsTrue(tdq.NamedParams["6"].Type.Equals(NHibernateUtil.DateTime));
			Assert.IsTrue(tdq.NamedParams["6"].Value.Equals(DateTime.MaxValue));
			Assert.IsTrue(tdq.NamedParams["7"].Type.Equals(NHibernateUtil.Decimal));
			Assert.IsTrue(tdq.NamedParams["8"].Type.Equals(NHibernateUtil.Double));
			Assert.IsTrue(tdq.NamedParams["9"].Type.Equals(NHibernateUtil.Entity(typeof(Foo))));

			// merge/override named parameters list
			int expected = 1;
			foreach (int i in tdq.NamedUntypeListParams["UntypedList"])
			{
				Assert.AreEqual(expected, i);
				expected++;
			}

			// merge/override named params lists
			long lexpected = 1;
			foreach (long i in (tdq.NamedListParams["TypedList"].Value as IEnumerable))
			{
				Assert.AreEqual(lexpected, i);
				lexpected++;
			}
		}

		[Test]
		public void OverrideTest()
		{
			TestDetachedQuery origin = new TestDetachedQuery();
			origin.SetMaxResults(10).SetFirstResult(5).SetCacheable(true).SetReadOnly(true).SetTimeout(444).SetFlushMode
				(FlushMode.Auto).SetCacheRegion("A_REGION").SetResultTransformer(new AliasToBeanResultTransformer(typeof(NoFoo)));
			origin.SetLockMode("LM1", LockMode.Upgrade);
			origin.SetProperties(new Foo("Pallino", "Pinco"));
			origin.SetInt64(1, 1);

			TestDetachedQuery tdq = new TestDetachedQuery();
			tdq.SetLockMode("LM1", LockMode.Read);
			tdq.SetLockMode("LM2", LockMode.Write);
			tdq.SetProperties(new Foo("Fulano", "De Tal"));
			tdq.SetAnsiString(1, ""); //will be override
			tdq.SetByte(4, 255);
			tdq.SetCharacter(5, 'A');
			tdq.SetDateTime(6, DateTime.MinValue); // will be override
			tdq.SetDateTime("6", DateTime.MinValue); // will be override
			tdq.SetDouble("8", 8.1f);
			tdq.SetEntity("9", new Foo("Fulano", "De Tal"));
			tdq.SetParameterList("UntypedList", new int[] { 5, 6, 7, 8 }); // will be override
			tdq.SetParameterList("TypedList", new Int64[] { 5, 6, 7, 8 }, NHibernateUtil.Int64); // will be override

			tdq.OverrideInfoFrom(origin);

			Assert.AreEqual(5, tdq.Selection.FirstRow);
			Assert.AreEqual(444, tdq.Selection.Timeout);
			Assert.IsTrue(tdq.Cacheable);
			Assert.IsTrue(tdq.ReadOnly);
			Assert.AreEqual(FlushMode.Auto, tdq.FlushMode);
			Assert.AreEqual("A_REGION", tdq.CacheRegion);
			Assert.IsNotNull(tdq.ResultTransformer);

			// override of LockModes
			Assert.AreEqual(1, tdq.LockModes.Count);
			Assert.IsTrue(tdq.LockModes.ContainsKey("LM1"));
			Assert.AreEqual(LockMode.Upgrade, tdq.LockModes["LM1"]);

			// override OptionalUntypeParams
			Assert.AreEqual(1, tdq.OptionalUntypeParams.Count);
			Assert.IsTrue(tdq.OptionalUntypeParams[0].Equals(new Foo("Pallino", "Pinco")));

			// override positional parameters
			Assert.AreEqual(1, tdq.PosParams.Count);
			Assert.IsTrue(tdq.PosParams[1].Type.Equals(NHibernateUtil.Int64));

			// override named parameters
			Assert.AreEqual(0, tdq.NamedParams.Count);

			// override named parameters list
			Assert.AreEqual(0, tdq.NamedUntypeListParams.Count);

			// override named params lists
			Assert.AreEqual(0, tdq.NamedListParams.Count);
		}

		[Test]
		public void ExecutableQuery()
		{
			// Simply fetch
			IDetachedQuery dq = new DetachedQuery("from Foo");
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList l = q.List();
				Assert.AreEqual(totalFoo, l.Count);
			}

			// With Typed Parameters
			dq = new DetachedQuery("from Foo f where f.Name=:pn and f.Description=:pd");
			dq.SetString("pn", "N2").SetString("pd", "D2");
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList<Foo> l = q.List<Foo>();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual("N2", l[0].Name);
				Assert.AreEqual("D2", l[0].Description);
			}

			// With UnTyped Parameters
			dq = new DetachedQuery("from Foo f where f.Name=:pn and f.Description=:pd");
			dq.SetParameter("pn", "N2").SetParameter("pd", "D2");
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList<Foo> l = q.List<Foo>();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual("N2", l[0].Name);
				Assert.AreEqual("D2", l[0].Description);
			}

			// With UnTyped Parameter List
			dq = new DetachedQuery("from Foo f where f.IntValue in (:pn)");
			dq.SetParameterList("pn", new int[] { 2, 3 });
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList<Foo> l = q.List<Foo>();
				Assert.AreEqual(2, l.Count);

				Assert.True(l.Contains(new Foo("N2", "D2")));
				Assert.True(l.Contains(new Foo("N3", "D3")));
			}

			// Pagination
			dq = new DetachedQuery("from Foo f order by f.IntValue");
			dq.SetFirstResult(0).SetMaxResults(2);
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList<Foo> l = q.List<Foo>();
				Assert.AreEqual(2, l.Count);
				Assert.AreEqual("N0", l[0].Name);
				Assert.AreEqual("N1", l[1].Name);
			}
			dq.SetFirstResult(2).SetMaxResults(1);
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList<Foo> l = q.List<Foo>();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual("N2", l[0].Name);
			}
		}

		[Test]
		public void ExecutableNamedQuery()
		{
			IDetachedQuery dq = new DetachedNamedQuery("Foo.WithParameters");
			dq.SetString("pn", "N2");
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList<Foo> l = q.List<Foo>();
				Assert.AreEqual(1, l.Count);
				Assert.AreEqual("N2", l[0].Name);
				Assert.AreEqual("D2", l[0].Description);
			}
			// reusing same IDetachedQuery
			dq.SetString("pn", "@All@");
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList l = q.List();
				Assert.AreEqual(totalFoo, l.Count);
			}
		}

		[Test]
		public void ResultTransformer()
		{
			IDetachedQuery dq = new DetachedNamedQuery("NoFoo.SQL.Parameters");
			dq.SetString("p1", "%1_")
				.SetResultTransformer(new AliasToBeanResultTransformer(typeof(NoFoo)));
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
				IList<NoFoo> l = q.List<NoFoo>();
				Assert.AreEqual(5, l.Count);
			}
		}

		[Test]
		public void Serializable()
		{
			DetachedQuery dq = new DetachedQuery("from Foo f where f.Name=:pn and f.Description=:pd");
			dq.SetString("pn", "N2").SetString("pd", "D2");
			byte[] bytes = SerializationHelper.Serialize(dq);

			DetachedQuery dqs = (DetachedQuery)SerializationHelper.Deserialize(bytes);

			using (ISession s = OpenSession())
			{
				dqs.GetExecutableQuery(s).List();
			}
		}

		[Test, Explicit]
		public void PerformanceDiffSimplyQuery()
		{

			DateTime sDQStart = DateTime.Now;
			DetachedQuery dq = new DetachedQuery("from Foo f where f.Name=:pn and f.Description=:pd");
			dq.SetString("pn", "N2").SetString("pd", "D2");
			using (ISession s = OpenSession())
			{
				IQuery q = dq.GetExecutableQuery(s);
			}
			DateTime sDQStop = DateTime.Now;

			DateTime sQStart = DateTime.Now;
			using (ISession s = OpenSession())
			{
				IQuery q = s.CreateQuery("from Foo f where f.Name=:pn and f.Description=:pd").SetString("pn", "N2").SetString("pd", "D2");
			}
			DateTime sQStop = DateTime.Now;


			Console.WriteLine("DetachedQueryCycle={0} QueryCycl={1}  Diff={2}", sDQStop - sDQStart, sQStop - sQStart,
												(sDQStop - sDQStart) - (sQStop - sQStart));
		}

		private class TestDetachedQuery : AbstractDetachedQuery
		{
			public Dictionary<int, object> PosUntypeParams
			{
				get { return posUntypeParams; }
			}

			public Dictionary<string, object> NamedUntypeParams
			{
				get { return namedUntypeParams; }
			}

			public IList OptionalUntypeParams
			{
				get { return optionalUntypeParams; }
			}

			public Dictionary<int, TypedValue> PosParams
			{
				get { return posParams; }
			}

			public Dictionary<string, TypedValue> NamedParams
			{
				get { return namedParams; }
			}

			public Dictionary<string, LockMode> LockModes
			{
				get { return lockModes; }
			}

			public RowSelection Selection
			{
				get { return selection; }
			}

			public bool Cacheable
			{
				get { return cacheable; }
			}

			public string CacheRegion
			{
				get { return cacheRegion; }
			}

			public bool ReadOnly
			{
				get { return readOnly; }
			}

			public FlushMode FlushMode
			{
				get { return flushMode; }
			}

			public IResultTransformer ResultTransformer
			{
				get { return resultTransformer; }
			}

			public bool ShouldIgnoredUnknownNamedParameters
			{
				get { return shouldIgnoredUnknownNamedParameters; }
			}

			public Dictionary<string, IEnumerable> NamedUntypeListParams
			{
				get { return namedUntypeListParams; }
			}

			public Dictionary<string, TypedValue> NamedListParams
			{
				get { return namedListParams; }
			}

			public override IQuery GetExecutableQuery(ISession session)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public new void CopyTo(IDetachedQuery destination)
			{
				base.CopyTo(destination);
			}

			public void OverrideInfoFrom(IDetachedQueryImplementor origin)
			{
				(this as IDetachedQueryImplementor).OverrideInfoFrom(origin);
			}

			public string Comment
			{
				get { return comment; }
			}
		}
	}

	public class Foo
	{
#pragma warning disable 649
		private int id;
#pragma warning restore 649
		public virtual int Id
		{
			get { return id; }
		}

		private string name;
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string description;
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		private int intValue;
		public virtual int IntValue
		{
			get { return intValue; }
			set { intValue = value; }
		}

		public Foo()
		{
		}

		public Foo(string name, string description)
		{
			this.name = name;
			this.description = description;
		}

		public Foo(string name, string description, int intValue)
			: this(name, description)
		{
			this.intValue = intValue;
		}

		public override bool Equals(object obj)
		{
			Foo foo = obj as Foo;
			if (foo != null) return Equals(foo);
			return base.Equals(obj);
		}

		public virtual bool Equals(Foo obj)
		{
			return name.Equals(obj.name) && description.Equals(obj.description);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class NoFoo
	{
		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string description;
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
	}
}
