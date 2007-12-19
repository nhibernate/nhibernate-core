namespace NHibernate.Validator
{
    using System;

    /// <summary>
    /// Responsible for validator message interpolation (variable replacement etc)
    /// this extension point is useful if the call has some contextual informations to
    /// interpolate in validator messages
    /// </summary>
    public interface IMessageInterpolator
    {
        string Interpolate<A>
            (
                String message, 
                IValidator<A> validator, 
                IMessageInterpolator defaultInterpolator
            ) where A : Attribute;

        string Interpolate
            (
                String message,
                IValidator validator,
                IMessageInterpolator defaultInterpolator
            );
    }
}