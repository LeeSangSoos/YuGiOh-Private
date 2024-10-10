using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Fissure : IEffect
{
	#region Card Effect Works Functions
	public override bool TargetCondition(Card card, Card target)
	{
		return true;
	}
	public override bool EffectCondition(Card card)
	{
		if ((card.pos != CardPosition.MagicField && card.pos != CardPosition.Hand) ||
			(card.owner.playmanager.GetPage() != Page.Main1 && card.owner.playmanager.GetPage() != Page.Main2) ||
			((card.owner.enemy.MonsterField.All(c => c == null)) || !(card.owner.enemy.MonsterField.Any(c => c != null && c.iscardfaceup)))
			)
		{
			return false;
		}
		return true;
	}
	public override void Effect(Card card)
	{
		PlayManagerScript playManagerScript = card.playManager;
		playManagerScript.DestroyCard(card.targetcard, DeathReason.EffectDestroy);
	}
	public override Card AutoTargetFunction(Card card)
	{
		List<Card> monsterlist = card.owner.enemy.MonsterField;
		MonsterCard target = null;

		foreach (Card monster in monsterlist)
		{
			if (monster != null)
			{
				if (monster is not MonsterCard)
				{
					throw new Exception(this.ToString() + "Target Unexpected value: not a MonsterCard");
				}
				MonsterCard mons = monster as MonsterCard;
				if (mons.iscardfaceup)
				{
					if (target == null) target = mons;
					else if (target.atk > mons.atk) target = mons;
				}
			}
		}
		return target;
	}
	public override void RemovePassiveEffect(Card card)
	{
		return;
	}
	#endregion
}
