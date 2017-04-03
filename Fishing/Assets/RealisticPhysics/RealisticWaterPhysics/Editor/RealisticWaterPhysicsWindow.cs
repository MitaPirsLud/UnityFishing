using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum FloatingQuality
{
	ExtremeLow,
	Low,
	Medium,
	High,
	ExtremeHigh,
	Ultra
}

public class RealisticWaterPhysicsWindow : EditorWindow 
{

	//For supporting other systems.
	#if CETO
	private bool currentUseCeto = true;
	#else
	private bool currentUseCeto = false;
	#endif

	#if DW
	private bool currentUseDW = true;
	#else
	private bool currentUseDW = false;
	#endif

	#if AQUAS
	private bool currentUseAquas = true;
	#else
	private bool currentUseAquas = false;
	#endif

	#if PW
	private bool currentUsePlayWayWater = true;
	#else
	private bool currentUsePlayWayWater = false;
	#endif

	private GameObject RealisticWaterPhysicsManager;

	private bool OverWriteDisableWindow = false; // On a Error that breaks the interface.
	private static Texture textureLogo;
	private Vector2 scrollPos = new Vector2(0,0);
	private bool ShowAdvancedInfo = false;
	//show or hide setup
	private bool showSetupOptions = true;

	private float selectedMaterialValue;
	private LiquidsMaterialList selectedOceanLiquidsMaterial = LiquidsMaterialList.WaterFresh;
	private GasesMaterialList selectedAirGasesMaterial = GasesMaterialList.Air;
	private MaterialTypes SelectedMaterialType = MaterialTypes.Solid;
	private GasesMaterialList selectedGasesMaterial = GasesMaterialList.Air;
	private LiquidsMaterialList selectedLiquidsMaterial = LiquidsMaterialList.WaterFresh;
	private SolidsMaterialList selectedSolidsMaterial = SolidsMaterialList.Oak;

	private float waterLevel;
	private GameObject waterObject;
	private GameObject AquaswaterObject; // Makes the menu easyer ;)

	private bool useExternalWaterLevel; //This will allow users to use somthing other then a value or a gameobject as water.

	private bool useAirDensity = true;
	private float airDensity = 1.225f;
	private float maxAirLevel = 100f;
	//
	//Advanced settings // Work in progress!
	private MeshFilter SelectedMesh;

	//show or hide gameobject setup.
	private bool showGameobjectSetupOptions = false;
	private bool existingObjectsManager = false;

	//Options for gameobject.
	private FloatingQuality selectedFloatingQuality = FloatingQuality.Low;
	private int slicesPerAxis = 2; // 1-8
	private bool isConcave = false;
	private int voxelsLimit = 16;

	private int percentageSolid = 100; // 0 -100

	//Check changes
	private int OldslicesPerAxis;
	private bool OldisConcave;
	private int OldvoxelsLimit;

	private int OldpercentageSolid;
	private GameObject OldSelectedObject;

	private GameObject SelectedObject;

	//Experimental importing of gameobjects. Multi Select, but works odd. 
	//Will not be used in this version.

	List<GameObject> selectionGameObjects = new List<GameObject>(); //Selected in the editor.

	//Some checks...
	private bool didImport = false;
	private bool DidTest = false;
	private bool ScriptSavedAfterTest = false;
	private bool firstStart = false;

	//Log messages for the user.
	private string lastErrorLog = "";
	private string stateInfo = "";
	private string testResultsString = "";
		
	//Calculated test results.
	private float CalculatedWaterDensity;
	private float CalculatedAirDensity;
	private float CalculatedDensity;
	private float CalculatedMass;
	private float CalculatedVolume;

	private bool CalculatedWillFloatInWater = false;
	private bool CalculatedWillFloatInAir = false;

	//OnScript update
	private bool LightRunTimeEdits;
	private bool HeavyRunTimeEdits;

