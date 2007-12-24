namespace NHibernate.Validator.Tests.Collections
{
	using NUnit.Framework;

	[TestFixture]
	public class CollectionFixture
	{
		/// <summary>
		/// Validate Generic Collections
		/// </summary>
		[Test]
		public void TestCollection()
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
		public void TestDictionary()
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
		public void testArray()
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
	}
}