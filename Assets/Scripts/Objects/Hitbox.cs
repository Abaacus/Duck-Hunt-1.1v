using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hitbox : MonoBehaviour // stores whether or not this hitbox has been hit, and the points the player gets for hitting it
{
    public bool hit;    // has it been hit?
    public int pointValue;  // how much the hitbox is worth
}
