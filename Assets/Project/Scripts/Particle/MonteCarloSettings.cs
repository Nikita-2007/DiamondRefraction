using UnityEngine;

[System.Serializable]
public class MonteCarloSettings
{
    public PhotonMode mode = PhotonMode.Collimated;

    public PhotonPattern pattern =
        PhotonPattern.White;

    [Range(100, 100000)]
    public int photonCount = 10000;

    [Range(1, 50)]
    public int maxBounces = 10;

    [Range(1f, 3f)]
    public float ior = 2.42f;

    public float spread = 0.5f;

    public float exitDistance = 5f;

    public bool chromaticAberration = false;
}