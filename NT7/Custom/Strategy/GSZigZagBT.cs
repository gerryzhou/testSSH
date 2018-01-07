#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
	/// Two questions: 
	/// 1) the volality of the current market
	/// 2) counter swing/trend or follow swing/trend? (Trending, Reversal or Range)
    /// </summary>
    [Description("GSZigZag BackTesting")]
    public class GSZigZagBT : GSZigZagBase
    {
        #region Variables
        // Wizard generated variables

		/// <summary>
		/// Order handling
		/// </summary>
		//private IOrder entryOrder = null;

		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			base.Initialize();
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			base.OnBarUpdate();
			/*
			if(!Historical) Print(CurrentBar + "- GSZZ1 OnBarUpdate - " + Time[0].ToShortTimeString());
			if(CurrentBar < BarsRequired+2) return;
			int bsx = BarsSinceExit();
			int bse = BarsSinceEntry();
			
			double gap = GIZigZag(DeviationType.Points, retracePnts, false, false, false, true).ZigZagGap[0];
			//CheckPerformance();
			//ChangeSLPT();
			//CheckEnOrder();
			if(PrintOut > 0)
				Print("GI gap=" + gap + "," + Position.MarketPosition.ToString() + "=" + Position.Quantity.ToString()+ ", price=" + Position.AvgPrice + ", BarsSinceEx=" + bsx + ", BarsSinceEn=" + bse);
			
			DrawGapText(gap, "gap-");
			double gapAbs = Math.Abs(gap);
			if(NewOrderAllowed()) 
			{
//			if(!Historical && Position.Quantity == 0 && (bsx == -1 || bsx > barsSincePtSl)) {
//-1, 0, 1 vs -1, 0, 1
				if(tradeStyle == 0) // scalping, counter trade the pullbackMinPnts
				{
					if(tradeDirection >= 0) //1=long only, 0 is for both;
					{
						if(gap < 0 && gapAbs >= enPullbackMinPnts && gapAbs < enPullbackMaxPnts)
							NewLongLimitOrder("scalping long");
					}
					else if(tradeDirection <= 0) //-1=short only, 0 is for both;
					{
						if(gap > 0 && gapAbs >= enPullbackMinPnts && gapAbs < enPullbackMaxPnts)
							NewShortLimitOrder("scalping short");
					}
				}
				else if(tradeStyle < 0) //counter trend trade, , counter trade the swingMinPnts
				{
					if(tradeDirection >= 0) //1=long only, 0 is for both;
					{
						if(gap < 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts)
							NewLongLimitOrder("counter trade long");
					}
					else if(tradeDirection <= 0) //-1=short only, 0 is for both;
					{
						if(gap > 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts)
							NewShortLimitOrder("counter trade short");
					}
				}
				else // tradeStyle > 0, trend following
				{
					if(tradeDirection >= 0) //1=long only, 0 is for both;
					{
						if((gap > 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts) || (gap < 0 && gapAbs >= enPullbackMinPnts && gapAbs < enPullbackMaxPnts))
							NewLongLimitOrder("trend follow long");
					}
					else if(tradeDirection <= 0) //-1=short only, 0 is for both;
					{
						if((gap < 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts) || (gap > 0 && gapAbs >= enPullbackMinPnts && gapAbs < enPullbackMaxPnts))
							NewShortLimitOrder("trend follow short");
					}
				}
			} */
        }
		
        #region Properties
        
		
        #endregion
    }
}
