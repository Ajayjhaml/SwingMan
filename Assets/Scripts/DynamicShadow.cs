using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class DynamicShadow : Shadow
{
    public bool isDynamic;
    public float maxOffset;
    [SerializeField] private CameraFollow cameraFollow;

    private void LateUpdate ()
    {
        if(isDynamic)
        {
            shadowSpriteRenderer.sprite = spriteRenderer.sprite;
            shadowSpriteRenderer.flipX = spriteRenderer.flipX;
            float offset = 0.1f;
            if(cameraFollow.maxOffset != 0)
            {
                offset = (cameraFollow.offset / cameraFollow.maxOffset) * (maxOffset * 100);
                offset /= 100;
            }
            shadowGameObject.transform.position = gameObject.transform.position + new Vector3 (shadowOffset.x, shadowOffset.y) + new Vector3 (-offset, 0f, 0f);
        }
    }
}
