namespace NHibernate.Validator.Event
{
	using System;
	using System.Collections.Generic;
	using Cfg;
    using NHibernate.Event;
	using NHibernate.Mapping;
    using Property;
	using Util;
	using Validator;

	/// <summary>
	/// Before insert and update, executes the validator framework
    /// </summary>
    public class ValidateEventListener : IPreInsertEventListener, IPreUpdateEventListener, IInitializable
    {
		private bool isInitialized;
		private Dictionary<Type, ValidatableElement> validators = new Dictionary<Type, ValidatableElement>();


		/// <summary>
		/// Initialize the validators, any non significant validators are not kept
		/// </summary>
		/// <param name="cfg"></param>
		public void Initialize(Configuration cfg) 
		{
			if (isInitialized) return;

			IMessageInterpolator interpolator = GetInterpolator(cfg);

			ICollection<PersistentClass> classes = cfg.ClassMappings;

			foreach(PersistentClass clazz in classes)
			{
				Type mappedClass = clazz.MappedClass;
				ClassValidator validator = new ClassValidator(mappedClass, null, null, interpolator, null);
				ValidatableElement element = new ValidatableElement(mappedClass, validator);
				AddSubElement(clazz.IdentifierProperty, element);
				
				foreach(Property property in clazz.PropertyIterator)
				{
					AddSubElement(property, element);
				}

				if (element.SubElements.Count != 0 || element.Validator.HasValidationRules) 
				{
					validators.Add(mappedClass, element);
				}
			}

			isInitialized = true;
		}

		/// <summary>
		/// Get the custom <see cref="IMessageInterpolator"/> from the <see cref="Configuration"/>
		/// </summary>
		/// <param name="cfg"></param>
		/// <returns></returns>
		private IMessageInterpolator GetInterpolator(Configuration cfg)
		{
			string interpolatorString = cfg.GetProperty(NHibernate.Validator.Environment.MESSAGE_INTERPOLATOR_CLASS);
			IMessageInterpolator interpolator = null;

			if(!string.IsNullOrEmpty(interpolatorString))
			{
				try
				{
					Type interpolatorType = ReflectHelper.ClassForName(interpolatorString);
					interpolator = (IMessageInterpolator) Activator.CreateInstance(interpolatorType);
				}
				catch(MissingMethodException ex)
				{
					throw new HibernateException("Public constructor was not found at message interpolator: " + interpolatorString, ex);
				}
				catch(InvalidCastException ex)
				{
					throw new HibernateException(
						"Type does not implement the interface " + typeof(IMessageInterpolator).GetType().Name + ": " + interpolatorString,
						ex);
				}
				catch(Exception ex)
				{
					throw new HibernateException("Unable to instanciate message interpolator: " + interpolatorString, ex);
				}
			}
			return interpolator;
		}

		/// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public bool OnPreInsert(PreInsertEvent @event)
        {
            Validate( @event.Entity, @event.Source.EntityMode );
			return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public bool OnPreUpdate(PreUpdateEvent @event)
        {
			Validate(@event.Entity, @event.Source.EntityMode);
			return false;
        }

		/// <summary>
		/// Add sub elements. Composite Elements.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="element"></param>
		private void AddSubElement(Property property, ValidatableElement element)
		{
			if(property != null && property.IsComposite && !property.BackRef)
			{
				Component component = (Component) property.Value;
				if(component.IsEmbedded) return;

				IPropertyAccessor accesor = PropertyAccessorFactory.GetPropertyAccessor(property, EntityMode.Poco);

				IGetter getter = accesor.GetGetter(element.Clazz, property.Name);

				ClassValidator validator = new ClassValidator(getter.ReturnType);

				ValidatableElement subElement = new ValidatableElement(getter.ReturnType, validator, getter);

				foreach(Property currentProperty in component.PropertyIterator)
				{
					AddSubElement(currentProperty,subElement);
				}

				if (subElement.SubElements.Count != 0 || subElement.Validator.HasValidationRules)
					element.AddSubElement(subElement);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		protected void Validate(object entity, EntityMode mode)
		{
			if (entity == null || !EntityMode.Poco.Equals(mode)) return;

			ValidatableElement element;

			if(isInitialized)
				element = validators[entity.GetType()];
			else
				throw new AssertionFailure("Validator event not initialized");

			if (element == null) return; //No validation to do 

			List<InvalidValue> consolidatedInvalidValues = new List<InvalidValue>();
			ValidateSubElements(element, entity, consolidatedInvalidValues);
			InvalidValue[] invalidValues = element.Validator == null ? null : element.Validator.GetInvalidValues(entity);
			if (invalidValues != null) 
			{
				foreach (InvalidValue invalidValue in invalidValues) 
					consolidatedInvalidValues.Add( invalidValue );
			}
			if (consolidatedInvalidValues.Count > 0)
				throw new InvalidStateException(consolidatedInvalidValues.ToArray(),entity.GetType().Name);
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <param name="entity"></param>
		/// <param name="consolidatedInvalidValues"></param>
		private void ValidateSubElements(ValidatableElement element, Object entity, IList<InvalidValue> consolidatedInvalidValues)
		{
			if ( element != null ) 
			{
				foreach (ValidatableElement subElement in element.SubElements)
				{
					Object component = subElement.Getter.Get(entity);
					
					InvalidValue[] invalidValues = subElement.Validator.GetInvalidValues(component);
					
					foreach (InvalidValue invalidValue in invalidValues) 
						consolidatedInvalidValues.Add( invalidValue );
					
					ValidateSubElements( subElement, component, consolidatedInvalidValues );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Serializable]
		private class ValidatableElement
		{
			private Type clazz;

			private ClassValidator validator;

			private IGetter getter;

			private List<ValidatableElement> subElements = new List<ValidatableElement>();

			public ValidatableElement(Type clazz, ClassValidator validator, IGetter getter)
				:this(clazz,validator)
			{
				this.getter = getter;
			}

			public ValidatableElement(Type clazz, ClassValidator validator)
			{
				this.clazz = clazz;
				this.validator = validator;
			}

			public IList<ValidatableElement> SubElements 
			{
				get { return this.subElements; }
			}

			public void AddSubElement(ValidatableElement subValidatableElement)
			{
				subElements.Add(subValidatableElement);
			}

			public IGetter Getter
			{
				get { return getter; }
			}

			public Type Clazz
			{
				get { return clazz; }
			}

			public ClassValidator Validator
			{
				get { return validator; }
			}
		}

    }
}