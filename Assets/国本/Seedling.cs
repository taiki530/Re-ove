using UnityEngine;

using System.Collections;



public class Seedling : MonoBehaviour

{

    public VineSpawner[] vineSpawners;

    public float interactDistance = 2f;



    private int currentIndex = 0;

    private bool isGrowing = false;

    private float recordedDelay = -1f;

    private Coroutine autoGrowCoroutine;



    void Update()

    {

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton2))

        {

            if (Vector3.Distance(Camera.main.transform.position, transform.position) <= interactDistance)

            {

                RecordTouchTime(VineTimeManager.Instance.GetStartTime());

                TryStartNextVine();

            }

        }

    }



    public void TryStartNextVine()

    {

        if (isGrowing || currentIndex >= vineSpawners.Length)

            return;



        VineSpawner spawner = vineSpawners[currentIndex];



        if (!spawner.HasVine())

        {

            isGrowing = true;

            spawner.SpawnVine();



            VineGrow grow = spawner.SpawnedVine.GetComponent<VineGrow>();

            if (grow != null)

            {

                grow.Initialize(Vector3.up, spawner.transform.position);

                grow.OnFinishedGrow += () =>

                {

                    isGrowing = false;

                    currentIndex++;

                    TryStartNextVine(); // 自動的に次を始める

                };

            }

        }

    }



    public void RecordTouchTime(float startTime)

    {

        float delay = Time.time - startTime;

        if (recordedDelay < 0f || delay < recordedDelay)

        {

            recordedDelay = delay;

            Debug.Log($"[{name}] 蔦の成長時間を記録: {recordedDelay:F2}秒");

        }

    }



    public void ScheduleAutoGrow(float startTime)

    {

        if (recordedDelay < 0f) return;



        if (autoGrowCoroutine != null)

            StopCoroutine(autoGrowCoroutine);



        autoGrowCoroutine = StartCoroutine(AutoGrowAfterDelay(startTime));

    }



    private IEnumerator AutoGrowAfterDelay(float startTime)

    {

        float elapsed = Time.time - startTime;

        float waitTime = recordedDelay - elapsed;

        if (waitTime > 0f)

            yield return new WaitForSeconds(waitTime);



        Debug.Log($"[{name}] 自動蔦成長開始！");

        TryStartNextVine();

    }



    public void ResetVines()

    {

        foreach (var spawner in vineSpawners)

        {

            spawner.DestroyVine();

        }



        currentIndex = 0;

        isGrowing = false;



        if (autoGrowCoroutine != null)

        {

            StopCoroutine(autoGrowCoroutine);

            autoGrowCoroutine = null;

        }

    }



    public void ClearRecord()

    {

        recordedDelay = -1f;

        if (autoGrowCoroutine != null)

        {

            StopCoroutine(autoGrowCoroutine);

            autoGrowCoroutine = null;

        }

    }

}