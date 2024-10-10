using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
	#region values
	//Public data
	public PlayManagerScript playmanager;
	//UI
	public Transform HandParent;
	public GameObject HandPrefab;
	public Sprite backsideimage;
	public bool ai;
	public Player player;
	Card cardonwork;
	MonsterCard monsterCardOnWork;
	MagicCard magicCardOnWork;
	TrapCard trapCardOnWork;

	//User UI
	public GameObject HandEventPanel;
	public GameObject Summon1;
	public GameObject Summon2;
	public GameObject Set;
	public GameObject Effect;
	public GameObject EndPageBtn;
	public GameObject RandomCardListView;
	public Transform RandomCardListViewParent;
	public GameObject ChooseEffectToActivatePanel;
	public Transform ChooseEffectToActivateParent;
	public GameObject CardPrefab;
	public GameObject MonsterFieldActionPanel;
	public GameObject MagicFieldActivationPanel;

	//what Work
	public WhatWork whatwork = WhatWork.Null;
	SummonMethod summonMethodOnWork;
	List<int> poslist = new List<int>();
	public Text WhatToDo;

	#endregion
	#region Timefunctions
	private void Start()
	{
		HandEventPanel.SetActive(false);
		EndPageBtn.SetActive(false);
		RandomCardListView.SetActive(false);
		ChooseEffectToActivatePanel.SetActive(false);
		MonsterFieldActionPanel.SetActive(false);
		MagicFieldActivationPanel.SetActive(false);
	}
	#endregion
	#region UIFunction
	#region Hand Card Functions
	public void NormalSummon()
	{
		player.PlayerOnWork();
		HandEventPanel.SetActive(false);
		summonMethodOnWork = SummonMethod.Normal;
		whatwork = WhatWork.Summon;
	}
	public void SetCardFromHand()
	{
		player.PlayerOnWork();
		HandEventPanel.SetActive(false);
		if (cardonwork is MonsterCard)
		{
			summonMethodOnWork = SummonMethod.Set;
			whatwork = WhatWork.Summon;
		}
		else if (cardonwork is MagicCard)
		{
			whatwork = WhatWork.SetMagicTrap;
		}
		else if (cardonwork is TrapCard)
		{
			whatwork = WhatWork.SetMagicTrap;
		}
	}
	public void SpecialSummonHand()
	{

	}
	public void EffectHand()
	{
		if (!cardonwork.EffectCondition()) { return; }
		if (cardonwork is MagicCard)
		{
			if (player.MagicTrapField.All(card => card != null)) return;
			WhatToDo.text = "Choose Magic/Trap Field To Activate";
		}
		player.PlayerOnWork();
		HandEventPanel.SetActive(false);
		whatwork = WhatWork.EffectFromHand;
	}
	public void DetailImage()
	{
		//cardonwork.Image display
	}
	public void BackToHand()
	{
		player.PlayerEndWork();
		HandEventPanel.SetActive(false);
	}
	public void Summon(int pos)
	{
		switch (summonMethodOnWork)
		{
			case SummonMethod.Normal:
				if (monsterCardOnWork.level < 5)
				{
					player.NormalSummon(cardonwork, pos, monsterCardOnWork.level, -1, -1);
				}
				else if (monsterCardOnWork.level >= 5 && monsterCardOnWork.level <= 6)
				{
					player.NormalSummon(cardonwork, pos, monsterCardOnWork.level, poslist[0], -1);
				}
				else
				{
					player.NormalSummon(cardonwork, pos, monsterCardOnWork.level, poslist[0], poslist[1]);
				}
				break;
			case SummonMethod.Set:
				if (monsterCardOnWork.level < 5)
				{
					player.SetMonster(cardonwork, pos, monsterCardOnWork.level, -1, -1);
				}
				else if (monsterCardOnWork.level >= 5 && monsterCardOnWork.level <= 6)
				{
					player.SetMonster(cardonwork, pos, monsterCardOnWork.level, poslist[0], -1);
				}
				else
				{
					player.SetMonster(cardonwork, pos, monsterCardOnWork.level, poslist[0], poslist[1]);
				}
				break;
			case SummonMethod.Fusion: break;
			case SummonMethod.Link: break;
			case SummonMethod.Pendulum: break;
			case SummonMethod.Ritual: break;
			case SummonMethod.Special: break;
			case SummonMethod.Synchro: break;
			case SummonMethod.XYZ: break;
		}
		poslist.Clear();
		summonMethodOnWork = SummonMethod.Null;
	}
	public void SetMagicTrapOnField(int pos)
	{
		player.SetMagicTrap(cardonwork, pos);
		whatwork = WhatWork.Null;
	}
	#endregion
	#region Choose Field Zone
	public void MadeChoice(int pos)
	{
		Card ccard = null;
		if (pos >= 60 && pos <= 64)// Player 6 Monster Field
		{
			pos -= 60;
			ccard = player.MonsterField[pos];
			if (whatwork == WhatWork.Summon)
			{
				if ((summonMethodOnWork == SummonMethod.Normal || summonMethodOnWork == SummonMethod.Set))
				{
					if (monsterCardOnWork.level < 5)
					{
						Summon(pos);
					}
					else if (monsterCardOnWork.level >= 5 && monsterCardOnWork.level <= 6)
					{
						if (poslist.Count < 1)
						{
							poslist.Add(pos);
						}
						else Summon(pos);
					}
					else
					{
						if (poslist.Count < 2)
						{
							poslist.Add(pos);

						}
						else { Summon(pos); }
					}
				}
			}
			else if (whatwork == WhatWork.Null && player.MonsterField[pos] != null) //Click Field At AnyTime
			{
				if (!player.GetIsWorking() && player.myturn)
				{
					cardonwork = player.MonsterField[pos];
					MonsterFieldActionPanel.SetActive(true);
				}
			}
		}
		else if (pos >= 120 && pos <= 124) // Player 12 Monster Field
		{
			pos -= 120;
			ccard = player.enemy.MonsterField[pos];
			if (whatwork == WhatWork.Attack)
			{
				if(ccard != null)
					player.Attack(cardonwork, pos);
			}

		}
		else if (pos >= 65 && pos <= 69) //Player 6 Magic field
		{
			pos -= 65;
			ccard = player.MagicTrapField[pos];
			if (whatwork == WhatWork.SetMagicTrap)
			{
				SetMagicTrapOnField(pos);
			}
			else if (whatwork == WhatWork.Effect)
			{

			}
			else if (whatwork == WhatWork.EffectFromHand)
			{
				player.MagicTrapEffectFromHand(cardonwork, pos);
			}
			else if (whatwork == WhatWork.Null && player.MagicTrapField[pos] != null) //Click Field At AnyTime
			{
				if (!player.GetIsWorking() && player.myturn)
				{
					cardonwork = player.MagicTrapField[pos];
					MagicFieldActivationPanel.SetActive(true);
				}
			}
		}
		else if (pos >= 125 && pos <= 129) // Player 12 Magic field
		{
			pos -= 120;
			ccard = player.enemy.MagicTrapField[pos];
			if (whatwork == WhatWork.SetMagicTrap)
			{
				SetMagicTrapOnField(pos);
			}
			else if (whatwork == WhatWork.Effect)
			{

			}
		}
		if (whatwork == WhatWork.ChooseTarget)
		{
			player.EffectAddToChain(cardonwork, ccard);
		}
	}
	public void ChooseLinkZoneLeft() { MadeChoice(1); }
	public void ChooseLinkZoneRight() { MadeChoice(2); }

	public void ChooseMonsterZone120() { MadeChoice(120); }
	public void ChooseMonsterZone121() { MadeChoice(121); }
	public void ChooseMonsterZone122() { MadeChoice(122); }
	public void ChooseMonsterZone123() { MadeChoice(123); }
	public void ChooseMonsterZone124() { MadeChoice(124); }

	public void ChooseMagicZone125() { MadeChoice(125); }
	public void ChooseMagicZone126() { MadeChoice(126); }
	public void ChooseMagicZone127() { MadeChoice(127); }
	public void ChooseMagicZone128() { MadeChoice(128); }
	public void ChooseMagicZone129() { MadeChoice(129); }

	public void ChooseFieldMagicZone12() { MadeChoice(1210); }

	public void ChooseMonsterZone60() { MadeChoice(60); }
	public void ChooseMonsterZone61() { MadeChoice(61); }
	public void ChooseMonsterZone62() { MadeChoice(62); }
	public void ChooseMonsterZone63() { MadeChoice(63); }
	public void ChooseMonsterZone64() { MadeChoice(64); }

	public void ChooseMagicZone65() { MadeChoice(65); }
	public void ChooseMagicZone66() { MadeChoice(66); }
	public void ChooseMagicZone67() { MadeChoice(67); }
	public void ChooseMagicZone68() { MadeChoice(68); }
	public void ChooseMagicZone69() { MadeChoice(69); }

	public void ChooseFieldMagicZone6() { MadeChoice(610); }

	#endregion
	#region CardClickEvent Except field
	public void CardClickEvent(Card card)
	{
		if (!card.availableCard) return;
		cardonwork = card;
		if (card.owner != player)
		{
			Debug.Log(card + "Owner is differ");
			return;
		} //Owner of the card is different
		else
		{
			if (whatwork == WhatWork.ChainEffect) { ChainClickEvent(card); return; }
			if (!player.myturn) return;
			switch (card.pos)
			{
				case CardPosition.Hand: //Card is in Hand
					if (player.discardhand)
					{
						player.DiscardHand(card);
						break;
					}
					if (card is MonsterCard)
					{
						monsterCardOnWork = (MonsterCard)card;
						HandEventPanel.SetActive(true); //Open Monster Hand event Panel
						if (monsterCardOnWork.archetype[0] == Archetype.Normal) //Works the monster can do in hand
						{
							Effect.SetActive(false);
						}
						else if (monsterCardOnWork.archetype[0] == Archetype.Effect)
						{
							Effect.SetActive(true);
						}
						Summon1.GetComponentInChildren<TextMeshProUGUI>().text = "NormalSummon";
						Summon1.SetActive(true);
						Set.SetActive(true);

						Summon2.GetComponentInChildren<TextMeshProUGUI>().text = "SpecialSummon";
						Summon2.SetActive(false);
						break;
					}
					else if (card is MagicCard)
					{
						magicCardOnWork = (MagicCard)card;
						Summon1.SetActive(false); Summon2.SetActive(false);
						HandEventPanel.SetActive(true); //Open Hand event Panel
						Effect.SetActive(true);
						Set.SetActive(true);
					}
					else if (card is TrapCard)
					{
						trapCardOnWork = (TrapCard)card;
						Summon1.SetActive(false); Summon2.SetActive(false);
						HandEventPanel.SetActive(true); //Open Hand event Panel
						Effect.SetActive(true);
						Set.SetActive(true);
					}

					break;
				case CardPosition.MainDeck: break;
				case CardPosition.ExtraDeck: break;
				case CardPosition.Grave: break;
				case CardPosition.Exiled: break;
				case CardPosition.MonsterField:
					monsterCardOnWork = (MonsterCard)card;
					MonsterFieldActionPanel.SetActive(true);
					break;
				case CardPosition.MagicField:
					if (card is MagicCard)
						magicCardOnWork = (MagicCard)card;
					if (card is TrapCard)
						trapCardOnWork = (TrapCard)card;
					MagicFieldActivationPanel.SetActive(true);
					break;
				case CardPosition.FieldMagic: break;
				default: break;
			}
		}
		player.CardOnWork = card;
	}
	public void ChainClickEvent(Card card)
	{
		foreach (Transform child in ChooseEffectToActivateParent.transform)
		{
			Destroy(child.gameObject);
		}
		whatwork = WhatWork.Null;
		ChooseEffectToActivatePanel.SetActive(false);
		player.EffectAddToChain(card, null);
	}
	#endregion
	#region EffectListView
	public void CloseRandCardListView()
	{
		player.PlayerEndWork();
		RandomCardListView.SetActive(false);
	}
	public void CloseChooseEffectToActivatePanel()
	{
		whatwork = WhatWork.Null;
		player.PlayerEndWork();
		ChooseEffectToActivatePanel.SetActive(false);
		foreach (Transform child in ChooseEffectToActivateParent.transform)
		{
			Destroy(child.gameObject);
		}
		player.CancelEffect();
	}
	public void SeeCardsToActivate(List<Card> cardlist)
	{
		ChooseEffectToActivatePanel.SetActive(true);
		foreach (Card card in cardlist)
		{
			GameObject cardobject = Instantiate(CardPrefab);
			cardobject.transform.SetParent(ChooseEffectToActivateParent, false);
			cardobject.GetComponent<Image>().sprite = card.CardImage;
			cardobject.GetComponent<CardEvent>().card = card;
			cardobject.GetComponent<CardEvent>().player = player;
			cardobject.GetComponent<CardEvent>().playertype = player.GetPlayerType();
			cardobject.SetActive(true);
		}
		whatwork = WhatWork.ChainEffect;
	}
	#endregion
	#region UI
	public void NextPageBtn()
	{
		if (player.myturn)
		{
			if (player.WorkLeft) player.NoWorkLeft();
			player.ToPageEnd();
		}
	}
	public void EndPageBtnJob()
	{
		if (player.myturn)
		{
			if (player.WorkLeft) player.NoWorkLeft();
			player.ToEndPage();
		}
	}
	#endregion
	#endregion
	#region CardList views
	public void SeeCardList(Player ownplayer, CardPosition pos)
	{
		RandomCardListView.gameObject.SetActive(true);
		for (int i = 0; i < RandomCardListViewParent.childCount; i++)
		{
			Transform childTransform = RandomCardListViewParent.GetChild(i);
			GameObject childGameObject = childTransform.gameObject;
			Card card = childGameObject.GetComponent<CardEvent>().card;
			if (card.GetOriginOwner() == ownplayer && card.pos == pos)
			{
				childGameObject.SetActive(true);
			}
			else
			{
				childGameObject.SetActive(false);
			}
		}
	}
	public void MyGraveView()
	{
		if (playmanager.SomeoneWorking()) return;
		player.PlayerOnWork();
		SeeCardList(player, CardPosition.Grave);
	}
	public void MyDeckView()
	{
		if (playmanager.SomeoneWorking()) return;
		player.PlayerOnWork();
		SeeCardList(player, CardPosition.MainDeck);
	}
	public void MyExtraDeckView()
	{
		if (playmanager.SomeoneWorking()) return;
		player.PlayerOnWork();
		SeeCardList(player, CardPosition.ExtraDeck);
	}
	public void MyExiledView()
	{
		if (player.GetIsWorking()) return;
		player.PlayerOnWork();
		SeeCardList(player, CardPosition.Exiled);
	}
	public void EnemyGraveView()
	{
		if (playmanager.SomeoneWorking()) return;
		player.PlayerOnWork();
		SeeCardList(player.enemy, CardPosition.Grave);
	}
	public void EnemyDeckView()
	{
		if (playmanager.SomeoneWorking()) return;
		player.PlayerOnWork();
		SeeCardList(player.enemy, CardPosition.MainDeck);
	}
	public void EnemyExtraDeckView()
	{
		if (playmanager.SomeoneWorking()) return;
		player.PlayerOnWork();
		SeeCardList(player.enemy, CardPosition.ExtraDeck);
	}
	public void EnemyExiledView()
	{
		if (playmanager.SomeoneWorking()) return;
		player.PlayerOnWork();
		SeeCardList(player.enemy, CardPosition.Exiled);
	}
	#endregion
	#region Field Action Functions
	public void Attack()
	{
		MonsterFieldActionPanel.SetActive(false);
		whatwork = WhatWork.Attack;
		bool directAttack = true;
		foreach (Card card in player.enemy.MonsterField)
		{
			if (card != null) directAttack = false;
		}
		if (directAttack)
		{
			whatwork = WhatWork.Null;
			player.DirectAttack(cardonwork);
		}
	}
	public void MonsterEffectOnField()
	{
		MonsterFieldActionPanel.SetActive(false);
	}
	public void ChangeBattlePosition()
	{
		player.ChangeBattlePosition(cardonwork);
		MonsterFieldActionPanel.SetActive(false);
	}
	public void ActivateMagicTrapOnField()
	{
		player.MagicTrapEffectOnField(cardonwork);
		MagicFieldActivationPanel.SetActive(false);
	}
	#endregion
}
