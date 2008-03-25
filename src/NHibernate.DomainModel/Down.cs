using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Down.
	/// </summary>
	public class Down : Up
	{
		private long val;

		public Down()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public long Value
		{
			get { return val; }
			set { val = value; }
		}
	}
}