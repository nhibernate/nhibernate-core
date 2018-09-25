using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Proxy;
using NUnit.Framework;
#if !NETFX
using NHibernate.Util;
#endif

namespace NHibernate.Test.StaticProxyTest
{
	[TestFixture]
	public class StaticProxyFixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";
		protected override string[] Mappings => new[] { "StaticProxyTest.Mappings.hbm.xml" };

		private object _idSimpleEntity1;
		private object _idSimpleEntity2;
		private object _idInterfacedEntity1;
		private object _idInterfacedEntity2;
		private object _idInterfacedLazyTextEntity1;
		private object _idInterfacedLazyTextEntity2;
		private object _idInterfacedLazyTextEntity3;
		private object _idInterfacedLazyTextEntity4;
		private object _idInterfacedLazyTextEntity5;
		private object _idLazyTextEntity1;
		private object _idLazyTextEntity2;
		private object _idLazyTextEntity3;
		private object _idLazyTextEntity4;
		private object _idLazyTextEntity5;
		private object _idOverridingEntity1;
		private object _idOverridingEntityChildWithOtherThanIdAccess1;
		private object _idOverridingEntityWithField1;
		private object _idOverridingEntityWithFieldChild1;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var se = new SimpleEntity
				{
					Name = "1",
					Text = "Text1"
				};
				_idSimpleEntity1 = s.Save(se);

				se = new SimpleEntity
				{
					Name = "2",
					Text = "Text2"
				};
				_idSimpleEntity2 = s.Save(se);

				IEntity ie = new InterfacedEntity
				{
					Name = "1",
					Text = "Text1"
				};
				_idInterfacedEntity1 = s.Save(ie);

				ie = new InterfacedEntity
				{
					Name = "2",
					Text = "Text2"
				};
				_idInterfacedEntity2 = s.Save(ie);

				ILazyTextEntity ilte = new InterfacedLazyTextEntity()
				{
					Name = "1",
					Text = "Text1"
				};
				_idInterfacedLazyTextEntity1 = s.Save(ilte);

				ilte = new InterfacedLazyTextEntity
				{
					Name = "2",
					Text = "Text2"
				};
				_idInterfacedLazyTextEntity2 = s.Save(ilte);

				ilte = new InterfacedLazyTextEntity
				{
					Name = "3",
					Text = "Text3"
				};
				_idInterfacedLazyTextEntity3 = s.Save(ilte);

				ilte = new InterfacedLazyTextEntity
				{
					Name = "4",
					Text = "Text4"
				};
				_idInterfacedLazyTextEntity4 = s.Save(ilte);

				ilte = new InterfacedLazyTextEntity
				{
					Name = "5",
					Text = "Text5"
				};
				_idInterfacedLazyTextEntity5 = s.Save(ilte);

				var le = new LazyTextEntity()
				{
					Name = "1",
					Text = "Text1"
				};
				_idLazyTextEntity1 = s.Save(le);

				le = new LazyTextEntity
				{
					Name = "2",
					Text = "Text2"
				};
				_idLazyTextEntity2 = s.Save(le);

				le = new LazyTextEntity
				{
					Name = "3",
					Text = "Text3"
				};
				_idLazyTextEntity3 = s.Save(le);

				le = new LazyTextEntity
				{
					Name = "4",
					Text = "Text4"
				};
				_idLazyTextEntity4 = s.Save(le);

				le = new LazyTextEntity
				{
					Name = "5",
					Text = "Text5"
				};
				_idLazyTextEntity5 = s.Save(le);

				var oe = new OverridingEntity
				{
					Name = "1",
					Text = "Text1"
				};
				_idOverridingEntity1 = s.Save(oe);

				oe = new OverridingEntityChildWithOtherThanIdAccess
				{
					Name = "1",
					Text = "Text1"
				};
				_idOverridingEntityChildWithOtherThanIdAccess1 = s.Save(oe);

				var oef = new OverridingEntityWithField()
				{
					Name = "1",
					Text = "Text1"
				};
				_idOverridingEntityWithField1 = s.Save(oef);

