using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2772
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var trip = new Trip();
				trip.Header = "Header1";

				var tp1 = trip.CreateTrackpoint();
				tp1.Lat = 1;
				tp1.Lon = 1;
				var tp2 = trip.CreateTrackpoint();
				tp2.Lat = 2;
				tp2.Lon = 2;
				var tp3 = trip.CreateTrackpoint();
				tp3.Lat = 3;
				tp3.Lon = 3;

				s.Save(trip);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				t.Commit();
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Trip>(rc =>
				{
					rc.Id(x => x.Id, map => map.Generator(Generators.Identity));
					rc.Property(x => x.Header, map => map.NotNullable(true));
					rc.Property(x => x.Image, map => map.Lazy(true)); // This will make the test fail
					rc.Bag(x => x.Trackpoints,
						   map =>
							   {
								   map.Key(x => x.Column("TripId"));
								   map.Access(Accessor.Field);
								   map.Cascade(Mapping.ByCode.Cascade.All);
								   map.BatchSize(10);
								   map.Lazy(CollectionLazy.Lazy);
								   map.Inverse(true);
							   },
						   rel => rel.OneToMany());
				});

			mapper.Class<Trackpoint>(rc =>
				{
					rc.Id(x => x.Id, map => map.Generator(Generators.Identity));
					rc.Property(x => x.Lat, map => map.NotNullable(true));
					rc.Property(x => x.Lon, map => map.NotNullable(true));
					rc.ManyToOne(x => x.Trip,
								 map =>
									 {
										 map.Column("TripId");
										 map.Cascade(Mapping.ByCode.Cascade.None);
									 });
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void Lazy_Collection_Is_Not_Loaded()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var trip = s.Get<Trip>(1);
				Assert.That(trip.Trackpoints.Count(), Is.EqualTo(3));
			}
		}
	}
}
