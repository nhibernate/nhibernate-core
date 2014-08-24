namespace NHibernate.Intercept
{
	public interface IFieldInterceptorAccessor
	{
		IFieldInterceptor FieldInterceptor { get; set; }
	}
}