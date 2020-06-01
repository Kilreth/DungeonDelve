using UnityEngine;

public class DungeonMap : MonoBehaviour
{
    private Camera playerCamera;
    private Camera dungeonMapCamera;
    [SerializeField]
    private Camera dungeonMapCameraPrefab;

    [SerializeField]
    private float zoomSensitivity;
    [SerializeField]
    private float dragSensitivity;
    private bool dragging;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = gameObject.transform.Find(
            "FirstPersonCharacter").gameObject.GetComponent<Camera>();
        dungeonMapCamera = Instantiate(dungeonMapCameraPrefab);
        dungeonMapCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GM.Instance.MapActive)
                HideDungeonMap();
            else
                ShowDungeonMap();
        }
        if (GM.Instance.MapActive)
        {
            dungeonMapCamera.orthographicSize -= (Input.GetAxis("Mouse ScrollWheel")
                * dungeonMapCamera.orthographicSize * zoomSensitivity);
            if (dungeonMapCamera.orthographicSize < 1)
                dungeonMapCamera.orthographicSize = 1;

            if (Input.GetKeyDown(KeyCode.Mouse0))
                dragging = true;
            else if (Input.GetKeyUp(KeyCode.Mouse0))
                dragging = false;

            if (dragging)
            {
                float xOffset = Input.GetAxis("Mouse X") * dungeonMapCamera.orthographicSize * dragSensitivity;
                float yOffset = Input.GetAxis("Mouse Y") * dungeonMapCamera.orthographicSize * dragSensitivity;

                // transform.Translate() is relative to the object's rotation,
                // so yOffset is passed to y instead of z
                dungeonMapCamera.transform.Translate(-xOffset, -yOffset, 0);
            }
        }
    }

    private void ShowDungeonMap()
    {
        GM.Instance.UnreachableBlocksParent.SetActive(true);
        GM.Instance.MapActive = true;
        GM.Instance.Canvas.enabled = false;
        playerCamera.enabled = false;
        dungeonMapCamera.enabled = true;
        CenterCameraOnPlayer();
    }

    private void HideDungeonMap()
    {
        GM.Instance.UnreachableBlocksParent.SetActive(false);
        GM.Instance.MapActive = false;
        GM.Instance.Canvas.enabled = true;
        playerCamera.enabled = true;
        dungeonMapCamera.enabled = false;
    }

    private void CenterCameraOnPlayer()
    {
        dungeonMapCamera.transform.position = new Vector3(
            GM.Instance.Player.transform.position.x,
            30,
            GM.Instance.Player.transform.position.z);
    }
}
