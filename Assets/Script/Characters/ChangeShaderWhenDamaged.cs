using UnityEngine;

public class ChangeShaderWhenDamaged : MonoBehaviour
{
    [SerializeField]
    private Shader shader;

    [SerializeField]
    private Color colorWhenDamaged;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void TakeDamage()
    {
    }
}
