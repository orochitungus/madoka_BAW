using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveController : MonoBehaviour
{
    public Animator AnimatorUnit;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	 
	}
       
    public void OnClickMoveButton()
    {
        AnimatorUnit.SetTrigger("Move");
    }
}
