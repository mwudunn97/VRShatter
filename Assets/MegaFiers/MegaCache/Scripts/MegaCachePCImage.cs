
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MegaCachePCFrame
{
	public Vector3[] points;
	public float[] intensity;
}

[System.Serializable]
public class MegaCachePCImage : ScriptableObject
{
	public int maxpoints = 0;
	public List<MegaCachePCFrame> frames = new List<MegaCachePCFrame>();
}
