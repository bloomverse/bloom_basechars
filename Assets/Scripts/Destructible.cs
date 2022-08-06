using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{

    public GameObject destroyedVersion;
    

    public void destroyObj(){
        Instantiate(destroyedVersion,transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
