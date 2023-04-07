using CYM.UI;
namespace CYM.HUD
{
    public interface IHUDMgr
    {
        THUD SpawnDurableHUD<THUD>(string prefabName, BaseUnit target = null) where THUD : UHUDBar;
        UHUDText JumpChatBubbleStr(string str);
        UHUDText JumpChatBubble(string key);
    }
}