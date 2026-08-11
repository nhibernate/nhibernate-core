using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.GH3539;

public class CardInfo
{
	private readonly ISet<string> _oldCards;

	protected CardInfo() { }

	public CardInfo(params string[] cards)
	{
		_oldCards = cards.ToHashSet();
	}

	public virtual ISet<string> GetCardsCopy()
	{
		return _oldCards.ToHashSet();
	}

	public override bool Equals(object obj)
	{
		if (obj is null)
		{
			return false;
		}
		if (ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		var other = (CardInfo) obj;
		return _oldCards.SetEquals(other._oldCards);
	}

	public override int GetHashCode()
	{
		var hashCode = new HashCode();
		foreach (var card in _oldCards) hashCode.Add(card);
		return hashCode.ToHashCode();
	}
}
