using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;




public class ModelLoader : MonoBehaviour
{
    public string modelsFolderPath; // The path to the models folder inside Resources folder
    public string SagittalModelsFolderPath; // The alternative path to the models folder
    public string CoronalModelsFolderPath;
    private bool isAlternativePath = false; // Flag to track the current path state
    public GameObject sagittalGameObject; //For HalfDesing with both supports. 
    public GameObject coronalGameObject;
    private string[] desiredNames = {
        "left iliac vein", "left iliac vein", "cava and right iliac vein", "cava and renal vein", "right gonadal vein",
        "left gonadal vein", "cava and right iliac vein", "portal venous system", "right gonadal artery", "right iliac artery",
        "aorta and left iliac artery", "celiac arterial trunk", "bladder", "right kidney", "left kidney",
        "left ureter", "right ureter", "posterior back", "left iliac bone", "spinal cord and ribs",
        "sacrum", "right femur", "left femur", "right iliac bone", "uterus and vagina",
        "heart", "spleen", "liver", "urachus", "left umbilical ligament",
        "right umbilical ligament"
    }; // Example array of desired names: REMOVE ANTERIOR WALL FROM MODELS FOLDER


    public GameObject modelsParent; // Parent GameObject to hold the instantiated models
    public string cameraName;
    //public GameObject tipParent; // Parent GameObject to hold the instantiated tool tip
    // New variables for the button and its states
    private GameObject mainButton;
    private GameObject changeViewButton;
    private bool areModelsShown = true;

    private void Start()

    {   // Call the method to load the models by ascending order number
        LoadModels();

        // Call the method to add the specific material for each model
        AddMaterial();

        // Call the method to display Unity screen in two halves
        DisplayScreenInTwoHalves();

        //Call the method to create main button for show or hide all organs
        MainButton();

        // Create and configure the button
        CreateChangeViewButton();

        // Deactivate the sagittal game object and its children by default
        sagittalGameObject.SetActive(false);


    }
    private void Update()

    {

        
    }

    // Function for loading all the models and the components and scripts attached to them.
    private void LoadModels()
    {
        // Load all models from the specified folder
        GameObject[] models = Resources.LoadAll<GameObject>(modelsFolderPath);

        // Sort the models based on the number in their name in ascending order
        List<GameObject> sortedModels = models.OrderBy(model =>
        {
            int number = ExtractNumber(model.name);
            return number;
        }).ToList();

        // Create a parent GameObject if it doesn't exist
        if (modelsParent == null)
        {
            modelsParent = new GameObject("ModelsParent");
        }

        // Instantiate the models as child objects under the parent GameObject
        for (int i = 0; i < sortedModels.Count; i++)
        {
            GameObject instantiatedModel = Instantiate(sortedModels[i], modelsParent.transform);

            // Add MeshFilter component
            MeshFilter meshFilter = instantiatedModel.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = instantiatedModel.AddComponent<MeshFilter>();
            }

            // Add MeshCollider component
            MeshCollider meshCollider = instantiatedModel.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = instantiatedModel.AddComponent<MeshCollider>();
            }

            // Add MeshRenderer component
            MeshRenderer meshRenderer = instantiatedModel.AddComponent<MeshRenderer>();


