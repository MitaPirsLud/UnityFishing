using System.Collections.Generic;
using UnityEngine;


#if DW // Added support for DynamicWaterSystem
using LostPolygon.DynamicWaterSystem; 
#endif

#if CETO // Added support for Ceto: Ocean System
using Ceto;
#endif

[RequireComponent (typeof(Collider))][RequireComponent (typeof(Rigidbody))]
public class RealisticBuoyancy : MonoBehaviour
{
	//For v2.1 ;)
//	[Header("Realistic Material")]
//	public ScriptableObject RPM;

	[Header("RunTime options")]
	public bool LightRunTimeEdits = false;
	public bool HeavyRunTimeEdits = false;

	[HideInInspector]
	public bool isConcave = false;

	[HideInInspector]
	public MaterialTypes SelectedMaterialType = MaterialTypes.Solid;

	[HideInInspector]
	public GasesMaterialList selectedGasesMaterial;

	[HideInInspector]
	public LiquidsMaterialList selectedLiquidsMaterial;

	[HideInInspector]
	public SolidsMaterialList selectedSolidsMaterial;

	[HideInInspector]
	public int slicesPerAxis = 2;

	[HideInInspector]
	public int voxelsLimit = 16;

	[Range(0,100)][Tooltip("Less solid is lighter object.")]
	public int percentageSolid = 100;

	private bool inWater = false;
	
	private float density;
	private float volume;
	private float mass;
	private float WATER_DENSITY;
	private float selectedMaterialValue;

	private float voxelHalfHeight;
	private Vector3 localArchimedesForce;
	private Vector3 localArchimedesForceAir;
	private List<Vector3> voxels;
	private bool isMeshCollider;
	private List<Vector3[]> forces; // For drawing force gizmos
	private float DAMPFER = 0.1f;

	private float percentageSolidMass = 0f;
	private float percentageAir = 0f;
	private float waterDensity;
	private float CurrentVelocity;

	private Bounds bounds;
	private Rigidbody _thisRigidbody;

	//For Runtime edits!
	private float OldselectedMaterialValue;
	private MeshFilter oldMesh;
	private Collider oldCollider;
	private int oldPrcentageSolid;
	private Vector3 oldScale;

	[Tooltip("")]
	public string infoMaterialType;

	[Tooltip("")]
	public string infoSubType;

	[Tooltip("")]
	public string CalculatedDensity;

	[Tooltip("")]
	public string CalculatedMass;

	[Tooltip("")]
	public string CalculatedVolume;

	[Tooltip("Disable buoyancy but keep the script active.")]
	public bool BuoyancyActive = true;
	[HideInInspector]
	public bool BuoyancyEnabled = true;
	// ^ Added support of other buoyancy systems, 
	//This allows you to disable the Buoyancy, but keep the calculations in.

	//Other support stuff.
	#if PW
	PlayWay.Water.Water PlayWayWater;
	#endif


	//To allow for MovePosition
	//ignoreChanges will ignore the massive change is location
	//Stopping added force.
	private int ignoreChangesMaxDelay = 25;
	private int ignoreChangesDelay = 0;
	private bool ignoreChanges = false;

	//To stop a incorrect setup script.
	private int reSetupTimeout = 0;


	/// <summary>
	/// Get the calculated mass.
	/// </summary>
	public float getMass
	{
		get
		{
			return mass;
		}
	}
	
	/// <summary>
	/// Returns true if in water.
	/// </summary>
	public bool isInWater
	{
		get
		{
			return inWater;
		}
	}
	/// <summary>
	/// Set the MaterialType
	/// </summary>
	public void setMaterialType(MaterialTypes _MaterialTypes)
	{
		SelectedMaterialType = _MaterialTypes;
	}

	/// <summary>
	/// Set the Material solid sub type
	/// </summary>
	public void setSubType(SolidsMaterialList _MaterialSubTypes)
	{
		selectedSolidsMaterial = _MaterialSubTypes;
	}

	/// <summary>
	/// Set the Material liquid type
	/// </summary>
	public void setSubType(LiquidsMaterialList _MaterialSubTypes)
	{
		selectedLiquidsMaterial = _MaterialSubTypes;
	}

