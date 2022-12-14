using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
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
    [SerializeField] ParticleSystem pSystem;
    [SerializeField] GameObject sangrePart;
    [SerializeField] Color player1, player2;

    [SerializeField] TMP_Text numPlayers;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        playerInputManager = GetComponent<PlayerInputManager>();

        allPlayers = new List<PlayerController_2D>();
      
    }


    // This function is called everytime a player joined
    public void PlayerJoined(PlayerInput newplayer)
    {
        if (allPlayers.Count >= 2) allPlayers.Clear();
        Debug.Log("NEW PLAYER = " + newplayer);

        // Get a reference
        PlayerController_2D newPlayerController = newplayer.GetComponent<PlayerController_2D>();

        // Set player index
        int playerIndex = playerInputManager.playerCount - 1;
        allPlayers.Add(newPlayerController);

        if (playerIndex == 0)
            newPlayerController.triangulo.color = player1;
        else
        {
            foreach (PlayerController_2D playerController in allPlayers)
            {
                playerController.triangulo.enabled = true;
            }
            newPlayerController.triangulo.color = player2;
        }
            


        newPlayerController.playerIndex = playerIndex;
        newPlayerController.transform.SetParent(playerContainer);


        numPlayers.text = allPlayers.Count + " / " + playerInputManager.maxPlayerCount + " Jugadores";
        Respawn(newPlayerController);
        newPlayerController.transform.Translate(18f, -25, 0);
        //numPlayers.transform.DOScale(Vector3.one * 1.2f, .5f);
    }

    public int GetNumPlayers()
    {
       return allPlayers.Count;
    }
    public void Respawn(PlayerController_2D player)
    { StartCoroutine(Respawn2(player, respawnTime)); }

    //IEnumerator scaleDownparticles()
    //{
    //    while{

    //        return;
    //    }
    //}
    IEnumerator Respawn2(PlayerController_2D player, float time)
    {
        if (!player.killable)
        {
            yield break;
        }

        //// Todo esto es para desactivar el render espec?fico del sprite, se busca y se guarda, ya uqe tiene varios
        //SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>();
        //SpriteRenderer spRenderer = null;
        //for (int x = 0; x < renderers.Length; x++)
        //{
        //    if (renderers[x].transform.CompareTag("Animator"))
        //    {
        //        //Debug.Log("Tag animator encontrado");
        //        spRenderer = renderers[x];
        //        continue;
        //    }

        //}


        //Sistema de particulas de raparici?n
        var dur = pSystem.main;
        dur.startLifetime = respawnTime;
        pSystem.Play();

        AudioManager_PK.instance.Play("PDeath", Random.Range(0.8f, 1f));

        //Desactivar renderer, movimiento y collider del player

        // y tmb desactivar PlayerController_2D para que no se instancie otro player cuando este est? muerto
        Debug.Log("C muri? respawn2(LocalMultiplayere)");
        player.GotKilled();//Esto reactiva el sprite, cuidado
        player.enabled = false;
        player.GetComponent<BoxCollider2D>().enabled = false;
        player.GetGFX().enabled = false;

        //invencibilidad al reaparecer
        player.killable = false;

        Instantiate(sangrePart, player.transform.position, Quaternion.identity);
        //Crear marcador en la UI con el tiempo que queda
        UIManager.GetInstance().CreateSpawnTimer(player.gameObject.transform, time);


        yield return new WaitForSeconds(time);

        AudioManager_PK.instance.Play("Respawn", Random.Range(0.8f, 1f));

        //reactivar renderer, movimiento y collider
        player.GetGFX().enabled = true;

        player.transform.position = respawnPoint.position;
        player.enabled = true;
        player.GetComponent<BoxCollider2D>().enabled = true;
        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        StartCoroutine(player.RespawnColor(1.5f, 5));



    }


   
}