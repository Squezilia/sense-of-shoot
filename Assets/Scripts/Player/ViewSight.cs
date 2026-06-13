using System.Collections.Generic;
using UnityEngine;

public class ViewSight : MonoBehaviour
{
    [SerializeField] public LayerMask targetLayers;
    public float fov = 120f;
    public float rayStep = 10f;
    public float fovDepth = 50f;
    
    private HashSet<GameObject> sightSet = new();
    
    private void FixedUpdate()
    {
        if (Time.fixedTime % 0.1f > Time.fixedDeltaTime) return;
        
        float rayCount = (fov / rayStep);
        HashSet<GameObject> outSightObjects =  new HashSet<GameObject>(sightSet);
        
        for (int i = 0; i < rayCount; i++)
        {
            float rayRotation = (this.transform.rotation.eulerAngles.z - (fov / 2) + (i * rayStep)) *  Mathf.Deg2Rad;
            Vector2 rayVector = new(Mathf.Cos(rayRotation), Mathf.Sin(rayRotation));
            
            RaycastHit2D[] hits = new RaycastHit2D[10];
            int hitCount = Physics2D.RaycastNonAlloc(transform.position, rayVector, hits, fovDepth, targetLayers);
            Debug.DrawRay(transform.position, rayVector * fovDepth, Color.green);
            
            for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
            {
                RaycastHit2D hit = hits[hitIndex];
                if (hit.collider == null && !hit.collider.CompareTag("Enemy")) return;
                
                Debug.DrawLine(transform.position, hit.point, Color.red);
                bool inSight = sightSet.Contains(hit.collider.gameObject);
                
                // add object in sight so we don't try to activate this object next time
                if (!inSight)
                {
                    sightSet.Add(hit.collider.gameObject);
                    outSightObjects.Remove(hit.collider.gameObject);
                    SightEnter(hit.collider.gameObject);
                }

                // this means, this object is in sight. so we just simple skip
                if (inSight) outSightObjects.Remove(hit.collider.gameObject);
            }
        }
        
        // remainings are not in sight
        foreach (GameObject outSightObject in outSightObjects)
        {
            sightSet.Remove(outSightObject);
            SightExit(outSightObject);
        }
    }

    private void SightEnter(GameObject target)
    {
        target.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
    }

    private void SightExit(GameObject target)
    {
        target.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }
}
