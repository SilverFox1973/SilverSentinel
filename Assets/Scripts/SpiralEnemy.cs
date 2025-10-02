using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralEnemy : MonoBehaviour

{
    [Header("Boundary")]
    [SerializeField] private float _topBounds;
    [SerializeField] private float _bottomBounds;
    [SerializeField] private float _leftBounds;
    [SerializeField] private float _rightBounds;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject _beamPrefab; // Continous laser beam prefab
    [SerializeField] private float _firingDuration = 5.0f; // Beam on time
    [SerializeField] private float _beamCooldown = 5f;  // Beam off time

    private GameObject _beamInstance;
    private bool _isFiring = false;
    private float _firingTimer = 0f;

    private bool _isAlive = true;

    private Player _player;
    private SpawnManager _spawnManager;

    private Animator _enemyAnim;
    private AudioSource _audioSource;

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 2.0f;
    private float _shipTime = 0;
    private Vector3 _travelLinePosition;

    [SerializeField] private float _frequency = -0.75f;
    [SerializeField] private float _amplitude;

    private Vector3 _circleOffset = Vector3.zero;

    //Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        // Save the starting position as the baseline for spiral movement
        _travelLinePosition = transform.position;

        //Random radius 
        _amplitude = Random.Range(1.0f, 3.0f);


        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("The Player in NULL.");
        }

        _enemyAnim = GetComponent<Animator>();

        if (_enemyAnim == null)
        {
            Debug.LogError("The animator in NULL.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isAlive) //Ask if the Enemy is Alive.  If not, do not proceed with the code
            return;

        CalculateMovement();

        EnemyBeamFiring();

        _shipTime += Time.deltaTime;

    }

    public void SpawnManager(SpawnManager manager)
    {
        _spawnManager = manager;
    }

   
    private void EnemyBeamFiring()
    { 
        _firingTimer -= Time.deltaTime;

        if (_isFiring)
        {
            // currently firing beam
            if (_firingTimer <= 0f)
            {
                StopBeam();
                _firingTimer = _beamCooldown; //switch to cooldown
            }
        }

        else
        {
            //Currently cooling down
            if (_firingTimer <= 0f)
            {
                StartBeam();
                _firingTimer = _firingDuration; //switch to firing
            }
        }
    }

    private void StartBeam()
    {
        if (_beamInstance == null)
        {
            _beamInstance = Instantiate(_beamPrefab, transform.position, Quaternion.identity, transform);
        }

        _beamInstance.SetActive(true);
        _isFiring = true;
    }

    private void StopBeam()
    {
        if (_beamInstance != null)
        {
            _beamInstance.SetActive(false);
        }

        _isFiring= false;
    }

    private void CalculateMovement()
    {
        // Move the center point downward
        _travelLinePosition += Vector3.down * (_speed * Time.deltaTime);

        // Calculate the circular offset (circle arount the center) 
        float angle = -2 * Mathf.PI * _shipTime * _frequency;
        _circleOffset.x = Mathf.Cos(angle) * _amplitude;
        _circleOffset.y = Mathf.Sin(angle) * _amplitude;
       
        // Spiral = downward travel + circular offset
        transform.position = _travelLinePosition + _circleOffset;

        //Reset if we move off-screen
        if (transform.position.y <= _bottomBounds)
        {
            float randX = Random.Range(_leftBounds, _rightBounds);
            transform.position = new Vector2(randX, _topBounds);
            _travelLinePosition = transform.position;
            _shipTime = 0; //reset spiral motion
        }

    }

    public void EnemyDeath()
    {
        _isAlive = false; //prevents further firing
        _enemyAnim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>());

        if (_spawnManager != null)
        {
            _spawnManager.OnEnemyDestroyed();
        }

        Destroy(this.gameObject, 2.5f); //Let's animation/sound play out
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

