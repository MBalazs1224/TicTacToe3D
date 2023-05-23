using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_camera_anim : MonoBehaviour
{
    // Start is called before the first frame update
    Camera main_cam;
    Vector3 target;
    public IEnumerator Camera_movement()
    {
        main_cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        target = main_cam.transform.position + new Vector3(0, 0, 2.5f);
        yield return new WaitForSeconds(1);
    }
}

