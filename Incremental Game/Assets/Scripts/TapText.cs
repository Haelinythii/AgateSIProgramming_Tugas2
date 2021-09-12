using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapText : MonoBehaviour
{
    public float spawnTime = .5f;
    public Text text;

    private float spawnTimeTimer;

    private void OnEnable()
    {
        spawnTimeTimer = spawnTime;
    }

    private void Update()
    {
        spawnTimeTimer -= Time.unscaledDeltaTime;
        if(spawnTimeTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            text.CrossFadeAlpha(0f, 0.5f, false);
            if(text.color.a == 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
