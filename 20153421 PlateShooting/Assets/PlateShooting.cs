using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stat
{
    public int healthPoint;
    public float speed;
    public int power;

    public Stat(int hp,float speed,int power)
    {
        this.healthPoint = hp;
        this.speed = hp;
        this.power = power;
    }
}

public class PlateShooting : MonoBehaviour
{
    public float spawnDelay = 3.0f;
    public GameObject enemyPrefab;
    public GameObject spawnPoint;

    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f ,1.0f);      
    }

    void SpawnEnemy()
    {
        Vector2 rand = Random.insideUnitCircle.normalized;

        rand *= Random.Range(100.0f,150.0f);

        Vector3 pos = spawnPoint.transform.position +
            new Vector3(rand.x, 0 , rand.y);

        Enemy enemy = Instantiate<GameObject>(enemyPrefab, pos,Quaternion.identity).GetComponent<Enemy>();
        enemy.MyStat = new Stat(20, 10 , 1);
    }
}
