using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class obstacleBehaviour : EnemyBasics
{
    protected override void Update()
    {
        HP = Mathf.Clamp(HP, 0, HPMax);

        HPDisplay.text = HP.ToString();

        if (this.HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    protected override private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hurt");
    }
}
