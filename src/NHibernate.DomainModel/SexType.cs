using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for SexType.
	/// </summary>
	public class SexType
	{
		private int id;
		private string typeName;
		private string nonpublicString;

		public SexType()
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

		private string NonpublicString 
		{
			get 
			{
				if(typeName.Equals("Male"))
					return "MALE";
				else if(typeName.Equals("Female"))
					return "FEMALE";
				else
					return "UNKNOWN";
			}
			set {nonpublicString = value;}
		}



	}
}
