using System;
using System.IO;
using log4net;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace NHibernate.Search.Impl
{
	public class LuceneWorker
	{
		private LuceneWorkspace workspace;
		private static ILog log = LogManager.GetLogger(typeof (LuceneWorker));

		public LuceneWorker(LuceneWorkspace workspace)
		{
			this.workspace = workspace;
		}

		public void PerformWork(LuceneWork luceneWork)
		{
			if (luceneWork is AddLuceneWork)
			{
				PerformWork((AddLuceneWork) luceneWork);
			}
			else if (luceneWork is DeleteLuceneWork)
			{
				PerformWork((DeleteLuceneWork) luceneWork);
			}
			else
			{
				throw new AssertionFailure("Unknown work type: " + luceneWork.GetType());
			}
		}

		public void PerformWork(AddLuceneWork work)
		{
			Add(work.EntityClass, work.Id, work.Document);
		}

		private void Add(System.Type entity, object id, Document document)
		{
			if (log.IsDebugEnabled)
				log.Debug("Add to Lucene index: " + entity + "#" + id + ": " + document);
			IndexWriter writer = workspace.GetIndexWriter(entity);
			try
			{
				writer.AddDocument(document);
			}
			catch (IOException e)
			{
				throw new SearchException("Unable to Add to Lucene index: " + entity + "#" + id, e);
			}
		}

		public void PerformWork(DeleteLuceneWork work)
		{
			Remove(work.EntityClass, work.Id);
		}

		private void Remove(System.Type entity, object id)
		{
			/**
		 * even with Lucene 2.1, use of indexWriter to delte is not an option
		 * We can only delete by term, and the index doesn't have a termt that
		 * uniquely identify the entry. See logic below
		 */
			log.DebugFormat("remove from Lucene index: {0}#{1}", entity, id);
			DocumentBuilder builder = workspace.GetDocumentBuilder(entity);
			Term term = builder.GetTerm(id);
			IndexReader reader = workspace.GetIndexReader(entity);
			TermDocs termDocs = null;
			try
			{
				//TODO is there a faster way?
				//TODO include TermDocs into the workspace?
				termDocs = reader.TermDocs(term);
				String entityName = entity.AssemblyQualifiedName;
				while (termDocs.Next())
				{
					int docIndex = termDocs.Doc();
					if (entityName.Equals(reader.Document(docIndex).Get(DocumentBuilder.CLASS_FIELDNAME)))
					{
						//remove only the one of the right class
						//loop all to remove all the matches (defensive code)
						reader.DeleteDocument(docIndex);
					}
				}
			}
			catch (Exception e)
			{
				throw new SearchException("Unable to remove from Lucene index: " + entity + "#" + id, e);
			}
			finally
			{
				if (termDocs != null)
					try
					{
						termDocs.Close();
					}
					catch (IOException e)
					{
						log.Warn("Unable to close termDocs properly", e);
					}
			}
		}
	}
}