            // Find and assign the mesh to the MeshFilter based on the original model's name
            string originalName = sortedModels[i].name;
            string prefabPath = Path.Combine(modelsFolderPath, originalName);
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab != null)
            {
                // Instantiate the prefab to get access to its mesh
                GameObject prefabInstance = Instantiate(prefab);
                Mesh prefabMesh = prefabInstance.GetComponentInChildren<MeshFilter>().sharedMesh;

                if (prefabMesh != null)
                {
                    // Assign the prefab's mesh to the instantiated model's MeshFilter and MeshCollider
                    meshFilter.sharedMesh = prefabMesh;
                    meshCollider.sharedMesh = prefabMesh;
                }

                // Destroy the instantiated prefab
                Destroy(prefabInstance);
            }

            // Change the game object name to the desired name from the array
            if (i < desiredNames.Length)
            {
                instantiatedModel.name = desiredNames[i];
            }
            else
            {
                // Handle the case where there are more models than names in the array
                instantiatedModel.name = "ARSupportPiece" + i.ToString();
            }
        }
    }

    // Function to add the material to each model taking into account the name. 
    private void AddMaterial()
    {
        // Load the materials from the Resources folder
        Material veinMaterial = Resources.Load<Material>("Materials/vein");
        Material arteryMaterial = Resources.Load<Material>("Materials/artery");
        Material kidneyMaterial = Resources.Load<Material>("Materials/kidney");
        Material liverMaterial = Resources.Load<Material>("Materials/liver");
        Material skinMaterial = Resources.Load<Material>("Materials/skin");
        Material ureterMaterial = Resources.Load<Material>("Materials/ureter");
        Material spleenMaterial = Resources.Load<Material>("Materials/spleen");
        Material bloodVesselsMaterial = Resources.Load<Material>("Materials/blood_vessels");
        Material heartMaterial = Resources.Load<Material>("Materials/heart");
        Material uterusMaterial = Resources.Load<Material>("Materials/uterus_vagina");
        Material bladderMaterial = Resources.Load<Material>("Materials/bladder");
        Material boneMaterial = Resources.Load<Material>("Materials/bone");
        Material urachusMaterial = Resources.Load<Material>("Materials/urachus");
        Material ligamentMaterial = Resources.Load<Material>("Materials/ligament");

        // Get all child objects of the models parent
        Transform[] modelTransforms = modelsParent.GetComponentsInChildren<Transform>();

        // Loop through each child object and check its name for a match
        foreach (Transform childTransform in modelTransforms)
        {
            string childName = childTransform.gameObject.name.ToLower();

            if (childName.Contains("ve"))
            {
                // Assign the vein material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = veinMaterial;

            }
            else if (childName.Contains("arter"))
            {
                // Assign the artery material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = arteryMaterial;
            }
            else if (childName.Contains("kidney"))
            {
                // Assign the kidney material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = kidneyMaterial;

            }
            else if (childName.Contains("liver"))
            {
                // Assign the liver material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = liverMaterial;

            }
            else if (childName.Contains("back"))
            {
                // Assign the skin material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = skinMaterial;

            }
            else if (childName.Contains("ureter"))
            {
                // Assign the ureter material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = ureterMaterial;

            }
            else if (childName.Contains("spleen"))
            {
                // Assign the spleen material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = spleenMaterial;

            }
            else if (childName.Contains("urachus"))
            {
                // Assign the blood vessels material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = urachusMaterial;

            }
            else if (childName.Contains("ligament"))
            {
                // Assign the blood vessels material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = ligamentMaterial;

            }
            else if (childName.Contains("heart"))
            {
                // Assign the heart material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = heartMaterial;

            }
            else if (childName.Contains("uterus"))
            {
                // Assign the uterus material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = uterusMaterial;

            }
            else if (childName.Contains("bladder"))
            {
                // Assign the uterus_vagina material
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = bladderMaterial;

            }
            else if (childName.Contains("iliac") || childName.Contains("femur") || childName.Contains("sacrum") || childName.Contains("cord"))
            {
                MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
                meshRenderer.material = boneMaterial;

            }
        }
    }


    // Function for extracting the number in the original model name.
    private int ExtractNumber(string name)
    {
        Match match = Regex.Match(name, @"\d+");
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        return int.MaxValue; // Return a very large value if no number is found
    }

    // Function for splitting the canvas screen in two halves.
    private void DisplayScreenInTwoHalves()
    {
        // Create a Canvas GameObject
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Set the Canvas dimensions to cover the entire screen
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        canvasRectTransform.anchorMin = Vector2.zero;
        canvasRectTransform.anchorMax = Vector2.one;
        canvasRectTransform.offsetMin = Vector2.zero;
        canvasRectTransform.offsetMax = Vector2.zero;

        // Create a RawImage GameObject as a child of the Canvas
        GameObject rawImageObject = new GameObject("RawImage");
        rawImageObject.transform.SetParent(canvasObject.transform, false);
        RawImage rawImage = rawImageObject.AddComponent<RawImage>();

        // Set the RawImage dimensions to cover the right half of the screen
        RectTransform rawImageRectTransform = rawImage.GetComponent<RectTransform>();
        rawImageRectTransform.anchorMin = new Vector2(0.5f, 0f);
        rawImageRectTransform.anchorMax = Vector2.one;
        rawImageRectTransform.pivot = new Vector2(0.5f, 0.5f);
        rawImageRectTransform.offsetMin = Vector2.zero;
        rawImageRectTransform.offsetMax = Vector2.zero;

        // Create an empty GameObject and add a VideoPlayer component and CameraSelector script
        GameObject cameraObject = new GameObject("CameraObject");
        VideoPlayer videoPlayer = cameraObject.AddComponent<VideoPlayer>();
        CameraSelector cameraSelector = cameraObject.AddComponent<CameraSelector>();

        // Assign the RawImage to the CameraSelector script
        cameraSelector.rawImage = rawImage;

        // Set the name of the external USB camera to connect
        cameraSelector.cameraName = cameraName;
    }



    // Function for creating the tool tip and assign it as child of Image marker. 
    //private void ToolTip()
    //{
        // Create a parent GameObject if it doesn't exist
        //if (tipParent == null)
        //{
            //tipParent = new GameObject("ToolTipParent");
        //}

        // Create a sphere GameObject
        //GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //tip.name = "Tip";
        // Set the tag for collisions
        //tip.gameObject.tag = "Player";
        //tip.transform.localScale = Vector3.one * 10f;

        // Set the position of the tip GameObject
        //tip.transform.position = new Vector3(12.2f, -35.2f, 27.9f);

        // Add Rigidbody component without gravity
        //Rigidbody rigidbody = tip.AddComponent<Rigidbody>();
        //rigidbody.useGravity = false;


        // Add SphereCollider component and set it as a trigger
        //SphereCollider sphereCollider = tip.GetComponent<SphereCollider>();
        //if (sphereCollider == null)
        //{
            //sphereCollider = tip.AddComponent<SphereCollider>();
        //}

        // Check the isTrigger condition
        //sphereCollider.isTrigger = true;

        // Set the parent of the sphere GameObject
        //tip.transform.SetParent(tipParent.transform);
    //}

    private void MainButton()
    {
        // Create a Canvas GameObject
        GameObject canvasObject = new GameObject("CanvasShowOrgans");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Set the Canvas dimensions to cover the entire screen
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        canvasRectTransform.anchorMin = Vector2.zero;
        canvasRectTransform.anchorMax = Vector2.one;
        canvasRectTransform.offsetMin = Vector2.zero;
        canvasRectTransform.offsetMax = Vector2.zero;

        // Create a Button GameObject as a child of the Canvas
        GameObject buttonObject = new GameObject("MainButton");
        buttonObject.transform.SetParent(canvasObject.transform, false);
        RectTransform buttonRectTransform = buttonObject.AddComponent<RectTransform>();

        // Set the position and size of the button
        buttonRectTransform.anchorMin = new Vector2(0f, 1f);
        buttonRectTransform.anchorMax = new Vector2(0f, 1f);
        buttonRectTransform.pivot = new Vector2(0f, 1f);
        buttonRectTransform.anchoredPosition = Vector2.zero;
        buttonRectTransform.sizeDelta = new Vector2(200f, 50f);

        // Add the Button component to the button GameObject
        Button buttonComponent = buttonObject.AddComponent<Button>();

        // Add a Text component to the button GameObject and set its properties
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRectTransform = textObject.AddComponent<RectTransform>();
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = "Hide organs";
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontStyle = FontStyle.Bold; // Set the font style to bold
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.white;

        // Set the position and size of the text
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.pivot = new Vector2(0.5f, 0.5f);
        textRectTransform.anchoredPosition = Vector2.zero;
        textRectTransform.sizeDelta = Vector2.zero;

        // Set the text font size based on the button size
        float fontSize = buttonRectTransform.sizeDelta.y * 0.5f;
        textComponent.fontSize = Mathf.FloorToInt(fontSize);

        // Add a yellow Image component to the button GameObject
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.12f, 0.44f, 0.93f); // Set the color to 0C6FEE (hex) or (12, 111, 238) (RGB)

        // Add an onClick event to the button to call the ToggleModels function
        buttonComponent.onClick.AddListener(ToggleModels);

        // Assign the MainButton to the buttonObject
        mainButton = buttonObject;

        // Set the parent of the button GameObject
        buttonObject.transform.SetParent(canvasObject.transform, false);

    }




    // Function for toggling the visibility of the models
    private void ToggleModels()
    {
        // Toggle the visibility of the models
        areModelsShown = !areModelsShown;

        // Get all child objects of the models parent
        Transform[] modelTransforms = modelsParent.GetComponentsInChildren<Transform>();

        // Loop through each child object and toggle the MeshRenderer component
        foreach (Transform childTransform in modelTransforms)
        {
            MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                string modelName = childTransform.name;
                if(modelName.Contains("marker"))

                {
                    meshRenderer.enabled = true;
                }
                else
                {   // If the organs are hidden
                    if (areModelsShown == false)
                    {
                        meshRenderer.enabled = areModelsShown;

                        // Add the "OrganCollision" script to the models. 
                        OrganCollision organCollision = childTransform.gameObject.AddComponent<OrganCollision>();
                    }
                    else
                    {   // If the organs are shown, disable the collider. 
                        meshRenderer.enabled = areModelsShown;

                        // Remove the "OrganCollision" script from the models if it exists
                        OrganCollision organCollision = childTransform.gameObject.GetComponent<OrganCollision>();
                        if (organCollision != null)
                        {
                            Destroy(organCollision);
                        }
                    }
                }
            }
        }

        // Update the button text and color
        Text buttonText = mainButton.GetComponentInChildren<Text>();
        buttonText.text = areModelsShown ? "Hide organs" : "Show organs";

        Image buttonImage = mainButton.GetComponent<Image>();
        if (areModelsShown)
        {
            buttonImage.color = new Color(0.12f, 0.44f, 0.93f); // Set the color to 0C6FEE (hex) or (12, 111, 238) (RGB)
        }
        else
        {
            buttonImage.color = new Color(0.96f, 0.18f, 0.27f); // Set the color to F62E46 (hex) or (246, 46, 70) (RGB)
        }

    }

    
    // Method to create and configure the change view button
    private void CreateChangeViewButton()
    {
        // Create a Canvas GameObject
        GameObject canvasObject = new GameObject("CanvasView");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Set the Canvas dimensions to cover the entire screen
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        canvasRectTransform.anchorMin = Vector2.zero;
        canvasRectTransform.anchorMax = Vector2.one;
        canvasRectTransform.offsetMin = Vector2.zero;
        canvasRectTransform.offsetMax = Vector2.zero;

        // Create a Button GameObject as a child of the Canvas
        GameObject buttonObject = new GameObject("Change view button 2");
        buttonObject.transform.SetParent(canvasObject.transform, false);
        RectTransform buttonRectTransform = buttonObject.AddComponent<RectTransform>();

        // Set the position and size of the button
        buttonRectTransform.anchorMin = new Vector2(0f, 1f);
        buttonRectTransform.anchorMax = new Vector2(0f, 1f);
        buttonRectTransform.pivot = new Vector2(0f, 1f);
        buttonRectTransform.position = new Vector3(250f, -470f, 0f);
        buttonRectTransform.sizeDelta = new Vector2(200f, 30f);

        // Add the Button component to the button GameObject
        Button buttonComponent = buttonObject.AddComponent<Button>();

        // Add a Text component to the button GameObject and set its properties
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRectTransform = textObject.AddComponent<RectTransform>();
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = "Change to Sagittal View";
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontStyle = FontStyle.Bold; // Set the font style to bold
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.white;

        // Set the position and size of the text
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.pivot = new Vector2(0.5f, 0.5f);
        textRectTransform.anchoredPosition = Vector2.zero;
        textRectTransform.sizeDelta = Vector2.zero;

        // Set the text font size based on the button size
        float fontSize = buttonRectTransform.sizeDelta.y * 0.5f;
        textComponent.fontSize = Mathf.FloorToInt(fontSize);

        // Add a yellow Image component to the button GameObject
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = Color.blue;

        // Assign the MainButton to the buttonObject
        changeViewButton = buttonObject;

        // Set the parent of the button GameObject
        buttonObject.transform.SetParent(canvasObject.transform, false);

        // Add an onClick event to the button to call the ToggleModels function
        buttonComponent.onClick.AddListener(ToggleModelsFolderPath);
        buttonComponent.onClick.AddListener(() => DestroyChildObjects(modelsParent.transform));

        // Add a button click event listener
        buttonComponent.onClick.AddListener(LoadModels);

        buttonComponent.onClick.AddListener(AddMaterial);
        buttonComponent.onClick.AddListener(DestroyCanvasObject);
        buttonComponent.onClick.AddListener(MainButton);
        

       
    }


    // Method to toggle between models folder paths when the button is clicked, activating and deactivating tool gameobjects and changing the color and text of the button previously created. 
    private void ToggleModelsFolderPath()
    {
        // Toggle the flag between the alternative path and the default path
        isAlternativePath = !isAlternativePath;
        // Get the Text component of the button
        Text textComponent = changeViewButton.GetComponentInChildren<Text>();
        // Get the Image component of the button
        Image buttonImage = changeViewButton.GetComponent<Image>();

        // Update the models folder path based on the current state
        if (isAlternativePath)
        {
            UpdateModelsFolderPath(SagittalModelsFolderPath);
            // Activate the sagittal game object and its children
            sagittalGameObject.SetActive(true);

            // Deactivate the coronal game object and its children
            coronalGameObject.SetActive(false);

            // Change the text
            textComponent.text = "Change to Coronal View";

            // Change the color
            buttonImage.color = Color.magenta;


        }

        else
        {
            UpdateModelsFolderPath(CoronalModelsFolderPath);
            // Activate the coronal game object and its children
            coronalGameObject.SetActive(true);

            // Deactivate the sagittal game object and its children
            sagittalGameObject.SetActive(false);

            // Change the text
            textComponent.text = "Change to Sagittal View";

            // Change the color
            buttonImage.color = Color.blue;
        }
    }

    // Method to update models folder path and reload models
    public void UpdateModelsFolderPath(string newPath)
    {
        modelsFolderPath = newPath;
        LoadModels();
    }

    public void DestroyChildObjects(Transform parentTransform)
    {
        // Get all child objects of the parent
        Transform[] childTransforms = parentTransform.GetComponentsInChildren<Transform>();

        // Loop through each child transform
        foreach (Transform childTransform in childTransforms)
        {
            // Exclude the parent and the child object containing "marker" from destruction
            if (childTransform != parentTransform && !childTransform.name.Contains("marker"))
            {
                // Destroy the child gameobject
                Destroy(childTransform.gameObject);
            }
        }
    }

    public void DestroyCanvasObject()
    {
        GameObject canvasObject = GameObject.Find("CanvasShowOrgans");
        Destroy(canvasObject);
    }

}