using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

   

    void Update() {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Hit" + other.name);
        Destroy(gameObject);
    }

}
