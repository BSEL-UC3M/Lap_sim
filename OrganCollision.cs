using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OrganCollision : MonoBehaviour
{
    private static List<string> activeGameObjects = new List<string>();
    private static TextMeshProUGUI organTextBox;

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

        if (organTextBox == null)
        {
            GameObject canvas = GameObject.Find("CanvasOrgan");
            organTextBox = canvas.transform.Find("Organ text box").GetComponent<TextMeshProUGUI>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Enter");
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            AddGameObjectName(gameObject.name);
            UpdateTextBox();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Exit");
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            RemoveGameObjectName(gameObject.name);
            UpdateTextBox();
        }
    }

    void AddGameObjectName(string gameObjectName)
    {
        if (!activeGameObjects.Contains(gameObjectName))
        {
            activeGameObjects.Add(gameObjectName);
        }
    }

    void RemoveGameObjectName(string gameObjectName)
    {
        activeGameObjects.Remove(gameObjectName);
    }

    void UpdateTextBox()
    {
        organTextBox.text = string.Join("\n", activeGameObjects);
    }
}
