//------------------------------------------------------------------------------
// Enum.cs
// Created by CYM on 2021/6/6
// 填写类的描述...
//------------------------------------------------------------------------------
namespace Gamelogic
{
    public enum CrewAttr
    {
        //动态属性
        Coin,           //金币
        Pltpt,          //政令
        Prestige,       //威望
        Manpower,       //人力
        Notoriety,      //恶名
        Legitimacy,     //合法性
        Techpt,         //创新点
        Corrupt,        //腐败,影响军队战斗力,科研效率,行政效率,人力回复,驭人成本
        Inflation,      //通货膨胀,影响建筑,建造核心,建造军队的花费
        Mercantilism,   //重商主义,影响利率,最大贷款,贷款期限

        //战略资源
        Food,       //粮食
        Steel,      //金属
        Horse,      //马匹

        //核心属性
        PltptChaAdd,        //政令增长
        PrestigeChaAdd,     //威望增长
        LegitimacyChaAdd,   //合法性增长
        TechptChaAdd,       //创新点增长
        CorruptChaAdd,      //腐败度下降
        InflationChaAdd,    //通货膨胀下降
        ScieneffAdd,        //科研效率
        AdmeffAdd,          //行政效率,降低:降低通胀花费,滥发铸币花费,减少腐败花费,战争税花费
        AgritaxAdd,         //农业税
        IndutaxAdd,         //工业税
        CoreCostAdd,        //建立统治花费
        BuildCostAdd,       //建筑花费
        InterestAdd,        //借款利率
        LoanTimeAdd,        //借款期限
        LoanCapAdd,         //借款额度
        SalaryAdt,          //俸禄修正
        LegionMaintainAdt,  //军团维护费
        CastleMaintainAdt,  //城防维护费
        TributeAdt,         //贡金收入
        IndemnityAdt,       //赔款收入
        NotorietyChaAdd,    //恶名下降
        NotorietyMaxAdd,    //最大恶名
        ManpowerChaAdd,     //人力回复(100)
        ManpowerChaAdt,     //人力回复(10%)
        ManpowerMaxAdd,     //最大人力(1000)
        ManpowerMaxAdt,     //最大人力(10%)
        CtrlManCostAdt,     //驭人成本
        CtrlManMaxAdd,      //驭人上限
        GetSonPropAdd,      //生育几率
        WarPrepareAdt,      //战争准备,减少战前准备的时间,国土越大准备时间越长
        WarCostAdt,         //战争消耗,减少战争消耗(政令),国土越大,消耗越多

        //外交属性
        AllianceMaxAdd,     //盟友数量
        VassalMaxAdd,       //附庸数量
        DipReputationAdd,   //外交信誉,增加谈判成功率
        EspionageAdd,       //谍报力量
    }
    public enum CharaAttr
    {
        AgriTax,//农业税
        InduTax,//工业税
    }
    public enum LegionAttr
    {
        //动态值
        Life,
        Exp,
        Moral,

        //Max
        LifeMaxAdt,
        ExpMaxAdt,
        MoralMaxAdt,

        //Other
        TimeAdt,
        MaintainAdt,
        MoralRiseAdt,

        //Adt
        ShockAdt,
        FireAdt,
        ImpactAdt,
        RidingAdt,
        ArmorbreakAdt,
        ToughnessAdt,
        ParryingAdt,
        ArmorAdt,
        MobilityAdt,
    }
    public enum ForceType
    {
        //保留
        卫戍兵,

        //步兵
        刀盾兵,
        枪盾兵,
        长枪兵,
        破阵兵,
        刀斧手,

        //远程
        弓箭手,
        强弩兵,
        火枪兵,

        //骑兵
        刀骑兵,
        枪骑兵,
        弓骑兵,

        //战车
        战象兵,
        战车兵,
    }

    public enum ArmorType
    {
        Light,  //轻甲
        Middle, //中假
        Heavy,  //重甲
    }
}