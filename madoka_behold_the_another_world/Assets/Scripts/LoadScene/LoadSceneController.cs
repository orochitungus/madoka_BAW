using UnityEngine;
using System.Collections;

public class LoadSceneController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        LoadManager.Instance.SceneChange();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
