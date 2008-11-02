using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class EnumCharType<T> : ImmutableType, IDiscriminatorType
	{

		public EnumCharType() : base(new StringFixedLengthSqlType(1))
		{
			if (typeof(T).IsEnum)
			{
				this.enumClass = typeof(T);
			}
			else
			{
				throw new MappingException(enumClass.Name + " did not inherit from System.Enum");
			}
		}
		private readonly System.Type enumClass;

		public virtual object GetInstance(object code)
		{
			if (code is String)
			{
				return GetInstanceFromString((String) code);
			}
			else if (code is Char)
			{
				return GetInstanceFromChar((Char) code);
			}
			else
			{
				throw new HibernateException(string.Format("Can't Parse {0} as {1}", code, enumClass.Name));
			}
		}

		private object GetInstanceFromString(String s)
		{
			if (s.Length == 0) throw new HibernateException(string.Format("Can't Parse empty string as {0}", enumClass.Name));

			if (s.Length == 1)
			{
				//String representation of underlying char value e.g. "R"
				return GetInstanceFromChar(s[0]);
			}
			else
			{
				//Name of enum value e.g. "Red"
				try
				{
					return Enum.Parse(enumClass, s, false);
				}
				catch (ArgumentException)
				{
					try
					{
						return Enum.Parse(enumClass, s, true);
					}
					catch (ArgumentException ae)
					{
						throw new HibernateException(string.Format("Can't Parse {0} as {1}", s, enumClass.Name), ae);
					}
				}
			}
		}

		private object GetInstanceFromChar(Char c)
		{
			Object instance;

			instance = Enum.ToObject(enumClass, c);
			if (Enum.IsDefined(enumClass, instance)) return instance;

			instance = Enum.ToObject(enumClass, Alternate(c));
			if (Enum.IsDefined(enumClass, instance)) return instance;

			throw new HibernateException(string.Format("Can't Parse {0} as {1}", c, enumClass.Name));
		}

		private Char Alternate(Char c)
		{
			return Char.IsUpper(c) ? Char.ToLower(c) : Char.ToUpper(c);
		}

		/// <summary>
		/// Converts the given enum instance into a basic type.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public virtual object GetValue(object instance)
		{
			if (instance == null)
			{
				return null;
			}
			else
			{
				return (Char) (Int32) instance;
			}
		}

		public override System.Type ReturnedClass
		{
			get { return enumClass; }
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			IDataParameter par = (IDataParameter) cmd.Parameters[index];
			if (value == null)
			{
				par.Value = DBNull.Value;
			}
			else
			{
				par.Value = ((Char) (Int32) (value)).ToString();
			}
		}

		public override object Get(IDataReader rs, int index)
		{
			object code = rs[index];
			if (code == DBNull.Value || code == null)
			{
				return null;
			}
			else
			{
				return GetInstance(code);
			}
		}

		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override string Name
		{
			get { return "enumchar - " + enumClass.Name; }
		}

		public override string ToString(object value)
		{
			return (value == null) ? null : GetValue(value).ToString();
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			if (cached == null)
			{
				return null;
			}
			else
			{
				return GetInstance(cached);
			}
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return (value == null) ? null : GetValue(value);
		}

		public virtual object StringToObject(string xml)
		{
			return (string.IsNullOrEmpty(xml)) ? null : FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return GetInstance(xml);
		}

		public virtual string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + GetValue(value).ToString() + '\'';
		}
	}
}