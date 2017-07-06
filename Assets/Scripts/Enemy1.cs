using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour {

    private GameObject player;
    private PlayerMovement playerMovement;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision c)
    {
        if (c.contacts.Length > 0)
        {
            ContactPoint contact = c.contacts[0];

            // If the collision was above the enemy, it was hit from the top
            if (Vector3.Dot(contact.normal, Vector3.up) < -0.5f)
            {
                if (c.gameObject == player)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
