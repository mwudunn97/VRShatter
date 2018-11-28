
#if UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018
#else
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[AddComponentMenu("MegaCache/Particle Pro Legacy")]
public class MegaCacheParticleProLegacy : MegaCacheParticle
{
	public ParticleEmitter	particle;
	Particle[]				particles;
	float					emittime = 0.0f;

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=6224");
	}

	void Start()
	{
		if ( particle == null )
			particle = GetComponent<ParticleEmitter>();

		particles = new Particle[maxparticles];

		if ( prewarm && image )
		{
			particle.Emit((int)(emitrate * 5.0f));

			for ( float i = 0.0f; i < 5.0f; i += 0.01f )
			{
				if ( image.optimized )
					UpdateParticlesOpt(0.01f);
				else
					UpdateParticles(0.01f);
				particle.Simulate(0.01f);
			}
		}
	}

	void LateUpdate()
	{
		if ( particle && image )
		{
			if ( image.optimized )
				UpdateParticlesOpt(Time.deltaTime);
			else
				UpdateParticles(Time.deltaTime);
		}
	}

	void InitStates()
	{
		if ( image )
		{
			if ( image.optimized )
			{
				if ( states.Count != image.optparticles.Count )
				{
					states.Clear();
					for ( int i = 0; i < image.optparticles.Count; i++ )
						states.Add(new MegaCacheParticleState());
				}
			}
			else
			{
				if ( states.Count != image.particles.Count )
				{
					states.Clear();
					for ( int i = 0; i < image.particles.Count; i++ )
						states.Add(new MegaCacheParticleState());
				}
			}
		}
	}

	void UpdateParticles(float dt)
	{
		if ( dt > 0.01f )
			dt = 0.01f;

		if ( particle && image && image.particles.Count > 0 )
		{
			InitStates();

			if ( useemit )
			{
				emittime += dt * emitrate;

				int ecount = (int)emittime;
				if ( ecount > 0 )
				{
					particle.Emit(ecount);
					emittime -= 1.0f;
				}
			}

			int count = particle.particleCount;
			particles = particle.particles;

			int ix = 0;

			Matrix4x4 tm = transform.localToWorldMatrix;

			removeparticles.Clear();

			float pscl = scaleall * sizescale * particle.minSize;

			for ( int i = 0; i < activeparticles.Count; i++ )
			{
				MegaCacheParticleHistory ph = image.particles[activeparticles[i]];
				MegaCacheParticleState ps = states[activeparticles[i]];

				ps.time += Time.deltaTime * speed * ps.locspeed;

				if ( ps.time >= ph.life || ps.time < 0.0f )
					removeparticles.Add(i);
				else
				{
					float alpha = ps.time / ph.life;

					float fn = alpha * (ph.positions.Count - 1);
					framenum = (int)fn;
					float subalpha = fn - framenum;

					Vector3 lpos = Vector3.Lerp(ph.positions[framenum], ph.positions[framenum + 1], subalpha) * scaleall * ps.locscale;

					particles[ix].position = ps.tm.MultiplyPoint3x4(lpos);
					particles[ix].energy = ph.life - ps.time;
					particles[ix].startEnergy = ph.life;
					particles[ix].size = Mathf.Lerp(ph.scale[framenum], ph.scale[framenum + 1], subalpha) * pscl * ps.locscale;
					//particles[ix].rotation = Mathf.Lerp(ph.rots[framenum][(int)axis], ph.rots[framenum + 1][(int)axis], subalpha);
					particles[ix].rotation = Mathf.Lerp(ph.rots[framenum][(int)axis], ph.rots[framenum + 1][(int)axis], subalpha);

					ix++;
				}
			}

			for ( int i = removeparticles.Count - 1; i >= 0; i-- )
				activeparticles.RemoveAt(removeparticles[i]);

			if ( activeparticles.Count < image.particles.Count )
			{
				if ( count > activeparticles.Count )
				{
					int emit = count - activeparticles.Count;

					for ( int i = 0; i < emit; i++ )
					{
						MegaCacheParticleHistory ph = image.particles[particleindex];
						MegaCacheParticleState ps = states[particleindex];

						int px = 0;
						if ( emitspeed * speed >= 0.0f )
						{
							ps.time = 0.0f;
							particles[ix].energy = ph.life;
						}
						else
						{
							ps.time = ph.life;
							particles[ix].energy = 0.0f;
							px = ph.positions.Count - 1;
						}

						ps.locscale = emitscale;
						ps.locspeed = emitspeed;
						ps.tm = tm;

						activeparticles.Add(particleindex++);

						particles[ix].position = tm.MultiplyPoint3x4(ph.positions[px] * scaleall);
						particles[ix].startEnergy = ph.life;
						//particles[ix].rotation = ph.rots[px][(int)axis] * Mathf.Rad2Deg;
						particles[ix].rotation = ph.rots[i][(int)axis];	// * Mathf.Rad2Deg;

						particles[ix].size = 0.0f;

						if ( particleindex >= image.particles.Count )
							particleindex = 0;

						ix++;
					}
				}
			}
			else
			{
				//Debug.Log("No available particles");
			}

			particle.particles = particles;
		}
	}

	// Should be in another class
	void UpdateParticlesOpt(float dt)
	{
		if ( dt > 0.01f )
			dt = 0.01f;

		if ( particle && image && image.optparticles.Count > 0 )
		{
			InitStates();

			if ( useemit )
			{
				emittime += dt * emitrate;

				int ecount = (int)emittime;
				if ( ecount > 0 )
				{
					particle.Emit(ecount);
					emittime -= 1.0f;
				}
			}

			int count = particle.particleCount;
			particles = particle.particles;

			int ix = 0;

			Matrix4x4 tm = transform.localToWorldMatrix;

			removeparticles.Clear();

			float pscl = scaleall * sizescale * particle.minSize;

			for ( int i = 0; i < activeparticles.Count; i++ )
			{
				MegaCacheParticleHistoryOpt ph = image.optparticles[activeparticles[i]];
				MegaCacheParticleState ps = states[activeparticles[i]];

				ps.time += Time.deltaTime * speed * ps.locspeed;

				if ( ps.time >= ph.life || ps.time < 0.0f )
					removeparticles.Add(i);
				else
				{
					float alpha = ps.time / ph.life;

					float fn = alpha * (ph.count - 1);
					framenum = (int)fn;
					float subalpha = fn - framenum;

					Vector3 pos = DecodeV3(ph.pos, framenum * 6, ph.posmin, ph.possize);
					Vector3 pos1 = DecodeV3(ph.pos, (framenum + 1) * 6, ph.posmin, ph.possize);
					float scl = DecodeFloat(ph.scale, framenum, ph.scalemin, ph.scalesize);
					float scl1 = DecodeFloat(ph.scale, framenum + 1, ph.scalemin, ph.scalesize);
					Vector3 rot = DecodeV3b(ph.rots, framenum * 3, ph.rotmin, ph.rotsize);
					Vector3 rot1 = DecodeV3b(ph.rots, (framenum + 1) * 3, ph.rotmin, ph.rotsize);

					Vector3 lpos = Vector3.Lerp(pos, pos1, subalpha) * scaleall * ps.locscale;

					particles[ix].position = ps.tm.MultiplyPoint3x4(lpos);
					particles[ix].energy = ph.life - ps.time;
					particles[ix].startEnergy = ph.life;
					particles[ix].size = Mathf.Lerp(scl, scl1, subalpha) * pscl * ps.locscale;
					//particles[ix].rotation = Mathf.Lerp(rot[(int)axis], rot1[(int)axis], subalpha) * Mathf.Rad2Deg;
					particles[ix].rotation = Mathf.Lerp(rot[(int)axis], rot1[(int)axis], subalpha);

					ix++;
				}
			}

			for ( int i = removeparticles.Count - 1; i >= 0; i-- )
				activeparticles.RemoveAt(removeparticles[i]);

			if ( activeparticles.Count < image.optparticles.Count )
			{
				if ( count > activeparticles.Count )
				{
					int emit = count - activeparticles.Count;

					for ( int i = 0; i < emit; i++ )
					{
						MegaCacheParticleHistoryOpt ph = image.optparticles[particleindex];
						MegaCacheParticleState ps = states[particleindex];

						int px = 0;
						if ( emitspeed * speed >= 0.0f )
						{
							ps.time = 0.0f;
							particles[ix].energy = ph.life;
						}
						else
						{
							ps.time = ph.life;
							particles[ix].energy = 0.0f;
							px = ph.pos.Length - 1;
						}

						ps.locscale = emitscale;
						ps.locspeed = emitspeed;
						ps.tm = tm;

						activeparticles.Add(particleindex++);

						Vector3 pos = DecodeV3(ph.pos, px * 6, ph.posmin, ph.possize);
						Vector3 rot = DecodeV3b(ph.rots, px * 3, ph.rotmin, ph.rotsize);
						particles[ix].position = tm.MultiplyPoint3x4(pos * scaleall);
						particles[ix].startEnergy = ph.life;
						//particles[ix].rotation = rot[(int)axis] * Mathf.Rad2Deg;
						particles[ix].rotation = rot[(int)axis];	// * Mathf.Rad2Deg;

						particles[ix].size = 0.0f;

						if ( particleindex >= image.optparticles.Count )
							particleindex = 0;

						ix++;
					}
				}
			}
			else
			{
				//Debug.Log("No available particles");
			}

			particle.particles = particles;
		}
	}
}
#endif