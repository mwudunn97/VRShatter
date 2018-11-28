
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(MegaCacheParticlePlayback))]
public class MegaCacheParticlePlaybackEditor : Editor
{
	SerializedProperty _prop_particle;
	SerializedProperty _prop_speed;
	SerializedProperty _prop_importscale;
	SerializedProperty _prop_vel;
	SerializedProperty _prop_rot;
	SerializedProperty _prop_scale;
	SerializedProperty _prop_spin;
	SerializedProperty _prop_scaleall;
	SerializedProperty _prop_sizescale;
	SerializedProperty _prop_cachesrc;
	SerializedProperty _prop_optimize;
	SerializedProperty _prop_savevel;
	SerializedProperty _prop_saverot;
	SerializedProperty _prop_savescale;
	SerializedProperty _prop_savespin;
	SerializedProperty _prop_axis;
	SerializedProperty _prop_removealive;
	SerializedProperty _prop_loopmode;
	SerializedProperty _prop_fps;
	SerializedProperty _prop_time;
	SerializedProperty _prop_maxfile;
	SerializedProperty _prop_seqstart;
	SerializedProperty _prop_seqend;

	[MenuItem("GameObject/Create Other/MegaCache/Particle Playback")]
	static void CreateOBJRef()
	{
		Vector3 pos = Vector3.zero;
		if ( UnityEditor.SceneView.lastActiveSceneView != null )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("Mega Cache Particle");

		go.AddComponent<MegaCacheParticlePlayback>();
		go.transform.position = pos;
		Selection.activeObject = go;
	}

	private void OnEnable()
	{
		_prop_particle = serializedObject.FindProperty("particle");
		_prop_speed = serializedObject.FindProperty("speed");
		_prop_importscale = serializedObject.FindProperty("importscale");
		_prop_vel = serializedObject.FindProperty("vel");
		_prop_rot = serializedObject.FindProperty("rot");
		_prop_scale = serializedObject.FindProperty("scale");
		_prop_spin = serializedObject.FindProperty("spin");
		_prop_scaleall = serializedObject.FindProperty("scaleall");
		_prop_sizescale = serializedObject.FindProperty("sizescale");
		_prop_cachesrc = serializedObject.FindProperty("cachesrc");
		_prop_optimize = serializedObject.FindProperty("optimize");
		_prop_savevel = serializedObject.FindProperty("savevel");
		_prop_saverot = serializedObject.FindProperty("saverot");
		_prop_savescale = serializedObject.FindProperty("savescale");
		_prop_savespin = serializedObject.FindProperty("savespin");
		_prop_axis = serializedObject.FindProperty("axis");
		_prop_removealive = serializedObject.FindProperty("removealive");
		_prop_loopmode = serializedObject.FindProperty("loopmode");
		_prop_fps = serializedObject.FindProperty("fps");
		_prop_time = serializedObject.FindProperty("time");
		_prop_maxfile = serializedObject.FindProperty("maxfile");
		_prop_seqstart = serializedObject.FindProperty("seqstart");
		_prop_seqend = serializedObject.FindProperty("seqend");
	}