	/// <summary>
	/// Set the Material gas type
	/// </summary>
	public void setSubType(GasesMaterialList _MaterialSubTypes)
	{
		selectedGasesMaterial = _MaterialSubTypes;
	}

	/// <summary>
	/// Will re do the setup.
	/// </summary>
	public void reSetup()
	{
		reSetupTimeout++;
		setup();
	}

	#if PW
	private void findPlayWaterWater()
	{
		if(PlayWayWater == null)
			PlayWayWater = (PlayWay.Water.Water) FindObjectOfType(typeof(PlayWay.Water.Water));
		if(RealisticWaterPhysics.currentDebugEnabled)
		{
			Debug.LogFormat("Found PlayWay Water: {0}", ( (PlayWayWater == null) ? "No" : "Yes").ToString() );
		}
	}
	#endif

	private void setup()
	{
		//Eanble support
		#if PW
		findPlayWaterWater();
		#endif

		if(RealisticWaterPhysics.currentDebugEnabled)
		{
			forces = new List<Vector3[]>(); // For drawing force gizmos
		}

		selectedMaterialValue = getSelectedMaterialValue();

		_thisRigidbody = this.GetComponent<Rigidbody>();

		waterDensity = RealisticWaterPhysics.currentWaterDensity;

		isMeshCollider = GetComponent<MeshCollider>() != null;

		if(this.gameObject.GetComponent<MeshFilter>() && this.gameObject.GetComponent<MeshFilter>().sharedMesh != null)
		{ 
			volume = MeshVolume.getVolume(this.gameObject);
			Bounds meshBounds = GetComponent<MeshFilter>().sharedMesh.bounds;

			bounds = new Bounds(); //A fix for a odd bug.
			bounds.center = meshBounds.center;
			bounds.extents = new Vector3(
				meshBounds.extents.x * this.transform.localScale.x, 
				meshBounds.extents.y * this.transform.localScale.y,
				meshBounds.extents.z * this.transform.localScale.z); //meshBounds.extents * this.transform.localScale;
			
			if(RealisticWaterPhysics.currentDebugEnabled)
				Debug.LogFormat("bounds.extents={0} |bounds.center={1} |this.transform.position={2} |_thisRigidbody.centerOfMass={3}",bounds.extents,bounds.center,this.transform.position,_thisRigidbody.centerOfMass);

			//if object scale = 10,10,10 and sharedMesh.bounds; returns 0.5 then result must be 5,5,5; FIX THIS !!!

			secondSetup();
		}
		else if(this.gameObject.GetComponent<Collider>())
		{ 
			// Use collider based bounds
			if(RealisticWaterPhysics.currentDebugEnabled)
				Debug.LogFormat("[Realistic Water Physics] Object: {0} is missing a mesh filter, using collider Bounds!", this.gameObject.name);
			volume = MeshVolume.getVolumeByColliderBounds(this.gameObject.GetComponent<Collider>());
			bounds = GetComponent<Collider>().bounds;

			secondSetup();
		}
		else
		{
			Debug.LogFormat("[Realistic Water Physics] Object: {0} is missing a mesh filter and collider.", this.gameObject.name);
			this.gameObject.SetActive(false);
		}
	}

