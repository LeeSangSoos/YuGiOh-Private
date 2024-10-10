using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
	public GameManagerScript gamemanager;
	public Text decksizenotice;
	int mydecksize, myextrasize, aidecksize, aiextrasize;
	public Text UserOrAiText;

	private void Start()
	{
		mydecksize = JsonSaveLoad.LoadMyDeckData().Count;
		myextrasize = JsonSaveLoad.LoadMyExtraDeckData().Count;
		aidecksize = JsonSaveLoad.LoadAiDeckData().Count;
		aiextrasize = JsonSaveLoad.LoadAiExtraDeckData().Count;

		decksizenotice.text = "";
		decksizenotice.text += ("UserDeckSize: " +	mydecksize.ToString() + "\n");
		decksizenotice.text += ("UserExtraDeckSize: " + myextrasize.ToString() + "\n");
		decksizenotice.text += ("AiDeckSize: " + aidecksize.ToString() + "\n");
		decksizenotice.text += ("AiExtraDeckSize: " + aiextrasize.ToString());
		gamemanager = Object.FindFirstObjectByType<GameManagerScript>();

		if (gamemanager.IsUser)
		{
			UserOrAiText.text = "Player: User";
		}
		else
		{
			UserOrAiText.text = "Player: Ai";
		}
	}
	public void ToDeckEdit()
	{
		SceneManager.LoadScene("DeckEditScene");
		GameManagerScript.editmydeck = true;
	}

	public void ToAiDeckEdit()
	{
		SceneManager.LoadScene("DeckEditScene");
		GameManagerScript.editmydeck = false;
	}
	public void UserOrAi()
	{
		if (gamemanager.IsUser)
		{
			UserOrAiText.text = "Player: Ai";
			gamemanager.setisuser(false);
		}
		else {
			UserOrAiText.text = "Player: User";
			gamemanager.setisuser(true);
		}
	}
	public void StartPlay()
	{
		if (mydecksize >= 40 && mydecksize <= 60 && myextrasize <=15 &&
			aidecksize >= 40 && aidecksize <= 60 && aiextrasize <= 15)
		{
			SceneManager.LoadScene("PlayScene");
		}
	}

	public void ResestDeck()
	{
		resetfile(Path.Combine(Application.persistentDataPath, "MyDeckData.json"));
		resetfile(Path.Combine(Application.persistentDataPath, "MyExtraDeckData.json"));
		resetfile(Path.Combine(Application.persistentDataPath, "AiDeckData.json"));
		resetfile(Path.Combine(Application.persistentDataPath, "AiExtraDeckData.json"));
	}

	void resetfile(string path)
	{
		if (File.Exists(path))
		{
			File.Delete(path);
			Debug.Log(path + " file is Deleted");
			if (!File.Exists(path))
			{
				List<Card> Deck = new List<Card>();
				string json = JsonUtility.ToJson(Deck);
				File.WriteAllText(path, json);
				Debug.Log(path + " file is Reseted");
			}
		}
		else
		{
			Debug.Log("fileDoesn't exists: " + path);
		}
	}

}
