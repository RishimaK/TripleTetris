// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();
    private GameObject _objectPoolEmptyHolder;
    private static GameObject _particleSytemsEmpty;
    private static GameObject _gameObjectEmpty;
    public enum PoolType
    {
        ParticleSytem,
        GameObject,
        None
    }

    private void Awake()
    {
        // SetupEmpties();
    }

    private void SetupEmpties()
    {
        _objectPoolEmptyHolder = new GameObject("Pooled Object");

        _particleSytemsEmpty = new GameObject("Particle Effects");
        _particleSytemsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _gameObjectEmpty = new GameObject("Particle Effects");
        _gameObjectEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotaion
        // , PoolType poolType = PoolType.None
    ) 
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        // PooledObjectInfo pool = null;
        // foreach (PooledObjectInfo p in ObjectPools)
        // {
        //     if (p.LookupString == objectToSpawn.name)
        //     {
        //         pool = p;
        //         break;
        //     }
        // }

        if(pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name};
            ObjectPools.Add(pool);
        }

        // check if there are any iactive object in the pool
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        // GameObject spawnableObj = null;
        // foreach (GameObject obj in pool.InactiveObjects)
        // {
        //     if(obj != null)
        //     {
        //         spawnableObj = obj;
        //         break;
        //     }
        // }

        if(spawnableObj == null)
        {
            // Find the parent of the empty object
            // GameObject parentObject = SetParentObject(poolType);

            // if there are no inactive objects, create a new one
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotaion);
            // spawnableObj.GetComponent<RectTransform>().anchoredPosition = spawnPosition;
            // if(parentObject != null)
            // {
            //     spawnableObj.transform.SetParent(parentObject.transform);
            // }
        } 
        else
        {
            // if there is an inactive object, reactive it
            // spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotaion;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }
        
        return spawnableObj;
    }

    // public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform) {
    //     PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

    //     if(pool == null)
    //     {
    //         pool = new PooledObjectInfo() { LookupString = objectToSpawn.name};
    //         ObjectPools.Add(pool);
    //     }

    //     // check if there are any iactive object in the pool
    //     GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

    //     if(spawnableObj == null)
    //     {
    //         // if there are no inactive objects, create a new one
    //         spawnableObj = Instantiate(objectToSpawn, parentTransform);
    //     } 
    //     else
    //     {
    //         // if there is an inactive object, reactive it
    //         pool.InactiveObjects.Remove(spawnableObj);
    //         spawnableObj.SetActive(true);
    //     }
        
    //     return spawnableObj;
    // }

    public static void ReturnObjectToPool (GameObject obj)
    {
        // string goName = obj.name.Substring(0, obj.name.Length - 7);
        string goName = obj.name; 
        // by taking off 7, we are removing the (clone) from the name of the passed in object

        // Debug.Log(p.LookupString);
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

        if(pool == null)
        {
            Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
        }
        else
        {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.ParticleSytem:
                return _particleSytemsEmpty;
            case PoolType.GameObject:
                return _gameObjectEmpty;
            case PoolType.None:
                return null;
            default:
                return null;
        }
    }
}

public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