	void secondSetup()
	{
		//Now the percentage thats solit and the remaining is air.
		percentageSolidMass = volume * ( ( selectedMaterialValue / 100 ) * percentageSolid );
		percentageAir = volume * ( ( RealisticWaterPhysics.currentAirDensity / 100 ) * (100 - percentageSolid) );

		//The totaal mass of the object is the amount of solit material + the remaining amount of air.
		mass = percentageSolidMass + percentageAir;

		//the density of the intire object is its calculated mass / its volume.
		density = mass / volume;
		_thisRigidbody.mass = mass;

		if(RealisticWaterPhysics.currentDebugEnabled)
		{
			Debug.Log("volume:" + volume);
			Debug.Log("percentageSolidMass:" + percentageSolidMass);
			Debug.Log("percentageAir:" + percentageAir);
			Debug.Log("density set to:" + density);
			Debug.Log("Mass set to:" + mass);
		}
		//Now the density of the ocean. Global.
		WATER_DENSITY = RealisticWaterPhysics.currentWaterDensity;

		//Added support of others
		#if DW

		if(RealisticWaterPhysics.currentUseDW == true)
		{
			if(GetComponent<BuoyancyForce>() == true)
			{
				if(GetComponent<BuoyancyForce>().enabled == true)
				{
					GetComponent<BuoyancyForce>().Density = density;
					BuoyancyEnabled = false;
					// Set the calculated density, as we used a material based system.
					//And we dont like to have to set it by hand. ;)
				}
			}
		}
		#endif

		if(BuoyancyEnabled)
		{
			// Store original rotation and position
			var originalRotation = transform.rotation;
			var originalPosition = transform.position;
			transform.rotation = Quaternion.identity;
			transform.position = Vector3.zero;

			if (bounds.size.x < bounds.size.y)
			{
				voxelHalfHeight = bounds.size.x;
			}
			else
			{
				voxelHalfHeight = bounds.size.y;
			}
			if (bounds.size.z < voxelHalfHeight)
			{
				voxelHalfHeight = bounds.size.z;
			}

			voxelHalfHeight /= 2 * slicesPerAxis;

			if(RealisticWaterPhysics.currentDebugEnabled)
				Debug.LogFormat("slicesPerAxis={0} |bounds={1} |voxelsLimit={2}",slicesPerAxis,bounds,voxelsLimit);
			if(this.gameObject.GetComponent<MeshFilter>() && this.gameObject.GetComponent<MeshFilter>().sharedMesh != null)
			{ 
				_thisRigidbody.centerOfMass = new Vector3(0, -bounds.extents.y * 0f, 0) + transform.InverseTransformPoint(bounds.center);
			}

			voxels = SliceIntoVoxels(isMeshCollider && isConcave); // need to change this

			// Restore original rotation and position
			transform.rotation = originalRotation;
			transform.position = originalPosition;

			WeldPoints(voxels, voxelsLimit);

			float archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y) * volume;
			localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0) / voxels.Count;

			float archimedesForceMagnitudeAir = RealisticWaterPhysics.currentAirDensity * Mathf.Abs(Physics.gravity.y) * volume;
			localArchimedesForceAir = new Vector3(0, archimedesForceMagnitudeAir, 0) / voxels.Count;

