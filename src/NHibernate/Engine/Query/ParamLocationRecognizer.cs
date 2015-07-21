using System.Collections.Generic;

namespace NHibernate.Engine.Query
{
	/// <summary> 
	/// Implements a parameter parser recognizer specifically for the purpose
	/// of journaling parameter locations. 
	/// </summary>
	public class ParamLocationRecognizer : ParameterParser.IRecognizer
	{
		private readonly Dictionary<string, NamedParameterDescription> namedParameterDescriptions =
			new Dictionary<string, NamedParameterDescription>();

		private readonly List<int> ordinalParameterLocationList = new List<int>();

		/// <summary> 
		/// Convenience method for creating a param location recognizer and
		/// initiating the parse. 
		/// </summary>
		/// <param name="query">The query to be parsed for parameter locations. </param>
		/// <returns> The generated recognizer, with journaled location info. </returns>
		public static ParamLocationRecognizer ParseLocations(string query)
		{
			var recognizer = new ParamLocationRecognizer();
			ParameterParser.Parse(query, recognizer);
			return recognizer;
		}

		/// <summary> 
		/// The dictionary of named parameter locations.
		/// The dictionary is keyed by parameter name.
		/// </summary>
		public IDictionary<string, NamedParameterDescription> NamedParameterDescriptionMap
		{
			get { return namedParameterDescriptions; }
		}

		/// <summary> 
		/// The list of ordinal parameter locations. 
		/// </summary>
		/// <remarks>
		/// The list elements are integers, representing the location for that given ordinal.
		/// Thus OrdinalParameterLocationList[n] represents the location for the nth parameter.
		/// </remarks>
		public List<int> OrdinalParameterLocationList
		{
			get { return ordinalParameterLocationList; }
		}

		#region IRecognizer Members

		public void OutParameter(int position)
		{
			// don't care...
		}

		public void OrdinalParameter(int position)
		{
			ordinalParameterLocationList.Add(position);
		}

		public void NamedParameter(string name, int position)
		{
			GetOrBuildNamedParameterDescription(name, false).Add(position);
		}

		public void JpaPositionalParameter(string name, int position)
		{
			GetOrBuildNamedParameterDescription(name, true).Add(position);
		}

		public void Other(char character)
		{
			// don't care...
		}

		public void Other(string sqlPart)
		{
			// don't care...
		}

		private NamedParameterDescription GetOrBuildNamedParameterDescription(string name, bool jpa)
		{
			NamedParameterDescription desc;
			namedParameterDescriptions.TryGetValue(name, out desc);
			if (desc == null)
			{
				desc = new NamedParameterDescription(jpa);
				namedParameterDescriptions[name] = desc;
			}
			return desc;
		}
		#endregion

		public class NamedParameterDescription
		{
			private readonly bool jpaStyle;
			private readonly List<int> positions = new List<int>();

			public NamedParameterDescription(bool jpaStyle)
			{
				this.jpaStyle = jpaStyle;
			}

			internal void Add(int position)
			{
				positions.Add(position);
			}

			public int[] BuildPositionsArray()
			{
				return positions.ToArray();
			}

			public bool JpaStyle
			{
				get { return jpaStyle; }
			}
		}

	}
}