	public override void OnInspectorGUI()
	{
		MegaCacheParticlePlayback mod = (MegaCacheParticlePlayback)target;

		serializedObject.Update();

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018
		EditorGUIUtility.LookLikeControls();
#endif

		EditorGUILayout.PropertyField(_prop_time, new GUIContent("Time"));
		if ( mod.time < 0.0f )
			mod.time = 0.0f;

		EditorGUILayout.PropertyField(_prop_fps, new GUIContent("Fps"));
		EditorGUILayout.PropertyField(_prop_loopmode, new GUIContent("Loop Mode"));
		EditorGUILayout.PropertyField(_prop_particle, new GUIContent("Particle System"));
		EditorGUILayout.PropertyField(_prop_importscale, new GUIContent("Import Scale"));

		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Data Import Options");
		EditorGUILayout.PropertyField(_prop_vel, new GUIContent("Velocity"));
		EditorGUILayout.PropertyField(_prop_rot, new GUIContent("Rotation"));
		EditorGUILayout.PropertyField(_prop_scale, new GUIContent("Scale"));
		EditorGUILayout.PropertyField(_prop_spin, new GUIContent("Spin"));
		EditorGUILayout.PropertyField(_prop_removealive, new GUIContent("Remove Alive"));
		EditorGUILayout.PropertyField(_prop_maxfile, new GUIContent("Max File"));
		EditorGUILayout.EndVertical();

		if ( GUILayout.Button("Import Particle Cache") )
		{
			string file = EditorUtility.OpenFilePanel("Import Particle Cache", mod.lastpath, "prt");

			if ( file != null && file.Length > 1 )
			{
				mod.lastpath = file;
				//MegaCacheParticle.LoadFilePlayBack(mod, file);
				LoadFilePlayBack(mod, file);
			}
		}

		EditorGUILayout.PropertyField(_prop_seqstart, new GUIContent("Maya Seq Start"));
		EditorGUILayout.PropertyField(_prop_seqend, new GUIContent("Maya Seq End"));

		if ( GUILayout.Button("Import Maya Particles") )
		{
			string file = EditorUtility.OpenFilePanel("Import Maya Particles", mod.lastpath, "pda");

			if ( file != null && file.Length > 1 )
			{
				mod.lastpath = file;
				LoadPDAPlayback(mod, file, mod.seqstart, mod.seqend);
			}
		}

		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Save Data Options");
		EditorGUILayout.PropertyField(_prop_optimize, new GUIContent("Optimize"));
		EditorGUILayout.PropertyField(_prop_savevel, new GUIContent("Velocity"));
		EditorGUILayout.PropertyField(_prop_saverot, new GUIContent("Rotation"));
		EditorGUILayout.PropertyField(_prop_savescale, new GUIContent("Scale"));
		EditorGUILayout.PropertyField(_prop_savespin, new GUIContent("Spin"));
		EditorGUILayout.EndVertical();

		if ( GUILayout.Button("Save Particle Cache File") )
		{
			string file = EditorUtility.SaveFilePanel("Save MegaCache Particle Cache", mod.lastpath, "Particles", "mcp");

			if ( file != null && file.Length > 1 )
			{
				mod.lastpath = file;
				MegaCacheParticle.SaveCacheFile(mod, file);
			}
		}

		if ( GUILayout.Button("Load MegaCache File") )
		{
			string file = EditorUtility.OpenFilePanel("Load MegaCache Particle Cache", mod.lastpath, "mcp");

			if ( file != null && file.Length > 1 )
			{
				mod.lastpath = file;
				//MegaCacheParticleImage img = MegaCacheParticleImage.LoadCache(file);
				MegaCacheParticleImage img = MegaCacheFile.LoadCache(file);

				if ( img )
				{
					if ( mod.image )
					{
						if ( Application.isEditor )
							DestroyImmediate(mod.image);
						else
							Destroy(mod.image);
					}
					mod.image = img;
				}
			}
		}

		string infstring = "";
		if ( mod.image )
		{
			infstring = "Current Memory: " + (mod.image.CalcMemory() / 1024) + "KB";

			if ( mod.image.optimized )
				infstring += "\nParticles Optmized: " + mod.image.optparticles.Count;
			else
				infstring += "\nParticles: " + mod.image.particles.Count;
		}

		EditorGUILayout.HelpBox(infstring, MessageType.None);

		EditorGUILayout.PropertyField(_prop_cachesrc, new GUIContent("Cache Data Src"));

		EditorGUILayout.BeginVertical("box");
		mod.showpaths = EditorGUILayout.BeginToggleGroup("Show Paths", mod.showpaths);
		mod.showstart = EditorGUILayout.IntSlider("Start", mod.showstart, 0, mod.showparticlestep);
		mod.showparticlestep = EditorGUILayout.IntSlider("Particle Step", mod.showparticlestep, 1, 50);
		mod.showposstep = EditorGUILayout.IntSlider("Position Step", mod.showposstep, 1, 10);
		mod.showcolor = EditorGUILayout.ColorField("Color", mod.showcolor);
		EditorGUILayout.EndToggleGroup();
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical("box");

		EditorGUILayout.Slider(_prop_speed, -2.0f, 2.0f, new GUIContent("Speed"));
		EditorGUILayout.Slider(_prop_scaleall, 0.0f, 4.0f, new GUIContent("Scale"));
		EditorGUILayout.Slider(_prop_sizescale, 0.0f, 8.0f, new GUIContent("Size"));
		EditorGUILayout.PropertyField(_prop_axis, new GUIContent("Rot Axis"));
		EditorGUILayout.EndVertical();

		if ( GUI.changed )
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}

