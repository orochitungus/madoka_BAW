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
        ParticleCollisionEvent[] ces = new ParticleCollisionEvent[GetComponent<ParticleSystem>().GetSafeCollisionEventSize()];
        foreach (ParticleCollisionEvent item in ces)
        {
            // action
            Debug.Log(item.colliderComponent);
        }

    }
}
