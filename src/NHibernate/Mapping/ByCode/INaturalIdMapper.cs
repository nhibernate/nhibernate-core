namespace NHibernate.Mapping.ByCode
{
	public interface INaturalIdAttributesMapper
	{
		void Mutable(bool isMutable);
	}

	public interface INaturalIdMapper : INaturalIdAttributesMapper, IBasePlainPropertyContainerMapper {}
}