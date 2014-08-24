using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NHibernate.Test.NHSpecificTest.NH2985
{
	[Serializable]
	public class ClassA
	{
		public virtual Guid Id { get; set; }
		public virtual String Name { get; set; }

		public virtual IList<WebImage> Childs { get; set; }

		
	}

	/// <summary>
	/// Rappresenta una immagine nel web.
	/// </summary>
	public class WebImage
	{
		public WebImage()
		{

			OldPosition = -1; //di base un immagine è nuova.
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual Guid Id
		{
			get;
			set;
		}

		/// <summary>
		/// I dati grezzi dell'immagine, è il conenuto binario che io ottengo
		/// scaricando l'immagine
		/// </summary>
		/// <value>The image data.</value>
		public virtual Byte[] ImageData { get; set; }

		/// <summary>
		/// Se ci sta la pagina che contiene l'immagine.
		/// </summary>
		public virtual Uri ContainingPage { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public virtual String AltText { get; set; }

		/// <summary>
		/// il testo che sta intorno.
		/// </summary>
		public virtual String SurroundingText { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public virtual Int32 Width { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public virtual Int32 Height { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public virtual Int32? ImgEval { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public virtual Int32? ContextEval { get; set; }

		/// <summary>
		/// L'immagine è trattata talvolta come una vera e propria rilevazione per cui
		/// ha comunque al suo interno la possibilità di avere uno stato associato.
		/// </summary>
		public virtual Int32 Status { get; set; }

		/// <summary>
		/// queste sono note relative all'immagine
		/// </summary>
		public virtual String Note { get; set; }

		/// <summary>
		/// alcune immagini possono essere bookmarkate.
		/// </summary>
		public virtual Boolean Bookmarked { get; set; }

		/// <summary>
		/// La posizione nella ricerca precedente, -1 se è la prima apparizione.
		/// </summary>
		public virtual Int32 OldPosition { get; protected internal set; }

		/// <summary>
		/// Un'immagine che non è più tornata 
		/// </summary>
		protected internal virtual Boolean OldImage { get; set; }

		private Bitmap _bitmap;

		/// <summary>
		/// Gets the bitmap.
		/// </summary>
		/// <value>The bitmap.</value>
		public virtual Bitmap Bitmap
		{
			get
			{
				return _bitmap ?? CreateBitmap();
			}
		}

		/// <summary>
		/// La url dell'immagine.
		/// </summary>
		/// <value>The image URL.</value>
		public virtual String ImageUrl { get; set; }

		private Bitmap CreateBitmap()
		{
			try
			{
				if (ImageData != null)
				{
					using (MemoryStream stream = new MemoryStream(ImageData))
					{
						_bitmap = (Bitmap)Bitmap.FromStream(stream);
					}
				}
				return _bitmap;
			}
			catch (Exception)
			{
				return null;
			}

		}
	}

}
