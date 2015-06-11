using UnityEngine;
using System.Collections;
using System; 
using EleckTek ; 

public class PauseControllerInputDetector : MonoBehaviour {
	
	public string a_Description ; 
	public string pauseControllerName ;
	
	public InputPressStyle inputPressStyle ;  
	public ActivationBehaviour activationBehaviour ; 
	public PauseController pauseController ; 

	public KeyCode[] keys ;
	public string[] inputNames ; 
	
	public bool guiOff ; 
	public GuiHandler pauseActiveGui ; 
	public GuiHandler pauseInactiveGui ;
	bool guiButtonDown ; 
	
	public ActivateGameObject[] gameObjectActivations ; 

	bool inputManagerAxisDown ; 
	bool inputManagerActivate ; 

	#region ClassInterfaceAssistants
	public enum InputPressStyle
	{
		Tapped,
		Held
	}
	
	public enum ActivationBehaviour
	{
		Toggle,
		Activate,
		Deactivate
	}
	
	public enum GameObjectActivationBehaviour
	{
		ActivateWithPauseActivation , 
		ActivateInverseWithPauseActivation , 
		ActivationWithInput ,
		DeactivationWithInput ,
	}
	
	[ Serializable ] 
	public class PauseControllerHandler
	{
		public ActivationBehaviour activationBehaviour ; 
		public PauseController pauseController ; 
	}
	
	[ Serializable ]
	public class ActivateGameObject
	{
		public GameObjectActivationBehaviour activationBehaviour ; 
		public bool affectsChildren ; 
		public GameObject gobj ; 
	}
	
	[ Serializable ]
	public class GuiStateHandler{
		public GuiHandler pauseGui ;
		public GuiHandler unpauseGui ;
	}
	
	[ Serializable ]
	public class GuiHandler
	{
		public GuiHandler()
		{
			rectColor = Color.white ; 
		}
		
		public enum DisplayMode
		{
			Button,
			Label,
		}
		public DisplayMode displayMode ; 
		public bool normalizedScreenRect;
		public Rect screenRect;
		public Color rectColor ; 
		public string textToDisplay;
		public Texture2D textureToDisplay ;
		public string toolTip; 
	}
	#endregion
	
	void SeekPauseControllersOnGameObject( ActivationBehaviour behaviour )
	{
		pauseController = (PauseController)gameObject.GetComponent< PauseController >() ; 
		
		if( pauseController )
		{
			pauseControllerName = pauseController.pauseName ; 
		}
		
	}
	
	
	void ProcessButtonPress( bool inputActivated )
	{
		if( inputPressStyle == InputPressStyle.Tapped )
		{
			if( inputActivated && !inputManagerAxisDown  )
			{
				inputManagerAxisDown = true ; 
				
				ProcessButtonPress() ; 
			}
			else if( !inputActivated && inputManagerAxisDown )
			{
				inputManagerAxisDown = false ; 
			}
		}
		else if( inputPressStyle == InputPressStyle.Held )
		{

			if( inputActivated && !inputManagerAxisDown )
			{ 
				inputManagerAxisDown = true ;
				ProcessButtonPress() ; 
			}
			else if( !inputActivated && inputManagerAxisDown )
			{
				inputManagerAxisDown = false ; 
				ProcessButtonPress() ; 
			}
		}	
		
	}
	
	
	void ProcessButtonPress()
	{
		if( activationBehaviour == ActivationBehaviour.Toggle )
		{
			pauseController.activatePause = !pauseController.activatePause ; 
		}
		else if( activationBehaviour == ActivationBehaviour.Activate )
		{
			pauseController.activatePause = true ; 
		}
		else if( activationBehaviour == ActivationBehaviour.Deactivate )
		{
			pauseController.activatePause = false ;
		}
		
		foreach( ActivateGameObject activateGameObject in gameObjectActivations )
		{
			if( !activateGameObject.gobj )
			{
				continue ; 
			}
			
			GameObject gobj = activateGameObject.gobj ; 
			

			
			GameObjectActivationBehaviour ab = activateGameObject.activationBehaviour ; 
			
			if ( ab == GameObjectActivationBehaviour.ActivationWithInput )
			{
				if( activateGameObject.affectsChildren )
				{					
					// 子のactiveを操作
					gobj.SetActiveRecursively( true ) ;
				}
				else
				{
					// 親子まとめて操作
					//gobj.active = true ;
					gobj.SetActive(true);
				}
			}
			else if( ab == GameObjectActivationBehaviour.DeactivationWithInput )
			{
				if( activateGameObject.affectsChildren )
				{
					gobj.SetActiveRecursively( false ) ;					
				}
				else
				{
					//gobj.active = false ;
					gobj.SetActive(true);
				}
			}
			
		}

	}
	
	
	protected void Reset()
	{
		SeekPauseControllersOnGameObject( activationBehaviour ) ; 
		
		if( pauseActiveGui != null )
			pauseActiveGui.rectColor = Color.white ; 
		
		if( pauseInactiveGui != null )
			pauseInactiveGui.rectColor = Color.white ; 
	}
	

