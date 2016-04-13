using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
	public GameObject Chara;
	public GameObject Cam;
	public GameObject CamBody;
	Vector3 Mouse;
	Vector3 MouseMove;
	Vector2 crot;
	Vector3 CamBodyDef;
	Vector3 CamBodyNow;

	public float CamBodyMin = -0.5f;
	public float CamBodyMax = -6.0f;
	public float ZoomSpd = 0.25f;
	public float NowZ = 0;
	public float MobileSpd = 0.5f;
	
	private float backDist = 0.0f;
	private Vector2 T1Pos;
	private Vector2 T2Pos;
	public float DownTime = 0.5f;

	public float NextIdleAnm = 0;


	// Use this for initialization
	void Start () {
		CamBodyDef = CamBody.transform.localPosition;
		CamBodyNow = CamBodyDef;
		Mouse = Input.mousePosition;
		crot = new Vector2 (0, 0);
		NextIdleAnm = Random.Range (20, 30);
	}
	
	// Update is called once per frame
	void Update () {
		NextIdleAnm -= Time.deltaTime;
		if (NextIdleAnm < 0) {
			Chara.GetComponent<Animator> ().SetTrigger ("Idle");
			Chara.GetComponent<Animator> ().SetInteger ("RndMotion",0);
			NextIdleAnm = Random.Range (20, 30);
		}

		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform == RuntimePlatform.Android) {
			// Android
			if(Input.touchCount >= 2)
			{
				DownTime = 0;
				Touch t1 = Input.GetTouch(0);
				Touch t2 = Input.GetTouch(1);
				//2点タッチ開始時の距離を記憶
				if(t2.phase == TouchPhase.Began){
					backDist = Vector2.Distance(t1.position, t2.position);
					T1Pos = t1.position;
					T2Pos = t2.position;
				}
				// ピンチイン・アウト.
				else if(t1.phase == TouchPhase.Moved && t2.phase == TouchPhase.Moved){
					float newDist = Vector2.Distance(t1.position, t2.position);
					float transZ = (backDist - newDist) / 10.0f;
					Vector2 newPos1 = t1.position;
					Vector2 newPos2 = t2.position;
					Vector2 vec1 = T1Pos - newPos1;
					Vector2 vec2 = T2Pos - newPos2;
					float d = Vector2.Dot(vec1,vec2);
					Debug.Log(d);
					if(d > 0)
					{
						Cam.transform.localPosition += Cam.transform.localRotation * new Vector3 (vec1.x * 0.005f*MobileSpd, vec1.y * 0.005f*MobileSpd, 0);
					}else if(transZ != 0){
						Vector3 newPos = CamBody.transform.localPosition;
						newPos.z -= transZ*ZoomSpd*MobileSpd;
						//zMin～zMaxの範囲内のみ.
						if(CamBodyMax <= newPos.z && newPos.z <= CamBodyMin){
							CamBody.transform.localPosition = newPos;
							backDist = newDist;
						}
					}
					T1Pos = t1.position;
					T2Pos = t2.position;
				}
			}else if(Input.touchCount == 1){
				Touch t1 = Input.GetTouch(0);
				if(t1.phase == TouchPhase.Began){
					T1Pos = t1.position;
					DownTime=0.5f;
				}else if(t1.phase == TouchPhase.Moved){
					Vector2 newPos = t1.position;
					Vector2 trans = (T1Pos - newPos);
					float len = Vector2.Distance(T1Pos,newPos);
					if(len > 1.5f)
					{
						crot.x += trans.x*1.0f*MobileSpd;
						crot.y += trans.y*1.0f*MobileSpd;
						Cam.transform.rotation = Quaternion.Euler (crot.y, -crot.x, 0);
						T1Pos = t1.position;
					}
				}
				DownTime -= Time.deltaTime;
			}
		} else {
			Vector3 Now = Input.mousePosition;

			MouseMove = Now - Mouse;
			Mouse = Now;
			
			if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (2)) {
				MouseMove = Vector3.zero;
			}
			if (Input.GetMouseButton (0)) {
				DownTime -= Time.deltaTime;
				crot.x += MouseMove.x;
				crot.y += MouseMove.y;
				Cam.transform.rotation = Quaternion.Euler (-crot.y, crot.x, 0);
			}
			if (Input.GetMouseButtonUp (0)) {
				DownTime = 0.5f;
			}
			float scroll = Input.GetAxis ("Mouse ScrollWheel");
			if (scroll > 0) {
				CamBodyNow.z += ZoomSpd;
				if (CamBodyNow.z > CamBodyMin)
					CamBodyNow.z = CamBodyMin;
			}
			if (scroll < 0) {
				CamBodyNow.z -= ZoomSpd;
				if (CamBodyNow.z < CamBodyMax)
					CamBodyNow.z = CamBodyMax;
			}
			NowZ = CamBodyNow.z;
			CamBody.transform.localPosition = CamBodyNow;

			if (Input.GetMouseButton (2)) {
				Cam.transform.localPosition += Cam.transform.localRotation * new Vector3 (-MouseMove.x * 0.01f, -MouseMove.y * 0.01f, 0);
			}
		}

	}
}
