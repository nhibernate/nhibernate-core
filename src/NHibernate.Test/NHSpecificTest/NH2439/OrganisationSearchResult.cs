using System;

namespace NHibernate.Test.NHSpecificTest.NH2439
{
	public class OrganisationSearchResult
	{
		protected virtual Guid Id { get; set; }

		public virtual Organisation Organisation { get; set; }
	}
}
