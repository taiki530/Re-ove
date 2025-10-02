using System;
using UnityEngine;

public class VineGrow : MonoBehaviour
{
    public float growSpeed = 2f;
    public float maxLength = 10f;

    private float currentLength = 0.1f;
    private Vector3 originalScale;
    private Vector3 growDirection;
    private Vector3 basePosition;
    private bool isBlocked = false;
    private bool isGrowing = false;
    private bool finishedNotified = false;

    public event Action OnFinishedGrow;

    public void Initialize(Vector3 direction, Vector3 origin)
    {
        growDirection = direction.normalized;
        basePosition = origin;

        originalScale = transform.localScale;
        currentLength = 0.1f;
        transform.localScale = new Vector3(originalScale.x, currentLength, originalScale.z);
        transform.position = basePosition + growDirection * (currentLength * 0.5f);

        isBlocked = false;
        isGrowing = true;
        finishedNotified = false;
    }

    void Update()
    {
        if (!isGrowing) return;

        if (currentLength >= maxLength)
        {
            if (!finishedNotified)
            {
                finishedNotified = true;
                isGrowing = false;
                OnFinishedGrow?.Invoke();
            }
            return;
        }

        float growth = growSpeed * Time.deltaTime;
        Vector3 tip = basePosition + growDirection * currentLength;

        if (Physics.Raycast(tip, growDirection, growth))
        {
            isBlocked = true;
            isGrowing = false;
            if (!finishedNotified)
            {
                finishedNotified = true;
                OnFinishedGrow?.Invoke();
            }
            return;
        }

        currentLength += growth;
        currentLength = Mathf.Min(currentLength, maxLength);

        transform.localScale = new Vector3(originalScale.x, currentLength, originalScale.z);
        transform.position = basePosition + growDirection * (currentLength * 0.5f);
    }
}

