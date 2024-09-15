using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrackRotation : MonoBehaviour
{
    private float scrollSpeed = 5f;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

    }

    void FixedUpdate()
    {
        var offset = Time.time * scrollSpeed * Input.GetAxisRaw("Vertical");
        meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.0f, offset));
        meshRenderer.material.SetTextureOffset("_BumpMap", new Vector2(0.0f, offset));
    }
}
