using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPartStats : MonoBehaviour
{
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static float ApplyModifier(float value, float modifier)
    {
        if (value < 0)
            return value * (1f - (modifier - 1f));
        else
            return value * modifier;
    }

    public static float ApplyLowIsPosModifier(float value, float modifier)
    {
        if (value < 0)
            return value * modifier;
        else
            return value * (1f - (modifier - 1f));
    }
}
