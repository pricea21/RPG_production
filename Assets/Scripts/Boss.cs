using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Boss : MonoBehaviourPun
{
	[Header("Info")]
	public string enemyName;
	public float moveSpeed;

	public int bossCurHp;
	public int maxHp;

	public float chaseRange;
	public float attackRange;

	private PlayerController targetPlayer;

	public float playerDetectRate = 0.2f;
	private float lastPlayerDetectTime;

	public string objectToSpawnOnDeath;

	[Header("Attack")]
	public int damage;
	public float attackRate;
	private float lastAttackTime;

	[Header("Components")]
	public HeaderInfo healthBar;
	public SpriteRenderer sr;
	public Rigidbody2D rig;

	void Start()
	{
		healthBar.Initialize(enemyName, maxHp);
	}

	void Update()
	{
		if(!PhotonNetwork.IsMasterClient)
			return;


		if(targetPlayer != null)
		{
			//calculate the distance 
			float dist = Vector2.Distance(transform.position, targetPlayer.transform.position);

			//if were able to attack then do so
			if(dist < attackRange && Time.time - lastAttackTime >= attackRate)
				Attack();
			//otherwise do we move the player? 
			else if(dist > attackRange)
			{
				Vector3 dir = targetPlayer.transform.position - transform.position;
				rig.velocity = dir.normalized * moveSpeed;
			}
			else
			{
				rig.velocity = Vector2.zero;
			}
		}

		DetectPlayer();
	}

	//attacks the targeted player 
	void Attack()
	{
		lastAttackTime = Time.time;
		targetPlayer.photonView.RPC("TakeDamage", targetPlayer.photonPlayer, damage);
	}

	//update the targeted player 
	void DetectPlayer()
	{
		if(Time.time - lastPlayerDetectTime > playerDetectRate)

		//loop through all the players 
		foreach(PlayerController player in GameManager.instance.players)
		{
			//calculate distance between us and the player
			float dist = Vector2.Distance(transform.position, player.transform.position);

			if(player == targetPlayer)
			{
				if(dist > chaseRange)
					targetPlayer = null;
			}
			else if(dist < chaseRange)
			{
				if(targetPlayer == null)
					targetPlayer = player;
			}
		}
	}

	[PunRPC]
	public void TakeDamage(int damage)
	{
		bossCurHp -= damage;

		//update health bar 
		healthBar.photonView.RPC("UpdateHealthBar", RpcTarget.All, bossCurHp);

		if(bossCurHp <= 0)
			Die();
		else
		{
			photonView.RPC("FlashDamage", RpcTarget.All);
		}
	}

	[PunRPC]
	void FlashDamage()
	{
		StartCoroutine(DamageFlash());

		IEnumerator DamageFlash()
		{
			sr.color = Color.red;
			yield return new WaitForSeconds(0.05f);
			sr.color = Color.white;
		}
	}

	void Die()
	{
		if(bossCurHp == 0)
			GameManager.instance.CheckWinCondition();

		if(objectToSpawnOnDeath != string.Empty)
			PhotonNetwork.Instantiate(objectToSpawnOnDeath, transform.position, Quaternion.identity);

		//destroy the object across the network
		PhotonNetwork.Destroy(gameObject);
	}
}
