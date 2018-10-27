using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotController : MonoBehaviour
{
    SkillController _representedSkill = null;

    public Image IconImage;
    public Text NameText;
    public Image CooldownOverlay;
    public Text ButtonText;

    public void SetSkill(SkillController skillController)
    {
        if (skillController == null)
            return;

        //handle
        Skill skill = skillController.Skill;

        _representedSkill = skillController;
        IconImage.sprite = skill.Icon;
        NameText.text = skill.ExternalName;
        CooldownOverlay.fillAmount = 0;
    }
    public void EraseSkill()
    {
        _representedSkill = null;

        IconImage.sprite = null;
        NameText.text = "";
        CooldownOverlay.fillAmount = 0;
    }
    public void SetButton(string buttonName)
    {
        ButtonText.text = buttonName;
    }
    private void Update()
    {
        if (_representedSkill != null)
            GetCooldownProgress();
    }
    public void GetCooldownProgress()
    {
        if (_representedSkill.IsOnCooldown)
            CooldownOverlay.fillAmount = 1 - _representedSkill.Cooldown.ProgressNormalized;        //opposite value for visuals only
        else
            CooldownOverlay.fillAmount = 0;
        //can use CooldownDuration.ProgressExact if we want to show a seconds timer.
    }
}
