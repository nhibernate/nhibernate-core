using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Several.
	/// </summary>
	[Serializable]
	public class Several
	{
		private string _id;
		private string _prop;
		private Single _single;
		private string _string;

		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Prop
		{
			get { return _prop; }
			set { _prop = value; }
		}

		public Single Single
		{
			get { return _single; }
			set { _single = value; }
		}

		public string String
		{
			get { return _string; }
			set { _string = value; }
		}

		#region System.Object members

		public override bool Equals(object obj)
		{
			if (this == obj) return true;

			Several rhs = obj as Several;
			if (rhs == null) return false;

			return (rhs.Id.Equals(this.Id) && rhs.String.Equals(this.String));
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		#endregion
	}
}