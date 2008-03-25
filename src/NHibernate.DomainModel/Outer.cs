using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Outer.
	/// </summary>
	[Serializable]
	public class Outer
	{
		private OuterKey _id;
		private string _bubu;

		public OuterKey Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Bubu
		{
			get { return _bubu; }
			set { _bubu = value; }
		}

		#region System.Object Members

		public override bool Equals(object obj)
		{
			if (this == obj) return true;

			Outer rhs = obj as Outer;
			if (rhs == null) return false;

			if (_id != null ? !_id.Equals(rhs.Id) : rhs.Id != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			return (_id != null ? _id.GetHashCode() : 0);
		}

		#endregion
	}
}