using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _playerSpeed = 4.5f;

    [SerializeField]
    private GameObject _tripleShotPrefab;
   
    [SerializeField]
    private GameObject _sprayShotPrefab;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.25f;
    private float _nextFire = -1f;
    [SerializeField]
    private int _lives;
     
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    private bool _isThrusterEngaged = false;
    private bool _isSprayShotActive = false;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _shieldHealth = 3;
    [SerializeField]
    private GameObject _rightWingFire, _leftWingFire;

    [SerializeField]
    private int _score;

    [SerializeField]
    private int _ammoCount = 15;

    private float _thrusterMaxEnergy = 100;

    private float _thrusterCurrentEnergy;

    [SerializeField]
    private float _thrusterEnergyMultiply = 10f;

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

        _thrusterCurrentEnergy = _thrusterMaxEnergy;
        _uiManager.UpdateThrusterBar(_thrusterCurrentEnergy);  
    }

    // Update is called once per frame
    void Update()
    {

        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            if (_ammoCount == 0)
            {
                return; 
            }
            FireLaser();
        }


    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        Vector3 _direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(_direction * (CalculateSpeed() * Time.deltaTime)); 

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

        if (Input.GetKey(KeyCode.LeftShift) && _thrusterCurrentEnergy > 0)
        {
            _isThrusterEngaged = true;
            _thrusterCurrentEnergy -= Time.deltaTime * _thrusterEnergyMultiply;
            _uiManager.UpdateThrusterBar(_thrusterCurrentEnergy);
        }
        else
        {
            _isThrusterEngaged = false;
            if (_thrusterCurrentEnergy < _thrusterMaxEnergy)
            {
                _thrusterCurrentEnergy += Time.deltaTime * _thrusterEnergyMultiply;
                _uiManager.UpdateThrusterBar(_thrusterCurrentEnergy);
            }
        }
            
    }

    public void AmmoCount(int bolts)
    {
        _ammoCount += bolts;
        _uiManager.UpdateAmmoCount(_ammoCount);
    }

    public void AmmoRefillActive()
    {
        AmmoCount(+15);
    }
    
    void FireLaser()
    {
        AmmoCount(-1);
        _nextFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }

        //Check to see if Spray Shot is enabled.
        else if (_isSprayShotActive == true)
        {
            Instantiate(_sprayShotPrefab, transform.position + new Vector3(-1.08f, 0.25f, 0), Quaternion.identity);

        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.06f, 0), Quaternion.identity);
        }

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

    public void AddLifeRefill()
    {
        if (_lives < 3)
        {
            _lives++;
            if (_lives == 2)
            {
                _leftWingFire.SetActive(false);
            }
            else if ( _lives == 3) 
            {
                _rightWingFire.SetActive(false);
            }
            _uiManager.UpdateLives(_lives);
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

    public void SprayShotActive()
    {
        _isSprayShotActive = true;
        StartCoroutine(SprayShotPowerDownRoutine());
    }

    IEnumerator SprayShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSprayShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
    }

    private float CalculateSpeed()
    {
        float speedMultiplier = 1.0f;

        if (_isThrusterEngaged)
        {
            speedMultiplier *= 2.0f;
        }

        if (_isSpeedBoostActive)
        {
            speedMultiplier *= 2.0f;
        }
        //_isThrusterEngaged = False and _isSpeedBoostActive = False then speedMultipler = 1
        //_isThrusterEngaged = True and _isSpeedBoostActive = False then speedMultipler = 2
        //_isThrusterEngaged = False and _isSpeedBoostActive = True then speedMultipler = 2
        //_isThrusterEngaged = True and _isSpeedBoostActive = True then speedMultipler = 4

        return _playerSpeed * speedMultiplier;
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
