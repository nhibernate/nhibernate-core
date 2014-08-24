using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1775
{
	public class Member
	{
		public virtual int Id { get; set;}
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
		public virtual int Roles { get; set; }
	}

	public class DTO
	{
		public DTO(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public int Id { get; set; }
		public string Name { get; set; }
	}
}