				oef = new OverridingEntityWithFieldChild()
				{
					Name = "1",
					Text = "Text1"
				};
				_idOverridingEntityWithFieldChild1 = s.Save(oef);

				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void SessionSerializationWithNHProxy()
		{
			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					var s1 = s.Load<SimpleEntity>(_idSimpleEntity1);
					NHibernateUtil.Initialize(s1);
					Assert.That(
						NHibernateUtil.IsInitialized(s1),
						Is.True,
						"s1 should be initialized after initialization");
					var i1 = s.Load<IEntity>(_idInterfacedEntity1);
					NHibernateUtil.Initialize(i1);
					Assert.That(
						NHibernateUtil.IsInitialized(i1),
						Is.True,
						"i1 should be initialized after initialization");

					var s2 = s.Load<SimpleEntity>(_idSimpleEntity2);
					Assert.That(
						NHibernateUtil.IsInitialized(s2),
						Is.False,
						"s2 should not be initialized after loading");
					var i2 = s.Load<IEntity>(_idInterfacedEntity2);
					Assert.That(
						NHibernateUtil.IsInitialized(i2),
						Is.False,
						"i2 should not be initialized after loading");

					t.Commit();
				}

				var serializer = GetFormatter();
				ISession ds;
				using (var memoryStream = new MemoryStream())
				{
					serializer.Serialize(memoryStream, s);
					s.Close();
					memoryStream.Seek(0L, SeekOrigin.Begin);
					ds = (ISession) serializer.Deserialize(memoryStream);
				}

