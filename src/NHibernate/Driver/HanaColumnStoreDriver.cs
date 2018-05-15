using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Driver
{
	/// <summary>
	/// Provides a database driver for the SAP HANA column store.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In order to use this driver you must have the assembly <c>Sap.Data.Hana.v4.5.dll</c> available for 
	/// NHibernate to load, including its dependencies (<c>libadonetHDB.dll</c> and <c>libSQLDBCHDB.dll</c> 
	/// are required by the assembly <c>Sap.Data.Hana.v4.5.dll</c> as of the time of this writing).
	/// </para>
	/// <para>
	/// Please check the product's <see href="https://help.sap.com/viewer/0eec0d68141541d1b07893a39944924e/2.0.02/en-US/469dee9e6d611014af70d4e9a9cd6b0a.html">website</see>
	/// for any updates and/or documentation regarding SAP HANA.
	/// </para>
	/// </remarks>
	public class HanaColumnStoreDriver : HanaDriverBase
	{
	}
}
