using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for MoreStuff.
	/// </summary>
	[Serializable]
	public class MoreStuff
	{
		private string _stringId;
		private int _intId;
		// <bag> mapping
		private IList<Stuff> _stuffs;
		private string _name;


		public string StringId
		{
			get { return _stringId; }
			set { _stringId = value; }
		}

		public int IntId
		{
			get { return _intId; }
			set { _intId = value; }
		}

		public IList<Stuff> Stuffs
		{
			get { return _stuffs; }
			set { _stuffs = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#region System.Object Members

		public override bool Equals(object obj)
		{
			if (this == obj) return true;

			MoreStuff rhs = obj as MoreStuff;
			if (rhs == null) return false;

			return (rhs.IntId == this.IntId && rhs.StringId.Equals(this.StringId));
		}


		public override int GetHashCode()
		{
			return _stringId.GetHashCode();
		}

		#endregion
	}
}