using UnityEngine;
using UnityEditor ; 
using System.Collections;
using EleckTek ; 

[ CustomEditor( typeof( PauseController ) ) ] 
public class PauseControllerEditor : PauseControllerEditorBase {


	[MenuItem ("Window/Pause Controller/Pause Controller Manager Window")]
    static void Init () {
        // Get existing open window or if none, make a new one:
        PauseControllerWindow window = (PauseControllerWindow)EditorWindow.GetWindow (typeof (PauseControllerWindow));
		window.Show() ; 
    }
	
	
	[MenuItem ("GameObject/Pause Controller/Create Pause Controller")]
	static void CreatePauseControllerGameObject() {
		new GameObject("Pause Controller", typeof(PauseController) ) ; 
		
	}
	
	[MenuItem ("GameObject/Pause Controller/Create Pause Controller with Input")]
	static void CreatePauseControllerGameObjectWithInput() {
		GameObject gobj = new GameObject( "Pause Controller" ) ; //, typeof( PauseController ), typeof( PauseControllerInputDetector ) ) ; 
		
		PauseController controller = gobj.AddComponent< PauseController >() ; 
		controller.pauseName = "NewPauseFromMenu" ; 
		gobj.AddComponent< PauseControllerInputDetector >() ; 
		
		
	}
	
	[MenuItem ("Component/Pause Controller/Pause Controller")]
	static void AddPauseControllerComponent() {
		foreach( UnityEngine.Object obj in Selection.objects )
		{
			GameObject gobj = ( GameObject )obj ; 
			if( gobj )
			{
				gobj.AddComponent< PauseController >() ; 
			}
		}
	}
	
	[MenuItem ("Component/Pause Controller/Helpers/Pause Controller Input Detector")]
	static void AddInputDetectorComponent() {
		foreach( UnityEngine.Object obj in Selection.objects )
		{
			GameObject gobj = ( GameObject )obj ; 
			if( gobj )
			{
				gobj.AddComponent< PauseControllerInputDetector >() ; 
			}
		}
	}
	
	[MenuItem ("Component/Pause Controller/Helpers/Pause Controller Matcher")]
	static void AddMatcherComponent() {
		foreach( UnityEngine.Object obj in Selection.objects )
		{
			GameObject gobj = ( GameObject )obj ; 
			if( gobj )
			{
				gobj.AddComponent< PauseControllerMatcher >() ; 
			}
		}
	}
	
	

	
}
