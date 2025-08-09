using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动时背景跟随移动，并通过速度差制造视差效应
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float xPosition;
    private float length;
    private float yPosition;
    private float high;

    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
        high = GetComponent<SpriteRenderer>().bounds.size.y;
        xPosition = transform.position.x;
        yPosition = transform.position.y;
    }

    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;
        float distanceToJump = cam.transform.position.y * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, yPosition + distanceToJump);


        //制造无尽背景
        if (distanceMoved > xPosition + length)
            xPosition = xPosition + length;
        else if (distanceMoved < xPosition - length)
            xPosition = xPosition - length;

    }
}
