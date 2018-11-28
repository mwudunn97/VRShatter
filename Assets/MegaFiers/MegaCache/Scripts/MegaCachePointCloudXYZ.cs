
using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if !UNITY_FLASH && !UNITY_PS3 && !UNITY_METRO && !UNITY_WP8
using System.Threading;
#endif

[AddComponentMenu("MegaCache/Point Cloud XYZ")]
[ExecuteInEditMode]
public class MegaCachePointCloudXYZ : MonoBehaviour
{
	public MegaCachePCXYZImage	image;
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

	public bool					showdataimport = false;
	public int					firstframe		= 0;
	public int					lastframe		= 0;
	public int					skip			= 0;
	public int					particleskip	= 0;

	public int					decformat		= 0;
	//public string				namesplit		= "";
	public float				transparency	= 1.0f;
	public bool					yupimport		= false;
	public bool					streamply		= false;
	public bool					update			= true;
	public bool					havecol			= true;
	public Color				color			= Color.white;

#if false
	public MegaCachePCXYZFrame LoadFrame(string filename)
	{
		StreamReader stream = File.OpenText(filename);
		if ( stream == null )
			return null;

		string entireText = stream.ReadToEnd();
		stream.Close();

		entireText.Replace('\n', '\r');

		char[] splitIdentifier = { ' ' };

		//StringReader reader = new StringReader(entireText);
		MegaCacheParticle.offset = 0;

		List<Vector3> pos = new List<Vector3>();
		List<Color32> col = new List<Color32>();

		Vector3 p = Vector3.zero;
		Color32 c = new Color32(255, 255, 255, 255);

		MegaCachePCXYZFrame frame = new MegaCachePCXYZFrame();

		int sk = 0;

		while ( true )
		{
			//string ps = reader.ReadLine();
			string ps = MegaCacheParticle.ReadLine(entireText);
			if ( ps == null || ps.Length == 0 )
				break;

			sk--;
			if ( sk < 0 )
			{
				string[] brokenString = ps.Split(splitIdentifier, 50);

				if ( brokenString.Length == 6 )
				{
					p.x = float.Parse(brokenString[0]);
					p.y = float.Parse(brokenString[1]);
					p.z = float.Parse(brokenString[2]);

					if ( yupimport )
						p = MegaCachePointCloud.AdjustYUp(p);

					c.r = byte.Parse(brokenString[3]);
					c.g = byte.Parse(brokenString[4]);
					c.b = byte.Parse(brokenString[5]);

					pos.Add(p * importscale);
					col.Add(c);
				}
				sk = particleskip;
			}
		}

		frame.points = pos.ToArray();
		frame.color = col.ToArray();

		//mod.image.frames.Add(frame);
		return frame;
	}
#endif

	public MegaCachePCXYZFrame LoadFrameStream(string filename)
	{
		StreamReader stream = File.OpenText(filename);
		if ( stream == null )
			return null;

		//string entireText = stream.ReadToEnd();

		//entireText.Replace('\n', '\r');

		char[] splitIdentifier = { ' ' };

		//StringReader reader = new StringReader(entireText);

		List<Vector3> pos = new List<Vector3>();
		List<Color32> col = new List<Color32>();

		Vector3 p = Vector3.zero;
		Color32 c = new Color32(255, 255, 255, 255);

		MegaCachePCXYZFrame frame = new MegaCachePCXYZFrame();

		int sk = 0;
		if ( havecol )
		{
			while ( true )
			{
				string ps = stream.ReadLine();
				//string ps = MegaCacheParticle.ReadLine(entireText);
				if ( ps == null || ps.Length == 0 )
					break;

				sk--;
				if ( sk < 0 )
				{
					string[] brokenString = ps.Split(splitIdentifier, 50);

					if ( brokenString.Length == 6 )
					{
						p.x = float.Parse(brokenString[0]);
						p.y = float.Parse(brokenString[1]);
						p.z = float.Parse(brokenString[2]);

						if ( yupimport )
							p = MegaCachePointCloud.AdjustYUp(p);

						c.r = byte.Parse(brokenString[3]);
						c.g = byte.Parse(brokenString[4]);
						c.b = byte.Parse(brokenString[5]);

						pos.Add(p * importscale);
						col.Add(c);
					}
					sk = particleskip;
				}
			}
		}
		else
		{
			while ( true )
			{
				string ps = stream.ReadLine();
				//string ps = MegaCacheParticle.ReadLine(entireText);
				if ( ps == null || ps.Length == 0 )
					break;

				sk--;
				if ( sk < 0 )
				{
					string[] brokenString = ps.Split(splitIdentifier, 50);

					if ( brokenString.Length == 6 )
					{
						p.x = float.Parse(brokenString[0]);
						p.y = float.Parse(brokenString[1]);
						p.z = float.Parse(brokenString[2]);

						if ( yupimport )
							p = MegaCachePointCloud.AdjustYUp(p);

						pos.Add(p * importscale);
					}
					sk = particleskip;
				}
			}
		}

		stream.Close();

		frame.points = pos.ToArray();
		if ( havecol )
			frame.color = col.ToArray();

		//mod.image.frames.Add(frame);
		return frame;
	}


