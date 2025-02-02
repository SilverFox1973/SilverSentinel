using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour

{
    [SerializeField]
    private float _powerupSpeed = 3.0f;

    [SerializeField] //0 = Triple Shot 1 = Speed 2 = Shields 3 = Ammo Refill
    private int powerupID;
    [SerializeField]
    private AudioClip _audioClip;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * (_powerupSpeed * Time.deltaTime));

        if (transform.position.y < -5.8f)
        { 
        Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
       
            if (player != null)
            { 
                switch(powerupID) 
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AmmoRefillActive();
                        break;
                    case 4:
                        player.AddLifeRefill();
                        break;
                    case 5:
                        player.SprayShotActive();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;  
                }  
            }

            Destroy(this.gameObject);
        }
    }

    
}
