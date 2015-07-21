using System;

namespace NHibernate.Test.NHSpecificTest.FileStreamSql2008
{
	public class VendorCatalog
	{
		/// <summary>
		/// Usual Object Id, nothing weird
		/// </summary>
		public virtual int Id { get; set; }
		
		/// <summary>
		/// Simple string property, nothing weird
		/// </summary>
		public virtual string Name { get; set; }
		
		/// <summary>
		/// Represents the mandatory UNIQUEIDENTIFIER ROWGUIDCOL
		/// </summary>
		public virtual Guid CatalogID { get; set; }
		
		/// <summary>
		/// FileStream property
		/// </summary>
		public virtual Byte[] Catalog { get; set; }
	}
}