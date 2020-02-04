using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlugWire))]
public class Grapple : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField]
    private PlugWire plugWire;
    [SerializeField]
    private LineRenderer lineRenderer;

    [Header("Keybinds")]
    [SerializeField]
    private KeyCode grappleShotKey;

    [Header("Grapple Settings")]
    [Range(0, 50)]
    [SerializeField]
    private float grappleMaxDistance;

    [Header("Debugging")]
    [SerializeField]
    private bool grappleFired = false;

    private void Update()
    {
        if (Input.GetKeyDown(grappleShotKey) && !plugWire.plugAttached)
        {
            plugWire.plugAttached = true;
            grappleFired = true;
            Debug.Log("Shot");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grappleMaxDistance))
            {
                print("Hit");
                StartCoroutine(WireMoving(hit));
            }
        }
        else if (Input.GetKeyDown(grappleShotKey) && plugWire.plugAttached && grappleFired == false)
        {
            StopCoroutine("WireMoving()");
        }
    }

    private IEnumerator WireMoving(RaycastHit hit)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, plugWire.man.position);
        lineRenderer.SetPosition(1, hit.point);
        yield return new WaitForFixedUpdate();
        StartCoroutine(WireMoving(hit));
    }
}
