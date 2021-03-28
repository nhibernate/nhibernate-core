using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2174
{
	class DocumentDetail
	{
		private int _id_item;

		protected bool Equals(DocumentDetail other)
		{
			return _id_item == other._id_item && _Version == other._Version && _id_Doc == other._id_Doc && _id_base == other._id_base;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DocumentDetail) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _id_item;
				hashCode = (hashCode * 397) ^ _Version;
				hashCode = (hashCode * 397) ^ _id_Doc;
				hashCode = (hashCode * 397) ^ _id_base;
				return hashCode;
			}
		}

		private int _Version;
		private int _id_Doc;
		private int _id_base;

		public int Id_Item
		{
			get => _id_item;
			set => _id_item = value;
		}

		public int Version
		{
			get => _Version;
			set => _Version = value;
		}

		public int Id_Doc
		{
			get => _id_Doc;
			set => _id_Doc = value;
		}

		public int Id_Base
		{
			get => _id_base;
			set => _id_base = value;
		}
	}

	class Document
	{
		protected bool Equals(Document other)
		{
			return _id_Doc == other._id_Doc && _id_base == other._id_base;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Document) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_id_Doc * 397) ^ _id_base;
			}
		}

		private int _id_Doc;
		private int _id_base;
		public virtual IList<DocumentDetailDocument> RefferedDetails { get; set; } = new List<DocumentDetailDocument>();

		public int Id_Doc
		{
			get => _id_Doc;
			set => _id_Doc = value;
		}

		public int Id_Base
		{
			get => _id_base;
			set => _id_base = value;
		}
	}

	class DocumentDetailDocument : DocumentDetail
	{
		public virtual Document ReferencedDocument { get; set; }
	}
}
