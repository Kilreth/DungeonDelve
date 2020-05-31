using UnityEngine;

public class DungeonMap : MonoBehaviour
{
    private Camera playerCamera;
    private Camera dungeonMapCamera;
    [SerializeField]
    private Camera dungeonMapCameraPrefab;

    [SerializeField]
    private float zoomSensitivity;
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
            float zoomInput = Input.GetAxis("Mouse ScrollWheel");
            dungeonMapCamera.orthographicSize -= zoomInput * zoomSensitivity;

            if (Input.GetKeyDown(KeyCode.Mouse0))
                dragging = true;
            else if (Input.GetKeyUp(KeyCode.Mouse0))
                dragging = false;

            if (dragging)
            {
                float xOffset = Input.GetAxis("Mouse X");
                float yOffset = Input.GetAxis("Mouse Y");

                // transform.Translate() is relative to the object's rotation,
                // so yOffset is passed to y instead of z
                dungeonMapCamera.transform.Translate(-xOffset, -yOffset, 0);
            }
        }
    }

    private void ShowDungeonMap()
    {
        GM.Instance.MapActive = true;
        GM.Instance.Canvas.enabled = false;
        playerCamera.enabled = false;
        dungeonMapCamera.enabled = true;
        CenterCameraOnPlayer();
    }

    private void HideDungeonMap()
    {
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
