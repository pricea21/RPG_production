using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
	[Header("Players")]
	public string playerPrefabPath;
	public PlayerController[] players;
	public Transform[] spawnPoints;
	public float respawnTime;
	public int bossCurHp;

	private int playersInGame;

	//instance
	public static GameManager instance;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		players = new PlayerController[PhotonNetwork.PlayerList.Length];
		photonView.RPC("ImInGame", RpcTarget.AllBuffered);
	}

	[PunRPC]
	void ImInGame()
	{
		playersInGame++;

		if(playersInGame == PhotonNetwork.PlayerList.Length)
			SpawnPlayer();
	}

	void SpawnPlayer()
	{
		GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

		//initialize player 
		playerObj.GetComponent<PhotonView>().RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
	}

	public void CheckWinCondition()
    {
        if (bossCurHp == 0)
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
    }

    [PunRPC]
    void WinGame(int winningPlayer)
    {
        // set the UI win text
        GameUI.instance.SetWinText();
    }
}