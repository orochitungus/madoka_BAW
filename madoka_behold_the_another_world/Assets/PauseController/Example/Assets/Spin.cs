using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

	public Vector3 spin ; 
	public Material alphaMaterial ; 
	int counter ; 
	// Use this for initialization
	void Start () {
	
	}
	
	public bool IsSpinning()
	{
		return counter > 0 ; 
	}
	
	public void DoSpin()
	{
		if( !IsSpinning() )
		{
			if( alphaMaterial )
			{
				Color color = alphaMaterial.color ;
				color.a = 1.0f ; 
				alphaMaterial.color = color ; 
			}
		}
		
		++counter ; 	
		
	}
	
	public void DontSpin()
	{
		--counter ;
		if( !IsSpinning() )
		{

			Color color = alphaMaterial.color ;
			color.a = .4f ; 
			alphaMaterial.color = color ; 

			counter = 0 ; 
			transform.rotation = Quaternion.identity ; 
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if( IsSpinning() )
		{
			transform.Rotate( spin * Time.deltaTime ) ; 
		}
		
	}
}
