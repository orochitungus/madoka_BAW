using UnityEngine;
using System.Collections;
using EleckTek ; 

public class PauseKeyboardInput : MonoBehaviour {

	
	public KeyCode[] pauseKeys ; 
	public PauseController[] pauseControllers ; 
	public GameObject spinningIcon ; 
	public Material alphaMaterial ; 
	public string pauseMessage ; 
	bool activated ; 
	
	// Update is called once per frame
	void Update () {
		
		Spin spin = null ; 
		if( spinningIcon )
		{
			spin = (Spin)spinningIcon.GetComponent( "Spin" ) ; 
		}
		
		foreach( KeyCode key in pauseKeys )
		{
			if( Input.GetKeyDown( key ) )
			{
				if( spin.IsSpinning() && !activated )
				{
					
					break ; 
				}
				
				GameObject gobj_text = GameObject.FindWithTag("PauseText") ; 
				GUIText gui_text = gobj_text.guiText ; 
				
				activated = !activated ; 
	
				
				foreach( PauseController pause in pauseControllers )
				{
					pause.activatePause = activated; 
				}
				

				if( gobj_text && gui_text )
				{
					if( activated )
					{
						gui_text.text = pauseMessage ; 
					}
					else
					{
						gui_text.text = "" ; 
					}
				}
			
				
				if( spin )
				{
					if( !activated )
					{
						spin.DontSpin() ; 

					}
					else
					{
						spin.DoSpin() ; 
					}
				}
				break ; 
				
				
			}
		}
		
		
	}
}
