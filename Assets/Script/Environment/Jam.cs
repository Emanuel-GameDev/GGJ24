public class Jam : EnvironmentEffects
{

    // Modifica della potenza del salto causata dalla marmellata
    public float jumpPowerReduction = 0.5f;

    // Override del metodo per applicare l'effetto specifico della marmellata
    protected override void ApplyEffect()
    {
        base.ApplyEffect();
        if (playerController != null)
        {
            // Riduci la potenza del salto
            playerController.SetJumpPower(playerController.GetJumpPower() * jumpPowerReduction);
        }
    }

    // Override del metodo per ripristinare l'effetto specifico della marmellata
    protected override void ResetEffect()
    {
        base.ResetEffect();
        if (playerController != null)
        {
            // Ripristina la potenza del salto
            playerController.ResetJumpPower();
        }
    }
}
