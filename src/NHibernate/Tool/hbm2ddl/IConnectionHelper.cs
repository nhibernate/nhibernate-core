using System.Data.Common;

namespace NHibernate.Tool.hbm2ddl
{
	/// <summary>
	/// Contract for delegates responsible for managing connection used by the hbm2ddl tools.
	/// </summary>
	public partial interface IConnectionHelper
	{
		/// <summary>
		///  Prepare the helper for use.
		/// </summary>
		void Prepare();

		/// <summary>
		/// Get a reference to the connection we are using.
		/// </summary>
		DbConnection Connection { get;}

		/// <summary>
		/// Release any resources held by this helper.
		/// </summary>
		void Release();
	}

}
