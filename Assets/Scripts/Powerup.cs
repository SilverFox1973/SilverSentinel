using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour

{
    [SerializeField]
    private float _powerupSpeed = 3.0f;

    [SerializeField] //0 = Triple Shot 1 = Speed 2 = Shields
    private int powerupID;



    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);

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
            if (player != null)
            {
                if (powerupID == 0)
                {
                    player.TripleShotActive();
                }
                else if (powerupID == 1) 
                {
                    Debug.Log("Collected Speed Boost");
                }
                else if (powerupID == 2) 
                {
                    Debug.Log("Collected Shields");
                }

            }

            Destroy(this.gameObject);
        }
    }

    
}
