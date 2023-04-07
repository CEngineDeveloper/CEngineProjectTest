using CYM.Pathfinding;
namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        public IAStarMoveMgr AStarMoveMgr => MoveMgr as IAStarMoveMgr;
    }
}