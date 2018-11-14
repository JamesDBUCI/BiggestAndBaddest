using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillSlot : MonoBehaviour
{
    Skill.Controller _representedController = null;

    public Image IconImage;
    public Text NameText;
    public Image CooldownOverlay;
    public Text ButtonText;

    public void SetSkill(Skill.Controller skillController)
    {
        if (skillController == null || skillController.IsEmpty)
            return;

        _representedController = skillController;

        //handle
        Skill.Template template = skillController.Contents.Template;

        IconImage.sprite = template.Icon;
        NameText.text = template.NameExternal;
        CooldownOverlay.fillAmount = 0;
    }
    public void EraseSkill()
    {
        _representedController = null;

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
        if (_representedController != null)
            GetCooldownProgress();
    }
    public void GetCooldownProgress()
    {
        if (_representedController.Contents.IsOnCooldown)
            CooldownOverlay.fillAmount = 1 - _representedController.Contents.CooldownProgressNormalized;        //opposite value for visuals only
        else
            CooldownOverlay.fillAmount = 0;
        
        //can use CooldownRemainingTime if we want to show a seconds timer.
    }
}
