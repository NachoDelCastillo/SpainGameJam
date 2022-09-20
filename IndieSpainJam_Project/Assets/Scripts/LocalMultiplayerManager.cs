using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMultiplayerManager : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    // A list with all the players
    List<PlayerController_2D> allPlayers;

    [Header("Parameters")]
    // Empty object that contains all the players as childs
    [SerializeField] Transform playerContainer;

    private void Awake()
    {
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
}
