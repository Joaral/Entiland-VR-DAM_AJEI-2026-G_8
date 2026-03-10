using UnityEngine;

public class TargetHit : MonoBehaviour
{
    [Header("Hit Effects")]
    public GameObject hitDecal;
    public ParticleSystem hitParticle;

    [Header("Score")]
    public int scoreValue = 10;

    private TargetMovement targetMovement;

    void Start()
    {
        targetMovement = GetComponent<TargetMovement>();
    }

    public void OnShot(RaycastHit hit)
    {
        CreateHitParticle(hit);
        CreateHitDecal(hit);

        if (targetMovement != null)
            targetMovement.wasShoot = true;

        //GameManager.Instance.AddScore(scoreValue);
    }

    void CreateHitParticle(RaycastHit hit)
    {
        if (hitParticle == null) return;

        ParticleSystem particle = Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal));
        particle.Play();
        Destroy(particle.gameObject, 2f);
    }

    void CreateHitDecal(RaycastHit hit)
    {
        if (hitDecal == null) return;

        GameObject decal = Instantiate(hitDecal, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(decal, 10f);
    }
}
