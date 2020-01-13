using TMPro;
using UnityEngine;

public class MoveOnlinePlayer : MonoBehaviour
{
    public int playerID;
    public Vector3 currentPos;
    public Vector3 currentRot;
    public Animator anim;
    public float inputX;
    public float inputZ;
    public string playerName;
    public TextMeshPro playerNameText;

    void Update()
    {
        playerNameText.text = playerName;
        transform.position = currentPos;
        transform.localRotation = Quaternion.Euler(currentRot.x, currentRot.y, currentRot.z);
        anim.SetFloat("InputX", inputX);
        anim.SetFloat("InputZ", inputZ);
    }
}
