using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugWire : MonoBehaviour
{
    public Transform man;
    public Transform plug;
    private LineRenderer plugLine;
    public float maxDistance;
    public float currentDistance;
    public float rayDistance;
    public GameObject currentHighlightPlug;
    public GameObject currentPlug;
    public bool highlightToggle;
    public bool plugAttached;

    void Start()
    {
        plugLine = GetComponent<LineRenderer>();
    }

    private void GetPlug()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.name == "HighlightPlug" && hit.distance <= rayDistance)
            {
                currentHighlightPlug = hit.transform.gameObject;
                currentHighlightPlug.GetComponent<Renderer>().enabled = true;
                highlightToggle = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    plug = currentHighlightPlug.transform.parent;
                    plugAttached = true;
                }
            }
            if (highlightToggle == true && hit.collider.gameObject.name != "HighlightPlug")
            {
                currentHighlightPlug.GetComponent<Renderer>().enabled = false;
                highlightToggle = false;
            }
        }
    }

    private void AttachPlugLead()
    {
        if (plugAttached == true)
        {
            plugLine.enabled = true;
            man.GetComponent<Renderer>().enabled = true;
            currentDistance = Vector3.Distance(man.position, plug.position);
            plugLine.SetPosition(0, man.position);
            plugLine.SetPosition(1, plug.position);
        }
    }

    private void DetachPlugLead()
    {
        if (plugAttached == true && currentDistance >= maxDistance)
        {
            plugAttached = false;
            plugLine.enabled = false;
        }
    }

    void Update()
    {
        GetPlug();
        AttachPlugLead();
        DetachPlugLead();
    }
}
