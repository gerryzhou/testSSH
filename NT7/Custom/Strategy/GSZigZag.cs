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
    [Description("Enter the description of your strategy here")]
    public class GSZigZag : GSZigZagBase
    {
        #region Variables
        // Wizard generated variables
		/*
        private double retracePnts = 4; // Default setting for RetracePnts
//        private int profitTargetAmt = 36 //Default setting for ProfitTargetAmt
//        private int stopLossAmt = 16 //Default setting for StopLossAmt
		private double profitTargetAmt = 450; //36 Default setting for ProfitTargetAmt
        private double stopLossAmt = 200; //16 Default setting for StopLossAmt
		private double breakEvenAmt = 150; //150 the profits amount to trigger setting breakeven order
		private double dailyLossLmt = -400; //-400 the daily loss limit amount
		private double enOffsetPnts = 1;//the price offset for entry
        private int timeStart = 0; //93300 Default setting for TimeStart
        private int timeEnd = 235900; // Default setting for TimeEnd
		//private int minutesChkEnOrder = 60; //how long before checking an entry order filled or not
        private int barsSincePtSl = 1; // Default setting for BarsSincePtSl
		private int barsToCheckPL = 2; // Number of Bars to check P&L since the entry
        private double enSwingMinPnts = 10; //6 Default setting for EnSwingMinPnts
        private double enSwingMaxPnts = 15; //10 Default setting for EnSwingMaxPnts
		private double enPullbackMinPnts = 5; //6 Default setting for EnPullbackMinPnts
        private double enPullbackMaxPnts = 9; //10 Default setting for EnPullbackMaxPnts
		private int tradeDirection = 0; // -1=short; 0-both; 1=long;
		private int tradeStyle = 1; // -1=counter trend; 1=trend following;
		private bool backTest = false; //if it runs for backtesting;
		private bool printOut = false;
		private bool drawTxt = false; // User defined variables (add any user defined variables below)
		private IText it_gap = null; //the Text draw for gap on current bar

		private IDataSeries zzHighValue;
		private IDataSeries zzLowValue;
		private DataSeries		zigZagSizeSeries;
		private DataSeries		zigZagSizeZigZag;
*/		
		/// <summary>
		/// Order handling
		/// </summary>
/*
		private string zzEntrySignal = "ZZEntry";

		private int ZZ_Count_0_6 = 0;
		private int ZZ_Count_6_10 = 0;
		private int ZZ_Count_10_16 = 0;
		private int ZZ_Count_16_22 = 0;
		private int ZZ_Count_22_30 = 0;
		private int ZZ_Count_30_ = 0;
		private int ZZ_Count = 0;
*/
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			base.Initialize();
			/*
			Add(GIZigZag(NinjaTrader.Data.DeviationType.Points, 4, false, false, false, true));
//            SetProfitTarget("EnST1", CalculationMode.Ticks, ProfitTargetAmt);
//            SetStopLoss("EnST1", CalculationMode.Ticks, StopLossAmt, false);
//			SetProfitTarget("EnLN1", CalculationMode.Ticks, ProfitTargetAmt);
//            SetStopLoss("EnLN1", CalculationMode.Ticks, StopLossAmt, false);
			zigZagSizeSeries = new DataSeries(this, MaximumBarsLookBack.Infinite);
			zigZagSizeZigZag = new DataSeries(this, MaximumBarsLookBack.Infinite);
			
//			SetProfitTarget(CalculationMode.Ticks, ProfitTargetAmt);
//            SetStopLoss(CalculationMode.Ticks, StopLossAmt);
            SetStopLoss(StopLossAmt);
			SetProfitTarget(ProfitTargetAmt);
			DefaultQuantity = 1;
            CalculateOnBarClose = true;
			IncludeCommission = true;
			// Triggers the exit on close function 30 seconds prior to session end
			ExitOnClose = true;
			ExitOnCloseSeconds = 30;
			*/
        }
		

		/// <summary>
		/// Print zig zag size.
		/// </summary>
