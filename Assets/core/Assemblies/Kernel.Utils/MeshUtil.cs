using UnityEngine;

public class MeshUtil : MonoBehaviour
{
	public SkinnedMeshRenderer[] meshs;

	void Start()
	{
		transform.GetComponent<MeshFilter> ().sharedMesh = new Mesh ();
	}

	void FixedUpdate()
	{
		System.Collections.Generic.List<Vector3> vList = new System.Collections.Generic.List<Vector3> ();
		System.Collections.Generic.List<int> tList = new System.Collections.Generic.List<int> ();
		System.Collections.Generic.List<Vector2> uList = new System.Collections.Generic.List<Vector2> ();
		for (int i = 0; i < meshs.Length; i++) {
			vList.AddRange (meshs[i].sharedMesh.vertices);
			tList.AddRange (meshs[i].sharedMesh.triangles);
			uList.AddRange (meshs[i].sharedMesh.uv);
		}
		transform.GetComponent<MeshFilter> ().sharedMesh.vertices = vList.ToArray ();
		transform.GetComponent<MeshFilter> ().sharedMesh.triangles = tList.ToArray ();
		transform.GetComponent<MeshFilter> ().sharedMesh.uv = uList.ToArray ();
		transform.GetComponent<MeshFilter> ().sharedMesh.RecalculateNormals();
	}

}
