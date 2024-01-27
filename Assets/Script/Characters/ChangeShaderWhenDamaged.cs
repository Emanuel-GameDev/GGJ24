using UnityEngine;

public class ChangeShaderWhenDamaged : MonoBehaviour
{
    [SerializeField]
    private Color colorWhenDamaged;

    [SerializeField] float fadeSpeed = 1;

    private SpriteRenderer spriteRenderer;

    private Material _material;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _material = spriteRenderer.material;
    }

    private void Update()
    {
        if (colorWhenDamaged.a > 0)
        {
            colorWhenDamaged.a = Mathf.Clamp01(colorWhenDamaged.a - fadeSpeed * Time.deltaTime);
            _material.SetColor("_Tint", colorWhenDamaged);
        }
    }

    public void TakeDamage(Color newColor)
    {
        colorWhenDamaged = newColor;
        _material.SetColor("_Tint", colorWhenDamaged);
    }
}
