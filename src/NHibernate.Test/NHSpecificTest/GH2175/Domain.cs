using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2175
{
	public class Concept
	{
		public int Id { get; set; }
		public string DisplayName { get; set; }
		public ISet<ConceptCodeMapping> Mappings { get; private set; } = new HashSet<ConceptCodeMapping>();
	}

	public class ConceptCodeMapping
	{
		/// <summary>
		/// Constructs empty <see cref="ConceptCodeMapping" /> instance. Intended for use by persistence 
		/// frameworks only. 
		/// </summary>
		protected ConceptCodeMapping()
		{ }

		public ConceptCodeMapping(ConceptCodeRelationship relationship, ConceptCode code)
		{
			this.Relationship = relationship;
			this.Code = code;
		}

		public ConceptCodeRelationship Relationship { get; protected set; }
		public ConceptCode Code { get; protected set; }

		public static bool operator ==(ConceptCodeMapping left, ConceptCodeMapping right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

			return left.Relationship == right.Relationship
				&& Equals(left.Code, right.Code);
		}

		public static bool operator !=(ConceptCodeMapping left, ConceptCodeMapping right)
		{
			return !(left == right);
		}

		public bool Equals(ConceptCodeMapping other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return this == obj as ConceptCodeMapping;
		}

		public override int GetHashCode()
		{
			return this.Code?.GetHashCode() ?? 0;
		}
	}

	public enum ConceptCodeRelationship
	{
		Undefined = 0,
		SameAs = '=',
		NarrowerThan = '<',
		BroaderThan = '>'
	}

	public class ConceptCode : IEquatable<ConceptCode>
	{
		protected ConceptCode()
		{ }

		public ConceptCode(string codeSource, string value)
		{
			this.CodeSource = codeSource;
			this.Value = value;
		}

		public int Id { get; set; }
		public string CodeSource { get; protected set; }
		public string Value { get; protected set; }

		public static bool operator ==(ConceptCode left, ConceptCode right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

			return left.Value == right.Value
				&& left.CodeSource == right.CodeSource;
		}

		public static bool operator !=(ConceptCode left, ConceptCode right)
		{
			return !(left == right);
		}

		public bool Equals(ConceptCode other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return this == obj as ConceptCode;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((this.CodeSource?.GetHashCode() ?? 0) * 397)
					^ (this.Value?.GetHashCode() ?? 0);
			}
		}

		public override string ToString()
		{
			return this.CodeSource + "::" + this.Value;
		}
	}
}
