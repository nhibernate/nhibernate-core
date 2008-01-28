using System;

namespace NHibernate.Validator
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	[ValidatorClass(typeof(DigitsValidator))]
	public class DigitsAttribute : Attribute
	{
	}
}