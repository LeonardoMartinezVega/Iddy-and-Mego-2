using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Enemy") {
            Destroy(other.gameObject);
        }
    }
}
