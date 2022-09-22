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

        // Todo esto es para desactivar el render específico del sprite
        SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>();
        SpriteRenderer spRenderer = null;
        for (int x = 0; x < renderers.Length; x++)
        {
            if (renderers[x].transform.CompareTag("Animator"))
            {
                //Debug.Log("Tag animator encontrado");
                spRenderer = renderers[x];
                continue;
            }

        }

        if (spRenderer) spRenderer.enabled = false;
        // y tmb desactivar PlayerController_2D para que no se instancie otro player cuando este está muerto
        player.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        player.enabled = false;

        UIManager.GetInstance().CreateSpawnTimer(player.gameObject.transform, time);


        yield return new WaitForSeconds(time);

        if (spRenderer)spRenderer.enabled = true;

        player.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        player.enabled = true;
        player.transform.position = respawnPoint.position;
    }
}