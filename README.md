<div dir="rtl">
<p>בס"ד</p>
<h3>קישור למשחק</h3>
https://izk.itch.io/hello-3d-world
<h3>הוראות</h3>
<p>הליכה: קליק שמאלי</p>
<p>הזזת מצלמה: חיצים ימין ושמאל או מקשים A, D</p>
<p>זום מצלמה: גלגלת</p>
<h3>צילום מסך</h3>
<img align="center" width="50%" src="https://img.itch.zone/aW1nLzM2MjQ3MzAucG5n/original/DSgFq8.png">
<h3>הסבר</h3>  
<p>משימה זו מדגימה את השימוש בבינה המלאכותית של מנוע המשחק Unity.</p>
<p>למציאת מסלול הזזת הדמות הראשית השתמשנו ב NavMeshAgent, כאשר נקודת היעד נקבעת ע"י השחקן באמצעות קליק שמאלי.</p>
<p>בכדי למצוא את הנקודה הטלנו קרן באמצעות Physics.Raycast.</p><p>מלבד כוונות טובות, הדרך רצופה במכשולים כמו עצים, מדורה וכדו' שלא ניתן לעבור אותם, וכמו נהר שניתן לחצות רק באמצעות הגשר.</p>

<h3 dir="rtl">בחירת המשתמש לאן להזיז את הדמות</h3> 

```cs
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public LayerMask movementMask; // מפלטר כל מה שלא ניתן ללכת עליו
    public Interactable focus; // על מה ממוקד הפוקוס של השחקן

    Camera cam; // משתנה יחוס למצלמה
    PlayerMotor motor; // משתנה יחוס למנוע שמזיז את השחקן

    void Start()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // אם הכפתור השמאלי של העכבר נלחץ
        {
            RaycastHit hitInfo; // משתנה יחוס כדי שיקבל ערך מהקריאה הבאה לפונקציה
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, movementMask))
            {
                // משתנה היחוס מקבל את הקולידר שעליו נלחץ סמן העכבר וכן את מיקומו
                // Debug.Log("We hit " + hitInfo.collider.name + " " + hitInfo.point);

                // הזזת השחקן לנקודה שעליה נלחץ סמן העכבר
                motor.MoveToPoint(hitInfo.point);

                RemoveFocus();
            }
        }

        if (Input.GetMouseButtonDown(1)) // אם הכפתור הימני של העכבר נלחץ
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo, 100))
            {
                // אם הוא נלחץ על אוביקט שניתן לקיים איתו אינטרקציה
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (interactable)
                    SetFocus(interactable);
            }
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus) // אם הפוקוס השתנה
        {
            if (focus) // וקיים פוקוס ישן
                focus.OnDefocused(); // נבטל את הישן
            focus = newFocus; // נעדכן את החדש
            motor.FollowTarget(newFocus); // ונעדכן את המנוע שמזיז את השחקן בחדש
        }
        newFocus.OnFocused(transform);
    }

    void RemoveFocus()
    {
        if (focus) // אם קיים פוקוס
        {
            focus.OnDefocused();
            focus = null;
        }
        motor.StopFollowTarget();
    }
}
```

<h3 dir="rtl">מנוע הזזת הדמות</h3>

```cs
using UnityEngine;
using UnityEngine.AI;

/* NavMeshAgent הזזת השחקן באמצעות */

[RequireComponent(typeof(NavMeshAgent))] // באופן אוטומטי ברגע שמשתמשים בקומפוננטה NavMeshAgent הוספת
public class PlayerMotor : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target; // המטרה שהסוכן עוקב אחריה

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target) // עדכון הסוכן כשנבחרה מטרה
        {
            agent.SetDestination(target.position);
            FaceTarget();
        }
    }

    public void MoveToPoint (Vector3 point)
    {
        agent.SetDestination(point); // הזזת השחקן למטרה
    }

    public void FollowTarget (Interactable newTarget)
    {
        agent.stoppingDistance = newTarget.radius * .8f; // (קביעת טווח מרחק עצירת הסוכן לפני המטרה (ולא בתוכה

        agent.updateRotation = false; // אם המטרה התקרבה לסוכן מעבר לטווח הוא לא יפנה אליה פנים
                                      // FaceTarget לכן במקרה כזה נבטל את הפניית הפנים האוטומטית ונשתמש בפונקציה

        target = newTarget.interactionTransform; // הזזת הסוכן למטרה
    }

    public void StopFollowTarget()
    {
        agent.stoppingDistance = 0f;
        agent.updateRotation = true;
        target = null;
    }

    void FaceTarget () // הפונקציה מגדירה שהסוכן יפנה פניו למטרה גם אם הוא עצר בטווח שנקבע לו לפני המטרה ולאחר מכן היא זזה בתוך הטווח
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
```

<h3 dir="rtl">מצלמת המעקב אחר הדמות המסתובבת בהתאם למיקומו בעולם</h3> 

```cs
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
```
