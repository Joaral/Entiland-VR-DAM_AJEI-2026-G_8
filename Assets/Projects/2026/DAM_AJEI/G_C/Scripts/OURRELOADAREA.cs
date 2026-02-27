using Autohand.Demo;
using System.Collections;
using UnityEngine;

public class OURRELOADAREA : MonoBehaviour
{
    public OURPISTOL pistolita;
    public AudioSource reloadSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gun")
        {

            if(pistolita.ammo != pistolita.ammoMax)
            {
                StartCoroutine(Reload(reloadSound));

                pistolita.loaded = true;
                pistolita.ammo = pistolita.ammoMax;
            }
        }
    }

    IEnumerator Reload(AudioSource source)
    {

        source.Play();

        yield return new WaitWhile(() => source.isPlaying);

    }

}
