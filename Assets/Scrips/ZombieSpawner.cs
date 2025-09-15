using System;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public Transform[] spawnpoints;

    public GameObject zombie;

    private void Start(){
        InvokeRepeating("Spawnzombie", 2, 5);
    }

    void Spawnzombie()
    {
        int r = UnityEngine.Random.Range(0, spawnpoints.Length);
        GameObject myzombie = Instantiate(zombie, spawnpoints[r].position,Quaternion.identity);
    }
}
