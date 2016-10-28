using UnityEngine;
using System.Collections;

public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
