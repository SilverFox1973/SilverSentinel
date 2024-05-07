using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _playerSpeed = 4.5f;
    private float _speedMultiplier = 2f;
    


    [SerializeField]
    private GameObject _tripleShotPrefab; 
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.25f;
    private float _nextFire = -1f;
    [SerializeField]
    private int _lives = 3;

    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldsActive = false;
    private bool _isThrusterEngaged = false;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _shieldHealth = 3;
    [SerializeField]
    private GameObject _rightWingFire, _leftWingFire;

    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    //variable to store the audio clip
    [SerializeField]
    private AudioClip _laserFire;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on player in NULL!");
        }
        else
        {
            _audioSource.clip = _laserFire;
        }
    }

    // Update is called once per frame
    void Update()
    {

        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
        }


    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        Vector3 _direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(_direction * ThrustBoost() * Time.deltaTime); 

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.9f)
        {
            transform.position = new Vector3(transform.position.x, -3.9f, 0);
        }

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isThrusterEngaged = true;
        }
        else
        {
            _isThrusterEngaged = false;
        }
    }

    void FireLaser()
    {
        _nextFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.06f, 0), Quaternion.identity);
        }

        //play the laser audio clip
        _audioSource.Play();

    }

    public void Damage()
    {
        if (_isShieldsActive == true)
        {
            _shieldHealth--;

            if (_shieldHealth < 1)
            {
                _isShieldsActive = false;
                _shieldVisualizer.SetActive(false);

                return;
            }
        
        }

        if (_isShieldsActive)
        {
            switch (_shieldHealth)
            {
                case 1:
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
                    break;
                case 2:
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
                    break;
                case 3:
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
                    break;
            }
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            _rightWingFire.SetActive(true);
        }
        else if (_lives ==1)
        {
            _leftWingFire.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _playerSpeed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _playerSpeed /= _speedMultiplier;
    }

    private float ThrustBoost()
    {
        return (_playerSpeed * (_isThrusterEngaged ? 2.0f : 1.0f) * (_isSpeedBoostActive ? 2.0f : 1.0f));
    }
    public void ShieldsActive()
    {
        _isShieldsActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }


}
