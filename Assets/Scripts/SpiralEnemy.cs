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
    [SerializeField] private GameObject _enemyBeamPrefab;     // Continuous laser beam prefab
    [SerializeField] private float _firingDuration = 5.0f;    // Beam activation time length
    [SerializeField] private float _beamCooldown = 5.0f;      // Beam off time
    [SerializeField] private Vector3 _beamOffset = new Vector3(0, -1f, 0);   // Position offset for beam, can adjust in Inspector

    private GameObject _enemyBeamInstance;  
    private bool _isFiring = false;
    private float _beamFiringTimer = 0f;
    private float _beamCooldownTimer = 0f;

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


        //_audioSource = GetComponent<AudioSource>();

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
        if (!_isAlive) 
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
        if (_isFiring)
        {
            // currently firing beam
            if (_beamFiringTimer <= 0f)
            {
                StopBeam();
                _beamFiringTimer = _firingDuration; // switch to cooldown
                _beamCooldownTimer = _beamCooldown;
            }
        }
        else
        {
            // currently cooling down
            if (_beamCooldownTimer <= 0f)
            {
                StartBeam();
                _beamFiringTimer = _firingDuration; // switch to firing
            }
        }

        if (_isFiring)
            _beamFiringTimer -= Time.deltaTime;
        else 
            _beamCooldownTimer -= Time.deltaTime;
    }

    private void StartBeam()
    {
        if (_enemyBeamInstance == null)
        {
            _enemyBeamInstance = Instantiate(_enemyBeamPrefab, transform.position + 
                _beamOffset, Quaternion.identity, transform);
        }
        
        _enemyBeamInstance.SetActive(true);
        //_enemyBeamInstance.transform.localPosition = _beamOffset; // ensure correct offset
        _isFiring = true;
    }

    private void StopBeam()
    {
        if (_enemyBeamInstance != null)
        {
            _enemyBeamInstance.SetActive(false);
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
        //_audioSource.Play();
        Destroy(GetComponent<Collider2D>());

        if (_spawnManager != null)
        {
            _spawnManager.OnEnemyDestroyed();
        }

        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
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

