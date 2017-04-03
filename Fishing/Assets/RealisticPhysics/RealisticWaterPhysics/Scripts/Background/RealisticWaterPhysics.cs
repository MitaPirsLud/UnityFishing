using UnityEngine;
using System.Collections;

public static class RealisticWaterPhysics 
{
	/// This is a Static inbetween link, other script get info from here.
	/// Best not to change anything here.
	/// 
	static float waterLevel;
	static bool ExternalWater;
	static float waterDensity = 0f;
	static float waterTemp;

	static bool useAquas;
	static bool useCeto;
	static bool useDW;
	static bool usePW;

	static float airLevel;
	static float airDensity = 0f;
	static float airTemp;

	static bool DebugInfoEnabled = false;

	public static void Setup(float _waterDensity, float _waterLevel, bool _ExternalWater, bool _useCeto, bool _useDW, bool _useAquas, bool _usePW, GasesMaterialList _SelectedGasesMaterial, float _airLevel, float _waterTemp, float _airTemp)
	{

		waterDensity = _waterDensity;
		waterLevel = _waterLevel;
		ExternalWater = _ExternalWater;
		airLevel = _airLevel;
		airDensity = PhysicsMaterialsList.getGasesMaterialValue(_SelectedGasesMaterial);
		airTemp = _airTemp;
		waterTemp = _waterTemp;
		//Support
		useCeto = _useCeto;
		useDW = _useDW;
		useAquas = _useAquas;
		usePW = _usePW;
	}

	public static bool currentDebugEnabled
	{
		get
		{
			return DebugInfoEnabled;
		}	
		set
		{
			DebugInfoEnabled = value;
		}
	}

	public static float currentWaterDensity
	{
		get
		{
			return waterDensity;
		}	
//		set
//		{
//			
//		}
	}

	public static bool currentUsePlayWater
	{
		get
		{
			return usePW;
		}	
		set
		{
			usePW = value;
		}
	}

	public static bool currentUseCeto
	{
		get
		{
			return useCeto;
		}	
		set
		{
			useCeto = value;
		}
	}
	public static bool currentUseDW
	{
		get
		{
			return useDW;
		}	
		set
		{
			useDW = value;
		}
	}
	public static bool currentUseAquas
	{
		get
		{
			return useAquas;
		}	
		set
		{
			useAquas = value;
		}
	}

	public static float currentAirDensity
	{
		get
		{
			return airDensity;
		}	
//		set
//		{
//			airDensity = value;
//		}
	}

	public static float currentAirLevel
	{
		get
		{
			return airLevel;
		}	
		set
		{
			airLevel = value;
		}
	}

//	public static float currentAirTemp
//	{
//		get
//		{
//			return airTemp;
//		}	
//		set
//		{
//			airTemp = value;
//		}
//	}
		
//	public static float currentWaterTemp
//	{
//		get
//		{
//			return waterTemp;
//		}	
//		set
//		{
//			waterTemp = value;
//		}
//	}

	public static float currentWaterLevel
	{
		get
		{
			return waterLevel;
		}	
		set
		{
			waterLevel = value;
		}
	}

	public static bool ExternalWaterLevel
	{
		get
		{
			return ExternalWater;
		}	
		set
		{
			ExternalWater = value;
		}
	}
}
