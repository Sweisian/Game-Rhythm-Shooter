using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Shrink : MonoBehaviour {

    public float targetScale = 0.0f;
    public float shrinkSpeed = 0.1f;
    public float colorThreshold = 0.2f;
    public float killThreshold = 0.1f;

    private SpriteRenderer m_SpriteRenderer;

    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

	// Update is called once per frame
	void Update () {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime * shrinkSpeed);

	    //transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * shrinkSpeed, transform.localScale.y - Time.deltaTime * shrinkSpeed, transform.localScale.z);

        //using x here because its a circle, and all scales should be the same
        if (transform.localScale.x < colorThreshold)
	    {
	        //m_SpriteRenderer.color = Color.green;
	    }

	    if (transform.localScale.x < killThreshold)
	    {
	        Destroy(gameObject);
	    }

    }
}
