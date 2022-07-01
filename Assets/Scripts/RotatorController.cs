using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorController : MonoBehaviour
{

    private CharacterController player;
    private GameObject level;
    private bool colliding;
    private bool ePress;
    private bool qPress;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<CharacterController>();
        level = GameObject.Find("Level");
        colliding = false;
        ePress = false;
        qPress = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Register an e press if player is colliding with a rotator 
        if(player.getGameState() == MelonGame.game_states.game_in_progress && colliding && Input.GetKeyDown(KeyCode.E)){
            ePress = true;
        }
        //Register a q press if player is colliding with a rotator 
        if (player.getGameState() == MelonGame.game_states.game_in_progress && colliding && Input.GetKeyDown(KeyCode.Q)){
            qPress = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("Collision enter");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colliding with player");
            colliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            colliding = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (player.getGameState() == MelonGame.game_states.game_in_progress)
            {

            //If player is colliding with rotator and pressed e or q rotate the map
                if (ePress)
                {
                    Debug.Log("E pressed");
                    RotateMapClockwise();
                    ePress = false;
                }
                if (qPress)
                {
                    Debug.Log("Q pressed");
                    RotateMapAntiClockwise();
                    qPress = false;
                }
            }
        }
    }

    //Rotates the level clockwise around the rotator
    private void RotateMapClockwise()
    {
        level.transform.RotateAround(this.transform.position, Vector3.forward, -90);
    }

    //Rotates the level anticlockwise around the rotator
    private void RotateMapAntiClockwise()
    {
        level.transform.RotateAround(this.transform.position, Vector3.forward, 90);
    }

}