	public MegaCachePCXYZFrame LoadFrame(string filename, int frame)
	{
		MegaCachePCXYZFrame fr = null;

		string dir = Path.GetDirectoryName(filename);
		string file = Path.GetFileNameWithoutExtension(filename);

		file = MegaCacheUtils.MakeFileName(file, ref decformat);

		//if ( file.Length > 0 )
		{
			string newfname = dir + "/" + file + frame.ToString("D" + decformat) + ".xyz";
			fr = LoadFrameStream(newfname);
		}

		return fr;
	}


	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=6222");
	}

	void Start()
	{
		//lastsetframe = -1;
		if ( particle == null )
			particle = GetComponent<ParticleSystem>();

		if ( image )
			particles = new ParticleSystem.Particle[image.maxpoints];

		if ( particle && image )
			particle.Emit(image.maxpoints);

		UpdateParticles(0.01f);
	}

	void LateUpdate()
	{
		if ( particle )
		{
			if ( animate )
			{
				if ( Application.isPlaying )
					time += Time.deltaTime * speed;

				float maxtime = (float)image.frames.Count / fps;

				float ftime = time;

				switch ( loopmode )
				{
					case MegaCacheRepeatMode.Loop:
						ftime = Mathf.Repeat(time, maxtime);
						break;

					case MegaCacheRepeatMode.Clamp:
						ftime = Mathf.Clamp(time, 0.0f, maxtime);
						break;
					case MegaCacheRepeatMode.PingPong:
						ftime = Mathf.PingPong(time, maxtime);
						break;
				}

				framenum = (int)((ftime / maxtime) * (float)image.frames.Count);
			}
			UpdateParticles(Time.deltaTime);
		}
	}

	public float playscale = 1.0f;
	public float playsize = 1.0f;

	//int lastsetframe = -1;

	void UpdateParticles(float dt)
	{
		if ( !update )
			return;

		if ( dt > 0.01f )
			dt = 0.01f;

		//if ( framenum == lastsetframe )
			//return;

		if ( particle && image && image.frames != null && image.frames.Count > 0 )
		{
			if ( particles == null || particles.Length != image.maxpoints )
				particles = new ParticleSystem.Particle[image.maxpoints];

			framenum = Mathf.Clamp(framenum, 0, image.frames.Count - 1);
			//lastsetframe = framenum;
			MegaCachePCXYZFrame frame = image.frames[framenum];

			// Do we need this
			//particle.GetParticles(particles);
			if ( havecol )
			{
				byte alpha = (byte)(transparency * 255.0f);

				for ( int i = 0; i < frame.points.Length; i++ )
				{
					particles[i].position = frame.points[i] * playscale;
#if UNITY_2017 || UNITY_2018
					particles[i].remainingLifetime = 1.0f;
#else
					particles[i].lifetime = 1.0f;	//ph.life - ps.time;
#endif
					particles[i].startLifetime = 1.0f;  //ph.life;
					Color32 c = frame.color[i];
					c.a = alpha;
#if UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
					particles[i].startColor = c;
					particles[i].startSize = playsize * playscale;
#else
					particles[i].color = c;
					particles[i].size = playsize * playscale;
#endif
				}
			}
			else
			{
				//byte alpha = (byte)(transparency * 255.0f);
				for ( int i = 0; i < frame.points.Length; i++ )
				{
					particles[i].position = frame.points[i] * playscale;
#if UNITY_2017 || UNITY_2018
					particles[i].remainingLifetime = 1.0f;
#else
					particles[i].lifetime = 1.0f;	//ph.life - ps.time;
#endif
					particles[i].startLifetime = 1.0f;  //ph.life;
#if UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
					particles[i].startColor = color;
					particles[i].startSize = playsize * playscale;
#else
					particles[i].size = playsize * playscale;
					particles[i].color = color;
#endif
				}
			}

			particle.SetParticles(particles, frame.points.Length);
		}
	}

#if !UNITY_FLASH && !UNITY_PS3 && !UNITY_METRO && !UNITY_WP8
	public bool		threadupdate = false;

	public class MegaCachePointTaskInfo
	{
		public string					name;
		public AutoResetEvent			pauseevent;
		public Thread					_thread;
		public MegaCachePointCloudXYZ	objcache;
		public int						startp;
		public int						endp;
		public int						end;
	}

	public int					Cores = 1;
	static bool					isRunning = false;
	MegaCachePointTaskInfo[]	tasks;

	void MakeThreads(MegaCachePointCloudXYZ cache)
	{
		if ( Cores > 0 )
		{
			isRunning = true;
			tasks = new MegaCachePointTaskInfo[Cores];

			for ( int i = 0; i < Cores; i++ )
			{
				tasks[i] = new MegaCachePointTaskInfo();

				tasks[i].objcache = cache;
				tasks[i].name = "ThreadID " + i;
				tasks[i].pauseevent = new AutoResetEvent(false);
				tasks[i]._thread = new Thread(DoWork);
				tasks[i]._thread.Start(tasks[i]);
			}
		}
	}

	void DoWork(object info)
	{
		MegaCachePointTaskInfo inf = (MegaCachePointTaskInfo)info;

		while ( isRunning )
		{
			inf.pauseevent.WaitOne(Timeout.Infinite, false);

			//if ( inf.end > 0 )
				//PreLoad(inf.frame, inf.objcache);

			inf.end = 0;	// Done the job
		}
	}
#endif
}
