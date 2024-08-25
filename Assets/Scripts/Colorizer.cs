using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorizer : ProjectBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isInBackGround = false;
    private ParallaxEffect parallaxEffect;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (TryGetComponent<ParallaxEffect>(out var parallaxEffect))
        {
            isInBackGround = true;
            this.parallaxEffect = parallaxEffect;
        }
    }

    private void Start()
    {
        if (isInBackGround)
        {
            Color color = Color.Lerp(GameManager.BackgroundColor1, GameManager.BackgroundColor2, (parallaxEffect.AmountOfParallax - 0.125f) * 2.5f); //spriteRenderer.color;
            //color -= Color.grey / 2 * parallaxEffect.AmountOfParallax;
            //color.r -= GameManager.BackgroundColor.r / 2 * (parallaxEffect.AmountOfParallax * 1.5f);
            //color.g -= GameManager.BackgroundColor.g / 2 * (parallaxEffect.AmountOfParallax * 1.5f);
            //color.b -= GameManager.BackgroundColor.b / 2 * (parallaxEffect.AmountOfParallax * 1.5f);
            spriteRenderer.color = color;
            return;
        }

        spriteRenderer.color = GameManager.ForegroundColor;
    }
}
