using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Properties
{
	/// <summary>
	/// Accesses mapped property values via a get/set pair, which may be nonpublic.
	/// The default (and recommended strategy).
	/// </summary>
	[Serializable]
	public class BasicPropertyAccessor : IPropertyAccessor
	{
		#region IPropertyAccessor Members

		/// <summary>
		/// Create a <see cref="BasicGetter"/> for the mapped property.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="BasicGetter"/> to use to get the value of the Property from an
		/// instance of the <see cref="System.Type"/>.</returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Property specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		public IGetter GetGetter(System.Type type, string propertyName)
		{
			BasicGetter result = GetGetterOrNull(type, propertyName);
			if (result == null)
			{
				throw new PropertyNotFoundException(type, propertyName, "getter");
			}
			return result;
		}

		/// <summary>
		/// Create a <see cref="BasicSetter"/> for the mapped property.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="BasicSetter"/> to use to set the value of the Property on an
		/// instance of the <see cref="System.Type"/>.
		/// </returns>
		/// <exception cref="PropertyNotFoundException" >
		/// Thrown when a Property specified by the <c>propertyName</c> could not
		/// be found in the <see cref="System.Type"/>.
		/// </exception>
		public ISetter GetSetter(System.Type type, string propertyName)
		{
			BasicSetter result = GetSetterOrNull(type, propertyName);
			if (result == null)
			{
				throw new PropertyNotFoundException(type, propertyName, "setter");
			}
			return result;
		}

		public bool CanAccessThroughReflectionOptimizer => true;

		#endregion

		/// <summary>
		/// Helper method to find the Property <c>get</c>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to get.</param>
		/// <returns>
		/// The <see cref="BasicGetter"/> for the Property <c>get</c> or <see langword="null" />
		/// if the Property could not be found.
		/// </returns>
		internal static BasicGetter GetGetterOrNull(System.Type type, string propertyName)
		{
			if (type == typeof(object) || type == null)
			{
				// the full inheritance chain has been walked and we could
				// not find the Property get
				return null;
			}

			PropertyInfo property =
				type.GetProperty(propertyName,
								 BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

			if (property != null && property.CanRead)
			{
				return new BasicGetter(type, property, propertyName);
			}
			else
			{
				// recursively call this method for the base Type
				BasicGetter getter = GetGetterOrNull(type.BaseType, propertyName);

				// didn't find anything in the base class - check to see if there is 
				// an explicit interface implementation.
				if (getter == null)
				{
					System.Type[] interfaces = type.GetInterfaces();
					for (int i = 0; getter == null && i < interfaces.Length; i++)
					{
						getter = GetGetterOrNull(interfaces[i], propertyName);
					}
				}
				return getter;
			}
		}

		/// <summary>
		/// Helper method to find the Property <c>set</c>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to find the Property in.</param>
		/// <param name="propertyName">The name of the mapped Property to set.</param>
		/// <returns>
		/// The <see cref="BasicSetter"/> for the Property <c>set</c> or <see langword="null" />
		/// if the Property could not be found.
		/// </returns>
		internal static BasicSetter GetSetterOrNull(System.Type type, string propertyName)
		{
			if (type == typeof(object) || type == null)
			{
				// the full inheritance chain has been walked and we could
				// not find the Property get
				return null;
			}

			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			PropertyInfo property = type.GetProperty(propertyName, bindingFlags);

			if (property != null && property.CanWrite)
			{
				return new BasicSetter(type, property, propertyName);
			}

			// recursively call this method for the base Type
			BasicSetter setter = GetSetterOrNull(type.BaseType, propertyName);

			// didn't find anything in the base class - check to see if there is 
			// an explicit interface implementation.
			if (setter == null)
			{
				System.Type[] interfaces = type.GetInterfaces();
				for (int i = 0; setter == null && i < interfaces.Length; i++)
				{
					setter = GetSetterOrNull(interfaces[i], propertyName);
				}
			}
			return setter;
		}

		/// <summary>
		/// An <see cref="IGetter"/> for a Property <c>get</c>.
		/// </summary>
		[Serializable]
		public sealed class BasicGetter : IGetter, IOptimizableGetter
		{
			[NonSerialized]
			private System.Type _clazz;
			private SerializableSystemType _serializableClazz;
			[NonSerialized]
			private PropertyInfo _property;
			private SerializablePropertyInfo _serializableProperty;
			private readonly string _propertyName;

			/// <summary>
			/// Initializes a new instance of <see cref="BasicGetter"/>.
			/// </summary>
			/// <param name="clazz">The <see cref="System.Type"/> that contains the Property <c>get</c>.</param>
			/// <param name="property">The <see cref="PropertyInfo"/> for reflection.</param>
			/// <param name="propertyName">The name of the Property.</param>
			public BasicGetter(System.Type clazz, PropertyInfo property, string propertyName)
			{
				_clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
				_property = property ?? throw new ArgumentNullException(nameof(property));
				_propertyName = propertyName;
			}

			[OnSerializing]
			private void OnSerializing(StreamingContext context)
			{
				_serializableClazz = SerializableSystemType.Wrap(_clazz);
				_serializableProperty = SerializablePropertyInfo.Wrap(_property);
			}

			[OnDeserialized]
			private void OnDeserialized(StreamingContext context)
			{
				_clazz = _serializableClazz?.GetSystemType();
				_property = _serializableProperty?.Value;
			}

			public PropertyInfo Property => _property;

			#region IGetter Members

			/// <summary>
			/// Gets the value of the Property from the object.
			/// </summary>
			/// <param name="target">The object to get the Property value from.</param>
			/// <returns>
			/// The value of the Property for the target.
			/// </returns>
			public object Get(object target)
			{
				try
				{
					return _property.GetValue(target, Array.Empty<object>());
				}
				catch (Exception e)
				{
					throw new PropertyAccessException(e, "Exception occurred", false, _clazz, _propertyName);
				}
			}

			/// <summary>
			/// Gets the <see cref="System.Type"/> that the Property returns.
			/// </summary>
			/// <value>The <see cref="System.Type"/> that the Property returns.</value>
			public System.Type ReturnType => _property.PropertyType;

			/// <summary>
			/// Gets the name of the Property.
			/// </summary>
			/// <value>The name of the Property.</value>
			public string PropertyName => _property.Name;

			/// <summary>
			/// Gets the <see cref="PropertyInfo"/> for the Property.
			/// </summary>
			/// <value>
			/// The <see cref="PropertyInfo"/> for the Property.
			/// </value>
			public MethodInfo Method => _property.GetGetMethod(true);

			public object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
			{
				return Get(owner);
			}

			#endregion

			public void Emit(ILGenerator il)
			{
				MethodInfo method = Method;
				if (method == null)
				{
					throw new PropertyNotFoundException(_clazz, _property.Name, "getter");
				}
				il.EmitCall(OpCodes.Callvirt, method, null);
			}
		}

		/// <summary>
		/// An <see cref="ISetter"/> for a Property <c>set</c>.
		/// </summary>
		[Serializable]
		public sealed class BasicSetter : ISetter, IOptimizableSetter
		{
			[NonSerialized]
			private System.Type _clazz;
			private SerializableSystemType _serializableClazz;
			[NonSerialized]
			private PropertyInfo _property;
			private SerializablePropertyInfo _serializableProperty;
			private readonly string _propertyName;

			/// <summary>
			/// Initializes a new instance of <see cref="BasicSetter"/>.
			/// </summary>
			/// <param name="clazz">The <see cref="System.Type"/> that contains the Property <c>set</c>.</param>
			/// <param name="property">The <see cref="PropertyInfo"/> for reflection.</param>
			/// <param name="propertyName">The name of the mapped Property.</param>
			public BasicSetter(System.Type clazz, PropertyInfo property, string propertyName)
			{
				_clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
				_property = property ?? throw new ArgumentNullException(nameof(property));
				_propertyName = propertyName;
			}

			[OnSerializing]
			private void OnSerializing(StreamingContext context)
			{
				_serializableClazz = SerializableSystemType.Wrap(_clazz);
				_serializableProperty = SerializablePropertyInfo.Wrap(_property);
			}

			[OnDeserialized]
			private void OnDeserialized(StreamingContext context)
			{
				_clazz = _serializableClazz?.GetSystemType();
				_property = _serializableProperty?.Value;
			}

			public PropertyInfo Property => _property;

			#region ISetter Members

			/// <summary>
			/// Sets the value of the Property on the object.
			/// </summary>
			/// <param name="target">The object to set the Property value in.</param>
			/// <param name="value">The value to set the Property to.</param>
			/// <exception cref="PropertyAccessException">
			/// Thrown when there is a problem setting the value in the target.
			/// </exception>
			public void Set(object target, object value)
			{
				try
				{
					_property.SetValue(target, value, Array.Empty<object>());
				}
				catch (ArgumentException ae)
				{
					// if I'm reading the msdn docs correctly this is the only reason the ArgumentException
					// would be thrown, but it doesn't hurt to make sure.
					if (_property.PropertyType.IsInstanceOfType(value) == false)
					{
						// add some details to the error message - there have been a few forum posts an they are 
						// all related to an ISet and IDictionary mixups.
						var msg = string.Format(
							"The type {0} can not be assigned to a property of type {1}", value.GetType(),
							_property.PropertyType);
						throw new PropertyAccessException(ae, msg, true, _clazz, _propertyName);
					}
					else
					{
						throw new PropertyAccessException(
							ae, "ArgumentException while setting the property value by reflection", true,
							_clazz, _propertyName);
					}
				}
				catch (Exception e)
				{
					throw new PropertyAccessException(e, "could not set a property value by reflection", true, _clazz, _propertyName);
				}
			}

			/// <summary>
			/// Gets the name of the mapped Property.
			/// </summary>
			/// <value>The name of the mapped Property or <see langword="null" />.</value>
			public string PropertyName => _property.Name;

			/// <summary>
			/// Gets the <see cref="PropertyInfo"/> for the mapped Property.
			/// </summary>
			/// <value>The <see cref="PropertyInfo"/> for the mapped Property.</value>
			public MethodInfo Method => _property.GetSetMethod(true);

			public System.Type Type => _property.PropertyType;

			#endregion

			public void Emit(ILGenerator il)
			{
				MethodInfo method = Method;
				if (method == null)
				{
					throw new PropertyNotFoundException(_clazz, _property.Name, "setter");
				}
				il.EmitCall(OpCodes.Callvirt, method, null);
			}
		}
	}
}
