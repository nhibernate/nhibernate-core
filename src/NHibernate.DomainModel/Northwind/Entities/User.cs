﻿using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.DomainModel.Northwind.Entities
{
    [Flags]
    public enum FeatureSet 
    {
        HasThis = 1,
        HasThat = 2,
        HasMore = 4,
        HasAll = 8
    }

	public interface IUser
	{
		int Id { get; set; }
		string Name { get; set; }
		int InvalidLoginAttempts { get; set; }
		DateTime RegisteredAt { get; set; }
		DateTime? LastLoginDate { get; set; }
		UserComponent Component { get; set; }
        FeatureSet Features { get; set; }
		Role Role { get; set; }
		EnumStoredAsString Enum1 { get; set; }
		EnumStoredAsInt32 Enum2 { get; set; }
		IUser CreatedBy { get; set; }
		IUser ModifiedBy { get; set; }
	}

	public class User : IUser, IEntity, INamedEntity
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual int InvalidLoginAttempts { get; set; }

		public virtual DateTime RegisteredAt { get; set; }

		public virtual DateTime? LastLoginDate { get; set; }

		public virtual UserComponent Component { get; set; }

		public virtual Role Role { get; set; }

        public virtual FeatureSet Features { get; set; }

		public virtual EnumStoredAsString Enum1 { get; set; }

		public virtual EnumStoredAsInt32 Enum2 { get; set; }

		public virtual IUser CreatedBy { get; set; }

		public virtual IUser ModifiedBy { get; set; }

		public virtual int NotMapped { get; set; }

		public virtual Role NotMappedRole { get; set; }

		public User() { }

		public User(string name, DateTime registeredAt)
		{
			Name = name;
			RegisteredAt = registeredAt;
		}
	}

	public enum EnumStoredAsString { Unspecified, Small, Medium, Large }

	public enum EnumStoredAsInt32 { Unspecified, High, Low }

	public class EnumStoredAsStringType : EnumStringType
	{
		public EnumStoredAsStringType()
			: base(typeof(EnumStoredAsString), 12) { }

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			if (value is EnumStoredAsString && (EnumStoredAsString)value == EnumStoredAsString.Unspecified)
				base.Set(cmd, null, index, session);
			else
				base.Set(cmd, value, index, session);
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			object obj = base.Get(rs, index, session);
			if (obj == null) return EnumStoredAsString.Unspecified;
			return obj;
		}
	}
}
