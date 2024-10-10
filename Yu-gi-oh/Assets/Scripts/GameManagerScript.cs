using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
	public static GameManagerScript instance;
	public static bool editmydeck;
	private static bool OnGameStart = true;

	[SerializeField]
	private List<Card> CardData1;
	private static List<Card> CardData2 = new List<Card>();
	public static List<Card> ALLCardList
	{
		get { return CardData2; }
	}
	private List<IEffect> effects = new List<IEffect>();
	public List<IEffect> IEffects { get { return effects; } }

	[SerializeField]
	private bool isuser;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);  // Destroy the new instance
			return;                    // Exit the Awake method
		}
		else if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			Application.targetFrameRate = 60;
		}
		MakeJsonFiles();
		DontDestroyOnLoad(gameObject);

		if (OnGameStart)
		{
			foreach (Card card in CardData1)
			{
				CardData2.Add(card);
			}
			isuser = true;
		}
		OnGameStart = false;
	}

	// Check and make json files
	private void MakeJsonFiles()
	{
		string myDeckFilePath = Path.Combine(Application.persistentDataPath, "MyDeckData.json");
		string myExtraDeckFilePath = Path.Combine(Application.persistentDataPath, "MyExtraDeckData.json");
		string aiDeckFilePath = Path.Combine(Application.persistentDataPath, "AiDeckData.json");
		string aiExtraDeckFilePath = Path.Combine(Application.persistentDataPath, "AiExtraDeckData.json");
		List<string> DeckData = new List<string>();
		string json = JsonUtility.ToJson(DeckData);

		if (!File.Exists(myDeckFilePath))
		{
			File.WriteAllText(myDeckFilePath, json);
		}
		if (!File.Exists(myExtraDeckFilePath))
		{
			File.WriteAllText(myExtraDeckFilePath, json);
		}
		if (!File.Exists(aiDeckFilePath))
		{
			File.WriteAllText(aiDeckFilePath, json);
		}
		if (!File.Exists(aiExtraDeckFilePath))
		{
			File.WriteAllText(aiExtraDeckFilePath, json);
		}

	}
	public void setisuser(bool setting)
	{
		isuser = setting;
	}

	public bool IsUser
	{
		get { return isuser; }
	}
}
