using System;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1882
{
	[TestFixture]
	public partial class TestCollectionInitializingDuringFlush : TestCaseMappingByCode
	{
		private readonly InitializingPreUpdateEventListener listener = new InitializingPreUpdateEventListener();

		protected override void Configure(Configuration configuration)
		{
			configuration.EventListeners.PreUpdateEventListeners = new IPreUpdateEventListener[]
			{
				listener
			};
			base.Configure(configuration);
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Author>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Publisher, m => m.Cascade(Mapping.ByCode.Cascade.All));
				rc.Set(x => x.Books, m =>
				{
					m.Cascade(Mapping.ByCode.Cascade.All);
					m.Lazy(CollectionLazy.Lazy);
				}, r => r.OneToMany());
			});
			mapper.Class<Book>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
				rc.Property(x => x.Title);
				rc.ManyToOne(x => x.Author);
			});
			mapper.Class<Publisher>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
				rc.Property(x => x.Name);
				rc.Set(x => x.Authors, m => m.Cascade(Mapping.ByCode.Cascade.All), r => r.OneToMany());
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		public partial class InitializingPreUpdateEventListener : IPreUpdateEventListener
		{
			public static InitializingPreUpdateEventListener Instance = new InitializingPreUpdateEventListener();

			public bool Executed { get; set; }

			public bool FoundAny { get; set; }

			public bool OnPreUpdate(PreUpdateEvent @event)
			{
				Executed = true;
				Object[] oldValues = @event.OldState;
				String[] properties = @event.Persister.PropertyNames;

				// Iterate through all fields of the updated object
				for (int i = 0; i < properties.Length; i++)
				{
					if (oldValues != null && oldValues[i] != null)
					{
						if (!NHibernateUtil.IsInitialized(oldValues[i]))
						{
							// force any proxies and/or collections to initialize to illustrate HHH-2763
							FoundAny = true;
							NHibernateUtil.Initialize(oldValues[i]);
						}
					}
				}
				return true;
			}
		}

		[Test]
		public void TestInitializationDuringFlush()
		{
			Assert.False(listener.Executed);
			Assert.False(listener.FoundAny);

			using (var s1 = OpenSession())
			{
				s1.BeginTransaction();
				var publisher = new Publisher("acme");
				var author = new Author("john");
				author.Publisher = publisher;
				publisher.Authors.Add(author);
				author.Books.Add(new Book("Reflections on a Wimpy Kid", author));
				s1.Save(author);
				s1.Transaction.Commit();
				s1.Clear();
				using (var s2 = OpenSession())
				{
					s2.BeginTransaction();
					publisher = s2.Get<Publisher>(publisher.Id);
					publisher.Name = "random nally";
					s2.Flush();
					s2.Transaction.Commit();
					s2.Clear();
					using (var s3 = OpenSession())
					{
						s3.BeginTransaction();
						s3.Delete(author);
						s3.Transaction.Commit();
						s3.Clear();
						s3.Close();
					}
				}
			}
			Assert.That(listener.Executed, Is.True);
			Assert.That(listener.FoundAny, Is.True);
		}
	}
}