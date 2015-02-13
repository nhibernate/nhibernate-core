using System;
using System.Linq;
using System.Text;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.DdlGen.Model
{
    public class ColumnModel
    {
        public ColumnModel()
        {
            Nullable = true;
        }
        public const int DefaultLength = 255;
        public const int DefaultPrecision = 19;
        public const int DefaultScale = 2;

        private int? _length;
        private int? _precision;
        private int? _scale;


        public string SqlType { get; set; }
        public SqlType SqlTypeCode { get; set; }
        public ColumnCheckModel CheckConstraint { get; set; }
        public string DefaultValue { get; set; }

        public bool Nullable { get; set; }

        public string Name { get; set; }


        public string Comment { get; set; }

        public bool HasDefaultValue
        {
            get { return !String.IsNullOrWhiteSpace(DefaultValue); }
        }

        public bool HasCheckConstraint
        {
            get { return CheckConstraint != null && !String.IsNullOrWhiteSpace(CheckConstraint.Expression); }
        }

        public bool IsCaracteristicsDefined()
        {
            return IsLengthDefined() || IsPrecisionDefined();
        }

        public bool IsPrecisionDefined()
        {
            return _precision.HasValue || _scale.HasValue;
        }

        public bool IsLengthDefined()
        {
            return _length.HasValue;
        }

        public int Precision
        {
            get { return _precision.GetValueOrDefault(DefaultPrecision); }
            set { _precision = value; }
        }

        public int Scale
        {
            get { return _scale.GetValueOrDefault(DefaultScale); }
            set { _scale = value; }
        }
        public int Length
        {
            get { return _length.GetValueOrDefault(DefaultLength); }
            set { _length = value; }
        }

        public string GetSqlType(Dialect.Dialect dialect)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(SqlType))
                {
                    return SqlType;
                }
                else if (IsCaracteristicsDefined())
                {
                    // NH-1070 (the size should be 0 if the precision is defined)
                    return dialect.GetTypeName(SqlTypeCode, (!IsPrecisionDefined()) ? Length : 0, Precision, Scale);
                }
                else
                {
                    return dialect.GetTypeName(SqlTypeCode);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public StringBuilder AppendTemporaryTableColumnDefinition(StringBuilder sb, Dialect.Dialect dialect)
        {
            var columnName = BacktickQuoteUtil.ApplyDialectQuotes(Name, dialect);
            return sb.AppendFormat("{0} {1} {2}", columnName, GetSqlType(dialect),
                Nullable ? dialect.NullColumnString : "not null");
        }
        public StringBuilder AppendColumnDefinitinon(StringBuilder sb, Dialect.Dialect dialect,
            bool isIdentityColumn)
        {
            var columnName = BacktickQuoteUtil.ApplyDialectQuotes(Name, dialect);
            sb.Append(columnName).Append(" ");
            if (isIdentityColumn)
            {
                if (dialect.HasDataTypeInIdentityColumn)
                {
                    sb.Append(GetSqlType(dialect)).Append(" ");
                }
                sb.Append(dialect.GetIdentityColumnString(SqlTypeCode.DbType));
            }
            else
            {
                sb.Append(GetSqlType(dialect));
                sb.Append(Nullable ? dialect.NullColumnString : " not null");
            }

            if (HasDefaultValue)
            {
                sb.Append(" default ").Append(DefaultValue).Append(" ");
            }


            if (dialect.SupportsColumnCheck && HasCheckConstraint)
            {
                sb.Append(" check( ").Append(CheckConstraint.Expression).Append(") ");
            }

            return sb;
        }

        public void SetPrecsisionNullable(int? value)
        {
            _precision = value;
        }

        public void SetLengthNullable(int? value)
        {
            _length = value;
        }

        public void SetScaleNullable(int? value)
        {
            _scale = value;
        }
    }
}
