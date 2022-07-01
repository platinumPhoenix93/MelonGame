using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public Animator animator;
    public bool isActive;
    private LevelManager levelManager;
    // Start is called before the first frame update

    private void Awake()
    {
        isActive = false;
    }

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with object");

        //If player comes in contact with checkpoint activate this checkpoint and deactivate all others
        if (collision.gameObject.tag == "Player")
        {
            isActive = true;
            levelManager.DeactivateCheckpoints(this.gameObject.name);
            animator.SetBool("isActive", isActive);
        }
    }

    //Deactivate checkpoint
    public void Deactivate()
    {
        isActive = false;
        animator.SetBool("isActive", false);
        Debug.Log("Deactivating");
    }

}
