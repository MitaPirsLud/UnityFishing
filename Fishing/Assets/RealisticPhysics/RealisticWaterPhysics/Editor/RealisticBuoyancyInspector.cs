using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RealisticBuoyancy))]
public class LevelScriptEditor : Editor 
{
	Texture textureLogo;

	public override void OnInspectorGUI()
	{
		if(textureLogo == null)
		{ 
			textureLogo = (Texture2D)EditorGUIUtility.Load("Assets/RealisticPhysics/RealisticWaterPhysics/Resources/icon.png");
		}
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(textureLogo);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		RealisticBuoyancy realisticBuoyancy = (RealisticBuoyancy)target;

		realisticBuoyancy.BuoyancyEnabled = EditorGUILayout.Toggle("Buoyancy enabled", realisticBuoyancy.BuoyancyEnabled);

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Run time modifications:");
		realisticBuoyancy.LightRunTimeEdits = EditorGUILayout.Toggle("Light edits", realisticBuoyancy.LightRunTimeEdits);
		realisticBuoyancy.HeavyRunTimeEdits = EditorGUILayout.Toggle("Heavy edits", realisticBuoyancy.HeavyRunTimeEdits);

		if(realisticBuoyancy.LightRunTimeEdits || realisticBuoyancy.HeavyRunTimeEdits)
		{
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Objects properies:");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Percentage solid: ");
			realisticBuoyancy.percentageSolid = EditorGUILayout.IntSlider(realisticBuoyancy.percentageSolid,0,100);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.LabelField("Density: ",realisticBuoyancy.CalculatedDensity);
			EditorGUILayout.LabelField("Mass: ",realisticBuoyancy.CalculatedMass);
			EditorGUILayout.LabelField("Volume: ",realisticBuoyancy.CalculatedVolume);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Material type: ",realisticBuoyancy.infoMaterialType);
			EditorGUILayout.LabelField("Material sub type: ",realisticBuoyancy.infoSubType);
		}
	}
}
