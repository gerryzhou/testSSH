#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Collections.Generic;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
//Add trailing profits target: every hour trail certain amout profits up, and tight the range more and more;
//Add retrieving the parameters from file;
//Add WebAPI to read/write the parameters remotely;
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Base class for GSZigZag
    /// </summary>
    [Description("Base class for GSZigZag")]
    public class GSZigZagBase : Strategy
    {
        #region Variables
		
		protected double retracePnts = 6; // Default setting for RetracePnts
		
		protected double profitTargetAmt = 450; //36 Default(450-650 USD) setting for ProfitTargetAmt
		protected double profitTgtIncTic = 8; //8 Default tick Amt for ProfitTarget increase Amt
		protected double profitLockMinTic = 16; //24 Default ticks Amt for Min Profit locking
		protected double profitLockMaxTic = 30; //80 Default ticks Amt for Max Profit locking
        protected double stopLossAmt = 200; //16 Default setting for StopLossAmt
		protected double stopLossIncTic = 4; //4 Default tick Amt for StopLoss increase Amt
		protected double breakEvenAmt = 150; //150 the profits amount to trigger setting breakeven order
		protected double trailingSLAmt = 300; //300 Default setting for trailing Stop Loss Amt
		protected double dailyLossLmt = -200; //-300 the daily loss limit amount
		
        protected int timeStart = 90100; //93300 Default setting for TimeStart
        protected int timeEnd = 105900; // Default setting for TimeEnd
		protected int minutesChkEnOrder = 10; //how long before checking an entry order filled or not
		protected int minutesChkPnL = 30; //how long before checking P&L
		
		protected int barsHoldEnOrd = 2; // Bars count since en order was issued
        protected int barsSincePtSl = 1; // Bar count since last P&L was filled
		protected int barsToCheckPL = 2; // Bar count to check P&L since the entry
		//protected int barsPullback = 1; // Bars count for pullback
        protected double enSwingMinPnts = 11; //10 Default setting for EnSwingMinPnts
        protected double enSwingMaxPnts = 16; //16 Default setting for EnSwingMaxPnts
		protected double enPullbackMinPnts = 5; //6 Default setting for EnPullbackMinPnts
        protected double enPullbackMaxPnts = 9; //10 Default setting for EnPullbackMaxPnts
		protected double enOffsetPnts = 2;//Price offset for entry
		protected double enOffset2Pnts = 0.5;//Price offset for entry
		protected int enCounterPBBars = 1;//Bar count of pullback for breakout entry setup
		protected double enResistPrc = 2700; // Resistance price for entry order
		protected double enSupportPrc = 2600; // Support price for entry order
		
		protected bool enTrailing = true; //use trailing entry: counter pullback bars or simple enOffsetPnts
		protected bool ptTrailing = true; //use trailing profit target every bar
		protected bool slTrailing = true; //use trailing stop loss every bar
		protected bool resistTrailing = false; //track resistance price for entry order
		protected bool supportTrailing = false; //track resistance price for entry order
		
		protected int tradeDirection = 0; // -1=short; 0-both; 1=long;
		protected int tradeStyle = 1; // -1=counter trend; 1=trend following;
		protected bool backTest = false; //if it runs for backtesting;
		
		protected int printOut = 1; //0,1,2,3 more print
		protected bool drawTxt = false; // User defined variables (add any user defined variables below)
		protected IText it_gap = null; //the Text draw for gap on current bar

		protected IOrder entryOrder = null;
		protected IOrder profitTargetOrder = null;
		protected IOrder stopLossOrder = null;
		protected double trailingPTTic = 36; //400, tick amount of trailing target
		protected double trailingSLTic = 16; // 200, tick amount of trailing stop loss
		protected int barsSinceEnOrd = 0; // bar count since the en order issued
		
		protected string AccName = null;
		
		protected IDataSeries zzHighValue;
		protected IDataSeries zzLowValue;
		protected DataSeries		zigZagSizeSeries;
		protected DataSeries		zigZagSizeZigZag;
		protected Dictionary<int, IText> dictZZText;

		//protected double[]			lastZZs; // the ZZ prior to cur bar
		protected ZigZagSwing[]		latestZZs;
		protected string zzEntrySignal = "ZZEntry";

		protected int ZZ_Count_0_6 = 0;
		protected int ZZ_Count_6_10 = 0;
		protected int ZZ_Count_10_16 = 0;
		protected int ZZ_Count_16_22 = 0;
		protected int ZZ_Count_22_30 = 0;
		protected int ZZ_Count_30_ = 0;
		protected int ZZ_Count = 0;
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(GIZigZag(NinjaTrader.Data.DeviationType.Points, retracePnts, false, false, false, true));
			AccName = GetTsTAccName(Account.Name);
//            SetProfitTarget("EnST1", CalculationMode.Ticks, ProfitTargetAmt);
//            SetStopLoss("EnST1", CalculationMode.Ticks, StopLossAmt, false);
//			SetProfitTarget("EnLN1", CalculationMode.Ticks, ProfitTargetAmt);
//            SetStopLoss("EnLN1", CalculationMode.Ticks, StopLossAmt, false);
			zigZagSizeSeries = new DataSeries(this, MaximumBarsLookBack.Infinite);
			zigZagSizeZigZag = new DataSeries(this, MaximumBarsLookBack.Infinite);
			dictZZText = new Dictionary<int, IText>();
			
			trailingPTTic = profitTargetAmt/12.5;
			trailingSLTic = stopLossAmt/12.5;
			barsSinceEnOrd = 0;
//			SetProfitTarget(CalculationMode.Ticks, ProfitTargetAmt);
//            SetStopLoss(CalculationMode.Ticks, StopLossAmt);
            SetStopLoss(StopLossAmt);
			SetProfitTarget(ProfitTargetAmt);
			DefaultQuantity = 1;
			
            CalculateOnBarClose = true;
			IncludeCommission = true;
			TimeInForce = Cbi.TimeInForce.Day;
			SyncAccountPosition = true;
			// Triggers the exit on close function 30 seconds prior to session end
			ExitOnClose = true;
			ExitOnCloseSeconds = 30;
        }
		
		/// <summary>
		/// Print zig zag size.
		/// </summary>
		public void PrintZZSize()
		{
			String str_Plus = " ++ ";
			String str_Minus = " -- ";
			String str_Minutes = "m";
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
					if(PrintOut > 1)
						Print(idx.ToString() + "-ZZ= " + zzSize + " [" + Time[CurrentBar-lastZZIdx].ToString() + "-" + Time[CurrentBar-idx].ToString() + "] >=10" + str_suffix + GetTimeDiff(Time[CurrentBar-lastZZIdx], Time[CurrentBar-idx]) + str_Minutes);
				}
				else if(zzSizeAbs >= 16 && zzSizeAbs <22){
					ZZ_Count_16_22 ++;
					if(PrintOut > 1)
						Print(idx.ToString() + "-ZZ= " + zzSize + " [" + Time[CurrentBar-lastZZIdx].ToString() + "-" + Time[CurrentBar-idx].ToString() + "] >=16" + str_suffix + GetTimeDiff(Time[CurrentBar-lastZZIdx], Time[CurrentBar-idx]) + str_Minutes);
				}
				else if(zzSizeAbs >= 22 && zzSizeAbs <30){
					ZZ_Count_22_30 ++;
					if(PrintOut > 1)
						Print(idx.ToString() + "-ZZ= " + zzSize + " [" + Time[CurrentBar-lastZZIdx].ToString() + "-" + Time[CurrentBar-idx].ToString() + "] >=22" + str_suffix + GetTimeDiff(Time[CurrentBar-lastZZIdx], Time[CurrentBar-idx]) + str_Minutes);
				}
				else if(zzSizeAbs >= 30){
					ZZ_Count_30_ ++;
					if(PrintOut > 1)
						Print(idx.ToString() + "-ZZ= " + zzSize + " [" + Time[CurrentBar-lastZZIdx].ToString() + "-" + Time[CurrentBar-idx].ToString() + "] >=30" + str_suffix + GetTimeDiff(Time[CurrentBar-lastZZIdx], Time[CurrentBar-idx]) + str_Minutes);
				}
				if(zzSize != 0) {
					DrawZZSizeText(idx, "txt-");
					if(zzSizeAbs < 10)
						if(PrintOut > 2)
							Print(idx.ToString() + "-zzS= " + zzSize + " [" + Time[CurrentBar-lastZZIdx].ToString() + "-" + Time[CurrentBar-idx].ToString() + "]" );
					lastZZIdx = idx;
				}
			}
			ZZ_Count = ZZ_Count_0_6 + ZZ_Count_6_10 + ZZ_Count_10_16 + ZZ_Count_16_22 + ZZ_Count_22_30 + ZZ_Count_30_ ;
			if(PrintOut > 2)
				Print(CurrentBar + "\r\n ZZ_Count \t" + ZZ_Count + "\r\n ZZ_Count_0_6 \t" + ZZ_Count_0_6 + "\r\n ZZ_Count_6_10 \t" + ZZ_Count_6_10 + "\r\n ZZ_Count_10_16 \t" + ZZ_Count_10_16 + "\r\n ZZ_Count_16_22 \t" + ZZ_Count_16_22 + "\r\n ZZ_Count_22_30 \t" + ZZ_Count_22_30 + "\r\n ZZ_Count_30_ \t" + ZZ_Count_30_);
		}

		
		/// <summary>
		/// Draw the ZZ size for barNo
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
			if(PrintOut > 3)
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
					if(PrintOut > 3)
						Print(idx + " DrawZZSize called");
					if(zzSizeAbs >= 10) draw_color = dn_color;
					it = DrawText(tag+idx.ToString(), GetTimeDate(Time[CurrentBar-idx], 1)+"\r\n#"+idx.ToString()+"\r\n"+zzSize, CurrentBar-idx, double.Parse(High[CurrentBar-idx].ToString())+2.5, draw_color);
					break;
				}
				if(zzSize > 0) {
					idx_hilo = idx;
					if(PrintOut > 3)
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
		/// Draw the ZZ size for the latest ZZs
		/// </summary>
		/// <param name="zzs"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool DrawZZSizeText(ZigZagSwing[] zzs, string tag)
		{
			int idx_hilo = -1; // the last ZZ hi or ZZ lo index;
			Color up_color = Color.Green;
			Color dn_color = Color.Red;
			Color sm_color = Color.Black;
			Color draw_color = sm_color;
			
			double zzSize = -1;//the previous zz size
			double zzSizeAbs = Math.Abs(zzSize);
			IText it = null;
			Print(CurrentBar + " DrawZZSizeText called:" + zzs.Length);			
			
			for (int idx = zzs.Length-1; idx >= 0; idx--)
			{
				zzSize = zzs[idx].Size;
				Print(CurrentBar + " DrawZZSizeText zzSize:" + zzSize);
				
				if(zzSize == 0) return false;
				
				zzSizeAbs = Math.Abs(zzSize);
				idx_hilo = zzs[idx].Bar_End;
				Print(CurrentBar + " DrawZZSizeText idx_hilo:" + idx_hilo);
				draw_color = sm_color;
				if(PrintOut > 3)
					Print(CurrentBar + " DrawZZSizeText called");
				
				if (dictZZText.ContainsKey(idx_hilo)) {
					dictZZText.TryGetValue(idx_hilo, out it);
					dictZZText.Remove(idx_hilo);
					RemoveDrawObject(it);
				}

				if(zzSize < 0) {					
					if(PrintOut > 3)
						Print(idx_hilo + " DrawZZSize2 called");
					if(zzSizeAbs >= 10) draw_color = dn_color;
					it = DrawText(tag+idx_hilo.ToString(), GetTimeDate(Time[CurrentBar-idx_hilo], 1)+"\r\n#"+idx_hilo.ToString()+"\r\n"+zzSize, CurrentBar-idx_hilo, double.Parse(High[CurrentBar-idx_hilo].ToString())+2.5, draw_color);
					dictZZText.Add(idx_hilo,it);
					break;
				} else if(zzSize > 0) {
					if(PrintOut > 3)
						Print(idx_hilo + " DrawZZSize2 called");
					if(zzSizeAbs >= 10) draw_color = up_color;
					it = DrawText(tag+idx_hilo.ToString(), GetTimeDate(Time[CurrentBar-idx_hilo], 1)+"\r\n#"+idx_hilo.ToString()+"\r\n"+zzSize, CurrentBar-idx_hilo, double.Parse(Low[CurrentBar-idx_hilo].ToString())-2.5, draw_color);
					dictZZText.Add(idx_hilo,it);
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
			if(PrintOut > 3)
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
			if(zzSize == 0) {
				if(zzGap > 0) draw_color = up_color;
				else if (zzGap < 0) draw_color = dn_color;
				it_gap = DrawText(tag+CurrentBar.ToString(), GetTimeDate(Time[0], 1)+"\r\n#"+gap+":"+zzGap, 0, double.Parse(Low[0].ToString())-1, draw_color);
			}
			if(it_gap != null) it_gap.Locked = false;
			if(PrintOut > 0)
				Print(CurrentBar + "::" + this.ToString() + " GaP= " + gap + " - " + Time[0].ToShortTimeString());
			return gap; 
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			//FileTest(CurrentBar);
			
			if(!Historical) Print(CurrentBar + "- GSZZ1 OnBarUpdate - " + Time[0].ToShortTimeString());
			if(CurrentBar < BarsRequired+2) return;
			int bsx = BarsSinceExit();
			int bse = BarsSinceEntry();
			
			double gap = GIZigZag(DeviationType.Points, retracePnts, false, false, false, true).ZigZagGap[0];
			//lastZZs = GIZigZag(DeviationType.Points, retracePnts, false, false, false, true).GetZigZag(CurrentBar, 3, retracePnts, 100);
			latestZZs = GIZigZag(DeviationType.Points, retracePnts, false, false, false, true).GetZigZag(CurrentBar, 1, retracePnts, 100);
			//GIZigZag(DeviationType.Points, retracePnts, false, false, false, true).GetZigZag(out zigZagSizeSeries, out zigZagSizeZigZag);
//			if(printOut > 2)
//				for (int idx = 0; idx < 3; idx++)
//				{
//					double zzS = 0;//zigZagSizeSeries.Get(idx);
//					double zzSize = lastZZs[idx];
//					Print(CurrentBar + "-" + AccName + ":(zzSize,zzS)=" + zzSize + "," + zzS);
//				}
						
			if(!backTest) {
				DrawZZSizeText(latestZZs, "zz-");
				DrawGapText(gap, "gap-");
			}
			
			CheckPerformance();
			ChangeSLPT();
			CheckEnOrder(gap);
			if(printOut > -1) {				
				Print(CurrentBar + "-" + AccName + ":GI gap=" + gap + "," + Position.MarketPosition.ToString() + "=" + Position.Quantity.ToString()+ ", price=" + Position.AvgPrice + ", BarsSinceEx=" + bsx + ", BarsSinceEn=" + bse);
			}	
						
			if(NewOrderAllowed())
			{
				PutTrade(gap);
			}
			
			if(backTest && printOut > 1 && IsLastBarOnChart() > 0) {
				bool GIZZ = GIZigZag(DeviationType.Points, retracePnts, false, false, false, true).GetZigZag(out zigZagSizeSeries, out zigZagSizeZigZag);
				PrintZZSize();
			}
        }

		protected double GetLastZZ(){
			double zz = 0;
			if(latestZZs.Length > 0)
				zz = latestZZs[latestZZs.Length-1].Size;
			return zz;
		}
		protected void PutTrade(double gap) {
			double gapAbs = Math.Abs(gap);			
			double lastZZ = GetLastZZ();
			double lastZZAbs = Math.Abs(lastZZ);
			if(entryOrder == null)
				Print(CurrentBar + "-" + AccName + ":PutOrder-(tradeStyle,tradeDirection,gap,enSwingMinPnts,enSwingMaxPnts,enPullbackMinPnts,enPullbackMaxPnts, entryOrder)= " + tradeStyle + "," + tradeDirection + "," + gap + "," + enSwingMinPnts + "," + enSwingMaxPnts + "," + enPullbackMinPnts + "," + enPullbackMaxPnts + "--null");
			else
				Print(CurrentBar + "-" + AccName + ":PutOrder-(tradeStyle,tradeDirection,gap,enSwingMinPnts,enSwingMaxPnts,enPullbackMinPnts,enPullbackMaxPnts, entryOrder)= " + tradeStyle + "," + tradeDirection + "," + gap + "," + enSwingMinPnts + "," + enSwingMaxPnts + "," + enPullbackMinPnts + "," + enPullbackMaxPnts + "--" + entryOrder.ToString());
			if(tradeStyle == 0) // scalping, counter trade the pullbackMinPnts
			{
				if(tradeDirection >= 0) //1=long only, 0 is for both;
				{
					if(gap < 0 && gapAbs >= enPullbackMinPnts && gapAbs < enPullbackMaxPnts)
						NewLongLimitOrder("scalping long");
				}
				 
				if(tradeDirection <= 0) //-1=short only, 0 is for both;
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
				
				if(tradeDirection <= 0) //-1=short only, 0 is for both;
				{
					if(gap > 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts)
						NewShortLimitOrder("counter trade short");
				}
			}
			 // tradeStyle > 0, trend following, tradeStyle=1:entry at breakout; tradeStyle=2:entry at pullback;
			else if(tradeStyle == 1) //entry at breakout
			{
				if(tradeDirection >= 0) //1=long only, 0 is for both;
				{
					if(gap > 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts)
						if(enCounterPBBars < 0 || IsTwoBarReversal(gap, TickSize, enCounterPBBars))
							NewLongLimitOrder("trend follow long entry at breakout");
				}
				
				if(tradeDirection <= 0) //-1=short only, 0 is for both;
				{
					if(gap < 0 && gapAbs >= enSwingMinPnts && gapAbs < enSwingMaxPnts)
						if(enCounterPBBars < 0 || IsTwoBarReversal(gap, TickSize, enCounterPBBars))
							NewShortLimitOrder("trend follow short entry at breakout");
				}
			}
			else if(tradeStyle == 2) //entry at pullback
			{
				Print(CurrentBar + "-" + AccName + ":PutOrder(tradeStyle,tradeDirection,gap,lastZZs[0],lastZZAbs,enPullbackMinPnts,enPullbackMaxPnts)= " + tradeStyle + "," + tradeDirection + "," + gap + "," + lastZZ + "," + lastZZAbs + "," + enPullbackMinPnts + "," + enPullbackMaxPnts);
				if(tradeDirection >= 0) //1=long only, 0 is for both;
				{
					if(gap < 0 && gapAbs >= enPullbackMinPnts && gapAbs < enPullbackMaxPnts && lastZZ > 0 && lastZZAbs >= enSwingMinPnts && lastZZAbs <= enSwingMaxPnts)
						NewLongLimitOrder("trend follow long entry at pullback");
				}
				
				if(tradeDirection <= 0) //-1=short only, 0 is for both;
				{
					if(gap > 0 && gapAbs >= enPullbackMinPnts && gapAbs < enPullbackMaxPnts && lastZZ < 0 && lastZZAbs >= enSwingMinPnts && lastZZAbs <= enSwingMaxPnts)
						NewShortLimitOrder("trend follow short entry at pullback");
				}
			}
			else {
				Print(CurrentBar + "-" + AccName + ":PutOrder no-(tradeStyle,tradeDirection,gap,enSwingMinPnts,enSwingMaxPnts,enPullbackMinPnts,enPullbackMaxPnts)= " + tradeStyle + "," + tradeDirection + "," + gap + "," + enSwingMinPnts + "," + enSwingMaxPnts + "," + enPullbackMinPnts + "," + enPullbackMaxPnts);
			}
		}

		protected void NewShortLimitOrder(string msg)
		{
			double prc = (enTrailing && enCounterPBBars>0) ? Close[0]+EnOffset2Pnts : High[0]+EnOffsetPnts;
			//enCounterPBBars
			if(entryOrder == null) {
				if(PrintOut > -1)
					Print(CurrentBar + "-" + AccName + ":" + msg + ", EnterShortLimit called short price=" + prc + "--" + Time[0].ToString());			
			}
			else if (entryOrder.OrderState == OrderState.Working) {
				if(PrintOut > -1)
					Print(CurrentBar + "-" + AccName + ":" + msg +  ", EnterShortLimit updated short price (old, new)=(" + entryOrder.LimitPrice + "," + prc + ") -- " + Time[0].ToString());		
				CancelOrder(entryOrder);
				//entryOrder = EnterShortLimit(0, true, DefaultQuantity, prc, zzEntrySignal);
			}
			entryOrder = EnterShortLimit(0, true, DefaultQuantity, prc, zzEntrySignal);
			barsSinceEnOrd = 0;
		}
		
		protected void NewLongLimitOrder(string msg)
		{
			double prc = (enTrailing && enCounterPBBars>0) ? Close[0]-EnOffset2Pnts :  Low[0]-EnOffsetPnts;

			if(entryOrder == null) {
				entryOrder = EnterLongLimit(0, true, DefaultQuantity, prc, zzEntrySignal);
				if(PrintOut > -1)
					Print(CurrentBar + "-" + AccName + ":" + msg +  ", EnterLongLimit called buy price= " + prc + " -- " + Time[0].ToString());		
			}
			else if (entryOrder.OrderState == OrderState.Working) {
				if(PrintOut > -1)
					Print(CurrentBar + "-" + AccName + ":" + msg +  ", EnterLongLimit updated buy price (old, new)=(" + entryOrder.LimitPrice + "," + prc + ") -- " + Time[0].ToString());
				CancelOrder(entryOrder);
				entryOrder = EnterLongLimit(0, true, DefaultQuantity, prc, zzEntrySignal);
			}
			barsSinceEnOrd = 0;
		}
		
		protected bool NewOrderAllowed()
		{
			int bsx = BarsSinceExit();
			int bse = BarsSinceEntry();
			double pnl = CheckAccPnL();//GetAccountValue(AccountItem.RealizedProfitLoss);
			double plrt = CheckAccCumProfit();
			if(PrintOut > -1)				
				Print(CurrentBar + "-" + AccName + ":(RealizedProfitLoss,RealtimeTrades.CumProfit)=(" + pnl + "," + plrt + ")--" + Time[0].ToString());	

			if((backTest && !Historical) || (!backTest && Historical)) {
				Print(CurrentBar + "-" + AccName + "[backTest,Historical]=" + backTest + "," + Historical + "- NewOrderAllowed=false - " + Time[0].ToString());
				return false;
			}
			if(!backTest && (plrt <= dailyLossLmt || pnl <= dailyLossLmt))
			{
				if(PrintOut > -1) {
					Print(CurrentBar + "-" + AccName + ": dailyLossLmt reached = " + pnl + "," + plrt);
				}
				return false;
			}
		
			if (IsTradingTime(TimeStart, TimeEnd, 170000) && Position.Quantity == 0)
			{
				if (entryOrder == null || entryOrder.OrderState != OrderState.Working || EnTrailing)
				{
					if(bsx == -1 || bsx > barsSincePtSl)
					{
						Print(CurrentBar + "-" + AccName + "- NewOrderAllowed=true - " + Time[0].ToString());
						return true;
					} else 
						Print(CurrentBar + "-" + AccName + "-NewOrderAllowed=false-[bsx,barsSincePtSl]" + bsx + "," + barsSincePtSl + " - " + Time[0].ToString());
				} else
					Print(CurrentBar + "-" + AccName + "-NewOrderAllowed=false-[entryOrder.OrderState,entryOrder.OrderType]" + entryOrder.OrderState + "," + entryOrder.OrderType + " - " + Time[0].ToString());
			} else 
				Print(CurrentBar + "-" + AccName + "-NewOrderAllowed=false-[TimeStart,TimeEnd,Position.Quantity]" + TimeStart + "," + TimeEnd + "," + Position.Quantity + " - " + Time[0].ToString());
				
			return false;
		}

		protected bool ChangeSLPT()
		{
			int bse = BarsSinceEntry();
			double timeSinceEn = -1;
			if(bse > 0) {
				timeSinceEn = GetMinutesDiff(Time[0], Time[bse]);
			}
			
			double pl = Position.GetProfitLoss(Close[0], PerformanceUnit.Currency);
			 // If not flat print out unrealized PnL
    		if (Position.MarketPosition != MarketPosition.Flat) 
			{
         		Print(AccName + "- Open PnL: " + pl);
				//int nChkPnL = (int)(timeSinceEn/minutesChkPnL);
				double curPTTics = -1;
				double slPrc = stopLossOrder == null ? Position.AvgPrice : stopLossOrder.StopPrice;
				
				if(ptTrailing && pl >= 12.5*(trailingPTTic - 2*profitTgtIncTic))
				{
					trailingPTTic = trailingPTTic + profitTgtIncTic;
					if(profitTargetOrder != null) {
						curPTTics = Math.Abs(profitTargetOrder.LimitPrice - Position.AvgPrice)/TickSize;
					}
					Print(AccName + "- update PT: PnL=" + pl + ",(trailingPTTic, curPTTics, $Amt, $Amt_cur)=(" + trailingPTTic + "," + curPTTics + "," + 12.5*trailingPTTic + "," + 12.5*curPTTics + ")");
					if(profitTargetOrder == null || trailingPTTic > curPTTics)
						SetProfitTarget(CalculationMode.Ticks, trailingPTTic);
				}
				
				if(pl >= breakEvenAmt) { //setup breakeven order
					Print(AccName + "- setup SL Breakeven: (PnL, posAvgPrc)=(" + pl + "," + Position.AvgPrice + ")");
					slPrc = Position.AvgPrice;
					//SetStopLoss(0);
				}
				
				if(slTrailing) { // trailing max and min profits then converted to trailing stop after over the max
//					if(trailingSLTic > profitLockMaxTic && pl >= 12.5*(trailingSLTic + 2*profitTgtIncTic)) {
//						trailingSLTic = trailingSLTic + profitTgtIncTic;
//						if(Position.MarketPosition == MarketPosition.Long)
//							slPrc = Position.AvgPrice+TickSize*trailingSLTic;
//						if(Position.MarketPosition == MarketPosition.Short)
//							slPrc = Position.AvgPrice-TickSize*trailingSLTic;
//						Print(AccName + "- update SL over Max: PnL=" + pl + "(slTrailing, trailingSLTic, slPrc)= (" + slTrailing + "," + trailingSLTic + "," + slPrc + ")");						
//					}
					if(trailingSLTic > profitLockMaxTic && pl >= 12.5*(trailingSLTic + 2*profitTgtIncTic)) {
						trailingSLTic = trailingSLTic + profitTgtIncTic;
						if(stopLossOrder != null)
							CancelOrder(stopLossOrder);
						if(profitTargetOrder != null)
							CancelOrder(profitTargetOrder);
						SetTrailStop(trailingSLAmt);
						Print(AccName + "- SetTrailStop over SL Max: PnL=" + pl + "(slTrailing, trailingSLTic, slPrc)= (" + slTrailing + "," + trailingSLTic + "," + slPrc + ")");						
					}
					else if(pl >= 12.5*(profitLockMaxTic + 2*profitTgtIncTic)) { // lock max profits
						trailingSLTic = trailingSLTic + profitTgtIncTic;
						if(Position.MarketPosition == MarketPosition.Long)
							slPrc = trailingSLTic > profitLockMaxTic ? Position.AvgPrice+TickSize*trailingSLTic : Position.AvgPrice+TickSize*profitLockMaxTic;
						if(Position.MarketPosition == MarketPosition.Short)
							slPrc = trailingSLTic > profitLockMaxTic ? Position.AvgPrice-TickSize*trailingSLTic :  Position.AvgPrice-TickSize*profitLockMaxTic;
						Print(AccName + "- update SL Max: PnL=" + pl + "(slTrailing, trailingSLTic, slPrc)= (" + slTrailing + "," + trailingSLTic + "," + slPrc + ")");
						//SetStopLoss(CalculationMode.Price, slPrc);
					}
					else if(pl >= 12.5*(profitLockMinTic + 2*profitTgtIncTic)) { //lock min profits
						trailingSLTic = trailingSLTic + profitTgtIncTic;
						if(Position.MarketPosition == MarketPosition.Long)
							slPrc = Position.AvgPrice+TickSize*profitLockMinTic;
						if(Position.MarketPosition == MarketPosition.Short)
							slPrc = Position.AvgPrice-TickSize*profitLockMinTic;
						Print(AccName + "- update SL Min: PnL=" + pl + "(slTrailing, trailingSLTic, slPrc)= (" + slTrailing + "," + trailingSLTic + "," + slPrc + ")");
						//SetStopLoss(CalculationMode.Price, slPrc);
					}
				}
				if(stopLossOrder == null || 
					(Position.MarketPosition == MarketPosition.Long && slPrc > stopLossOrder.StopPrice) ||
					(Position.MarketPosition == MarketPosition.Short && slPrc < stopLossOrder.StopPrice)) 
				{
					SetStopLoss(CalculationMode.Price, slPrc);
				}
			} else {
				SetStopLoss(StopLossAmt);
				SetProfitTarget(ProfitTargetAmt);
			}

			return false;
		}
		
		public double CheckAccPnL() {
			double pnl = GetAccountValue(AccountItem.RealizedProfitLoss);
			//Print(CurrentBar + "-" + AccName + ": GetAccountValue(AccountItem.RealizedProfitLoss)= " + pnl + " -- " + Time[0].ToString());
			return pnl;
		}
		
		public double CheckAccCumProfit() {
			double plrt = Performance.RealtimeTrades.TradesPerformance.Currency.CumProfit;
			//Print(CurrentBar + "-" + AccName + ": Cum runtime PnL= " + plrt);
			return plrt;
		}
		
		public double CheckPerformance()
		{
			double pl = Performance.AllTrades.TradesPerformance.Currency.CumProfit;
			double plrt = Performance.RealtimeTrades.TradesPerformance.Currency.CumProfit;
			Print(CurrentBar + "-" + AccName + ": Cum all PnL= " + pl + ", Cum runtime PnL= " + plrt);
			return plrt;
		}
		
		public bool CheckEnOrder(double cur_gap)
        {
            double min_en = -1;

            if (entryOrder != null && entryOrder.OrderState == OrderState.Working)
            {
                min_en = GetMinutesDiff(entryOrder.Time, Time[0]);// DateTime.Now);
                if ( IsTwoBarReversal(cur_gap, TickSize, enCounterPBBars) || (barsHoldEnOrd > 0 && barsSinceEnOrd >= barsHoldEnOrd) || ( minutesChkEnOrder > 0 &&  min_en >= minutesChkEnOrder))
                {
                    CancelOrder(entryOrder);
                    Print("Order cancelled for " + AccName + ":" + barsSinceEnOrd + "/" + min_en + " bars/mins elapsed--" + entryOrder.ToString());
					return true;
                }
				else {
					Print("Order working for " + AccName + ":" + barsSinceEnOrd + "/" + min_en + " bars/mins elapsed--" + entryOrder.ToString());
					barsSinceEnOrd++;
				}
            }
            return false;
        }
		
		protected override void OnExecution(IExecution execution)
		{
			// Remember to check the underlying IOrder object for null before trying to access its properties
			if (execution.Order != null && execution.Order.OrderState == OrderState.Filled) {
				if(PrintOut > -1)
					Print(CurrentBar + "-" + AccName + " Exe=" + execution.Name + ",Price=" + execution.Price + "," + execution.Time.ToShortTimeString());
				if(drawTxt) {
					IText it = DrawText(CurrentBar.ToString()+Time[0].ToShortTimeString(), Time[0].ToString().Substring(10)+"\r\n"+execution.Name+":"+execution.Price, 0, execution.Price, Color.Red);
					it.Locked = false;
				}
			}
		}

		protected override void OnOrderUpdate(IOrder order)
		{
		    if (entryOrder != null && entryOrder == order)
		    {
				Print(order.ToString() + "--" + order.OrderState);
		        if (order.OrderState == OrderState.Cancelled || 
					order.OrderState == OrderState.Filled || 
					order.OrderState == OrderState.Rejected || 
					order.OrderState == OrderState.Unknown)
				{
					barsSinceEnOrd = 0;
					entryOrder = null;
				}
		    }
			
			if (order.OrderState == OrderState.Working || order.OrderType == OrderType.Stop) {
				if(PrintOut > -1)
					Print(CurrentBar + "-" + AccName + ":" + order.ToString());
			}
			
			if(profitTargetOrder == null && order.Name == "Profit target" && order.OrderState == OrderState.Working) {
				profitTargetOrder = order;
			}
			if(stopLossOrder == null && order.Name == "Stop loss" && (order.OrderState == OrderState.Accepted || order.OrderState == OrderState.Working)) {
				stopLossOrder = order;
			}
			
			if( order.OrderState == OrderState.Filled || order.OrderState == OrderState.Cancelled) {
				if(order.Name == "Stop loss")
					stopLossOrder = null;
				if(order.Name == "Profit target")
					profitTargetOrder = null;
			}
		}

		protected override void OnPositionUpdate(IPosition position)
		{
			//Print(position.ToString() + "--MarketPosition=" + position.MarketPosition);
			if (position.MarketPosition == MarketPosition.Flat)
			{
				trailingPTTic = profitTargetAmt/12.5;
				trailingSLTic = stopLossAmt/12.5;
			}
		}
		
        #region Properties
		
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
            set { profitTargetAmt = Math.Max(0, value); }
        }

        [Description("Money amount for profit target increasement")]
        [GridCategory("Parameters")]
        public double ProfitTgtIncTic
        {
            get { return profitTgtIncTic; }
            set { profitTgtIncTic = Math.Max(0, value); }
        }
		
        [Description("Tick amount for min profit locking")]
        [GridCategory("Parameters")]
        public double ProfitLockMinTic
        {
            get { return profitLockMinTic; }
            set { profitLockMinTic = Math.Max(0, value); }
        }

		[Description("Tick amount for max profit locking")]
        [GridCategory("Parameters")]
        public double ProfitLockMaxTic
        {
            get { return profitLockMaxTic; }
            set { profitLockMaxTic = Math.Max(0, value); }
        }
		
        [Description("Money amount of stop loss")]
        [GridCategory("Parameters")]
        public double StopLossAmt
        {
            get { return stopLossAmt; }
            set { stopLossAmt = Math.Max(0, value); }
        }
		
        [Description("Money amount of trailing stop loss")]
        [GridCategory("Parameters")]
        public double TrailingStopLossAmt
        {
            get { return trailingSLAmt; }
            set { trailingSLAmt = Math.Max(0, value); }
        }
		
		[Description("Money amount for stop loss increasement")]
        [GridCategory("Parameters")]
        public double StopLossIncTic
        {
            get { return stopLossIncTic; }
            set { stopLossIncTic = Math.Max(0, value); }
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
		
        [Description("How long to check P&L")]
        [GridCategory("Parameters")]
        public int MinutesChkPnL
        {
            get { return minutesChkPnL; }
            set { minutesChkPnL = Math.Max(-1, value); }
        }		

        [Description("Bar count since en order issued")]
        [GridCategory("Parameters")]
        public int BarsHoldEnOrd
        {
            get { return barsHoldEnOrd; }
            set { barsHoldEnOrd = Math.Max(1, value); }
        }
		
        [Description("Bar count for en order counter pullback")]
        [GridCategory("Parameters")]
        public int EnCounterPullBackBars
        {
            get { return enCounterPBBars; }
            set { enCounterPBBars = Math.Max(-1, value); }
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
		
        [Description("Offeset points for limit price entry, pullback entry")]
        [GridCategory("Parameters")]
        public double EnOffset2Pnts
        {
            get { return enOffset2Pnts; }
            set { enOffset2Pnts = Math.Max(0, value); }
        }
		
		[Description("Use trailing entry every bar")]
        [GridCategory("Parameters")]
        public bool EnTrailing
        {
            get { return enTrailing; }
            set { enTrailing = value; }
        }
		
		[Description("Use trailing profit target every bar")]
        [GridCategory("Parameters")]
        public bool PTTrailing
        {
            get { return ptTrailing; }
            set { ptTrailing = value; }
        }
		
		[Description("Use trailing stop loss every bar")]
        [GridCategory("Parameters")]
        public bool SLTrailing
        {
            get { return slTrailing; }
            set { slTrailing = value; }
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
		
		[Description("Print out level: large # print out more")]
        [GridCategory("Parameters")]
        public int PrintOut
        {
            get { return printOut; }
            set { printOut = Math.Max(-1, value); }
        }
		
        #endregion
    }
}
