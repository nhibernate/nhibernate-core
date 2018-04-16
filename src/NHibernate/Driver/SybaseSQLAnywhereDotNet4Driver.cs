namespace NHibernate.Driver
{
	/// <summary>
	/// SQL Dialect for SQL Anywhere 12 - for the NHibernate 3.2.0 distribution
	/// Copyright (C) 2011 Glenn Paulley
	/// Contact: http://iablog.sybase.com/paulley
	///
	/// This NHibernate dialect for SQL Anywhere 12 is a contribution to the NHibernate
	/// open-source project. It is intended to be included in the NHibernate 
	/// distribution and is licensed under LGPL.
	///
	/// This library is free software; you can redistribute it and/or
	/// modify it under the terms of the GNU Lesser General Public
	/// License as published by the Free Software Foundation; either
	/// version 2.1 of the License, or (at your option) any later version.
	///
	/// This library is distributed in the hope that it will be useful,
	/// but WITHOUT ANY WARRANTY; without even the implied warranty of
	/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	/// Lesser General Public License for more details.
	///
	/// You should have received a copy of the GNU Lesser General Public
	/// License along with this library; if not, write to the Free Software
	/// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
	/// </summary>
	/// <remarks>
	/// The SybaseSQLAnywhereDotNet4Driver provides a .NET 4 database driver for 
	/// Sybase SQL Anywhere 12 using the versioned ADO.NET driver 
	/// iAnywhere.Data.SQLAnywhere.v4.0.
	/// </remarks>
	public class SybaseSQLAnywhereDotNet4Driver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SybaseSQLAnywhereDotNet4Driver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the iAnywhere.Data.SQLAnywhere.v4.0 assembly is not and can not be loaded.
		/// </exception>
		public SybaseSQLAnywhereDotNet4Driver()
			: base("iAnywhere.Data.SQLAnywhere", "iAnywhere.Data.SQLAnywhere.v4.0", "iAnywhere.Data.SQLAnywhere.SAConnection", "iAnywhere.Data.SQLAnywhere.SACommand")
		{
		}

		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		public override string NamedPrefix
		{
			get { return ":"; }
		}

		public override bool RequiresTimeSpanForTime => true;
	}
}