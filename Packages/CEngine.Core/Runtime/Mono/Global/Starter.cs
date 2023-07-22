using CYM.DLC;
using CYM.UI;
using HybridCLR;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CYM
{
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
                DontDestroyOnLoad(this);
                //��ʾLogo
                await LogoUI.Show();
                //��ӱ�Ҫ���
                SetupComponet<DLCDownloader>();
                SetupComponet<DLCManager>();
                SetupComponet<Console>();
                //��ʾ��������
                await StarterUI.Show();
                //������Դ
                await DLCDownloader.StartDownload();
                //���������ļ�
                ScriptConfigHub.Load();
                //�����ȸ�����
                LoadHotFixScript();
                //����ȫ�ֶ���
                AddScript(GlobalType);
                //��ʼ������Console
                Console.Initialize();
                //����Loader
                await BaseGlobal.LoaderMgr.StartAllLoader();
                //�ر���������
                await StarterUI.Close();
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
                CLog.Error("����,AddScript,name���Ϸ�");
                return null;
            }
            string fullName = "none";
            UnityEngine.Object ret = null;
            EnsureAssembly();
            //�����߼��㻷��
            fullName = BuildConfig.NameSpace + "." + name;
            Type type = AssemblyGameLogic.GetType(fullName);
            //���ؽ̳̻���
            if (type == null)
            {
                fullName = nameof(CYM)+"." + name;
                type = AssemblyCEngine.GetType(fullName);
            }
            //������
            if (type == null)
            {
                Debug.LogError($"�ش���󣡣�{name} ������û�ж���");
                return null;
            }
            ret = gameObject.GetComponent(type);
            if (ret == null)
                ret = gameObject.AddComponent(type);
            return ret;
        }
        void LoadHotFixScript()
        {
            //�������
            if (!Application.isEditor &&
                BuildConfig.Ins.IsHotFix)
            {
                ///�����ȸ�����
                AssetBundle hotUpdateDllAb = DLCDownloader.GetAssetBundle("hotupdatedlls");
                TextAsset dllBytes = hotUpdateDllAb.LoadAsset<TextAsset>("Assembly-CSharp.dll.bytes");
                AssemblyGameLogic = Assembly.Load(dllBytes.bytes);
                /// ���Լ�������aot assembly�Ķ�Ӧ��dll����Ҫ��dll������unity build���������ɵĲü����dllһ�£�������ֱ��ʹ��ԭʼdll��
                /// ������BuildProcessors������˴�����룬��Щ�ü����dll�ڴ��ʱ�Զ������Ƶ� {��ĿĿ¼}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} Ŀ¼��
                /// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
                /// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش��� 
                AssetBundle dllAB = DLCDownloader.GetAssetBundle("aotdlls");
                foreach (var aotDllName in dllAB.GetAllAssetNames())
                {
                    byte[] aotdllBytes = dllAB.LoadAsset<TextAsset>(aotDllName).bytes;
                    /// ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
                    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(aotdllBytes, HomologousImageMode.SuperSet);
                    Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
                }
            }
            EnsureAssembly();
            Debug.Log($"{nameof(VersionUtil.RealResBuildType)}:{VersionUtil.RealResBuildType}");
            Debug.Log("�ȸ���ʼ�����");
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