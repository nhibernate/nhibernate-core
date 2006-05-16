using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Property
{
	/// <summary>
	/// An <see cref="IGetter"/> that uses a Field instead of the Property <c>get</c>.
	/// </summary>
	public sealed class FieldGetter : IGetter, IOptimizableGetter
	{
		private readonly FieldInfo field;
		private readonly System.Type clazz;
		private readonly string name;

		/// <summary>
		/// Initializes a new instance of <see cref="FieldGetter"/>.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> that contains the field to use for the Property <c>get</c>.</param>
		/// <param name="field">The <see cref="FieldInfo"/> for reflection.</param>
		/// <param name="name">The name of the Field.</param>
		public FieldGetter( FieldInfo field, System.Type clazz, string name )
		{
			this.field = field;
			this.clazz = clazz;
			this.name = name;
		}

		#region IGetter Members

		/// <summary>
		/// Gets the value of the Field from the object.
		/// </summary>
		/// <param name="target">The object to get the Field value from.</param>
		/// <returns>
		/// The value of the Field for the target.
		/// </returns>
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

		/// <summary>
		/// Gets the <see cref="System.Type"/> that the Field returns.
		/// </summary>
		/// <value>The <see cref="System.Type"/> that the Field returns.</value>
		public System.Type ReturnType
		{
			get { return field.FieldType; }
		}

		/// <summary>
		/// Gets the name of the Property.
		/// </summary>
		/// <value><c>null</c> since this is a Field - not a Property.</value>
		public string PropertyName
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the <see cref="PropertyInfo"/> for the Property.
		/// </summary>
		/// <value><c>null</c> since this is a Field - not a Property.</value>
		public MethodInfo Method
		{
			get { return null; }
		}

		#endregion

		public void Emit( ILGenerator il )
		{
			il.Emit( OpCodes.Ldfld, field ); 
		}
	}
}