using UnityEngine;

public class SideMoverScript : MonoBehaviour
{
    public float maxScale = 5f;
    public float extendSpeed = 1f;
    public float retractSpeed = 0.5f;
    private enum MoverState { REST, RETRACT, EXPAND };
    private MoverState state;
    
    void Start()
    {
        state = MoverState.REST;
    }

    // Update is called once per frame
    void Update()
    {
        var sx = transform.localScale.x;
        var sy = transform.localScale.y;
        var sz = transform.localScale.z;
        // Debug.Log($"{state} : {sx}");
        if (state == MoverState.EXPAND)
        {
            // transform.localScale.Set(sx + extendSpeed, sy, sz);
            transform.localScale = new Vector3(sx + extendSpeed * Time.deltaTime, sy, sz);
            if (sx >= maxScale)
            {
                state = MoverState.RETRACT;
            }
        }
        if (state == MoverState.RETRACT)
        {
            transform.localScale = new Vector3(sx - retractSpeed * Time.deltaTime, sy, sz);
            if (sx <= 0)
            {
                state = MoverState.REST;
                transform.localScale = new Vector3(0, sy, sz);
            }
        }
    }

    public void Fling()
    {
        state = MoverState.EXPAND;
    }
}
