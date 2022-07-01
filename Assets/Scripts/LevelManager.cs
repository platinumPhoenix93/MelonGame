using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MelonGame
{
    public enum game_states
    {
        game_in_progress,
        game_over,
        game_won,
        game_paused,
        respawning
    }
}

public class LevelManager : MonoBehaviour
{
    public Sprite[] lifeSprites;
    private CharacterController player;
    private GameObject gameOverUi;
    private GameObject livesUi;
    private GameObject winUi;
    private GameObject pauseUi;
    private int lives;
    private MelonGame.game_states gameState;
    private GameObject[] checkpoints;
    private GameObject restartButton;
    private GameObject menuButton;
    private GameObject nextLevelButton;
    private GameObject level;
    private GameObject overlayTint;
    private GameObject respawnPoint;
    private GameObject levelName;

    // Start is called before the first frame update
    void Start()
    {
        gameState = MelonGame.game_states.game_in_progress;
        FindGameObjects();
        HideUiElements();

        lives = player.GetLives();
        levelName.GetComponent<TextMeshProUGUI>().SetText(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {

        //Pause game
        if (Input.GetKeyDown(KeyCode.Escape) && gameState == MelonGame.game_states.game_in_progress)
        {
            gameState = MelonGame.game_states.game_paused;
            player.SetGameState(MelonGame.game_states.game_paused);
            //Set ui options to active
            pauseUi.SetActive(true);
            restartButton.SetActive(true);
            menuButton.SetActive(true);
            overlayTint.SetActive(true);

        }


        //Unpause game
        else if(Input.GetKeyDown(KeyCode.Escape) && gameState == MelonGame.game_states.game_paused)
        {
            gameState = MelonGame.game_states.game_in_progress;
            restartButton.SetActive(false);
            menuButton.SetActive(false);
            overlayTint.SetActive(false);
            pauseUi.SetActive(false);
            player.Unpause();
        }
    }

    public void Respawn()
    {

        ResetRotation();
        //Deactivate player so it takes no more inputs
        player.gameObject.SetActive(false);

        //Reset x and y coordinates of player. respawnPoint z coordinate seems to be behind sprites so ignore z
        player.transform.position = new Vector3(respawnPoint.transform.position.x, respawnPoint.transform.position.y);


        //Reactivate player after respawned
        player.gameObject.SetActive(true);

        lives = player.GetLives();

        //Update ui
        livesUi.GetComponent<Image>().sprite = lifeSprites[lives];



    }

    //Displays game over screen
    public void GameOver()
    {
        gameState = MelonGame.game_states.game_over;
        //Set game over ui active
        gameOverUi.SetActive(true);
        restartButton.SetActive(true);
        menuButton.SetActive(true);
        overlayTint.SetActive(true);

    }


    //Display win screen
    public void Win()
    {
        gameState = MelonGame.game_states.game_won;

        //Set win ui active
        winUi.SetActive(true);
        menuButton.SetActive(true);
        if (SceneManager.GetActiveScene().name != "LevelThree" && SceneManager.GetActiveScene().name != "DropJumpPlayTest3") {

            nextLevelButton.SetActive(true);
        }

        overlayTint.SetActive(true);

    }
        //Deactivate all but the active checkpoint
        public void DeactivateCheckpoints(String checkpointName)
    {
        foreach(GameObject checkpoint in checkpoints)
        {

            Debug.Log("CHECKPOINT: " + checkpoint.name);
            if (checkpoint.gameObject.name != checkpointName)
            {
                
                checkpoint.GetComponent<CheckpointController>().Deactivate();

            }
        }
    }


    //Hides all UI elements
    private void HideUiElements()
    {

        gameOverUi.SetActive(false);
        winUi.SetActive(false);
        pauseUi.SetActive(false);
        restartButton.SetActive(false);
        menuButton.SetActive(false);
        nextLevelButton.SetActive(false);
        overlayTint.SetActive(false);
    }

    //Finds all required game objects
    private void FindGameObjects()
    {
        player = FindObjectOfType<CharacterController>();
        livesUi = GameObject.Find("Lives");
        gameOverUi = GameObject.Find("GameOverScreen");
        winUi = GameObject.Find("WinScreen");
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        restartButton = GameObject.Find("RestartButton");
        menuButton = GameObject.Find("MenuButton");
        nextLevelButton = GameObject.Find("NextLevelButton");
        level = GameObject.Find("Level");
        overlayTint = GameObject.Find("GameOverlayTint");
        respawnPoint = GameObject.Find("RespawnPoint");
        pauseUi = GameObject.Find("PauseScreen");
        levelName = GameObject.Find("LevelName");
    }

    //Reset the level rotation back to 0
    public void ResetRotation()
    {
        level.transform.rotation = Quaternion.Euler(0, 0, 0);
        player.SetRespawnPoint(respawnPoint.transform.position);
    }


}
