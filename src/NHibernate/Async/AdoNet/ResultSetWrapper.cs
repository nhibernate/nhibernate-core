﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.AdoNet
{
	public partial class ResultSetWrapper : DbDataReader
	{

		public override Task CloseAsync()
		{
			return rs.CloseAsync();
		}
	}
}