using System;


namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for UnsavedType.
	/// </summary>
	public class UnsavedType
	{
		private int _id = 0;
		private string _typeName;

		public UnsavedType()
		{
		}

		public int Id 
		{
			get {return _id;}
			set {_id = value;}
		}

		public string TypeName 
		{
			get {return _typeName;}
			set {_typeName = value;}
		}
	}
}
