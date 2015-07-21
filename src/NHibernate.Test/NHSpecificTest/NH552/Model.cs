using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH552
{
	public class Question
	{
		private long id;
		private ISet<Answer> answers;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual ISet<Answer> Answers
		{
			get { return answers; }
			set { answers = value; }
		}
	}

	public class Answer
	{
		private long id;
		private Question question;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Question Question
		{
			get { return question; }
			set { question = value; }
		}
	}
}