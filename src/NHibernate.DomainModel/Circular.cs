using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Circular.
	/// </summary>
	public class Circular
	{
		private string _id;
		private System.Type _clazz;
		private Circular _other;
		private object _anyEntity;

		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}


		public System.Type Clazz 
		{
			get { return _clazz; }
			set { _clazz = value; }
		}

		public Circular Other 
		{
			get { return _other; }
			set { _other = value; }
		}

		public object AnyEntity 
		{
			get { return _anyEntity; }
			set { _anyEntity = value; }
		}
	}
}
