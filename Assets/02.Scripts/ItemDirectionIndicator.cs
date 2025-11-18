using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDirectionIndicator : MonoBehaviour
{
    [Header("UI References")]
    public Image arrowImage;
    public TextMeshProUGUI distanceText;

    [Header("Settings")]
    public float edgeOffset = 50f;
    public Color nearColor = Color.green;
    public Color farColor = Color.red;
    public float nearDistance = 5f;
    public float farDistance = 20f;

    private Transform player;
    private Camera mainCamera;
    private RectTransform canvasRect;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        mainCamera = Camera.main;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        if (arrowImage != null)
        {
            arrowImage.enabled = false;
        }
    }

    void Update()
    {
        if (player == null || arrowImage == null)
            return;

        GameObject nearestItem = FindNearestItem();

        if (nearestItem != null)
        {
            arrowImage.enabled = true;
            UpdateArrow(nearestItem.transform);
        }
        else
        {
            arrowImage.enabled = false;
        }
    }

    GameObject FindNearestItem()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject item in items)
        {
            if (!item.activeInHierarchy)
                continue;

            float distance = Vector3.Distance(player.position, item.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = item;
            }
        }

        return nearest;
    }

    void UpdateArrow(Transform target)
    {
        Vector3 direction = target.position - player.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distanceText != null)
        {
            distanceText.text = distance.ToString("F1") + "m";
        }

        float t = Mathf.InverseLerp(nearDistance, farDistance, distance);
        arrowImage.color = Color.Lerp(nearColor, farColor, t);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width ||
                          screenPos.y < 0 || screenPos.y > Screen.height;

        if (isOffScreen)
        {
            Vector3 cappedPos = screenPos;

            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            cappedPos.x = Mathf.Clamp(screenPos.x, edgeOffset, Screen.width - edgeOffset);
            cappedPos.y = Mathf.Clamp(screenPos.y, edgeOffset, Screen.height - edgeOffset);

            arrowImage.transform.position = cappedPos;

            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 arrowDirection = (Vector2)cappedPos - screenCenter;
            float angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;
            arrowImage.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
        else
        {
            arrowImage.transform.position = screenPos;

            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 arrowDirection = (Vector2)screenPos - screenCenter;
            float angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;
            arrowImage.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
}