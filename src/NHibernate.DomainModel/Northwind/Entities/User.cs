using System;
using System.Data;
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
	}

	public class User : IUser
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

		public override void Set(IDbCommand cmd, object value, int index)
		{
			if (value is EnumStoredAsString && (EnumStoredAsString)value == EnumStoredAsString.Unspecified)
				base.Set(cmd, null, index);
			else
				base.Set(cmd, value, index);
		}

		public override object Get(IDataReader rs, int index)
		{
			object obj = base.Get(rs, index);
			if (obj == null) return EnumStoredAsString.Unspecified;
			return obj;
		}
	}
}
