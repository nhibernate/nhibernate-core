using System;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Stuff.
	/// </summary>
	[Serializable]
	public class Stuff 
	{
		private long _id;
		private FooProxy _foo;
		private MoreStuff _moreStuff;
		//private TimeZone property;  TODO - does this exists in .net???
		
		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public FooProxy Foo
		{
			get { return _foo; }
			set { _foo = value; }
		}

		public MoreStuff MoreStuff
		{
			get { return _moreStuff; }
			set { _moreStuff = value; }
		}

		#region System.Object Members

		public override bool Equals(object obj)
		{
			if(this==obj) return true;

			Stuff rhs = obj as Stuff;
			if(rhs==null) return false;

			return rhs.Id.Equals(this.Id) && rhs.Foo.key.Equals(_foo.key) && rhs.MoreStuff.Equals(_moreStuff) ;
					
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		#endregion
	}
}