/*		public void PrintZZSize()
		{
			String str_Plus = " + + + ";
			String str_Minus = " - - - ";
			//Update();
			Print(CurrentBar + " PrintZZSize called from GS");			
			double zzSize = 0;
			double zzSizeAbs = -1;
			double zzS = 0;
			int lastZZIdx = BarsRequired;
			for (int idx = BarsRequired; idx <= Input.Count; idx++)
			{
				zzS = zigZagSizeSeries.Get(idx);
				zzSize = zigZagSizeZigZag.Get(idx);
				zzSizeAbs = Math.Abs(zzSize);
				String str_suffix = "";
				//Print(idx.ToString() + " - ZZSizeSeries=" + zzS);
				if(zzSize>0) str_suffix = str_Plus;
				else if(zzSize<0) str_suffix = str_Minus;
				
				if(zzSizeAbs > 0 && zzSizeAbs <6){
					ZZ_Count_0_6 ++;
				}
				else if(zzSizeAbs >= 6 && zzSizeAbs <10){
					ZZ_Count_6_10 ++;
				}
				else if(zzSizeAbs >= 10 && zzSizeAbs <16){
					ZZ_Count_10_16 ++;
					if(printOut)
						Print(idx.ToString() + " - ZZSize=" + zzSize + "  [" + Time[CurrentBar-lastZZIdx].ToString() + "--" + Time[CurrentBar-idx].ToString() + "] >=10" + str_suffix + GetTimeDiff(Time[CurrentBar-lastZZIdx], Time[CurrentBar-idx]));
				}
				else if(zzSizeAbs >= 16 && zzSizeAbs <22){
					ZZ_Count_16_22 ++;
					if(printOut)
						Print(idx.ToString() + " - ZZSize=" + zzSize + "  [" + Time[CurrentBar-lastZZIdx].ToString() + "--" + Time[CurrentBar-idx].ToString() + "] >=16" + str_suffix + GetTimeDiff(Time[CurrentBar-lastZZIdx], Time[CurrentBar-idx]));
				}
				else if(zzSizeAbs >= 22 && zzSizeAbs <30){
					ZZ_Count_22_30 ++;
					if(printOut)
						Print(idx.ToString() + " - ZZSize=" + zzSize + "  [" + Time[CurrentBar-lastZZIdx].ToString() + "--" + Time[CurrentBar-idx].ToString() + "] >=22" + str_suffix + GetTimeDiff(Time[CurrentBar-lastZZIdx], Time[CurrentBar-idx]));
				}
				else if(zzSizeAbs >= 30){
					ZZ_Count_30_ ++;
					if(printOut)
						Print(idx.ToString() + " - ZZSize=" + zzSize + "  [" + Time[CurrentBar-lastZZIdx].ToString() + "--" + Time[CurrentBar-idx].ToString() + "] >=30" + str_suffix + GetTimeDiff(Time[CurrentBar-lastZZIdx], Time[CurrentBar-idx]));
				}
				if(zzSize != 0) {
					DrawZZSizeText(idx, "txt-");
					if(zzSizeAbs < 10 && printOut)
						Print(idx.ToString() + " - zzSize=" + zzSize + "  [" + Time[CurrentBar-lastZZIdx].ToString() + "--" + Time[CurrentBar-idx].ToString() + "]" );
					lastZZIdx = idx;
				}
			}
			ZZ_Count = ZZ_Count_0_6 + ZZ_Count_6_10 + ZZ_Count_10_16 + ZZ_Count_16_22 + ZZ_Count_22_30 + ZZ_Count_30_ ;
			if(printOut)
				Print(CurrentBar + "\r\n ZZ_Count=" + ZZ_Count + "\r\n ZZ_Count_0_6=" + ZZ_Count_0_6 + "\r\n ZZ_Count_6_10=" + ZZ_Count_6_10 + "\r\n ZZ_Count_10_16=" + ZZ_Count_10_16 + "\r\n ZZ_Count_16_22=" + ZZ_Count_16_22 + "\r\n ZZ_Count_22_30=" + ZZ_Count_22_30 + "\r\n ZZ_Count_30_=" + ZZ_Count_30_);
		}

		/// <summary>
		/// Remove the drawing object for barNo
		/// </summary>
		/// <param name="barNo"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool DrawZZSizeText(int barNo, string tag)
		{
			int idx_hilo = -1; // the last ZZ hi or ZZ lo index;
			Color up_color = Color.Green;
			Color dn_color = Color.Red;
			Color sm_color = Color.Black;
			Color draw_color = sm_color;
			//Update();
			if(printOut)
				Print(barNo + " DrawZZSizeText called");
//			double zzhi = getLastZZHighLow(CurrentBar, 1, out idx_hi); //zigZagHighZigZags.Get(idx);
//			double zzlo = getLastZZHighLow(CurrentBar, -1, out idx_lo); // zigZagLowZigZags.Get(idx);
			
			double zzSize = zigZagSizeZigZag.Get(barNo);//the previous zz size
			double zzSizeAbs = Math.Abs(zzSize);
			IText it = null;
			if(zzSize < 0) {
				if(zzSizeAbs >= 10) draw_color = dn_color;
				it = DrawText(tag+barNo.ToString(), GetTimeDate(Time[CurrentBar-barNo], 1)+"\r\n#"+barNo.ToString()+"\r\n"+zzSize, CurrentBar-barNo, double.Parse(High[CurrentBar-barNo].ToString())+2.5, draw_color);
			}
			if(zzSize > 0) {
				if(zzSizeAbs >= 10) draw_color = up_color;
				it = DrawText(tag+barNo.ToString(), GetTimeDate(Time[CurrentBar-barNo], 1)+"\r\n#"+barNo.ToString()+"\r\n"+zzSize, CurrentBar-barNo, double.Parse(Low[CurrentBar-barNo].ToString())-2.5, draw_color);
			}
			it.Locked = false;
			
			for (int idx = barNo-1; idx >= BarsRequired; idx--)
			{
				zzSize = zigZagSizeZigZag.Get(idx);
				zzSizeAbs = Math.Abs(zzSize);
				draw_color = sm_color;
				if(zzSize < 0) {
					idx_hilo = idx;
					if(printOut)
						Print(idx + " DrawZZSize called");
					if(zzSizeAbs >= 10) draw_color = dn_color;
					it = DrawText(tag+idx.ToString(), GetTimeDate(Time[CurrentBar-idx], 1)+"\r\n#"+idx.ToString()+"\r\n"+zzSize, CurrentBar-idx, double.Parse(High[CurrentBar-idx].ToString())+2.5, draw_color);
					break;
				}
				if(zzSize > 0) {
					idx_hilo = idx;
					if(printOut)
						Print(idx + " DrawZZSize called");
					if(zzSizeAbs >= 10) draw_color = up_color;
					it = DrawText(tag+idx.ToString(), GetTimeDate(Time[CurrentBar-idx], 1)+"\r\n#"+idx.ToString()+"\r\n"+zzSize, CurrentBar-idx, double.Parse(Low[CurrentBar-idx].ToString())-2.5, draw_color);
					break;
				}
				it.Locked = false;
			}
			return true; 
		}
		
		/// <summary>
		/// Draw Gap from current bar to last ZZ
		/// </summary>
		/// <returns></returns>
		public double DrawGapText(double zzGap, string tag)
		{
			int idx_hilo = -1; // the last ZZ hi or ZZ lo index;
			double gap = 0;
			Color up_color = Color.Green;
			Color dn_color = Color.Red;
			Color sm_color = Color.Black;
			Color draw_color = sm_color;
			//Update();
			if(printOut)
				Print(CurrentBar + " DrawGapText called");
//			double zzhi = getLastZZHighLow(CurrentBar, 1, out idx_hi); //zigZagHighZigZags.Get(idx);
//			double zzlo = getLastZZHighLow(CurrentBar, -1, out idx_lo); // zigZagLowZigZags.Get(idx);
			if(it_gap != null) RemoveDrawObject(it_gap);
			double zzSize = zigZagSizeZigZag.Get(CurrentBar);//the previous zz size
			//double zzSizeAbs = Math.Abs(zzSize);
			//IText it = null;
			for (int idx = CurrentBar -1; idx >= BarsRequired; idx--)
			{
				//zzS = zigZagSizeSeries.Get(idx);
				zzSize = zigZagSizeZigZag.Get(idx);
				//zzSizeAbs = Math.Abs(zzSize);
				//String str_suffix = "";
				//Print(idx.ToString() + " - ZZSizeSeries=" + zzS);
				if(zzSize == 0) continue;
				else if(zzSize>0) {
					gap = Low[0] - High[CurrentBar-idx];
					draw_color = dn_color;
					it_gap = DrawText(tag+CurrentBar.ToString(), GetTimeDate(Time[0], 1)+"\r\n#"+gap+":"+zzGap, 0, double.Parse(Low[0].ToString())-1, draw_color);
					break;
				}
				else if(zzSize<0) {
					gap = High[0] - Low[CurrentBar-idx];
					draw_color = up_color;
					it_gap = DrawText(tag+CurrentBar.ToString(), GetTimeDate(Time[0], 1)+"\r\n#"+gap+":"+zzGap, 0, double.Parse(High[0].ToString())+1, draw_color);
					break;
				}
			}
			if(it_gap != null) it_gap.Locked = false;
			Print(CurrentBar + "::" + this.ToString() + " GaP= " + gap + " - " + Time[0].ToShortTimeString());
			return gap; 
		}
*/		
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
			CheckPerformance();
			ChangeSLPT();
			CheckEnOrder();
			if(printOut > -1)
				Print(CurrentBar + "-" + Account.Name + ":GI gap=" + gap + "," + Position.MarketPosition.ToString() + "=" + Position.Quantity.ToString()+ ", price=" + Position.AvgPrice + ", BarsSinceEx=" + bsx + ", BarsSinceEn=" + bse);
			
			DrawGapText(gap, "gap-");
			*/
