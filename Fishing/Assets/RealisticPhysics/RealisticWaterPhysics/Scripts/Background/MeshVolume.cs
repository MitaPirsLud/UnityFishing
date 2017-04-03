using UnityEngine;
using System.Collections;

public static class MeshVolume 
{
	//Calculate volume of the mesh of the object.
	public static float getBounds(MeshFilter meshFtr)
	{
		Mesh mesh = meshFtr.sharedMesh;
		Bounds bounds = mesh.bounds;
		return mesh.bounds.size.x * mesh.bounds.size.y * mesh.bounds.size.z;
	}
	public static float getBounds(Collider CollObj)
	{
		Bounds bounds = CollObj.bounds;
		return bounds.size.x * bounds.size.y * bounds.size.z;
	}
	public static float getVolumeByColliderBounds(Collider CollObj)
	{
		Bounds bounds = CollObj.bounds;
		return bounds.size.x * bounds.size.y * bounds.size.z;
	}
	public static float getVolume(GameObject meshObj)
	{
		float volume = VolumeOfMesh(meshObj.GetComponent<MeshFilter>().sharedMesh);
		volume *= meshObj.gameObject.transform.localScale.x * meshObj.gameObject.transform.localScale.y * meshObj.gameObject.transform.localScale.z;
		return volume;
	}
	public static float getVolume(GameObject meshObj, MeshFilter meshFtr) // To support a diferent mesh on a gameobject, currently not used!
	{
		float volume = VolumeOfMesh(meshFtr.sharedMesh);
		volume *= meshObj.gameObject.transform.localScale.x * meshObj.gameObject.transform.localScale.y * meshObj.gameObject.transform.localScale.z;
		return volume;
	}
	static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float v321 = p3.x * p2.y * p1.z;
		float v231 = p2.x * p3.y * p1.z;
		float v312 = p3.x * p1.y * p2.z;
		float v132 = p1.x * p3.y * p2.z;
		float v213 = p2.x * p1.y * p3.z;
		float v123 = p1.x * p2.y * p3.z;
		return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
	}
	static float VolumeOfMesh(Mesh mesh)
	{
		float volume = 0;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		for (int i = 0; i < mesh.triangles.Length; i += 3)
		{
			Vector3 p1 = vertices[triangles[i + 0]];
			Vector3 p2 = vertices[triangles[i + 1]];
			Vector3 p3 = vertices[triangles[i + 2]];
			volume += SignedVolumeOfTriangle(p1, p2, p3);
		}
		return Mathf.Abs(volume);
	}
}
