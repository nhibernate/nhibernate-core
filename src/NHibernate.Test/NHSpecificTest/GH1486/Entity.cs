using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.NHSpecificTest.GH1486
{
	public class Company
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Contact Contact { get; set; }
	}
	public class Contact
	{
		public virtual string Phone { get; set; }
		public virtual string Email { get; set; }
		public virtual bool? IsActive { get; set; }
	}
}
