using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Properties
{
	/// <summary>
	/// Access the mapped property by using a Field to <c>get</c> and <c>set</c> the value.
	/// </summary>
	/// <remarks>
	/// The <see cref="FieldAccessor"/> is useful when you expose <c>getter</c> and <c>setters</c>
	/// for a Property, but they have extra code in them that shouldn't be executed when NHibernate
	/// is setting or getting the values for loads or saves.
	/// </remarks>
	[Serializable]
	public class FieldAccessor : IPropertyAccessor
	{
		private readonly IFieldNamingStrategy namingStrategy;

		/// <summary>
		/// Initializes a new instance of <see cref="FieldAccessor"/>.
		/// </summary>
		public FieldAccessor()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="FieldAccessor"/>.
		/// </summary>
		/// <param name="namingStrategy">The <see cref="IFieldNamingStrategy"/> to use.</param>
		public FieldAccessor(IFieldNamingStrategy namingStrategy)
		{
			this.namingStrategy = namingStrategy;
		}

		/// <summary>
		/// Gets the <see cref="IFieldNamingStrategy"/> used to convert the name of the
		/// mapped Property in the hbm.xml file to the name of the field in the class.
		/// </summary>
		/// <value>The <see cref="IFieldNamingStrategy"/> or <see langword="null" />.</value>
		public IFieldNamingStrategy NamingStrategy
		{
			get { return namingStrategy; }
		}

		#region IPropertyAccessor Members

		/// <summary>
		/// Create a <see cref="FieldGetter"/> to <c>get</c> the value of the mapped Property
		/// through a <c>Field</c>.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="FieldGetter"/> to use to get the value of the Property from an
		/// instance of the <see cref="System.Type"/>.</returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Field specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			string fieldName = GetFieldName(propertyName);
			if (!Equals(fieldName, propertyName) && !theClass.HasProperty(propertyName))
			{
				// it is a field access that imply the existence of the property
				throw new PropertyNotFoundException(propertyName, fieldName, theClass);
			}
			return new FieldGetter(GetField(theClass, fieldName), theClass, fieldName);
		}

		/// <summary>
		/// Create a <see cref="FieldSetter"/> to <c>set</c> the value of the mapped Property
		/// through a <c>Field</c>.
		/// </summary>
		/// <param name="theClass">The <see cref="System.Type"/> to find the mapped Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to set.</param>
		/// <returns>
		/// The <see cref="FieldSetter"/> to use to set the value of the Property on an
		/// instance of the <see cref="System.Type"/>.
		/// </returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Field for the Property specified by the <c>propertyName</c> using the
		/// <see cref="IFieldNamingStrategy"/> could not be found in the <see cref="System.Type"/>.
		/// </exception>
		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			string fieldName = GetFieldName(propertyName);
			return new FieldSetter(GetField(theClass, fieldName), theClass, fieldName);
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return true; }
		}

		#endregion

		private static FieldInfo GetField(System.Type type, string fieldName, System.Type originalType)
		{
			if (type == null || type == typeof(object))
			{
				// the full inheritance chain has been walked and we could
				// not find the Field
				throw new PropertyNotFoundException(originalType, fieldName);
			}

			FieldInfo field =
				type.GetField(fieldName,
							  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (field == null)
			{
				// recursively call this method for the base Type
				field = GetField(type.BaseType, fieldName, originalType);
			}

			return field;
		}

		/// <summary>
		/// Helper method to find the Field.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Field in.</param>
		/// <param name="fieldName">The name of the Field to find.</param>
		/// <returns>
		/// The <see cref="FieldInfo"/> for the field.
		/// </returns>
		/// <exception cref="PropertyNotFoundException">
		/// Thrown when a field could not be found.
		/// </exception>
		internal static FieldInfo GetField(System.Type type, string fieldName)
		{
			return GetField(type, fieldName, type);
		}

		/// <summary>
		/// Converts the mapped property's name into a Field using 
		/// the <see cref="IFieldNamingStrategy"/> if one exists.
		/// </summary>
		/// <param name="propertyName">The name of the Property.</param>
		/// <returns>The name of the Field.</returns>
		private string GetFieldName(string propertyName)
		{
			if (namingStrategy == null)
			{
				return propertyName;
			}
			else
			{
				return namingStrategy.GetFieldName(propertyName);
			}
		}

		/// <summary>
		/// An <see cref="IGetter"/> that uses a Field instead of the Property <c>get</c>.
		/// </summary>
		[Serializable]
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
			public FieldGetter(FieldInfo field, System.Type clazz, string name)
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
			public object Get(object target)
			{
				try
				{
					return field.GetValue(target);
				}
				catch (Exception e)
				{
					throw new PropertyAccessException(e, "could not get a field value by reflection", false, clazz, name);
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
			/// <value><see langword="null" /> since this is a Field - not a Property.</value>
			public string PropertyName
			{
				get { return null; }
			}

			/// <summary>
			/// Gets the <see cref="PropertyInfo"/> for the Property.
			/// </summary>
			/// <value><see langword="null" /> since this is a Field - not a Property.</value>
			public MethodInfo Method
			{
				get { return null; }
			}

			public object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
			{
				return Get(owner);
			}

			#endregion

			public void Emit(ILGenerator il)
			{
				il.Emit(OpCodes.Ldfld, field);
			}
		}

		/// <summary>
		/// An <see cref="IGetter"/> that uses a Field instead of the Property <c>set</c>.
		/// </summary>
		[Serializable]
		public sealed class FieldSetter : ISetter, IOptimizableSetter
		{
			private readonly FieldInfo field;
			private readonly System.Type clazz;
			private readonly string name;

			/// <summary>
			/// Initializes a new instance of <see cref="FieldSetter"/>.
			/// </summary>
			/// <param name="clazz">The <see cref="System.Type"/> that contains the Field to use for the Property <c>set</c>.</param>
			/// <param name="field">The <see cref="FieldInfo"/> for reflection.</param>
			/// <param name="name">The name of the Field.</param>
			public FieldSetter(FieldInfo field, System.Type clazz, string name)
			{
				this.field = field;
				this.clazz = clazz;
				this.name = name;
			}

			#region ISetter Members

			/// <summary>
			/// Sets the value of the Field on the object.
			/// </summary>
			/// <param name="target">The object to set the Field value in.</param>
			/// <param name="value">The value to set the Field to.</param>
			/// <exception cref="PropertyAccessException">
			/// Thrown when there is a problem setting the value in the target.
			/// </exception>
			public void Set(object target, object value)
			{
				try
				{
					field.SetValue(target, value);
				}
				catch (ArgumentException ae)
				{
					// if I'm reading the msdn docs correctly this is the only reason the ArgumentException
					// would be thrown, but it doesn't hurt to make sure.
					if (field.FieldType.IsInstanceOfType(value) == false)
					{
						// add some details to the error message - there have been a few forum posts an they are 
						// all related to an ISet and IDictionary mixups.
						string msg =
							String.Format("The type {0} can not be assigned to a field of type {1}", value.GetType().ToString(),
														field.FieldType.ToString());
						throw new PropertyAccessException(ae, msg, true, clazz, name);
					}
					else
					{
						throw new PropertyAccessException(ae, "ArgumentException while setting the field value by reflection", true, clazz,
																							name);
					}
				}
				catch (Exception e)
				{
					throw new PropertyAccessException(e, "could not set a field value by reflection", true, clazz, name);
				}
			}

			/// <summary>
			/// Gets the name of the Property.
			/// </summary>
			/// <value><see langword="null" /> since this is a Field - not a Property.</value>
			public string PropertyName
			{
				get { return null; }
			}

			/// <summary>
			/// Gets the <see cref="PropertyInfo"/> for the Property.
			/// </summary>
			/// <value><see langword="null" /> since this is a Field - not a Property.</value>
			public MethodInfo Method
			{
				get { return null; }
			}

			#endregion

			public void Emit(ILGenerator il)
			{
				il.Emit(OpCodes.Stfld, field);
			}
		}
	}
}
