using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3 (0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _speed * Time.deltaTime);

        //if player position on the y is greater than 0
        //y position = 0

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }



        //if player on the x > 13
        //x pos = -13
        //else if the player on the x axis is less than -13
        //x pos = 13


    }
}
