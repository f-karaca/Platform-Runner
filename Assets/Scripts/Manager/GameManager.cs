using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    #region Inspector Variables

    [Header("Reference")]
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject painterManager;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform paintPoint;
    [SerializeField] private ParticleSystem finishParticle;
    [Header("Settings")]
    [SerializeField] private float cameraTime = 2f;
    public float paintEnergy = 100f;

    [Space(10f)]
    [SerializeField] private List<GameObject> levels;
    [Header("UI")]
    [SerializeField] public UIManager uIManager;
    [Header("AI")]
    [SerializeField] private List<Transform> characters;

    #endregion

    [HideInInspector] public bool isDestination = false;
    [HideInInspector] public bool isclick = false;

    #region Private Variables

    private GameObject player;
    private PlayerController playerController;

    private int currentLevel = 1;

    #endregion

    public Transform StartPoint => startPoint;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        LevelProperties(currentLevel);
    }

    //********UPDATE********
    private void Update()
    {
        characters.Sort(CharacterSorting);
        int characterNumber = characters.Count - characters.IndexOf(player.transform);
        uIManager.sortingText.text = characterNumber.ToString();
    }

    #region Character Management

    //Player restart function
    public void RestartCharacter()
    {
        playerController.SetMovement(false); //Player movement is stopped
        player.transform.position = startPoint.position; //Player position is replaced by the starting position
        cameraObject.GetComponent<CameraFollow>().enabled = true; //The camera follows the player
        playerController.isPlayerActive = true; //The player is made movable
        SetUI(UIManager.UIState.Begin); //Startup ui is set
    }

    public void PlayerStop()
    {
        //Player movement is stopped
        playerController.isPlayerActive = false;
        playerController.SetMovement(false);
    }

    public void CharacterFinishAnim()
    {
        playerController.AnimManager.SetAnimationState(AnimManager.AnimationStates.Win);
    }
    #endregion

    #region Level Management

    public void OnFinish()
    {
        playerController.isPlayerActive = false;
        playerController.SetMovement(false);
        playerController.AnimManager.SetAnimationState(AnimManager.AnimationStates.Win);
        StartCoroutine(CameraTimer(paintPoint.position, cameraTime));
    }

    //The function to be called when the level is completely finished
    public void FinishLevel()
    {
        if (currentLevel < 2)
            currentLevel++;
        else
        {
            currentLevel = 1;
            paintEnergy = 100;
        }

        LevelProperties(currentLevel);

        int count = 0;
        //level pass is provided
        foreach (GameObject item in levels)
        {
            if (count == currentLevel - 1)
            {
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
            count++;
        }

        RestartCharacter();
    }

    public void FinishParticle()
    {
        finishParticle.Play();
    }

    private void LevelProperties(int levelIndex)
    {
        switch (levelIndex)
        {
            case 1:
                uIManager.paintingBar.transform.parent.gameObject.SetActive(true);
                uIManager.sortingPanel.SetActive(false);
                break;

            case 2:
                uIManager.paintingBar.transform.parent.gameObject.SetActive(false);
                uIManager.sortingPanel.SetActive(true);
                isDestination = false;

                break;

            default:
                break;
        }
    }

    public void FinishUI()
    {
        SetUI(UIManager.UIState.Win);
    }

    #endregion

    #region Camera Management

    private void CameraTween(Vector3 endValue, float time)
    {
        cameraObject.transform.DOMove(endValue, time).SetEase(Ease.Linear);
        cameraObject.GetComponent<CameraFollow>().enabled = false;
    }

    IEnumerator CameraTimer(Vector3 endValue, float time)
    {
        yield return new WaitForSeconds(time);
        CameraTween(endValue, time);
        yield return new WaitForSeconds(time);
        painterManager.SetActive(true);
    }

    #endregion

    public void SetUI(UIManager.UIState state)
    {
        uIManager.SetUIPanels(state);
    }

    private int CharacterSorting(Transform p1, Transform p2)
    {
        return p1.position.z.CompareTo(p2.position.z);
    }






}
