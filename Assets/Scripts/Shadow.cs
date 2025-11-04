using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] Material shadowMaterial;
    public Vector2 shadowOffset;

    protected SpriteRenderer spriteRenderer;
    protected GameObject shadowGameObject;
    protected SpriteRenderer shadowSpriteRenderer;


    private void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shadowGameObject = new GameObject ("Shadow2D");

        shadowSpriteRenderer = shadowGameObject.AddComponent<SpriteRenderer> ();
        shadowSpriteRenderer.sprite = spriteRenderer.sprite;
        shadowSpriteRenderer.material = shadowMaterial;
        shadowSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        shadowSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder;

        shadowGameObject.transform.parent = gameObject.transform;
        shadowGameObject.transform.localScale = Vector3.one;
        shadowGameObject.transform.rotation = gameObject.transform.rotation;
        shadowGameObject.transform.position = gameObject.transform.position + (Vector3) shadowOffset;
    }
}