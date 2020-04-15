using TMPro;
using UnityEngine;

public class MoveAIOnline : MonoBehaviour
{
    public int aiID;
    public Vector3 currentPos;
    public Vector3 currentRot;

    public Spinning aiScript;

    private void OnEnable()
    {
    Setup:
        aiScript = GetComponent<Spinning>();
        aiID = aiScript.ID;
        NetworkManager.instance.AIListDictionary.Add(aiID, gameObject);
    }

    void Update()
    {
        if (!NetworkManager.instance.playerPrefab.GetComponent<NetWorkMoving>().isHost && NetworkManager.instance.playerPrefab != null)
        {
            if(aiScript != null)
            aiScript.enabled = false;
            transform.position = currentPos;
            transform.localRotation = Quaternion.Euler(currentRot.x, currentRot.y, currentRot.z);
        }
        if (aiScript != null)
        {
            currentPos = transform.position;
            currentRot = transform.rotation.eulerAngles;
            NetworkManager.instance.ForwardAIPos(aiID + "#" + currentPos.x + "*" + currentPos.y + "*" + currentPos.z + "#" + currentRot.x.ToString("n4") + "*" + currentRot.y.ToString("n4") + "*" + currentRot.z.ToString("n4"));
        }
    }

    public void isHit(float damage)
    {
        NetworkManager.instance.ForwardEnemyState(damage.ToString());
    }
}
