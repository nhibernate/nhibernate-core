using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection
{
	/// <summary>
	/// Summary description for ElementPropertyMapping.
	/// </summary>
	public class ElementPropertyMapping : IPropertyMapping
	{
		private readonly string[] elementColumns;
		private readonly IType type;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementColumns"></param>
		/// <param name="type"></param>
		public ElementPropertyMapping( string[] elementColumns, IType type)
		{
			this.elementColumns = elementColumns;
			this.type = type;
		}

		#region IPropertyMapping Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public IType ToType( string propertyName )
		{
			if ( propertyName == null || "id".Equals( propertyName ) ) 
			{
				return type;
			}
			else
			{
				throw new QueryException( string.Format( "cannot dereference scalar collection element: {0}", propertyName ) );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string[] ToColumns( string alias, string propertyName )
		{
			if ( propertyName == null || "id".Equals( propertyName ) ) 
			{
				return StringHelper.Qualify( alias, elementColumns );
			}
			else
			{
				throw new QueryException( string.Format( "cannot dereference scalar collection element: {0}", propertyName ) );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IType Type
		{
			get	{ return type; }
		}
		#endregion
	}
}
