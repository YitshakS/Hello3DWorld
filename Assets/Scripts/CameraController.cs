using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset; // ביחס לשחקן X, Y, Z מיקום המצלמה בצירים
    public Transform target; // המטרה שהמצלמה עוקבת אחריה
    public float pitch = 2f; // הזווית שהמצלמה נוטה
    public float yawSpeed = 100f; // מהירות סיבוב המצלמה

    // זום לשחקן עם גלגלת העכבר
    public float currentZoom = 10f; // הזום הנוכחי
    public float minZoom = 5f; // הזום הכי קרוב
    public float maxZoom = 15f; // הזום הכי רחוק
    public float speedZoom = 4f; // מהירות הזום

    float currentTaw = 0f;

    void Update()
    {
        // גלגלת עכבר לפנים מקרבת זום לשחקן
        // גלגלת עכבר לאחור מרחקת זום מהשחקן
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * speedZoom;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // מסובבים את המצלמה לימין D חץ ימני או מקש
        // מסובבים את המצלמה לשמאל A חץ שמאלי או מקש
        currentTaw -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
    }

    void LateUpdate() // עדכון מצלמת מעקב אחרי שכל פונקציות העדכון נקראו
    {
        transform.position = target.position - offset * currentZoom;
        transform.LookAt(target.position + Vector3.up * pitch);
        transform.RotateAround(target.position.normalized, Vector3.up, currentTaw);
    }
}