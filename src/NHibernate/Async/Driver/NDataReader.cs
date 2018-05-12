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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Util;

namespace NHibernate.Driver
{
	public partial class NDataReader : DbDataReader
	{

		/// <summary>
		/// Creates a NDataReader from a <see cref="DbDataReader" />
		/// </summary>
		/// <param name="reader">The <see cref="DbDataReader" /> to get the records from the Database.</param>
		/// <param name="isMidstream"><see langword="true" /> if we are loading the <see cref="DbDataReader" /> in the middle of reading it.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <remarks>
		/// NHibernate attempts to not have to read the contents of an <see cref="DbDataReader"/> into memory until it absolutely
		/// has to.  What that means is that it might have processed some records from the <see cref="DbDataReader"/> and will
		/// pick up the <see cref="DbDataReader"/> midstream so that the underlying <see cref="DbDataReader"/> can be closed 
		/// so a new one can be opened.
		/// </remarks>
		public static async Task<NDataReader> CreateAsync(DbDataReader reader, bool isMidstream, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var dataReader = new NDataReader();
			var resultList = new List<NResult>(2);

			try
			{
				// if we are in midstream of processing a DataReader then we are already
				// positioned on the first row (index=0)
				if (isMidstream)
				{
					dataReader.currentRowIndex = 0;
				}

				// there will be atleast one result 
				resultList.Add(await (NResult.CreateAsync(reader, isMidstream, cancellationToken)).ConfigureAwait(false));

				while (await (reader.NextResultAsync(cancellationToken)).ConfigureAwait(false))
				{
					// the second, third, nth result is not processed midstream
					resultList.Add(await (NResult.CreateAsync(reader, false, cancellationToken)).ConfigureAwait(false));
				}

				dataReader.results = resultList.ToArray();
			}
			catch (OperationCanceledException) { throw; }
			catch (Exception e)
			{
				throw new ADOException("There was a problem converting an DbDataReader to NDataReader", e);
			}
			finally
			{
				reader.Close();
			}
			return dataReader;
		}

		private partial class NResult
		{

			/// <summary>
			/// Initializes a new instance of the NResult class.
			/// </summary>
			/// <param name="reader">The DbDataReader to populate the Result with.</param>
			/// <param name="isMidstream">
			/// <see langword="true" /> if the <see cref="DbDataReader"/> is already positioned on the record
			/// to start reading from.
			/// </param>
			/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
			internal static async Task<NResult> CreateAsync(DbDataReader reader, bool isMidstream, CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var result = new NResult
				{
					schemaTable = reader.GetSchemaTable()
				};

				List<object[]> recordsList = new List<object[]>();
				int rowIndex = 0;

				// if we are in the middle of processing the reader then don't bother
				// to move to the next record - just use the current one.
				while (isMidstream || await (reader.ReadAsync(cancellationToken)).ConfigureAwait(false))
				{
					if (rowIndex == 0)
					{
						for (int i = 0; i < reader.FieldCount; i++)
						{
							string fieldName = reader.GetName(i);
							result.fieldNameToIndex[fieldName] = i;
							result.fieldIndexToName.Add(fieldName);
							result.fieldTypes.Add(reader.GetFieldType(i));
							result.fieldDataTypeNames.Add(reader.GetDataTypeName(i));
						}

						result.colCount = reader.FieldCount;
					}

					rowIndex++;

					object[] colValues = new object[reader.FieldCount];
					reader.GetValues(colValues);
					recordsList.Add(colValues);

					// we can go back to reading a reader like normal and don't need
					// to consider where we started from.
					isMidstream = false;
				}

				result.records = recordsList.ToArray();
				return result;
			}
		}
	}
}
