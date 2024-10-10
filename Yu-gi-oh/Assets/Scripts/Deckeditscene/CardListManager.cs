using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CardListManager : MonoBehaviour
{
	public GameObject cardListPrefab;
	public Transform listParent;
	public DeckEditManager deckEditManager;
	private GameManagerScript gameManagerScript;
	private List<Card> cardlist;

	private void Start()
	{
		gameManagerScript = Object.FindFirstObjectByType<GameManagerScript>();
		cardlist = GameManagerScript.ALLCardList;
		foreach (Card card in cardlist)
		{
			makelist(card);
		}
	}

	public void FindandDisplay(string s)
	{
		int childcount = listParent.childCount;
		for (int i = 0; i < childcount; i++) {
			Transform child = listParent.GetChild(i);
			Destroy(child.gameObject);
		}

		if(s == null || s.Length == 0)
		{
			foreach (Card card in cardlist)
			{
				makelist(card);
			}
		}

		foreach(Card card in cardlist)
		{
			if (card.CardName.Contains(s))
			{
				makelist(card);
			}
		}
	}

	/*Set info for each card in card list view
		 1. Make GameObject from Prefab of cardlistview
		2. Set image of object
		3. Set text of object
		4. Set DeckManager for script
		 */
	private void makelist(Card card)
	{
		GameObject cardGameObject = Instantiate(cardListPrefab, listParent);

		EachCardList cardSetting = cardGameObject.GetComponent<EachCardList>();
		cardSetting.image.sprite = card.CardImage;
		string cardname = Regex.Replace(card.CardName, @"(?<=\p{Ll})(\p{Lu})", " $1");
		cardSetting.text.text = cardname;
		cardSetting.Setdeckmanager(deckEditManager);
		cardSetting.SetCardinfo(card);
	}
}
