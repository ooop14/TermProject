using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int health;
    public float speed;

    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate() { 
        transform.position -= new Vector3(speed, 0, 0);
    }
}
