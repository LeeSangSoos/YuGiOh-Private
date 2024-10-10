using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class DarkHole : IEffect
{
	#region Card Effect Works Functions
	public override bool TargetCondition(Card card, Card target)
	{
		return true;
	}
	public override bool EffectCondition(Card card)
	{
		if ((card.pos != CardPosition.MagicField && card.pos != CardPosition.Hand) || 
			(card.owner.playmanager.GetPage()!=Page.Main1 && card.owner.playmanager.GetPage() != Page.Main2)||
			((card.owner.MonsterField.All(card => card == null)) && (card.owner.enemy.MonsterField.All(card => card == null)))
			)
		{
			return false;
		}
		return true;
	}
	public override void Effect(Card card)
	{
		PlayManagerScript playManagerScript = card.playManager;
		List<Card> cardstodestroy = new List<Card>();
		foreach(Card target in card.owner.MonsterField)
		{
			if(target!=null)
				cardstodestroy.Add(target);
				
		}
		foreach (Card target in card.owner.enemy.MonsterField)
		{
			if (target != null)
				cardstodestroy.Add(target);
		}
		foreach(Card target in cardstodestroy)
		{
			playManagerScript.DestroyCard(target, DeathReason.EffectDestroy);
		}
	}
	public override Card AutoTargetFunction(Card card)
	{
		throw new System.NotImplementedException();
	}
	public override void RemovePassiveEffect(Card card)
	{
		return;
	}
	#endregion
}
