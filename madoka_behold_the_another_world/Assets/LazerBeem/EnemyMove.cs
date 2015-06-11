using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour {
    public float speed = 10f;

	// Use this for initialization
	void Start () {
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = dir.normalized * 10f;
        }
	}
	
	// Update is called once per frame
	void Update () {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
//            rb.position
        }	
	}
}
