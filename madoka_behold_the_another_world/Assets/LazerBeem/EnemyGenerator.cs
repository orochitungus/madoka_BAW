using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
    public GameObject enemyInst;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 40; ++i)
        {
            GameObject o = GameObject.Instantiate(enemyInst) as GameObject;
            o.transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
        }
	
	}

    // Update is called once per frame
    void Update()
    {
	
	}
}
