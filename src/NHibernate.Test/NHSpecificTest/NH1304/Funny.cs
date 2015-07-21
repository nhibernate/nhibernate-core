namespace NHibernate.Test.NHSpecificTest.NH1304
{
	public class Funny
	{
		private int id;
		private string field;
		private string fieldCamelcase;
		private string _fieldCamelcaseUnderscore;
		private string fieldlowercase;
		private string _fieldlowercaseunderscore;
		private string _FieldPascalcaseUnderscore;
		private string m_FieldPascalcaseMUnderscore;
		private string mFieldPascalcaseM;
#pragma warning disable 649
		private string nosetterCamelcase;
		private string _nosetterCamelcaseUnderscore;
		private string nosetterlowercase;
		private string _nosetterlowercaseunderscore;
		private string _NosetterPascalcaseUnderscore;
		private string m_NosetterPascalcaseMUnderscore;
		private string mNosetterPascalcase;
#pragma warning restore 649

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Field
		{
			get { return field; }
			set { field = value; }
		}

		public virtual string FieldCamelcase
		{
			get { return fieldCamelcase; }
			set { fieldCamelcase = value; }
		}

		public virtual string FieldCamelcaseUnderscore
		{
			get { return _fieldCamelcaseUnderscore; }
			set { _fieldCamelcaseUnderscore = value; }
		}

		public virtual string FieldLowercase
		{
			get { return fieldlowercase; }
			set { fieldlowercase = value; }
		}

		public virtual string FieldLowercaseUnderscore
		{
			get { return _fieldlowercaseunderscore; }
			set { _fieldlowercaseunderscore = value; }
		}

		public virtual string FieldPascalcaseUnderscore
		{
			get { return _FieldPascalcaseUnderscore; }
			set { _FieldPascalcaseUnderscore = value; }
		}

		public virtual string FieldPascalcaseMUnderscore
		{
			get { return m_FieldPascalcaseMUnderscore; }
			set { m_FieldPascalcaseMUnderscore = value; }
		}

		public virtual string FieldPascalcaseM
		{
			get { return mFieldPascalcaseM; }
			set { mFieldPascalcaseM = value; }
		}

		public virtual string NosetterCamelcase
		{
			get { return nosetterCamelcase; }
		}

		public virtual string NosetterCamelcaseUnderscore
		{
			get { return _nosetterCamelcaseUnderscore; }
		}

		public virtual string NosetterLowercase
		{
			get { return nosetterlowercase; }
		}

		public virtual string NosetterLowercaseUnderscore
		{
			get { return _nosetterlowercaseunderscore; }
		}

		public virtual string NosetterPascalcaseUnderscore
		{
			get { return _NosetterPascalcaseUnderscore; }
		}

		public virtual string NosetterPascalcaseMUnderscore
		{
			get { return m_NosetterPascalcaseMUnderscore; }
		}

		public virtual string NosetterPascalcase
		{
			get { return mNosetterPascalcase; }
		}
	}
}
