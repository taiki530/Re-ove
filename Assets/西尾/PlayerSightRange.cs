using System.Collections.Generic;
using UnityEngine;

public class PlayerSightRange : MonoBehaviour
{
    LineRenderer line;
    [SerializeField] int angle = 90;
    [SerializeField] float distance = 10.0f;

    List<Vector3> positions = new List<Vector3>();

    private PlayerSightChecker sightChecker;
    private GameObject target;

    private void Start()
    {
        if (CompareTag("TimeLoopPlayer"))
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;

            line.material = new Material(Shader.Find("Sprites/Default"));
            line.material.color = Color.green;

            sightChecker = GetComponent<PlayerSightChecker>();
            if (sightChecker == null)
            {
                Debug.LogError("PlayerSightChecker ��������܂���B");
            }

            target = GameObject.Find("PlayerRoot");
            if (target == null)
            {
                Debug.LogError("�^�[�Q�b�g��������܂���B");
            }
        }
    }

    private void Update()
    {
        if (CompareTag("TimeLoopPlayer"))
        {
            positions.Clear();
            positions.Add(transform.position);

            Vector3 axis = transform.up;
            Vector3 forward = transform.forward;

            for (int i = -angle / 2; i <= angle / 2; i++)
            {
                Quaternion rot = Quaternion.AngleAxis(i, axis);
                Vector3 dir = rot * forward;
                Vector3 pos = transform.position + dir * distance;
                positions.Add(pos);
            }

            positions.Add(transform.position);

            line.positionCount = positions.Count;
            line.SetPositions(positions.ToArray());

            if (sightChecker != null && target != null)
            {
                Vector3 selfPos = transform.position;
                Vector3 targetPos = target.transform.position;
                Vector3 targetDir = targetPos - selfPos;

                if (sightChecker.IsVisible())
                {
                    line.material.color = Color.red;
                }
                else
                {
                    // ���E�͈͓������ǎՕ����Ō����Ȃ����� Raycast �Ń`�F�b�N
                    float targetDistance = targetDir.magnitude;
                    float cosHalf = Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad);
                    float innerProduct = Vector3.Dot(transform.forward, targetDir.normalized);

                    if (innerProduct > cosHalf && targetDistance <= distance)
                    {
                        if (Physics.Raycast(selfPos, targetDir.normalized, out RaycastHit hit, distance))
                        {
                            if (hit.collider.gameObject != target)
                            {
                                // �Օ�������
                                line.material.color = Color.black;
                            }
                            else
                            {
                                // �Օ����Ȃ������� IsVisible() �� false �͂��肦�Ȃ��̂ŗ΂�
                                line.material.color = Color.green;
                            }
                        }
                        else
                        {
                            // Ray �����ɂ�������Ȃ�
                            line.material.color = Color.green;
                        }
                    }
                    else
                    {
                        // ���E�͈͊O
                        line.material.color = Color.green;
                    }
                }
            }
        }
    }
}
