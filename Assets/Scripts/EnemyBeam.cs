using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeam : MonoBehaviour
{
    private SpiralEnemy _owner;

    public void SetOwner(SpiralEnemy enemy)
    {
        _owner = enemy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
                _owner.BeamHitPlayer(player);
        }
    }
}


