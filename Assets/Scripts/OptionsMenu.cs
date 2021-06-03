using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject explosionsCheck;
    public GameObject relaxedModeCheck;
    void Start()
    {
        bool explosions = PlayerPrefs.GetInt("explosions", 1) == 0 ? false : true;
        explosionsCheck.GetComponent<Toggle>().isOn = explosions;
        bool relaxedMode = PlayerPrefs.GetInt("relaxedMode", 0) == 0 ? false : true;
        relaxedModeCheck.GetComponent<Toggle>().isOn = relaxedMode;
    }

    public void SetExplosions(bool explosions)
    {
        int explosionInt = explosions ? 1 : 0;
        PlayerPrefs.SetInt("explosions", explosionInt);
    }

    public void SetRelaxedMode(bool relaxedMode)
    {
        int relaxedModeInt = relaxedMode ? 1 : 0;
        PlayerPrefs.SetInt("relaxedMode", relaxedModeInt);
    }
}
