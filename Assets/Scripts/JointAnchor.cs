using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointAnchor : MonoBehaviour
{
    [SerializeField] private Sprite spriteUnsticked;
    [SerializeField] private Sprite spriteSticked;
    [SerializeField] private GameObject dashLine;  //  Assign this manually in Inspector

    public float animTime;
    public AnimationCurve animationCurve;

    private SpriteRenderer spriteRenderer;
    private bool sticked = false;
    private Coroutine selectingCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Safety check
        if (dashLine == null)
        {
            Debug.LogWarning($"{name}: dashLine not assigned in Inspector!");
        }
    }

    public void SetSticked()
    {
        spriteRenderer.sprite = spriteSticked;
        sticked = true;
    }

    public void SetUnsticked()
    {
        spriteRenderer.sprite = spriteUnsticked;
        sticked = false;
        Unselected();
    }

    public void Selected()
    {
        if (!sticked)
        {
            if (selectingCoroutine != null)
                StopCoroutine(selectingCoroutine);

            selectingCoroutine = StartCoroutine(SelectingJoint());
        }
        else if (dashLine != null)
        {
            dashLine.transform.localScale = Vector3.zero;
        }
    }

    public void Unselected()
    {
        if (selectingCoroutine != null)
            StopCoroutine(selectingCoroutine);

        if (dashLine != null)
            dashLine.transform.localScale = Vector3.zero;
    }

    IEnumerator SelectingJoint()
    {
        float time = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = new Vector3(1.2f, 1.2f, 1f);

        while (time <= animTime)
        {
            time += Time.deltaTime;

            if (dashLine != null)
                dashLine.transform.localScale = Vector3.Lerp(startScale, endScale, animationCurve.Evaluate(time));

            yield return null;
        }
    }
}
