using UnityEngine;
using System.Collections;

/// <summary>
/// Instances of this object will hold additional wall information not
/// contained in a standard GameObject.
/// 
/// Units (generally) should be the same as those of the a head tracking
/// system (if one is used).
/// </summary>
/// 
public class WallDefinition : MonoBehaviour
{
    /// <summary>
    /// Width of wall
    /// </summary>
    public float Width = 1.0f;

    /// <summary>
    /// Height of wall
    /// </summary>
    public float Height = 1.0f;
}
