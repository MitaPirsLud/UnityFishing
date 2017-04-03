using UnityEngine;
using System.Collections;

//If you dont want to use a static water level, but something thats a bit more dynamic use this script.
//Drag and drop it on a GameObject that acts as your water.

[AddComponentMenu("RealisticPhysics/RealisticWaterPhysics/ExternalWaterLevelManager")]
public class ExternalWaterLevelManager : MonoBehaviour 
{
	public bool fastUpdates = false; // Switch from fixed to normal update.

	public float MinSeaLevel = 0f;
	public float MaxSeaLevel = 5f;
	public float CycleSpeed = 2.5f;
	public float MovementSpeed = 0.2f;

	private float CycleCounter = 0.0f;
	private float waterLevel;

	void Start()
	{
		waterLevel = MinSeaLevel;
	}

	void FixedUpdate () 
	{
		if(!fastUpdates)
		{
			newWaterLevel();
			this.transform.position = Vector3.Lerp(
				this.transform.position, 
				new Vector3(this.transform.position.x,waterLevel,this.transform.position.z), 
				Time.deltaTime * 0.1f);
			setWaterLevel();
		}
	}

	void Update () 
	{
		if(fastUpdates)
		{
			newWaterLevel();
			this.transform.position = Vector3.Lerp(
				this.transform.position, 
				new Vector3(this.transform.position.x,waterLevel,this.transform.position.z), 
				MovementSpeed);
			setWaterLevel();
		}
	}

	private void newWaterLevel()
	{
		CycleCounter += Time.fixedDeltaTime;
		if(CycleCounter >= CycleSpeed)
		{
			CycleCounter  = 0.0f;
			waterLevel = UnityEngine.Random.Range(MinSeaLevel, MaxSeaLevel);
		}
	}

	private void setWaterLevel()
	{
		if(RealisticWaterPhysics.currentWaterLevel != this.transform.position.y) // no need to change something to the same :P
			RealisticWaterPhysics.currentWaterLevel = this.transform.position.y;
	}
}
