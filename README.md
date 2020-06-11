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

<h3>בחירת המשתמש לאן להזיז את הדמות</h3> 

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
</div>
