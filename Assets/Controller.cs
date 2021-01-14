using System.Collections;
using System.Runtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Controller : MonoBehaviour
{
    //Panels
    public VideoPlayer videoPlayer;
    public GameObject window;
    //Input fields
    public GameObject nameinputField;
    public GameObject textInputField;
    //Buttons
    public GameObject hotspot;
    public GameObject back_button;
    //displays
    public GameObject id_display;
    public GameObject start_time;
    public GameObject photoDisplay;
    public GameObject videoDisplay;


    public List<string> hotspotjsons = new List<string>();

    //Private variables 
    private Hashtable all_hotspots;

    // Start is called before the first frame update
    void Start()
    {
        all_hotspots = new Hashtable();
        window.SetActive(false);
        back_button.SetActive(false);
        //load();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,20));
            //Vector4 position = new Vector4(worldPosition.x, worldPosition.y, worldPosition.z, (float)videoPlayer.time);
            double start_time = videoPlayer.time;
            GameObject a = Instantiate(hotspot, worldPosition, transform.rotation);
            a.name = a.GetInstanceID().ToString();
            a.SetActive(true);
            all_hotspots.Add(a.name, new Hotspot(a, start_time,videoPlayer.length));
            addJson(a.name, worldPosition);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 100.0f)){
                if(hit.transform.gameObject.tag == "Trigger")
                {
                    window.SetActive(true);
                    string current_id = hit.transform.gameObject.GetInstanceID().ToString();
                    id_display.GetComponent<Text>().text = current_id;
                    Hotspot h = (Hotspot)all_hotspots[current_id];
                    start_time.GetComponent<Text>().text = h.getStart().ToString();
                    videoPlayer.Pause();
                }
            }
        }

        double current_time = videoPlayer.time;
        foreach(DictionaryEntry entry in all_hotspots)
        {
            Hotspot h = (Hotspot)entry.Value;
            if(h.getStart() >= current_time || h.getEnd() <= current_time)
            {
                h.getHotspot().SetActive(false);
            }
            else
            {
                h.getHotspot().SetActive(true);
            }
        }
    }

    public void saveJson()
    {
        save();
    }

    public void closeWindow()
    {
        window.SetActive(false);
        videoPlayer.Play();
    }

    public void SetEndtime()
    {
        Hotspot a = (Hotspot)all_hotspots[id_display.GetComponent<Text>().text];
        a.SetEndTime(videoPlayer.time);
    }

    public void delete_hotspot()
    {
        string id = id_display.GetComponent<Text>().text;
        Hotspot h = (Hotspot)all_hotspots[id];
        GameObject a = h.getHotspot();
        all_hotspots.Remove(id);
        Destroy(a);
        window.SetActive(false);
        videoPlayer.Play();
    }

    public void ClickSave()
    {
        string name = nameinputField.GetComponent<InputField>().text;
        string text = textInputField.GetComponent<InputField>().text;
        string url_photo = photoDisplay.GetComponent<Text>().text;
        string url_video = videoDisplay.GetComponent<Text>().text;

        string current_id = id_display.GetComponent<Text>().text;
        Hotspot h = (Hotspot)all_hotspots[current_id];
        h.SetMoreInfo(name, text, url_photo, url_video);

        Debug.Log(name);
        Debug.Log(text);
        Debug.Log(url_photo);
        Debug.Log(url_video);

        nameinputField.GetComponent<InputField>().text = "Hotspot Name";
        textInputField.GetComponent<InputField>().text = "Description";
        photoDisplay.GetComponent<Text>().text = "Photo path here.";
        videoDisplay.GetComponent<Text>().text = "Video path here.";
    }

    class Hotspot
    {
        GameObject hotspot;
        double start_time;
        double end_time;
        string name;
        string text;
        string url_photo;
        string url_video;

        public Hotspot(GameObject a, double start, double end)
        {
            this.hotspot = a;
            this.start_time = start;
            this.end_time = end;
        }

        public double getStart()
        {
            return start_time;
        }

        public double getEnd()
        {
            return end_time;
        }

        public GameObject getHotspot()
        {
            return hotspot;
        }

        public void SetEndTime(double endtime)
        {
            this.end_time = endtime;
        }

        public void SetMoreInfo(string name, string text, string url_photo, string url_video)
        {
            if(!name.Equals("Hotspot Name")) { this.name = name;}
            if(!text.Equals("Description")) { this.text = text; }
            if (!url_photo.Equals("Photo path here")) { this.url_photo = url_photo;}
            if (!url_video.Equals("Video path here.")) { this.url_video = url_video; }
                
        }

    }
    //saving stuff
    public void addJson(string id, Vector3 position)
    {
        HotspotData hotspotdata = new HotspotData();
        hotspotdata.name = id.ToString();
        hotspotdata.position = position;
        string json = JsonUtility.ToJson(hotspotdata);
        hotspotjsons.Add(json);
    }
    public void save()
    {
        Debug.Log("Saving");
        string[] jsons = new string[hotspotjsons.Count];
        jsons = hotspotjsons.ToArray();
        HotspotDatas hotspotdatas = new HotspotDatas() { hotspotdatas = jsons };
        string json = JsonUtility.ToJson(hotspotdatas);
        //Debug.Log(json);
        File.WriteAllText(Application.dataPath + "/hotspots.json", json);
    }
    public void load()
    {
        string hotspotjsons = File.ReadAllText(Application.dataPath + "/hotspots.json");
        HotspotDatas hotspotdatas = JsonUtility.FromJson<HotspotDatas>(hotspotjsons);
        foreach (string json in hotspotdatas.hotspotdatas)
        {
            HotspotData hotspotdata = JsonUtility.FromJson<HotspotData>(json);
            GameObject a = Instantiate(hotspot, hotspotdata.position, transform.rotation);
            a.name = hotspotdata.name;
            a.SetActive(true);
            addJson(hotspotdata.name, hotspotdata.position);
        }
    }
    public class HotspotData
    {
        public string name;
        public Vector3 position;
    }
    public class HotspotDatas
    {
        public string[] hotspotdatas;
    }
}

