using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(PlugWire))]
public class Grapple : MonoBehaviour
{
    [Header("Component References")]

    [SerializeField]
    private PlugWire plugWire;
    [SerializeField]
    private FirstPersonController firstPersonController;

    [SerializeField]
    private LineRenderer lineRenderer;

    [Header("Keybinds")]

    [SerializeField]
    private KeyCode grappleShotKey;
    [SerializeField]
    private KeyCode reelInKey;

    [Header("Grapple Settings")]

    [Range(0, 1000)]
    [SerializeField]
    private float grappleMaxDistance;

    [Header("Debugging")]

    private Coroutine grappleIE;
    private Coroutine reelIE;
    [SerializeField]
    private RaycastHit hit;

    [SerializeField]
    public bool grappleFired = false;

    [Range(1, 500)]
    [SerializeField]
    private int reelSpeed;
    private Vector3 reelDestination;

    [Range(0f, 10f)]
    [SerializeField]
    private float grappleCooldown;
    [SerializeField]
    private float grappleTimer;

    // Button Inputs
    private void Update()
    {
        if (Input.GetKeyDown(grappleShotKey) && !plugWire.plugAttached && (grappleTimer + grappleCooldown) < Time.time)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, grappleMaxDistance))
            {
                grappleTimer = Time.time;
                plugWire.plugAttached = true;
                grappleFired = true;

                grappleIE = StartCoroutine(WireMoving());
            }
        }
        else
        {
            plugWire.plugAttached = false;
            grappleFired = false;
        }

        if (Input.GetKeyDown(reelInKey) && grappleFired)
        {
            reelIE = StartCoroutine(WirePull());
        }
    }

    private IEnumerator WireMoving()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, plugWire.man.position);
        lineRenderer.SetPosition(1, hit.point);

        yield return new WaitForFixedUpdate();
        grappleIE = StartCoroutine(WireMoving());

        if (Input.GetKeyDown(grappleShotKey) && plugWire.plugAttached && (grappleTimer + grappleCooldown) < Time.time)
        {
            grappleTimer = Time.time;
            plugWire.plugAttached = false;
            grappleFired = false;
            lineRenderer.enabled = false;
            StopCoroutine(grappleIE);
        }
    }

    private IEnumerator WirePull()
    {
        firstPersonController.enabled = false;
        plugWire.man.GetComponent<Rigidbody>().useGravity = false;
        if (!Input.GetKey(reelInKey))
        {
            firstPersonController.enabled = true;
            StopCoroutine(reelIE);
            plugWire.man.GetComponent<Rigidbody>().useGravity = true;
            yield break;
        }

        if (plugWire.man.transform.position != hit.point)
        {
            reelDestination = hit.point - plugWire.man.position;
            reelDestination = (reelDestination / 10000) * reelSpeed;

            plugWire.man.GetComponent<CharacterController>().Move(reelDestination);
        }

        yield return new WaitForFixedUpdate();
        reelIE = StartCoroutine(WirePull());
    }
}
