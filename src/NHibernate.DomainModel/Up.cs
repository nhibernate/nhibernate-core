using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Up.
	/// </summary>
	public class Up
	{
		private string id1;
		private long id2;

		public Up()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string Id1
		{
			get { return id1; }
			set { id1 = value; }
		}

		public long Id2
		{
			get { return id2; }
			set { id2 = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		/// <remarks>This is important, otherwise the Identifier and Instance don't match inside SessionImpl</remarks>
		public override bool Equals( object other )
		{
			if ( !(other is Up) )
			{
				return false;
			}
			Up that = other as Up;

			return this.id1.Equals( that.id1 ) && this.id2.Equals( that.id2 );
		}

		public override int GetHashCode()
		{
			return id1.GetHashCode();
		}
	}
}
