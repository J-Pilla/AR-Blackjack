using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _instructionsPanel;

    public void PlayGame() => SceneManager.LoadScene("SampleScene");
    
    public void ShowInstructions()
    {
        _mainMenuPanel.SetActive(false);
        _instructionsPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        _instructionsPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
    }

    public void QuitGame() => Application.Quit();
}