using UnityEngine;

public class Card : ScriptableObject
{
	#region cardinfo
	[SerializeField]
	private string description;
	[SerializeField]
	private Sprite cardimage;
	public bool iscardfaceup;

	public string Description
	{
		get { return description ?? ""; }
	}
	public string CardName
	{
		get { return name.ToString(); }
	}
	public Sprite CardImage
	{
		get { return cardimage; }
	}
	public DeathReason deathreason;
	#endregion
	#region cardeffect variables
	public PlayManagerScript playManager;
	public Trigger trigger;
	public Card targetcard;
	public bool cardNameUsed1 = false;
	public bool thisCardUsed = false;
	public IEffect effect;
	public bool NeedTarget;
	public CardPosition Needtargetpos;
	public TargetType Needtargettype;
	public TargetOwner Needtargetowner;
	public bool Autotarget;
	#endregion
	#region Card Public Data
	public CardPosition pos; //Card position on field
	public Player owner; //Card possesion
	private Player originowner; //Card original owner
	public void SetOriginOwner(Player player) { originowner = player; }
	public Player GetOriginOwner() {  return originowner; }
	private int cardnum; //Card identification number
	public void SetCardNum(int n)
	{
		cardnum = n;
	}
	public bool availableCard;
	public int GetCardNum() {  return cardnum; }
	private GameObject cardobject;
	public void SetCardObject(GameObject cardo) { cardobject = cardo; }
	public GameObject CardObejct { get { return cardobject; } }

	public int setturn = -1;
	public int attackchance = 1;
	public int changebattlepos = 1;
	#endregion
	#region CardFunction
	public virtual void CopyProperties(Card other)
	{
		// Copy basic properties
		this.name = other.CardName;
		this.cardimage = other.CardImage;
		this.description = other.Description;
		this.pos = other.pos;
		this.owner = other.owner;
		this.originowner = other.originowner;
		this.cardnum = other.cardnum;
		this.cardobject = other.cardobject;

		// Copy card effect-related properties
		this.playManager = other.playManager;
		this.trigger = other.trigger;
		this.targetcard = other.targetcard;
		this.cardNameUsed1 = other.cardNameUsed1;
		this.thisCardUsed = other.thisCardUsed;
		this.effect = other.effect;
		this.NeedTarget = other.NeedTarget;
		this.Needtargetpos = other.Needtargetpos;
		this.Needtargettype = other.Needtargettype;
		this.Needtargetowner = other.Needtargetowner;
		this.Autotarget = other.Autotarget;

		// Copy other properties specific to your Card class
		this.deathreason = other.deathreason;
		this.availableCard = other.availableCard;
		this.setturn = other.setturn;
		this.attackchance = other.attackchance;
		this.changebattlepos = other.changebattlepos;
	}

	#endregion
	#region Card Effect Works Functions
	public bool TargetTempCondition(Card target)
	{
		return effect.TargetCondition(this, target);
	}
	public bool TargetCondition()
	{
		return effect.TargetCondition(this, targetcard);
	}
	public bool EffectCondition()
	{
		if (thisCardUsed) return false;
		return effect.EffectCondition(this);
	}
	public void Effect()
	{
		effect.Effect(this); return;
	}
	public Card GetAutoTarget()
	{
		return effect.AutoTargetFunction(this);
	}
	public void RemovePassiveEffect()
	{
		effect.RemovePassiveEffect(this);
	}
	#endregion
}