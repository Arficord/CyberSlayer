using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalProperties : MonoBehaviour
{
    public static GlobalProperties gp;
    [SerializeField] public LayerMask groundLayerMask;
    [SerializeField] public LayerMask ceilingLayerMask;
    [SerializeField] public int fallingLayer;
    [SerializeField] public LayerMask usableLayerMask;


    private void Awake()
    {
        gp = this;
    }
}
