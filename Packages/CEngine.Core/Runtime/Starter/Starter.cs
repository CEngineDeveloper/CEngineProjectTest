using CYM.DLC;
using HybridCLR;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CYM
{
    //[RequireComponent(typeof(Console))]
    //[RequireComponent(typeof(FPSCounter))]
    //[RequireComponent(typeof(ErrorCatcher))]
    [ExecuteInEditMode]
    [HideMonoScript]
    public class Starter : MonoBehaviour
    {
        #region assembly
        public static Assembly AssemblyGameLogic { get; private set; }
        public static Assembly AssemblyCEngine { get; private set; }
        #endregion

        #region prop
        public static Starter Ins { get; private set; }
        public static UnityEngine.Object GlobalObj { get; private set; }
        static StarterUI StarterUI = null;
        #endregion

        #region Inspector
        [SerializeField]
        [ValueDropdown("GetGlobalTypeList")]
        [DisableInPlayMode]
        string GlobalType;
        #endregion

        #region life
        async void Awake()
        {
            Ins = this;
            transform.hideFlags = HideFlags.NotEditable;
            transform.position = SysConst.VEC_GlobalPos;
            if (Application.isPlaying)
            {
                StarterUI = Util.CreateGlobalResourceObj<StarterUI>("BaseStarterPlayer");
                DontDestroyOnLoad(this);
                SetupComponet<DLCDownloader>();
                SetupComponet<DLCManager>();
                SetupComponet<Console>();
                //下载资源
                await DLCDownloader.StartDownload();
                //加载配置文件
                ScriptConfigHub.Load();
                //加载热更代码
                LoadHotFixScript();
                //初始化Console
                Console.Initialize();
            }
        }
        #endregion

        #region set
        void SetupComponet<T>() where T : MonoBehaviour
        {
            if (gameObject.GetComponent<T>() == null)
                gameObject.AddComponent<T>();
        }
        UnityEngine.Object AddScript(string name)
        {
            if (name.IsInv())
            {
                CLog.Error("错误,AddScript,name不合法");
                return null;
            }
            string fullName = "none";
            UnityEngine.Object ret = null;
            EnsureAssembly();
            //加载逻辑层环境
            fullName = BuildConfig.NameSpace + "." + name;
            Type type = AssemblyGameLogic.GetType(fullName);
            //加载教程环境
            if (type == null)
            {
                fullName = nameof(CYM)+"." + name;
                type = AssemblyCEngine.GetType(fullName);
            }
            //添加组件
            if (type == null)
            {
                Debug.LogError($"重大错误！！{name} 这类型没有定义");
                return null;
            }
            ret = gameObject.GetComponent(type);
            if (ret == null)
                ret = gameObject.AddComponent(type);
            return ret;
        }
        void LoadHotFixScript()
        {
            //真机环境
            if (!Application.isEditor &&
                BuildConfig.Ins.IsHotFix)
            {
                ///加载热更代码
                AssetBundle hotUpdateDllAb = DLCDownloader.GetAssetBundle("hotupdatedlls");
                TextAsset dllBytes = hotUpdateDllAb.LoadAsset<TextAsset>("Assembly-CSharp.dll.bytes");
                AssemblyGameLogic = Assembly.Load(dllBytes.bytes);
                /// 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
                /// 我们在BuildProcessors里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。
                /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
                /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误 
                AssetBundle dllAB = DLCDownloader.GetAssetBundle("aotdlls");
                foreach (var aotDllName in dllAB.GetAllAssetNames())
                {
                    byte[] aotdllBytes = dllAB.LoadAsset<TextAsset>(aotDllName).bytes;
                    /// 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(aotdllBytes);
                    Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
                }
            }
            EnsureAssembly();
            GlobalObj = AddScript(GlobalType);
            Debug.Log("热更初始化完成");
        }
        void EnsureAssembly()
        {
            if (AssemblyGameLogic == null)
            {
                AssemblyGameLogic = Assembly.Load(SysConst.STR_AssemblyGameLogic);
            }
            if (AssemblyCEngine == null)
            {
                AssemblyCEngine = Assembly.Load(SysConst.STR_AssemblyCEngine);
            }
        }
        #endregion

        #region get
        string[] GetGlobalTypeList()
        {
            EnsureAssembly();
            var ret1 = AssemblyGameLogic.GetTypes()
                .Where(x=>!x.IsGenericTypeDefinition)
                .Where(x=>x.IsSubclassOf(typeof(BaseGlobal)));
            var ret2 = AssemblyCEngine.GetTypes()
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => x.IsSubclassOf(typeof(BaseGlobal)));
            ret1 = ret1.Concat(ret2);
            return ret1.Select(x=>x.Name).ToArray();
        }
        #endregion
    }
}