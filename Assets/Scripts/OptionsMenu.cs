using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public void SetExplosions(bool explosions)
    {
        Debug.Log("explosions" + explosions);
    }

    public void SetRelaxedMode(bool relaxedMode)
    {
        Debug.Log("relaxed" + relaxedMode);
    }
}
