using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for ClassWithCompositeId.
	/// </summary>
	public class ClassWithCompositeId
	{
		private CompositeId _id;
		private int _oneProperty;

		public ClassWithCompositeId(){}

		public CompositeId Id {
			get {return _id;}
			set {_id = value;}
		}

		public int OneProperty {
			get {return _oneProperty;}
			set {_oneProperty = value;}
		}
	}
}
