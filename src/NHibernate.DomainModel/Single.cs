using System;
using System.Collections;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Single.
	/// </summary>
	[Serializable]
	public class Single 
	{
		private string _id;
		private string _prop;
		private string _string;
		private IDictionary _several;

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

		public string String
		{
			get { return _string; }
			set { _string = value; }
		}

		public IDictionary Several
		{
			get { return _several; }
			set { _several = value; }
		}

		#region System.Object members
		
		public override bool Equals(object obj)
		{
			if(this==obj) return true;

			Single rhs = obj as Single;
			if(rhs==null) return false;

			return ( rhs.Id.Equals(this.Id) && rhs.String.Equals(this.String) );
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		#endregion
	}
}
