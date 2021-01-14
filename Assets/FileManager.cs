using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Video;

public class FileManager : MonoBehaviour
{
    public string photo_path;
    public string video_path;
    public GameObject inputpanel;
    public RawImage image;
    public GameObject url_display;
    public VideoPlayer videoPlayer;
    public GameObject url_video;
    public GameObject back_button;

    //Variables for switching videos
    private GameObject[] objs;
    private List<GameObject> needs_back;
    private long location;
    private string url_old_video;
    private Vector3 camera_pos;

    public void OpenExplorer()
    {
        photo_path = EditorUtility.OpenFilePanel("Get photo.", "", "png");
        GetImage();
        url_display.GetComponent<Text>().text = photo_path;
    }

    public void ChangeVideo()
    {
        video_path = EditorUtility.OpenFilePanel("Change photo.", "", "mp4");
        //Record the old video;
        videoPlayer.Stop();
        location = videoPlayer.frame;
        url_old_video = videoPlayer.url;
        //Get new video and play
        url_video.GetComponent<Text>().text = video_path;
    }

    public void PlaySecondVideo()
    {
        videoPlayer.url = video_path;
        videoPlayer.Play();
        camera_pos = Camera.main.transform.position;
        //Deactive all buttons from the previous video
        objs = GameObject.FindGameObjectsWithTag("Trigger");
        needs_back = new List<GameObject>();
        foreach (GameObject button in objs)
        {
            if (button.activeSelf)
            {
                needs_back.Add(button);
            }
            button.SetActive(false);
        }
        inputpanel.SetActive(false);
        back_button.SetActive(true);
    }

    public void BacktoPrevVideo()
    {
        videoPlayer.url = url_old_video;
        videoPlayer.frame = location;
        Camera.main.transform.rotation = Quaternion.Euler(camera_pos);
        foreach (GameObject button in needs_back)
        {
            button.SetActive(true);
        }
        videoPlayer.Play();
        inputpanel.SetActive(true);
        back_button.SetActive(false);
    }

    void GetImage()
    {
        if(photo_path != null)
        {
            UpdateImage();
        }
    }

    void UpdateImage()
    {
        WWW www = new WWW("file:///" + photo_path);
        image.texture = www.texture;
    }

    public string getPhoto()
    {
        return photo_path;
    }
}
