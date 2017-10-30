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
    public class GSZigZag : Strategy
    {
        #region Variables
        // Wizard generated variables
        private double retracePnts = 4; // Default setting for RetracePnts
        private double enSwingMinPnts = 6; // Default setting for EnSwingMinPnts
        private double enSwingMaxPnts = 10; // Default setting for EnSwingMaxPnts
        private int profitTargetAmt = 450; // Default setting for ProfitTargetAmt
        private int stopLossAmt = 200; // Default setting for StopLossAmt
        private int barsSincePT = 1; // Default setting for BarsSincePT
        private int barsSinceSL = 1; // Default setting for BarsSinceSL
        private int timeStart = 933; // Default setting for TimeStart
		private int timeEnd = 1245; // Default setting for TimeStart
        // User defined variables (add any user defined variables below)
		private IDataSeries zzHighValue;
		private IDataSeries zzLowValue;
		private DataSeries		zigZagSizeSeries;
		private DataSeries		zigZagSizeZigZag;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            //Add(ZigZag(High, NinjaTrader.Data.DeviationType.Points, 4, true));
            Add(GIZigZag(NinjaTrader.Data.DeviationType.Points, 4, true));
			//zzHighValue = new DataSeries(this, MaximumBarsLookBack.Infinite);
			//zzLowValue = new DataSeries(this, MaximumBarsLookBack.Infinite);
			
			//zzHighValue = GIZigZag(DeviationType.Points, 4, true).ZigZagHigh;
			//zzLowValue = GIZigZag(DeviationType.Points, 4, true).ZigZagLow;
			
            //SetProfitTarget("LN933", CalculationMode.Price, Variable0);
            //SetStopLoss("LN933", CalculationMode.Ticks, StopLossAmt, false);
            CalculateOnBarClose = true;
			zigZagSizeSeries	= new DataSeries(this, MaximumBarsLookBack.Infinite);
			zigZagSizeZigZag = new DataSeries(this, MaximumBarsLookBack.Infinite);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			IDataSeries highSeries	= High;
			IDataSeries lowSeries	= Low;
			//double	highValue	= High[Math.Max(0, GIZigZag(DeviationType.Points, 4, true).HighBar(0, 1, 100))];
			//double	lowValue	= Low[Math.Max(0, GIZigZag(DeviationType.Points, 4, true).LowBar(0, 1, 100))];
			//zzHighValue.Set(ZigZag(DeviationType.Points, 4, true).ZigZagHigh[0]);
			//zzLowValue.Set(ZigZag(DeviationType.Points, 4, true).ZigZagLow[0]);
			//Print("Time=" + Time[0].ToString() + ", High=" + highValue + ", Low=" + lowValue);
			Print("Time=" + Time[0].ToString() + ", High=" + High + ", Low=" + Low);
			if (CurrentBar > BarsRequired)
			{
				double zzSize = GIZigZag(DeviationType.Points, 4, true).GetZigZagSize(CurrentBar,CurrentBar-BarsRequired, 0, true);
				//zigZagSizeSeries.Set(zzSize);
				//Print(CurrentBar+ " LastBarOnChart=" + GIZigZag(DeviationType.Points, 4, true).IsLastBarOnChart());
				//Print(CurrentBar+ " zzSize=" + zigZagSizeSeries.Get(CurrentBar).ToString());
				/*
				double zzPoint = GIZigZag(DeviationType.Points, 4, true).GetZigZagPoint(CurrentBar);
				Print(CurrentBar+ " zzPoint=" + zzPoint.ToString());// + "-" + GIZigZag(DeviationType.Points, 4, true).PrintZZHighLow(CurrentBar, 10));
				if (zzPoint != 0) {
					DrawText("tag-"+Time.ToString(), ToTime(Time[1]).ToString()+"-"+(CurrentBar-1).ToString()+"\r\n"				
					+zzPoint.ToString(),
					1, double.Parse(High[1].ToString())+0.5, Color.Cyan);
				}
				*/
			}
			
			/*
			if(zzHighValue[0]==zzHighValue[1] && zzLowValue[0]==zzLowValue[1]) {			
            	DrawText("tag-"+Time.ToString(), ToTime(Time[1]).ToString()+"-"+CurrentBar.ToString()+"\r\n"				
				+zzLowValue[0].ToString()+":"+zzHighValue[0].ToString(),
				1, double.Parse(High.ToString())+0.5, Color.Green);
			} else {
				DrawText("tag-"+Time.ToString(), ToTime(Time[1]).ToString()+"-"+(CurrentBar-1).ToString()+"\r\n"
				+(zzHighValue[1]-zzLowValue[1]).ToString()+"\r\n"
				+zzLowValue[1].ToString()+":"+zzHighValue[1].ToString()+"\r\n"
				+zzLowValue[0].ToString()+":"+zzHighValue[0].ToString(),
				1, double.Parse(High.ToString())+0.5, Color.Blue);
			} */
			// Condition set 1
            if (ToTime(Time[0]) >= ToTime(9, 33, 0)
                && ZigZag(High, DeviationType.Points, 4, true).ZigZagHigh[0] < BarsSincePT)
            {
                //EnterLongLimit(DefaultQuantity, Low[0] + 4 * TickSize, "LN933");
                //DrawText("My text" + CurrentBar, "High", 0, 0, Color.Red);
            }

            // Condition set 2
            if (ZigZag(DeviationType.Points, 4, true).ZigZagHigh[0] >= EnSwingMaxPnts)
            {
                //EnterShortLimit(DefaultQuantity, High[0] + 4 * TickSize, "ST933");
            }
        }

        #region Properties
        [Description("Retracement Points.")]
        [GridCategory("Parameters")]
        public double RetracePnts
        {
            get { return retracePnts; }
            set { retracePnts = Math.Max(4, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double EnSwingMinPnts
        {
            get { return enSwingMinPnts; }
            set { enSwingMinPnts = Math.Max(6, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double EnSwingMaxPnts
        {
            get { return enSwingMaxPnts; }
            set { enSwingMaxPnts = Math.Max(10, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTargetAmt
        {
            get { return profitTargetAmt; }
            set { profitTargetAmt = Math.Max(200, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLossAmt
        {
            get { return stopLossAmt; }
            set { stopLossAmt = Math.Max(50, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int BarsSincePT
        {
            get { return barsSincePT; }
            set { barsSincePT = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int BarsSinceSL
        {
            get { return barsSinceSL; }
            set { barsSinceSL = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TimeStart
        {
            get { return timeStart; }
            set { timeStart = Math.Max(933, value); }
        }
        #endregion
    }
}
