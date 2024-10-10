using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "MagicCard")]
public class MagicCard : Card
{

	public MagicCardtype magictype;

	public override void CopyProperties(Card other)
	{
		if (other is MagicCard magicCard)
		{
			base.CopyProperties(magicCard);
			this.magictype = magicCard.magictype;
		}
	}
}
