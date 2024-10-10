using System;
using System.Linq;

public class BeastFangs : IEffect
{
	#region Card Effect Works Functions
	public override bool TargetCondition(Card card, Card target)
	{
		if (target == null || target is not MonsterCard) return false;

		MonsterCard monsterCard = target as MonsterCard;
		if (monsterCard.monstertype == MonsterCardType.Beast && monsterCard.iscardfaceup)
			return true;
		else
			return false;
	}
	public override bool EffectCondition(Card card)
	{
		if ((card.pos != CardPosition.MagicField && card.pos != CardPosition.Hand) ||
			(card.owner.playmanager.GetPage() != Page.Main1 && card.owner.playmanager.GetPage() != Page.Main2)
			)
		{
			return false;
		}
		if (card.targetcard != null)
		{
			if (card.targetcard is MonsterCard)
			{
				MonsterCard monster = card.targetcard as MonsterCard;
				return monster.monstertype == MonsterCardType.Beast && monster.iscardfaceup;
			}
			else return false;
		}
		if (card.owner.MonsterField.Any(card =>
		{
			if (card != null)
			{
				MonsterCard monster = card as MonsterCard;
				return monster.monstertype == MonsterCardType.Beast && monster.iscardfaceup;
			}
			return false;
		}) || card.owner.enemy.MonsterField.Any(card =>
		{
			if (card != null)
			{
				MonsterCard monster = card as MonsterCard;
				return monster.monstertype == MonsterCardType.Beast && monster.iscardfaceup;
			}
			return false;
		}))
			return true;
		else return false;
	}
	public override void Effect(Card card)
	{
		if (card.targetcard.pos == CardPosition.MonsterField)
		{
			MonsterCard mons = card.targetcard as MonsterCard;
			mons.atk += 300;
		}
	}
	public override Card AutoTargetFunction(Card card)
	{
		throw new NotImplementedException();
	}
	public override void RemovePassiveEffect(Card card)
	{
		MonsterCard mons = (MonsterCard)card.targetcard;
		if (mons.pos == CardPosition.MonsterField)
			mons.atk -= 300;
	}
	#endregion
}
