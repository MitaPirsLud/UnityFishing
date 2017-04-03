using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpawnObjectsDemo : MonoBehaviour
{
	public GameObject BorderBox;


	public GameObject canon1;
	public GameObject canon2;
	public GameObject canon3;
	public GameObject canon4;

	public int FPS { get; private set; }
	public GameObject FPSLabel;

	public GameObject PresetCube;
	public GameObject PresetBall;
	public GameObject PresetBarrel;
	public GameObject PresetKyle;

	private List<GameObject> allSpawnedObjects = new List<GameObject>();

	private GameObject SpawnObj;
	public Transform shotSpawn;
	private float fireRate = 0.1f;

	private float nextFire = 0.0f;
	bool enableFire = false;

	public GameObject WaterPlane;
	public GameObject CetoWater;

	void Update ()
	{
		
		FPS = (int)(1f / Time.unscaledDeltaTime);
		FPSLabel.GetComponent<Text>().text = "FPS: " + FPS.ToString();
			
		if (enableFire && Time.time > nextFire)
		{
			enableFire = false;
			nextFire = Time.time + fireRate;
			GameObject ShotObj = Instantiate(SpawnObj, shotSpawn.position, shotSpawn.rotation) as GameObject;
			allSpawnedObjects.Add(ShotObj);
		}
	}

	int CubeCount = 0;
	int MaxCubeCount = 100;
	public void SpawnCube()
	{
		if(CubeCount < MaxCubeCount)
		{
			SpawnObj = PresetCube;
			enableFire = true;
			CubeCount++;
		}
	}

	int BallCount = 0;
	int MaxBallCount = 100;
	public void SpawnBall()
	{
		if(BallCount < MaxBallCount)
		{
			SpawnObj = PresetBall;
			enableFire = true;
			BallCount++;
		}
	}

	int BarrelCount = 0;
	int MaxBarrelCount = 100;
	public void SpawnBarrel()
	{
		if(BarrelCount < MaxBarrelCount)
		{
			SpawnObj = PresetBarrel;
			enableFire = true;
			BarrelCount++;
		}
	}

	int KyleCount = 0;
	int MaxKyleCount = 25;
	public void SpawnKyle()
	{
		if(KyleCount < MaxKyleCount)
		{
			SpawnObj = PresetKyle;
			enableFire = true;
			KyleCount++;
		}
	}
	public void ToggleCannons()
	{
		canon1.GetComponent<CanonScript>().ToggleOnOFF(); 
		canon2.GetComponent<CanonScript>().ToggleOnOFF(); 
		canon3.GetComponent<CanonScript>().ToggleOnOFF(); 
		canon4.GetComponent<CanonScript>().ToggleOnOFF(); 
	}
		
	bool UseCeto = false;

	public void ToggleCeto()
	{
		#if CETO
		UseCeto = !UseCeto;

		if(UseCeto == true)
		{
			WaterPlane.SetActive(false);
			CetoWater.SetActive(true);
		}
		else
		{
			WaterPlane.SetActive(true);
			CetoWater.SetActive(false);
		}
		RealisticWaterPhysics.currentUseCeto = UseCeto;
		#endif
	}

	public void ToggleBorder()
	{
		BorderBox.SetActive(!BorderBox.activeSelf);
	}

	public void ClearAll()
	{
		for (int i = 0; i < allSpawnedObjects.Count; i++) 
		{
			Destroy(allSpawnedObjects[i]);
		}
		canon1.GetComponent<CanonScript>().ClearAll(); 
		canon2.GetComponent<CanonScript>().ClearAll(); 
		canon3.GetComponent<CanonScript>().ClearAll(); 
		canon4.GetComponent<CanonScript>().ClearAll(); 

		KyleCount = 0;
		BarrelCount = 0;
		CubeCount = 0;
		BallCount = 0;
	}

}
