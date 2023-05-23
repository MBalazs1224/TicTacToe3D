using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_button : MonoBehaviour
{
    bool sound = true;
    SpriteRenderer music_sprite;
    [SerializeField] Sprite music_on;
    [SerializeField] Sprite music_off;
    AudioSource music_source;
    Audio_manager audio_manager;
    private void Start()
    {
    }
    private void OnMouseDown()
    {
        music_sprite = GameObject.Find("Sound_sprite").GetComponent<SpriteRenderer>();
        music_source = GameObject.Find("Audio_main_source").GetComponent<AudioSource>();
        audio_manager = Camera.main.GetComponent<Audio_manager>();
        if (sound)
        {
            music_sprite.sprite = music_off;
            music_source.mute = true;
            sound = false;
        }
        else
        {
            music_sprite.sprite = music_on;
            music_source.mute = false;
            sound = true;
        }
    }
}
