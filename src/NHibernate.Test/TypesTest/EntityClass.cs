using System;

namespace NHibernate.Test.TypesTest
{
	public class EntityClass
	{
		private int _id;

		public EntityClass() {}

		public EntityClass(int _id)
		{
			this._id = _id;
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}

	public class ComparableEntityClass : EntityClass, IComparable
	{
		public ComparableEntityClass() {}

		#region IComparable Members

		public ComparableEntityClass(int _id) : base(_id) {}

		public int CompareTo(object obj)
		{
			EntityClass other = obj as EntityClass;

			if (other == null)
			{
				return 1;
			}

			return Id.CompareTo(other.Id);
		}

		#endregion
	}
}
