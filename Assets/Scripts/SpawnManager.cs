using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _powerupContainer;
    [SerializeField] private GameObject[] _powerUps;

    [Header("Enemy Wave Settings")]
    
    private int _enemyWaveCount;
    private int _spawnedEnemyCount = 0;
    private int _aliveEnemyCount = 0;
    

    [SerializeField] private int _wave = 0;
    [SerializeField] private int _waveMultiplier = 5;
    [SerializeField] private float _waveStartDelay = 5f;
    [SerializeField] private float _afterWaveDelay = 10f;
    
    
    [Space(10)]
    private bool _stopSpawning = false;

    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        StartSpawning();
    }

    public void StartSpawning()
    {
        _wave = 0;
        WaveAdvance();
        StartCoroutine(SpawnPowerupRoutine());
    }
    
    private void WaveAdvance()
    {
        _wave++;
        _enemyWaveCount = _wave * _waveMultiplier;
        _spawnedEnemyCount = 0; //reset for new wave

        FindObjectOfType<UIManager>().ShowWaveNumber(_wave);

        //Debug.Log($"Wave {_wave} starting in 10 seconds with {_enemyWaveCount} enemies...");
        StartCoroutine(StartWaveAfterDelay(_afterWaveDelay));
    }

    private int GetPowerup()
    {
        int number = Random.Range(0, 10);
        int randomPowerup;

        if (number < 8) // 0-7 -> 80%
        {
            randomPowerup = Random.Range(0, 5); // 0-4 (common)
        }
        else if (number == 8) // -> 10%
        {
            randomPowerup = 5; //rare
        }
        else //number == 9 -> 10%
        {
            randomPowerup = 6; //rare
        }
        return randomPowerup;
    }

    
    private IEnumerator StartWaveAfterDelay(float delay)
    {
        if (_uiManager != null)
        {
            _uiManager.ShowWaveNumber(_wave); //Show "Wave X" UI
        }

        yield return new WaitForSeconds(5f);

        //Wait the remaining delay time (delay - 5f)
        float remainingDelay = Mathf.Max(0, delay - 5f);
        yield return new WaitForSeconds(remainingDelay);

        _aliveEnemyCount = 0; //Reset here before spawning
        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (!_stopSpawning  && _spawnedEnemyCount < _enemyWaveCount) 
        {
            Vector2 posToSpawn = new Vector2(Random.Range(-12f, 12f), 7);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            _spawnedEnemyCount++;
            _aliveEnemyCount++;
            
            yield return new WaitForSeconds(_waveStartDelay); //wait between spawns
        }  
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        while (_stopSpawning == false)
        {
            Vector2 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7);
            GameObject newPowerup = Instantiate(_powerUps[GetPowerup()], posToSpawn, Quaternion.identity);
            newPowerup.transform.parent = _powerupContainer.transform;
            yield return new WaitForSeconds(Random.Range(3, 11));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void OnEnemyDestroyed()
    {
        _aliveEnemyCount--;
        //Debug.Log($"Enemy destroyed. Alive: {_aliveEnemyCount}");

        if (_aliveEnemyCount <= 0 && _spawnedEnemyCount >= _enemyWaveCount && !_stopSpawning)
        {
            WaveAdvance();
        }
    }

}

