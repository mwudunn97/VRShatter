
#if UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
#else
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(MegaCacheParticleLegacyPlayback))]
public class MegaCacheParticleLegacyPlaybackEditor : Editor
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

	[MenuItem("GameObject/Create Other/MegaCache/Particle Legacy Playback")]
	static void CreateOBJRef()
	{
		Vector3 pos = Vector3.zero;
		if ( UnityEditor.SceneView.lastActiveSceneView != null )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("Mega Cache Particle");

		go.AddComponent<MegaCacheParticleLegacyPlayback>();
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
	}

	public override void OnInspectorGUI()
	{
		MegaCacheParticleLegacyPlayback mod = (MegaCacheParticleLegacyPlayback)target;

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
				MegaCacheParticlePlaybackEditor.LoadFilePlayBack(mod, file);
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
}
#endif