//
// NHibernate.Mapping.Attributes
// This product is under the terms of the GNU Lesser General Public License.
//
namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Attribute taking an order position.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.All)]
	[System.Serializable]
	public abstract class BaseAttribute : System.Attribute, System.IComparable
	{
		private int _position;


		/// <summary>
		/// Initializes a new instance of the <see cref="BaseAttribute"/> class.
		/// </summary>
		/// <remarks>
		/// This constructor is only called by classes that derive from <see cref="BaseAttribute"/>.
		/// </remarks>
		/// <param name="position">The position at which the attribute has been declared.</param>
		protected BaseAttribute(int position)
		{
			this._position = position;
		}


		/// <summary> The position at which the attribute has been declared. </summary>
		public int Position
		{
			get { return _position; }
		}


		/// <summary> Compares the current instance with another object of the same type. </summary>
		public int CompareTo(object obj)
		{
			if( ! (obj is BaseAttribute) )
				throw new System.ArgumentException("object is not a BaseAttribute");

			return this.Position.CompareTo((obj as BaseAttribute).Position);
		}
	}
}
