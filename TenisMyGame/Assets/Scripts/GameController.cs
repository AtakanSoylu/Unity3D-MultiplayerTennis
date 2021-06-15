using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
public class GameController : NetworkBehaviour
{
    public GameObject playerOneScoreLabel;
    public GameObject playerTwoScoreLabel;
    public Text playerOneScoreText;
    public Text playerTwoScoreText;
    public Text winnerText;
    public static float gravity = -9f;
    public static bool lastHitPlayerOne;
    public static bool isGameReady;
    public static List<Player> players = new List<Player>();
    public Player LocalPlayer;

    void Start()
    {
        winnerText.enabled = false;
        playerOneScoreLabel.SetActive(false);
        playerTwoScoreLabel.SetActive(false);
        isGameReady = false;
    }

    void Update()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            GameReadyCheck();
            if (isGameReady)
            {
                if (LocalPlayer == null)
                {
                    FindLocalPlayer();
                }
                else
                {
                    playerOneScoreLabel.SetActive(true);
                    playerTwoScoreLabel.SetActive(true);
                    scoreUpdate();
                    isGameOver();
                }
            }

        }
        else
        {
            isGameReady = false;
            players.Clear();
        }

    }
    void GameReadyCheck()
    {
        if (!isGameReady)
        {
            foreach (KeyValuePair<uint, NetworkIdentity> kvp in NetworkIdentity.spawned)
            {
                Player comp = kvp.Value.GetComponent<Player>();

                //Yeniyse Ekle
                if (comp != null && !players.Contains(comp))
                {
                    players.Add(comp);
                }
            }

            if (players.Count == 2)
            {
                isGameReady = true;
            }
        }
    }
    void FindLocalPlayer()
    {

        if (ClientScene.localPlayer == null)
            return;

        LocalPlayer = ClientScene.localPlayer.GetComponent<Player>();
    }
    void isGameOver()
    {
        foreach (Player player in players)
        {
            if (player.score >= 5)
            {
                winnerText.text = "Player "+ player.thisPlayer +" Won";
                winnerText.enabled = true;
                Application.Quit();
            }
        }
    }

    void scoreUpdate()
    {
        foreach (Player player in players)
        {
            if (player.thisPlayer == 1)
            {
                playerOneScoreText.text = "" + player.score;
            }
            else if (player.thisPlayer == 2) { 
                playerTwoScoreText.text = "" + player.score;
            }
        }
    }
}
