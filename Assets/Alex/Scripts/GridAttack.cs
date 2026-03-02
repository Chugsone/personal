using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GridAttack : MonoBehaviour
{
    private GameObject bulletPrefab;
    private GameObject warningPrefab;

    [SerializeField] private float arenaSize = 26f;
    [SerializeField] private float lineSpacing = 1.5f;
    [SerializeField] private float lineWidth = 0.5f;
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float delayAfterWarning = 0.5f;
    [SerializeField] private float delayBetween = 0.5f;


    private float angleOffset;
    private List<GameObject> activeLines = new List<GameObject>();

    void Start()
    {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/BossProj");
        warningPrefab = Resources.Load<GameObject>("Prefabs/Warning");
    }

    public void Attack()
    {
        StartCoroutine(SliceAttack());
        
    }

    IEnumerator SliceAttack()
    {
        CreateSlice();
        yield return new WaitForSeconds(warningDuration);

        ClearGrid();
        yield return new WaitForSeconds(delayAfterWarning);

        FireProjectiles();
        if (angleOffset >= 181f)
        {
            angleOffset = 0f;
            yield break;
        }
        yield return new WaitForSeconds(delayBetween);

        StartCoroutine(SliceAttack());


    }

    private void CreateSlice()
    {
        float half = arenaSize / 2f;
        for (float x = -half; x <= half; x += lineSpacing)
        {
            GameObject line = Instantiate(warningPrefab, transform.position, Quaternion.Euler(0f, 0f, angleOffset)); //Instead of around the boss make it around the player
            LineRenderer lr = line.GetComponent<LineRenderer>();

            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;

            Vector2 center = transform.position;

            Vector2 dir = new Vector2(Mathf.Cos(angleOffset * Mathf.Deg2Rad), Mathf.Sin(angleOffset * Mathf.Deg2Rad));
            Vector2 offset = Vector2.Perpendicular(dir) * x;
            Vector2 start = center + offset - dir * half;
            Vector2 end = center + offset + dir * half;


            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.startColor = Color.red;
            lr.endColor = Color.red;

            activeLines.Add(line);

        }
    }

    private void ClearGrid()
    {
       foreach (GameObject line in activeLines)
        {
            Destroy(line);
        } 
        activeLines.Clear();
    }

    private void FireProjectiles()
    {
        float half = arenaSize / 2f;
        for (float x = -half; x <= half ; x += lineSpacing)
        {
            Vector2 center = transform.position;

            Vector2 dir = new Vector2(Mathf.Cos(angleOffset * Mathf.Deg2Rad), Mathf.Sin(angleOffset * Mathf.Deg2Rad));
            Vector2 offset = Vector2.Perpendicular(dir) * x;
            Vector2 start = center + offset - dir * half;
            Vector2 end = center + offset + dir * half;


            GameObject proj = Instantiate(bulletPrefab, start, Quaternion.Euler(0f, 0f, angleOffset));
            proj.GetComponent<BossProj>().ProjectileSpeed = 70f;
            proj.GetComponent<BossProj>().IgnoreGround = true;
        }
        angleOffset += 30f;
    }


}
