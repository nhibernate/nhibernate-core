using System;


namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for UnsavedType.
	/// </summary>
	public class UnsavedType
	{
		private int id = 0;
		private string typeName;

		public UnsavedType()
		{
		}

		public int Id 
		{
			get {return id;}
			set {id = value;}
		}

		public string TypeName 
		{
			get {return typeName;}
			set {typeName = value;}
		}



	}
}
