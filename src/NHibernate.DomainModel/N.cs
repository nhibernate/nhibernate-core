using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for N.
	/// </summary>
	public class N
	{
		private long uniqueSequence;
		private M parent;
		private string str;

		public N()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public long UniqueSequence
		{
			get { return uniqueSequence; }
			set { uniqueSequence = value; }
		}

		public M Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public string String
		{
			get { return str; }
			set { str = value; }
		}

		public override int GetHashCode()
		{
			return str.GetHashCode();
		}
	}
}