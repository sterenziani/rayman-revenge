using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveTrigger : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] spawnable;
    public int remainingEnemiesBeforeSpawn = 0;

    void Start()
    {
        foreach (GameObject obj in spawnable)
            obj.SetActive(false);
    }

    void Update()
    {
        int remainingEnemies = enemies.Length;
        foreach (GameObject obj in enemies)
        {
            if (obj == null)
                remainingEnemies--;
        }
        if(remainingEnemies <= remainingEnemiesBeforeSpawn)
        {
            foreach (GameObject obj in spawnable)
                obj.SetActive(true);
        }
    }
}
