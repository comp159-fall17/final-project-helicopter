using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    public AudioClip fallMusic;
    public AudioClip forestMusic;
    public AudioClip snowMusic;
    public AudioClip bossMusic;
	
	// Update is called once per frame
	void Update () {
		if(LevelGen.Instance.CurrentBiome.Name == "fall")
        {
            this.GetComponent<AudioSource>().clip = fallMusic;
        }
        else if (LevelGen.Instance.CurrentBiome.Name == "forest")
        {
            this.GetComponent<AudioSource>().clip = forestMusic;
        }
        else
        {
            this.GetComponent<AudioSource>().clip = snowMusic;
        }
    }
}
