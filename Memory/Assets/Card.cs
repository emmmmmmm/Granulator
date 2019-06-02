using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    private string value;
    public bool visible = false;

   public Card(string val) {
        value = val;
        GetComponentInChildren<TextMesh>().text = val;
    }
    public string Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
        }
    }
}