			if(RealisticWaterPhysics.currentDebugEnabled)
			{
				Debug.Log(string.Format("[RealisticBuoyancy.cs] Name=\"{0}\" volume={1:0.0}, mass={2:0.0}, density={3:0.0}, water density={4:0.0}, air density={5:0.0}, useCeto: {6}, useAquas: {7}, UseDW:{8}, UsePW{9}", name, volume, GetComponent<Rigidbody>().mass, density, RealisticWaterPhysics.currentWaterDensity, RealisticWaterPhysics.currentAirDensity, RealisticWaterPhysics.currentUseCeto, RealisticWaterPhysics.currentUseAquas, RealisticWaterPhysics.currentUseDW,RealisticWaterPhysics.currentUsePlayWater));
			}
			if(HeavyRunTimeEdits || LightRunTimeEdits)
			{
				getRunTimeEditsInfo();
			}
			else
			{
				//Bit of more info to the user
				setDesabledRunTimeEditsInfo();
			}
		}
	}

	private void setDesabledRunTimeEditsInfo()
	{
		infoMaterialType = "RunTimeEdits are disabled.";
		infoSubType = "RunTimeEdits are disabled.";
		CalculatedDensity = "RunTimeEdits are disabled.";
		CalculatedMass = "RunTimeEdits are disabled.";
		CalculatedVolume = "RunTimeEdits are disabled.";
	}

	private void getRunTimeEditsInfo()
	{
		infoMaterialType = SelectedMaterialType.ToString();

		if(SelectedMaterialType == MaterialTypes.Solid)
		{
			infoSubType = selectedSolidsMaterial.ToString();
		}
		else if(SelectedMaterialType == MaterialTypes.Liquids)
		{
			infoSubType = selectedLiquidsMaterial.ToString();
		}
		else
		{
			infoSubType = selectedGasesMaterial.ToString();
		}

		CalculatedDensity = density.ToString();;
		CalculatedMass = mass.ToString();
		CalculatedVolume = volume.ToString();

	}

	private float getSelectedMaterialValue()
	{
		if(SelectedMaterialType == MaterialTypes.Solid)
		{
			return PhysicsMaterialsList.getSolidMaterialValue( selectedSolidsMaterial);
		}
		else if(SelectedMaterialType == MaterialTypes.Liquids)
		{
			return PhysicsMaterialsList.getLiquidsMaterialValue( selectedLiquidsMaterial);
		}
		else
		{
			return PhysicsMaterialsList.getGasesMaterialValue( selectedGasesMaterial);
		}
		return 0.0f;
	}

	private List<Vector3> SliceIntoVoxels(bool concave)
	{
		var points = new List<Vector3>(slicesPerAxis * slicesPerAxis * slicesPerAxis);

		if (concave)
		{
			var meshCol = GetComponent<MeshCollider>();

			var convexValue = meshCol.convex;
			meshCol.convex = false;

			// Concave slicing
			bounds = GetComponent<Collider>().bounds;
			for (int ix = 0; ix < slicesPerAxis; ix++)
			{
				for (int iy = 0; iy < slicesPerAxis; iy++)
				{
					for (int iz = 0; iz < slicesPerAxis; iz++)
					{
						float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
						float y = bounds.min.y + bounds.size.y / slicesPerAxis * (0.5f + iy);
						float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);

						var p = transform.InverseTransformPoint(new Vector3(x, y, z));

						if (PointIsInsideMeshCollider(meshCol, p))
						{
							points.Add(p);
						}
					}
				}
			}
			if (points.Count == 0)
			{
				points.Add(bounds.center);
			}

			meshCol.convex = convexValue;
		}
		else
		{
			// Convex slicing
			bounds = GetComponent<Collider>().bounds; // bounds based on the collider.
			for (int ix = 0; ix < slicesPerAxis; ix++)
			{
				for (int iy = 0; iy < slicesPerAxis; iy++)
				{
					for (int iz = 0; iz < slicesPerAxis; iz++)
					{
						float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
						float y = bounds.min.y + bounds.size.y / slicesPerAxis * (0.5f + iy);
						float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);

						var p = transform.InverseTransformPoint(new Vector3(x, y, z));

						points.Add(p);
					}
				}
			}
		}

		return points;
	}


	private static bool PointIsInsideMeshCollider(Collider c, Vector3 p)
	{
		Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

		for (int i = 0; i < directions.Length; i++)
		{
			RaycastHit hit;
			if (c.Raycast(new Ray(p - directions[i] * 1000, directions[i]), out hit, 1000f) == false)
			{
				return false;
			}
		}
		return true;
	}

	private static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
	{
		float minDistance = float.MaxValue, maxDistance = float.MinValue;
		firstIndex = 0;
		secondIndex = 1;

		for (int i = 0; i < list.Count - 1; i++)
		{
			for (int j = i + 1; j < list.Count; j++)
			{
				float distance = Vector3.Distance(list[i], list[j]);
				if (distance < minDistance)
				{
					minDistance = distance;
					firstIndex = i;
					secondIndex = j;
				}
				if (distance > maxDistance)
				{
					maxDistance = distance;
				}
			}
		}
	}
		
	private static void WeldPoints(IList<Vector3> list, int targetCount)
	{
		if (list.Count <= 2 || targetCount < 2)
		{
			return;
		}

		while (list.Count > targetCount)
		{
			int first, second;
			FindClosestPoints(list, out first, out second);

			var mixed = (list[first] + list[second]) * 0.5f;
			list.RemoveAt(second); // the second index is always greater that the first => removing the second item first
			list.RemoveAt(first);
			list.Add(mixed);
		}
	}

	public float GetWaterLevel(float x, float z)
	{
		//For more info read the HowToUse.Txt file included.
		#if CETO
		if(RealisticWaterPhysics.currentUseCeto)
		{
			if(Ocean.Instance != null)
			{
				return Ocean.Instance.QueryWaves(x, z);
			}
			else
			{
				Debug.Log("[Realistic Water Physics] Ceto support enabled but missing Ocean!");
				this.gameObject.SetActive(false);
				return 0;
			}
		}
		#endif

		#if PW
		if(RealisticWaterPhysics.currentUsePlayWater == true)
		{
			if(PlayWayWater != null)
			{
				return PlayWayWater.GetHeightAt(x,z,0,1.0f,PlayWayWater.Time) - voxelHalfHeight; //small correction!
			}
			else
			{
				findPlayWaterWater();
			}
		}
		#endif
		//If code ends up here, that means no support is enabled :P Cheap version to check hehe
		//If you have a external water level manager you need to keep this!
		//Read the HowToUse file for more info!
		return RealisticWaterPhysics.currentWaterLevel;


	}




	void RememberOlds()
	{
		OldselectedMaterialValue = selectedMaterialValue;
		oldMesh = this.gameObject.GetComponent<MeshFilter>();
		oldCollider = this.gameObject.GetComponent<Collider>();
		oldPrcentageSolid = percentageSolid;
		oldScale = this.gameObject.transform.localScale;
	}
	bool CheckOlds()
	{
		if( OldselectedMaterialValue != selectedMaterialValue ||
			oldMesh != this.gameObject.GetComponent<MeshFilter>() ||
			oldCollider != this.gameObject.GetComponent<Collider>() ||
			oldPrcentageSolid != percentageSolid ||
			oldScale != this.gameObject.transform.localScale)
		{
			RememberOlds();
			return true;
		}
		else
		{
			return false;
		}
	}



	/// <summary>
	/// Move the RigidBody to position
	/// Without tipping over the object.
	/// </summary>
	public void MovePosition(Vector3 position)
	{
		//Overwrite the default movePosition of Rigidbody!
		//Mark ignoreChanges as true will ingore the following changes without losing buoyancy.
		ignoreChangesDelay = 0;
		ignoreChanges = true;
		if(_thisRigidbody != null)
			_thisRigidbody.MovePosition(position);
	}
		
	/// Calculates physics.
	/// 
	private void CalculatePhysics()
	{
		if(_thisRigidbody.isKinematic == false)
		{
			if(HeavyRunTimeEdits || LightRunTimeEdits && CheckOlds())
			{
				if(HeavyRunTimeEdits)
				{
					if(this.gameObject.GetComponent<MeshFilter>() && this.gameObject.GetComponent<MeshFilter>().mesh != null)
					{ 
						volume = MeshVolume.getVolume(this.gameObject);
					}
					else if(this.gameObject.GetComponent<Collider>())
					{ 
						// Use collider based bounds
						volume = MeshVolume.getVolumeByColliderBounds(this.gameObject.GetComponent<Collider>());
					}
					else
					{
						return;
					}
				}

				percentageSolidMass = volume * ( ( selectedMaterialValue / 100 ) * percentageSolid );
				percentageAir = volume * ( ( RealisticWaterPhysics.currentAirDensity / 100 ) * (100 - percentageSolid) );

				//The totaal mass of the object is the amount of solit material + the remaining amount of air.
				mass = percentageSolidMass + percentageAir;

				//the density of the intire object is its calculated mass / its volume.
				density = mass / volume;

				_thisRigidbody.mass = mass;

				getRunTimeEditsInfo();

			}

			if(RealisticWaterPhysics.currentDebugEnabled) // Just skip this if debug info is disabled.
			{
				forces.Clear(); // For drawing force gizmos
			}

			if(BuoyancyEnabled)
			{
				if(voxels == null)
				{
					if(reSetupTimeout < 5) // its a fail safe, if indeed no voxels are found and resetup is done 5 times, it wil trip!
					{	
						reSetup();
					}
					else
					{
						this.GetComponent<RealisticBuoyancy>().enabled = false;
						Debug.LogErrorFormat("Gameobject {0} Disabled due to incorrect setup!", this.name);
					}
				}
				else
				{
					inWater = false;
					for (int i = 0; i < voxels.Count; i++) 
					{
						var wp = transform.TransformPoint(voxels[i]);
						float waterLevel = GetWaterLevel(wp.x, wp.z);

						if (wp.y - voxelHalfHeight < waterLevel) // if underwater
						{
							inWater = true;
							float k = (waterLevel - wp.y) / (2 * voxelHalfHeight) + 0.5f;
							if (k > 1)
							{
								k = 1f;
							}
							else if (k < 0)
							{
								k = 0f;
							}

							var velocity = GetComponent<Rigidbody>().GetPointVelocity(wp);
							var localDampingForce = -velocity * DAMPFER * _thisRigidbody.mass;
							var force = localDampingForce + Mathf.Sqrt(k) * localArchimedesForce;

							//Again this is for MovePosition
							if(ignoreChanges == true && ignoreChangesDelay < ignoreChangesMaxDelay)
							{
								ignoreChangesDelay++;
							}
							else
							{
								if(ignoreChangesDelay > ignoreChangesMaxDelay)
									ignoreChanges = false;

								_thisRigidbody.AddForceAtPosition(force, wp);
							}

							if(RealisticWaterPhysics.currentDebugEnabled)
							{
								forces.Add(new[] { wp, force }); // For drawing force gizmos
							}
						}
						else //not underwater? so above water!
						{
							if(RealisticWaterPhysics.currentAirDensity > 0f) //is there air to calculate with ?
							{
								float k = ((waterLevel + RealisticWaterPhysics.currentAirLevel) - wp.y) / (2 * voxelHalfHeight) + 0.5f;
								if (k > 1)
								{
									k = 1f;
								}
								else if (k < 0)
								{
									k = 0f;
								}

								var velocity = GetComponent<Rigidbody>().GetPointVelocity(wp);
								//var localDampingForce = -velocity * DAMPFER * GetComponent<Rigidbody>().mass;
								var localDampingForce = -velocity * 0.01f * _thisRigidbody.mass; //remove the dempfer, but keep a tiny bit in.
								var force = localDampingForce + Mathf.Sqrt(k) * localArchimedesForceAir;

								if(ignoreChanges == true && ignoreChangesDelay < ignoreChangesMaxDelay)
								{
									ignoreChangesDelay++;
								}
								else
								{
									if(ignoreChangesDelay > ignoreChangesMaxDelay)
										ignoreChanges = false;

									_thisRigidbody.AddForceAtPosition(force, wp);
								}

								if(RealisticWaterPhysics.currentDebugEnabled) // Just skip this if debug info is disabled.
								{
									forces.Add(new[] { wp, force }); // For drawing force gizmos
								}
							}
						}
					}
				}
			}
			else
			{
				if(HeavyRunTimeEdits || LightRunTimeEdits)
				{
					#if DW
					if(RealisticWaterPhysics.currentUseDW == true)
					{
						if(GetComponent<BuoyancyForce>() == false || GetComponent<BuoyancyForce>().enabled == false)
						{
							//System is disabled or not found so turn the system back on!
							BuoyancyEnabled = true;
						}
					}
					#endif
				}
			}

			BuoyancyActive = BuoyancyEnabled;
		}
	}

	private void FixedUpdate()
	{
		if(RealisticWaterPhysics.currentWaterDensity != waterDensity)
		{
			reSetup(); //this removes the to fast Start(), else if will do setup on start and then again here.
		}

		if(_thisRigidbody == null) // it sometimes fails to get this for some reason...
		{
			reSetup(); //this removes the to fast Start(), else if will do setup on start and then again here.
		}

		CalculatePhysics();
	}

	private void OnDrawGizmosSelected()
	{
		if(RealisticWaterPhysics.currentDebugEnabled) // Just skip this if debug info is disabled.
		{
			const float gizmoSize = 0.05f;

			if (voxels == null || forces == null)
			{
				return;
			}


			Gizmos.color = Color.green;
			Gizmos.DrawCube(transform.TransformPoint(_thisRigidbody.centerOfMass), new Vector3(gizmoSize, gizmoSize, gizmoSize));

			Gizmos.color = Color.yellow;

			for (int i = 0; i < voxels.Count; i++)
			{
				Gizmos.DrawCube(transform.TransformPoint(voxels[i]), new Vector3(gizmoSize, gizmoSize, gizmoSize));
			}

			Gizmos.color = Color.cyan;

			for (int i = 0; i < forces.Count; i++) 
			{
				Gizmos.DrawCube(forces[i][0], new Vector3(gizmoSize, gizmoSize, gizmoSize));
				Gizmos.DrawLine(forces[i][0], forces[i][0] + forces[i][1] / GetComponent<Rigidbody>().mass);
			}

		}
	}
}