using System;
using System.Collections;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Class mapped with optimistic-lock="all"
	/// </summary>
	public class Optimistic
	{
		private int _id;
		private string _string;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string String
		{
			get { return _string; }
			set { _string = value; }
		}
	}
}
