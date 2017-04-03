using UnityEngine;
using System.Collections;

[AddComponentMenu("RealisticPhysics/RealisticWaterPhysics/RealisticRiverFlow")]
public class RealisticRiverFlow : MonoBehaviour 
{
	public bool ForceOnWaterLevel = true;
	public Mesh GizmosIconMesh;

	private float gizmosHeightCorrection;
	private Vector3 gizmosIconLocation;
	private Vector3 thisPosition;
	//private GameObject OnlyEffectWaterObject = null;

	public float currentForce = 100.0f;


	void Start ()
	{
		if(this.gameObject.GetComponent<BoxCollider>() == false)
		{
			this.gameObject.AddComponent<BoxCollider>();
			this.GetComponent<BoxCollider>().isTrigger = true;
		}
		else
		{
			this.GetComponent<BoxCollider>().isTrigger = true;
		}
	}

	void OnTriggerStay(Collider other)
	{
		RealisticBuoyancy RB = other.GetComponent<RealisticBuoyancy>();
		if(RB != null && other.transform.position.y < RealisticWaterPhysics.currentWaterLevel)
		{
			other.attachedRigidbody.AddForce(-transform.right * (currentForce * RB.getMass));
		}
	}

	private void OnDrawGizmos() 
	{
		thisPosition = this.transform.position;

		if(ForceOnWaterLevel == true)
		{
			this.transform.position = new Vector3(thisPosition.x, 
				RealisticWaterPhysics.currentWaterLevel - gizmosHeightCorrection,
				thisPosition.z);
		}

		gizmosHeightCorrection = this.GetComponent<Collider>().bounds.extents.y;

		gizmosIconLocation = new Vector3(this.transform.position.x,
			thisPosition.y + gizmosHeightCorrection,
			thisPosition.z);


		Gizmos.DrawWireMesh(GizmosIconMesh, gizmosIconLocation, this.transform.rotation, (this.transform.localScale / 3));

	}
}
