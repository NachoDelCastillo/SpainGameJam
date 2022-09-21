using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMultiplayerManager : MonoBehaviour
{
    static LocalMultiplayerManager instance;

    public static LocalMultiplayerManager GetInstance()
    { return instance; }

    PlayerInputManager playerInputManager;

    // A list with all the players
    List<PlayerController_2D> allPlayers;

    [Header("Parameters")]
    // Empty object that contains all the players as childs
    [SerializeField] Transform playerContainer;

    [SerializeField] Transform respawnPoint;
    [SerializeField] float respawnTime;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        playerInputManager = GetComponent<PlayerInputManager>();

        allPlayers = new List<PlayerController_2D>();
        //playerInputManager.JoinPlayer(1);

        //playerInputManager.JoinPlayer();
        //playerInputManager.JoinPlayer(2);
    }

    // This function is called everytime a player joined
    public void PlayerJoined(PlayerInput newplayer)
    {
        // Get a reference
        PlayerController_2D newPlayerController = newplayer.GetComponent<PlayerController_2D>();

        // Set player index
        int playerIndex = playerInputManager.playerCount - 1;
        newPlayerController.playerIndex = playerIndex;
        newPlayerController.transform.SetParent(playerContainer);

        allPlayers.Add(newPlayerController);
    }

    public void Respawn(PlayerController_2D player)
    { StartCoroutine(Respawn2(player, respawnTime)); }

    IEnumerator Respawn2(PlayerController_2D player, float time)
    {
        player.gameObject.SetActive(false); 
        yield return new WaitForSeconds(time);
        player.gameObject.SetActive(true); 
        player.transform.position = respawnPoint.position;
    }
}