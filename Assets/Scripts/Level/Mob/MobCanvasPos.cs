using UnityEngine;

public class MobCanvasPos : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height;

    private void Awake()
    {
        if (target != null)
            return;

        Transform root = transform.root;
        MobController mob = root != null
            ? root.GetComponentInChildren<MobController>(true)
            : null;
        if (mob != null)
            target = mob.transform;
    }

    private void Update()
    {
        if (target == null)
            return;
        transform.position = new Vector2(target.position.x, target.position.y + height);
    }
}
