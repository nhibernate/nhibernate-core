using System;

namespace NHibernate.DomainModel.NHSpecific 
{

	/// <summary>
	/// Used to test how NHibernate handles mappings for type="Object"
	/// and type="Any"
	/// </summary>
	/// <remarks>
	/// This class is used in two hbm.xml files.
	/// </remarks>
	public class BasicObject 
	{
		private int _id;
		private string _name;
		private object _any;
		private object _anyProxy;
		
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public object Any
		{
			get { return _any; }
			set { _any = value; }
		}

		public object AnyWithProxy 
		{
			get { return _anyProxy; }
			set { _anyProxy = value; }
		}

	}

	/// <summary>
	/// Summary description for BasicObject.
	/// </summary>
	[Serializable]
	public class BasicObjectRef 
	{
		private int _id;
		private string _name;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

	}

	public interface IBasicObjectProxy 
	{
		int Id { get; set; }
		string Name { get; set; }
	}

	[Serializable]
	public class BasicObjectProxy : IBasicObjectProxy
	{
		private int _id;
		private string _name;

		#region IBasicObjectProxy Members

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#endregion

	}

}
