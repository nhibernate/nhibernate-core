using System;

namespace NHibernate.Test.CompositeId
{
	/// <summary>
	/// Summary description for ClassWithCompositeId.
	/// </summary>
	public class ClassWithCompositeId
	{
		private Id _id;
		private int _oneProperty;

		public ClassWithCompositeId(){}
	
		public ClassWithCompositeId(Id id ) 
		{
			_id = id;
		}

		public Id Id 
		{
			get {return _id;}
		}

		public int OneProperty 
		{
			get {return _oneProperty;}
			set {_oneProperty = value;}
		}
	}
}
