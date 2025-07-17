using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour

{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _bottomBounds = -5.8f;

    //0 = Triple Shot 1 = Speed 2 = Shields 3 = Ammo Refill, etc.
    [SerializeField] private int _powerupID;
    [SerializeField] private AudioClip _audioClip;

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(Vector2.down * (_speed * Time.deltaTime));

        if (transform.position.y < _bottomBounds)
        { 
            //Debug.Log(transform.position.y);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"Hit: {other.name}");
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
       
            if (player != null)
            { 
                switch(_powerupID) 
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
