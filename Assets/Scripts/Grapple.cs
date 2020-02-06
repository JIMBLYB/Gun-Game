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

    [Range(0, 50)]
    [SerializeField]
    private float grappleMaxDistance;

    [Header("Debugging")]

    private Coroutine grappleIE;
    private Coroutine reelIE;
    private RaycastHit hit;

    [SerializeField]
    public bool grappleFired = false;

    [Range(1, 50)]
    [SerializeField]
    private int reelSpeedDivisor;
    private Vector3 reelDestination;

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

            if (Physics.Raycast(ray, out hit, grappleMaxDistance))
            {
                grappleIE = StartCoroutine(WireMoving());
            }
        }

        if (Input.GetKeyDown(reelInKey) && grappleFired)
        {
            reelDestination = hit.point - plugWire.man.position;
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
        plugWire.man.GetComponent<Rigidbody>().useGravity = false;
        if (!Input.GetKey(reelInKey))
        {
            StopCoroutine(reelIE);
            plugWire.man.GetComponent<Rigidbody>().useGravity = true;
            yield break;
        }

        if (plugWire.man.transform.position != hit.point)
        {
            plugWire.man.GetComponent<CharacterController>().Move(reelDestination / reelSpeedDivisor);
        }

        yield return new WaitForFixedUpdate();
        reelIE = StartCoroutine(WirePull());
    }
}
