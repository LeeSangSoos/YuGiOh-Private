using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class JsonSaveLoad
{
	public static void SaveMyDeckData(List<Card> Deck, List<Card> ExtraDeck)
	{
		Json_Deckclass deckdata = new Json_Deckclass();
		foreach (Card card in Deck)
		{
			deckdata.deck.Add(card.CardName);
		}

		string json = JsonUtility.ToJson(deckdata);
		string filePath = Path.Combine(Application.persistentDataPath, "MyDeckData.json");
		File.WriteAllText(filePath, json);

		Json_Deckclass extradata = new Json_Deckclass();
		foreach (Card card in ExtraDeck)
		{
			extradata.deck.Add(card.CardName);
		}
		json = JsonUtility.ToJson(extradata);
		filePath = Path.Combine(Application.persistentDataPath, "MyExtraDeckData.json");
		File.WriteAllText(filePath, json);
	}
	public static List<Card> LoadMyDeckData()
	{
		//json에서 덱 카드들의 이름 읽어오기
		string filePath = Path.Combine(Application.persistentDataPath, "MyDeckData.json");
		string json = File.ReadAllText(filePath);
		Json_Deckclass deckdata = JsonUtility.FromJson<Json_Deckclass>(json);

		//GameManager의 카드 리스트에서 카드명에 따른 카드들 로드
		List<Card> Deck = new List<Card>();
		foreach (string cardname in deckdata.deck)
		{
			Card originalCard = GameManagerScript.ALLCardList.Find(match => match.CardName == cardname);
			Card copiedCard = CreateCardInstance(originalCard);
			copiedCard.CopyProperties(originalCard);
			Deck.Add(copiedCard);
		}
		return Deck;
	}
	public static List<Card> LoadMyExtraDeckData()
	{
		string filePath = Path.Combine(Application.persistentDataPath, "MyExtraDeckData.json");
		string json = File.ReadAllText(filePath);
		Json_Deckclass ExtraDeckData = JsonUtility.FromJson<Json_Deckclass>(json);

		List<Card> ExtraDeck = new List<Card>();
		foreach (string cardname in ExtraDeckData.deck)
		{
			Card originalCard = GameManagerScript.ALLCardList.Find(match => match.CardName == cardname);
			Card copiedCard = CreateCardInstance(originalCard);
			copiedCard.CopyProperties(originalCard);
			ExtraDeck.Add(copiedCard);
		}

		return ExtraDeck;
	}
	public static void SaveAiDeckData(List<Card> Deck, List<Card> ExtraDeck)
	{
		Json_Deckclass deckdata = new Json_Deckclass();
		foreach (Card card in Deck)
		{
			deckdata.deck.Add(card.CardName);
		}
		string json = JsonUtility.ToJson(deckdata);
		string filePath = Path.Combine(Application.persistentDataPath, "AiDeckData.json");
		File.WriteAllText(filePath, json);

		Json_Deckclass extradata = new Json_Deckclass();
		foreach (Card card in ExtraDeck)
		{
			extradata.deck.Add(card.CardName);
		}
		json = JsonUtility.ToJson(extradata);
		filePath = Path.Combine(Application.persistentDataPath, "AiExtraDeckData.json");
		File.WriteAllText(filePath, json);
	}
	public static List<Card> LoadAiDeckData()
	{
		string filePath = Path.Combine(Application.persistentDataPath, "AiDeckData.json");
		string json = File.ReadAllText(filePath);
		Json_Deckclass deckdata = JsonUtility.FromJson<Json_Deckclass>(json);

		List<Card> Deck = new List<Card>();
		foreach (string cardname in deckdata.deck)
		{
			Card originalCard = GameManagerScript.ALLCardList.Find(match => match.CardName == cardname);
			Card copiedCard = CreateCardInstance(originalCard);
			copiedCard.CopyProperties(originalCard);
			Deck.Add(copiedCard);
		}
		return Deck;
	}
	public static List<Card> LoadAiExtraDeckData()
	{
		string filePath = Path.Combine(Application.persistentDataPath, "AiExtraDeckData.json");
		string json = File.ReadAllText(filePath);
		Json_Deckclass ExtraDeckData = JsonUtility.FromJson<Json_Deckclass>(json);

		List<Card> ExtraDeck = new List<Card>();
		foreach (string cardname in ExtraDeckData.deck)
		{
			Card originalCard = GameManagerScript.ALLCardList.Find(match => match.CardName == cardname);
			Card copiedCard = CreateCardInstance(originalCard);
			copiedCard.CopyProperties(originalCard);
			ExtraDeck.Add(copiedCard);
		}

		return ExtraDeck;
	}

	public static Card CreateCardInstance(Card originalCard)
	{
		if (originalCard is MonsterCard)
		{
			return ScriptableObject.CreateInstance<MonsterCard>();
		}
		else if (originalCard is MagicCard)
		{
			return ScriptableObject.CreateInstance<MagicCard>();
		}
		else if (originalCard is TrapCard)
		{
			return ScriptableObject.CreateInstance<TrapCard>();
		}
		else { Debug.LogError("Card Creation from jsonfile is not type of anything"); return ScriptableObject.CreateInstance<Card>();  }
	}
}