using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeckEditManager : MonoBehaviour
{
	public GameObject DeckCardPrefab;

	public List<Card> Decklist;
	public Transform DeckParent;

	public List<Card> ExtraDecklist;
	public Transform ExtraDeckParent;

	public EachCardinDeck[] cardobjects;
	public Text DeckSizetext;

	private void Start()
	{
		try
		{
			if (GameManagerScript.editmydeck)
			{
				Decklist = JsonSaveLoad.LoadMyDeckData();
				ExtraDecklist = JsonSaveLoad.LoadMyExtraDeckData();
			}
			else
			{
				Decklist = JsonSaveLoad.LoadAiDeckData();
				ExtraDecklist = JsonSaveLoad.LoadAiExtraDeckData();
			}
		}
		catch (FileNotFoundException ex)
		{
			// Handle the file not found error
			Debug.LogError("JSON file not found: " + ex.FileName);
		}

		if (Decklist != null)
		{
			foreach (Card card in Decklist)
			{
				GameObject cardGameObject = Instantiate(DeckCardPrefab, DeckParent);
				EachCardinDeck cardSetting = cardGameObject.GetComponent<EachCardinDeck>();
				cardSetting.image.sprite = card.CardImage;
				cardSetting.SetCardinfo(card);
				cardSetting.Setdeckmanager(this);
				DeckSizetext.text = Decklist.Count.ToString();
			}
		}
	}

	public void AddToDeck(Card card)
	{
		if (Decklist.Count >= 60) return;
		int count = 0;
		foreach (Card c in Decklist)
		{
			if (c.CardName == card.CardName)
			{
				count++;
			}
		}
		if (count == 3) return;
		GameObject cardGameObject = Instantiate(DeckCardPrefab, DeckParent);
		EachCardinDeck cardSetting = cardGameObject.GetComponent<EachCardinDeck>();
		cardSetting.image.sprite = card.CardImage;
		cardSetting.SetCardinfo(card);
		cardSetting.Setdeckmanager(this);
		Decklist.Add(card);
		DeckSizetext.text = Decklist.Count.ToString();
	}

	public void AddToExtraDeck()
	{
		foreach (Card card in ExtraDecklist)
		{
			GameObject cardGameObject = Instantiate(DeckCardPrefab, ExtraDeckParent);

			EachCardList cardSetting = cardGameObject.GetComponent<EachCardList>();
			cardSetting.image.sprite = card.CardImage;
		}
	}

	//Find card from Deck by card name and draw it from deck
	public void DrawfromDeck(Card card)
	{
		cardobjects = DeckParent.GetComponentsInChildren<EachCardinDeck>();
		foreach (EachCardinDeck child in cardobjects)
		{
			if (child.card.CardName == card.CardName)
			{
				child.transform.SetParent(null);

				Destroy(child.gameObject);
				Decklist.Remove(card);
				break;
			}
		}
		DeckSizetext.text = Decklist.Count.ToString();
	}

	public void ToMenu() => SceneManager.LoadScene("MenuScene");

	public void SaveDeck()
	{
		if(GameManagerScript.editmydeck)
			JsonSaveLoad.SaveMyDeckData(Decklist, ExtraDecklist);
		else
			JsonSaveLoad.SaveAiDeckData(Decklist, ExtraDecklist);
	}
}
