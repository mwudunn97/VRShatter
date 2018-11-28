
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class MegaCacheParticleHistory
{
	public int				id;
	public float			life;
	public List<Vector3>	positions	= new List<Vector3>();
	public List<Vector3>	rots		= new List<Vector3>();
	public List<Vector3>	vels		= new List<Vector3>();
	public List<float>		scale		= new List<float>();
	public List<float>		spin		= new List<float>();
}

[System.Serializable]
public class MegaCacheParticleHistoryOpt
{
	public int				id;
	public int				count;
	public float			life;
	public Vector3			possize;
	public Vector3			posmin;
	public Vector3			rotsize;
	public Vector3			rotmin;
	public Vector3			velsize;
	public Vector3			velmin;
	public float			scalesize;
	public float			scalemin;
	public float			spinsize;
	public float			spinmin;
	public byte[]			pos;
	public byte[]			rots;
	public byte[]			vels;
	public byte[]			scale;
	public byte[]			spin;
}

[System.Serializable]
public class MegaCacheParticleState
{
	public Matrix4x4		tm;
	public float			time = 0.0f;
	public float			locscale;
	public float			locspeed;
}

public class MegaCacheParticleImage : ScriptableObject
{
	public bool									optimized		= false;
	public List<MegaCacheParticleHistory>		particles		= new List<MegaCacheParticleHistory>();
	public List<MegaCacheParticleHistoryOpt>	optparticles	= new List<MegaCacheParticleHistoryOpt>();
	public int									frames			= 0;
	public int									maxparticles	= 0;

	public int CalcMemory()
	{
		int mem = 0;

		int skip = 1;

		if ( optimized )
		{
			for ( int i = 0; i < optparticles.Count; i++ )
			{
				MegaCacheParticleHistoryOpt ph = optparticles[i];

				mem += ph.pos.Length;
				mem += ph.vels.Length;
				mem += ph.rots.Length;
				mem += ph.scale.Length;
				mem += ph.spin.Length;
			}
		}
		else
		{
			for ( int i = 0; i < particles.Count; i++ )
			{
				MegaCacheParticleHistory ph = particles[i];

				mem += ph.positions.Count * 12;
				mem += ph.vels.Count * 12;
				mem += ph.rots.Count * 12;
				mem += ph.scale.Count * 4;
				mem += ph.spin.Count * 4;
			}
		}

		return mem / skip;
	}

#if false
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
#endif

	public void ReadData(BinaryReader br)
	{
		int version = br.ReadInt32();

		if ( version < 2 )
		{
			int pcount = br.ReadInt32();

			bool optimize = br.ReadBoolean();
			bool hasrot = br.ReadBoolean();
			bool hasvel = br.ReadBoolean();
			bool hasscale = br.ReadBoolean();
			bool hasspin = br.ReadBoolean();

			if ( version == 1 )
			{
				frames = br.ReadInt32();
				maxparticles = br.ReadInt32();
			}

			optimized = optimize;

			if ( optimize )
			{
				//Debug.Log("v " + version);
				//Debug.Log("frames " + frames);
				//Debug.Log("max " + maxparticles);
				for ( int i = 0; i < pcount; i++ )
				{
					int count = br.ReadInt32();

					MegaCacheParticleHistoryOpt ph = new MegaCacheParticleHistoryOpt();

					ph.count = count;
					ph.life = br.ReadSingle();
					ph.id = br.ReadInt32();

					// load pos
					ph.posmin.x = br.ReadSingle();
					ph.posmin.y = br.ReadSingle();
					ph.posmin.z = br.ReadSingle();

					ph.possize.x = br.ReadSingle();
					ph.possize.y = br.ReadSingle();
					ph.possize.z = br.ReadSingle();

					ph.possize *= 1.0f / 65535.0f;

					ph.pos = br.ReadBytes(count * 6);

					if ( hasvel )
					{
						ph.velmin.x = br.ReadSingle();
						ph.velmin.y = br.ReadSingle();
						ph.velmin.z = br.ReadSingle();

						ph.velsize.x = br.ReadSingle();
						ph.velsize.y = br.ReadSingle();
						ph.velsize.z = br.ReadSingle();

						ph.vels = br.ReadBytes(count * 3);
					}

					if ( hasscale )
					{
						ph.scalemin = br.ReadSingle();
						ph.scalesize = br.ReadSingle();

						ph.scalesize *= 1.0f / 255.0f;

						ph.scale = br.ReadBytes(count);
					}

					if ( hasrot )
					{
						ph.rotmin.x = br.ReadSingle();
						ph.rotmin.y = br.ReadSingle();
						ph.rotmin.z = br.ReadSingle();

						ph.rotsize.x = br.ReadSingle();
						ph.rotsize.y = br.ReadSingle();
						ph.rotsize.z = br.ReadSingle();

						ph.rotsize *= 1.0f / 255.0f;

						ph.rots = br.ReadBytes(count * 3);
					}

					if ( hasspin )
					{
						ph.spinmin = br.ReadSingle();
						ph.spinsize = br.ReadSingle();

						ph.spinsize *= 1.0f / 255.0f;

						ph.spin = br.ReadBytes(count);
					}

					optparticles.Add(ph);
				}
			}
			else
			{
				for ( int i = 0; i < pcount; i++ )
				{
					int count = br.ReadInt32();

					MegaCacheParticleHistory ph = new MegaCacheParticleHistory();

					Vector3 p3 = Vector3.zero;

					// load pos
					for ( int v = 0; v < count; v++ )
					{
						p3.x = br.ReadSingle();
						p3.y = br.ReadSingle();
						p3.z = br.ReadSingle();

						ph.positions.Add(p3);
					}

					if ( hasvel )
					{
						for ( int v = 0; v < count; v++ )
						{
							p3.x = br.ReadSingle();
							p3.y = br.ReadSingle();
							p3.z = br.ReadSingle();

							ph.vels.Add(p3);
						}
					}

					if ( hasscale )
					{
						for ( int v = 0; v < count; v++ )
							ph.scale.Add(br.ReadSingle());
					}

					if ( hasrot )
					{
						for ( int v = 0; v < count; v++ )
						{
							p3.x = br.ReadSingle();
							p3.y = br.ReadSingle();
							p3.z = br.ReadSingle();

							ph.rots.Add(p3);
						}
					}

					if ( hasspin )
					{
						for ( int v = 0; v < count; v++ )
							ph.spin.Add(br.ReadSingle());
					}

					particles.Add(ph);
				}
			}
		}
	}
}
