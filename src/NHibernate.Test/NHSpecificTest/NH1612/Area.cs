using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1612
{
	public abstract class Area
	{
		public virtual string Code { get; protected set; }
		public virtual string Name { get; protected set; }
		public virtual int Version { get; protected set; }
		public virtual IDictionary<int, AreaStatistics> Statistics { get; protected set; }

		protected Area() {}

		protected Area(string code, string name)
		{
			Code = code;
			Name = name;
			Statistics = new Dictionary<int, AreaStatistics>();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var other = obj as Area;
			if (ReferenceEquals(obj, null))
			{
				return false;
			}

			return GetType() == other.GetType() && Code == other.Code;
		}

		public override int GetHashCode()
		{
			return (Code ?? string.Empty).GetHashCode() ^ GetType().GetHashCode();
		}
	}
}