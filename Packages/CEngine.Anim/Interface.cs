using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CYM.Anim
{
    public interface IMecAnimMgr
    {
        void ChangeState(int state, int index = 0);
    }
}
