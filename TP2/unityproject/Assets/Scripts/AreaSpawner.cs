using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawner : MonoBehaviour
{
    public float XSpread = 10;
    public float YSpread = 0;
    public float ZSpread = 10;

    [SerializeField] List<Collider> exclusionZones;

    public GameObject item;

    private IEnumerator spawningCoroutine;

    public void BeginSpawning(float timeBetweenSpawns, int? spawnsMax = null)
    {
        spawningCoroutine = SpawnCoroutine(timeBetweenSpawns, spawnsMax);
        StartCoroutine(spawningCoroutine);
    }

    public void StopSpawning()
    {
        StopCoroutine(spawningCoroutine);
    }

    private bool SpawnItem()
    {
        Vector3 offset = new Vector3(Random.Range(-XSpread, XSpread), 0, Random.Range(-ZSpread, ZSpread));
        Vector3 origin = transform.position + offset;

        Debug.DrawRay(origin, Vector3.down);

        RaycastHit hit;
        if(Physics.Raycast(origin, Vector3.down, out hit, YSpread))
        {
            if(!exclusionZones.Contains(hit.collider))
            {
                GameObject obj = Instantiate(item, hit.point, Quaternion.identity);
                obj.SetActive(true);

                return true;
            }
        }

        return false;
    }

    private IEnumerator SpawnCoroutine(float timeBetweenSpawns, int? remainingSpawns = null)
    {
        while (remainingSpawns == null || remainingSpawns > 0)
        {
            if (remainingSpawns.HasValue)
                remainingSpawns--;

            while (!SpawnItem()) ;

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
