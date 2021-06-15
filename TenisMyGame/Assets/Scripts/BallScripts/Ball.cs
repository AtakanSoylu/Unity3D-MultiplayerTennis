using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Ball : NetworkBehaviour
{

    public GameObject lastPlayer;
    public GameObject anotherPlayer;
    List<Player> players;
    private void Start()
    {
        
        players = GameController.players;
    }
    void Update()
    {
        foreach (Player player in players)
        {
            if (lastPlayer.gameObject != player.gameObject)
            {
                anotherPlayer = player.gameObject;
            }
        }
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Player1in")|| col.transform.CompareTag("Player2in"))
        {
            //GameController.lastHitPlayerOne = !GameController.lastHitPlayerOne;
            lastPlayer.GetComponent<Player>().lastHit = false;
            anotherPlayer.GetComponent<Player>().lastHit = true;
        }
        else if (col.transform.CompareTag("Out"))
        {
            if (lastPlayer.GetComponent<Player>().lastHit)
            {
                anotherPlayer.GetComponent<Player>().score++;
                anotherPlayer.GetComponent<Player>().canServe = true;
                lastPlayer.GetComponent<Player>().canServe = false;
            }
            else
            {
                lastPlayer.GetComponent<Player>().score++;
                lastPlayer.GetComponent<Player>().canServe = true;
                anotherPlayer.GetComponent<Player>().canServe = false;
            }
            NetworkServer.Destroy(gameObject);
            GameController.gravity = -9f;

        }
    }
}
