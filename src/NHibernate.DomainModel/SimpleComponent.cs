using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for SimpleComponent.
	/// </summary>
	public class SimpleComponent
	{
		private int key;
		private string name;
		private string address;
		private int count;
		private DateTime date;
		
		private AuditComponent audit;
		
		public SimpleComponent() {
			this.audit = new AuditComponent();
		}

		public int Key {
			get {return key;}
			set {key = value;}
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public string Address {
			get { return address; }
			set { address = value; }
		}

		public int Count {
			get { return count; }
			set { count = value; }
		}

		public DateTime Date {
			get { return date; }
			set { date = value; }
		}

		public AuditComponent Audit {
			get {return audit;}
			set {audit = value;}
		}

	}
}
