using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH623
{
	public abstract class Element
	{
		protected int elementId;
		protected Document document;

		public virtual int ElementId
		{
			get { return elementId; }
			set { elementId = value; }
		}

		public virtual Document Document
		{
			get { return document; }
			set { document = value; }
		}
	}

	public class Paragraph : Element
	{
		private string font;

		public virtual string Font
		{
			get { return font; }
			set { font = value; }
		}

		public Paragraph()
		{
		}

		public Paragraph(int elementId, Document document, string font)
		{
			this.font = font;
			this.elementId = elementId;
			this.document = document;
		}
	}

	public class Image : Element
	{
		private string img;

		public virtual string Img
		{
			get { return img; }
			set { img = value; }
		}

		public Image()
		{
		}

		public Image(int elementId, Document document, string img)
		{
			this.img = img;
			this.elementId = elementId;
			this.document = document;
		}
	}

	public class Document
	{
		private IList paragraphs;
		private IList images;
		private IList pages;
		private int docId;
		private string name;
		private Review review;

		public Document()
		{
		}

		public Document(int docId, string name)
		{
			this.docId = docId;
			this.name = name;
			paragraphs = new ArrayList();
			images = new ArrayList();
			pages = new ArrayList();
		}

		public virtual IList Paragraphs
		{
			get { return paragraphs; }
			set { paragraphs = value; }
		}

		public virtual IList Images
		{
			get { return images; }
			set { images = value; }
		}

		public virtual IList Pages
		{
			get { return pages; }
			set { pages = value; }
		}

		public virtual int DocId
		{
			get { return docId; }
			set { docId = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual Review Review
		{
			get { return review; }
			set { review = value; }
		}
	}

	public class Page
	{
		private int pageId;
		private Document document;
		private IList pages;
		private bool isActive;

		public Page()
		{
		}

		public Page(int pageId, Document document)
		{
			this.pageId = pageId;
			this.document = document;
			pages = new ArrayList();
		}

		public virtual int PageId
		{
			get { return pageId; }
			set { pageId = value; }
		}

		public virtual Document Document
		{
			get { return document; }
			set { document = value; }
		}

		public virtual IList Pages
		{
			get { return pages; }
			set { pages = value; }
		}

		public virtual bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }
		}
	}

	public class Review
	{
		private int reviewId;
		private Document document;
		private string content;

		public Review()
		{
		}

		public Review(int reviewId, Document document, string content)
		{
			this.reviewId = reviewId;
			this.document = document;
			this.content = content;
		}

		public virtual int ReviewId
		{
			get { return reviewId; }
			set { reviewId = value; }
		}

		public virtual Document Document
		{
			get { return document; }
			set { document = value; }
		}

		public virtual string Content
		{
			get { return content; }
			set { content = value; }
		}
	}
}