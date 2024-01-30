using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour

{
    [SerializeField]
    private float _enemySpeed = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //move down at 4 meters per second
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        //if bottom of screen
        //respawn at top with a new random x position
        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 7, 0);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        //if other is Player
        //Damage player
        //Destroy Us
        if (other.tag == "Player")
        {
            //damage player
            Destroy(this.gameObject);
        }

        //if other is Laser
        //destroy laser
        //destroy us
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
