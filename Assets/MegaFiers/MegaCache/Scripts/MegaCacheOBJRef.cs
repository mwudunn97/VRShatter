
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class MegaCacheOBJRef : MonoBehaviour
{
	public MegaCacheOBJ	source;
	MeshFilter			mf;
	MeshRenderer		mr;
	public bool			usematerials		= true;
	public bool			animate				= false;
	public MegaCacheRepeatMode	loopmode	= MegaCacheRepeatMode.Loop;
	public float		fps					= 25.0f;
	public float		speed				= 1.0f;
	public float		time				= 0.0f;
	public float		looptime			= 0.0f;
	public int			frame				= 0;
	int					currentframe		= -1;
	Mesh				imagemesh;
	MeshCollider		meshCol;
	public bool			updatecollider		= false;

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=6228");
	}

	void Start()
	{
		mf = GetComponent<MeshFilter>();
		mr = GetComponent<MeshRenderer>();
	}

	public void SetSource(MegaCacheOBJ src)
	{
		source = src;

		if ( source )
			mr.sharedMaterials = src.GetComponent<Renderer>().sharedMaterials;

		currentframe = -1;
	}

	public int GetFrames()
	{
		int fc = 0;

		if ( source )
		{
			switch ( source.datasource )
			{
				case MegaCacheData.Mesh: fc = source.meshes.Count - 1; break;
				case MegaCacheData.File: fc = source.framecount - 1; break;
				case MegaCacheData.Image:
					if ( source.cacheimage && source.cacheimage.frames != null )
						fc = source.cacheimage.frames.Count - 1;
					break;
			}
		}

		return fc;
	}

	void Update()
	{
		if ( source )
		{
			int fc = 0;

			switch ( source.datasource )
			{
				case MegaCacheData.Mesh: fc = source.meshes.Count - 1; break;
				case MegaCacheData.File: fc = source.framecount - 1; break;
				case MegaCacheData.Image:
					if ( source.cacheimage && source.cacheimage.frames != null )
						fc = source.cacheimage.frames.Count - 1;
					break;
			}

			if ( fc > 0 )
			{
				if ( animate )
				{
					looptime = fc / fps;
					if ( Application.isPlaying )
						time += Time.deltaTime * speed;

					float at = time;

					switch ( loopmode )
					{
						case MegaCacheRepeatMode.Loop:
							at = Mathf.Repeat(time, Mathf.Abs(looptime));
							if ( looptime < 0.0f )
								at = looptime - at;
							break;
						case MegaCacheRepeatMode.PingPong: at = Mathf.PingPong(time, looptime); break;
						case MegaCacheRepeatMode.Clamp: at = Mathf.Clamp(time, 0.0f, looptime); break;
					}

					frame = (int)((at / looptime) * fc);
				}

				frame = Mathf.Clamp(frame, 0, fc);

				if ( frame != currentframe )
				{
					currentframe = frame;
					if ( source.datasource == MegaCacheData.Image && source.cacheimage )
					{
						if ( imagemesh == null )
							imagemesh = new Mesh();

						if ( mf.sharedMesh != imagemesh )
							mf.sharedMesh = imagemesh;

						source.cacheimage.GetMeshRef(imagemesh, frame, source);
					}

					if ( source.datasource == MegaCacheData.File )
					{
						if ( imagemesh == null )
							imagemesh = new Mesh();

						if ( mf.sharedMesh != imagemesh )
							mf.sharedMesh = imagemesh;

						source.GetFrameRef(frame, imagemesh);
					}

					if ( source.datasource == MegaCacheData.Mesh )
					{
						if ( mf && source.meshes.Count > 0 )
						{
							if ( mf.sharedMesh != source.meshes[frame] )
								mf.sharedMesh = source.meshes[frame];
						}
					}

					if ( updatecollider )
					{
						if ( meshCol == null )
							meshCol = GetComponent<MeshCollider>();

						if ( meshCol != null )
						{
							meshCol.sharedMesh = null;
							meshCol.sharedMesh = mf.sharedMesh;
						}
					}
				}
			}
		}
	}
}
