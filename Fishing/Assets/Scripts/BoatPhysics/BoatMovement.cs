using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour {

	[SerializeField]
	private Transform Boat;
	[SerializeField]
	private Transform Cam;
	private bool right = true;

	void FixedUpdate(){

		// Brod se prvo ljulja desno, right je true dok nije 2f na rotaciji
		float look=0f;
		float ThisLook = (float) Cam.transform.eulerAngles.y;
		float interval = 0.15f / 90f;
		if (ThisLook >= 0f && ThisLook < 90f) 
			look = -ThisLook * interval;
		if (ThisLook >= 90f && ThisLook < 180f) 
			look = -interval*(90-(ThisLook-90f));
		if (ThisLook >= 180f && ThisLook < 270f) 
			look = interval*(ThisLook-180f);
		if (ThisLook >= 270f && ThisLook < 360f) 
			look = interval*(90-(ThisLook-270));
		Debug.Log (look);
		Boat.Rotate (new Vector3 (0f, 0f, look- Boat.rotation.z)*7f);
		Cam.transform.position = new Vector3 (Boat.position.x+0.14f-(look*1.5f), 0.614f,-1.027f );
	


	/*	if (right) {
			Boat.Rotate ((new Vector3 (0f, 0f, Boat.rotation.z + 0.2f+look)) * 0.2f);

			if (Boat.rotation.z >= 0.04)
				right = false;
		} else {
			Boat.Rotate ((new Vector3(0f,0f,Boat.rotation.z - 0.2f+look)) * 0.2f);
			if (Boat.rotation.z <= -0.04)
				right = true;
		}*/

	}

	IEnumerator RotateLeft(){
		Boat.Rotate (Vector3.left * 0.2f);
		yield return null;
	}
	IEnumerator RotateRight(){
		Boat.Rotate (Vector3.right * 0.2f);
		yield return null;
	}
}