	//Menus
	public int toolbarInt = 0;
	public string[] toolbarStrings = new string[] {"Setup", "Setup gameObjects", "Manage gameObjects"};
	int OldtoolbarInt = 0; // To stop rapit updats



	void OnDestroy() 
	{ //Clean up after yourself!
		searchFilterResetDone = false;
		resetSearchFilter();
		hideManagerGameObject();
	}

	[MenuItem("Window/RealisticPhysics/Realistic Water Physics/Manager window")]
	static void Init()
	{
		//Added support for other assets.
		#if RWP
		//Check if Tag RWP (RealisticWaterPhysics) is added or not, we use it for other things to ;)
		#else
		string newSDS = "RWP;" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newSDS);
		#endif


		RealisticWaterPhysicsWindow RealisticWaterPhysicsWindow = (RealisticWaterPhysicsWindow)EditorWindow.GetWindow (typeof (RealisticWaterPhysicsWindow));
		RealisticWaterPhysicsWindow.maxSize = new Vector2(450, 400);
		textureLogo = (Texture2D)EditorGUIUtility.Load("Assets/RealisticPhysics/RealisticWaterPhysics/Resources/icon0.png");
	}


	bool searchFilterResetDone = true; // So it wont trigger 10/s!
	void resetSearchFilter()
	{
		if(searchFilterResetDone == false) // its false so we need to reset it!
		{
			RealisticComponentSearch.SetSearchFilter("", 0); // Clear the search!
			searchFilterResetDone = true;
		}
	}

	void RememberOlds()
	{
		OldslicesPerAxis = slicesPerAxis;
		OldisConcave = isConcave;
		OldvoxelsLimit = voxelsLimit;
		//OldmaterialType = materialType;
		OldpercentageSolid = percentageSolid;
	}

	void checkForChanges()
	{
		if(OldslicesPerAxis != slicesPerAxis ||
			OldisConcave != isConcave ||
			OldvoxelsLimit != voxelsLimit ||
			//OldmaterialType != materialType ||
			OldpercentageSolid != percentageSolid)
		{
			clearTestResults();
		}
	}

	void clearTestResults()
	{
		DidTest = false;
		ScriptSavedAfterTest = false;
		CalculatedWaterDensity = 0;
		CalculatedAirDensity = 0;
		CalculatedDensity = 0;
		CalculatedMass = 0;
		CalculatedVolume = 0;

		CalculatedWillFloatInWater = false;
		CalculatedWillFloatInAir = false;
	}

	void importSettingsFromExistingManager()
	{
		RealisticWaterPhysicsManager = null;
		RealisticWaterPhysicsManager = GameObject.Find("RealisticWaterPhysicsManager") as GameObject;

		if(RealisticWaterPhysicsManager != null)
		{
			if(RealisticWaterPhysicsManager.GetComponent<RealisticWaterPhysicsManager>() != null)
			{
				RealisticWaterPhysicsManager RWPM = RealisticWaterPhysicsManager.GetComponent<RealisticWaterPhysicsManager>();
				selectedOceanLiquidsMaterial = RWPM.selectedOceanLiquidsMaterial;
				useExternalWaterLevel = RWPM.useExternalWaterLevel;
				waterObject = RWPM.waterLevelObject;
				useAirDensity = RWPM.useAirDensity;
				selectedGasesMaterial = RWPM.selectedGasesMaterial;
				maxAirLevel = RWPM.maxAirLevel;
				waterLevel = RWPM.waterLevel;
				currentUseCeto = RWPM.useCeto;
				currentUseAquas = RWPM.useAquas;
				currentUseDW = RWPM.useDW;
				currentUsePlayWayWater = RWPM.usePW;
			}
			else
			{
				Debug.LogError("RealisticWaterPhysicsManager is missing its RealisticWaterPhysicsManager script!");
				lastErrorLog = "RealisticWaterPhysicsManager is missing its RealisticWaterPhysicsManager script!";
				//Disable All controlls!
				OverWriteDisableWindow = true;
			}
		}
	}

	void importSettingsFromSelectedGameObject()
	{
		clearTestResults();

		if(SelectedObject.GetComponent<RealisticBuoyancy>() == true)
		{
			RealisticBuoyancy BuoyancyScript = SelectedObject.GetComponent<RealisticBuoyancy>();

			slicesPerAxis = BuoyancyScript.slicesPerAxis;
			isConcave = BuoyancyScript.isConcave;
			voxelsLimit = BuoyancyScript.voxelsLimit;
			percentageSolid = BuoyancyScript.percentageSolid;
			SelectedMaterialType = BuoyancyScript.SelectedMaterialType;
			selectedGasesMaterial = BuoyancyScript.selectedGasesMaterial;
			selectedLiquidsMaterial = BuoyancyScript.selectedLiquidsMaterial;
			selectedSolidsMaterial = BuoyancyScript.selectedSolidsMaterial;

			if(BuoyancyScript.SelectedMaterialType != MaterialTypes.Solid)
			{
				ShowAdvancedInfo = true;
			}
			
			if(slicesPerAxis == 1)
			{
				selectedFloatingQuality = FloatingQuality.ExtremeLow;
			}
			if(slicesPerAxis == 2)
			{
				selectedFloatingQuality = FloatingQuality.Low;
			}
			if(slicesPerAxis == 4)
			{
				selectedFloatingQuality = FloatingQuality.Medium;
			}

			if(slicesPerAxis == 6)
			{
				selectedFloatingQuality = FloatingQuality.High;
			}

			if(slicesPerAxis == 8)
			{
				selectedFloatingQuality = FloatingQuality.ExtremeHigh;
			}
		}
		else
		{
			//Cant find settings to import.
			//Nothing wrong with that!
		}
	}
	void createOrUpdateManager()
	{
		RealisticWaterPhysicsManager = null;
		if(GameObject.Find("RealisticWaterPhysicsManager") == false)
		{
			RealisticWaterPhysicsManager = new GameObject();

			RealisticWaterPhysicsManager.name = "RealisticWaterPhysicsManager";
			RealisticWaterPhysicsManager.AddComponent<RealisticWaterPhysicsManager>();

			hideManagerGameObject();

			lastErrorLog = "Can't find the RealisticWaterPhysicsManager, one have been created!";
		}
		else 
		{
			RealisticWaterPhysicsManager = null;
			RealisticWaterPhysicsManager = GameObject.Find("RealisticWaterPhysicsManager") as GameObject;

			if(RealisticWaterPhysicsManager.GetComponent<RealisticWaterPhysicsManager>() != null) //Just a extra check
			{
				RealisticWaterPhysicsManager RWPM = RealisticWaterPhysicsManager.GetComponent<RealisticWaterPhysicsManager>();
				RWPM.selectedOceanLiquidsMaterial = selectedOceanLiquidsMaterial;
				RWPM.useExternalWaterLevel = useExternalWaterLevel;
				RWPM.waterLevel = waterLevel;
				RWPM.waterLevelObject = waterObject;
				RWPM.useAirDensity = useAirDensity;
				RWPM.selectedGasesMaterial = selectedAirGasesMaterial;
				RWPM.maxAirLevel = maxAirLevel;

				RWPM.useCeto = currentUseCeto;
				RWPM.useDW = currentUseDW;
				RWPM.useAquas = currentUseAquas;
				RWPM.usePW = currentUsePlayWayWater;
			}
			else
			{
				Debug.LogError("RealisticWaterPhysicsManager is missing its RealisticWaterPhysicsManager script!");
				lastErrorLog = "RealisticWaterPhysicsManager is missing its RealisticWaterPhysicsManager script!";
				//Disable All controlls!
				OverWriteDisableWindow = true;
			}
		}

	}

	void showManagerGameObject()
	{
		//as its hidden by default, going to advanced options will show it again.
		RealisticWaterPhysicsManager = GameObject.Find("RealisticWaterPhysicsManager") as GameObject;
		if(RealisticWaterPhysicsManager.GetComponent<RealisticWaterPhysicsManager>() != null) //Just a extra check
		{ // just to be sure ;)
			RealisticWaterPhysicsManager.hideFlags =0;
		}
	}

	void hideManagerGameObject()
	{
		if(RealisticWaterPhysicsManager != null)
		{
			RealisticWaterPhysicsManager.hideFlags = HideFlags.HideInHierarchy;
		}
	}

	void createObjectScript(GameObject _SelectedObj) // create or update the scripts.
	{
		GameObject SelectedObj = _SelectedObj; //To fixs editor not knowing what SelectedObj is :/

		//Check if it got a collider, it must have one!
		if(SelectedObj.GetComponent<Collider>() == false)
		{
			SelectedObj.AddComponent<MeshCollider>();
			SelectedObj.GetComponent<MeshCollider>().convex = true;
			lastErrorLog = "Can't find a Collider, one have been created!";
		}

		//First check if not already contains a script
		if(SelectedObj.GetComponent<RealisticBuoyancy>() == true)
		{
			//Remember some states!
			LightRunTimeEdits = SelectedObj.GetComponent<RealisticBuoyancy>().LightRunTimeEdits;
			HeavyRunTimeEdits = SelectedObj.GetComponent<RealisticBuoyancy>().HeavyRunTimeEdits;

			addOrUpdateFloatingComponent(SelectedObj);
		}
		else
		{
			SelectedObj.AddComponent<RealisticBuoyancy>();
			addOrUpdateFloatingComponent(SelectedObj);
		}


		ScriptSavedAfterTest = true;
	}
		
	void addOrUpdateFloatingComponent(GameObject SelectedObj)
	{
		RealisticBuoyancy addedBuoyancyScript = SelectedObj.GetComponent<RealisticBuoyancy>();
		addedBuoyancyScript.slicesPerAxis = slicesPerAxis;
		addedBuoyancyScript.isConcave = isConcave;
		addedBuoyancyScript.voxelsLimit = voxelsLimit;
		addedBuoyancyScript.percentageSolid = percentageSolid;
		addedBuoyancyScript.SelectedMaterialType = SelectedMaterialType;
		addedBuoyancyScript.selectedGasesMaterial = selectedGasesMaterial;
		addedBuoyancyScript.selectedLiquidsMaterial = selectedLiquidsMaterial;
		addedBuoyancyScript.selectedSolidsMaterial = selectedSolidsMaterial;
		addedBuoyancyScript.LightRunTimeEdits = LightRunTimeEdits;
		addedBuoyancyScript.HeavyRunTimeEdits = HeavyRunTimeEdits;
		addedBuoyancyScript.reSetup();
		Debug.Log("Created / Updated script");
	}

	public void OnInspectorUpdate()
	{
		// This will only get called 10 times per second.
		//To make sure the window will stay up to date!
		Repaint();
	}

	void NoneGUIElementsUpdate()
	{
		selectionGameObjects = new List<GameObject>(Selection.gameObjects);
		//Its not so nice but will shot the window search blocking Light window search.
		if(OldtoolbarInt != toolbarInt)
		{
			//User pressed as button!
			OldtoolbarInt = toolbarInt;
			//To keep the OnGUI() cleaner!
			if(toolbarInt == 0)
			{
				showSetupOptions = true;
				showGameobjectSetupOptions = false;
				existingObjectsManager = false;
				searchFilterResetDone = false;
				resetSearchFilter();
			}
			else if(toolbarInt == 1)
			{
				showGameobjectSetupOptions = true;
				showSetupOptions = false;
				existingObjectsManager = false;
				searchFilterResetDone = false;
				resetSearchFilter();
			}
			else if (toolbarInt == 2)
			{
				showGameobjectSetupOptions = true;
				showSetupOptions = false;
				existingObjectsManager = true;
				searchFilterResetDone = false;
				resetSearchFilter();
				RealisticComponentSearch.SetSearchFilter("RealisticBuoyancy", 2);
			}
		}

		if(SelectedMaterialType == MaterialTypes.Solid)
		{
			selectedMaterialValue = PhysicsMaterialsList.getSolidMaterialValue( selectedSolidsMaterial);
		}
		else if(SelectedMaterialType == MaterialTypes.Liquids)
		{
			selectedMaterialValue = PhysicsMaterialsList.getLiquidsMaterialValue( selectedLiquidsMaterial);
		}
		else
		{
			selectedMaterialValue = PhysicsMaterialsList.getGasesMaterialValue( selectedGasesMaterial);
		}


		checkForChanges(); //Check to see if user changed anything, exept the gameobject thats a check of its own type.
		RememberOlds(); // Remember the options for now!

		createOrUpdateManager(); //Create or update the manager.

		CalculatedWaterDensity = PhysicsMaterialsList.getLiquidsMaterialValue(selectedOceanLiquidsMaterial);
		//CalculatedAirDensity = CalculateStuff(airTempeture, airDensity);
		CalculatedAirDensity = airDensity; // Work in progress!
	}

	void OnGUI () 
	{		
		//To fix the crapy static stuff and missing Start void :/
		if(firstStart == false)
		{
			firstStart = true;
			importSettingsFromExistingManager();

			if(SelectedObject != null)
			{
				importSettingsFromSelectedGameObject();
			}
		}

		//To keep everything updated!
		NoneGUIElementsUpdate();

		if(OverWriteDisableWindow == false) // If this value = true there is something wrong!
		{
			if(ShowAdvancedInfo == true)
			{
				if(GUILayout.Button("Hide advanced options/info"))
				{
					ShowAdvancedInfo = false;;
					hideManagerGameObject();
				}
			}
			else
			{
				if(GUILayout.Button("Show advanced options/info"))
				{
					ShowAdvancedInfo = true;
					showManagerGameObject();
				}
			}

			if(textureLogo == null)
			{ 
				textureLogo = (Texture2D)EditorGUIUtility.Load("Assets/RealisticPhysics/RealisticWaterPhysics/Resources/icon0.png");
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(textureLogo);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

			EditorGUILayout.BeginHorizontal();
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width (450), GUILayout.Height (300));
		}

		EditorGUILayout.HelpBox("Waring log: " + lastErrorLog, MessageType.Info, true); //Desplay the last error log, warning!

		if(OverWriteDisableWindow == false) // If this value = true there is something wrong!
		{
			EditorGUILayout.Space();
			if(showSetupOptions == true)
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Water settings:");
				selectedOceanLiquidsMaterial = (LiquidsMaterialList) EditorGUILayout.EnumPopup("Water Classification:", selectedOceanLiquidsMaterial);

				if(ShowAdvancedInfo)
				{
					EditorGUILayout.LabelField("Density of the water:" + CalculatedWaterDensity);
				}
				EditorGUILayout.Space();

				if(waterObject != null)
				{
					waterLevel = waterObject.transform.position.y;
				}

				if(currentUseCeto == false && currentUseDW == false  && currentUseAquas == false  && currentUsePlayWayWater == false )
				{
					if(waterObject == null)
					{
						waterLevel = EditorGUILayout.FloatField("Water Level:", waterLevel);
					}
					if(ShowAdvancedInfo)
					{
						if(waterObject == null)
						{
							EditorGUILayout.LabelField("Or");
						}
						if(waterObject = EditorGUILayout.ObjectField("Water GameObject: ", waterObject, typeof(GameObject), true)as GameObject)
						{
							waterLevel = waterObject.transform.position.y;
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("");
							EditorGUILayout.LabelField("Water level: "+waterLevel.ToString());
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.LabelField("Or");
						useExternalWaterLevel = EditorGUILayout.Toggle("Use external water level", useExternalWaterLevel); // Allows you to make something yourself ;)
					}
				}	

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Air settings:");
				useAirDensity = EditorGUILayout.Toggle("Use air density", useAirDensity);
				if(useAirDensity == true)
				{
					maxAirLevel = EditorGUILayout.FloatField("Max air Level:", maxAirLevel);
					if(ShowAdvancedInfo)
					{

						selectedAirGasesMaterial = (GasesMaterialList) EditorGUILayout.EnumPopup("Air type: ", selectedAirGasesMaterial);
						airDensity = PhysicsMaterialsList.getGasesMaterialValue(selectedAirGasesMaterial);
						EditorGUILayout.LabelField("Density of the air:" + CalculatedAirDensity); // work in progress!
					}
				}

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Support settings:");

				//Again for support
				#if CETO
				currentUseCeto = EditorGUILayout.Toggle("Enable Ceto support", currentUseCeto);
				#endif

				#if DW
				currentUseDW = EditorGUILayout.Toggle("Enable Dynamic Water support", currentUseDW);
				#endif

				#if AQUAS
				currentUseAquas = EditorGUILayout.Toggle("Enable Aquas support", currentUseAquas);
				#endif
				//Aquas needs a water level to work with, so leave that at them, just let the users select the aqaus water object
				if(currentUseAquas == true)
				{
					if(AquaswaterObject = EditorGUILayout.ObjectField("AQUAS water object: ", AquaswaterObject, typeof(GameObject), true)as GameObject)
					{
						waterLevel = AquaswaterObject.transform.position.y;
					}
					if(AquaswaterObject != null)
					{
						waterLevel = AquaswaterObject.transform.position.y;
					}
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("");
					EditorGUILayout.LabelField("Water level: "+waterLevel.ToString());
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space();
				}


				#if PW
				currentUsePlayWayWater = EditorGUILayout.Toggle("Enable PlayWay Water support", currentUsePlayWayWater);
				#endif

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Read the Manual for more info.");
				//get some room!
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}

			if(showGameobjectSetupOptions == true)
			{
				if(selectionGameObjects.Count > 0)
				{

					EditorGUILayout.LabelField("All Gameobjects below will get the same settings!");
					SelectedObject = selectionGameObjects[0];

					for (int i = 0; i < selectionGameObjects.Count; i++) 
					{
						selectionGameObjects[i] = EditorGUILayout.ObjectField("GameObject " +i.ToString() + ": ", selectionGameObjects[i], typeof(GameObject), true)as GameObject;
					}
				}
				else
				{
					EditorGUILayout.LabelField("Select a gameOject in the Hierarchy");
					SelectedObject = null;
				}


				if(SelectedObject != null)
				{
					//Check if there is a mesh filter else report it to the user and clear the selection!
					if(SelectedObject.GetComponent<MeshFilter>() == true && SelectedObject.GetComponent<MeshFilter>() != null)
					{
						lastErrorLog = "";
						if(didImport == false)
						{
							importSettingsFromSelectedGameObject();
							OldSelectedObject = SelectedObject;
							didImport = true;
						}
						else if(OldSelectedObject != SelectedObject)
						{
							didImport = false;
						}
					}
					else
					{
						//So there is no Mesh filter on this object!
						//We can still use it we just got to use somthing else for volume... collider?
						if(SelectedObject.GetComponent<Collider>() == true)
						{
							lastErrorLog = "Can't find MeshFilter, using collider!";
							if(didImport == false)
							{
								importSettingsFromSelectedGameObject();
								OldSelectedObject = SelectedObject;
								didImport = true;
							}
							else if(OldSelectedObject != SelectedObject)
							{
								didImport = false;
							}
						}
						else
						{
							//Also no collider....
							lastErrorLog = "There is no MeshFilter or Collider on this object!";
						}
					}




					EditorGUILayout.Space();

					if(ShowAdvancedInfo == true)
					{
						EditorGUILayout.LabelField("Gameobject settings:");
						slicesPerAxis = EditorGUILayout.IntField("Slices Per Axis", slicesPerAxis);
						if(slicesPerAxis < 1)
							slicesPerAxis = 1;
						else if(slicesPerAxis > 8)
							slicesPerAxis = 8;
						isConcave = EditorGUILayout.Toggle("GameObject is concave", isConcave); // has its issues!
						voxelsLimit = EditorGUILayout.IntField("Voxels limit", voxelsLimit);			
						if(slicesPerAxis < 1)
							slicesPerAxis = 1;
						else if(slicesPerAxis > 64)
							slicesPerAxis = 64;
					}
					else
					{
						selectedFloatingQuality = (FloatingQuality) EditorGUILayout.EnumPopup("Floating Quality:", selectedFloatingQuality);
						if(selectedFloatingQuality == FloatingQuality.ExtremeLow)
						{
							slicesPerAxis = 1;
							voxelsLimit = 2;
						}
						if(selectedFloatingQuality == FloatingQuality.Low)
						{
							slicesPerAxis = 2;
							voxelsLimit = 4;
						}
						if(selectedFloatingQuality == FloatingQuality.Medium)
						{
							slicesPerAxis = 4;
							voxelsLimit = 8;
						}
						if(selectedFloatingQuality == FloatingQuality.High)
						{
							slicesPerAxis = 6;
							voxelsLimit = 12;
						}
						if(selectedFloatingQuality == FloatingQuality.ExtremeHigh)
						{
							slicesPerAxis = 8;
							voxelsLimit = 16;
						}
					}

					if(ShowAdvancedInfo == true)
					{
						SelectedMaterialType = (MaterialTypes) EditorGUILayout.EnumPopup("Material type:", SelectedMaterialType);
					}
					else
					{
						//Back to basic solids only!
						SelectedMaterialType = MaterialTypes.Solid;
					}
					if(SelectedMaterialType == MaterialTypes.Solid)
					{
						selectedSolidsMaterial = (SolidsMaterialList) EditorGUILayout.EnumPopup("Solid Material:", selectedSolidsMaterial);
					}
					else if(SelectedMaterialType == MaterialTypes.Liquids)
					{
						selectedLiquidsMaterial = (LiquidsMaterialList) EditorGUILayout.EnumPopup("Liquid Material:", selectedLiquidsMaterial);
					}
					else
					{
						selectedGasesMaterial = (GasesMaterialList) EditorGUILayout.EnumPopup("Gas Material:", selectedGasesMaterial);
					}


					percentageSolid = EditorGUILayout.IntField("Percentage Solid:", percentageSolid);	
					if(percentageSolid < 0)
						percentageSolid = 0;
					else if(percentageSolid > 100)
						percentageSolid = 100;
					EditorGUILayout.Space();

					if(GUILayout.Button("Calculate test results"))
					{
						calculateResults(); //Update the results.
					}

					if(DidTest == true)
					{
						if(ShowAdvancedInfo)
						{
							testResultsString = "Test result of the object:" + System.Environment.NewLine + System.Environment.NewLine +
								"Calculated mass: " + CalculatedMass + System.Environment.NewLine +
								"Calculated volume: " + CalculatedVolume + System.Environment.NewLine +
								"Calculated density: " + CalculatedDensity + System.Environment.NewLine +
								"Float in water: " + ((CalculatedWillFloatInWater == true) ? "Yes" : "No").ToString() + System.Environment.NewLine +
								"Float in Air: " + ((CalculatedWillFloatInAir == true) ? "Yes" : "No").ToString();
						}
						else
						{
							//Will it float or not?!
							testResultsString = "Test result of the object:" + System.Environment.NewLine + System.Environment.NewLine +
								"Float in water: " + ((CalculatedWillFloatInWater == true) ? "Yes" : "No").ToString() + System.Environment.NewLine +
								"Float in Air: " + ((CalculatedWillFloatInAir == true) ? "Yes" : "No").ToString();
						}
					}
					else
					{
						testResultsString = "No test results yet!";
					}

					EditorGUILayout.HelpBox(testResultsString, MessageType.Info, true);

					//Button
					if(SelectedObject != null)
					{
						if (SelectedObject.GetComponent<RealisticBuoyancy>())
						{
							if(GUILayout.Button("Update gameobject script"))
							{
								if(selectionGameObjects != null && selectionGameObjects.Count > 0)
								{
									foreach (var Selectedgameobject in selectionGameObjects) 
									{
										createObjectScript(Selectedgameobject);
									}
								}
								else
								{
									createObjectScript(SelectedObject);  // Update the script on the selected object.
								}
							}
						}
						else
						{
							if(GUILayout.Button("Create gameobject script"))
							{
								if(selectionGameObjects != null && selectionGameObjects.Count > 0)
								{
									foreach (var Selectedgameobject in selectionGameObjects) 
									{
										createObjectScript(Selectedgameobject);
									}
								}
								else
								{
									createObjectScript(SelectedObject);  // Update the script on the selected object.
								}
							}
						}
					}
					else //double check to make sure there is indeed a gamobject selected!
					{
						if(GUILayout.Button("First select a gameobject."))
						{
							//Still show the button but its does nothing :P
						}
					}
				}

				EditorGUILayout.Space();

				if(SelectedObject != null)
				{
					if(ScriptSavedAfterTest == true)
					{
						stateInfo = "Changes have been saved!";
					}
					else
					{
						stateInfo = "Changes have NOT been saved!";

					}
					EditorGUILayout.HelpBox(stateInfo, MessageType.Info, true);
				}

			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndHorizontal();
		}
	}

	void calculateResults()
	{

		//Make sure a gameobject is selected!
		if(SelectedObject != null)
		{
			bool Failed = false;
			//First calculate the density of the object.
			if(SelectedObject.GetComponent<MeshFilter>() && SelectedObject.GetComponent<MeshFilter>().sharedMesh != null)
			{ // Use meshfilter based bounds
				CalculatedVolume = MeshVolume.getVolume(SelectedObject as GameObject);
			}
			else if(SelectedObject.GetComponent<Collider>())
			{ // Use collider based bounds
				CalculatedVolume = MeshVolume.getVolumeByColliderBounds(SelectedObject.GetComponent<Collider>());
			}
			else
			{
				Debug.Log("[Realistic Water Physics] Object is missing a mesh filter or collider.");
				Failed = true;
			}

			if(!Failed) // a small fix!
			{
				//Now the percentage thats solit and the remaining is air.
				float percentageSolidMass = CalculatedVolume * ( ( selectedMaterialValue / 100 ) * percentageSolid );
				float percentageAir = CalculatedVolume * ( ( RealisticWaterPhysics.currentAirDensity / 100 ) * (100 - percentageSolid) );

				//The totaal mass of the object is the amount of solit material + the remaining amount of air.
				CalculatedMass = percentageSolidMass + percentageAir;

				//the density of the intire object is its calculated mass / its volume.
				CalculatedDensity = CalculatedMass / CalculatedVolume;

				CalculatedWaterDensity = PhysicsMaterialsList.getLiquidsMaterialValue(selectedOceanLiquidsMaterial);
				CalculatedAirDensity = airDensity;



				if(CalculatedDensity > CalculatedWaterDensity) // if denser then water density it will sink.
				{
					CalculatedWillFloatInWater = false;
				}
				else
				{
					CalculatedWillFloatInWater = true;
				}


				if(CalculatedDensity > CalculatedAirDensity) // if denser then air density it will sink
				{
					CalculatedWillFloatInAir = false;
				}
				else 
				{
					CalculatedWillFloatInAir = true;
				}
				DidTest = true;
			}
		}
	}
}
