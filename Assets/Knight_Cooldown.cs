using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knight_Cooldown : MonoBehaviour
{
    public Button skillButton; // 技能按钮
    public Image cooldownFill; // 冷却填充图像
    public float cooldownDuration = 5.0f; // 冷却时间
    private bool isCooldownActive = false; // 是否处于冷却状态
    private float fillAmountPerSecond;

    void Start()
    {
       // skillButton.onClick.AddListener(ActivateSkill);
        fillAmountPerSecond = 1f / cooldownDuration; // 每秒填充量
    }

    public void ActivateSkill()
    {
        
        if (!isCooldownActive)
        {
            isCooldownActive = true;
            skillButton.interactable = false; // 禁用按钮
            StartCoroutine(CooldownRoutine());
        }
    }

    IEnumerator CooldownRoutine()
    {
        float currentTime = 0;

        while (currentTime < cooldownDuration)
        {
            currentTime += Time.deltaTime;
            cooldownFill.fillAmount = 1 - (currentTime / cooldownDuration); // 更新填充量
            yield return null;
        }

        isCooldownActive = false;
        skillButton.interactable = true; // 技能冷却结束，启用按钮
        cooldownFill.fillAmount = 1; // 重置填充量
    }
}
