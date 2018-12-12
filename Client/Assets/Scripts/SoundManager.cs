using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviourInstance<SoundManager> {	

    public AudioSource PlayBg(SoundBgTpye bgType, GameObject go) {
        if (go.GetComponent<AudioSource>() == false) {
            go.AddComponent<AudioSource>();
        }

        AudioSource audioSource = go.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load(string.Format("Sounds/{0}", bgType.ToString())) as AudioClip;
        audioSource.Play();
        return audioSource;
    }

    public AudioSource PlayFx(SoundFxType soundType, GameObject go) {
        if (go.GetComponent<AudioSource>() == false) {
            go.AddComponent<AudioSource>();
        }

        AudioSource audioSource = go.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load(string.Format("Sounds/{0}", soundType.ToString())) as AudioClip;
        audioSource.Play();
        return audioSource;
    }

    public void Stop() {

    }
}

public enum SoundFxType {
    None,
    Shoot,
    LazerShoot,
    HitRock,
    SpawnPlayer,
    HitPlayer,
    Aming,
    DeadPlayer,
}

public enum SoundBgTpye {
    None,
    Bg1,
}