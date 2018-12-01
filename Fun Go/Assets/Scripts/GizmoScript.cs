using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoScript : MonoBehaviour {

    // Source : https://www.youtube.com/watch?v=itIh8PYGP7w
    public float gizmoSize = 0.75f;
    public Color gizmoColor = Color.yellow;

    // Snap this Gizmoz into Your Object Point by
    // On Move Tool , Hold V + Drag -> To make a snap point

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }
}
