namespace NHibernate.Validator.Interpolator
{
    using System;
    using System.Collections.Generic;
    using System.Resources;

    [Serializable]
    public class DefaultMessageInterpolatorAggregator : IMessageInterpolator
    {
        private IDictionary<IValidator, DefaultMessageInterpolator> interpolators = new Dictionary<IValidator, DefaultMessageInterpolator>();
        
        //transient but repopulated by the object owing a reference to the interpolator
	    [NonSerialized] private  ResourceManager messageBundle;
	    
        //transient but repopulated by the object owing a reference to the interpolator
	    [NonSerialized] private ResourceManager defaultMessageBundle;

        public void Initialize(ResourceManager messageBundle, ResourceManager defaultMessageBundle)
        {
           	this.messageBundle = messageBundle;
	        this.defaultMessageBundle = defaultMessageBundle;

	        //useful when we deserialize
            foreach(DefaultMessageInterpolator interpolator in interpolators.Values)
            {
                interpolator.Initialize( messageBundle, defaultMessageBundle );
            }
        }

        public string Interpolate<A>(string message, IValidator<A> validator, IMessageInterpolator defaultInterpolator)
            where A : Attribute
        {
            return Interpolate(message, (IValidator)validator, defaultInterpolator);
        }

        public string Interpolate(string message, IValidator validator, IMessageInterpolator defaultInterpolator)
        {
            DefaultMessageInterpolator defaultMessageInterpolator = interpolators[validator];
            
            if (defaultMessageInterpolator == null) return message;

            return defaultMessageInterpolator.Interpolate(message, validator, defaultInterpolator);
        }

        public void AddInterpolator(Attribute attribute, IValidator validator)
        {
            
        }

        public string GetAttributeMessage(IValidator validator)
        {
            DefaultMessageInterpolator defaultMessageInterpolator = interpolators[validator];

            string message = defaultMessageInterpolator != null
                                 ? defaultMessageInterpolator.GetAttributeMessage()
                                 : null;

            if (message == null) throw new AssertionFailure("Validator not registred to the messageInterceptorAggregator");
            return message;
        }
    }
}