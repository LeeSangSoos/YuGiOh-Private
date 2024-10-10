using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvent : MonoBehaviour, IPointerClickHandler
{
	public Player player;
	public Card card;
	public PlayerType playertype;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (playertype == PlayerType.Ai) return;
		if(player.UserScript.whatwork != WhatWork.ChainEffect && !player.myturn) return;
		player.UserAction(card);
	}
}