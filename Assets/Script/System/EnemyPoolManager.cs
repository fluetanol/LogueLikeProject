using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolDebugData
{
    public int ActiveCount { get; }
    public int PoolCount { get; }
}

public class EnemyPoolManager : ObjectPoolManager<Enemy, EnemyPoolManager>, IPoolDebugData
{
    public EnemyStats enemyStats;
    public IMoveData moveData;
    public List<ObjectPool<Enemy>> enemyPool;

    protected override void Awake()
    {
        base.Awake();
        moveData = FindAnyObjectByType<PlayerMovement>();
        print("moveData " + moveData);

        enemyPool = new List<ObjectPool<Enemy>>(enemyStats.getSize());
        for (int i = 0; i < enemyStats.getSize(); i++)
        {
            enemyPool.Add(null);
        }
    }

    public override void FirstCreate(PoolingInfo poolInfo)
    {
        //풀링할 적 오브젝트 코드
        foreach (var code in poolInfo.PoolTypes)
        {
            GameObject prefab = enemyStats[code].EnemyPrefab;
            Enemy prefabEnemy = prefab.GetComponentInChildren<Enemy>();
            prefabEnemy.EnemyCode = code;
            prefabEnemy.MoveData = moveData;


            int count = poolInfo.PoolCount[code];
            //그 적을 생성시킬 갯수
            //            print("create count : " + count);
            Enemy[] objs = new Enemy[count];
            for (int i = 0; i < count; i++)
            {
                objs[i] = Instantiate(prefab, Parent).GetComponentInChildren<Enemy>();
                objs[i].EnemyCode = code;
                objs[i].MoveData = moveData;
                objs[i].transform.parent.gameObject.SetActive(false);
            }


            //            print("create code : " + code);
            enemyPool[code] = new ObjectPool<Enemy>(Parent, prefab, enableCreateNew, objs);
        }

    }

    public override Enemy Get(int num)
    {
        return Get<Enemy>(num);
    }

    public override Enemy Get(int num, Vector3 position, Quaternion rotation, bool isActive = false)
    {
        return Get<Enemy>(num, position, rotation, isActive);
    }


    public override P Get<P>(int num)
    {
        return enemyPool[num].Get() as P;
    }


    public override P Get<P>(int num, Vector3 position, Quaternion rotation, bool isActive = false)
    {
        P enemy = Get<P>(num);

        if (enemy != null)
        {
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.transform.parent.gameObject.SetActive(isActive);
        }

        return enemy;
    }

    public override Enemy[] Get(int num, int count)
    {
        Enemy[] enemies = new Enemy[count];

        for (int i = 0; i < count; i++)
        {
            enemies[i] = enemyPool[num].Get();
        }

        return enemies;
    }

    public override void Return(int num, Enemy obj)
    {
        enemyPool[num].Return(obj);
    }

    public override void DestroyPoolObject(int num, int count)
    {
        for (int i = 0; i < num; i++)
        {
            Enemy enemy = Get<Enemy>(num);
            Destroy(enemy.gameObject);
        }
    }

    public override void Add(Enemy obj)
    {
        throw new NotImplementedException();
    }

    public override void Add(Enemy[] obj)
    {
        throw new NotImplementedException();
    }

    public int ActiveCount
    {
        get
        {
            int count = 0;
            foreach (var pool in enemyPool)
            {
                count += pool.ActiveCount;
            }
            return count;
        }
    }

    public int PoolCount
    {
        get
        {
            int count = 0;
            foreach (var pool in enemyPool)
            {
                count += pool.PoolCount;
            }
            return count;
        }
    }

}
