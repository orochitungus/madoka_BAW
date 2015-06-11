using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem.CollisionEvent[] ces = new ParticleSystem.CollisionEvent[particleSystem.safeCollisionEventSize];
        foreach (ParticleSystem.CollisionEvent item in ces)
        {
            // action
            Debug.Log(item.collider);
        }

    }
}
