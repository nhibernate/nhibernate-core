using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// Summary description for ElementPropertyMapping.
	/// </summary>
	public class ElementPropertyMapping : IPropertyMapping
	{
		private readonly string[] elementColumns;
		private readonly IType type;

		public ElementPropertyMapping(string[] elementColumns, IType type)
		{
			this.elementColumns = elementColumns;
			this.type = type;
		}

		#region IPropertyMapping Members

		public IType ToType(string propertyName)
		{
			if (propertyName == null || "id".Equals(propertyName))
			{
				return type;
			}
			else
			{
				throw new QueryException(string.Format("cannot dereference scalar collection element: {0}", propertyName));
			}
		}

		public string[] ToColumns(string alias, string propertyName)
		{
			if (propertyName == null || "id".Equals(propertyName))
			{
				return StringHelper.Qualify(alias, elementColumns);
			}
			else
			{
				throw new QueryException(string.Format("cannot dereference scalar collection element: {0}", propertyName));
			}
		}

		public string[] ToColumns(string propertyName)
		{
			throw new System.NotSupportedException("References to collections must be define a SQL alias");
		}

		public IType Type
		{
			get { return type; }
		}

		#endregion
	}
}