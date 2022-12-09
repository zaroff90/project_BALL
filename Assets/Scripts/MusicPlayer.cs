using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] tracks;
    public string[] name;
    public string[] artist;

    public bool loop = false;
    public bool shuffle = true;

    private AudioSource sound;
    private Animator anim;

    void Start()
    {
        if (GameObject.Find("MUSICPLAYER") != null)
        {
            sound = GameObject.Find("MUSICPLAYER").GetComponent<AudioSource>();
            anim = this.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sound != null)
        {
            if (!sound.isPlaying)
            {
                if (tracks.Length > 1)
                {
                    OnNext();
                }
            }
        }
    }

    public void OnNext()
    {
        sound.Stop();
        int randomNumber = Random.Range(0, tracks.Length - 1);
        sound.clip = tracks[randomNumber];
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = name[randomNumber];
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = artist[randomNumber];
        anim.Play("in");
        sound.Play();
    }

}
