using System;
using System.Reflection;

namespace NHibernate.Property
{
	public sealed class FieldGetter : IGetter
	{
		private readonly FieldInfo field;
		private readonly System.Type clazz;
		private readonly string name;

		public FieldGetter(FieldInfo field, System.Type clazz, string name) 
		{
			this.field = field;
			this.clazz = clazz;
			this.name = name;
		}

		#region IGetter Members

		public object Get(object target)
		{
			try 
			{
				return field.GetValue(target);
			}
			catch(Exception e) 
			{
				throw new PropertyAccessException( e, "could not get a field value by reflection", false, clazz, name );
			}
		}

		public System.Type ReturnType
		{
			get { return field.FieldType; }
		}

		public string PropertyName
		{
			get { return null; }
		}

		public PropertyInfo Property
		{
			get { return null; }
		}

		#endregion
	}
}