	public static void LoadFilePlayBack(MegaCacheParticle mod, string filename)
	{
		MegaCacheParticle.offset = 0;
		StreamReader stream = File.OpenText(filename);
		string entireText = stream.ReadToEnd();
		stream.Close();

		char[] splitIdentifier = { ' ' };

		//StringReader reader = new StringReader(entireText);

		//int.Parse(reader.ReadLine());	// max
		int.Parse(MegaCacheParticle.ReadLine(entireText));	// max

		//int frames = int.Parse(reader.ReadLine());
		int frames = int.Parse(MegaCacheParticle.ReadLine(entireText));

		MegaCacheParticleImage img;

		if ( mod.image == null )
			img = ScriptableObject.CreateInstance<MegaCacheParticleImage>();
		else
		{
			if ( EditorUtility.DisplayDialog("Add to or Replace", "Particles already loaded do you want to Replace?", "Yes", "No") )
				img = ScriptableObject.CreateInstance<MegaCacheParticleImage>();
			else
				img = mod.image;
		}

		int skip = 1;
		int scount = 0;
		int maxp = 0;

		for ( int i = 0; i < frames; i++ )
		{
			MegaCacheParticleHistory ph = new MegaCacheParticleHistory();
			img.particles.Add(ph);

			//int p = int.Parse(reader.ReadLine());
			int p = int.Parse(MegaCacheParticle.ReadLine(entireText));

			Vector3 pos = Vector3.zero;
			Vector3 rot = Vector3.zero;
			Vector3 vel = Vector3.zero;

			EditorUtility.DisplayProgressBar("Particle Import", "Importing frame " + i + " off " + frames, ((float)i / (float)frames));

			for ( int j = 0; j < p; j++ )
			{
				//string ps = reader.ReadLine();
				string ps = MegaCacheParticle.ReadLine(entireText);
				ps = ps.Replace(',', '.');

				string[] brokenString = ps.Split(splitIdentifier, 50);

				//int id = int.Parse(brokenString[0]) - 1;

				if ( scount == 0 )
				{
					pos.x = float.Parse(brokenString[1]);
					if ( mod.maxfile )
						pos.x = -pos.x;
					pos.y = float.Parse(brokenString[2]);
					pos.z = float.Parse(brokenString[3]);

					float life = float.Parse(brokenString[12]);	// / 30.0f;
					//float age = life - (float.Parse(brokenString[13]));	// / 30.0f;

					if ( mod.vel )
					{
						vel.x = float.Parse(brokenString[4]);
						if ( mod.maxfile )
							vel.x = -vel.x;
						vel.y = float.Parse(brokenString[5]);
						vel.z = float.Parse(brokenString[6]);

						ph.vels.Add(vel * mod.importscale);
					}

					if ( mod.rot )
					{
						if ( mod.maxfile )
						{
							rot.x = float.Parse(brokenString[7]) * Mathf.Rad2Deg;
							rot.z = float.Parse(brokenString[8]) * Mathf.Rad2Deg;
							rot.y = float.Parse(brokenString[9]) * Mathf.Rad2Deg;
						}
						else
						{
							rot.x = float.Parse(brokenString[7]) * Mathf.Rad2Deg;
							rot.y = float.Parse(brokenString[8]) * Mathf.Rad2Deg;
							rot.z = float.Parse(brokenString[9]) * Mathf.Rad2Deg;
						}

						ph.rots.Add(rot);
					}

					if ( mod.scale )
					{
						float scale = float.Parse(brokenString[11]);
						ph.scale.Add(scale * mod.importscale);
					}

					if ( mod.spin )
					{
						float spin = float.Parse(brokenString[10]) * Mathf.Rad2Deg;
						ph.spin.Add(spin);
					}

					ph.life = life;
					//ph.age.Add(age);

					ph.positions.Add(pos * mod.importscale);
				}

				if ( ph.positions.Count > maxp )
					maxp = ph.positions.Count;
			}

			scount++;
			if ( scount == skip )
				scount = 0;
		}

		EditorUtility.ClearProgressBar();

		for ( int i = img.particles.Count - 1; i >= 0; i-- )
		{
			MegaCacheParticleHistory ph = img.particles[i];

			if ( ph.positions.Count < 1 )
			{
				img.particles.RemoveAt(i);
				frames--;
			}
		}

		img.frames = frames;
		img.maxparticles = maxp;
		mod.image = img;
	}

