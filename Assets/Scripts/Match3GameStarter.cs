using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Match3GameStarter : MonoBehaviour
{
	[SerializeField]
	private Match3Game gamePrefab;

	[SerializeField]
	private Transform parent;

	private Match3Game _match3Game;

	[NonSerialized]
	private List<GameObject> createdGameObjects = new List<GameObject>();

	public void DestroyCreatedGameObjects()
	{
		for (int i = 0; i < createdGameObjects.Count; i++)
		{
			UnityEngine.Object.Destroy(createdGameObjects[i]);
		}
	}

	public Match3Game CreateGame()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(gamePrefab.gameObject, parent);
		createdGameObjects.Add(gameObject);
		gameObject.SetActive(value: true);
		return _match3Game = gameObject.GetComponent<Match3Game>();
	}

	public Match3Game GetGame()
	{
		return _match3Game;
	}
	
	public void ShowGame(bool state)
	{
		Debug.LogError(state);
		parent.gameObject.SetActive(state);
	}

	public void GoToMainScene()
	{
		SceneManager.LoadScene(0);
	}
}
