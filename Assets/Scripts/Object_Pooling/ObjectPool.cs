using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool { 
    public int amount = 10;
    public GameObject prefabObj;

    private Dictionary<int, GameObject> pool = new Dictionary<int, GameObject>();
    private int lastUsedItem = 0;

   /* private void Awake() {
        StartPool();
    }*/

  /*  private void StartPool() {
        for (int i = 0; i < amount; i++) {
            if (prefabObj) {
                GameObject obj = Instantiate(prefabObj, new Vector3(0, 0, 0), Quaternion.identity);
                pool.Add(pool.Count, obj);

                obj.SetActive(false);
            }
        }
    }*/

    public Dictionary<int, GameObject> GetPoolDic() {
        return pool;
    }

    public int GetPoolSize() {
        return pool.Count;
    }

    public int GetCurrentPoolNumber() {
        return lastUsedItem;
    }


    public void AddObjectToPool(GameObject obj) {

        if (obj) {
            pool.Add(pool.Count, obj);
            obj.SetActive(false);
        }
    }

    public GameObject GetNextPooledItem() {

          GameObject poolObj;

            if ((lastUsedItem + 1) >= pool.Count) {
                     lastUsedItem = 0;
                }

            poolObj = pool[lastUsedItem];
            poolObj.SetActive(true);
            lastUsedItem += 1;

        return poolObj;
    }
}