				try
				{
					using (var t = ds.BeginTransaction())
					{
						var s1 = ds.Load<SimpleEntity>(_idSimpleEntity1);
						Assert.That(
							NHibernateUtil.IsInitialized(s1),
							Is.True,
							"s1 should be initialized after deserialization");
						Assert.That(s1.Name, Is.EqualTo("1"), "s1.Name");
						Assert.That(s1.Text, Is.EqualTo("Text1"), "s1.Text");
						var i1 = ds.Load<IEntity>(_idInterfacedEntity1);
						Assert.That(
							NHibernateUtil.IsInitialized(i1),
							Is.True,
							"i1 should be initialized after deserialization");
						Assert.That(i1.Name, Is.EqualTo("1"), "i1.Name");
						Assert.That(i1.Text, Is.EqualTo("Text1"), "i1.Text");

						var s2 = ds.Load<SimpleEntity>(_idSimpleEntity2);
						Assert.That(
							NHibernateUtil.IsInitialized(s2),
							Is.False,
							"s2 should not be initialized after deserialization");
						NHibernateUtil.Initialize(s2);
						Assert.That(
							NHibernateUtil.IsInitialized(s2),
							Is.True,
							"s2 should be initialized after initialization");
						Assert.That(s2.Name, Is.EqualTo("2"), "s2.Name");
						Assert.That(s2.Text, Is.EqualTo("Text2"), "s2.Text");
						var i2 = ds.Load<IEntity>(_idInterfacedEntity2);
						Assert.That(
							NHibernateUtil.IsInitialized(i2),
							Is.False,
							"i2 should not be initialized after deserialization");
						NHibernateUtil.Initialize(i2);
						Assert.That(
							NHibernateUtil.IsInitialized(i2),
							Is.True,
							"i2 should be initialized after initialization");
						Assert.That(i2.Name, Is.EqualTo("2"), "i2.Name");
						Assert.That(i2.Text, Is.EqualTo("Text2"), "i2.Text");

						t.Commit();
					}
				}
				finally
				{
					ds.Dispose();
				}
			}
		}

		[Test]
		public void SessionSerializationWithLazyProperty()
		{
			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					// Get + initialize property
					var le1 = s.Get<LazyTextEntity>(_idLazyTextEntity1);
					Assert.That(le1.Text, Is.EqualTo("Text1"), "le1.Text after Get");
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(le1, nameof(LazyTextEntity.Text)),
						Is.True, "Lazy Text property should be initialized on le1 after reading it");
					Assert.That(le1.IsProxy(), Is.False, "le1 should not be a lazy entity proxy");
					var ilte1 = s.Get<ILazyTextEntity>(_idInterfacedLazyTextEntity1);
					Assert.That(ilte1.Text, Is.EqualTo("Text1"), "ilte1.Text after Get");
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(ilte1, nameof(ILazyTextEntity.Text)),
						Is.True, "Lazy Text property should be initialized on ilte1 after reading it");
					Assert.That(ilte1.IsProxy(), Is.False, "ilte1 should not be a lazy entity proxy");

					// Just Get
					var le2 = s.Get<LazyTextEntity>(_idLazyTextEntity2);
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(le2, nameof(LazyTextEntity.Text)),
						Is.False, "Lazy Text property should not be initialized on le2 after Get");
					Assert.That(le2.IsProxy(), Is.False, "le2 should not be a lazy entity proxy");
					var ilte2 = s.Get<ILazyTextEntity>(_idInterfacedLazyTextEntity2);
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(ilte2, nameof(ILazyTextEntity.Text)),
						Is.False, "Lazy Text property should not be initialized on ilte2 after Get");
					Assert.That(ilte2.IsProxy(), Is.False, "ilte2 should not be a lazy entity proxy");

					// Load + initialize property
					var le3 = s.Load<LazyTextEntity>(_idLazyTextEntity3);
					Assert.That(le3.Text, Is.EqualTo("Text3"), "le3.Text after Load");
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(le3, nameof(LazyTextEntity.Text)),
						Is.True, "Lazy Text property should be initialized on le3 after reading it");
					Assert.That(le3.IsProxy(), Is.True, "le3 should be a lazy entity proxy");
					var ilte3 = s.Load<ILazyTextEntity>(_idInterfacedLazyTextEntity3);
					Assert.That(ilte3.Text, Is.EqualTo("Text3"), "ilte3.Text after Load");
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(ilte3, nameof(ILazyTextEntity.Text)),
						Is.True, "Lazy Text property should be initialized on ilte3 after reading it");
					Assert.That(ilte3.IsProxy(), Is.True, "ilte3 should be a lazy entity proxy");

					// Just Load
					var le4 = s.Load<LazyTextEntity>(_idLazyTextEntity4);
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(le4, nameof(LazyTextEntity.Text)),
						Is.False, "Lazy Text property should not be initialized on le4 after Load");
					Assert.That(
						NHibernateUtil.IsInitialized(le4), Is.False,
						"le4 should not be initialized after checking if a property is initialized");
					Assert.That(le4.IsProxy(), Is.True, "le4 should be a lazy entity proxy");
					var ilte4 = s.Load<ILazyTextEntity>(_idInterfacedLazyTextEntity4);
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(ilte4, nameof(ILazyTextEntity.Text)),
						Is.False, "Lazy Text property should not be initialized on ilte4 after Load");
					Assert.That(
						NHibernateUtil.IsInitialized(ilte4), Is.False,
						"ilte4 should not be initialized after checking if a property is initialized");
					Assert.That(ilte4.IsProxy(), Is.True, "ilte4 should be a lazy entity proxy");

					// Load + initialize entity proxy
					var le5 = s.Load<LazyTextEntity>(_idLazyTextEntity5);
					Assert.That(le5.Name, Is.EqualTo("5"), "le5.Name after load");
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(le5, nameof(LazyTextEntity.Text)),
						Is.False, "Lazy Text property should not be initialized on le5 after initializing its proxy");
					Assert.That(le5.IsProxy(), Is.True, "le5 should be a lazy entity proxy");
					var ilte5 = s.Load<ILazyTextEntity>(_idInterfacedLazyTextEntity5);
					Assert.That(ilte5.Name, Is.EqualTo("5"), "ilte5.Name after load");
					Assert.That(
						NHibernateUtil.IsPropertyInitialized(ilte5, nameof(ILazyTextEntity.Text)),
						Is.False, "Lazy Text property should not be initialized on ilte5 after initializing its proxy");
					Assert.That(ilte5.IsProxy(), Is.True, "ilte5 should be a lazy entity proxy");

					t.Commit();
				}

				var serializer = GetFormatter();
				ISession ds;
				using (var memoryStream = new MemoryStream())
				{
					serializer.Serialize(memoryStream, s);
					s.Close();
					memoryStream.Seek(0L, SeekOrigin.Begin);
					ds = (ISession) serializer.Deserialize(memoryStream);
				}

				try
				{
					using (var t = ds.BeginTransaction())
					{
						var le1 = ds.Load<LazyTextEntity>(_idLazyTextEntity1);
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(le1, nameof(LazyTextEntity.Text)),
							Is.True, "Lazy Text property should be initialized on le1 after deserializing it");
						Assert.That(le1.Name, Is.EqualTo("1"), "le1.Name after deserialization");
						Assert.That(le1.Text, Is.EqualTo("Text1"), "le1.Text after deserialization");
						Assert.That(le1.IsProxy(), Is.False, "le1 should not be a lazy entity proxy after deserializing it");
						var ilte1 = ds.Load<ILazyTextEntity>(_idInterfacedLazyTextEntity1);
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(ilte1, nameof(ILazyTextEntity.Text)),
							Is.True, "Lazy Text property should be initialized on ilte1 after deserializing it");
						Assert.That(ilte1.Name, Is.EqualTo("1"), "ilte1.Name after deserialization");
						Assert.That(ilte1.Text, Is.EqualTo("Text1"), "ilte1.Text after deserialization");
						Assert.That(ilte1.IsProxy(), Is.False, "ilte1 should not be a lazy entity proxy after deserializing it");

						var le2 = ds.Load<LazyTextEntity>(_idLazyTextEntity2);
						Assert.That(
							NHibernateUtil.IsInitialized(le2), Is.True,
							"le2 should be initialized after deserialization");
						Assert.That(le2.Name, Is.EqualTo("2"), "le2.Name after deserialization");
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(le2, nameof(LazyTextEntity.Text)),
							Is.False, "Lazy Text property should not be initialized on le2 after deserializing it");
						Assert.That(le2.Text, Is.EqualTo("Text2"), "le2.Text after deserialization");
						Assert.That(le2.IsProxy(), Is.False, "le2 should not be a lazy entity proxy after deserializing it");
						var ilte2 = ds.Load<ILazyTextEntity>(_idInterfacedLazyTextEntity2);
						Assert.That(
							NHibernateUtil.IsInitialized(ilte2), Is.True,
							"ilte2 should be initialized after deserialization");
						Assert.That(ilte2.Name, Is.EqualTo("2"), "ilte2.Name after deserialization");
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(ilte2, nameof(ILazyTextEntity.Text)),
							Is.False, "Lazy Text property should not be initialized on ilte2 after deserializing it");
						Assert.That(ilte2.Text, Is.EqualTo("Text2"), "ilte2.Text after deserialization");
						Assert.That(ilte2.IsProxy(), Is.False, "ilte2 should not be a lazy entity proxy after deserializing it");

						var le3 = ds.Load<LazyTextEntity>(_idLazyTextEntity3);
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(le3, nameof(LazyTextEntity.Text)),
							Is.True, "Lazy Text property should be initialized on le3 after deserializing it");
						Assert.That(le3.Name, Is.EqualTo("3"), "le3.Name after deserialization");
						Assert.That(le3.Text, Is.EqualTo("Text3"), "le3.Text after deserialization");
						Assert.That(le3.IsProxy(), Is.True, "le3 should be a lazy entity proxy after deserializing it");
						var ilte3 = ds.Load<ILazyTextEntity>(_idInterfacedLazyTextEntity3);
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(ilte3, nameof(ILazyTextEntity.Text)),
							Is.True, "Lazy Text property should be initialized on ilte3 after deserializing it");
						Assert.That(ilte3.Name, Is.EqualTo("3"), "ilte3.Name after deserialization");
						Assert.That(ilte3.Text, Is.EqualTo("Text3"), "ilte3.Text after deserialization");
						Assert.That(ilte3.IsProxy(), Is.True, "ilte3 should be a lazy entity proxy after deserializing it");

						var le4 = ds.Load<LazyTextEntity>(_idLazyTextEntity4);
						Assert.That(
							NHibernateUtil.IsInitialized(le4), Is.False,
							"le4 should not be initialized after deserialization");
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(le4, nameof(LazyTextEntity.Text)),
							Is.False, "Lazy Text property should not be initialized on le4 after deserializing it");
						Assert.That(le4.Text, Is.EqualTo("Text4"), "le4.Text after deserialization");
						Assert.That(le4.Name, Is.EqualTo("4"), "le4.Name after deserialization");
						Assert.That(le4.IsProxy(), Is.True, "le4 should be a lazy entity proxy after deserializing it");
						var ilte4 = ds.Load<ILazyTextEntity>(_idInterfacedLazyTextEntity4);
						Assert.That(
							NHibernateUtil.IsInitialized(ilte4), Is.False,
							"ilte4 should not be initialized after deserialization");
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(ilte4, nameof(ILazyTextEntity.Text)),
							Is.False, "Lazy Text property should not be initialized on ilte4 after deserializing it");
						Assert.That(ilte4.Text, Is.EqualTo("Text4"), "ilte4.Text after deserialization");
						Assert.That(ilte4.Name, Is.EqualTo("4"), "ilte4.Name after deserialization");
						Assert.That(ilte4.IsProxy(), Is.True, "ilte4 should be a lazy entity proxy after deserializing it");

						var le5 = ds.Load<LazyTextEntity>(_idLazyTextEntity5);
						Assert.That(
							NHibernateUtil.IsInitialized(le5), Is.True,
							"le5 should be initialized after deserialization");
						Assert.That(le5.Name, Is.EqualTo("5"), "le5.Name after deserialization");
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(le5, nameof(LazyTextEntity.Text)),
							Is.False, "Lazy Text property should not be initialized on le5 after deserializing it");
						Assert.That(le5.Text, Is.EqualTo("Text5"), "le5.Text after deserialization");
						Assert.That(le5.IsProxy(), Is.True, "le5 should be a lazy entity proxy after deserializing it");
						var ilte5 = ds.Load<ILazyTextEntity>(_idInterfacedLazyTextEntity5);
						Assert.That(
							NHibernateUtil.IsInitialized(ilte5), Is.True,
							"ilte5 should be initialized after deserialization");
						Assert.That(ilte5.Name, Is.EqualTo("5"), "ilte5.Name after deserialization");
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(ilte5, nameof(ILazyTextEntity.Text)),
							Is.False, "Lazy Text property should not be initialized on ilte5 after deserializing it");
						Assert.That(ilte5.Text, Is.EqualTo("Text5"), "ilte5.Text after deserialization");
						Assert.That(ilte5.IsProxy(), Is.True, "ilte5 should be a lazy entity proxy after deserializing it");

						t.Commit();
					}
				}
				finally
				{
					ds.Dispose();
				}
			}
		}

		[Test]
		public void OverridenGetHashCodeAndEqualsDoNotAlwaysInitializeProxyOfEntityWithoutFields()
		{
			// A proxy does not need to force initialization when calling an overriden GetHashCode or Equals
			// if the entity does not have fields: in such case their implementation can only rely either on
			// things not needing the proxy initialization (like original object.GetHashCode) or on properties
			// overriden by the proxy and which will yield the correct value, either initializing the proxy or
			// being the id. In the later case, initializing the proxy can be avoided.
			OverridingEntity oeProxy, oe;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oe = s.Get<OverridingEntity>(_idOverridingEntity1);
				Assert.That(oe.IsProxy(), Is.False, "oe is unexpectedly a proxy");
				t.Commit();
			}
			Sfi.Evict(typeof(OverridingEntity));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oeProxy = s.Load<OverridingEntity>(_idOverridingEntity1);
				Assert.That(NHibernateUtil.IsInitialized(oeProxy), Is.False, "oeProxy is unexpectedly initialized");

				Assert.That(oeProxy.GetHashCode(), Is.EqualTo(oe.GetHashCode()), "Unexpected hashcode");
				Assert.That(
					NHibernateUtil.IsInitialized(oeProxy),
					Is.False,
					"oeProxy is initialized after hashcode test");

				Assert.That(oeProxy.Equals(oe), Is.True, "Unexpected inequality");
				Assert.That(
					NHibernateUtil.IsInitialized(oeProxy),
					Is.False,
					"oeProxy is initialized after equality test");
				t.Commit();
			}
		}

		[Test]
		public void GetHashCodeAndEqualsDependingOnStateDoInitializeProxyOfEntityWithoutFields()
		{
			OverridingEntityChildWithOtherThanIdAccess oeProxy, oe;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oe = s.Get<OverridingEntityChildWithOtherThanIdAccess>(_idOverridingEntityChildWithOtherThanIdAccess1);
				Assert.That(oe.IsProxy(), Is.False, "oe is unexpectedly a proxy");
				t.Commit();
			}
			Sfi.Evict(typeof(OverridingEntityChildWithOtherThanIdAccess));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oeProxy = s.Load<OverridingEntityChildWithOtherThanIdAccess>(_idOverridingEntityChildWithOtherThanIdAccess1);
				Assert.That(NHibernateUtil.IsInitialized(oeProxy), Is.False, "oeProxy is unexpectedly initialized");

				Assert.That(oeProxy.GetHashCode(), Is.EqualTo(oe.GetHashCode()), "Unexpected hashcode");
				Assert.That(
					NHibernateUtil.IsInitialized(oeProxy),
					Is.True,
					"oeProxy is not initialized after hashcode test");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oeProxy = s.Load<OverridingEntityChildWithOtherThanIdAccess>(_idOverridingEntityChildWithOtherThanIdAccess1);
				Assert.That(NHibernateUtil.IsInitialized(oeProxy), Is.False, "oeProxy is unexpectedly initialized");

				Assert.That(oeProxy.Equals(oe), Is.True, "Unexpected inequality");
				Assert.That(
					NHibernateUtil.IsInitialized(oeProxy),
					Is.True,
					"oeProxy is not initialized after equality test");
				t.Commit();
			}
		}

		[Test]
		public void OverridenGetHashCodeAndEqualsDoInitializeProxyOfEntityWithFields()
		{
			// A proxy of an entity having fields must initialize it on GetHashCode or Equals calls, because
			// it cannot know if their base implementation will access those fields or not.
			OverridingEntityWithField oeProxy, oe;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oe = s.Get<OverridingEntityWithField>(_idOverridingEntityWithField1);
				Assert.That(oe.IsProxy(), Is.False, "oe is unexpectedly a proxy");
				t.Commit();
			}
			Sfi.Evict(typeof(OverridingEntityWithField));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oeProxy = s.Load<OverridingEntityWithField>(_idOverridingEntityWithField1);
				Assert.That(NHibernateUtil.IsInitialized(oeProxy), Is.False, "oeProxy is unexpectedly initialized");

				Assert.That(oeProxy.GetHashCode(), Is.EqualTo(oe.GetHashCode()), "Unexpected hashcode");
				Assert.That(
					NHibernateUtil.IsInitialized(oeProxy),
					Is.True,
					"oeProxy is not initialized after hashcode test");
				t.Commit();
			}
			Sfi.Evict(typeof(OverridingEntityWithField));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oeProxy = s.Load<OverridingEntityWithField>(_idOverridingEntityWithField1);
				Assert.That(NHibernateUtil.IsInitialized(oeProxy), Is.False, "oeProxy is unexpectedly initialized");

				Assert.That(oeProxy.Equals(oe), Is.True, "Unexpected inequality");
				Assert.That(
					NHibernateUtil.IsInitialized(oeProxy),
					Is.True,
					"oeProxy is not initialized after equality test");
				t.Commit();
			}

			// Also check inheritance case
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oe = s.Get<OverridingEntityWithFieldChild>(_idOverridingEntityWithFieldChild1);
				Assert.That(oe.IsProxy(), Is.False, "oe child is unexpectedly a proxy");
				t.Commit();
			}
			Sfi.Evict(typeof(OverridingEntityWithFieldChild));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oeProxy = s.Load<OverridingEntityWithFieldChild>(_idOverridingEntityWithFieldChild1);
				Assert.That(NHibernateUtil.IsInitialized(oeProxy), Is.False, "oeProxy child is unexpectedly initialized");

				Assert.That(oeProxy.GetHashCode(), Is.EqualTo(oe.GetHashCode()), "Unexpected child hashcode");
				Assert.That(
					NHibernateUtil.IsInitialized(oeProxy),
					Is.True,
					"oeProxy child is not initialized after hashcode test");
				t.Commit();
			}
			Sfi.Evict(typeof(OverridingEntityWithFieldChild));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				oeProxy = s.Load<OverridingEntityWithFieldChild>(_idOverridingEntityWithFieldChild1);
				Assert.That(NHibernateUtil.IsInitialized(oeProxy), Is.False, "oeProxy is unexpectedly initialized");

				Assert.That(oeProxy.Equals(oe), Is.True, "Unexpected child inequality");
				Assert.That(
					NHibernateUtil.IsInitialized(oeProxy),
					Is.True,
					"oeProxy child is not initialized after equality test");
				t.Commit();
			}
		}

		private static BinaryFormatter GetFormatter()
		{
#if NETFX
			return new BinaryFormatter();
#else
			return new BinaryFormatter
			{
				SurrogateSelector = new SerializationHelper.SurrogateSelector()
			};
#endif
		}
	}
}
