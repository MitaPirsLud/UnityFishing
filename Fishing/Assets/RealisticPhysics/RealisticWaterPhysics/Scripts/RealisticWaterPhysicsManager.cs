using UnityEngine;
using System.Collections;
#if DW // Added support for DynamicWaterSystem
using LostPolygon.DynamicWaterSystem; 
#endif

public class RealisticWaterPhysicsManager : MonoBehaviour
{
	[HideInInspector]
	public bool useExternalWaterLevel;

	[HideInInspector]
	public float waterLevel = 0;

	[HideInInspector]
	public GameObject waterLevelObject;

	[HideInInspector]
	public bool useAirDensity = true;

	[HideInInspector]
	public LiquidsMaterialList selectedOceanLiquidsMaterial = LiquidsMaterialList.WaterFresh;

	[HideInInspector]
	public MaterialTypes SelectedMaterialType = MaterialTypes.Solid;

	[HideInInspector]
	public GasesMaterialList selectedGasesMaterial = GasesMaterialList.Air;

	[HideInInspector]
	public SolidsMaterialList selectedSolidsMaterial = SolidsMaterialList.Oak;

	[HideInInspector]
	public float maxAirLevel = 100f;

	//support
	[HideInInspector]
	public bool useCeto = false;

	[HideInInspector]
	public bool useDW = false;

	[HideInInspector]
	public bool useAquas = false;

	[HideInInspector]
	public bool usePW = false;

	#if DW
	private GameObject DynamicWaterSystemWater;
	#endif

	public static RealisticWaterPhysicsManager Instance;

	void Awake()
	{
		if (Instance)
			DestroyImmediate(gameObject);
		else
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
	}

	void Start()
	{
		#if DW
		if(RealisticWaterPhysics.currentUseDW == true)
		{
			DynamicWaterSystemWater = GameObject.FindGameObjectWithTag("DynamicWater");
			if(DynamicWaterSystemWater != null)
			{
				if(DynamicWaterSystemWater.GetComponent<DynamicWater>() == true && DynamicWaterSystemWater.GetComponent<DynamicWater>().enabled == true)
				{
					//Its limited to 10,000 by them, not me :P
					DynamicWaterSystemWater.GetComponent<DynamicWater>().Density = PhysicsMaterialsList.getLiquidsMaterialValue(selectedOceanLiquidsMaterial);
				}
			}
		}
		#endif

		float CalculatedWaterDensity = PhysicsMaterialsList.getLiquidsMaterialValue(selectedOceanLiquidsMaterial);
	
		RealisticWaterPhysics.Setup(CalculatedWaterDensity, waterLevel, useExternalWaterLevel, useCeto, useDW, useAquas, usePW, selectedGasesMaterial, maxAirLevel, 4, 4); 

		if(RealisticWaterPhysics.currentDebugEnabled) // Just skip this if debug info is disabled.
		{
			Debug.Log("Calculated Water Density:" + CalculatedWaterDensity);
		}
	}

	void FixedUpdate()
	{
		if(RealisticWaterPhysics.ExternalWaterLevel == false)
		{
			if(waterLevelObject != null)
				waterLevel = waterLevelObject.transform.position.y;
			else
				RealisticWaterPhysics.currentWaterLevel = waterLevel; //keeps the water level working in runtime.
		}

		#if DW
		if(RealisticWaterPhysics.currentUseDW == true && DynamicWaterSystemWater != null)
		{
			if(DynamicWaterSystemWater.GetComponent<DynamicWater>() == true && DynamicWaterSystemWater.GetComponent<DynamicWater>().enabled == true)
			{
				RealisticWaterPhysics.currentWaterLevel = DynamicWaterSystemWater.transform.position.y;
			}
		}
		#endif
	}
}
