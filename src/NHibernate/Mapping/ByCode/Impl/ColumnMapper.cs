using System;
using System.Runtime.Serialization;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ColumnMapper : IColumnMapper
	{
		private readonly HbmColumn mapping;

		public ColumnMapper(HbmColumn mapping, string memberName)
		{
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			if (memberName == null)
			{
				throw new ArgumentNullException("memberName");
			}
			if (string.Empty == memberName.Trim())
			{
				throw new ArgumentNullException("memberName", "The column name should be a valid not empty name.");
			}
			this.mapping = mapping;
			if (string.IsNullOrEmpty(mapping.name))
			{
				mapping.name = memberName;
			}
		}

		#region Implementation of IColumnMapper

		public void Name(string name)
		{
			mapping.name = name;
		}

		public void Length(int length)
		{
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException("length", "The length should be positive value");
			}
			mapping.length = length.ToString();
		}

		public void Precision(short precision)
		{
			if (precision <= 0)
			{
				throw new ArgumentOutOfRangeException("precision", "The precision should be positive value");
			}
			mapping.precision = precision.ToString();
		}

		public void Scale(short scale)
		{
			if (scale < 0)
			{
				throw new ArgumentOutOfRangeException("scale", "The scale should be positive value");
			}
			mapping.scale = scale.ToString();
		}

		public void NotNullable(bool notnull)
		{
			mapping.notnull = mapping.notnullSpecified = notnull;
		}

		public void Unique(bool unique)
		{
			mapping.unique = mapping.uniqueSpecified = unique;
		}

		public void UniqueKey(string uniquekeyName)
		{
			mapping.uniquekey = uniquekeyName;
		}

		public void SqlType(string sqltype)
		{
			mapping.sqltype = sqltype;
		}

		public void Index(string indexName)
		{
			mapping.index = indexName;
		}

		public void Check(string checkConstraint)
		{
			mapping.check = checkConstraint;
		}

		public void Default(object defaultValue)
		{
			var formatterConverter = new FormatterConverter();
			mapping.@default = defaultValue == null ? "null" : formatterConverter.ToString(defaultValue);
		}

		#endregion
	}
}