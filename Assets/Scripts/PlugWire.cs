using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogLead : MonoBehaviour
{
    public Transform man;
    public Transform dog;
    private LineRenderer leadLine;
    public int maxDistance;
    private float currentDistance;

    void Start()
    {
        leadLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        currentDistance = Vector3.Distance(man.position, dog.position);

        for (int i = 0; i < leadLine.positionCount; i++)
        {
            if (i == 0)
            {
                leadLine.SetPosition(i , man.position);
            }

            else if (i + 1 == leadLine.positionCount)
            {
                leadLine.SetPosition(i, dog.position);
            }

            else
            {
                float position = i;               
                float point = position / leadLine.positionCount;

                int centerPos = Mathf.FloorToInt((leadLine.positionCount + 1) / 2);
                int centerDist = Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(centerPos - (i + 1), 2)));
                float sag = ((maxDistance - currentDistance) / 5 - (Mathf.Pow(centerDist , 2) * ((maxDistance - currentDistance) / 100)));
                
                if (sag < 0)
                {
                    sag = 0;
                }

                Vector3 newPos = new Vector3(man.position.x - ((man.position.x - dog.position.x) * point),
                                             man.position.y - (man.position.y - dog.position.y) - sag,
                                             man.position.z - ((man.position.z - dog.position.z) * point));
                leadLine.SetPosition(i, newPos);
            }
        }        
    }
}
