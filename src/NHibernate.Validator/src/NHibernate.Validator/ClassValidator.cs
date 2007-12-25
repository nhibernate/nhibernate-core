namespace NHibernate.Validator
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;
	using Iesi.Collections;
	using Iesi.Collections.Generic;
	using Interpolator;
	using Property;
	using Util;

	[Serializable]
	public class ClassValidator
	{
		//TODO: Logging
		//private static Log log = LogFactory.getLog( ClassValidator.class );

		public BindingFlags AnyVisibilityInstance = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

		private Type beanClass;

		private DefaultMessageInterpolatorAggregator defaultInterpolator;

		[NonSerialized] private ResourceManager messageBundle;

		[NonSerialized] private ResourceManager defaultMessageBundle;

		[NonSerialized] private IMessageInterpolator userInterpolator;

		private readonly Dictionary<Type, ClassValidator> childClassValidators;

		private IList<IValidator> beanValidators;

		private IList<IValidator> memberValidators;

		private List<MemberInfo> memberGetters;

		private List<MemberInfo> childGetters;

		private static readonly InvalidValue[] EMPTY_INVALID_VALUE_ARRAY = new InvalidValue[] {};
		private CultureInfo culture;

		/// <summary>
		/// Create the validator engine for this bean type
		/// </summary>
		/// <param name="beanClass"></param>
		public ClassValidator(Type beanClass)
			: this(beanClass, (ResourceManager) null,(CultureInfo)null)
		{
		}

		/// <summary>
		/// Create the validator engine for a particular bean class, using a resource bundle
		/// for message rendering on violation
		/// </summary>
		/// <param name="beanClass">bean type</param>
		/// <param name="resourceManager"></param>
		public ClassValidator(Type beanClass, ResourceManager resourceManager, CultureInfo culture)
			: this(beanClass, resourceManager, culture, null, new Dictionary<Type, ClassValidator>())
		{
		}

		/// <summary>
		/// Create the validator engine for a particular bean class, using a custom message interpolator
		/// for message rendering on violation
		/// </summary>
		/// <param name="beanClass"></param>
		/// <param name="interpolator"></param>
		public ClassValidator(Type beanClass, IMessageInterpolator interpolator)
			: this(beanClass, null,null, interpolator, new Dictionary<Type, ClassValidator>())
		{
		}

		/// <summary>
		/// Not a public API
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="resourceManager"></param>
		/// <param name="culture"></param>
		/// <param name="userInterpolator"></param>
		/// <param name="childClassValidators"></param>
		internal ClassValidator(
			Type clazz,
			ResourceManager resourceManager,
			CultureInfo culture,
			IMessageInterpolator userInterpolator,
			Dictionary<Type, ClassValidator> childClassValidators)
		{
			this.beanClass = clazz;

			this.messageBundle = resourceManager ?? GetDefaultResourceManager();
			this.defaultMessageBundle = GetDefaultResourceManager();
			this.culture = culture;
			this.userInterpolator = userInterpolator;
			this.childClassValidators = childClassValidators;

			//Initialize the ClassValidator
			InitValidator(beanClass, childClassValidators);
		}

		public ClassValidator(Type type, CultureInfo culture)
			: this(type)
		{
			this.culture = culture;
		}

		private ResourceManager GetDefaultResourceManager()
		{
			return new ResourceManager("NHibernate.Validator.Resources.DefaultValidatorMessages",
			                           Assembly.GetExecutingAssembly());
		}

		/// <summary>
		/// Initialize the <see cref="ClassValidator"/> type.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="childClassValidators"></param>
		private void InitValidator(Type clazz, IDictionary<Type, ClassValidator> childClassValidators)
		{
			this.beanValidators = new List<IValidator>();
			this.memberValidators = new List<IValidator>();
			this.memberGetters = new List<MemberInfo>();
			this.childGetters = new List<MemberInfo>();
			this.defaultInterpolator = new DefaultMessageInterpolatorAggregator();
			this.defaultInterpolator.Initialize(messageBundle, defaultMessageBundle, culture);

			//build the class hierarchy to look for members in
			childClassValidators.Add(clazz, this);
			ISet<Type> classes = new HashedSet<Type>();
			AddSuperClassesAndInterfaces(clazz, classes);

			foreach(Type currentClass in classes)
			{
				foreach(Attribute classAttribute in currentClass.GetCustomAttributes(false))
				{
					IValidator validator = CreateValidator(classAttribute);

					if (validator != null)
					{
						beanValidators.Add(validator);
					}

					HandleAggregateAnnotations(classAttribute, null);
				}
			}

			//Check on all selected classes
			foreach(Type currentClass in classes)
			{
				foreach(PropertyInfo currentProperty in currentClass.GetProperties())
				{
					CreateMemberValidator(currentProperty);
					CreateChildValidator(currentProperty);
				}

				foreach(FieldInfo currentField in currentClass.GetFields(AnyVisibilityInstance))
				{
					CreateMemberValidator(currentField);
					CreateChildValidator(currentField);
				}
			}
		}

		public InvalidValue[] GetInvalidValues(object bean)
		{
			return this.GetInvalidValues(bean, new IdentitySet());
		}

		private InvalidValue[] GetInvalidValues(object bean, ISet circularityState)
		{
			if (bean == null || circularityState.Contains(bean))
			{
				return EMPTY_INVALID_VALUE_ARRAY; //Avoid circularity
			}
			else
			{
				circularityState.Add(bean);
			}

			if (!beanClass.IsInstanceOfType(bean))
			{
				throw new ArgumentException("not an instance of: " + bean.GetType());
			}

			List<InvalidValue> results = new List<InvalidValue>();

			//Bean Validation
			foreach(IValidator validator in beanValidators)
			{
				if (!validator.IsValid(bean))
				{
					results.Add(new InvalidValue(Interpolate(validator), beanClass, null, bean, bean));
				}
			}

			//Property & Field Validation
			for(int i = 0; i < memberValidators.Count; i++)
			{
				MemberInfo member = memberGetters[i];

				if (IsPropertyInitialized(bean, member.Name))
				{
					object value = GetMemberValue(bean, member);

					IValidator validator = memberValidators[i];

					if (!validator.IsValid(value))
					{
						results.Add(new InvalidValue(Interpolate(validator), beanClass, member.Name, value, bean));
					}
				}
			}

			//Child validation
			for(int i = 0; i < childGetters.Count; i++)
			{
				MemberInfo member = childGetters[i];

				if (IsPropertyInitialized(bean, member.Name))
				{
					object value = GetMemberValue(bean, member);

					if (value != null && NHibernateUtil.IsInitialized(value))
					{
						MakeChildValidation(value,bean,member,circularityState,results);
					}
				}
			}
			return results.ToArray();
		}

		/// <summary>
		/// Validate the child validation to objects and collections
		/// </summary>
		/// <param name="value">value to validate</param>
		/// <param name="bean"></param>
		/// <param name="member"></param>
		/// <param name="circularityState"></param>
		/// <param name="results"></param>
		public void MakeChildValidation(object value, object bean, MemberInfo member,ISet circularityState, List<InvalidValue> results)
		{
			if (value is IEnumerable)
			{
				MakeChildValidation((IEnumerable) value, bean, member, circularityState, results);
			}
			else
			{
				//Simple Value, Not a Collection
				InvalidValue[] invalidValues = GetClassValidator(value)
					.GetInvalidValues(value, circularityState);

				foreach(InvalidValue invalidValue in invalidValues)
				{
					invalidValue.AddParentBean(bean, member.Name);
					results.Add(invalidValue);
				}
			}
		}

		/// <summary>
		/// Validate the child validation to collections
		/// </summary>
		/// <param name="value"></param>
		/// <param name="bean"></param>
		/// <param name="member"></param>
		/// <param name="circularityState"></param>
		/// <param name="results"></param>
		public void MakeChildValidation(IEnumerable value, object bean, MemberInfo member,ISet circularityState, List<InvalidValue> results)
		{
			if(IsGenericDictionary(value.GetType())) //Generic Dictionary
			{
				int index = 0;
				foreach (object item in value) 
				{
					if (item == null) 
					{
						index++;
						continue;
					}

					IGetter ValueProperty = new BasicPropertyAccessor().GetGetter(item.GetType(), "Value");
					IGetter KeyProperty = new BasicPropertyAccessor().GetGetter(item.GetType(), "Key");

					InvalidValue[] invalidValuesKey = GetClassValidator(ValueProperty.Get(item)).GetInvalidValues(ValueProperty.Get(item), circularityState);
					String indexedPropName = string.Format("{0}[{1}]", member.Name, index);
					
					foreach (InvalidValue invalidValue in invalidValuesKey) 
					{
						invalidValue.AddParentBean(bean, indexedPropName);
						results.Add(invalidValue);
					}

					InvalidValue[] invalidValuesValue = GetClassValidator(KeyProperty.Get(item)).GetInvalidValues(KeyProperty.Get(item), circularityState);
					indexedPropName = string.Format("{0}[{1}]", member.Name, index);

					foreach (InvalidValue invalidValue in invalidValuesValue) 
					{
						invalidValue.AddParentBean(bean, indexedPropName);
						results.Add(invalidValue);
					}

					index++;
				}
			}
			else //Generic collection
			{
				int index = 0;
				foreach(object item in value)
				{
					if (item == null)
					{
						index++;
						continue;
					}

					InvalidValue[] invalidValues = GetClassValidator(item).GetInvalidValues(item, circularityState);

					String indexedPropName = string.Format("{0}[{1}]", member.Name, index);

					index++;

					foreach(InvalidValue invalidValue in invalidValues)
					{
						invalidValue.AddParentBean(bean, indexedPropName);
						results.Add(invalidValue);
					}
				}
			}
		}

		/// <summary>
		/// Get the ClassValidator for the <see cref="Type"/> of the <see cref="value"/>
		/// parametter  from <see cref="childClassValidators"/>. If doesn't exist, a 
		/// new <see cref="ClassValidator"/> is returned.
		/// </summary>
		/// <param name="value">object to get type</param>
		/// <returns></returns>
		private ClassValidator GetClassValidator(object value)
		{
			Type clazz = value.GetType();

			ClassValidator classValidator = childClassValidators[clazz];

			return classValidator ?? new ClassValidator(clazz);
		}

		private bool IsPropertyInitialized(object bean, string propertyName)
		{
			//TODO: This method maybe need to be at NHibernate.Util
			return true;
		}

		/// <summary>
		/// Get the message of the <see cref="IValidator"/> and 
		/// interpolate it.
		/// </summary>
		/// <param name="validator"></param>
		/// <returns></returns>
		private string Interpolate(IValidator validator)
		{
			String message = defaultInterpolator.GetAttributeMessage(validator);

			if (userInterpolator != null)
			{
				return userInterpolator.Interpolate(message, validator, defaultInterpolator);
			}
			else
			{
				return defaultInterpolator.Interpolate(message, validator, null);
			}
		}

		/// <summary>
		/// Create a <see cref="IValidator{A}"/> from a <see cref="ValidatorClassAttribute"/> attribute.
		/// If the attribute is not a <see cref="ValidatorClassAttribute"/> type return null.
		/// </summary>
		/// <param name="attribute">attribute</param>
		/// <returns>the validator for the attribute</returns>
		private IValidator CreateValidator(Attribute attribute)
		{
			try
			{
				ValidatorClassAttribute validatorClass = null;
				object[] AttributesInTheAttribute = attribute.GetType().GetCustomAttributes(typeof(ValidatorClassAttribute), false);

				if (AttributesInTheAttribute.Length > 0)
				{
					validatorClass = (ValidatorClassAttribute) AttributesInTheAttribute[0];
				}

				if (validatorClass == null)
				{
					return null;
				}

				IValidator beanValidator = (IValidator) Activator.CreateInstance(validatorClass.Value);
				beanValidator.Initialize(attribute);
				defaultInterpolator.AddInterpolator(attribute, beanValidator);
				return beanValidator;
			}
			catch(Exception ex)
			{
				throw new ArgumentException("could not instantiate ClassValidator", ex);
			}
		}

		/// <summary>
		/// Create a Validator from a property or field.
		/// </summary>
		/// <param name="member"></param>
		private void CreateMemberValidator(MemberInfo member)
		{
			//TODO: Agregate Annotations
			//bool validatorPresent = false;

			object[] memberAttributes = member.GetCustomAttributes(false);

			foreach(Attribute memberAttribute in memberAttributes)
			{
				IValidator propertyValidator = CreateValidator(memberAttribute);

				if (propertyValidator != null)
				{
					memberValidators.Add(propertyValidator);
					memberGetters.Add(member);
					//validatorPresent = true;
				}
				//bool agrValidPresent = HandleAggregateAnnotations(memberAttribute, member);
				//validatorPresent = validatorPresent || agrValidPresent;
			}
		}

		/// <summary>
		/// Create the validator for the children, who got the <see cref="ValidAttribute"/>
		/// on the fields or properties
		/// </summary>
		/// <param name="member"></param>
		public void CreateChildValidator(MemberInfo member)
		{
			if (!member.IsDefined(typeof(ValidAttribute), false)) return;

			KeyValuePair<Type, Type> clazzDictionary;
			Type clazz = null;

			childGetters.Add(member);

			if (IsGenericDictionary(GetType(member)))
			{
				clazzDictionary = GetGenericTypesOfDictionary(member);
				if(!childClassValidators.ContainsKey(clazzDictionary.Key))
					new ClassValidator(clazzDictionary.Key, messageBundle, culture, userInterpolator, childClassValidators);
				if (!childClassValidators.ContainsKey(clazzDictionary.Value))
					new ClassValidator(clazzDictionary.Value, messageBundle, culture, userInterpolator, childClassValidators);

				return;
			}
			else
			{
				clazz = GetTypeOfMember(member);
			} 
			
			if (!childClassValidators.ContainsKey(clazz))
			{
				new ClassValidator(clazz, messageBundle, culture, userInterpolator, childClassValidators);
			}
		}

		/// <summary>
		/// Get the Generic Arguments of a <see cref="IDictionary{TKey,TValue}"/>
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		private KeyValuePair<Type, Type> GetGenericTypesOfDictionary(MemberInfo member)
		{
			Type clazz = GetType(member);

			return new KeyValuePair<Type, Type> (clazz.GetGenericArguments()[0], clazz.GetGenericArguments()[1]);
		}

		/// <summary>
		/// Get the type of the a Field or Property. 
		/// If is a: Generic Collection or a Array, return the type of the elements.
		/// TODO: Refactor this method to some Utils.
		/// </summary>
		/// <param name="member">MemberInfo, represent a property or field</param>
		/// <returns>type of the member or collection member</returns>
		private Type GetTypeOfMember(MemberInfo member)
		{
			Type clazz = GetType(member);

			if (clazz.IsArray) // Is Array
			{
				return clazz.GetElementType();
			} 
			else if (IsEnumerable(clazz)  && clazz.IsGenericType) //Is Collection Generic  
			{
				return clazz.GetGenericArguments()[0];
			}

			return clazz; //Single type, not a collection/array
		}

		/// <summary>
		/// Indicates if a <see cref="Type"/> is <see cref="IEnumerable"/>
		/// </summary>
		/// <param name="clazz"></param>
		/// <returns>is enumerable or not</returns>
		private bool IsEnumerable(Type clazz)
		{
			return clazz.GetInterface(typeof(IEnumerable).FullName) == null ? false : true;
		}

		private bool IsGenericDictionary(Type clazz)
		{
			if(clazz.IsInterface&&clazz.IsGenericType)
				return typeof(IDictionary<,>).Equals(clazz.GetGenericTypeDefinition());
			else 
				return clazz.GetInterface(typeof(IDictionary<,>).Name) == null ? false : true;
		}

		/// <summary>
		/// Get the <see cref="Type"/> of a <see cref="MemberInfo"/>.
		/// TODO: works only with properties and fields.
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public Type GetType(MemberInfo member)
		{
			switch(member.MemberType)
			{
				case MemberTypes.Field:
					return ((FieldInfo) member).FieldType;

				case MemberTypes.Property:
					return ((PropertyInfo) member).PropertyType;
				default:
					throw new ArgumentException("The argument must be a property or field","member");
			}
		}


		/// <summary>
		/// Get the value of some Property or Field.
		/// TODO: refactor this to some Utils.
		/// </summary>
		/// <param name="bean"></param>
		/// <param name="member"></param>
		/// <returns></returns>
		public object GetMemberValue(object bean, MemberInfo member)
		{
			FieldInfo fi = member as FieldInfo;
			if (fi != null)
				return fi.GetValue(bean);

			PropertyInfo pi = member as PropertyInfo;
			if (pi != null)
				return pi.GetValue(bean, ReflectHelper.AnyVisibilityInstance | BindingFlags.GetProperty, null, null, null);
			
			return null;
		}

		/// <summary>
		/// Aggregate annotations are like the composite validator in EntLib.
		/// </summary>
		/// <param name="attribute"></param>
		/// <param name="member"></param>
		/// <returns></returns>
		private bool HandleAggregateAnnotations(Attribute attribute, MemberInfo member)
		{
			//PropertyInfo propertyValue =  attribute.GetType().GetProperty("Value");
			//TODO: handle with another more elegant way the composite validator attributes
			return false;
		}

		/// <summary>
		/// Add recursively the inheritance tree of types (Classes and Interfaces)
		/// to the parameter <paramref name="classes"/>
		/// </summary>
		/// <param name="clazz">Type to be analyzed</param>
		/// <param name="classes">Collections of types</param>
		private void AddSuperClassesAndInterfaces(Type clazz, ISet<Type> classes)
		{
			//iterate for all SuperClasses
			for(Type currentClass = clazz; currentClass != null; currentClass = currentClass.BaseType)
			{
				if (!classes.Add(clazz))
				{
					return; //Base case for the recursivity
				}

				Type[] interfaces = currentClass.GetInterfaces();

				foreach(Type @interface in interfaces)
				{
					AddSuperClassesAndInterfaces(@interface, classes);
				}
			}
		}

		/// <summary>
		/// Assert a valid Object. A <see cref="InvalidStateException"/> 
		/// will be throw in a Invalid state.
		/// </summary>
		/// <param name="bean">Object to be asserted</param>
		public void AssertValid(object bean)
		{
			InvalidValue[] values = GetInvalidValues(bean);
			if (values.Length > 0)
			{
				throw new InvalidStateException(values);
			}
		}
	}
}