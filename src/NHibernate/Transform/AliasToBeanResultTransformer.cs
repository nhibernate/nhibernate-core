using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Properties;

namespace NHibernate.Transform
{
	/// <summary>
	/// Result transformer that allows to transform a result to 
	/// a user specified class which will be populated via setter  
	/// methods or fields matching the alias names. 
	/// </summary>
	/// <example>
	/// <code>
	/// IList resultWithAliasedBean = s.CreateCriteria(typeof(Enrollment))
	/// 			.CreateAlias("Student", "st")
	/// 			.CreateAlias("Course", "co")
	/// 			.SetProjection( Projections.ProjectionList()
	/// 					.Add( Projections.Property("co.Description"), "CourseDescription" )
	/// 			)
	/// 			.SetResultTransformer( new AliasToBeanResultTransformer(typeof(StudentDTO)) )
	/// 			.List();
	/// 
	/// StudentDTO dto = (StudentDTO)resultWithAliasedBean[0];
	/// </code>
	/// </example>
	[Serializable]
	public class AliasToBeanResultTransformer : IResultTransformer
	{
		private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private readonly System.Type resultClass;
		private ISetter[] setters;
		private readonly IPropertyAccessor propertyAccessor;
		private readonly ConstructorInfo constructor;
		private Dictionary<string, System.Type> propertyTypes;
		private Dictionary<string, System.Type> fieldTypes;

		public AliasToBeanResultTransformer(System.Type resultClass)
		{
			if (resultClass == null)
			{
				throw new ArgumentNullException("resultClass");
			}
			this.resultClass = resultClass;

			constructor = resultClass.GetConstructor(flags, null, System.Type.EmptyTypes, null);

			// if resultClass is a ValueType (struct), GetConstructor will return null... 
			// in that case, we'll use Activator.CreateInstance instead of the ConstructorInfo to create instances
			if (constructor == null && resultClass.IsClass)
			{
				throw new ArgumentException("The target class of a AliasToBeanResultTransformer need a parameter-less constructor",
				                            "resultClass");
			}

			propertyAccessor =
				new ChainedPropertyAccessor(new[]
				                            	{
				                            		PropertyAccessorFactory.GetPropertyAccessor(null),
				                            		PropertyAccessorFactory.GetPropertyAccessor("field")
				                            	});

			// Get and store the public and non-public type for the instance properties
			var properties = resultClass.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			propertyTypes = new Dictionary<string, System.Type>(properties.Length);

			foreach (var property in properties)
			{
				propertyTypes.Add(property.Name, property.PropertyType);
			}

			// Get and store the public and non-public type for the instance fields
			var fields = resultClass.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			fieldTypes = new Dictionary<string, System.Type>(fields.Length);

			foreach (var field in fields)
			{
				fieldTypes.Add(field.Name, field.FieldType);
			}
		}

		public object TransformTuple(object[] tuple, String[] aliases)
		{
			if (aliases == null)
			{
				throw new ArgumentNullException("aliases");
			}
			object result;

			try
			{
				if (setters == null)
				{
					setters = new ISetter[aliases.Length];
					for (int i = 0; i < aliases.Length; i++)
					{
						string alias = aliases[i];
						if (alias != null)
						{
							setters[i] = propertyAccessor.GetSetter(resultClass, alias);
						}
					}
				}
				
				// if resultClass is not a class but a value type, we need to use Activator.CreateInstance
				result = resultClass.IsClass
				         	? constructor.Invoke(null)
				         	: Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(resultClass, true);

				for (int i = 0; i < aliases.Length; i++)
				{
					if (setters[i] != null)
					{
						System.Type type = null;
						var value = tuple[i];

						if (propertyTypes.ContainsKey(aliases[i]))
						{
							type = propertyTypes[aliases[i]];
						}
						else if (fieldTypes.ContainsKey(aliases[i]))
						{
							type = fieldTypes[aliases[i]];
						}

						var valueType = value.GetType();

						// NH-3290 fix works for convertible types
						if (type != null && typeof(IConvertible).IsAssignableFrom(valueType))
						{
							if (type.IsEnum)
							{
								type = Enum.GetUnderlyingType(type);
							}

							if (type != valueType)
							{
								value = Convert.ChangeType(value, type);
							}
						}

						setters[i].Set(result, value);
					}
				}
			}
			catch (InstantiationException e)
			{
				throw new HibernateException("Could not instantiate result class: " + resultClass.FullName, e);
			}
			catch (MethodAccessException e)
			{
				throw new HibernateException("Could not instantiate result class: " + resultClass.FullName, e);
			}

			return result;
		}

		public IList TransformList(IList collection)
		{
			return collection;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as AliasToBeanResultTransformer);
		}

		public bool Equals(AliasToBeanResultTransformer other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.resultClass, resultClass);
		}

		public override int GetHashCode()
		{
			return resultClass.GetHashCode();
		}
	}
}