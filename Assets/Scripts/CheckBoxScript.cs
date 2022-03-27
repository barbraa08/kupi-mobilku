using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxScript : MonoBehaviour
{
    public Text Name;
    public void SetUp(string name)
    {
        this.name = $"B{name}";
        Name.text = name;
    }
}
