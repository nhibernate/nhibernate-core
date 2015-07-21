using System;

namespace NHibernate.Dialect
{
	/// <summary>
	/// SQL Dialect for SQL Anywhere 11 - for the NHibernate 3.0.0 distribution
	/// Copyright (C) 2010 Glenn Paulley
	/// Contact: http://iablog.sybase.com/paulley
	///
	/// This NHibernate dialect should be considered BETA software.
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
	/// The dialect defaults the following configuration properties:
	/// <list type="table">
	///	<listheader>
	///		<term>Property</term>
	///		<description>Default Value</description>
	///	</listheader>
	///	<item>
	///		<term>connection.driver_class</term>
	///		<description><see cref="NHibernate.Driver.SybaseSQLAnywhereDriver" /></description>
	///	</item>
	///	<item>
	///		<term>prepare_sql</term>
	///		<description><see langword="false" /></description>
	///	</item>
	/// </list>
	/// </remarks>
	public class SybaseSQLAnywhere11Dialect : SybaseSQLAnywhere10Dialect
	{
		public SybaseSQLAnywhere11Dialect() : base()
		{
		}
	}
}