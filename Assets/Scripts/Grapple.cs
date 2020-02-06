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
    public bool grappleFired = false;
    private Coroutine grappleIE;
    [Range(0f, 10f)]
    [SerializeField]
    private float grappleCooldown;
    [SerializeField]
    private float grappleTimer;

    private void Update()
    {
        if (Input.GetKeyDown(grappleShotKey) && !plugWire.plugAttached && (grappleTimer + grappleCooldown) < Time.time)
        {
            grappleTimer = Time.time;
            plugWire.plugAttached = true;
            grappleFired = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, grappleMaxDistance))
            {
                grappleIE = StartCoroutine(WireMoving(hit));
            }
        }
    }

    private IEnumerator WireMoving(RaycastHit hit)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, plugWire.man.position);
        lineRenderer.SetPosition(1, hit.point);
        
        yield return new WaitForFixedUpdate();
        grappleIE = StartCoroutine(WireMoving(hit));
        if (Input.GetKeyDown(grappleShotKey) && plugWire.plugAttached && (grappleTimer + grappleCooldown) < Time.time)
        {
            grappleTimer = Time.time;
            plugWire.plugAttached = false;
            grappleFired = false;
            lineRenderer.enabled = false;
            StopCoroutine(grappleIE);
        }
    }
}
