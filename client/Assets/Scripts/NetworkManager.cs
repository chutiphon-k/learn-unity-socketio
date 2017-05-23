using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class NetworkManager : MonoBehaviour {
	public static NetworkManager instance;
	public Canvas canvas;
	public SocketIOComponent socket;
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
		socket.On("enemies", OnEnemies);
		socket.On("other player connected", OnOtherPlayerConnected);
		socket.On("play", OnPlay);
		socket.On("player move", OnPlayerMove);
		socket.On("player turn", OnPlayerTurn);
		socket.On("player shoot", OnPlayerShoot);
		socket.On("health", OnHealth);
		socket.On("other player disconnected", OnOtherPlayerDisconnect);
	}
	
	public void JoinGame(){
		StartCoroutine(ConnectToServer());
	}

	void OnEnemies(SocketIOEvent socketIOEvent){
		EnemiesJSON enemiesJSON = EnemiesJSON.CreateFromJSON(socketIOEvent.data.ToString());
		EnemySpawner es = GetComponent<EnemySpawner>();
		es.SpawnEnemies(enemiesJSON);
	}

	void OnOtherPlayerConnected(SocketIOEvent socketIOEvent){
		print("Someone else joined");
		string data = socketIOEvent.data.ToString();
		UserJSON userJSON = UserJSON.CreateFromJSON(data);
		Vector3 position = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);
		Quaternion rotation = Quaternion.Euler(userJSON.rotation[0], userJSON.rotation[1], userJSON.rotation[2]);
		GameObject o = GameObject.Find(userJSON.name) as GameObject;
		if(o != null){
			return ;
		}
		GameObject p = Instantiate(player, position, rotation) as GameObject;
		PlayerController pc = p.GetComponent<PlayerController>();
		Transform t = p.transform.Find("Healthbar Canvas");
		Transform t1 = t.transform.Find("Player Name");
		Text playerName = t1.GetComponent<Text>();
		playerName.text = userJSON.name;
		pc.isLocalPlayer = false;
		p.name = userJSON.name;
		Health h = p.GetComponent<Health>();
		h.currentHealth = userJSON.health;
		h.OnChangeHealth();
	}

	void OnPlay(SocketIOEvent socketIOEvent){
		print("you joined");
		string data = socketIOEvent.data.ToString();
		UserJSON currentUserJSON = UserJSON.CreateFromJSON(data);
		Vector3 position = new Vector3(currentUserJSON.position[0], currentUserJSON.position[1], currentUserJSON.position[2]);
		Quaternion rotation = Quaternion.Euler(currentUserJSON.rotation[0], currentUserJSON.rotation[1], currentUserJSON.rotation[2]);
		GameObject p = Instantiate(player, position, rotation) as GameObject;
		PlayerController pc = p.GetComponent<PlayerController>();
		Transform t = p.transform.Find("Healthbar Canvas");
		Transform t1 = t.transform.Find("Player Name");
		Text playerName = t1.GetComponent<Text>();
		playerName.text = currentUserJSON.name;
		pc.isLocalPlayer = true;
		p.name = currentUserJSON.name;
	}

	void OnPlayerTurn(SocketIOEvent socketIOEvent){

	}

	void OnPlayerMove(SocketIOEvent socketIOEvent){

	}

	void OnPlayerShoot(SocketIOEvent socketIOEvent){

	}

	void OnHealth(SocketIOEvent socketIOEvent){

	}

	void OnOtherPlayerDisconnect(SocketIOEvent socketIOEvent){
		print("user disconnected");
		string data = socketIOEvent.data.ToString();
		UserJSON userJSON = UserJSON.CreateFromJSON(data);
		Destroy(GameObject.Find(userJSON.name));
	}

	IEnumerator ConnectToServer(){
		yield return new WaitForSeconds(0.5f);

		socket.Emit("player connect");

		yield return new WaitForSeconds(1f);

		string playerName = playerNameInput.text;
		List<SpawnPoint> playerSpawnPoints = GetComponent<PlayerSpawner>().playerSpawnPoints;
		List<SpawnPoint> enemySpawnPoints = GetComponent<EnemySpawner>().enemySpawnPoints;
		PlayerJSON playerJSON = new PlayerJSON(playerName, playerSpawnPoints, enemySpawnPoints);
		string data = JsonUtility.ToJson(playerJSON);
		socket.Emit("play", new JSONObject(data));
		canvas.gameObject.SetActive(false);
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
				enemySpawnPoints.Add(pointJSON);
			}
		}
	}

	[Serializable]
	public class PointJSON {
		public float[] position;
		public float[] rotation;
		public PointJSON(SpawnPoint spawnPoint){
			position = new float[] {
				spawnPoint.transform.position.x,
				spawnPoint.transform.position.y,
				spawnPoint.transform.position.z
			};
			rotation = new float[] {
				spawnPoint.transform.eulerAngles.x,
				spawnPoint.transform.eulerAngles.y,
				spawnPoint.transform.eulerAngles.z
			};	
		}
	}

	[Serializable]
	public class PositionJSON {
		public float[] position;

		public PositionJSON(Vector3 _position){
			position = new float[] { _position.x, _position.y, _position.z };
		}
	}

	[Serializable]
	public class RotationJSON {
		public float[] rotation;

		public RotationJSON(Quaternion _rotation){
			rotation = new float[] {
				_rotation.eulerAngles.x,
				_rotation.eulerAngles.y,
				_rotation.eulerAngles.z
			};
		}
	}

	[Serializable]
	public class UserJSON {
		public string name;
		public float[] position;
		public float[] rotation;
		public int health;

		public static UserJSON CreateFromJSON(string data){
			return JsonUtility.FromJson<UserJSON>(data);
		}
	}

	[Serializable]
	public class HealthChangeJSON {
		public string name;
		public int healthChange;
		public string from ;
		public bool isEnemy;

		public HealthChangeJSON(string _name, int _healthChange, string _from, bool _isEnemy){
			name = _name;
			healthChange = _healthChange;
			from = _from;
			isEnemy = _isEnemy;
		}
	}

	[Serializable]
	public class EnemiesJSON {
		public List<UserJSON> enemies;

		public static EnemiesJSON CreateFromJSON(string data){
			return JsonUtility.FromJson<EnemiesJSON>(data);
		}
	}	

	[Serializable]
	public class ShootJSON {
		public string name;
		public static ShootJSON CreateFromJSON(string data) {
			return JsonUtility.FromJson<ShootJSON>(data);
		}
	}	

	[Serializable]
	public class UserHealthJSON {
		public string name;
		public int health;

		public static UserHealthJSON CreateFromJSON(string data){
			return JsonUtility.FromJson<UserHealthJSON>(data);			
		}
	}	
}
