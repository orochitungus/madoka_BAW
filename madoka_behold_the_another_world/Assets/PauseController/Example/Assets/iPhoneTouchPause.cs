using UnityEngine;
using System.Collections;

public class iPhoneTouchPause : MonoBehaviour {
	
	public EleckTek.PauseController[] pauseControllers ; 
	public Spin spinner ; 
	bool myActivated ; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		
		if( GUI.Button( new Rect( 0.0f * Screen.width, 0.85f * Screen.height, .15f * Screen.width, .15f * Screen.height), new GUIContent( "Pause Scene"," Pause all objects without special coding" ) ) )
		{
			myActivated = !myActivated ; 
			foreach( EleckTek.PauseController pause in pauseControllers )
			{
				pause.activatePause = myActivated ; 
			}
			
			if( myActivated )
			{
				spinner.DoSpin() ;
			}
			else
			{
				spinner.DontSpin() ; 
			}
		}
		
	}
}
