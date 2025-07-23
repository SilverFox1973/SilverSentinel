using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour

{
    [SerializeField] private float _speed;

    [Header("Boundary")]
    [SerializeField] private float _topBounds;
    [SerializeField] private float _bottomBounds;
    [SerializeField] private float _leftBounds;
    [SerializeField] private float _rightBounds;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _fireRate = 3.0f;
    [SerializeField] private float _canFire = -1f;

    private Player _player;

    private Animator _enemyAnim;
    private AudioSource _audioSource;

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
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
            //Debug.Break();
        }
    }

    void CalculateMovement() 
    {
        transform.Translate(Vector2.down * (_speed * Time.deltaTime));

        if (transform.position.y < _bottomBounds)
        {
            float randomX = Random.Range(_leftBounds, _rightBounds);
            transform.position = new Vector2(randomX, _topBounds);
        }
    }
    
    public void EnemyDeath()
    {
        _enemyAnim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, 2.5f);
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
            EnemyDeath();   
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            EnemyDeath();
        }
    }
}
