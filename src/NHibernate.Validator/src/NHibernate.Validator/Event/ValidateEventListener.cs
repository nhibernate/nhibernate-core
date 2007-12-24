namespace NHibernate.Validator.Event
{
	using System;
	using System.Collections.Generic;
	using Cfg;
    using NHibernate.Event;
	using NHibernate.Mapping;
    using Property;

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
			throw new NotImplementedException();
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
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="element"></param>
		private void AddSubElement(Property property, ValidatableElement element)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		protected void Validate(object entity, EntityMode mode)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <param name="entity"></param>
		/// <param name="consolidatedInvalidValues"></param>
		private void validateSubElements(ValidatableElement element, Object entity, IList<InvalidValue> consolidatedInvalidValues)
		{
			throw new NotImplementedException();
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
			
			public ValidatableElement(IGetter getter, ClassValidator validator, Type clazz)
			{
				this.getter = getter;
				this.validator = validator;
				this.clazz = clazz;
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
		}

    }
}