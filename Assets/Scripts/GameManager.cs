using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform finishLine;
    [SerializeField] private CameraFollow cameraFollow;
    private Stickman stickman;
    [SerializeField] private float speedOnWin = 3f;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject particleEffect;

    private Vector3 initPos;
    private bool won;

    private void Start()
    {
        stickman = player.GetComponent<Stickman>();
        initPos = player.transform.position;
    }

    private void Update()
    {
        if (!won)
        {
            if (!stickman.getSticked())
            {
                if (player.transform.position.x < -5 || player.transform.position.y < -6)
                {
                    ResetGame();
                }
            }

            if (player.transform.position.x > finishLine.position.x)
            {
                won = true;
                Win();
            }
        }
    }

    private void ResetGame()
    {
        stickman.reset(initPos);
    }

    private void Win()
    {
        stickman.Win(speedOnWin);

        if (particleEffect != null)
        {
            particleEffect.SetActive(true);
            particleEffect.transform.parent = null;
        }

        if (cameraFollow != null)
            cameraFollow.Win();

        StartCoroutine(FinishLevel());
    }

    IEnumerator FinishLevel()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
