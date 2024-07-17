using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator CameraShaker(float duration, float magnitude)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration) 
        { 
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition;
    }
}
