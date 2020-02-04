using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlugWire))]
public class Grapple : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField]
    private PlugWire plugWire;

    [Header("Keybinds")]
    [SerializeField]
    private KeyCode grappleShotKey;

    [Header("Grapple Settings")]
    [Range(0, 50)]
    [SerializeField]
    private float grappleMaxDistance;




    private void Update()
    {
        if (Input.GetKeyDown(grappleShotKey) && !plugWire.plugAttached)
        {
            Debug.Log("Shot");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grappleMaxDistance))
            {
                print("Hit"); 
            }
        }
    }
}
