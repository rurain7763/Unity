using UnityEngine;

public class Enemy : MonoBehaviour
{
    Stat myStat;

    public Stat MyStat
    {
        get
        {
            return myStat;
        }

        set
        {
            myStat = value;
        }
    }

    static Transform target;
    public GameObject hp;
    Hp hpScript;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;

        GameObject go= Instantiate<GameObject>(hp,transform);
        go.transform.localPosition=new Vector3(0,5,0);
        hpScript = go.GetComponent<Hp>();
    }

    private void Start()
    {
        hpScript.curhealthPoint = myStat.healthPoint;
        hpScript.maxhealthPoint = myStat.healthPoint;
        hpScript.curSize = 10;
        hpScript.maxSize = 10;
    }

    private void Update()
    {
        Rotate();
        Chase();
        DetectCollision();
    }

    void Chase()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * myStat.speed);
    }

    float rotSpeed = 2.0f;

    void Rotate()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotSpeed * Time.deltaTime);

    }

    public LayerMask layerMask;

    void DetectCollision()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, 
            GetComponent<BoxCollider>().bounds.extents,transform.rotation,layerMask);

        if(cols.Length != 0)
        {
            cols[0].gameObject.GetComponent<PlayerController>().TakeDamage(myStat.power);
            Destroy(gameObject);
        }
    }

    public GameObject damagedEffect;

    public void TakeDamage(int damage)
    {
        Instantiate<GameObject>(damagedEffect,transform.position,Quaternion.identity);

        myStat.healthPoint -= damage;
        hpScript.TakeDamage(damage);

        if(myStat.healthPoint <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
