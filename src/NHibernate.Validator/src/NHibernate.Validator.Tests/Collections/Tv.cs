namespace NHibernate.Validator.Tests.Collections
{
	using System;
	using System.Collections.Generic;

	public class Tv
	{
		[NotNull] 
		public String name;

		//[Valid] 
		public IList<Presenter> presenters = new List<Presenter>();

		//[Valid] 
		public IDictionary<String, Show> shows = new Dictionary<String, Show>();

		[Valid]
		public Movie[] movies;
	}
}