	void LoadPDAPlayback(MegaCacheParticle mod, string filename, int start, int end)
	{
		char[] namesplit = { '.' };

		string[] fname = filename.Split(namesplit);

		//List<int> dellist = new List<int>();

		MegaCacheParticleImage img = ScriptableObject.CreateInstance<MegaCacheParticleImage>();

		bool remove = false;

		img.frames = 0;
		for ( int i = start; i < end; i++ )
		{
			if ( i == end - 1 )
				remove = true;

			string name = fname[0] + "." + i + ".pda";

			if ( File.Exists(name) )
			{
				LoadPDAFile(mod, img, name, remove);
			}
		}

		img.maxparticles = 0;
		for ( int i = img.particles.Count - 1; i >= 0; i-- )
		{
			MegaCacheParticleHistory ph = img.particles[i];

			if ( ph.positions.Count > img.maxparticles )
				img.maxparticles = ph.positions.Count;
		}

		img.frames = img.particles.Count;
		mod.image = img;

		//Debug.Log("maxp " + img.maxparticles);
	}

	void LoadPDAFile(MegaCacheParticle mod, MegaCacheParticleImage img, string filename, bool remove)
	{
		StreamReader stream = File.OpenText(filename);
		string entireText = stream.ReadToEnd();
		stream.Close();

		char[] splitIdentifier = { ' ' };

		//StringReader reader = new StringReader(entireText);

		MegaCacheParticle.offset = 0;
		//reader.ReadLine();	// ATTRIBUTES
		MegaCacheParticle.ReadLine(entireText);

		//string[] attribs = reader.ReadLine().Split(splitIdentifier, System.StringSplitOptions.RemoveEmptyEntries);
		string[] attribs = MegaCacheParticle.ReadLine(entireText).Split(splitIdentifier, System.StringSplitOptions.RemoveEmptyEntries);

		//reader.ReadLine();	// TYPES
		MegaCacheParticle.ReadLine(entireText);
		//string[] types = reader.ReadLine().Split(splitIdentifier, System.StringSplitOptions.RemoveEmptyEntries);	// actual values
		string[] types = MegaCacheParticle.ReadLine(entireText).Split(splitIdentifier, System.StringSplitOptions.RemoveEmptyEntries);	// actual values

		int[] attriboff = new int[attribs.Length];

		int off = 0;

		for ( int i = 0; i < types.Length; i++ )
		{
			attriboff[i] = off;

			switch ( types[i] )
			{
				case "V":	off += 3;	break;
				case "I":	off += 1;	break;
				case "R":	off += 1;	break;
				default:	Debug.Log("Unknown Type " + types[i]);	off += 1;	break;
			}
		}

		//string[] vals = reader.ReadLine().Split(splitIdentifier, 2);
		string[] vals = MegaCacheParticle.ReadLine(entireText).Split(splitIdentifier, 2);
		int numParticles = int.Parse(vals[1]);

		//reader.ReadLine();	// BEGIN DATA
		MegaCacheParticle.ReadLine(entireText);

		MegaCacheParticleHistory ph = new MegaCacheParticleHistory();

		for ( int j = 0; j < numParticles; j++ )
		{
			Vector3 pos = Vector3.zero;
			Vector3 rot = Vector3.zero;
			Vector3 vel = Vector3.zero;
			//int id = 0;
			float life = 0.0f;
			float scale = 1.0f;
			float spin = 0.0f;

			//string ps = reader.ReadLine();
			string ps = MegaCacheParticle.ReadLine(entireText);
			string[] values = ps.Split(splitIdentifier, 50);

			for ( int a = 0; a < attribs.Length; a++ )
			{
				int of = attriboff[a];

				switch ( attribs[a] )
				{
					case "position":
						pos.x = float.Parse(values[of]);
						pos.y = float.Parse(values[of + 1]);
						pos.z = float.Parse(values[of + 2]);
						break;

					case "velocity":
						vel.x = float.Parse(values[of]);
						vel.y = float.Parse(values[of + 1]);
						vel.z = float.Parse(values[of + 2]);
						break;

					case "id":
						//id = int.Parse(values[of]);
						break;

					case "lifespanPP":
						life = (float)double.Parse(values[of]);
						break;

					//case "age":
						//age = float.Parse(values[of]);
						//break;

					case "radiusPP":
						scale = float.Parse(values[of]);
						break;

					default:
						break;
				}
			}

			if ( mod.vel )
				ph.vels.Add(vel * mod.importscale);

			if ( mod.rot )
				ph.rots.Add(rot);

			if ( mod.scale )
				ph.scale.Add(scale * mod.importscale);

			if ( mod.spin )
				ph.spin.Add(spin);

			ph.life = life;
			ph.positions.Add(pos * mod.importscale);
		}

		img.particles.Add(ph);
	}
}