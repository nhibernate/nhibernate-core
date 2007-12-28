namespace NHibernate.Validator.Tests.Integration
{
	using System.Collections;
	using Cfg;
	using Event;
	using Mapping;
	using NHibernate.Event;
	using NUnit.Framework;
	using TestCase=NHibernate.Test.TestCase;

	[TestFixture]
	public class HibernateAnnotationIntegrationFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Validator.Tests"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"Integration.Address.hbm.xml",
						"Integration.Tv.hbm.xml",
						"Integration.TvOwner.hbm.xml",
						"Integration.Martian.hbm.xml",
						"Integration.Music.hbm.xml",
						"Integration.Rock.hbm.xml"
					};
			}
		}

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(NHibernate.Validator.Environment.MESSAGE_INTERPOLATOR_CLASS,
			                typeof(PrefixMessageInterpolator).AssemblyQualifiedName);
			cfg.SetListener(ListenerType.PreInsert,new ValidatePreInsertEventListener());
		}

		public void CleanupData()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			s.Delete("from Address");

			txn.Commit();
			s.Close();
		}

		[Test]
		public void Apply()
		{
			PersistentClass classMapping = cfg.GetClassMapping(typeof(Address));
			new ClassValidator(typeof(Address)).Apply(classMapping);
			IEnumerator ie1 = classMapping.GetProperty("State").ColumnIterator.GetEnumerator();
			ie1.MoveNext();
			Column stateColumn = (Column) ie1.Current;
			Assert.AreEqual(3, stateColumn.Length);

			IEnumerator ie2 = classMapping.GetProperty("Zip").ColumnIterator.GetEnumerator();
			ie2.MoveNext();
			Column zipColumn = (Column) ie2.Current;
			Assert.AreEqual(5, zipColumn.Length);
			Assert.IsFalse(zipColumn.IsNullable);
		}

		[Test]
		public void ApplyOnIdColumn()
		{
			PersistentClass classMapping = cfg.GetClassMapping(typeof(Tv));
			new ClassValidator(typeof(Tv)).Apply(classMapping);
			IEnumerator ie = classMapping.IdentifierProperty.ColumnIterator.GetEnumerator();
			ie.MoveNext();
			Column serialColumn = (Column) ie.Current;
			Assert.AreEqual(2, serialColumn.Length, "Validator annotation not applied on ids");
		}

		[Test]
		public void ApplyOnManyToOne()
		{
			PersistentClass classMapping = cfg.GetClassMapping(typeof(TvOwner));
			new ClassValidator(typeof(TvOwner)).Apply(classMapping);
			IEnumerator ie = classMapping.GetProperty("tv").ColumnIterator.GetEnumerator();
			ie.MoveNext();
			Column serialColumn = (Column) ie.Current;
			Assert.IsFalse(serialColumn.IsNullable, "Validator annotations not applied on associations");
		}

		[Test]
		public void SingleTableAvoidNotNull()
		{
			PersistentClass classMapping = cfg.GetClassMapping(typeof(Rock));
			IEnumerator ie = classMapping.GetProperty("bit").ColumnIterator.GetEnumerator();
			ie.MoveNext();
			Column serialColumn = (Column) ie.Current;
			Assert.IsTrue(serialColumn.IsNullable, "Notnull should not be applised on single tables");
		}

		/// <summary>
		/// Test pre-update/save events and custom interpolator
		/// </summary>
		[Test]
		public void Events()
		{
			ISession s;
			ITransaction tx;
			Address a = new Address();
			Address.blacklistedZipCode = "3232";
			a.Id = 12;
			a.Country = "Country";
			a.Line1 = "Line 1";
			a.Zip = "nonnumeric";
			a.State = "NY";
			s = OpenSession();
			tx = s.BeginTransaction();
			s.Save(a);
			try
			{
				
				tx.Commit();
				Assert.Fail("bean should have been validated");
			}
			catch(InvalidStateException e)
			{
				//success
				Assert.AreEqual(2, e.GetInvalidValues().Length);
				Assert.IsTrue(e.GetInvalidValues()[0].Message.StartsWith("prefix_"),"Environment.MESSAGE_INTERPOLATOR_CLASS does not work");
				Assert.IsTrue(e.GetInvalidValues()[1].Message.StartsWith("prefix_"), "Environment.MESSAGE_INTERPOLATOR_CLASS does not work");
			}
			finally
			{
				if (tx != null && !tx.WasCommitted)
				{
					tx.Rollback();
				}
				s.Close();
			}
			s = OpenSession();
			tx = s.BeginTransaction();
			a.Country = "Country";
			a.Line1 = "Line 1";
			a.Zip = "4343";
			a.State = "NY";
			s.Save(a);
			a.State = "TOOLONG";
			try 
			{
				s.Flush();
				Assert.Fail("bean should have been validated");
			} 
			catch (InvalidStateException e) 
			{
				Assert.AreEqual(1, e.GetInvalidValues().Length);
			} 
			finally 
			{
				if (tx != null && !tx.WasCommitted) 
					tx.Rollback();
				
				s.Close();
			}
		}

		/// <summary>
		/// Test components and composite-id on validation
		/// </summary>
		[Test]
		public void Components()
		{
			ISession s;
			ITransaction tx;
			s = OpenSession();
			tx = s.BeginTransaction();
			Martian martian = new Martian();
			martian.Id = new MartianPk("Liberal", "Biboudie");
			martian.Address = new MarsAddress("Plus","cont");
			s.Save(martian);
			try 
			{
				s.Flush();
				Assert.Fail("Components are not validated");
			}
			catch (InvalidStateException e) 
			{
				Assert.AreEqual(2, e.GetInvalidValues().Length );
			}
			finally 
			{
				tx.Rollback();
				s.Close();
			}
		}
	}
}