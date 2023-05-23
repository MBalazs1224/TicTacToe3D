using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound_button : MonoBehaviour
{
    bool sound = true;
    SpriteRenderer sound_sprite;
    [SerializeField] Sprite sound_on;
    [SerializeField] Sprite sound_off;
    AudioSource effect_source;
    AudioSource ambience_source;
    Audio_manager audio_manager;
    float original_sound_volume;
    private void Start()
    {
        sound_sprite = GameObject.Find("Effect_sprite").GetComponent<SpriteRenderer>();
        effect_source = GameObject.Find("Audio_effect_source").GetComponent<AudioSource>();
        ambience_source = GameObject.Find("Audio_ambience_source").GetComponent<AudioSource>();
        audio_manager = Camera.main.GetComponent<Audio_manager>();
    }
    private void OnMouseDown()
    {
        if (sound)
        {
            sound_sprite.sprite = sound_off;
            //audio_manager.Mute_effect();
            effect_source.mute = true;
            ambience_source.mute = true;
            sound = false;
        }
        else
        {
            sound_sprite.sprite = sound_on;
            //audio_manager.Resume_effect_volume();
            effect_source.mute = false;
            ambience_source.mute = false;
            sound = true;
        }
    }
}
