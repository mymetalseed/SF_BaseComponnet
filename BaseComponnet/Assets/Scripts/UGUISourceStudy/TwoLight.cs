using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoLight : MonoBehaviour
{
    public Light light1;
    public Light light2;
    public GameObject obj;

    public bool switchLight = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (light1.gameObject.activeInHierarchy)
            {
                light1.gameObject.SetActive(false);
                light2.gameObject.SetActive(true);
                obj.SetActive(false);
            }
            else
            {
                switchLight = true;
            }
        }
    }

    private void OnPostRender()
    {
        if (switchLight)
        {
            switchLight = false;
            light1.gameObject.SetActive(true);
            light2.gameObject.SetActive(false);
            obj.SetActive(true);
        }
    }
}
