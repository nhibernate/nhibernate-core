namespace NHibernate.Linq.Expressions
{
	public enum NHExpressionType
	{
		QuerySource = 100,
		Select,
		Projection,
		SimpleProperty,
		ComponentProperty,
		OneToManyProperty,
		OneToOneProperty,
		Parameter,
		Order,
	}
}