using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Invoke("DestroySelf", 3f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

}
