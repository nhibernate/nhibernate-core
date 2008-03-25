namespace NHibernate.Tool.hbm2ddl
{
	using System.Collections;
	using System.Data;
	using Connection;

	/// <summary>
	/// Contract for delegates responsible for managing connection used by the
	///hbm2ddl tools.
	/// </summary>
	public interface IConnectionHelper
	{
		/// <summary>
		///  Prepare the helper for use.
		/// </summary>
		void Prepare();

		/// <summary>
		/// Get a reference to the connection we are using.
		/// </summary>
		IDbConnection GetConnection();

		/// <summary>
		/// Release any resources held by this helper.
		/// </summary>
		void Release();
	}

}
