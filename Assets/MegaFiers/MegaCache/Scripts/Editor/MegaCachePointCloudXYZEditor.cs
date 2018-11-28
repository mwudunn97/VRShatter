
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(MegaCachePointCloudXYZ))]
public class MegaCachePointCloudXYZEditor : Editor
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
	SerializedProperty _prop_trans;
	SerializedProperty _prop_pskip;
	SerializedProperty _prop_yup;
	//SerializedProperty _prop_streamply;
	SerializedProperty _prop_update;
	SerializedProperty _prop_color;

	[MenuItem("GameObject/Create Other/MegaCache/Point Cloud XYZ")]
	static void CreatePointCloud()
	{
		Vector3 pos = Vector3.zero;
		if ( UnityEditor.SceneView.lastActiveSceneView != null )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("Mega Cache Point Cloud XYZ");

		go.AddComponent<MegaCachePointCloudXYZ>();
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
		_prop_trans = serializedObject.FindProperty("transparency");
		_prop_pskip = serializedObject.FindProperty("particleskip");
		_prop_yup = serializedObject.FindProperty("yupimport");
		_prop_update = serializedObject.FindProperty("update");
		_prop_color = serializedObject.FindProperty("color");
	}

	public override void OnInspectorGUI()
	{
		MegaCachePointCloudXYZ mod = (MegaCachePointCloudXYZ)target;

		serializedObject.Update();

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018
		EditorGUIUtility.LookLikeControls();
#endif

		EditorGUILayout.PropertyField(_prop_particle, new GUIContent("Particle System"));
		EditorGUILayout.PropertyField(_prop_importscale, new GUIContent("Import Scale"));
		EditorGUILayout.PropertyField(_prop_sizescale, new GUIContent("Size Scale"));

		mod.playscale = EditorGUILayout.FloatField("Play Scale", mod.playscale);
		mod.playsize = EditorGUILayout.FloatField("Play Size", mod.playsize);
		EditorGUILayout.Slider(_prop_trans, 0.0f, 1.0f, "Transparency");
		//mod.color = EditorGUILayout.ColorField("Color", mod.color);

		EditorGUILayout.PropertyField(_prop_framenum, new GUIContent("Frame Num"));
		EditorGUILayout.PropertyField(_prop_time, new GUIContent("Time"));
		EditorGUILayout.PropertyField(_prop_animate, new GUIContent("Animate"));
		EditorGUILayout.PropertyField(_prop_fps, new GUIContent("Fps"));
		EditorGUILayout.PropertyField(_prop_speed, new GUIContent("Speed"));
		EditorGUILayout.PropertyField(_prop_loopmode, new GUIContent("Loop Mode"));

		EditorGUILayout.PropertyField(_prop_color, new GUIContent("Color"));
		EditorGUILayout.PropertyField(_prop_update, new GUIContent("Update"));

		EditorGUILayout.BeginVertical("box");
		mod.showdataimport = EditorGUILayout.Foldout(mod.showdataimport, "Data Import");

		if ( mod.showdataimport )
		{
			EditorGUILayout.PropertyField(_prop_firstframe, new GUIContent("First"));
			EditorGUILayout.PropertyField(_prop_lastframe, new GUIContent("Last"));
			EditorGUILayout.PropertyField(_prop_skip, new GUIContent("Frame Skip"));
			EditorGUILayout.PropertyField(_prop_pskip, new GUIContent("Particle Skip"));
			EditorGUILayout.PropertyField(_prop_yup, new GUIContent("Y Up Change"));

			//int val = 0;
			//mod.decformat = EditorGUILayout.IntSlider("Format name" + val.ToString("D" + mod.decformat) + ".xyz", mod.decformat, 1, 6);
			//mod.namesplit = EditorGUILayout.TextField("Name Split Char", mod.namesplit);

			if ( GUILayout.Button("Load Frames") )
			{
				string file = EditorUtility.OpenFilePanel("Point Cloud XYZ File", mod.lastpath, "xyz");

				if ( file != null && file.Length > 1 )
				{
					mod.lastpath = file;
					LoadPC(mod, file, mod.firstframe, mod.lastframe, mod.skip);
				}
			}

			//EditorGUILayout.PropertyField(_prop_streamply, new GUIContent("Stream PLY file"));
			if ( GUILayout.Button("Load PLY File") )
			{
				string file = EditorUtility.OpenFilePanel("PLY File", mod.lastpath, "ply");

				if ( file != null && file.Length > 1 )
				{
					mod.lastpath = file;
					//if ( mod.streamply )
						LoadPLYFileStream(mod, file);
					//else
						//LoadPLYFile(mod, file);	//, mod.firstframe, mod.lastframe, mod.skip);
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

	public void LoadPC(MegaCachePointCloudXYZ mod, string filename, int first, int last, int step)
	{
		if ( mod.image == null )
		{
			mod.image = ScriptableObject.CreateInstance<MegaCachePCXYZImage>();
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
				MegaCachePCXYZFrame fr = mod.LoadFrame(filename, i);
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

#if false
	public void LoadPLYFile(MegaCachePointCloudXYZ mod, string filename)	//, int first, int last, int step)
	{
		if ( mod.image == null )
			mod.image = ScriptableObject.CreateInstance<MegaCachePCXYZImage>();

		if ( mod.image && mod.image.frames.Count > 0 )
		{
			if ( !EditorUtility.DisplayDialog("Add to or Replace", "Add new Frames to existing list, or Replace All", "Add", "Replace") )
			{
				mod.image.frames.Clear();
				mod.image.maxpoints = 0;
			}
		}

		StreamReader stream = File.OpenText(filename);
		if ( stream == null )
			return;

		string entireText = stream.ReadToEnd();

		stream.Close();

		entireText.Replace('\n', '\r');

		char[] splitIdentifier = { ' ' };

		StringReader reader = new StringReader(entireText);

		bool readheader = true;
		int haveverts = -1;
		//int havenorms = -1;
		int havecols = -1;
		bool havealpha = false;

		int vcount = 0;

		int index = 0;

		while ( readheader )
		{
			string ps = reader.ReadLine();
			if ( ps == null || ps.Length == 0 )
				break;

			string[] brokenString = ps.Split(splitIdentifier, 50);

			switch ( brokenString[0] )
			{
				case "ply":
					break;

				case "format":
					if ( brokenString[1] != "ascii" )
					{
						Debug.LogWarning("Only Ascii PLY format files are supported!");
						stream.Close();
						return;
					}
					break;

				case "end_header":
					readheader = false;
					break;

				case "comment":	break;
				case "element":
					if ( brokenString[1] == "vertex" )
					{
						vcount = int.Parse(brokenString[2]);
					}
					break;

				case "property":
					if ( brokenString[2] == "x" )
						haveverts = index;

					//if ( brokenString[2] == "nx" )
						//havenorms = index;

					if ( brokenString[2] == "red" )
						havecols = index;

					if ( brokenString[2] == "alpha" )
						havealpha = true;

					index++;
					break;
			}
		}

		List<Vector3> pos = new List<Vector3>();
		List<Color32> col = new List<Color32>();

		Vector3 p = Vector3.zero;
		Color32 c = new Color32(255, 255, 255, 255);

		MegaCachePCXYZFrame frame = new MegaCachePCXYZFrame();

		int barcount = 0;
		int countlim = vcount / 100;
		int skip = 0;

		for ( int i = 0; i < vcount; i++ )
		{
			float a = (float)i / (float)vcount;

			barcount++;
			if ( barcount > countlim )
			{
				barcount = 0;
				if ( EditorUtility.DisplayCancelableProgressBar("Loading Points", "Point " + i, a) )
					break;
			}

			string ps = reader.ReadLine();
			if ( ps == null || ps.Length == 0 )
				break;

			string[] brokenString = ps.Split(splitIdentifier, 50);

			p.x = float.Parse(brokenString[haveverts]);
			p.y = float.Parse(brokenString[haveverts + 1]);
			p.z = float.Parse(brokenString[haveverts + 2]);

			c.r = byte.Parse(brokenString[havecols]);
			c.g = byte.Parse(brokenString[havecols + 1]);
			c.b = byte.Parse(brokenString[havecols + 2]);

			if ( havealpha )
				c.a = byte.Parse(brokenString[havecols + 3]);

			skip++;
			if ( skip >= mod.particleskip )
			{
				skip = 0;
				pos.Add(p * mod.importscale);
				col.Add(c);
			}
		}

		EditorUtility.ClearProgressBar();

		frame.points = pos.ToArray();
		frame.color = col.ToArray();

		if ( frame != null )
		{
			mod.image.frames.Add(frame);

			if ( frame.points.Length > mod.image.maxpoints )
				mod.image.maxpoints = frame.points.Length;
		}
	}
#endif

	public void LoadPLYFileStream(MegaCachePointCloudXYZ mod, string filename)	//, int first, int last, int step)
	{
		if ( mod.image == null )
			mod.image = ScriptableObject.CreateInstance<MegaCachePCXYZImage>();

		if ( mod.image && mod.image.frames.Count > 0 )
		{
			if ( !EditorUtility.DisplayDialog("Add to or Replace", "Add new Frames to existing list, or Replace All", "Add", "Replace") )
			{
				mod.image.frames.Clear();
				mod.image.maxpoints = 0;
			}
		}

		StreamReader stream = File.OpenText(filename);
		if ( stream == null )
			return;

		//string entireText = stream.ReadToEnd();

		char[] splitIdentifier = { ' ' };

		//StringReader reader = new StringReader(entireText);

		bool readheader = true;
		int haveverts = -1;
		//int havenorms = -1;
		int havecols = -1;
		bool havealpha = false;

		int vcount = 0;

		int index = 0;

		while ( readheader )
		{
			string ps = stream.ReadLine();
			if ( ps == null || ps.Length == 0 )
				break;

			string[] brokenString = ps.Split(splitIdentifier, 50);

			switch ( brokenString[0] )
			{
				case "ply":
					break;

				case "format":
					if ( brokenString[1] != "ascii" )
					{
						Debug.LogWarning("Only Ascci PLY format files are supported!");
						stream.Close();
						return;
					}
					break;

				case "end_header":
					readheader = false;
					break;

				case "comment": break;
				case "element":
					if ( brokenString[1] == "vertex" )
					{
						vcount = int.Parse(brokenString[2]);
					}
					break;

				case "property":
					if ( brokenString[2] == "x" )
						haveverts = index;

					//if ( brokenString[2] == "nx" )
					//havenorms = index;

					if ( brokenString[2] == "red" )
						havecols = index;

					if ( brokenString[2] == "alpha" )
						havealpha = true;

					index++;
					break;
			}
		}

		if ( havecols == -1 )
			mod.havecol = false;
		else
			mod.havecol = true;

		List<Vector3> pos = new List<Vector3>();
		List<Color32> col = new List<Color32>();

		Vector3 p = Vector3.zero;
		Color32 c = new Color32(255, 255, 255, 255);

		MegaCachePCXYZFrame frame = new MegaCachePCXYZFrame();

		int barcount = 0;
		int countlim = vcount / 100;

		int skip = 0;

		for ( int i = 0; i < vcount; i++ )
		{
			float a = (float)i / (float)vcount;

			barcount++;
			if ( barcount > countlim )
			{
				barcount = 0;
				if ( EditorUtility.DisplayCancelableProgressBar("Loading Points", "Point " + i, a) )
					break;
			}

			string ps = stream.ReadLine();
			if ( ps == null || ps.Length == 0 )
				break;

			string[] brokenString = ps.Split(splitIdentifier, 50);
			//if ( brokenString.Length != 8 )
			//{
				//Debug.Log("l " + brokenString.Length + " i " + i + " ps " + ps);
				//break;
			//}

			p.x = float.Parse(brokenString[haveverts]);
			p.y = float.Parse(brokenString[haveverts + 1]);
			p.z = float.Parse(brokenString[haveverts + 2]);

			if ( havecols >= 0 )
			{
				c.r = byte.Parse(brokenString[havecols]);
				c.g = byte.Parse(brokenString[havecols + 1]);
				c.b = byte.Parse(brokenString[havecols + 2]);

				if ( havealpha )
					c.a = byte.Parse(brokenString[havecols + 3]);
			}

			skip++;
			if ( skip >= mod.particleskip )
			{
				skip = 0;
				pos.Add(p * mod.importscale);
				col.Add(c);
			}
		}

		stream.Close();
		EditorUtility.ClearProgressBar();

		frame.points = pos.ToArray();
		frame.color = col.ToArray();

		if ( frame != null )
		{
			mod.image.frames.Add(frame);

			if ( frame.points.Length > mod.image.maxpoints )
				mod.image.maxpoints = frame.points.Length;
		}
	}

}