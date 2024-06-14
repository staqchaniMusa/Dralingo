using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameContextButton : UIButton
{
    public override void OnClick()
    {
        if(!string.IsNullOrEmpty(method))
        AppContext.instance.game.Invoke(method, 0);
    }
}
