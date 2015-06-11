using UnityEngine;
using System.Collections;
using EleckTek ; 

public class PauseControllerMatcher : MonoBehaviour {
	
	public PauseController controllerToWatch ; 
	public PauseControllerWatcher[] watchers ; 
	
	[ System.Serializable ]
	public class PauseControllerWatcher
	{
		public PauseController pauseController ; 
		public bool inversely ;
		
		public void Update( PauseController to_watch )
		{
			if( !inversely &&  to_watch.activatePause != pauseController.activatePause )
			{
				pauseController.activatePause = to_watch.activatePause ;
			}
			else if( inversely && to_watch.activatePause == pauseController.activatePause )
			{
				pauseController.activatePause = !to_watch.activatePause ;
			}
		}
	}
	
	void LateUpdate()
	{
		DoWatch() ; 
	}
	
	void DoWatch()
	{
		foreach( PauseControllerWatcher watcher in watchers )
		{
			watcher.Update( controllerToWatch ) ; 
		}
	}
	
}
