using System;
using System.Reflection;

namespace NHibernate.Property
{
	public sealed class FieldSetter : ISetter
	{
		private readonly FieldInfo field;
		private readonly System.Type clazz;
		private readonly string name;

		public FieldSetter(FieldInfo field, System.Type clazz, string name) 
		{
			this.field = field;
			this.clazz = clazz;
			this.name = name;
		}

		#region ISetter Members

		public void Set(object target, object value)
		{
			try 
			{
				field.SetValue( target, value );
			}
			catch(Exception e) 
			{
				throw new PropertyAccessException(e, "could not set a field value by reflection", true, clazz, name);
			}
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
