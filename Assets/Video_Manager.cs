using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEditor;
//using System.Windows.Forms;


public class Video_Manager : MonoBehaviour
{
    public GameObject chose_button;
    public VideoPlayer videoPlayer;
    private string url;

    public void getVideo()
    {
        url = EditorUtility.OpenFilePanel("get photo.", "", "mp4");
        if (url != null)
        {
            videoPlayer.url = url;
        }
        videoPlayer.Play();
        chose_button.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        if (url != null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
    }

    
}
