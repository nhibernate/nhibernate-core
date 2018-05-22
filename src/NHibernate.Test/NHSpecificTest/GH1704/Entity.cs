using System;

namespace NHibernate.Test.NHSpecificTest.GH1704
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Country { get; set; }
		public virtual string City { get; set; }
		public virtual decimal Budget { get; set; }
	}

	class GroupByEntity
	{
        public GroupByEntity(string country)
        {
            Country = country;
        }
        public virtual string Country { get; set; }
		public virtual string City { get; set; }
	}
}
