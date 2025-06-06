using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolingTest : MonoBehaviour
{
    [SerializeField] private float waittime = 3f;
    [SerializeField] private float time = 0;
    [SerializeField] private int PoolNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Awake()
    {
        PoolingInfo poolingInfo = new PoolingInfo();
        poolingInfo.size = EnemyPoolManager.Instance.enemyStats.getSize();
        poolingInfo.PoolTypes = new int[] { 0, 1 };
        poolingInfo.PoolCount = new int[] { 50, 50 };
        EnemyPoolManager.Instance.FirstCreate(poolingInfo);


        PoolingInfo poolingInfo2 = new PoolingInfo();
        poolingInfo2.size = BulletPoolManager.Instance.weaponStats.Count;
        poolingInfo2.PoolTypes = new int[poolingInfo2.size];
        poolingInfo2.PoolCount = new int[poolingInfo2.size];
        for (int i = 0; i < poolingInfo2.size; ++i)
        {

            poolingInfo2.PoolCount[i] = BulletPoolManager.Instance.weaponStats[i].PoolCount;
            poolingInfo2.PoolTypes[i] = BulletPoolManager.Instance.weaponStats[i].WeaponCode;
            print(poolingInfo2.PoolCount[i]);
        }


        BulletPoolManager.Instance.FirstCreate(poolingInfo2);
        // StartCoroutine(test());
    }


    void Start()
    {

    }

    

    // Update is called once per frame
    IEnumerator test(){
        int i = 0;
        Queue<Enemy> enemyQueue = new Queue<Enemy>();
        yield return new WaitForSeconds(waittime);
        while (true){
            print("get enemy from pool");
            var get = EnemyPoolManager.Instance.Get<Enemy>(PoolNum, transform.position, Quaternion.identity);
            if(get!= null) {
                print("enqueue");
                enemyQueue.Enqueue(get);
            }
            else print("no enemy in pool");
    
            yield return new WaitForSeconds(time);
            i++;
            if(i>5){
                EnemyPoolManager.Instance.Return(PoolNum, enemyQueue.Dequeue());
                print("dequeue " + enemyQueue.Count);
            }
        }
 
    }
}
