using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class EnumCharType<T> : AbstractEnumType
	{

		public EnumCharType() : base(new StringFixedLengthSqlType(1),typeof(T))
		{
		}

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
				throw new HibernateException(string.Format("Can't Parse {0} as {1}", code, ReturnedClass.Name));
			}
		}

		private object GetInstanceFromString(String s)
		{
			if (s.Length == 0) throw new HibernateException(string.Format("Can't Parse empty string as {0}", this.ReturnedClass.Name));

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
					return Enum.Parse(this.ReturnedClass, s, false);
				}
				catch (ArgumentException)
				{
					try
					{
						return Enum.Parse(this.ReturnedClass, s, true);
					}
					catch (ArgumentException ae)
					{
						throw new HibernateException(string.Format("Can't Parse {0} as {1}", s, this.ReturnedClass.Name), ae);
					}
				}
			}
		}

		private object GetInstanceFromChar(Char c)
		{
			Object instance;

			instance = Enum.ToObject(this.ReturnedClass, c);
			if (Enum.IsDefined(this.ReturnedClass, instance)) return instance;

			instance = Enum.ToObject(this.ReturnedClass, Alternate(c));
			if (Enum.IsDefined(this.ReturnedClass, instance)) return instance;

			throw new HibernateException(string.Format("Can't Parse {0} as {1}", c, this.ReturnedClass.Name));
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


		public override void Set(IDbCommand cmd, object value, int index)
		{
			IDataParameter par = (IDataParameter) cmd.Parameters[index];
			if (value == null)
			{
				par.Value = DBNull.Value;
			}
			else
			{
				par.Value = GetValue(value).ToString();
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
			get { return "enumchar - " + this.ReturnedClass.Name; }
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


		public override object FromStringValue(string xml)
		{
			return GetInstance(xml);
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return '\'' + GetValue(value).ToString() + '\'';
		}
	}
}
