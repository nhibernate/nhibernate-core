using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Custom.
	/// </summary>
	public class Custom : ICloneable
	{
		long _id;
		string _name;

		public long Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name 
		{
			get { return _name; }
			set { _name = value; }
		}
		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
			//TODO: h2.0.3 had a try-catch block -> not sure where it was used.
		}

		#endregion

		
	}
}
