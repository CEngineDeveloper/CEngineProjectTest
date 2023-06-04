#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CEngine.AdvancedFPSCounter.CountersData
{
	public abstract class StaticCounterData : BaseCounterData
	{
		internal override void Activate()
		{
			base.Activate();
			
			if (inited)
				UpdateValue();
		}
	}
}