using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShufflePlaylist : MonoBehaviour
{
    
    
    [FMODUnity.EventRef]
    public string[] fmodPaths;

    private FMOD.Studio.EventInstance instance;

    private string artistName, songName;
    private int timelineInfo;
   

   [SerializeField]
    private TMPro.TMP_Text Artist;
    [SerializeField]
    private TMPro.TMP_Text Song;
    [SerializeField]
    private TMPro.TMP_Text Time;

    FMOD.Studio.PLAYBACK_STATE PlaybackState(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE pS;
        instance.getPlaybackState(out pS);
        return pS;
    }
    void Start()
    {
        ShuffleMusic(fmodPaths);

    }

    private void Update()
    {
        DisplayMusic();
     
    }

    void ShuffleMusic(string[] a)
    {
        for (int i = a.Length - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i);

            string temp = a[i];

            a[i] = a[rnd];
            a[rnd] = temp;
        }

        for (int i = 0; i < a.Length; i++)
        {
            Debug.Log(a[i]);
        }

        StartCoroutine(PlayMusic(a));
    }

    IEnumerator PlayMusic(string[] a)
    {
        for (int i = 0; i < a.Length; i++)
        {
            instance = FMODUnity.RuntimeManager.CreateInstance(a[i]);

            if (PlaybackState(instance) != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                FMOD.Studio.EventDescription eD;
                instance.getDescription(out eD);

                int userPropertyCount;
                eD.getUserPropertyCount(out userPropertyCount);

                FMOD.Studio.USER_PROPERTY[] userProperties = new FMOD.Studio.USER_PROPERTY[userPropertyCount];

                for (int j = 0; j < userPropertyCount; j++)
                {
                    eD.getUserPropertyByIndex(j, out userProperties[j]);
                }

                artistName = userProperties[0].stringValue();
                songName = userProperties[1].stringValue();
                


                Debug.Log("Artist: " + artistName);
                Debug.Log("Song: " + songName);

                instance.start();
                instance.release();

                while (PlaybackState(instance) != FMOD.Studio.PLAYBACK_STATE.STOPPED)
                {
                    yield return null;
                }

                instance.clearHandle();

                while (instance.isValid())
                {
                    yield return null;
                }
            }
        }
        ShuffleMusic(fmodPaths);
        
    }

    void DisplayMusic()
    {
        
            Artist.SetText("Artist: " + artistName);
            Song.SetText("Song: " + songName);
            
        //timer
        instance.getTimelinePosition(out timelineInfo);
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(timelineInfo);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        Time.SetText("Time:"+timeText);
    }
}
