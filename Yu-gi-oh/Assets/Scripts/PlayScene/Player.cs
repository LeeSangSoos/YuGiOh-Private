using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	#region values
	//Field, turn info
	private List<Card> hand = new List<Card>();
	private List<Card> maindeck = new List<Card>();
	private List<Card> extradeck = new List<Card>();
	private List<Card> grave = new List<Card>();
	private List<Card> exiled = new List<Card>();
	private List<Card> monsterfield = Enumerable.Repeat<Card>(null, 5).ToList();
	private List<Card> magicfield = Enumerable.Repeat<Card>(null, 5).ToList();
	private Card linkleftfield;
	private List<Card> linkrightfield;
	private Card fieldmagic;
	public PlayManagerScript playmanager;
	public bool myturn = false;
	public List<Transform> MonsterZone = new List<Transform>(5);
	public List<Transform> MagicZone = new List<Transform>(5);

	//enemy & work available
	public Player enemy;
	private bool isworking = false;

	//card action in game
	public Card CardOnWork;
	private bool workleft = false;
	public bool WorkLeft
	{
		get { return workleft; }
	}
	public bool discardhand;

	//is this user or ai
	[SerializeField] private bool is12;
	private bool isuser;
	[SerializeField] private User userscript;
	[SerializeField] private AiPlayer aiscript;

	//UI
	public Transform HandParent;
	public Sprite backsideimage;
	public Text HandCountText;
	public Transform RandomCardListViewParent;
	public Text LifePointText;
	public Sprite UIMaskimg;

	//Turn action
	public int turndraw;
	public int turnsummon;
	public int turnpendulum;
	public int turndrawlimit = 1;
	public int turnsummonlimit = 1;
	public int turnpendulumlimit = 1;

	//User Info
	public int LifePoint = 8000;

	#endregion
	#region Getter&Setter
	public List<Card> Hand
	{
		get { return hand; }
		set
		{
			List<Card> cards1 = new List<Card>(value);
			hand.Clear();
			foreach (Card card in cards1)
			{
				hand.Add(card);
			}
		}
	}
	public List<Card> MainDeck
	{
		get { return maindeck; }
		set
		{
			List<Card> cards1 = new List<Card>(value);
			maindeck.Clear();
			foreach (Card card in cards1)
			{
				maindeck.Add(card);
			}
		}
	}
	public List<Card> ExtraDeck
	{
		get { return extradeck; }
		set
		{
			List<Card> cards1 = new List<Card>(value);
			extradeck.Clear();
			foreach (Card card in cards1)
			{
				extradeck.Add(card);
			}
		}
	}
	public List<Card> Grave
	{
		get { return grave; }
		set
		{
			List<Card> cards1 = new List<Card>(value);
			grave.Clear();
			foreach (Card card in cards1)
			{
				grave.Add(card);
			}
		}
	}
	public List<Card> Exiled
	{
		get { return exiled; }
		set
		{
			List<Card> cards1 = new List<Card>(value);
			exiled.Clear();
			foreach (Card card in cards1)
			{
				exiled.Add(card);
			}
		}
	}
	public List<Card> MonsterField
	{
		get { return monsterfield; }
		set
		{
			List<Card> cards1 = new List<Card>(value);
			monsterfield.Clear();
			foreach (Card card in cards1)
			{
				monsterfield.Add(card);
			}
		}
	}
	public List<Card> MagicTrapField
	{
		get { return magicfield; }
		set
		{
			List<Card> cards1 = new List<Card>(value);
			magicfield.Clear();
			foreach (Card card in cards1)
			{
				magicfield.Add(card);
			}
		}
	}
	public Card FieldMagic
	{
		get { return fieldmagic; }
		set
		{
			fieldmagic = value;
		}
	}
	public void setPlayerType(PlayerType p)
	{
		if (p == PlayerType.User)
		{
			isuser = true;
		}
		else
		{
			isuser = false;
		}
	}
	public PlayerType GetPlayerType() { return isuser ? PlayerType.User : PlayerType.Ai; }
	public User UserScript { get { return userscript; } set { } }
	public AiPlayer AiScript { get { return aiscript; } set { } }
	#endregion
	#region hand functions
	public void draw()
	{
		if (maindeck.Count == 0) { Debug.Log("No Deck left for player :" + GetPlayerType().ToString()); }
		else
		{
			ToHand(maindeck[0]);
			maindeck.RemoveAt(0);
		}
	}
	public void DiscardHand(Card card)
	{
		try
		{
			Card result = OutOfHand(card);
			ToGrave(result);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
		finally { discardhand = false; }
		PlayerEndWork();
	}
	public void SetMagicTrap(Card card, int pos)
	{
		if ((playmanager.GetPage() != Page.Main1 && playmanager.GetPage() != Page.Main2) || magicfield[pos] != null) { PlayerEndWork(); return; }
		Card targetcard = OutOfHand(card);
		TrapCard trapcard;
		if (card is TrapCard) { trapcard = card as TrapCard; trapcard.setturn = playmanager.GetTotalTurn(); }
		ToMagicField(targetcard, pos, false);
		PlayerEndWork();
		return;
	}
	public void MagicTrapEffectFromHand(Card card, int pos)
	{
		OutOfHand(card);
		ToMagicField(card, pos, true);
		PlayerEndWork();
		EffectAddToChain(card, null);
	}
	public void MagicTrapEffectOnField(Card card)
	{
		PlayerOnWork();
		if (!card.EffectCondition() && card is not TrapCard) { PlayerEndWork();  return; }
		card.iscardfaceup = true;
		int index = MagicTrapField.IndexOf(card);
		MagicZone[index].GetComponent<Image>().sprite = card.CardImage;
		PlayerEndWork();
		EffectAddToChain(card, null);
	}
	#endregion
	#region MonsterField Fuctions
	public void NormalSummon(Card card, int pos, int level, int cost1, int cost2)
	{
		Debug.Log("NormalSummon : " + card);
		if (turnsummon <= 0 || (playmanager.GetPage() != Page.Main1 && playmanager.GetPage() != Page.Main2)) { PlayerEndWork(); return; }
		if (level >= 7)
		{
			if (monsterfield[cost1] == null || monsterfield[cost2] == null || cost1 == cost2) { PlayerEndWork(); return; }
			if (monsterfield[pos] != null && (pos != cost1 && pos != cost2)) { PlayerEndWork(); return; }
			Card sacrifice1 = OutOfMonsterField(cost1);
			Card sacrifice2 = OutOfMonsterField(cost2);
			ToGrave(sacrifice1);
			ToGrave(sacrifice2);
		}
		else if (level >= 5 && level <= 6)
		{
			if (monsterfield[cost1] == null) { PlayerEndWork(); return; }
			if (monsterfield[pos] != null && pos != cost1) { PlayerEndWork(); return; }
			Card sacrifice1 = OutOfMonsterField(cost2);
			ToGrave(sacrifice1);
		}
		if (monsterfield[pos] != null) { PlayerEndWork(); return; }
		Card targetcard = OutOfHand(card);
		ToMonsterField(targetcard, pos, Formation.Attack);
		turnsummon--;
		PlayerEndWork();
		AddTrigger(targetcard, Trigger.NormalSummon);
		return;
	}
	public void SetMonster(Card card, int pos, int level, int cost1, int cost2)
	{
		Debug.Log("SetSummon : " + card);
		if (turnsummon <= 0 || (playmanager.GetPage() != Page.Main1 && playmanager.GetPage() != Page.Main2)) { PlayerEndWork(); return; }
		if (level >= 7)
		{
			if (monsterfield[cost1] == null || monsterfield[cost2] == null || cost1 == cost2) { PlayerEndWork(); return; }
			if (monsterfield[pos] != null && (pos != cost1 && pos != cost2)) { PlayerEndWork(); return; }
			Card sacrifice1 = OutOfMonsterField(cost1);
			Card sacrifice2 = OutOfMonsterField(cost2);
			ToGrave(sacrifice1);
			ToGrave(sacrifice2);
		}
		else if (level >= 5 && level <= 6)
		{
			if (monsterfield[cost1] == null) { PlayerEndWork(); return; }
			if (monsterfield[pos] != null && pos != cost1) { PlayerEndWork(); return; }
			Card sacrifice1 = OutOfMonsterField(cost2);
			ToGrave(sacrifice1);
		}
		if (monsterfield[pos] != null) { PlayerEndWork(); return; }
		Card targetcard = OutOfHand(card);
		ToMonsterField(targetcard, pos, Formation.Set);
		turnsummon--;
		PlayerEndWork();
		return;
	}
	public void Attack(Card attackcard, int pos)
	{
		MonsterCard playercard;
		if (attackcard is MonsterCard)
			playercard = attackcard as MonsterCard;
		else
		{
			return;
		}
		if (playercard.attackchance <= 0 || !attackcard.iscardfaceup) return;
		if (playmanager.GetPage() == Page.Battle && playmanager.GetPageTime() == PageTime.OnGoing && playercard.formation == Formation.Attack)
		{
			PlayerOnWork();
			MonsterCard enemycard = enemy.MonsterField[pos] as MonsterCard;
			if (enemycard.formation == Formation.Attack)
			{
				if (enemycard.atk < playercard.atk)
				{
					enemy.LifePointDown(playercard.atk - enemycard.atk);
					playmanager.DestroyCard(enemycard, DeathReason.BattleDestory);
				}
				else if (enemycard.atk == playercard.atk)
				{
					playmanager.DestroyCard(enemycard, DeathReason.BattleDestory);
					playmanager.DestroyCard(playercard, DeathReason.BattleDestory);
				}
				else
				{
					LifePointDown(enemycard.atk - playercard.atk);
					playmanager.DestroyCard(playercard, DeathReason.BattleDestory);
				}
			}
			else if (enemycard.formation == Formation.Defence || enemycard.formation == Formation.Set)
			{
				if (enemycard.formation == Formation.Set)
				{
					ReverseMonster(enemycard);
				}
				if (enemycard.def < playercard.atk)
				{
					playmanager.DestroyCard(enemycard, DeathReason.BattleDestory);
				}
				else if (enemycard.def == playercard.atk)
				{

				}
				else
				{
					LifePointDown(enemycard.def - playercard.atk);
				}
			}
			playercard.attackchance--;
			playercard.changebattlepos--;
			PlayerEndWork();
		}
	}
	public void DirectAttack(Card attackcard)
	{
		MonsterCard playercard = attackcard as MonsterCard;
		if (playercard.attackchance <= 0 || !attackcard.iscardfaceup) return;
		if (playmanager.GetPage() == Page.Battle && playmanager.GetPageTime() == PageTime.OnGoing && playercard.formation == Formation.Attack)
		{
			PlayerOnWork();
			enemy.LifePointDown(playercard.atk);
			playercard.attackchance--;
			playercard.changebattlepos--;
			PlayerEndWork();
		}
	}
	public void ReverseMonster(Card card)
	{
		if (card.iscardfaceup) { return; }
		int index = card.owner.MonsterField.IndexOf(card);
		card.owner.MonsterZone[index].GetComponent<Image>().sprite = card.CardImage;
		card.iscardfaceup = true;
	}
	public void ChangeBattlePosition(Card card)
	{
		if ((playmanager.GetPage() != Page.Main1 && playmanager.GetPage() != Page.Main2) || card.changebattlepos <= 0) { return; }
		PlayerOnWork();
		int index = monsterfield.IndexOf(card);
		MonsterCard mcard = card as MonsterCard;
		switch (mcard.formation)
		{
			case Formation.Attack:
				MonsterZone[index].eulerAngles = new Vector3(0, 0, 90);
				mcard.formation = Formation.Defence;
				break;
			case Formation.Defence:
				MonsterZone[index].eulerAngles = new Vector3(0, 0, 0);
				mcard.formation = Formation.Attack;
				break;
			case Formation.Set:
				MonsterZone[index].eulerAngles = new Vector3(0, 0, 0);
				mcard.formation = Formation.Attack;
				MonsterZone[index].GetComponent<Image>().sprite = card.CardImage;
				mcard.iscardfaceup = true;
				break;
		}
		mcard.changebattlepos--;
		PlayerEndWork();
	}
	#endregion
	#region MagicField Funtions

	#endregion
	#region LifePointFunctions
	public void LifePointUp(int amount) { LifePoint += amount; LifePointText.text = LifePoint.ToString(); }
	public void LifePointDown(int amount) { LifePoint -= amount; 
		if(LifePoint < 0) LifePoint = 0;
		LifePointText.text = LifePoint.ToString(); }
	#endregion
	#region TimeFunctions
	private void Update()
	{
		switch (playmanager.GetPage())
		{
			case Page.Draw:
				DrawPageWork();
				break;
			case Page.Standby:
				StandbyPageWork();
				break;
			case Page.Main1:
				Main1PageWork();
				break;
			case Page.Battle:
				BattlePageWork();
				break;
			case Page.Main2:
				Main2PageWork();
				break;
			case Page.End:
				EndPageWork();
				break;
		}
	}
	void DrawPageWork()
	{
		switch (playmanager.GetPageTime())
		{
			case PageTime.Start:
				if (myturn)
				{
					while (turndraw >= 1)
					{
						PlayerOnWork();
						draw();
						turndraw--;
						PlayerEndWork();
					}
				}
				break;
			case PageTime.OnGoing:
				break;
			case PageTime.End:
				break;
		}
	}
	void StandbyPageWork()
	{

	}
	void Main1PageWork()
	{
		switch (playmanager.GetPageTime())
		{
			case PageTime.Start:
				break;
			case PageTime.OnGoing:
				break;
			case PageTime.End:
				break;
		}
	}
	void BattlePageWork()
	{

	}
	void Main2PageWork()
	{

	}
	void EndPageWork()
	{

	}
	#endregion
	#region Check for work
	//Function for checking the effect of card
	//Function for checking the work to do
	public void SetTurnActions()
	{
		turndraw = turndrawlimit;
		turnsummon = turnsummonlimit;
		turnpendulum = turnpendulumlimit;
		foreach (Card card in monsterfield)
		{
			if (card != null)
			{
				MonsterCard monstercard = card as MonsterCard;
				monstercard.attackchance = 1;
				monstercard.changebattlepos = 1;
			}
		}
	}
	public void PlayerOnWork()
	{
		if (!isworking)
		{
			isworking = true;
			//Debug.Log((isuser ? "User " : "Ai ") + " On work");
		}
	}
	public void PlayerEndWork()
	{
		if (isworking)
		{
			isworking = false;
			if (userscript != null)
			{
				userscript.whatwork = WhatWork.Null;
				userscript.WhatToDo.text = "";
			}
			//Debug.Log((isuser ? "User " : "Ai ") + " Work Over");
		}
	}
	public bool GetIsWorking()
	{
		return isworking;
	}
	public void ToPageEnd()
	{
		if (playmanager.IsWorkLeft()) return;
		if (playmanager.GetPageTime() == PageTime.OnGoing)
		{
			workleft = false;
		}
	}
	public void ToEndPage()
	{
		if (playmanager.IsWorkLeft()) return;
		workleft = false;
		playmanager.main1toend = true;
	}
	public void NoWorkLeft() { workleft = false; }
	public void WaitForPageWork() { workleft = true; }
	public void ActivateCardLeft() { }
	#endregion
	#region UserActionCall
	public void UserAction(Card card)
	{
		userscript.CardClickEvent(card);
	}
	#endregion
	#region Function for Rule
	public void HandOver6()
	{
		if (hand.Count > 6)
		{
			discardhand = true;
		}
	}
	#endregion
	#region Card Move Function
	public void ToHand(Card card)
	{
		hand.Add(card);
		card.CardObejct.transform.SetParent(HandParent, false);
		card.CardObejct.SetActive(true);
		if (is12)
		{
			card.CardObejct.GetComponent<Image>().sprite = backsideimage;
		}
		else
		{
			card.CardObejct.GetComponent<Image>().sprite = card.CardImage;
		}
		card.pos = CardPosition.Hand;
		HandCountText.text = hand.Count.ToString();
	}
	public Card OutOfHand(Card card)
	{
		hand.Remove(card);
		card.CardObejct.SetActive(false);
		card.CardObejct.transform.SetParent(RandomCardListViewParent, false);
		HandCountText.text = hand.Count.ToString();
		card.pos = CardPosition.Unknown;
		return card;
	}
	public void ToGrave(Card card)
	{
		grave.Add(card);
		card.CardObejct.transform.SetParent(RandomCardListViewParent, false);
		card.CardObejct.SetActive(false);
		card.CardObejct.GetComponent<Image>().sprite = card.CardImage;
		card.pos = CardPosition.Grave;
	}
	public Card OutOfGrave(Card card)
	{
		grave.Remove(card);
		card.CardObejct.SetActive(false);
		card.pos = CardPosition.Unknown;
		return card;
	}
	public void ToDeck(Card card)
	{
		maindeck.Add(card);
		card.CardObejct.transform.SetParent(RandomCardListViewParent, false);
		card.CardObejct.SetActive(false);
		card.CardObejct.GetComponent<Image>().sprite = card.CardImage;
		card.pos = CardPosition.MainDeck;
	}
	public Card OutOfDeck(Card card)
	{
		maindeck.Remove(card);
		card.CardObejct.SetActive(false);
		card.pos = CardPosition.Unknown;
		return card;
	}
	public void ToExiled(Card card)
	{
		exiled.Add(card);
		card.CardObejct.transform.SetParent(RandomCardListViewParent, false);
		card.CardObejct.SetActive(false);
		card.CardObejct.GetComponent<Image>().sprite = card.CardImage;
		card.pos = CardPosition.Exiled;
	}
	public Card OutOfExiled(Card card)
	{
		exiled.Remove(card);
		card.CardObejct.SetActive(false);
		card.pos = CardPosition.Unknown;
		return card;
	}
	public void ToExtraDeck(Card card)
	{
		extradeck.Add(card);
		card.CardObejct.transform.SetParent(RandomCardListViewParent, false);
		card.CardObejct.SetActive(false);
		card.CardObejct.GetComponent<Image>().sprite = card.CardImage;
		card.pos = CardPosition.ExtraDeck;
	}
	public Card OutOfExtraDeck(Card card)
	{
		extradeck.Remove(card);
		card.CardObejct.SetActive(false);
		card.pos = CardPosition.Unknown;
		return card;
	}
	public void ToMagicField(Card card, int pos, bool isfaceup)
	{
		magicfield[pos] = card;
		card.pos = CardPosition.MagicField;
		if (isfaceup)
		{
			Debug.Log(this + " activate magic/trap : " + card);
			MagicZone[pos].GetComponent<Image>().sprite = card.CardImage;
			MagicZone[pos].eulerAngles = new Vector3(0, 0, 0);
			card.iscardfaceup = isfaceup;
		}
		else
		{
			Debug.Log(this + "set magic/trap : " + card);
			MagicZone[pos].GetComponent<Image>().sprite = backsideimage;
			MagicZone[pos].eulerAngles = new Vector3(0, 0, 0);
			card.iscardfaceup = isfaceup;
		}
	}
	public Card OutOfMagicField(int pos)
	{
		Card card = magicfield[pos];
		MagicZone[pos].GetComponent<Image>().sprite = UIMaskimg;
		magicfield[pos] = null;
		card.pos = CardPosition.Unknown;
		card.setturn = -1;
		card.RemovePassiveEffect();
		return card;
	}
	public void ToMonsterField(Card card, int pos, Formation form)
	{
		if (monsterfield[pos] == null)
		{
			try
			{
				MonsterCard monsterCard = card as MonsterCard;
				if (monsterCard != null)
				{
					monsterCard.formation = form;
					if (form == Formation.Set)
					{
						monsterCard.iscardfaceup = false;
						MonsterZone[pos].GetComponent<Image>().sprite = backsideimage;
						MonsterZone[pos].eulerAngles = new Vector3(0, 0, 90);
					}
					else if (form == Formation.Attack)
					{
						monsterCard.iscardfaceup = true;
						MonsterZone[pos].GetComponent<Image>().sprite = card.CardImage;
						MonsterZone[pos].eulerAngles = new Vector3(0, 0, 0);
					}
					else
					{
						monsterCard.iscardfaceup = true;
						MonsterZone[pos].GetComponent<Image>().sprite = card.CardImage;
						MonsterZone[pos].eulerAngles = new Vector3(0, 0, 90);
					}
					card.pos = CardPosition.MonsterField;
					monsterfield[pos] = card;
					monsterCard.atk = monsterCard.OriginAtk;
					monsterCard.def = monsterCard.OriginDef;
				}
				else
				{
					Debug.LogError("The card trying to summon is not a MonsterCard");
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
		else
		{
			Debug.LogError("monster field trying to summon on isn't null");
		}
	}
	public Card OutOfMonsterField(int pos)
	{
		try
		{
			Card card = monsterfield[pos];
			card.pos = CardPosition.Unknown;
			monsterfield[pos] = null;
			MonsterZone[pos].GetComponent<Image>().sprite = UIMaskimg;
			card.setturn = -1;
			MonsterCard monsterCard = card as MonsterCard;
			monsterCard.atk = monsterCard.OriginAtk;
			monsterCard.def = monsterCard.OriginDef;
			//destory all equippment with this card
			for (int i = 0; i < 5; i++)
			{
				MagicCard magic1;
				MagicCard magic2;
				if (MagicTrapField[i] != null)
				{
					if (MagicTrapField[i] is MagicCard)
					{
						magic1 = MagicTrapField[i] as MagicCard;
						if(magic1.magictype==MagicCardtype.Equip && magic1.targetcard == card)
						{
							playmanager.DestroyCard(magic1, DeathReason.EffectDestroy);
						}
					}
				}
				if (enemy.MagicTrapField[i] != null)
				{
					if (enemy.MagicTrapField[i] is MagicCard)
					{
						magic2 = enemy.MagicTrapField[i] as MagicCard;
						if (magic2.magictype == MagicCardtype.Equip && magic2.targetcard == card)
						{
							playmanager.DestroyCard(magic2, DeathReason.EffectDestroy);
						}
					}						
				}				
			}
			return card;
		}
		catch (Exception e) { Debug.LogException(e); return null; }
	}
	#endregion
	#region Effects Activation
	public void CheckCardListForActivate(List<Card> cardlist, Card recentactivatedcard)
	{
		if (isuser)
		{
			userscript.SeeCardsToActivate(cardlist);
		}
		else
		{
			aiscript.SeeCardsToActivate(cardlist, recentactivatedcard);
		}
	}
	public void AddChainEffect(Card card)
	{
		playmanager.AddChainEffect(card);
		playmanager.AddTrigger(Trigger.EffectActivated);
		PlayerEndWork();
		playmanager.ChainProcess(this, card);
	}
	public void AddTrigger(Card card, Trigger trigger)
	{
		playmanager.AddTrigger(trigger);
		playmanager.ChainProcess(this, card);
	}
	public void CancelEffect()
	{
		playmanager.ChainProcess2(this, playmanager.WhenActivatedCard);
	}
	private const int MaxRecursiveCalls = 10;

	public void EffectAddToChain(Card card, Card target, int recursiveCount = 0)
	{
		PlayerOnWork();
		bool goodtogo = true;

		if (card.NeedTarget)
		{
			if (card.Autotarget)
			{
				target = card.GetAutoTarget();
			}
			if (target == null)
			{
				goodtogo = false;
			}
			if (card.Needtargetpos == CardPosition.MonsterField)
			{
				if (target.pos != CardPosition.MonsterField)
				{
					goodtogo = false;
				}
			}
			if (card.Needtargetowner == TargetOwner.Mine)
			{
				
			}
			if (card.Needtargettype == TargetType.Monster)
			{
				if (target is not MonsterCard)
				{
					goodtogo = false;
				}
			}
		}

		if (!card.EffectCondition() || !card.TargetTempCondition(target)) goodtogo = false;

		if (goodtogo)
		{
			card.targetcard = target;
			AddChainEffect(card);
		}
		else
		{
			if (isuser)
			{
				userscript.WhatToDo.text = "Choose target";
				userscript.whatwork = WhatWork.ChooseTarget;
			}
			else
			{
				if (recursiveCount >= MaxRecursiveCalls)
				{
					return;
				}
				target = aiscript.ChooseTargetCard(card as MagicCard);
				EffectAddToChain(card, target, recursiveCount + 1);
			}
		}
		return;
	}

	#endregion
}