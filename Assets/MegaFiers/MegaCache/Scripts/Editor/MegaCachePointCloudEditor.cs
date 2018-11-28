
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if false
	public MegaCachePCImage		image;
	public int					framenum		= 0;
	public float				time			= 0.0f;
	public int					maxparticles	= 1000;
	public float				fps				= 25.0f;
	public float				speed			= 1.0f;
	public MegaCacheRepeatMode	loopmode		= MegaCacheRepeatMode.Loop;
	public float				importscale		= 1.0f;
	public bool					animate			= false;
	public string				lastpath		= "";
	public float				scaleall		= 1.0f;
	public float				sizescale		= 1.0f;
	public ParticleSystem		particle;
	ParticleSystem.Particle[]	particles;

#endif

[CustomEditor(typeof(MegaCachePointCloud))]
public class MegaCachePointCloudEditor : Editor
{
	SerializedProperty _prop_framenum;
	SerializedProperty _prop_time;
	SerializedProperty _prop_fps;
	SerializedProperty _prop_loopmode;
	SerializedProperty _prop_speed;
	SerializedProperty _prop_importscale;
	SerializedProperty _prop_animate;
	SerializedProperty _prop_sizescale;
	SerializedProperty _prop_particle;

	SerializedProperty _prop_firstframe;
	SerializedProperty _prop_lastframe;
	SerializedProperty _prop_skip;
	SerializedProperty _prop_yup;

	[MenuItem("GameObject/Create Other/MegaCache/Point Cloud")]
	static void CreatePointCloud()
	{
		Vector3 pos = Vector3.zero;
		if ( UnityEditor.SceneView.lastActiveSceneView != null )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("Mega Cache Point Cloud");

		go.AddComponent<MegaCachePointCloud>();
		go.transform.position = pos;
		Selection.activeObject = go;
	}

	private void OnEnable()
	{
		_prop_framenum = serializedObject.FindProperty("framenum");
		_prop_time = serializedObject.FindProperty("time");
		_prop_fps = serializedObject.FindProperty("fps");
		_prop_loopmode = serializedObject.FindProperty("loopmode");
		_prop_animate = serializedObject.FindProperty("animate");
		_prop_particle = serializedObject.FindProperty("particle");
		_prop_speed = serializedObject.FindProperty("speed");
		_prop_importscale = serializedObject.FindProperty("importscale");
		_prop_sizescale = serializedObject.FindProperty("sizescale");
		_prop_firstframe = serializedObject.FindProperty("firstframe");
		_prop_lastframe = serializedObject.FindProperty("lastframe");
		_prop_skip = serializedObject.FindProperty("skip");
		_prop_yup = serializedObject.FindProperty("yupimport");
	}

	public override void OnInspectorGUI()
	{
		MegaCachePointCloud mod = (MegaCachePointCloud)target;

		serializedObject.Update();

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018
		EditorGUIUtility.LookLikeControls();
#endif

		EditorGUILayout.PropertyField(_prop_particle, new GUIContent("Particle System"));
		EditorGUILayout.PropertyField(_prop_importscale, new GUIContent("Import Scale"));
		EditorGUILayout.PropertyField(_prop_sizescale, new GUIContent("Size Scale"));

		mod.playscale = EditorGUILayout.FloatField("Play Scale", mod.playscale);
		mod.playsize = EditorGUILayout.FloatField("Play Size", mod.playsize);
		//mod.color = EditorGUILayout.ColorField("Color", mod.color);

		EditorGUILayout.PropertyField(_prop_framenum, new GUIContent("Frame Num"));
		EditorGUILayout.PropertyField(_prop_time, new GUIContent("Time"));
		EditorGUILayout.PropertyField(_prop_animate, new GUIContent("Animate"));
		EditorGUILayout.PropertyField(_prop_fps, new GUIContent("Fps"));
		EditorGUILayout.PropertyField(_prop_speed, new GUIContent("Speed"));
		EditorGUILayout.PropertyField(_prop_loopmode, new GUIContent("Loop Mode"));


		EditorGUILayout.BeginVertical("box");
		mod.showdataimport = EditorGUILayout.Foldout(mod.showdataimport, "Data Import");

		if ( mod.showdataimport )
		{
			EditorGUILayout.PropertyField(_prop_firstframe, new GUIContent("First"));
			EditorGUILayout.PropertyField(_prop_lastframe, new GUIContent("Last"));
			EditorGUILayout.PropertyField(_prop_skip, new GUIContent("Skip"));
			EditorGUILayout.PropertyField(_prop_yup, new GUIContent("Y Up Change"));

			//int val = 0;
			//mod.decformat = EditorGUILayout.IntSlider("Format name" + val.ToString("D" + mod.decformat) + ".csv", mod.decformat, 1, 6);
			//mod.namesplit = EditorGUILayout.TextField("Name Split Char", mod.namesplit);

			if ( GUILayout.Button("Load Frames") )
			{
				string file = EditorUtility.OpenFilePanel("Point Cloud CSV File", mod.lastpath, "csv");

				if ( file != null && file.Length > 1 )
				{
					mod.lastpath = file;
					LoadPC(mod, file, mod.firstframe, mod.lastframe, mod.skip);
				}
			}
		}

		EditorGUILayout.EndVertical();

		string infstring = "";
		if ( mod.image )
		{
			infstring = "Current Memory: " + 0 + "KB";
			infstring += "\nMax Points: " + mod.image.maxpoints;
		}

		EditorGUILayout.HelpBox(infstring, MessageType.None);

		if ( GUI.changed )
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}

	public void LoadPC(MegaCachePointCloud mod, string filename, int first, int last, int step)
	{
		if ( mod.image == null )
		{
			mod.image = ScriptableObject.CreateInstance<MegaCachePCImage>();
		}

		if ( mod.image && mod.image.frames.Count > 0 )
		{
			if ( !EditorUtility.DisplayDialog("Add to or Replace", "Add new Frames to existing list, or Replace All", "Add", "Replace") )
			{
				mod.image.frames.Clear();
				mod.image.maxpoints = 0;
			}
		}

		if ( step < 1 )
			step = 1;

		for ( int i = first; i <= last; i += step )
		{
			float a = (float)(i + 1 - first) / (last - first);
			if ( !EditorUtility.DisplayCancelableProgressBar("Loading Clouds", "Frame " + i, a) )
			{
				MegaCachePCFrame fr = mod.LoadFrame(filename, i);
				if ( fr != null )
				{
					mod.image.frames.Add(fr);

					if ( fr.points.Length > mod.image.maxpoints )
					{
						mod.image.maxpoints = fr.points.Length;
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Can't Load File", "Could not load frame " + i + " of sequence! Import Stopped.", "OK");
					break;
				}
			}
			else
				break;
		}

		EditorUtility.ClearProgressBar();
	}
}