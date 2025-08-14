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
    
    [SerializeField] private int _enemyWaveCount = 5;
    [SerializeField] private int _spawnedEnemyCount = 0;
    [SerializeField] private int _aliveEnemyCount = 0;
    

    [SerializeField] private int _wave = 0;
    [SerializeField] private int _waveMultiplier = 5;
    [SerializeField] private float _waveStartDelay = 5f;

    [Space(10)]
    private bool _stopSpawning = false;

    private void Start()
    {
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

        //Debug.Log($"Wave {_wave} starting in 10 seconds with {_enemyWaveCount} enemies...");
        StartCoroutine(StartWaveAfterDelay(10f));
    }
    private int GetPowerup()
    {
        int number = Random.Range(0, 10);
        int randomPowerup;

        if (number < 9)
        {
            randomPowerup = Random.Range(0, 5);
        }
        else
        {
            randomPowerup = 5;
        }
        return randomPowerup;
    }

    private IEnumerator StartWaveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
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
