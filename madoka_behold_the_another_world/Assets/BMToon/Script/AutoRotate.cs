using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {
    public float RotateSpeed = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, RotateSpeed);
	}
}
