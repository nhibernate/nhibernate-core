using System;
using System.Reflection;

namespace NHibernate.Property
{
	/// <summary></summary>
	public sealed class FieldGetter : IGetter
	{
		private readonly FieldInfo field;
		private readonly System.Type clazz;
		private readonly string name;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <param name="clazz"></param>
		/// <param name="name"></param>
		public FieldGetter( FieldInfo field, System.Type clazz, string name )
		{
			this.field = field;
			this.clazz = clazz;
			this.name = name;
		}

		#region IGetter Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public object Get( object target )
		{
			try
			{
				return field.GetValue( target );
			}
			catch( Exception e )
			{
				throw new PropertyAccessException( e, "could not get a field value by reflection", false, clazz, name );
			}
		}

		/// <summary></summary>
		public System.Type ReturnType
		{
			get { return field.FieldType; }
		}

		/// <summary></summary>
		public string PropertyName
		{
			get { return null; }
		}

		/// <summary></summary>
		public PropertyInfo Property
		{
			get { return null; }
		}

		#endregion
	}
}