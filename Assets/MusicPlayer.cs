using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using FMODUnity;
using UnityEngine.UI;
public class MusicPlayer : MonoBehaviour
{
    public GameObject modui, panel;

    public List<GameObject> entries = new List<GameObject>();
    public string[] songlist;
    FMOD.RESULT result;
    FMOD.Sound soundObject;
    FMOD.System system;
    FMOD.Channel channel;
    FMOD.ChannelGroup channelGroup;
    //public List<AudioClip> music = new List<AudioClip>();
    public int currentSong = 0;
    public bool playing;
    float volume = 0.5f;
    bool abort = false;
    //public AudioSource player;

    void Awake()
    {
        InitializeUI();
        GetSongs();
        //gameObject.AddComponent<MusicPlayer>();
    }

    void GetSongs()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "Jukebox");
            songlist = Directory.GetFiles(path, "*.ogg");
            foreach (string songs in songlist)
            {
                print("Music Player", "Found " + songs);
            }
        }

        catch (Exception e)
        {
            print("Error", e.Message);
            print("Music Player", "aborting");
            abort = true;
        }
    }
    void InitializeUI()
    {
        modui = new GameObject("Modloader");
        panel = new GameObject("Panel");
        RectTransform mrt = modui.AddComponent<RectTransform>();
        RectTransform prt = panel.AddComponent<RectTransform>();
        modui.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = modui.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(2560, 1080);
        VerticalLayoutGroup pvlg = panel.AddComponent<VerticalLayoutGroup>();
        pvlg.childForceExpandHeight = false;
        pvlg.spacing = 10f;
        Image ip = panel.AddComponent<Image>();
        ip.color = new Color(0.12f, 0.18f, 0.24f, 0.33f);

        modui.AddComponent<GraphicRaycaster>();

        prt.SetParent(mrt);
        prt.anchoredPosition = new Vector2(930, -398);
        prt.sizeDelta = new Vector2(700, 300);

        //entries.Add(newEntry(entries.Count, "", ""));

        entries.Add(newEntry(entries.Count, "TinyMod ", "Loaded!"));


    }

    IEnumerator delmesssage()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (entries.Count != 0)
            {
                Destroy(entries[0]);
                entries.RemoveAt(0);
            }

        }
    }
    GameObject newEntry(int index, string Name, string Description)
    {
        GameObject entry = new GameObject("entry " + index);
        RectTransform rt = entry.AddComponent<RectTransform>();
        rt.SetParent(panel.GetComponent<RectTransform>());
        //rt.localPosition = new Vector3(0, index * -50f, 0);
        rt.sizeDelta = new Vector2(700, 100);
        Text txt = entry.AddComponent<Text>();
        txt.font = Font.CreateDynamicFontFromOSFont("Arial", 20);
        txt.fontSize = 15;

        txt.text = Name + " " + Description;
        return entry;
    }
    void print(string Title, string description)
    {
        entries.Add(newEntry(entries.Count, Title + ": ", description));
    }
    void Start()
    {
        StartCoroutine(delmesssage());
        system = RuntimeManager.CoreSystem;
        result = system.createChannelGroup(null, out channelGroup);
        channel.setChannelGroup(channelGroup);
        //result = system.createSound(Path.Combine(Application.dataPath, "Jukebox", "song1.mp3"), FMOD.MODE.DEFAULT, out sound1);
        //result = system.playSound(sound1, channelGroup, false, out channel);

    }
    void loadPreviousTrack()
    {
        if (songlist.Length == 0) return;
        if (currentSong - 1 >= 0)
        {
            currentSong--;
        }
        else
        {
            currentSong = songlist.Length - 1;
        }
        loadSong(currentSong);
    }
    void loadNextTrack()
    {
        if (songlist.Length == 0) return;
        if (currentSong + 1 < songlist.Length)
        {
            currentSong++;
        }
        else
        {
            currentSong = 0;
        }
        loadSong(currentSong);
    }

    void loadSong(int index)
    {
        result = soundObject.release();
        print("Music Player", string.Format("Playing {0}!", Path.GetFileName(songlist[index])));
        result = system.createSound(songlist[index], FMOD.MODE.DEFAULT, out soundObject);
        result = system.playSound(soundObject, channelGroup, false, out channel);
        channel.setVolume(volume);


    }
    void PlayPause()
    {
        bool state;
        channel.getPaused(out state);
        channel.setPaused(!state);
        if (!state) print("Music Player", "Pause");
        else print("Music Player", "Play");
    }

    void Update()
    {
        channel.isPlaying(out playing);
        if (playing)
        {
            if (Input.GetKey(KeyCode.F9))
            {
                volume -= Time.deltaTime;
                volume = Mathf.Clamp01(volume);
                channel.setVolume(volume);
            }
            if (Input.GetKey(KeyCode.F10))
            {
                volume += Time.deltaTime;
                volume = Mathf.Clamp01(volume);
                channel.setVolume(volume);
            }
        }
        if (Input.GetKeyUp(KeyCode.F7)) PlayPause();
        if (Input.GetKeyUp(KeyCode.F6)) loadPreviousTrack();
        if (Input.GetKeyUp(KeyCode.F8)) loadNextTrack();

    }
}