//			if(NewOrderAllowed()) 
//			{
//				PutTrade(gap);
//			}
						
			//if(backTest && IsLastBarOnChart() > 0) {
				//bool GIZZ = GIZigZag(DeviationType.Points, retracePnts, false, false, false, true).GetZigZag(out zigZagSizeSeries, out zigZagSizeZigZag);
				//PrintZZSize();
			//}
        }

/*
		protected void NewShortLimitOrder(string msg)
		{
			double prc = High[0]+EnOffsetPnts;
			entryOrder = EnterShortLimit(0, true, DefaultQuantity, prc, zzEntrySignal);
			//AccountItem.RealizedProfitLoss;
			
			//if(printOut)
			Print(CurrentBar + "-" + Account.Name + ":" + msg + ", EnterShortLimit called short price=" + prc + "--" + Time[0].ToString());			
		}
		
		protected void NewLongLimitOrder(string msg)
		{
			double prc = Low[0]-EnOffsetPnts;
			entryOrder = EnterLongLimit(0, true, DefaultQuantity, prc, zzEntrySignal);
			//if(printOut)
			Print(CurrentBar + "-" + Account.Name + ":" + msg +  ", EnterLongLimit called buy price= " + prc + " -- " + Time[0].ToString());		
		}
		
		protected bool NewOrderAllowed()
		{
			int bsx = BarsSinceExit();
			int bse = BarsSinceEntry();
			double pnl = GetAccountValue(AccountItem.RealizedProfitLoss);
			double plrt = Performance.RealtimeTrades.TradesPerformance.Currency.CumProfit;
			Print(CurrentBar + "-" + Account.Name + ": GetAccountValue(AccountItem.RealizedProfitLoss)= " + pnl + " -- " + Time[0].ToString());	

			if((backTest && !Historical) || (!backTest && Historical))
				return false;
			if(!backTest && plrt <= dailyLossLmt)
			{
				Print(CurrentBar + "-" + Account.Name + ": dailyLossLmt reached = " + plrt);
				return false;
			}
		
			if (ToTime(Time[0]) >= TimeStart && ToTime(Time[0]) <= TimeEnd && Position.Quantity == 0)
			{
				if (entryOrder == null || entryOrder.OrderState != OrderState.Working)
				{
					if(bsx == -1 || bsx > barsSincePtSl)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected bool ChangeSLPT()
		{
			double pl = Position.GetProfitLoss(Close[0], PerformanceUnit.Currency);
			 // If not flat print our unrealized PnL
    		if (Position.MarketPosition != MarketPosition.Flat) {
         		Print(Account.Name + " Open PnL: " + pl);
				if(pl >= breakEvenAmt) { //setup breakeven order
					SetStopLoss(0);
				}
			} else {
				SetStopLoss(StopLossAmt);
				SetProfitTarget(ProfitTargetAmt);
			}

			return false;
		}
		
		protected bool CheckEnOrder()
		{
			double min_en = -1;
			
			if(entryOrder != null && entryOrder.OrderState==OrderState.Working) 
			{
				min_en = GetMinutesDiff(entryOrder.Time, DateTime.Now);
				if(min_en >= minutesChkEnOrder ) {
					CancelOrder(entryOrder);
					Print("Order cancelled for " + Account.Name + ":" + min_en + " mins elapsed--" + entryOrder.ToString());
					return true;
				}
			}
			return false;
		}

		protected bool CheckPerformance()
		{
			double pl = Performance.AllTrades.TradesPerformance.Currency.CumProfit;
			double plrt = Performance.RealtimeTrades.TradesPerformance.Currency.CumProfit;
       		Print(Account.Name + " Cum all PnL: " + pl + ", Cum runtime PnL: " + plrt);
			return false;
		}
				
		protected override void OnExecution(IExecution execution)
		{
			// Remember to check the underlying IOrder object for null before trying to access its properties
			if (execution.Order != null && execution.Order.OrderState == OrderState.Filled) {
				//if(printOut)
					Print(CurrentBar + "-" + Account.Name + " Exe=" + execution.Name + ",Price=" + execution.Price + "," + execution.Time.ToShortTimeString());
				if(drawTxt) {
					IText it = DrawText(CurrentBar.ToString()+Time[0].ToShortTimeString(), Time[0].ToString().Substring(10)+"\r\n"+execution.Name+":"+execution.Price, 0, execution.Price, Color.Red);
					it.Locked = false;
				}
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
			if (order.OrderState == OrderState.Working) {
				Print(CurrentBar + "-" + Account.Name + ":" + order.ToString());
			}              
		}

		protected override void OnPositionUpdate(IPosition position)
		{
			//Print(position.ToString() + "--MarketPosition=" + position.MarketPosition);
			if (position.MarketPosition == MarketPosition.Flat)
			{
				// Do something like reset some variables here
			}
			else 
			{
				
			}
		}
*/
        #region Properties
		/*
        [Description("ZigZag retrace points")]
        [GridCategory("Parameters")]
        public double RetracePnts
        {
            get { return retracePnts; }
            set { retracePnts = Math.Max(1, value); }
        }

        [Description("Money amount of profit target")]
        [GridCategory("Parameters")]
        public double ProfitTargetAmt
        {
            get { return profitTargetAmt; }
            set { profitTargetAmt = value; }
        }

        [Description("Money amount of stop loss")]
        [GridCategory("Parameters")]
        public double StopLossAmt
        {
            get { return stopLossAmt; }
            set { stopLossAmt = value; }
        }
		
        [Description("Break Even amount")]
        [GridCategory("Parameters")]
        public double BreakEvenAmt
        {
            get { return breakEvenAmt; }
            set { breakEvenAmt = Math.Max(0, value); }
        }

		[Description("Daily Loss Limit amount")]
        [GridCategory("Parameters")]
        public double DailyLossLmt
        {
            get { return dailyLossLmt; }
            set { dailyLossLmt = Math.Min(-100, value); }
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
		
        [Description("How long to check entry order filled or not")]
        [GridCategory("Parameters")]
        public int MinutesChkEnOrder
        {
            get { return minutesChkEnOrder; }
            set { minutesChkEnOrder = Math.Max(0, value); }
        }		

        [Description("Bar count since last filled PT or SL")]
        [GridCategory("Parameters")]
        public int BarsSincePtSl
        {
            get { return barsSincePtSl; }
            set { barsSincePtSl = Math.Max(1, value); }
        }
		
		[Description("Bar count before checking P&L")]
        [GridCategory("Parameters")]
        public int BarsToCheckPL
        {
            get { return barsToCheckPL; }
            set { barsToCheckPL = Math.Max(1, value); }
        }

        [Description("Min swing size for entry")]
        [GridCategory("Parameters")]
        public double EnSwingMinPnts
        {
            get { return enSwingMinPnts; }
            set { enSwingMinPnts = Math.Max(1, value); }
        }

        [Description("Max swing size for entry")]
        [GridCategory("Parameters")]
        public double EnSwingMaxPnts
        {
            get { return enSwingMaxPnts; }
            set { enSwingMaxPnts = Math.Max(4, value); }
        }

		[Description("Min pullback size for entry")]
        [GridCategory("Parameters")]
        public double EnPullbackMinPnts
        {
            get { return enPullbackMinPnts; }
            set { enPullbackMinPnts = Math.Max(1, value); }
        }

        [Description("Max pullback size for entry")]
        [GridCategory("Parameters")]
        public double EnPullbackMaxPnts
        {
            get { return enPullbackMaxPnts; }
            set { enPullbackMaxPnts = Math.Max(2, value); }
        }
		
        [Description("Offeset points for limit price entry")]
        [GridCategory("Parameters")]
        public double EnOffsetPnts
        {
            get { return enOffsetPnts; }
            set { enOffsetPnts = Math.Max(0, value); }
        }		
		
        [Description("Short, Long or both direction for entry")]
        [GridCategory("Parameters")]
        public int TradeDirection
        {
            get { return tradeDirection; }
            set { tradeDirection = value; }
        }		

        [Description("Trade style: trend following, counter trend, scalp")]
        [GridCategory("Parameters")]
        public int TradeStyle
        {
            get { return tradeStyle; }
            set { tradeStyle = value; }
        }
		
		[Description("If it runs for backtesting")]
        [GridCategory("Parameters")]
        public bool BackTest
        {
            get { return backTest; }
            set { backTest = value; }
        }
		
		*/
        #endregion
    }
}
