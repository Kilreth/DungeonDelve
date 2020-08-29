using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadcrumbs : MonoBehaviour
{
    // Breadcrumb prefabs to set via Unity editor
    [SerializeField]
    private GameObject redBreadcrumb = null;
    [SerializeField]
    private GameObject blueBreadcrumb = null;
    [SerializeField]
    private GameObject greenBreadcrumb = null;

    // The current color prefab to be dropped
    private GameObject currentBreadcrumb;

    // All instantiated breadcrumbs are children of this empty GameObject
    [HideInInspector]
    public GameObject BreadcrumbsParent;

    // UI anchor transforms to get from the Canvas object
    private RectTransform UIRed;
    private RectTransform UIBlue;
    private RectTransform UIGreen;

    // An arrow that hovers over the active UI breadcrumb
    private RectTransform selectedIndicator;

    // Parameters related to dropping breadcrumbs
    private GameObject player;
    private Collider playerCollider;
    [SerializeField]
    private float dropHeight = 0.7f;
    [SerializeField]
    private float throwStrength = 0.8f;
    [SerializeField]
    private float pickUpRadius = 0.5f;
    [SerializeField]
    private float pickUpDistance = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        // This object will already be populated if we loaded a save
        if (BreadcrumbsParent == null)
            BreadcrumbsParent = new GameObject("Breadcrumbs");

        player = gameObject;
        playerCollider = player.GetComponent<Collider>();

        UIRed = GM.Instance.Canvas.transform.Find(
            "RedBreadcrumb").gameObject.GetComponent<RectTransform>();
        UIGreen = GM.Instance.Canvas.transform.Find(
            "GreenBreadcrumb").gameObject.GetComponent<RectTransform>();
        UIBlue = GM.Instance.Canvas.transform.Find(
            "BlueBreadcrumb").gameObject.GetComponent<RectTransform>();
        selectedIndicator = GM.Instance.Canvas.transform.Find(
            "SelectedIndicator").gameObject.GetComponent<RectTransform>();

        SelectBreadcrumb(blueBreadcrumb, UIBlue);
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.Instance.GameState != GameState.Active || GM.Instance.MapActive)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            DropBreadcrumb();
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            PickUpBreadcrumbs();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentBreadcrumb == blueBreadcrumb)
                SelectBreadcrumb(greenBreadcrumb, UIGreen);
            else if (currentBreadcrumb == redBreadcrumb)
                SelectBreadcrumb(blueBreadcrumb, UIBlue);
            else if (currentBreadcrumb == greenBreadcrumb)
                SelectBreadcrumb(redBreadcrumb, UIRed);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentBreadcrumb == blueBreadcrumb)
                SelectBreadcrumb(redBreadcrumb, UIRed);
            else if (currentBreadcrumb == redBreadcrumb)
                SelectBreadcrumb(greenBreadcrumb, UIGreen);
            else if (currentBreadcrumb == greenBreadcrumb)
                SelectBreadcrumb(blueBreadcrumb, UIBlue);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectBreadcrumb(blueBreadcrumb, UIBlue);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectBreadcrumb(redBreadcrumb, UIRed);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectBreadcrumb(greenBreadcrumb, UIGreen);
        }
    }

    public void LoadBreadcrumbs(List<ItemSave> breadcrumbs)
    {
        Dictionary<string, GameObject> colorToPrefab = new Dictionary<string, GameObject>()
        {
            { "r", redBreadcrumb },
            { "g", greenBreadcrumb },
            { "b", blueBreadcrumb },
        };

        BreadcrumbsParent = new GameObject("Breadcrumbs");
        foreach (ItemSave b in breadcrumbs)
        {
            Instantiate(colorToPrefab[b.Name], b.Position, b.Rotation,
                        BreadcrumbsParent.transform);
        }
    }

    private void SelectBreadcrumb(GameObject chosenBreadcrumb, RectTransform UIBreadcrumb)
    {
        currentBreadcrumb = chosenBreadcrumb;
        selectedIndicator.anchorMin = new Vector2(UIBreadcrumb.anchorMin.x, selectedIndicator.anchorMin.y);
        selectedIndicator.anchorMax = new Vector2(UIBreadcrumb.anchorMax.x, selectedIndicator.anchorMax.y);
        Jukebox.Instance.PlaySFX("Toggle breadcrumb");
    }

    private void PickUpBreadcrumbs()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 9;
        RaycastHit[] collisions = Physics.SphereCastAll(ray, pickUpRadius, pickUpDistance, layerMask);
        for (int i = 0; i < collisions.Length; ++i)
        {
            Destroy(collisions[i].transform.gameObject);
        }
        Jukebox.Instance.PlaySFX("Pick up breadcrumbs");
    }

    private void DropBreadcrumb()
    {
        Vector3 position = new Vector3(player.transform.position.x,
                                       player.transform.position.y + dropHeight,
                                       player.transform.position.z);
        GameObject breadcrumb = Instantiate(currentBreadcrumb, position, Random.rotation, BreadcrumbsParent.transform);
        breadcrumb.GetComponent<Rigidbody>().AddForce(
            player.transform.forward * GM.Instance.BlockScale * throwStrength, ForceMode.Impulse);
        Physics.IgnoreCollision(playerCollider, breadcrumb.GetComponent<Collider>());
        Jukebox.Instance.PlaySFX("Drop breadcrumb", UnityEngine.Random.Range(0.6f, 1.2f));
    }
}
