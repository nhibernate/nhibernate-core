namespace NHibernate.Type
{
	/// <summary>
	/// An IType that may be used for a discriminator column.
	/// </summary>
	/// <remarks>
	/// This interface contains no new methods but does require that an
	/// <see cref="IType"/> that will be used in a discriminator column must implement
	/// both the <see cref="IIdentifierType"/> and <see cref="ILiteralType"/> interfaces.
	/// </remarks>
	public interface IDiscriminatorType : IIdentifierType, ILiteralType
	{
	}
}