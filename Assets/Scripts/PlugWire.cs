using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugWire : MonoBehaviour
{
    public Transform man;
    public Transform plug;
    private LineRenderer plugLine;
    public int maxDistance;
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
            currentDistance = Vector3.Distance(man.position, plug.position);

            for (int i = 0; i < plugLine.positionCount; i++)
            {
                if (i == 0)
                {
                    plugLine.SetPosition(i, man.position);
                }

                else if (i + 1 == plugLine.positionCount)
                {
                    plugLine.SetPosition(i, plug.position);
                }

                else
                {
                    float position = i;
                    float point = position / plugLine.positionCount;

                    int centerPos = Mathf.FloorToInt((plugLine.positionCount + 1) / 2);
                    int centerDist = Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(centerPos - (i + 1), 2)));
                    float sag = ((maxDistance - currentDistance) / 5 - (Mathf.Pow(centerDist, 2) * ((maxDistance - currentDistance) / 100)));

                    if (sag < 0)
                    {
                        sag = 0;
                    }

                    Vector3 newPos = new Vector3(man.position.x - ((man.position.x - plug.position.x) * point),
                                                 man.position.y - (man.position.y - plug.position.y) - sag,
                                                 man.position.z - ((man.position.z - plug.position.z) * point));
                    plugLine.SetPosition(i, newPos);
                }
            }
        }
    }

    void Update()
    {
        GetPlug();
        AttachPlugLead();
    }
}
