using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class UserDto
	{
		public virtual int Id { get; private set; }
		public virtual string Name { get; private set; }
		public virtual int InvalidLoginAttempts { get; set; }
		public virtual string RoleName { get; set; }
		public virtual UserDto2 Dto2 { get; set; }
		public virtual List<UserDto2> Dto2List { get; set; } = new List<UserDto2>();

		public UserDto(int id, string name)
		{
			Id = id;
			Name = name;
			Dto2 = new UserDto2();
		}
	}

	public class UserDto2
	{
		public virtual DateTime RegisteredAt { get; set; }
		public virtual EnumStoredAsInt32 Enum { get; set; }
	}
}
