
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[AddComponentMenu("MegaCache/Point Cloud")]
public class MegaCachePointCloud : MonoBehaviour
{
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

	public bool					showdataimport = false;
	public int					firstframe		= 0;
	public int					lastframe		= 0;
	public int					skip			= 0;

	public int					decformat		= 0;
	//public string				namesplit		= "";
	public bool					yupimport		= false;

	public static Vector3 AdjustYUp(Vector3 p)
	{
		float y = p.y;
		p.y = p.z;
		p.z = y;
		p.x = -p.x;
		return p;
	}

#if false
	public static void LoadFile(MegaCachePointCloud mod, string filename)
	{
		StreamReader stream = File.OpenText(filename);
		string entireText = stream.ReadToEnd();
		stream.Close();

		char[] splitIdentifier = { ',' };

		//StringReader reader = new StringReader(entireText);
		MegaCacheParticle.offset = 0;
		List<Vector3> pos = new List<Vector3>();
		List<float> inten = new List<float>();

		Vector3 p = Vector3.zero;
		float	it = 0.0f;

		MegaCachePCFrame frame = new MegaCachePCFrame();

		while ( true )
		{
			//string ps = reader.ReadLine();
			string ps = MegaCacheParticle.ReadLine(entireText);
			if ( ps.Length == 0 )
				break;

			string[] brokenString = ps.Split(splitIdentifier, 50);

			p.x = float.Parse(brokenString[0]);
			p.y = float.Parse(brokenString[1]);
			p.z = float.Parse(brokenString[2]);

			if ( mod.yupimport )
			{
				p = AdjustYUp(p);
			}

			it = float.Parse(brokenString[3]);
			pos.Add(p);
			inten.Add(it);
		}

		frame.points = pos.ToArray();
		frame.intensity = inten.ToArray();

		mod.image.frames.Add(frame);
	}
#endif

	public MegaCachePCFrame LoadFrame(string filename)
	{
		StreamReader stream = File.OpenText(filename);
		string entireText = stream.ReadToEnd();
		stream.Close();

		char[] splitIdentifier = { ',' };

		//StringReader reader = new StringReader(entireText);
		MegaCacheParticle.offset = 0;
		List<Vector3> pos = new List<Vector3>();
		List<float> inten = new List<float>();

		Vector3 p = Vector3.zero;
		float it = 0.0f;

		MegaCachePCFrame frame = new MegaCachePCFrame();

		while ( true )
		{
			//string ps = reader.ReadLine();
			string ps = MegaCacheParticle.ReadLine(entireText);
			if ( ps == null || ps.Length == 0 )
				break;

			string[] brokenString = ps.Split(splitIdentifier, 50);

			p.x = float.Parse(brokenString[0]);
			p.y = float.Parse(brokenString[1]);
			p.z = float.Parse(brokenString[2]);

			if ( yupimport )
			{
				p = AdjustYUp(p);
			}

			it = float.Parse(brokenString[3]) / 255.0f;
			pos.Add(p * importscale);
			inten.Add(it * sizescale);
		}

		frame.points = pos.ToArray();
		frame.intensity = inten.ToArray();

		//mod.image.frames.Add(frame);
		return frame;
	}

	public MegaCachePCFrame LoadFrame(string filename, int frame)
	{
		MegaCachePCFrame fr = null;

		string dir = Path.GetDirectoryName(filename);
		string file = Path.GetFileNameWithoutExtension(filename);

		file = MegaCacheUtils.MakeFileName(file, ref decformat);

		//if ( file.Length > 0 )
		{
			string newfname = dir + "/" + file + frame.ToString("D" + decformat) + ".csv";
			fr = LoadFrame(newfname);
		}

		return fr;
	}
#if false
	public MegaCachePCFrame LoadFrame(string filename, int frame)
	{
		MegaCachePCFrame fr = null;

		char[] splits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		string dir = Path.GetDirectoryName(filename);
		string file = Path.GetFileNameWithoutExtension(filename);

		string[] names;

		if ( namesplit.Length > 0 )
		{
			names = file.Split(namesplit[0]);
			names[0] += namesplit[0];
		}
		else
			names = file.Split(splits);

		if ( names.Length > 0 )
		{
			string newfname = dir + "/" + names[0] + frame.ToString("D" + decformat) + ".csv";
			fr = LoadFrame(newfname);
		}

		return fr;
	}
#endif

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=6222");
	}

	void Start()
	{
		if ( particle == null )
			particle = GetComponent<ParticleSystem>();

		if ( image )
			particles = new ParticleSystem.Particle[image.maxpoints];

			particle.Emit(image.maxpoints);

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
	//public Color color = Color.white;

	void UpdateParticles(float dt)
	{
		if ( dt > 0.01f )
			dt = 0.01f;

		if ( particle && image )
		{
			framenum = Mathf.Clamp(framenum, 0, image.frames.Count - 1);
			MegaCachePCFrame frame = image.frames[framenum];

			// Do we need this
			particle.GetParticles(particles);

			//int ix = 0;

			//Matrix4x4 tm = transform.localToWorldMatrix;

			//Color col = color;

			for ( int i = 0; i < frame.points.Length; i++ )
			{
				particles[i].position = frame.points[i] * playscale;
#if UNITY_2017 || UNITY_2018
				particles[i].remainingLifetime = 1.0f;
#else
				particles[i].lifetime = 1.0f;	//ph.life - ps.time;
#endif
				particles[i].startLifetime = 1.0f;	//ph.life;
#if UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
				particles[i].startSize = frame.intensity[i] * playsize * playscale;
#else
				particles[i].size = frame.intensity[i] * playsize * playscale;
#endif
				//col.a = frame.intensity[i];
				//particles[i].color = col;
			}

			particle.SetParticles(particles, frame.points.Length);
		}
	}
}
