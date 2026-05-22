using UnityEngine;

public class OpticsTabSystem : MonoBehaviour
{
    public GameObject raysPage;
    public GameObject particlesPage;

    public void ShowRays()
    {
        raysPage.SetActive(true);
        particlesPage.SetActive(false);
    }

    public void ShowParticles()
    {
        raysPage.SetActive(false);
        particlesPage.SetActive(true);
    }
}