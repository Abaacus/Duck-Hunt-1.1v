using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Round // this holds the waves in used in the round
{
    public Wave[] waves;
}

[System.Serializable]
public struct Wave  // this holds the number of bullets, green ducks, and purple ducks present in the wave
{
    public int bullets;
    public int GDucks;
    public int PDucks;
}

// these two data structures allows for quick, direct customization of the game, allowing me to adjust the difficulty easily