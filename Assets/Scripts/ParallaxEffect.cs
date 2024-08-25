using UnityEngine;

public class ParallaxEffect : ProjectBehaviour
{
    private float startingPos;
    public float AmountOfParallax { get; private set; }
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        AmountOfParallax = Mathf.Abs(transform.position.z);
        startingPos = transform.position.x;
    }

    private void Update()
    {
        Vector3 Position = mainCamera.transform.position;
        float Distance = Position.x * AmountOfParallax;

        Vector3 NewPosition = new Vector3(startingPos + Distance, transform.position.y, transform.position.z);

        transform.position = NewPosition;
    }
}
