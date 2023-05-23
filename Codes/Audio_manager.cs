using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Audio_manager : MonoBehaviour
{
    AudioSource main_source;
    AudioSource effect_source;
    AudioSource ambience_source;
    AudioClip button_hover_clip;
    AudioClip button_click_clip;
    AudioClip intro_swoosh_clip;
    AudioClip swoosh_clip;
    AudioClip success_clip;
    AudioClip denied_buzzer_clip;
    AudioClip keyboard_press_clip;
    AudioClip slowmo_clip;
    AudioClip drawing;
    AudioClip cheering;
    List<AudioClip> bg_musics;
    float original_music_volume;
    void Start()
    {
        Set_variables();
        Load_background_musics();
    }

    private void Set_variables()
    {
        main_source = GameObject.Find("Audio_main_source").GetComponent<AudioSource>();
        main_source.volume = .1f;
        effect_source = GameObject.Find("Audio_effect_source").GetComponent<AudioSource>();
        effect_source.volume = .5f;
        ambience_source = GameObject.Find("Audio_ambience_source").GetComponent<AudioSource>();
        button_hover_clip = Resources.Load<AudioClip>("Sounds/buttonHover");
        button_click_clip = Resources.Load<AudioClip>("Sounds/buttonClick");
        intro_swoosh_clip = Resources.Load<AudioClip>("Sounds/introSwoosh");
        swoosh_clip = Resources.Load<AudioClip>("Sounds/swoosh");
        success_clip = Resources.Load<AudioClip>("Sounds/success");
        denied_buzzer_clip = Resources.Load<AudioClip>("Sounds/deniedBuzzer");
        keyboard_press_clip = Resources.Load<AudioClip>("Sounds/keyboardKeyPress");
        slowmo_clip = Resources.Load<AudioClip>("Sounds/slowMo");
        drawing = Resources.Load<AudioClip>("Sounds/scribble");
        cheering = Resources.Load<AudioClip>("Sounds/cheering");
        bg_musics = new List<AudioClip>();
    }
    public void Stop_playing_effect()
    {
        effect_source.Stop();
    }
    public void Play_cheering()
    {
        effect_source.PlayOneShot(cheering);
    }public void Play_drawing()
    {
        effect_source.PlayOneShot(drawing);
    }

    public void Play_slowmo()
    {
        effect_source.PlayOneShot(slowmo_clip);
    }
    public void Play_keyboard_press()
    {
        effect_source.PlayOneShot(keyboard_press_clip);
    }
    public void Play_denied_buzzer()
    {
        effect_source.PlayOneShot(denied_buzzer_clip);
    }
    public void Play_success()
    {
        effect_source.PlayOneShot(success_clip);
    }
    private void Load_background_musics()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/BackgroundMusic");
        bg_musics.AddRange(clips);
    }

    public void Play_swoosh()
    {
        effect_source.PlayOneShot(swoosh_clip);
    }
    private void OnLevelWasLoaded(int level)
    {
        StartCoroutine(Play_main_music());
        ambience_source.Play();
    }
    public void Play_intro_swoosh()
    {
        effect_source.PlayOneShot(intro_swoosh_clip);
    }
    public IEnumerator Speed_up_music()
    {
        while (main_source.pitch < 1)
        {
            main_source.pitch += .02f;
            yield return null;
        }
    }
    public IEnumerator Slow_down_music()
    {
        while (main_source.pitch > .4f)
        {
            main_source.pitch -= .02f;
            yield return null;
        }
    }
    public IEnumerator Mute_music()
    {
        original_music_volume = main_source.volume;
        while (main_source.volume > 0)
        {
            main_source.volume -= .05f;
            yield return null;
        }
    }
    public IEnumerator Increase_music_volume()
    {
        while (main_source.volume < original_music_volume)
        {
            main_source.volume += 0.01f;
            yield return null;
        }
    }
    private IEnumerator Play_main_music()
    {
        List<AudioClip> used_clips = new List<AudioClip>();
        while (true)
        {
            System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
            AudioClip clip = bg_musics[rnd.Next(0, bg_musics.Count)];
            main_source.PlayOneShot(clip);
            Debug.Log($"Song name:{clip.name}");
            yield return new WaitUntil(() => !main_source.isPlaying);
        }
        
    }
    public void Set_music_volume_to_variable(float volume)
    {
        main_source.volume = volume;
    }
    public void Set_effect_volume_to_variable(float volume)
    {
        effect_source.volume = volume;
        ambience_source.volume = volume / 4;
    }
    public void Play_button_hover()
    {
        effect_source.PlayOneShot(button_hover_clip);
    }
    public void Play_button_click()
    {
        effect_source.PlayOneShot(button_click_clip);
    }
}
