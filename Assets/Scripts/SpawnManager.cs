using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject[] _powerUps;
    [SerializeField] private GameObject _powerupContainer;

    private bool _stopSpawning = false;
   
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

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return null; 

        yield return new WaitForSeconds(5.0f);

        while (_stopSpawning == false) 
        {
            Vector2 posToSpawn = new Vector2(Random.Range(-12f, 12f), 7);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

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

}
