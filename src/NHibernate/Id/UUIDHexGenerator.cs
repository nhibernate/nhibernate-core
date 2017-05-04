using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that returns a string of length
	/// 32, 36, or 38 depending on the configuration.  
	/// </summary>
	/// <remarks>
	/// <p>
	///	This id generation strategy is specified in the mapping file as 
	///	<code>
	///	&lt;generator class="uuid.hex"&gt;
	///		&lt;param name="format"&gt;format_string&lt;/param&gt;
	///		&lt;param name="separator"&gt;separator_string&lt;/param&gt;
	///	&lt;/generator&gt;
	///	</code>
	/// </p>
	/// <p>
	/// The <c>format</c> and <c>separator</c> parameters are optional.
	/// </p>
	/// <p>
	/// The identifier string will consist of only hex digits.  Optionally, the identifier string
	/// may be generated with enclosing characters and separators between each component 
	/// of the UUID.  If there are separators then the string length will be 36.  If a format
	/// that has enclosing brackets is used, then the string length will be 38.
	/// </p>
	/// <p>
	/// <c>format</c> is either 
	/// "N" (<c>xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx</c>), 
	/// "D" (<c>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</c>), 
	/// "B" (<c>{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}</c>), 
	/// or "P" (<c>(xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)</c>).  These formats are described in
	/// the <see cref="System.Guid.ToString(string)">Guid.ToString(String)</see> method.
	/// If no <c>format</c> is specified the default is "N".
	/// </p>
	/// <p>
	/// <c>separator</c> is the char that will replace the "-" if specified.  If no value is
	/// configured then the default separator for the format will be used.  If the format "D", "B", or
	/// "P" is specified, then the separator will replace the "-".  If the format is "N" then this
	/// parameter will be ignored.
	/// </p>
	/// <p>
	/// This class is based on <see cref="System.Guid"/>
	/// </p>
	/// </remarks>
	public partial class UUIDHexGenerator : IIdentifierGenerator, IConfigurable
	{
		protected string format = FormatWithDigitsOnly;
		protected string sep;

		//"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
		protected const string FormatWithDigitsOnly = "N";

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generate a new <see cref="string"/> for the identifier using the "uuid.hex" algorithm.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>The new identifier as a <see cref="string"/>.</returns>
		public virtual object Generate(ISessionImplementor session, object obj)
		{
			string guidString = GenerateNewGuid();

			if (format != FormatWithDigitsOnly && sep != null)
			{
				return StringHelper.Replace(guidString, "-", sep);
			}

			return guidString;
		}

		#endregion

		#region IConfigurable Members

		/// <summary>
		/// Configures the UUIDHexGenerator by reading the value of <c>format</c> and
		/// <c>separator</c> from the <c>parms</c> parameter.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		public virtual void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			format = PropertiesHelper.GetString("format", parms, FormatWithDigitsOnly);

			if (format != FormatWithDigitsOnly)
			{
				sep = PropertiesHelper.GetString("separator", parms, null);
			}
		}

		#endregion

		/// <summary>
		/// Generate a Guid into a string using the <c>format</c>.
		/// </summary>
		/// <returns>A new Guid string</returns>
		protected virtual string GenerateNewGuid()
		{
			return Guid.NewGuid().ToString(format);
		}
	}
}