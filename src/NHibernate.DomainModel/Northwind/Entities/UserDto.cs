using System;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class UserDto
	{
		public virtual int Id { get; private set; }
		public virtual string Name { get; private set; }
		public virtual int InvalidLoginAttempts { get; set; }
		public virtual string RoleName { get; set; }
		public virtual UserDto2 Dto2 { get; set; }

		public UserDto(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class UserDto2
	{
		public virtual DateTime RegisteredAt { get; set; }
		public virtual EnumStoredAsInt32 Enum { get; set; }
	}
}
