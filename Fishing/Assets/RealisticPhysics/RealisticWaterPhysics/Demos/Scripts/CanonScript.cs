using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanonScript : MonoBehaviour 
{
	private List<GameObject> allSpawnedObjects = new List<GameObject>();

	public int Count;
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;

	private float nextFire = 0.0f;
	public bool enableFire = false;

	public bool AddForce = true;

	public void ToggleOnOFF()
	{
		enableFire = !enableFire;
	}

	void Update ()
	{
		if (enableFire && Time.time > nextFire && Count < 100)
		{
			Count += 1;
			nextFire = Time.time + fireRate;
			GameObject ShotObj = Instantiate(shot, shotSpawn.position, shotSpawn.rotation) as GameObject;
			allSpawnedObjects.Add(ShotObj);
			if(AddForce)
				ShotObj.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * (1000f * (shot.GetComponent<Rigidbody>().mass /2)));
		}
	}

	public void ClearAll()
	{
		for (int i = 0; i < allSpawnedObjects.Count; i++) 
		{
			Destroy(allSpawnedObjects[i]);
		}
		Count = 0;
	}
}
