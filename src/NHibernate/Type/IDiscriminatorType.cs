using System;

namespace NHibernate.Type {
	/// <summary>
	/// An IType that may be used for a discriminator column.
	/// </summary>
	public interface IDiscriminatorType : IIdentifierType, ILiteralType	{
	}
}
