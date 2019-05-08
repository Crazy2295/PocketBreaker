using UnityEngine;

public class FogHide : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Renderer[] rs = other.gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rs)
            r.enabled = false;
    }

    void OnTriggerExit(Collider other)
    {
        Renderer[] rs = other.gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rs)
            r.enabled = true;
    }
}