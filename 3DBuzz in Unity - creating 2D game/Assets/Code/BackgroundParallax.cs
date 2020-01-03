using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public Transform[] Background;
    public float ParallaxScale;
    public float ParallexReductionFactor;
    public float Smoothing;

    private Vector3 _lastPosition;

    public void Start()
    {
        _lastPosition = transform.position;
    }

    public void Update()
    {
        var parallax = (_lastPosition.x - transform.position.x) * ParallaxScale;

        for (var i = 0; i < Background.Length; i++)
        {
            var bacgroundTargetPosition = Background[i].position.x + parallax * (i * ParallexReductionFactor + 1);
            Background[i].position = Vector3.Lerp(
                Background[i].position, 
                new Vector3(bacgroundTargetPosition, Background[i].position.y, Background[i].position.z),
                Smoothing * Time.deltaTime);
        }

        _lastPosition = transform.position;
    }
}
