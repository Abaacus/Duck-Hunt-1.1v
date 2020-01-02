using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckBoundary : MonoBehaviour   // manages the area the ducks are allowed to fly in
{
    #region Singleton
    public static DuckBoundary instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;

        boundary = new Boundary(scale, offset); // calculates the duck x's & y's min and max
    }
    #endregion

    public Boundary boundary;   // the boundary info

    public Vector2 scale;   // length and width of the boundary
    public Vector2 offset;  // midpoint of the boundary

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; // debug draws the boundary
        Gizmos.DrawWireCube(offset, scale);
    }
}