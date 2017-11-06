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
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class GSZigZag1 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private double retracePnts = 4; // Default setting for RetracePnts
        private int profitTargetAmt = 36; // Default setting for ProfitTargetAmt
        private int stopLossAmt = 16; // Default setting for StopLossAmt
		private double EnOffsetPnts = 1;//the price offset for entry
        private int timeStart = 93300; // Default setting for TimeStart
        private int timeEnd = 124500; // Default setting for TimeEnd
        private int barsSincePtSl = 1; // Default setting for BarsSincePtSl
        private double enSwingMinPnts = 6; // Default setting for EnSwingMinPnts
        private double enSwingMaxPnts = 10; // Default setting for EnSwingMaxPnts
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(GIZigZag(NinjaTrader.Data.DeviationType.Points, 4, true));
            SetProfitTarget("EnST1", CalculationMode.Ticks, ProfitTargetAmt);
            SetStopLoss("EnST1", CalculationMode.Ticks, StopLossAmt, false);
			SetProfitTarget("EnLN1", CalculationMode.Ticks, ProfitTargetAmt);
            SetStopLoss("EnLN1", CalculationMode.Ticks, StopLossAmt, false);

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			int bsx = BarsSinceExit();
			double gap = GIZigZag(DeviationType.Points, 4, true).ZigZagGap[0];
			double gapAbs = Math.Abs(gap);
			Print("gap=" + gap + "," + Position.MarketPosition.ToString() + "=" + Position.Quantity.ToString()+ ", price=" + Position.AvgPrice + ", BarsSinceExit=" + bsx);
			//if(ToTime(Time[0]) >= TimeStart && ToTime(Time[0]) <= TimeEnd && Position.Quantity == 0 && (bsx == -1 || bsx > barsSincePtSl)) {

				if ( gap < 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts)
				{
					Print(CurrentBar + ", EnterLongLimit called");
					EnterLongLimit(DefaultQuantity, Low[0]-EnOffsetPnts, "EnLN1");
				} 
				else if ( gap > 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts)
				{
					Print(CurrentBar + ", EnterShortLimit called");
					EnterShortLimit(DefaultQuantity, High[0]+EnOffsetPnts, "EnST1");
				}
			//}
        }
		
protected override void OnExecution(IExecution execution)
{
    // Remember to check the underlying IOrder object for null before trying to access its properties
    if (execution.Order != null && execution.Order.OrderState == OrderState.Filled) {
		Print(execution.Name + ",Price=" + execution.Price + "," + execution.Time.ToShortTimeString());
		IText it = DrawText(CurrentBar.ToString()+Time[0].ToShortTimeString(), Time[0].ToString().Substring(10)+"\r\n"+execution.Name+":"+execution.Price, 0, execution.Price, Color.Red);
		it.Locked = false;
	}
}

protected override void OnOrderUpdate(IOrder order)
{
//    if (entryOrder != null && entryOrder == order)
//    {
        //Print(order.ToString() + "--" + order.OrderState);
//        if (order.OrderState == OrderState.Cancelled)
//         {
//              // Do something here
//              entryOrder = null;
//         }
//    }
}

protected override void OnPositionUpdate(IPosition position)
{
	//Print(position.ToString() + "--MarketPosition=" + position.MarketPosition);
    if (position.MarketPosition == MarketPosition.Flat)
    {
         // Do something like reset some variables here
    }
}

        #region Properties
        [Description("ZigZag retrace points")]
        [GridCategory("Parameters")]
        public double RetracePnts
        {
            get { return retracePnts; }
            set { retracePnts = Math.Max(4, value); }
        }

        [Description("Tick amount of profit target")]
        [GridCategory("Parameters")]
        public int ProfitTargetAmt
        {
            get { return profitTargetAmt; }
            set { profitTargetAmt = Math.Max(8, value); }
        }

        [Description("Tick amount of stop loss")]
        [GridCategory("Parameters")]
        public int StopLossAmt
        {
            get { return stopLossAmt; }
            set { stopLossAmt = Math.Max(8, value); }
        }

        [Description("Time start")]
        [GridCategory("Parameters")]
        public int TimeStart
        {
            get { return timeStart; }
            set { timeStart = Math.Max(0, value); }
        }

        [Description("Time end")]
        [GridCategory("Parameters")]
        public int TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = Math.Max(0, value); }
        }

        [Description("Bar count since last filled PT or SL")]
        [GridCategory("Parameters")]
        public int BarsSincePtSl
        {
            get { return barsSincePtSl; }
            set { barsSincePtSl = Math.Max(1, value); }
        }

        [Description("Min swing size for entry")]
        [GridCategory("Parameters")]
        public double EnSwingMinPnts
        {
            get { return enSwingMinPnts; }
            set { enSwingMinPnts = Math.Max(4, value); }
        }

        [Description("Max swing size for entry")]
        [GridCategory("Parameters")]
        public double EnSwingMaxPnts
        {
            get { return enSwingMaxPnts; }
            set { enSwingMaxPnts = Math.Max(4, value); }
        }
        #endregion
    }
}
