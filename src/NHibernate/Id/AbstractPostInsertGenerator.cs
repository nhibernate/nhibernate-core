using NHibernate.Engine;
using NHibernate.Id.Insert;

namespace NHibernate.Id
{
	public abstract class AbstractPostInsertGenerator : IPostInsertIdentifierGenerator
	{
		/// <summary>
		/// The IdentityGenerator for autoincrement/identity key generation. 
		/// </summary>
		/// <param name="s">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity the id is being generated for.</param>
		/// <returns>
		/// <c>IdentityColumnIndicator</c> Indicates to the Session that identity (i.e. identity/autoincrement column)
		/// key generation should be used.
		/// </returns>
		public object Generate(ISessionImplementor s, object obj)
		{
			return IdentifierGeneratorFactory.PostInsertIndicator;
		}

		#region IPostInsertIdentifierGenerator Members

		public abstract IInsertGeneratedIdentifierDelegate GetInsertGeneratedIdentifierDelegate(
			IPostInsertIdentityPersister persister, ISessionFactoryImplementor factory, bool isGetGeneratedKeysEnabled);

		#endregion
	}
}
