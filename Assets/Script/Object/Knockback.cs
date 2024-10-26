using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField, Tooltip("Durata in cui viene applicato il knockback, N.B. modificare questa var incide sulla knockbackForceCurve")]
    private float knockbackTime = 0.2f;

    [SerializeField, Tooltip("La potenza del Knockback, applicata in base alla provenienza del colpo")]
    private float hitDirectionForce = 10f;

    [SerializeField, Tooltip("Una forza costante (verso l'alto) che viene sommato alla forza")]
    private float constForceDirection = 3f;

    [SerializeField, Tooltip("Permette un controllo più preiso sull'intensità della curva, esempio: parte piano e accelera fino al suo massimo")]
    private AnimationCurve knockbackForceCurve;

    public bool isKnockingBack {get; private set;}

    private Rigidbody2D rb;
    private Coroutine knockbackCoroutine;

    private IEnumerator KnockbackAction(GameObject objToKnock, Vector2 hitDirection, Vector2 constForce)
    {
        isKnockingBack = true;

        rb = objToKnock.GetComponent<Rigidbody2D>();

        Vector2 hitForce;
        Vector2 constantForce;
        Vector2 knockbackForce;
        float time = 0f;

        hitForce = hitDirection * hitDirectionForce;
        constantForce = constForce * constForceDirection;

        float elapsedTime = 0f;

        while (elapsedTime <= knockbackTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            time += Time.fixedDeltaTime;

            // Aggiorno l'hitforce
            hitForce = hitDirection * hitDirectionForce * knockbackForceCurve.Evaluate(time);

            // Unisco hitForce e constForce
            knockbackForce = hitForce + constantForce;

            // Applico il knockback
            rb.velocity = knockbackForce;

            yield return new WaitForFixedUpdate();
        }

        isKnockingBack = false;
    }

    public void CallKnockback(GameObject objToKnock, Vector2 hitDirection, Vector2 constForceDirection)
    {
        knockbackCoroutine = StartCoroutine(KnockbackAction(objToKnock, hitDirection, constForceDirection));
    }
}
