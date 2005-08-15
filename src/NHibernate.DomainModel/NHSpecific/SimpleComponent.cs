using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for SimpleComponent.
	/// </summary>
	public class SimpleComponent
	{
		private long m_Key = 0;
		private string _name;
		private string _address;
		private int _count;
		private DateTime _date;
		
		private AuditComponent _audit;
		
		public SimpleComponent() 
		{
			_audit = new AuditComponent();
		}

		public long Key 
		{
			get {return m_Key;}
		}

		public string Name 
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Address 
		{
			get { return _address; }
			set { _address = value; }
		}

		public int Count 
		{
			get { return _count; }
			set { _count = value; }
		}

		public DateTime Date 
		{
			get { return _date; }
			set { _date = value; }
		}

		public AuditComponent Audit 
		{
			get {return _audit;}
			set {_audit = value;}
		}

	}
}
