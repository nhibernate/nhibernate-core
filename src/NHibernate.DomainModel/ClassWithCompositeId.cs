using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for ClassWithCompositeId.
	/// </summary>
	public class ClassWithCompositeId
	{
		private CompositeId id;
		private int oneProperty;

		public ClassWithCompositeId(){}

		public CompositeId Id {
			get {return this.id;}
			set {this.id = value;}
		}

		public int OneProperty {
			get {return this.oneProperty;}
			set {this.oneProperty = value;}
		}


	}
}
