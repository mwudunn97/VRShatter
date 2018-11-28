
using UnityEngine;
using UnityEditor;
using System.IO;

public class MegaCacheFile
{
	static public MegaCacheParticleImage LoadCache(string filename)
	{
		MegaCacheParticleImage img = null;

		FileStream fs = new FileStream(filename, FileMode.Open);
		if ( fs != null )
		{
			BinaryReader br = new BinaryReader(fs);

			if ( br != null )
			{
				img = ScriptableObject.CreateInstance<MegaCacheParticleImage>();
				img.ReadData(br);

				br.Close();
			}

			fs.Close();
		}

		return img;
	}
}

#if false
[CustomEditor(typeof(MegaCacheParticle))]
public class MegaCacheParticleEditor : Editor
{
	SerializedProperty _prop_particle;
	SerializedProperty _prop_time;
	SerializedProperty _prop_fps;
	SerializedProperty _prop_speed;
	SerializedProperty _prop_loopmode;
	SerializedProperty _prop_frame;
	SerializedProperty _prop_importscale;
	SerializedProperty _prop_maxparticles;

	[MenuItem("GameObject/Create Other/MegaCache/Particle")]
	static void CreateOBJRef()
	{
		Vector3 pos = Vector3.zero;
		if ( UnityEditor.SceneView.lastActiveSceneView != null )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("Mega Cache Particle");

		go.AddComponent<MegaCacheParticle>();
		go.transform.position = pos;
		Selection.activeObject = go;
	}

	private void OnEnable()
	{
		_prop_particle = serializedObject.FindProperty("particle");
		_prop_maxparticles = serializedObject.FindProperty("maxparticles");
		_prop_time = serializedObject.FindProperty("time");
		_prop_fps = serializedObject.FindProperty("fps");
		_prop_speed = serializedObject.FindProperty("speed");
		_prop_loopmode = serializedObject.FindProperty("loopmode");
		_prop_frame = serializedObject.FindProperty("framenum");
		_prop_importscale = serializedObject.FindProperty("importscale");
	}

	public override void OnInspectorGUI()
	{
		MegaCacheParticle mod = (MegaCacheParticle)target;

		serializedObject.Update();

		EditorGUIUtility.LookLikeControls();

		EditorGUILayout.PropertyField(_prop_particle, new GUIContent("Particle System"));
		EditorGUILayout.PropertyField(_prop_maxparticles, new GUIContent("Max Particles"));

		EditorGUILayout.PropertyField(_prop_importscale, new GUIContent("Import Scale"));

		if ( GUILayout.Button("Load Particle Cache") )
		{
			string file = EditorUtility.OpenFilePanel("Particle Cache", mod.lastpath, "txt");

			if ( file != null && file.Length > 1 )
			{
				mod.lastpath = file;
				LoadFile(mod, file);
			}
		}

		EditorGUILayout.BeginVertical("box");

		if ( mod.image )
			EditorGUILayout.IntSlider(_prop_frame, 0, mod.image.frames.Count - 1, new GUIContent("Frame"));

		mod.animate = EditorGUILayout.BeginToggleGroup("Animate", mod.animate);
		EditorGUILayout.PropertyField(_prop_time, new GUIContent("Time"));
		EditorGUILayout.PropertyField(_prop_fps, new GUIContent("Fps"));
		EditorGUILayout.PropertyField(_prop_speed, new GUIContent("Speed"));
		EditorGUILayout.PropertyField(_prop_loopmode, new GUIContent("Loop Mode"));
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndToggleGroup();

		if ( GUI.changed )
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}

	void LoadFile(MegaCacheParticle mod, string filename)
	{
		//mod.frames.Clear();

		StreamReader stream = File.OpenText(filename);
		string entireText = stream.ReadToEnd();
		stream.Close();

		char[] splitIdentifier = { ' ' };
		char[] fsplit = { 'f' };

		StringReader reader = new StringReader(entireText);

		int max = int.Parse(reader.ReadLine());
		//Debug.Log("max " + max);

		int frames = int.Parse(reader.ReadLine());
		//Debug.Log("frames " + frames);

		MegaCacheParticleImage img = CreateInstance<MegaCacheParticleImage>();

		for ( int i = 0; i < frames; i++ )
		{
			MegaCacheParticleFrame fr = new MegaCacheParticleFrame();

			int p = int.Parse(reader.ReadLine());
			//Debug.Log("parts " + p);

			for ( int j = 0; j < p; j++ )
			{
				string ps = reader.ReadLine();

				string[] brokenString = ps.Split(splitIdentifier, 50);

				Vector3 pos = Vector3.zero;

				pos.x = float.Parse(brokenString[0]);
				pos.y = float.Parse(brokenString[1]);
				pos.z = float.Parse(brokenString[2]);

				if ( brokenString.Length > 3 )
				{
					float life = float.Parse(brokenString[4].Split(fsplit)[0]) / 30.0f;
					fr.life.Add(life);

					float age = life - (float.Parse(brokenString[3].Split(fsplit)[0])) / 30.0f;
					fr.age.Add(age);

				}

				fr.positions.Add(pos * mod.importscale);
			}

			img.frames.Add(fr);
		}

		mod.image = img;
	}
}
#endif