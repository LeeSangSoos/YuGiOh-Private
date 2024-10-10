using UnityEngine;
using UnityEngine.UI;

public class EachCardList : MonoBehaviour
{
	/*Info of each card in list of cards
	 1. image, text, card
	2. Set button event for adding the card to the deck
	 */
	public Image image;
	public Text text;
	private DeckEditManager deckEditManager;
	private Card card;
	public Button button;

	private void Start()
	{
		button.onClick.AddListener(Addthis);
	}

	//Fuction for setting the deckmanager for this script
	public void Setdeckmanager(DeckEditManager deckmanager)
	{
		deckEditManager = deckmanager;
	}

	public void SetCardinfo(Card card)
	{
		this.card = card;
	}

	//Function for adding the card to deck
	public void Addthis()
	{
		deckEditManager.AddToDeck(card);
	}

}