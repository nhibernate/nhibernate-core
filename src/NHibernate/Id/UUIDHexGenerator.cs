using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that returns a string of length
	/// 32, 36, or 38 depending on the configuration.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// This string will consist of only hex digits.  Optionally, the string
	/// may be generated with enclosing characters and seperators between each component 
	/// of the UUID.  If there are seperators then the string length will be 36.  If a format
	/// that has enclosing brackets is used, then the string length will be 38.
	/// </para>
	/// <para>
	/// The mapping parameters supported are: <c>format</c> and <c>seperator</c>.
	/// </para>
	/// <para>
	/// <c>format</c> is either "N", "D", "B", or "P".  These formats are described in
	/// the <see cref="System.Guid.ToString(String)">Guid.ToString(String)</see> method.
	/// If no <c>format</c> is specified the default is "N".
	/// </para>
	/// <para>
	/// <c>seperator</c> is the char that will replace the "-" if specified.  If no value is
	/// configured then the default seperator for the format will be used.  If the format "D", "B", or
	/// "P" is specified, then the seperator will replace the "-".  If the format is "N" then this
	/// parameter will be ignored.
	/// </para>
	/// <para>
	/// This class is based on <see cref="System.Guid"/>
	/// </para>
	/// </remarks>
	public class UUIDHexGenerator : IIdentifierGenerator, IConfigurable	 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UUIDHexGenerator));

		private string format = FormatWithDigitsOnly;
		private string sep = null;
		
		//"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
		private const string FormatWithDigitsOnly = "N";
		//xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
		private const string FormatWithHyphens = "D";
		//{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
		private const string FormatWithEnclosingBrackets = "B";
		//(xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
		private const string FormatWithEnclosingParens = "P";

		public object Generate(ISessionImplementor cache, object obj) 
		{
			string guidString = Guid.NewGuid().ToString(format);

			if(format!=FormatWithDigitsOnly && sep!=null) 
			{
				return StringHelper.Replace(guidString, "-", sep);
			}
			
			return guidString;
		}

		
		#region IConfigurable Members

		public void Configure(IType type, IDictionary parms, Dialect.Dialect dialect) 
		{
			format = PropertiesHelper.GetString("format", parms, FormatWithDigitsOnly);
			
			if(format!=FormatWithDigitsOnly)
				sep = PropertiesHelper.GetString("seperator", parms, null);
			
		}

		#endregion
	}
}
