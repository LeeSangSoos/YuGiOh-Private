using UnityEngine;
using UnityEngine.UI;

public class EachCardinDeck : MonoBehaviour
{
	public Image image;
	private DeckEditManager deckEditManager;
	public Card card;
	public Button button;

	private void Start()
	{
		button.onClick.AddListener(Drawthis);
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
	public void Drawthis()
	{
		deckEditManager.DrawfromDeck(card);
	}

}
