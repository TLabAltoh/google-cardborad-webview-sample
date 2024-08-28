/**
 * Portions of this file are taken from the following repositories: 
 * Repository Name: cardboard-xr-plugin
 * Repository URL: https://github.com/joaoborks/cardboard-xr-plugin
 * Author: https://github.com/joaoborks
 * Lisence: Apache License 2.0
 * Source File: Samples~/hellocardboard-unity/Scripts/XRCardboard/XRCardboardInputSettings.cs
 * Source Commit ID: d9f0aa2eb03eb544069ea7278048994e548e6d03
 * Modifications: Add Properties (GazeEnabled)
 */

using UnityEngine;

[CreateAssetMenu(fileName = "XRCardboardInputSettings", menuName = "Google Cardboard/Cardboard Input Settings")]
public class XRCardboardInputSettings : ScriptableObject
{
    public string ClickInput => clickInputName;
    public float GazeTime => gazeTimeInSeconds;
    public bool GazeEnabled => gazeEnabled;

    [SerializeField]
    string clickInputName = "Submit";
    [SerializeField, Range(.5f, 5)]
    float gazeTimeInSeconds = 2f;
    [SerializeField]
    bool gazeEnabled = false;
}