using UnityEngine;
using System.Collections;

public class Lazer : MonoBehaviour {
    public GameObject lazerObj;
    GameObject entityLazer;
    int defaultLayer = 0;
    public GameObject hitParticle;

	// Use this for initialization
	void Start () {
        entityLazer = GameObject.Instantiate(lazerObj) as GameObject;
        defaultLayer = LayerMask.NameToLayer("Default");
	}
	
	// Update is called once per frame
    int hitCount = 0;
	void Update () {
        bool isHit = false;
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector3 posLazer = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 dirLazer = posLazer - transform.position;
            dirLazer.z = 0f;
            dirLazer.Normalize();
            dirLazer *= 100f;
            entityLazer.SetActive(true);
            Ray ray = new Ray(transform.position, dirLazer);
//            Debug.DrawRay(ray.origin, dirLazer);
//            Debug.Log("DRAW RAY " + ray.origin + " :" + ray.direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,100f, 1 << defaultLayer))
            {
                entityLazer.transform.position = (transform.position + hit.point) * 0.5f;
                Vector3 scl = entityLazer.transform.localScale;
                scl.y = (transform.position - hit.point).magnitude * 0.5f;
                entityLazer.transform.localScale = scl;
                entityLazer.transform.rotation = Quaternion.AngleAxis(90f, Vector3.back) * Quaternion.LookRotation(dirLazer);

                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(ray.direction * 100f, hit.point);
                }
                isHit = true;
                hitParticle.transform.position = hit.point;
                ++hitCount;
            }
            else
            {
                entityLazer.SetActive(false);
            }
        }
        else
        {
            entityLazer.SetActive(false);
        }

        if (isHit)
        {
            //hitParticle.GetComponent<ParticleSystem>().enableEmission = true;
        }
        else
        {
            //hitParticle.GetComponent<ParticleSystem>().enableEmission = false;
        }
	}
}
