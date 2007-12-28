namespace NHibernate.Validator.Tests.Collections
{
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	public class CollectionFixture
	{
		/// <summary>
		/// Validate Generic Collections
		/// </summary>
		[Test]
		public void Collection()
		{
			Tv tv = new Tv();
			tv.name = "France 2";
			Presenter presNok = new Presenter();
			presNok.name = null;
			Presenter presOk = new Presenter();
			presOk.name = "Thierry Ardisson";
			tv.presenters.Add(presOk);
			tv.presenters.Add(presNok);
			ClassValidator validator = new ClassValidator(typeof(Tv));

			InvalidValue[] values = validator.GetInvalidValues(tv);
			Assert.AreEqual(1, values.Length);
			Assert.AreEqual("presenters[1].name", values[0].PropertyPath);
		}

		/// <summary>
		/// Validate Generic Dictionaries
		/// </summary>
		[Test]
		public void Dictionary()
		{
			Tv tv = new Tv();
			tv.name = "France 2";
			Show showOk = new Show();
			showOk.name = "Tout le monde en parle";
			Show showNok = new Show();
			showNok.name = null;
			tv.shows.Add("Midnight", showOk);
			tv.shows.Add("Primetime", showNok);
			ClassValidator validator = new ClassValidator(typeof(Tv));
			InvalidValue[] values = validator.GetInvalidValues(tv);
			Assert.AreEqual(1, values.Length);
			Assert.AreEqual("shows[1].name", values[0].PropertyPath);
			//Assert.AreEqual("shows['Primetime'].name", values[0].PropertyPath);
		}

		/// <summary>
		/// Validate Arrays
		/// </summary>
		[Test]
		public void Array()
		{
			Tv tv = new Tv();
			tv.name = "France 2";
			Movie movieOk = new Movie();
			movieOk.Name = "Kill Bill";
			Movie movieNok = new Movie();
			movieNok.Name = null;
			tv.movies = new Movie[] {movieOk, null, movieNok};
			ClassValidator validator = new ClassValidator(typeof(Tv));
			InvalidValue[] values = validator.GetInvalidValues(tv);
			Assert.AreEqual(1, values.Length);
			Assert.AreEqual("movies[2].Name", values[0].PropertyPath);
		}

		[Test]
		public void Size()
		{
			HasCollection hc = new HasCollection();
			ClassValidator vtor = new ClassValidator(typeof(HasCollection));

			hc.StringCollection = new List<string>(new string[] {"cuidado", "con", "el", "carancho!"});
			Assert.AreEqual(0, vtor.GetInvalidValues(hc).Length);
			
			hc.StringCollection = new List<string>(new string[] { "pombero" });
			Assert.AreEqual(1, vtor.GetInvalidValues(hc).Length);
			
			hc.StringCollection = new List<string>(new string[] { "Los","Pekes","Chicho","Nino","Nono","Fito" });
			Assert.AreEqual(1, vtor.GetInvalidValues(hc).Length);
		}

		[Test]
		public void SizeWithValid()
		{
			HasShowCollection hsc = new HasShowCollection();
			ClassValidator vtor = new ClassValidator(typeof(HasShowCollection));

			hsc.Shows = new List<Show>( new Show[] {new Show("s1"), new Show("s2")} );
			Assert.AreEqual(0, vtor.GetInvalidValues(hsc).Length);

			hsc.Shows = new List<Show>(new Show[] { new Show("s1")});
			Assert.AreEqual(1, vtor.GetInvalidValues(hsc).Length);

			hsc.Shows = new List<Show>(new Show[] { new Show(null) });
			Assert.AreEqual(2, vtor.GetInvalidValues(hsc).Length);

			hsc.Shows = new List<Show>(new Show[] { new Show("s1"), new Show("s2"), new Show("s3"), new Show("s3") });
			Assert.AreEqual(1, vtor.GetInvalidValues(hsc).Length);

			hsc.Shows = new List<Show>(new Show[] { new Show(null), new Show("s2"), new Show("s3"), new Show("s3") });
			Assert.AreEqual(2, vtor.GetInvalidValues(hsc).Length);
		}

		[Test]
		public void SizeArrayWithValid()
		{
			HasArrayWithValid hsc = new HasArrayWithValid();
			ClassValidator vtor = new ClassValidator(typeof(HasArrayWithValid));

			hsc.Shows = new Show[] {new Show("s1"), new Show("s2")};
			Assert.AreEqual(0, vtor.GetInvalidValues(hsc).Length);

			hsc.Shows = new Show[] { new Show("s1"), new Show(null) };
			Assert.AreEqual(1, vtor.GetInvalidValues(hsc).Length);

			hsc.Shows = new Show[] { new Show("s1"), new Show("s2"), new Show("s3"), new Show("s4"),new Show("s5") };
			Assert.AreEqual(1, vtor.GetInvalidValues(hsc).Length);
		}
	}
}