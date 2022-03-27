using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePos : MonoBehaviour
{
    [SerializeField] private GameObject Parent;

    public void ChangeIn()
    {
        Parent.transform.position = new Vector3(132.04f, 0, 0);
    }

    public void ChangeOut()
    {
        Parent.transform.position = new Vector3(1788, 0, 0);
    }
}
