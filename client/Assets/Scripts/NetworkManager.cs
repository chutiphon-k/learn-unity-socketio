using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {
	public static NetworkManager instance;
	public Canvas canvas;
	// public SocketIOComponent socket;
	public InputField playerNameInput;
	public GameObject player;

	void Awake()
	{
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);
			DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	public void JoinGame(){
		StartCoroutine(ConnectToServer());
	}

	IEnumerator ConnectToServer(){
		yield return new WaitForSeconds(0.5f);
	}

	[Serializable]
	public class PlayerJSON {
		public string name;
		public List<PointJSON> playerSpawnPoints;
		public List<PointJSON> enemySpawnPoints;

		public PlayerJSON(string _name, List<SpawnPoint> _playerSpawnPoints, List<SpawnPoint> _enemySpawnPoints){
			playerSpawnPoints = new List<PointJSON>();
			enemySpawnPoints = new List<PointJSON>();
			name = _name;
			foreach(SpawnPoint playerSpawnPoint in _playerSpawnPoints){
				PointJSON pointJSON = new PointJSON(playerSpawnPoint);
				playerSpawnPoints.Add(pointJSON);
			}
			foreach(SpawnPoint enemySpawnPoint in _enemySpawnPoints){
				PointJSON pointJSON = new PointJSON(enemySpawnPoint);
				playerSpawnPoints.Add(pointJSON);
			}
		}
	}

	[Serializable]
	public class PointJSON {
		public PointJSON(SpawnPoint spawnPoint){
			
		}
	}
}
