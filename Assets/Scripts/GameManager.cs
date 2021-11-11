using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject plane;
    [SerializeField] private Material[] planeMats;
    [SerializeField] private Material finishPlaneMat;
    public int length;

    void Start()
    {
        int effectedSubPlane = -1;
        int selectedMat = -1;
        var offsetZ = plane.transform.localScale.z;
        Vector3 lastInstantiatePos = Vector3.zero;
        for (int i = 0; i < length; i++)
        {
            if (i % 2 == 0)
            {
                lastInstantiatePos += new Vector3(0, 0, offsetZ);
                Instantiate(plane, lastInstantiatePos, Quaternion.identity, this.transform);
                continue;
            }

            lastInstantiatePos += new Vector3(0, 0, offsetZ);
            var instantiatedPlane = Instantiate(plane, lastInstantiatePos, Quaternion.identity, this.transform);
            //
            int planeWithEffect = 2; // for every block add 2 subblock with effect
            for (int j = 0; j < planeWithEffect; j++)
            {
                int subPlaneNumber = Random.Range(0, 3);
                while (effectedSubPlane == subPlaneNumber)
                {
                    subPlaneNumber = Random.Range(0, 3);
                }

                effectedSubPlane = subPlaneNumber;
                var subPlane = instantiatedPlane.transform.GetChild(subPlaneNumber);
                var renderer = subPlane.gameObject.GetComponent<Renderer>();
                int randomMat = Random.Range(0, planeMats.Length);
                while (randomMat == selectedMat)
                {
                    randomMat = Random.Range(0, planeMats.Length);
                }

                selectedMat = randomMat;
                if (selectedMat == 0)
                {
                    subPlane.gameObject.AddComponent<Candle>();
                }
                else if (selectedMat == 1)
                {
                    subPlane.gameObject.AddComponent<Lava>();
                }

                renderer.sharedMaterial = planeMats[randomMat];
            }

            effectedSubPlane = -1;
            selectedMat = -1;
        }

        //create finish plane
        for (int j = 0; j < 30; j++)
        {
            var finishPlaneInstantiated = Instantiate(plane, lastInstantiatePos + new Vector3(0, 0, offsetZ),
                Quaternion.identity, this.transform);
            lastInstantiatePos += new Vector3(0, 0, offsetZ);
            if (j % 2 == 0)
            {
                for (int i = 0; i < finishPlaneInstantiated.transform.childCount; i++)
                {
                    var subPlane = finishPlaneInstantiated.transform.GetChild(i);

                    subPlane.gameObject.AddComponent<FinishPlane>();
                    var renderer = subPlane.gameObject.GetComponent<Renderer>();
                    renderer.sharedMaterial = finishPlaneMat;
                }
            }
        }
    }
}