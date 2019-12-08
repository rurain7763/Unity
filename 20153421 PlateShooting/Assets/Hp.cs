using UnityEngine;

public class Hp : MonoBehaviour
{
    public int curhealthPoint;
    public int maxhealthPoint;
    public float curSize =10;
    public float maxSize =10;

    private void Start()
    {
        if (GetComponent<Canvas>())
        {
            GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
    }

    public void TakeDamage(int damage)
    {
        curhealthPoint -= damage;

        Vector2 rect =transform.Find("CurrentHp").GetComponent<RectTransform>().sizeDelta;

        float minXScale = maxSize / maxhealthPoint;

        float x = minXScale * curhealthPoint;

        if (x > maxSize)
            x = maxSize;

        else if (x < 0.5 && x > 0) x = 0.05f;

        rect.x = x;

        transform.Find("CurrentHp").GetComponent<RectTransform>().sizeDelta = rect;
    }
}
