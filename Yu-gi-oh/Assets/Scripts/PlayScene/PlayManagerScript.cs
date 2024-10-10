using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayManagerScript : MonoBehaviour
{
	#region values
	//Turn & pages
	Turn turn;
	int TotalTurn = 1;
	Page gamepage;
	PageTime pageTime = PageTime.Start;
	public Text turntext;

	//Users
	public Player player_12;
	public Player player_6;
	Dictionary<Turn, Player> playermap = new Dictionary<Turn, Player>();
	Player player;
	public bool player12_chain = false;
	public bool player6_chain = false;

	//Manager work
	bool managerworkleft;
	public GameManagerScript gamemanager;
	private List<Card> CardsInGame = new List<Card>();
	Stack<Card> chainList = new Stack<Card>();
	Stack<Trigger> triggerList = new Stack<Trigger>();
	public Trigger triggerjustactivated;
	public Card WhenActivatedCard; // ¶§
	public List<Card> IfActivatedCard; //°æ¿ì
	public List<Card> recentCards;

	//UI objects
	public GameObject CardPrefab;
	public GameObject EndPageBtn;
	public GameObject NoticeBoard;
	public Transform RandomCardListViewParent;
	public List<GameObject> atklist;
	public List<GameObject> deflist;

	//Boolean values
	public bool main1toend = false;
	public bool ChainOnProcess = false;
	#endregion
	#region Getter & Setter
	public void ManagerWorkStart() { //Debug.Log("Manager work on"); 
		managerworkleft = true; }
	public void ManagerWorkEnd() { //Debug.Log("Manager work End"); 
		managerworkleft = false; }
	#endregion
	#region TimeFunctions
	private void Awake()
	{
		gamemanager = Object.FindFirstObjectByType<GameManagerScript>();
		if (gamemanager.IsUser)
		{
			player_6.GetComponent<AiPlayer>().enabled = false;
			player_6.GetComponent<User>().enabled = true;
			player_6.setPlayerType(PlayerType.User);
		}
		else
		{
			player_6.GetComponent<AiPlayer>().enabled = true;
			player_6.GetComponent<User>().enabled = false;
			player_6.setPlayerType(PlayerType.Ai);
		}
		SettingPanenl.SetActive(false);
	}
	private void Start()
	{
		NoticeBoard.SetActive(false);
		#region Set Decks
		player_6.MainDeck = (JsonSaveLoad.LoadMyDeckData());
		player_6.ExtraDeck = (JsonSaveLoad.LoadMyExtraDeckData());
		player_12.MainDeck = (JsonSaveLoad.LoadAiDeckData());
		player_12.ExtraDeck = (JsonSaveLoad.LoadAiExtraDeckData());

		InitializeDeck(player_6, player_6.MainDeck);
		InitializeDeck(player_12, player_12.MainDeck);

		player_6.MainDeck = (Shuffle(player_6.MainDeck));
		player_12.MainDeck = (Shuffle(player_12.MainDeck));
		#endregion
		#region Set Player AI and user
		playermap[Turn.Six] = player_6;
		playermap[Turn.Twelve] = player_12;
		player_6.setPlayerType(gamemanager.IsUser ? PlayerType.User : PlayerType.Ai);
		player_12.setPlayerType(PlayerType.Ai);
		#endregion
		#region Random Turn
		turn = Random.Range(0, 2) == 0 ? Turn.Twelve : Turn.Six;
		player = playermap[turn];
		player.myturn = true;
		TurnColor.color = turn == Turn.Six ? Constant.mycolor : Constant.aicolor;
		turntext.text = "Turn: " + TotalTurn;
		#endregion
		#region Game start
		player.SetTurnActions();
		gamepage = Page.Draw;

		for (int i = 0; i < 4; i++)
		{
			player_6.draw();
			player_12.draw();
		}
		player.enemy.draw();
		pageTime = PageTime.Start;
		#endregion
	}
	private void Update()
	{
		switch (gamepage)
		{
			case Page.Draw:
				DrawPageAction();
				break;
			case Page.Standby:

				break;
			case Page.Main1:

				break;
			case Page.Battle: break;
			case Page.Main2: break;
			case Page.End:
				EndAction();
				break;
		}
		if (player.LifePoint <= 0) { GameOver(player.enemy); }
		else if (player.enemy.LifePoint <= 0) { GameOver(player); }

		//Set atk, def value on field
		for (int i = 0; i < 10; i++)
		{
			if (i < 5)
			{
				if (player_6.MonsterField[i] != null)
				{
					MonsterCard mons = player_6.MonsterField[i] as MonsterCard;
					if (mons.iscardfaceup)
					{
						atklist[i].GetComponentInChildren<Text>().text = mons.atk.ToString();
						deflist[i].GetComponentInChildren<Text>().text = mons.def.ToString();
					}
				}
				else
				{
					atklist[i].GetComponentInChildren<Text>().text = "";
					deflist[i].GetComponentInChildren<Text>().text = "";
				}
			}
			else
			{
				int j = i - 5;
				if (player_12.MonsterField[j] != null)
				{
					MonsterCard mons = player_12.MonsterField[j] as MonsterCard;
					if (mons.iscardfaceup)
					{
						atklist[i].GetComponentInChildren<Text>().text = mons.atk.ToString();
						deflist[i].GetComponentInChildren<Text>().text = mons.def.ToString();
					}
				}
				else
				{
					atklist[i].GetComponentInChildren<Text>().text = "";
					deflist[i].GetComponentInChildren<Text>().text = "";
				}
			}
		}

	}
	private void LateUpdate()
	{
		if (!IsWorkLeft())
		{
			NextpageTime();
		}
	}
	#endregion
	#region Fuctions needed
	public List<Card> Shuffle(List<Card> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int randomIndex = Random.Range(0, i + 1);
			Card temp = list[i];
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}
		return list;
	}
	public int GetTotalTurn()
	{
		return TotalTurn;
	}

	public bool IsWorkLeft()
	{
		return player_6.WorkLeft || player_12.WorkLeft || managerworkleft;
	}
	public bool SomeoneWorking()
	{
		return player_6.GetIsWorking()|| player_12.GetIsWorking() || managerworkleft;
	}

	int cardnum = 0;
	private void InitializeDeck(Player player, List<Card> deck)
	{
		foreach (Card card in deck)
		{
			card.owner = player;
			card.SetOriginOwner(player);
			card.pos = CardPosition.MainDeck;
			card.SetCardNum(cardnum++);
			card.iscardfaceup = true;
			if (CardEffectMapping.EffectMap.TryGetValue(card.CardName, out IEffect effect))
			{
				card.effect = effect;
			}
			else
			{
				card.effect = new DefaultEffect();
			}
			GameObject cardobject = Instantiate(CardPrefab);
			cardobject.transform.SetParent(RandomCardListViewParent, false);
			cardobject.SetActive(false);

			CardEvent cardEventComponent = cardobject.GetComponent<CardEvent>();
			cardEventComponent.card = card;
			cardEventComponent.player = player;
			cardEventComponent.playertype = player.GetPlayerType();

			cardobject.name = card.CardName;
			card.SetCardObject(cardobject);
			card.playManager = this;
			card.availableCard = true;
			CardsInGame.Add(card);
		}
	}
	void GameOver(Player winner)
	{
		NoticeBoard.SetActive(true);
		NoticeBoard.GetComponentInChildren<Text>().text = ("Game Over : " + winner.name + " Won!");
		OpenSetting();
	}
	#endregion
	#region PageActions
	public Text PageText;
	public Image TurnColor;
	public void NextPage()
	{
		if (IsWorkLeft()) return;
		pageTime = PageTime.Start;
		switch (gamepage)
		{
			case Page.Draw:
				gamepage = Page.Standby;
				break;
			case Page.Standby:
				gamepage = Page.Main1;
				EndPageBtn.SetActive(true);
				break;
			case Page.Main1:
				if (TotalTurn == 1 || main1toend) { gamepage = Page.End; main1toend = false; }
				else gamepage = Page.Battle;
				EndPageBtn.SetActive(false);
				break;
			case Page.Battle:
				gamepage = Page.Main2;
				break;
			case Page.Main2:
				gamepage = Page.End;
				break;
			case Page.End:
				turn = turn == Turn.Six ? Turn.Twelve : Turn.Six;
				player.myturn = false;
				player = playermap[turn];
				player.myturn = true;
				player.SetTurnActions();
				TurnColor.color = turn == Turn.Six ? Constant.mycolor : Constant.aicolor;
				TotalTurn++;
				gamepage = Page.Draw;
				turntext.text = "Turn: " + TotalTurn;
				break;
		}
		PageText.text = gamepage.ToString();
	}
	private void NextpageTime()
	{
		switch (pageTime)
		{
			case PageTime.Start:
				pageTime = PageTime.OnGoing;
				if (gamepage == Page.Main1 || gamepage == Page.Main2 || gamepage == Page.Battle)
					player.WaitForPageWork();
				break;
			case PageTime.OnGoing:
				pageTime = PageTime.End;
				break;
			case PageTime.End:
				NextPage();
				break;
			default: Debug.LogError("NextPageTime : No Matching PageTime"); break;
		}
	}
	public PageTime GetPageTime()
	{
		return pageTime;
	}
	public Page GetPage()
	{
		return gamepage;
	}
	private void DrawPageAction()
	{

	}

	private void EndAction()
	{
		if (pageTime == PageTime.End)
		{
			if (player.Hand.Count > 6)
			{
				if (!managerworkleft) ManagerWorkStart();
				NoticeBoard.SetActive(true);
				NoticeBoard.GetComponentInChildren<Text>().text = "Choose Card to Discard from hand";
				player.HandOver6();
			}
			else { NoticeBoard.SetActive(false); if (managerworkleft) ManagerWorkEnd(); }
		}
	}
	#endregion
	#region Info for Ai
	public Turn GetTurn
	{
		get { return turn; }
		set { }
	}

	#endregion
	#region Function for Ai

	#endregion
	#region Funtions for UI & Button in game
	public GameObject SettingPanenl;
	public void OpenSetting()
	{
		SettingPanenl.SetActive(true);
	}
	public void CloseSetting()
	{
		SettingPanenl.SetActive(false);
	}
	public void Surrender()
	{
		SceneManager.LoadScene("MenuScene");
	}
	#endregion
	#region Functions for Info exchange between players
	public List<Card> DeckInfoRead(Player returner)
	{
		return returner.MainDeck.Select(card =>
		{
			Card newCard = new Card();
			newCard.CopyProperties(card);
			return newCard;
		}).ToList();
	}
	public List<Card> GraveInfoRead(Player returner)
	{
		return returner.Grave.Select(card =>
		{
			Card newCard = new Card();
			newCard.CopyProperties(card);
			return newCard;
		}).ToList();
	}
	public List<Card> HandInfoRead(Player returner)
	{
		return returner.Hand.Select(card =>
		{
			Card newCard = new Card();
			newCard.CopyProperties(card);
			return newCard;
		}).ToList();
	}
	public List<Card> ExiledInfoRead(Player returner)
	{
		return returner.Exiled.Select(card =>
		{
			Card newCard = new Card();
			newCard.CopyProperties(card);
			return newCard;
		}).ToList();
	}
	public List<Card> ExtraInfoRead(Player returner)
	{
		return returner.ExtraDeck.Select(card =>
		{
			Card newCard = new Card();
			newCard.CopyProperties(card);
			return newCard;
		}).ToList();
	}

	#endregion
	#region CardActivationValidation Check Functions
	public bool CheckActiveValid(Card card, Trigger trigger)
	{
		if ((card.trigger != trigger && card.trigger != Trigger.Any) || card.thisCardUsed)
		{
			return false;
		}
		if (card.EffectCondition() && card is TrapCard && card.GetAutoTarget()!=null)
		{
			Debug.Log(card.GetAutoTarget().ToString());
			Debug.Log("list : " + card.name);
			return true;
		}
		return false;
	}
	public List<Card> ValidCardList(Player user, Trigger trigger, Card targetcard)
	{
		List<Card> cards = new List<Card>();
		foreach (Card card in CardsInGame)
		{
			if (card.owner == user)
			{
				if (CheckActiveValid(card, trigger))
				{
					card.availableCard = true;
					cards.Add(card);
				}
				else
				{
					card.availableCard = false;
				}
			}
		}
		return cards;
	}
	public void AddTrigger(Trigger trigger)
	{
		ManagerWorkStart();
		triggerjustactivated = trigger;
		triggerList.Push(trigger);
		player12_chain = true;
		player6_chain = true;
	}
	public void ChainProcess(Player sender, Card recentactivatedcard)
	{
		Debug.Log(sender + " started chain : " + recentactivatedcard);
		ChainOnProcess = true;
		List<Card> cards = new List<Card>(); //Card Able to activate
		WhenActivatedCard = recentactivatedcard;
		recentactivatedcard.thisCardUsed = true;
		recentCards.Add(recentactivatedcard);

		//Iterate through triggers and add enabled cards
		foreach (Trigger trig in triggerList)
		{
			cards.AddRange(ValidCardList(sender.enemy, trig, recentactivatedcard));
		}
		//If there is card able to activate, call activation method of enemy
		if (cards.Count > 0)
		{
			sender.enemy.CheckCardListForActivate(cards, recentactivatedcard);
		}
		else
		{//If not, call deactivation method
			ChainProcess2(sender.enemy, recentactivatedcard);
		}
	}
	//No activation process
	public void ChainProcess2(Player enemy, Card recentactivatedcard)
	{
		Debug.Log("CainP2 : " + enemy);
		//Set current player Activation to false
		if (enemy == player_12) player12_chain = false;
		else player6_chain = false;

		//If Both players activation is false, break chain and execute
		if (player12_chain == false && player6_chain == false)
		{
			StartCoroutine(ExecuteChainEffect());
			return;
		}
		else //If one is still available, call chain for enemy again
		{
			ChainProcess(enemy, recentactivatedcard);
		}
	}
	public void AddChainEffect(Card card)
	{
		card.cardNameUsed1 = true;
		card.thisCardUsed = true;
		if (card is TrapCard) {
			int index = card.owner.MagicTrapField.IndexOf(card);
			card.owner.MagicZone[index].GetComponent<Image>().sprite = card.CardImage;
			card.iscardfaceup = true;
		}
		chainList.Push(card);
	}
	public IEnumerator ExecuteChainEffect()
	{
		yield return new WaitForSeconds(0.2f);
		if (chainList.Count > 0)
			Debug.Log("Chain Execution Start : ");
		foreach (Card card in chainList)
		{
			Debug.Log(card.ToString());
			card.Effect();
			card.cardNameUsed1 = false;
			card.thisCardUsed = false;
			if (card is MagicCard)
			{
				MagicCard magicCard = (MagicCard)card;
				if (magicCard.magictype == MagicCardtype.Equip)
				{
					card.thisCardUsed = true;
				}
			}
		}
		foreach (Card card in chainList)
		{
			switch (card.pos)
			{
				case CardPosition.MonsterField:
					card.owner.OutOfMonsterField(card.owner.MonsterField.IndexOf(card));
					card.owner.ToGrave(card);
					break;
				case CardPosition.MagicField:
					if (card is MagicCard)
					{
						MagicCard magic = card as MagicCard;
						if (magic.magictype == MagicCardtype.Normal)
						{
							card.owner.OutOfMagicField(card.owner.MagicTrapField.IndexOf(card));
							card.owner.ToGrave(card);
						}
					}
					else
					{
						card.owner.OutOfMagicField(card.owner.MagicTrapField.IndexOf(card));
						card.owner.ToGrave(card);
					}
					break;
				default: break;
			}
		}
		chainList.Clear();
		triggerList.Clear();
		triggerjustactivated = Trigger.None;
		foreach (Card card in CardsInGame)
		{
			card.availableCard = true;
		}
		ChainOnProcess = false;
		recentCards.Clear();
		ManagerWorkEnd();
	}
	#endregion
	#region CardEffect Functions
	public void DestroyCard(Card card, DeathReason deathReason)
	{
		CardPosition pos = card.pos;
		Player ownplayer = card.owner;
		int index;
		Card result;
		switch (pos)
		{
			case CardPosition.Hand: break;
			case CardPosition.MonsterField:
				index = ownplayer.MonsterField.IndexOf(card);
				result = ownplayer.OutOfMonsterField(index);
				ownplayer.ToGrave(result);
				break;
			case CardPosition.MagicField:
				index = ownplayer.MagicTrapField.IndexOf(card);
				result = ownplayer.OutOfMagicField(index);
				ownplayer.ToGrave(result);
				break;
			case CardPosition.FieldMagic: break;
			case CardPosition.MainDeck: break;
			case CardPosition.ExtraDeck: break;
			default: break;
		}
	}
	#endregion
}
