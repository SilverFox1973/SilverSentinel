using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour

{
    [SerializeField]
    private float _powerupSpeed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    //move down at a speed of 3 (adjust in the inspector)
    //when we leave the screen, destroy this object
    {
        transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);

        if (transform.position.y < -5.8f)
        { 
        Destroy(this.gameObject);
        }

    }

    //OnTriggerCollision
    //Only be collectible by the Player 
    //On collection, destroy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }

    
}
