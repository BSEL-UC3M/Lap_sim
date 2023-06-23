using UnityEngine;
using UnityEngine.UI;

public class SphereController : MonoBehaviour
{
    private Vector3 originalPosition;

    void Start()
    {
        // Store the original position of the sphere
        originalPosition = transform.localPosition;

        // Attach button click events to their respective functions
        Button moveUpButton = GameObject.Find("MoveUp").GetComponent<Button>();
        moveUpButton.onClick.AddListener(MoveSphereUp);

        Button moveDownButton = GameObject.Find("MoveDown").GetComponent<Button>();
        moveDownButton.onClick.AddListener(MoveSphereDown);

        Button resetButton = GameObject.Find("Reset").GetComponent<Button>();
        resetButton.onClick.AddListener(ResetSphere);
    }

    void MoveSphereUp()
    {
        // Move the sphere 5mm in the positive Y direction
        transform.Translate(Vector3.up * 5f);
    }

    void MoveSphereDown()
    {
        // Move the sphere 5mm in the negative Y direction
        transform.Translate(Vector3.down * 5f);
    }

    void ResetSphere()
    {
        // Reset the sphere's position to the original position
        transform.localPosition = originalPosition;
    }
}

