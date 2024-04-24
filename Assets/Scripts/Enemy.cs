using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour

{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;

    private Animator _enemyAnim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    //Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

        if (_player == null )
        {
            Debug.LogError("The Player in NULL.");
        }

        _enemyAnim = GetComponent<Animator>();

        if (_enemyAnim == null ) 
        {
            Debug.LogError("The animator in NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Debug.Break();
        }
    }

    void CalculateMovement() 
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null) 
            {
                player.Damage();
            }
       
            _enemyAnim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
            
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
          
            _enemyAnim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
            
        }
    }
}
