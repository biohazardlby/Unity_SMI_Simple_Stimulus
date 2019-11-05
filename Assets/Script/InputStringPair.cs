using UnityEngine;
using System.Collections;

[System.Serializable]
public class InputStringPair
{
    public KeyCode input;
    public string value;
    public InputStringPair(KeyCode input, string value)
    {
        this.input = input;
        this.value = value;
    }
}
