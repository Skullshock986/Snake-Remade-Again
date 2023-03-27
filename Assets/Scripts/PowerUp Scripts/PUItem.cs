using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUItem : ScriptableObject // Allow this class to create objects and be inherited from
{
    // Initialise variables for Power-Up name, description, activation time, a Visual for the Power-Up, and a value to differentiate Power-Ups of a similar type
    public string Name;
    public string Description;

    public float ActivateTime;

    public Sprite Visual;

    public float Value;
}