	// Use this for initialization
	void Start () {
	}
	
	void OnGUI()
	{
		if( !pauseController || guiOff )
		{
			return ; 
		}
		 
		GuiHandler gui_handle ; 

		if( pauseController.activatePause )
		{
			gui_handle = pauseActiveGui ; 
		}
		else
		{
			gui_handle = pauseInactiveGui ; 
		}

		Rect final_rect = new Rect( gui_handle.screenRect ) ;
		if( gui_handle.normalizedScreenRect )
		{
			final_rect.x = final_rect.x * Screen.width ;
			final_rect.y = final_rect.y * Screen.height ;
			final_rect.width = final_rect.width * Screen.width ;
			final_rect.height = final_rect.height * Screen.height ; 
		}
			
		GUIContent gui_content = new GUIContent( gui_handle.textToDisplay, gui_handle.textureToDisplay, gui_handle.toolTip ) ; 
		
		GUI.backgroundColor = gui_handle.rectColor ; 
		
		if( gui_handle.displayMode == GuiHandler.DisplayMode.Button )
		{
			if( GUI.RepeatButton( final_rect, gui_content ) )
			{
			    guiButtonDown = true ; 
			}
			else if ( Event.current.type == EventType.Repaint )
			{
			    guiButtonDown = false ; 
			}
		}
		else if( gui_handle.displayMode == GuiHandler.DisplayMode.Label )
		{
			GUI.Label( final_rect, gui_content ) ; 
		}
	}
	
	void DoGameObjectActivationWatch()
	{
		bool pause_activation = pauseController.activatePause ; 
		
		foreach( ActivateGameObject activateGameObject in gameObjectActivations )
		{
			if( !activateGameObject.gobj )
			{
				continue ; 
			}
			
			GameObject gobj = activateGameObject.gobj ; 

			GameObjectActivationBehaviour ab = activateGameObject.activationBehaviour ; 
			
			if( ab == GameObjectActivationBehaviour.ActivateWithPauseActivation )
			{
				if( activateGameObject.affectsChildren )
				{
					gobj.SetActiveRecursively( pause_activation ) ; 
				}
				else
				{
					gobj.active = pause_activation ; 
				}
			}
			else if ( ab == GameObjectActivationBehaviour.ActivateInverseWithPauseActivation )
			{
				if( activateGameObject.affectsChildren )
				{
					gobj.SetActiveRecursively( !pause_activation ) ; 
				}
				else
				{
					gobj.active = !pause_activation ; 
				}
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
		bool inputActivated = false ;
		
		foreach( KeyCode key in keys )
		{
			inputActivated |= Input.GetKey( key ) ; 
		}
		
		foreach( String name in inputNames )
		{
			inputActivated |= !Mathf.Approximately( Input.GetAxis( name ), 0.0f ) ; 
		}
		
		inputActivated |= guiButtonDown ; 
		
		ProcessButtonPress( inputActivated ) ; 
		

		
		DoGameObjectActivationWatch() ; 
	}
	
	void OnDrawGizmosSelected()
	{
		if( pauseController && pauseControllerName != pauseController.pauseName )
		{
			pauseControllerName = pauseController.pauseName ; 
		}
	}
}
