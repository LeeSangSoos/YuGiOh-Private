using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "MonsterCard")]
public class MonsterCard : Card
{
	#region CardData
	public MonsterCardType monstertype; // race
	public Attribute attribute; // element
	public int level; //level, rank, link
	public List<Archetype> archetype; // effect, fusion ...
	public Formation formation; //Attack, Defence, Set

	public int atk; 
	public int def;

	[SerializeField]
	private int originatk;
	[SerializeField]
	private int origindef;
	public int OriginAtk { get { return originatk; } set { } }
	public int OriginDef { get {  return origindef; } set { } }
	#endregion
	#region CardEffect
	[SerializeField]
	private SummonMethod summonmMethod;
	public SummonMethod SummonMethod { get { return summonmMethod; } }
	#endregion

	public override void CopyProperties(Card other)
	{
		if (other is MonsterCard monsterCard)
		{
			base.CopyProperties(monsterCard);
			this.monstertype = monsterCard.monstertype;
			this.attribute = monsterCard.attribute;
			this.level = monsterCard.level;
			this.archetype = new List<Archetype>(monsterCard.archetype);
			this.atk = monsterCard.atk;
			this.def = monsterCard.def;
			this.formation = monsterCard.formation;
			this.originatk = monsterCard.OriginAtk;
			this.origindef = monsterCard.OriginDef;
		}
	}
}