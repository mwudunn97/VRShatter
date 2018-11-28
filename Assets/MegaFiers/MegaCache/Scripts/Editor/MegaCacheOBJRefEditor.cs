using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(MegaCacheOBJRef))]
public class MegaCacheOBJRefEditor : Editor
{
	SerializedProperty _prop_time;
	SerializedProperty _prop_fps;
	SerializedProperty _prop_speed;
	SerializedProperty _prop_loopmode;
	SerializedProperty _prop_frame;
	SerializedProperty _prop_updatecollider;


	[MenuItem("GameObject/Create Other/MegaCache/OBJ Ref")]
	static void CreateOBJRef()
	{
		Vector3 pos = Vector3.zero;
		if ( UnityEditor.SceneView.lastActiveSceneView != null )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("Mega Cache ObjRef");

		go.AddComponent<MegaCacheOBJRef>();
		go.transform.position = pos;
		Selection.activeObject = go;
	}

	private void OnEnable()
	{
		_prop_time		= serializedObject.FindProperty("time");
		_prop_fps		= serializedObject.FindProperty("fps");
		_prop_speed		= serializedObject.FindProperty("speed");
		_prop_loopmode	= serializedObject.FindProperty("loopmode");
		_prop_frame		= serializedObject.FindProperty("frame");
		_prop_updatecollider = serializedObject.FindProperty("updatecollider");
	}

	public override void OnInspectorGUI()
	{
		MegaCacheOBJRef mod = (MegaCacheOBJRef)target;

		serializedObject.Update();

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018
		EditorGUIUtility.LookLikeControls();
#endif

		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.PropertyField(_prop_updatecollider, new GUIContent("Update Collider"));
		MegaCacheOBJ src = (MegaCacheOBJ)EditorGUILayout.ObjectField("Source", mod.source, typeof(MegaCacheOBJ), true);

		if ( src != mod.source )
		{
			mod.SetSource(src);
			EditorUtility.SetDirty(target);
		}

		int fc = mod.GetFrames();

		if ( fc > 0 )
			EditorGUILayout.IntSlider(_prop_frame, 0, fc);

		EditorGUILayout.BeginVertical("box");

		mod.animate = EditorGUILayout.BeginToggleGroup("Animate", mod.animate);
		EditorGUILayout.PropertyField(_prop_time, new GUIContent("Time"));
		EditorGUILayout.PropertyField(_prop_fps, new GUIContent("Fps"));
		EditorGUILayout.PropertyField(_prop_speed, new GUIContent("Speed"));
		EditorGUILayout.PropertyField(_prop_loopmode, new GUIContent("Loop Mode"));

		EditorGUILayout.EndToggleGroup();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();

		if ( GUI.changed )
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}
}