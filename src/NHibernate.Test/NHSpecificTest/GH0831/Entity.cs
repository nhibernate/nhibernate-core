using System;

namespace NHibernate.Test.NHSpecificTest.GH0831
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual decimal Value { get; set; }

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var that = obj as Entity;

			return (that != null) && Id.Equals(that.Id);
		}
	}
}
