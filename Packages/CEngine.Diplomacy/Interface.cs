using System.Collections;
namespace CYM.Diplomacy
{
    public interface IAlertMgr<out TData> where TData : TDBaseAlertData
    {
        #region Callback
        event Callback<TData> Callback_OnAdded;
        event Callback<TData> Callback_OnRemoved;
        event Callback<TData> Callback_OnMerge;
        event Callback<TData> Callback_OnCommingTimeOut;
        event Callback<TData> Callback_OnInteractionChange;
        event Callback<TData> Callback_DisposableChange;
        event Callback<TData> Callback_ContinueChange;
        #endregion

        #region pub
        IList RawData { get; }
        #endregion

        #region set
        void Remove(long id);
        #endregion
    }
    public interface IEventMgr<out TDataOut>
    {
        #region callback
        event Callback<TDataOut> Callback_OnEventAdded;
        event Callback<TDataOut> Callback_OnEventRemoved;
        event Callback<TDataOut> Callback_OnEventChange;
        #endregion

        #region set
        TDataOut Add(string eventDlgName);
        void SelOption(TDBaseEventData eventData, EventOption option);
        void SelOption(TDBaseEventData eventData, int index);
        #endregion

        #region is
        bool IsHave();
        #endregion

        #region get
        TDataOut Get(int id);
        TDataOut First();
        TDataOut Rand();
        int Count();
        #endregion
    }
}