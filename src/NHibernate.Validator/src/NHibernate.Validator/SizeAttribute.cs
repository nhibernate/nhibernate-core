namespace NHibernate.Validator
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	[ValidatorClass(typeof(SizeValidator))]
	public class SizeAttribute : Attribute
	{
	}
}