using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image _progressBar;
    private void Start()
    {
        _progressBar.fillAmount = 0;
    }
    void Update()
    {
       _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, LevelManager.Instance.Target, 2 * Time.deltaTime);
    }
}
