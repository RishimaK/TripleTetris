using DG.Tweening;
using UnityEngine;

public class Sale : MonoBehaviour
{
    public void OpenDialog ()
    {
        Transform board = gameObject.transform.GetChild(1);
        // audioManager.PlaySFX("click");
        gameObject.SetActive(true);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();

        board.DOScale(new Vector3(1f,1f,1f), 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
            // enabledTouch = true;

        });
    }

    public void CloseDialog()
    {
        Transform board = gameObject.transform.GetChild(1);
        // audioManager.PlaySFX("click");
        board.DOPause();
        // enabledTouch = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
            // gameManager.ContinueGame();
        });
    }
}
