#if UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
#else
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(MegaCacheParticleProLegacy))]
public class MegaCacheParticleProLegacyEditor : Editor
{
	SerializedProperty _prop_particle;
	SerializedProperty _prop_speed;
	SerializedProperty _prop_importscale;
	SerializedProperty _prop_maxparticles;
	SerializedProperty _prop_vel;
	SerializedProperty _prop_rot;
	SerializedProperty _prop_scale;
	SerializedProperty _prop_spin;
	SerializedProperty _prop_scaleall;
	SerializedProperty _prop_emitscale;
	SerializedProperty _prop_emitspeed;
	SerializedProperty _prop_useemit;
	SerializedProperty _prop_emitrate;
	SerializedProperty _prop_sizescale;
	SerializedProperty _prop_prewarm;
	SerializedProperty _prop_cachesrc;
	SerializedProperty _prop_optimize;
	SerializedProperty _prop_savevel;
	SerializedProperty _prop_saverot;
	SerializedProperty _prop_savescale;
	SerializedProperty _prop_savespin;
	SerializedProperty _prop_axis;
	SerializedProperty _prop_mode;
	SerializedProperty _prop_removealive;
	SerializedProperty _prop_seqstart;
	SerializedProperty _prop_seqend;
	SerializedProperty _prop_yup;

	[MenuItem("GameObject/Create Other/MegaCache/Particle Pro Legacy")]
	static void CreateOBJRef()
	{
		Vector3 pos = Vector3.zero;
		if ( UnityEditor.SceneView.lastActiveSceneView != null )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("MegaCache Particle Legacy");

		go.AddComponent<MegaCacheParticleProLegacy>();
		go.transform.position = pos;
		Selection.activeObject = go;
	}

	private void OnEnable()
	{
		_prop_particle = serializedObject.FindProperty("particle");
		_prop_maxparticles = serializedObject.FindProperty("maxparticles");
		_prop_speed = serializedObject.FindProperty("speed");
		_prop_importscale = serializedObject.FindProperty("importscale");
		_prop_vel = serializedObject.FindProperty("vel");
		_prop_rot = serializedObject.FindProperty("rot");
		_prop_scale = serializedObject.FindProperty("scale");
		_prop_spin = serializedObject.FindProperty("spin");
		_prop_scaleall = serializedObject.FindProperty("scaleall");
		_prop_emitscale = serializedObject.FindProperty("emitscale");
		_prop_emitspeed = serializedObject.FindProperty("emitspeed");
		_prop_useemit = serializedObject.FindProperty("useemit");
		_prop_emitrate = serializedObject.FindProperty("emitrate");
		_prop_sizescale = serializedObject.FindProperty("sizescale");
		_prop_prewarm = serializedObject.FindProperty("prewarm");
		_prop_cachesrc = serializedObject.FindProperty("cachesrc");
		_prop_optimize = serializedObject.FindProperty("optimize");
		_prop_savevel = serializedObject.FindProperty("savevel");
		_prop_saverot = serializedObject.FindProperty("saverot");
		_prop_savescale = serializedObject.FindProperty("savescale");
		_prop_savespin = serializedObject.FindProperty("savespin");
		_prop_axis = serializedObject.FindProperty("axis");
		_prop_mode = serializedObject.FindProperty("mode");
		_prop_removealive = serializedObject.FindProperty("removealive");
		_prop_seqstart = serializedObject.FindProperty("seqstart");
		_prop_seqend = serializedObject.FindProperty("seqend");
		_prop_yup = serializedObject.FindProperty("yupimport");
	}

	public override void OnInspectorGUI()
	{
		MegaCacheParticleProLegacy mod = (MegaCacheParticleProLegacy)target;

		serializedObject.Update();

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018
		EditorGUIUtility.LookLikeControls();
#endif

		EditorGUILayout.PropertyField(_prop_particle, new GUIContent("Particle System"));
		EditorGUILayout.PropertyField(_prop_maxparticles, new GUIContent("Max Particles"));
		EditorGUILayout.PropertyField(_prop_importscale, new GUIContent("Import Scale"));

		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Data Import Options");
		EditorGUILayout.PropertyField(_prop_vel, new GUIContent("Velocity"));
		EditorGUILayout.PropertyField(_prop_rot, new GUIContent("Rotation"));
		EditorGUILayout.PropertyField(_prop_scale, new GUIContent("Scale"));
		EditorGUILayout.PropertyField(_prop_spin, new GUIContent("Spin"));
		EditorGUILayout.PropertyField(_prop_removealive, new GUIContent("Remove Alive"));
		EditorGUILayout.PropertyField(_prop_yup, new GUIContent("Y Up Change"));
		EditorGUILayout.EndVertical();

		if ( GUILayout.Button("Import Particle Cache") )
		{
			string file = EditorUtility.OpenFilePanel("Import Particle Cache", mod.lastpath, "prt");

			if ( file != null && file.Length > 1 )
			{
				mod.lastpath = file;
				MegaCacheParticle.LoadFile(mod, file);
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
				MegaCacheParticle.LoadPDASequence(mod, file, mod.seqstart, mod.seqend);
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
				infstring += "\nParticles: " + mod.image.optparticles.Count;
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
		EditorGUILayout.Slider(_prop_emitscale, 0.0f, 4.0f, new GUIContent("Emit Scale"));
		EditorGUILayout.Slider(_prop_sizescale, 0.0f, 4.0f, new GUIContent("Size"));
		EditorGUILayout.Slider(_prop_emitspeed, 0.0f, 4.0f, new GUIContent("Emit Speed"));

		EditorGUILayout.PropertyField(_prop_mode, new GUIContent("Mode"));
		EditorGUILayout.PropertyField(_prop_useemit, new GUIContent("Use Emit Rate"));
		EditorGUILayout.PropertyField(_prop_emitrate, new GUIContent("Emit Rate"));
		EditorGUILayout.PropertyField(_prop_prewarm, new GUIContent("Pre Warm"));
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