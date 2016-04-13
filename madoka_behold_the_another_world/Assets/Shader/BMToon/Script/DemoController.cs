using UnityEngine;
using System.Collections;

public class DemoController : MonoBehaviour {

    public bool bPoint = false;
    
    public GameObject Point;

	// Use this for initialization
	void Start ()
    {
        Point.SetActive(bPoint);
    }

    void OnGUI()
    {
        GUI.Box(new Rect(90, 10, 100, 60), "ExtraLight");
        if (GUI.Button(new Rect(100, 40, 80, 20), "Pointlight"))
        {
            bPoint = !bPoint;
            Point.SetActive(bPoint);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
