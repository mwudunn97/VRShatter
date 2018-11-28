
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MegaCachePCXYZFrame
{
	public Vector3[]	points;
	public Color32[]	color;
}

[System.Serializable]
public class MegaCachePCXYZImage : ScriptableObject
{
	public int maxpoints = 0;
	public List<MegaCachePCXYZFrame> frames = new List<MegaCachePCXYZFrame>();
}