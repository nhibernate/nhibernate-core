using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3590
{
	public class Entity
	{
		public Entity()
		{
			Dates = new HashSet<DateTime>();
		}

		public virtual Guid Id { get; protected set; }
		public virtual ISet<DateTime> Dates { get; protected set; }

		public override bool Equals(object obj)
		{
			var that = obj as Entity;
			return that != null && that.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}