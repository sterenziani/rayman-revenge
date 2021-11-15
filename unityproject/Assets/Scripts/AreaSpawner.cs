using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawner : MonoBehaviour
{
    public float XSpread = 10f;
    public float YSpread = 0;
    public float ZSpread = 10f;

    [SerializeField] List<Collider> exclusionZones;

    public GameObject[] items;

    private Coroutine spawningCoroutine;

    public void BeginSpawning(float timeBetweenSpawns, int? spawnsMax = null, Vector3? origin = null)
    {
        StopSpawning();

        Vector3 location = transform.position;
        if (origin != null)
            location = origin.Value;

        spawningCoroutine = StartCoroutine(SpawnCoroutine(timeBetweenSpawns, spawnsMax, location));
    }

    public void StopSpawning()
    {
        if(spawningCoroutine != null)
            StopCoroutine(spawningCoroutine);

        StopAllCoroutines();
    }

    private bool SpawnItem(GameObject item, Vector3? vec = null)
    {
        Vector3 location = transform.position;
        if (vec != null)
            location = vec.Value;
        Vector3 offset = new Vector3(Random.Range(-XSpread, XSpread), 1, Random.Range(-ZSpread, ZSpread));
        Vector3 origin = location + offset;

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

    private IEnumerator SpawnCoroutine(float timeBetweenSpawns, int? remainingSpawns = null, Vector3? origin = null)
    {
        while (remainingSpawns == null || remainingSpawns > 0)
        {
            if (remainingSpawns.HasValue)
                remainingSpawns--;
            for (int i = 0; i < items.Length; i++)
            {
                int attempts = 0;
                while (attempts < 100 && !SpawnItem(items[i]))
                {
                    attempts++;
                }
            }
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
