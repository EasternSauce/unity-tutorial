using UnityEngine;
using UnityEngine.UI;

public class UIPoolBar : MonoBehaviour
{
    [SerializeField] Image bar;
    [SerializeField] TMPro.TextMeshProUGUI textValue;

    ValuePool targetPool;

    public void Show(ValuePool targetPool)
    {
        if (targetPool != null)
        {
            this.targetPool = targetPool;
            gameObject.SetActive(true);
        }
    }

    public void Clear()
    {
        this.targetPool = null;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (targetPool == null) { return; }
        bar.fillAmount = Mathf.InverseLerp(0f, targetPool.maxValue.integer_value, targetPool.currentValue);
        if (textValue != null)
        {
            textValue.text = targetPool.currentValue.ToString() + "/" + targetPool.maxValue.integer_value.ToString();
        }
    }
}
