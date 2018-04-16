using System;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH732
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void CaseInsensitiveId()
		{
			using (ISession session = OpenSession())
			{
				User user = new User();
				user.UserName = @"Domain\User";
				session.Save(user);
				Role role = new Role();
				role.RoleName = "ADMINS";
				session.Save(role);
				session.Flush();
			}
			using (ISession session = OpenSession())
			{
				User user = (User) session.Load(typeof(User), "DOMAIN\\USER");
				Role role = (Role) session.Load(typeof(Role), "Admins");
				UserToRole userToRole = new UserToRole();
				userToRole.User = user;
				userToRole.Role = role;
				session.Save(userToRole);
				session.Flush();
			}

			using (ISession session = OpenSession())
			{
				User user = session.Get<User>("domain\\user");
				Assert.AreEqual(1, user.UserToRoles.Count);
			}
			
			using (ISession session = OpenSession())
			{
				session.Delete("from System.Object o");
				session.Flush();
			}
		}
	}

	public class CaseInsensitiveStringType : IEnhancedUserType
	{
		public SqlType[] SqlTypes
		{
			get { return new SqlType[] { new StringSqlType() }; }
		}

		public System.Type ReturnedType
		{
			get { return typeof (string); }
		}

		
		public new bool Equals(object x, object y)
		{
			return StringComparer.InvariantCultureIgnoreCase.Equals((string)x, (string)y);
		}

		public int GetHashCode(object x)
		{
			return StringComparer.InvariantCultureIgnoreCase.GetHashCode((string)x);
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			int ordinal = rs.GetOrdinal(names[0]);
			string s = rs.GetString(ordinal);
			return s;
			/* Using this will work, because we normalize the returned value
			 * if(s==null)
				return null;
			return s.ToLower();
			 */
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var parameter = cmd.Parameters[index];
			parameter.Value = value ?? DBNull.Value;
		}

		public object DeepCopy(object value)
		{
			return value.ToString();
		}

		public bool IsMutable
		{
			get { return true; }
		}

		public object FromXMLString(string xml)
		{
			return xml;
		}

		public string ObjectToSQLString(object value)
		{
			return "'" + value.ToString() + "'";
		}

		public string ToXMLString(object value)
		{
			return value.ToString();
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}
	}
	
	public class User
	{
		string userName;

		ISet<UserToRole> userToRoles;

		public virtual ISet<UserToRole> UserToRoles
		{
			get { return userToRoles; }
			set { userToRoles = value; }
		}

		public virtual string UserName
		{
			get { return userName; }
			set { userName = value; }
		}
	}

	public class UserToRole
	{
		int id;

		User user;

		Role role;

		int type;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual User User
		{
			get { return user; }
			set { user = value; }
		}

		public virtual Role Role
		{
			get { return role; }
			set { role = value; }
		}

		public virtual int Type
		{
			get { return type; }
			set { type = value; }
		}
	}

	public class Role
	{
		string roleName;
		ISet<UserToRole> roleToUsers;

		public virtual string RoleName
		{
			get { return roleName; }
			set { roleName = value; }
		}

		public virtual ISet<UserToRole> RoleToUsers
		{
			get { return roleToUsers; }
			set { roleToUsers = value; }
		}
	}
}
