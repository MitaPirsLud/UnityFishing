using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour {

	public void LoadLevel(string LevelToLoad){
		
		Application.LoadLevel (LevelToLoad);
	}

}
