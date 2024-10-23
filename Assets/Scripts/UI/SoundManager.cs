using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioRefsSO audioRefsSO;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("SoundManager Intansce has exited");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void PlaySoundShoot(Vector3 position)
    {
        PlaySound(audioRefsSO.shoot[Random.Range(0, audioRefsSO.shoot.Length)], position); 
    }

    public void PlaySoundExplosion(Vector3 position)
    {
        PlaySound(audioRefsSO.explosion[Random.Range(0, audioRefsSO.explosion.Length)], position);
    }

    private void PlaySound(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }
}
