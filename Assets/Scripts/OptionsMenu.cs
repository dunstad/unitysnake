using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
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
