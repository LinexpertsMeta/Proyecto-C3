using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace MetaverseSample{
public class VideoManager : MonoBehaviour
{

    VideoPlayer _videoPlayer;

    bool isDone;
	
	public string video_url;

    public enum VideoState : int {Stopped,Playing,Paused,Ended};
   
   [Header("Video State")]
   public VideoState videoState = VideoState.Stopped;

   	[Header("Max Player Distance")]
	public float maxDistance = 1f;
	
	public bool OnPlayer;


    // Start is called before the first frame update
    void Start()
    {
      
      _videoPlayer = GetComponent<VideoPlayer>();
      	#if !UNITY_EDITOR 
        _videoPlayer.url = video_url; 
		#endif
      _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
      _videoPlayer.EnableAudioTrack(0,true);
      _videoPlayer.SetDirectAudioMute(0, true);
      _videoPlayer.Prepare();
      _videoPlayer.loopPointReached += CheckOver;
	 
	  
        
    }

    public void Play()
    {
        if(!IsPrepared()) return;
        _videoPlayer.Play();
        videoState = VideoState.Playing;
        Debug.Log("video is playing");

    }

    public void Restart()
    {
        if(!IsPrepared())return;
        _videoPlayer.time = 1;
        Pause();
       // Seek(0);
       // Play();


    }

     

    public void Pause()
    {
        if(!IsPlaying()) return;

        _videoPlayer.Pause();
        videoState = VideoState.Paused;

    }

   public bool IsPlaying()
   {
       return _videoPlayer.isPlaying;

   }
    public bool IsLooping()
   {
       return _videoPlayer.isLooping;
       
   }
    public bool IsPrepared()
   {
       return _videoPlayer.isPrepared;
       
   }

   public void Seek(float nTime)
   {
       if(!IsPrepared()) return;

       nTime = Mathf.Clamp(nTime,0,1);
       _videoPlayer.time = nTime *GetDuration();

   }
    public bool IsDone()
   {
       return isDone;
       
   }

   public double GetTime()
   {
        return _videoPlayer.time;

   }

   public ulong GetDuration()
   {
       return (ulong) (_videoPlayer.frameCount/_videoPlayer.frameRate);
   }

   public double GetNTime()
   {
       return GetTime()/GetDuration();
   }

   public void SetToFirstFrame()
   {
     if(!IsPrepared()) return;
     _videoPlayer.time = 1;
     _videoPlayer.Play();
     _videoPlayer.Pause();

   }

   void CheckOver(UnityEngine.Video.VideoPlayer vp)
   {
     Debug.Log("Video Is Over");
      if(!IsPrepared())return;
      Seek(0);
      Pause();
     videoState = VideoState.Stopped;


   }

   public string GetState()
	{
		switch (videoState)
		{
		    case VideoState.Stopped:
			return "Stopped";
			break;
	 	    case VideoState.Playing:
			return "Playing";
			break;
		     case VideoState.Paused:
			return "Paused";
			break;
		
		}//END_SWITCH

		return string.Empty;
	}

     void Update()
     {
       OnCustomCollider(); 
     }

    /// <summary>
	/// method for detecting the proximity of the player to the LootBox
	/// </summary>
	void OnCustomCollider()
	{
	
		if(NetworkManager.instance.localPlayer!=null )
		{
			Vector3 meToPlayer = transform.position - NetworkManager.instance.localPlayer.transform.position;

			Debug.Log("meToPlayer.sqrMagnitude: "+meToPlayer.sqrMagnitude);

			//check if player is near
			if (meToPlayer.sqrMagnitude < maxDistance) 
			{ 
				if (!OnPlayer) {
					OnPlayer = true;
                    _videoPlayer.SetDirectAudioMute(0, false);


				}
				
			  

			}

			else
			{
				if(OnPlayer)
				{
					OnPlayer = false;
                    _videoPlayer.SetDirectAudioMute(0, true);
				
				
				}
			}

		}
	}
}
}
