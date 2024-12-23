using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Transform Player;
    int MoveSpeed = 4;
    int MaxDist = 5;
    int MinDist = 0;

    float gravityStrength = 9.8f;
    
    Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get reference to the Rigidbody
    }

    void Update()
    {

        rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);

        // Get the direction to the Player
        Vector3 direction = Player.position - transform.position;

        direction.y = 0;

        // Check if the direction is valid to avoid errors
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        //if ai is in range
        if (Vector3.Distance(transform.position, Player.position) >= MinDist)
        {
            //travel toward player
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;


            //if player is withing a certain distance you can make the ai do something ex: shoot
            if (Vector3.Distance(transform.position, Player.position) <= MaxDist)
            {
                
            }

        }

        
    }
}
