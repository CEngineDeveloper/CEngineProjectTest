//------------------------------------------------------------------------------
// PlanetMgr.cs
// Created by CYM on 2022/7/26
// 填写类的描述...
//------------------------------------------------------------------------------
using CYM;
namespace Gamelogic
{
    public class PlanetMgr : BaseEntitySpawnMgr<UnitPlanet,TDPlanetData,DBPlanet, UnitPlanet>
    {
        public void TestMgr()
        {
            CLog.Cyan("PlanetMgr:Tested");
        }
    }
}