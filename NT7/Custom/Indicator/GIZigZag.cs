// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// The GIZigZag indicator shows trend lines filtering out changes below a defined level. 
    /// </summary>
    [Description("The GIZigZag indicator shows trend lines filtering out changes below a defined level. ")]
    public class GIZigZag : Indicator
    {

		private double			currentZigZagHigh	= 0;
		private double			currentZigZagLow	= 0;
		private DeviationType	deviationType		= DeviationType.Points;
		private double			deviationValue		= 4;
		private DataSeries		zigZagHighZigZags;
		private DataSeries		zigZagLowZigZags;
		private DataSeries		zigZagHighSeries;
		private DataSeries		zigZagLowSeries;
		private DataSeries		zigZagSizeSeries;
		private DataSeries		zigZagSizeZigZag;
		private int				lastSwingIdx		= -1;
		private double			lastSwingPrice		= 0.0;
		private int				lastZigZagIdx		= -1;
		private double			lastZigZagPrice		= 0.0;
		private int				trendDir			= 0; // 1 = trend up, -1 = trend down, init = 0
		private bool			useHighLow			= true;
		private ArrayList  zzSizeArray;// = new ArrayList();
		private bool printOut = false;       
		private bool drawTxt = false;
		private bool drawZZTxt = true; // User defined variables (add any user defined variables below)
        

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Line, "GIZigZag"));

			zigZagHighSeries	= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			zigZagHighZigZags	= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			zigZagLowSeries		= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			zigZagLowZigZags	= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			zigZagSizeSeries	= new DataSeries(this, MaximumBarsLookBack.Infinite);
			zigZagSizeZigZag	= new DataSeries(this, MaximumBarsLookBack.Infinite);
			DisplayInDataBox	= false;
            Overlay				= true;
			PaintPriceMarkers	= false;
			AllowRemovalOfDrawObjects = true;
//			Print("this.ChartControl.LastBarPainted=" + this.ChartControl.LastBarPainted);
//			Print("this.LastVisibleBar=" + this.LastVisibleBar);
//			Print("this.LastBarIndexPainted=" + this.LastBarIndexPainted);
        }

		/// <summary>
		/// Returns the number of bars ago a zig zag low occurred. Returns a value of -1 if a zig zag low is not found within the look back period.
		/// </summary>
		/// <param name="barsAgo"></param>
		/// <param name="instance"></param>
		/// <param name="lookBackPeriod"></param>
		/// <returns></returns>
		public int LowBar(int barsAgo, int instance, int lookBackPeriod) 
		{
			if (instance < 1)
				throw new Exception(GetType().Name + ".LowBar: instance must be greater/equal 1 but was " + instance);
			else if (barsAgo < 0)
				throw new Exception(GetType().Name + ".LowBar: barsAgo must be greater/equal 0 but was " + barsAgo);
			else if (barsAgo >= Count)
				throw new Exception(GetType().Name + ".LowBar: barsAgo out of valid range 0 through " + (Count - 1) + ", was " + barsAgo + ".");

			Update();
			for (int idx = CurrentBar - barsAgo - 1; idx >= CurrentBar - barsAgo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return -1;
				if (idx >= zigZagLowZigZags.Count)
					continue;				

				if (zigZagLowZigZags.Get(idx).Equals(0.0))			
					continue;

				if (instance == 1) // 1-based, < to be save
					return CurrentBar - idx;	

				instance--;
			}
	
			return -1;
		}


		/// <summary>
		/// Returns the number of bars ago a zig zag high occurred. Returns a value of -1 if a zig zag high is not found within the look back period.
		/// </summary>
		/// <param name="barsAgo"></param>
		/// <param name="instance"></param>
		/// <param name="lookBackPeriod"></param>
		/// <returns></returns>
		public int HighBar(int barsAgo, int instance, int lookBackPeriod) 
		{
			if (instance < 1)
				throw new Exception(GetType().Name + ".HighBar: instance must be greater/equal 1 but was " + instance);
			else if (barsAgo < 0)
				throw new Exception(GetType().Name + ".HighBar: barsAgo must be greater/equal 0 but was " + barsAgo);
			else if (barsAgo >= Count)
				throw new Exception(GetType().Name + ".HighBar: barsAgo out of valid range 0 through " + (Count - 1) + ", was " + barsAgo + ".");

			Update();
			for (int idx = CurrentBar - barsAgo - 1; idx >= CurrentBar - barsAgo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return -1;
				if (idx >= zigZagHighZigZags.Count)
					continue;				

				if (zigZagHighZigZags.Get(idx).Equals(0.0))			
					continue;

				if (instance <= 1) // 1-based, < to be save
					return CurrentBar - idx;	

				instance--;
			}

			return -1;
		}

		/// <summary>
		/// Print zig zag high/low occurred before the barNo.
		/// </summary>
		/// <param name="barNo"></param>
		/// <param name="lookBackPeriod"></param>
		/// <returns></returns>
		public bool PrintZZHighLow(int barNo, int lookBackPeriod) 
		{
			if (barNo < 0)
				throw new Exception(GetType().Name + ".PrintZZHighLow: barNo must be greater/equal 0 but was " + barNo);
			else if (barNo >= Count)
				throw new Exception(GetType().Name + ".PrintZZHighLow: barNo out of valid range 0 through " + (Count - 1) + ", was " + barNo + ".");

			Update();
			Print(barNo.ToString() + " PrintZZHL called");			

			for (int idx = barNo - 1; idx >= barNo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return false;
				//if (idx >= zigZagHighZigZags.Count) 
				//	continue;
				Print(idx.ToString() + "(L,H)=" + zigZagLowZigZags.Get(idx) + "," + zigZagHighZigZags.Get(idx));
				Print(idx.ToString() + "(l,h)=" + zigZagLowSeries.Get(idx) + "," + zigZagHighSeries.Get(idx));
			}

			return true;
		}

		/// <summary>
		/// Print zig zag size.
		/// </summary>
		public void PrintZZSize()
		{
			Update();
			Print(CurrentBar + " GI-PrintZZSize called");			

			int idx_prev = -1;
			double zzS = 0;
			double zzSize = 0;
			
			for (int idx = BarsRequired; idx <= Input.Count; idx++)
			{
				zzS= zigZagSizeSeries.Get(idx);
				zzSize = zigZagSizeZigZag.Get(idx);
				Print(idx.ToString() + " -GI ZZSizeSeries=" + zzS);
				Print(idx.ToString() + " -GI ZZSize=" + zzSize);
				DrawZZSizeText(idx, "txt-");
			}
			
		}

		/// <summary>
		/// Print zig zag high/low.
		/// </summary>
		public void PrintZZHiLo()
		{
			Update();
			Print(CurrentBar + " PrintZZHiLo called");			

			for (int idx = BarsRequired; idx <= Input.Count; idx++)
			{
				Print(idx.ToString() + " - ZZLo, ZZHi=" + zigZagLowZigZags.Get(idx) + ", " + zigZagHighZigZags.Get(idx));
				Print(idx.ToString() + " - ZZLoSeries, ZZHiSeries=" + zigZagLowSeries.Get(idx) + ", " + zigZagHighSeries.Get(idx));
			}
		}
		/// <summary>
		/// Get the latest zig zag high or low occurred before the barNo.
		/// </summary>
		/// <param name="barNo"></param>
		/// <param name="lookBackPeriod"></param>
		/// <param name="HIorLO">return zz hi or low, 1=hi, -1=lo </param>
		/// <param name="rmMidZZ">Remove the zz points in the middle of the zz line</param>
		/// <returns>the price of the last ZZ high or ZZ low</returns>
		public double getLastZZHighLow(int barNo, int lookBackPeriod, int HIorLO, bool rmMidZZ)
		{
			if (barNo < 0)
				throw new Exception(GetType().Name + ".getLastZZHighLow: barNo must be greater/equal 0 but was " + barNo);
			else if (barNo >= Count)
				throw new Exception(GetType().Name + ".getLastZZHighLow: barNo out of valid range 0 through " + (Count - 1) + ", was " + barNo + ".");

			Update();
			return 0.0;
			if(printOut)
				Print(barNo.ToString() + " getLastZZHighLow1 called");

			for (int idx = barNo - 1; idx >= barNo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return -1;
				//if (idx >= zigZagHighZigZags.Count) 
				//	continue;
				//Print(idx.ToString() + "(L,H)=" + zigZagLowZigZags.Get(idx) + "," + zigZagHighZigZags.Get(idx));
				//Print(idx.ToString() + "(l,h)=" + zigZagLowSeries.Get(idx) + "," + zigZagHighSeries.Get(idx));
				double zzhi = zigZagHighZigZags.Get(idx);
				double zzlo = zigZagLowZigZags.Get(idx);
				
				if(HIorLO == -1)
				{
					if(zzlo > 0)
						return zzlo;
					else if (zzhi > 0) {
						RemoveText(idx, "txt-"+idx.ToString());
						zigZagSizeSeries.Set(CurrentBar-idx, 0);
					}
				}
				else if(HIorLO == 1)
				{
					if(zzhi > 0)
						return zzlo;
					else if (zzlo > 0) {
						RemoveText(idx, "txt-"+idx.ToString());
						zigZagSizeSeries.Set(CurrentBar-idx, 0);
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Get the latest zig zag high or low occurred before the barNo.
		/// </summary>
		/// <param name="barNo"></param>
		/// <param name="lookBackPeriod"></param>
		/// <param name="HIorLO">return zz hi or low, 1=hi, -1=lo </param>
		/// <param name="rmMidZZ">Remove the zz points in the middle of the zz line</param>
		/// <returns>the price of the last ZZ high or ZZ low</returns>
		public double getLastZZHighLow(int barNo, int HIorLO, out int idx_hilo)
		{
			if (barNo < 0)
				throw new Exception(GetType().Name + ".getLastZZHighLow: barNo must be greater/equal 0 but was " + barNo);
			else if (barNo >= Count)
				throw new Exception(GetType().Name + ".getLastZZHighLow: barNo out of valid range 0 through " + (Count - 1) + ", was " + barNo + ".");

			if(printOut)
				Print(barNo.ToString() + " getLastZZHighLow2 called");
			idx_hilo = -1;
			if(barNo < BarsRequired) return -1;
			
			Update();
			for (int idx = barNo - 1; idx >= BarsRequired; idx--)
			{
				if (idx < 0) {
					return -1;
				} else if (idx == BarsRequired) {
					idx_hilo = idx;
					if(HIorLO == -1) {
						return Low[CurrentBar-BarsRequired];
						} 
					else if(HIorLO == 1) {
						return High[CurrentBar-BarsRequired];
					}	
				}
				//if (idx >= zigZagHighZigZags.Count) 
				//	continue;
				//Print(idx.ToString() + "(L,H)=" + zigZagLowZigZags.Get(idx) + "," + zigZagHighZigZags.Get(idx));
				//Print(idx.ToString() + "(l,h)=" + zigZagLowSeries.Get(idx) + "," + zigZagHighSeries.Get(idx));
				double zzhi = zigZagHighZigZags.Get(idx);
				double zzlo = zigZagLowZigZags.Get(idx);
				
				if(HIorLO == -1 && zzlo > 0) {
					idx_hilo = idx;
					return zzlo;
				} else if(HIorLO == 1 && zzhi > 0) {
					idx_hilo = idx;
					return zzhi;
				}					
			}

			return -1;
		}
		
		/// <summary>
		/// Get the zig zag turning point and the size of the swing before the barNo.
		/// </summary>
		/// <param name="barNo"></param>
		/// <param name="lookBackPeriod"></param>
		/// <returns></returns>
		public double GetZigZagPoint(int barNo)
		{
			Update();
			if(printOut)
				Print(CurrentBar.ToString() + "," + barNo.ToString() + " getZZPoint called");
			if(zigZagLowZigZags.Get(CurrentBar-1) >0)
				return zigZagLowSeries.Get(CurrentBar-1) - zigZagHighSeries.Get(CurrentBar-1);
			else if(zigZagHighZigZags.Get(CurrentBar-1)>0)
				return zigZagHighSeries.Get(CurrentBar-1) - zigZagLowSeries.Get(CurrentBar-1);
			else
				return 0;
		}

		/// <summary>
		/// Get the latest zig zag size for the barNo.
		/// </summary>
		/// <param name="barNo"></param>
		/// <param name="lookBackPeriod"></param>
		/// <param name="HIorLO">return zz hi or low, 1=hi, -1=lo </param>
		/// <param name="rmMidZZ">Remove the zz points in the middle of the zz line</param>
		/// <returns>the price of the last ZZ high or ZZ low</returns>
		public double GetZigZagSize(int barNo, int lookBackPeriod, int HIorLO, bool rmMidZZ)
		{
			if (barNo < 0)
				throw new Exception(GetType().Name + ".getLastZZHighLow: barNo must be greater/equal 0 but was " + barNo);
			else if (barNo >= Count)
				throw new Exception(GetType().Name + ".getLastZZHighLow: barNo out of valid range 0 through " + (Count - 1) + ", was " + barNo + ".");

			Update();
			//Print(barNo.ToString() + " GetZigZagSize called");
			/*
			zzSizeArray = new ArrayList();
			for (int idx = barNo - 1; idx >= barNo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return 0;
				double zzhi = zigZagHighZigZags.Get(idx);
				double zzlo = zigZagLowZigZags.Get(idx);
				
				double zzSize = zigZagSizeSeries.Get(CurrentBar-1); //get the last zz turn point size
				double zzS = 0; //current bar zz size
				if(trendDir == -1 && zzhi > 0)
				{
					zzS = Low[0] - zzhi;

				}
				else if(trendDir == 1 && zzlo > 0)
				{
					zzS = High[0] - zzlo;
				}
				
				zigZagSizeSeries.Set(zzS);
				
				if(zzS*zzSize < 0){
					zigZagSizeZigZag.Set(CurrentBar-idx, zigZagSizeSeries.Get(idx));
				}
				Print(CurrentBar+" zigZagSizeSeries="+zigZagSizeSeries.Get(CurrentBar)+","+(CurrentBar-idx).ToString()+" zigZagSizeZigZag="+zigZagSizeZigZag.Get(CurrentBar-idx));
				return zzS;
			} */

			return 0;
		}

		/// <summary>
		/// Get the zig zag turning point and the size of the swing before the barNo.
		/// </summary>

		/// <returns></returns>
		public bool GetZigZag(out DataSeries zZSizeSeries, out DataSeries zZSize)
		{
			Update();
			if(printOut)
				Print(CurrentBar.ToString() + ", GetZigZag called");
			zZSizeSeries = zigZagSizeSeries;
			zZSize = zigZagSizeZigZag;
			return true;
		}
		
		/// <summary>
		/// Get the zig zag size of the swing before the barNo.
		/// </summary>
		/// <param name="barNo">before this bar</param>
		/// <param name="zzCount">how many zzSize needed</param>
		/// <returns>a list of the zz size</returns>
		public double[] GetZigZag(int barNo, int zzCount)
		{
			double[] zzSize = new double[zzCount];
			for(int i=0; i<zzCount; i++) {
				zzSize[i] = 0;
			}
			int zzCnt = 0;
			Update();
			for(int idx = barNo-1; idx >= BarsRequired; idx--) {
				if(zzCnt >= zzCount) break;
				else if(zigZagSizeZigZag.Get(idx) != 0){
					zzSize[zzCnt] = zigZagSizeZigZag.Get(idx);
					zzCnt++;
				}
			}

			return zzSize;
		}
		
		/// <summary>
		/// Get the zig zag size of the swing before the barNo.
		/// </summary>
		/// <param name="barNo">before this bar</param>
		/// <param name="zzCount">how many zzSize needed</param>
		/// <param name="zz_min">min size of the swing expected</param>
		/// <param name="zz_max">max size of the swing expected</param>
		/// <returns>a list of the zz size</returns>
		public ZigZagSwing[] GetZigZag(int barNo, int zzCount, double zz_min, double zz_max)
		{
			ZigZagSwing[] ZZSW = new ZigZagSwing[zzCount];
			//double[] zzSize = new double[zzCount];
			for(int i=0; i<zzCount; i++) {
				//zzSize[i] = 0;
				ZZSW[i].Bar_Start = -1;
				ZZSW[i].Bar_End = -1;
				ZZSW[i].Size = 0;
			}
			
			int zzCnt = 0;
			Update();
			for(int idx = barNo-1; idx >= BarsRequired; idx--) {
				double zzS = Math.Abs(zigZagSizeZigZag.Get(idx));
				if(zzCnt >= zzCount) break;
				else if( zzS >= zz_min && zzS <= zz_max){
					//zzSize[zzCnt] = zigZagSizeZigZag.Get(idx);
					//ZZSW[i].Bar_Start = -1;
					ZZSW[zzCnt].Bar_End = idx;
					ZZSW[zzCnt].Size = zigZagSizeZigZag.Get(idx);;
					zzCnt++;
				}
			}

			return ZZSW;
		}				
		/// <summary>
		/// Set the latest zig zag size for the barNo.
		/// </summary>
		/// <param name="barNo"></param>
		/// <param name="HIorLO">return zz hi or low, 1=hi, -1=lo </param>
		/// <returns>the price of the last ZZ high or ZZ low</returns>
		public double SetZigZagSize1(int barNo, int HIorLO)
		{
			if (barNo < 0)
				throw new Exception(GetType().Name + ".getLastZZHighLow: barNo must be greater/equal 0 but was " + barNo);
			else if (barNo >= Count)
				throw new Exception(GetType().Name + ".getLastZZHighLow: barNo out of valid range 0 through " + (Count - 1) + ", was " + barNo + ".");

			Update();
			if(printOut)
				Print(barNo.ToString() + " SetZigZagSize1 called");
			
			zzSizeArray = new ArrayList();
			for (int idx = barNo - 1; idx >= BarsRequired; idx--)
			{
				if (idx < 0)
					return 0;
				//if (idx >= zigZagHighZigZags.Count) 
				//	continue;
				//Print(idx.ToString() + "(L,H)=" + zigZagLowZigZags.Get(idx) + "," + zigZagHighZigZags.Get(idx));
				//Print(idx.ToString() + "(l,h)=" + zigZagLowSeries.Get(idx) + "," + zigZagHighSeries.Get(idx));
				double zzhi = zigZagHighZigZags.Get(idx);
				double zzlo = zigZagLowZigZags.Get(idx);
				
				double zzSize = zigZagSizeSeries.Get(CurrentBar-1); //get the last zz turn point size
				double zzS = 0; //current bar zz size
				if(trendDir == -1 && zzhi > 0)
				{
					zzS = Low[0] - zzhi;

				}
				else if(trendDir == 1 && zzlo > 0)
				{
					zzS = High[0] - zzlo;
				}
				
				zigZagSizeSeries.Set(zzS);
				
				if(zzS*zzSize < 0){
					zigZagSizeZigZag.Set(CurrentBar-idx, zigZagSizeSeries.Get(idx));
				}
				if(printOut)
					Print(CurrentBar+" zigZagSizeSeries="+zigZagSizeSeries.Get(CurrentBar)+","+(CurrentBar-idx).ToString()+" zigZagSizeZigZag="+zigZagSizeZigZag.Get(CurrentBar-idx));
				return zzS;
			}

			return 0;
		}

		/// <summary>
		/// Set the latest zig zag size.
		/// </summary>
		/// <param name="addHi"></param>
		/// <param name="addLo"></param>
		/// <param name="updHi"></param>
		/// <param name="updLo"></param>
		/// <returns>the price of the last ZZ high or ZZ low</returns>
		public double SetZigZagSize(bool addHi, bool addLo, bool updHi, bool updLo)
		{
			int idx_lo, idx_hi; // the last ZZ hi or ZZ lo index;
			Update();
			if(printOut)
				Print(CurrentBar + " SetZigZagSize2 called");
			double zzhi = getLastZZHighLow(CurrentBar, 1, out idx_hi); //zigZagHighZigZags.Get(idx);
			double zzlo = getLastZZHighLow(CurrentBar, -1, out idx_lo); // zigZagLowZigZags.Get(idx);
			
			double zzSize = 0;//the last zz turn point size
			double zzS = 0; //current bar zz size
			if(trendDir == -1)
			{
//				zzS = Low[1] - zzhi;
//				zigZagSizeSeries.Set(1, zzS);
				if(addLo) {
					zzS = Low[1] - zzhi;
					zigZagSizeSeries.Set(1, zzS);
					zzSize = zzlo - zzhi;
					zigZagSizeZigZag.Set(CurrentBar-idx_lo, zzSize);
					//zzlo = getLastZZHighLow(CurrentBar, -1, out idx_lo);
				} else if(updLo){
					zzSize = zzhi - zzlo;
					zigZagSizeZigZag.Set(CurrentBar-idx_hi, zzSize);
				}
				SetZigZagSize(-1, idx_lo); //fix the zzSize for previous zz lo/hi;
			}
			else if(trendDir == 1)
			{
//				zzS = High[1] - zzlo;
//				zigZagSizeSeries.Set(1, zzS);
				if(addHi) {
					zzS = High[1] - zzlo;
					zigZagSizeSeries.Set(1, zzS);
					zzSize = zzhi - zzlo;
					zigZagSizeZigZag.Set(CurrentBar-idx_hi, zzSize);
				} else if(updHi) {
					zzS = High[1] - zzlo;
					zigZagSizeSeries.Set(1, zzS);
				}
				SetZigZagSize(1, idx_hi);
			}
			if(printOut)
				Print(CurrentBar+ " || trendDir=" + trendDir +" || addHi, addLo, updHi, updLo="+addHi + "," + addLo + "," + updHi + "," + updLo);
			if(printOut)
				Print(CurrentBar+" || idx_hi,idx_lo="+idx_hi + "," + idx_lo + " || zzhi,zzlo="+zzhi + "," + zzlo + " ||zzS,zzSize=" + zzS + "," + zzSize);
			if(drawZZTxt) {
				DrawZZSizeText(idx_lo, "txt-");
				DrawZZSizeText(idx_hi, "txt-");
			}
			return zzSize;
		}
		
		/// <summary>
		/// Fix the zig zag size for last swing low/high.
		/// </summary>
		/// <param name="isOverLowDeviation"></param>
		/// <param name="isOverHighDeviation"></param>
		/// <returns>the size of the last ZZ high or ZZ low</returns>
		public double SetZigZagSize(bool isOverHighDeviation, bool isOverLowDeviation)
		{
			int idx_hilo = -1; // the last ZZ hi or ZZ lo index;
			Update();
			if(printOut)
				Print(CurrentBar + " SetZigZagSize3 called");
//			double zzhi = getLastZZHighLow(CurrentBar, 1, out idx_hi); //zigZagHighZigZags.Get(idx);
//			double zzlo = getLastZZHighLow(CurrentBar, -1, out idx_lo); // zigZagLowZigZags.Get(idx);
			
			double zzSize = 0;//et the last zz turn point size
			double zzS = 0; //current bar zz size
			for (int idx = CurrentBar - 1; idx >= BarsRequired; idx--)
			{
				zzS = zigZagSizeSeries.Get(idx);
				zzSize = zigZagSizeZigZag.Get(idx);
				if(zzS != 0 && zzSize == 0) {
					zigZagSizeZigZag.Set(CurrentBar-idx, zzS);
					idx_hilo = idx;
					break;
				}
			}
			if(printOut)
				Print(CurrentBar+ " || trendDir=" + trendDir +" || isOverHighDeviation, isOverLowDeviation="+isOverHighDeviation + "," + isOverLowDeviation);
			if(printOut)
				Print(CurrentBar+" ||idx_hilo="+idx_hilo + " ||zzS,zzSize=" + zzS + "," + zzSize);
				//zigZagSizeSeries.Set(zzS);
				
//				if(zzS*zzSize < 0){
//					zigZagSizeZigZag.Set(CurrentBar-idx, zigZagSizeSeries.Get(idx));
//				}
//				Print(CurrentBar+" zigZagSizeSeries="+zigZagSizeSeries.Get(CurrentBar)+","+(CurrentBar-idx).ToString()+" zigZagSizeZigZag="+zigZagSizeZigZag.Get(CurrentBar-idx));
			return zzSize;
		}

		/// <summary>
		/// Remove the previous zig zag size for the same direction.
		/// </summary>
		/// <param name="barNo">barNo that current zigzag appears</param>
		/// <param name="up_down">search for up or down zigzag</param>
		/// <returns>the size of the last ZZ high or ZZ low</returns>
		public double SetZigZagSize(int up_down, int barNo)
		{
			int idx_hilo = -1; // the last ZZ hi or ZZ lo index;
			Update();
			if(printOut)
				Print(CurrentBar + " SetZigZagSize4 called");
//			double zzhi = getLastZZHighLow(CurrentBar, 1, out idx_hi); //zigZagHighZigZags.Get(idx);
//			double zzlo = getLastZZHighLow(CurrentBar, -1, out idx_lo); // zigZagLowZigZags.Get(idx);
			
			double zzSize = 0;//the previous zz size
			for (int idx = barNo - 1; idx >= BarsRequired; idx--)
			{
				zzSize = zigZagSizeZigZag.Get(idx);
				if((up_down ==1 && zzSize < 0) || (up_down == -1 && zzSize > 0)) {
					idx_hilo = idx;
					break;
				} else if (up_down ==1 && zzSize > 0) {
					idx_hilo = idx;
					zigZagSizeZigZag.Set(CurrentBar-idx, 0);
				} else if (up_down == -1 && zzSize < 0) {
					idx_hilo = idx;
					zigZagSizeZigZag.Set(CurrentBar-idx, 0);
				}
			}
			if(printOut)
				Print(barNo+ " || up_down=" + up_down+" || idx_hilo="+idx_hilo + " || zzSize=" + zzSize);
			return zzSize;
		}

		/// <summary>
		/// Set the previous zig zag size and the current bar zz series.
		/// </summary>
		/// <param name="barNo">barNo that current zigzag appears</param>
		/// <param name="lastSwingIdx"></param>
		/// <param name="lastSwingPrice"></param>
		/// <returns>the size of the last ZZ high or ZZ low</returns>
		public double SetZigZagSize( int barNo, int lastSwingIdx, double lastSwingPrice)
		{
			int idx_hilo = -1; // the last ZZ hi or ZZ lo index;
			Update();
			
//			double zzhi = getLastZZHighLow(CurrentBar, 1, out idx_hi); //zigZagHighZigZags.Get(idx);
//			double zzlo = getLastZZHighLow(CurrentBar, -1, out idx_lo); // zigZagLowZigZags.Get(idx);
			//Is the last swing a swing high or swing low?
			bool isLastSwingHigh	= High[barNo-lastSwingIdx] >= High[barNo-lastSwingIdx-1] - double.Epsilon 
								&& High[barNo-lastSwingIdx] >= High[barNo-lastSwingIdx+1] - double.Epsilon;
			bool isLastSwingLow		= Low[barNo-lastSwingIdx] <= Low[barNo-lastSwingIdx-1] + double.Epsilon 
								&& Low[barNo-lastSwingIdx] <= Low[barNo-lastSwingIdx+1] + double.Epsilon;  
			
			int hi_ago = HighBar(0, 1, barNo-BarsRequired);
			int lo_ago = LowBar(0, 1, barNo-BarsRequired);
			
			double hi_ago_price = High[hi_ago];
			double lo_ago_price = High[lo_ago];
			
			double zzSize = 0;//the previous zz size
			double zzS = 0;//
			if(hi_ago < lo_ago) //last swing is a swing hi
			{
				zzS = hi_ago_price-Low[0];
			} else if(lo_ago < hi_ago) ////last swing is a swing lo
			{
				zzS = High[0]-lo_ago_price;
			}
			
			zigZagSizeSeries.Set(zzS);
			if(printOut)
				Print(CurrentBar + " SetZigZagSize5 called || lo_ago,hi_ago=" + (CurrentBar-lo_ago) + "," + (CurrentBar-hi_ago) + " || lo_ago_prc,hi_ago_prc=" + lo_ago_price + "," + hi_ago_price 
			+ "\r\n zzS=" + zzS + "||Low[0]=" + Low[0]+ "||High[0]=" + High[0]);
			for (int idx = barNo - 1; idx >= BarsRequired; idx--)
			{
				zzSize = zigZagSizeZigZag.Get(idx);
//				if((up_down ==1 && zzSize < 0) || (up_down == -1 && zzSize > 0)) {
//					idx_hilo = idx;
//					break;
//				} else if (up_down ==1 && zzSize > 0) {
//					idx_hilo = idx;
//					zigZagSizeZigZag.Set(CurrentBar-idx, 0);
//				} else if (up_down == -1 && zzSize < 0) {
//					idx_hilo = idx;
//					zigZagSizeZigZag.Set(CurrentBar-idx, 0);
//				}
			}
//			Print(barNo+ " || up_down=" + up_down+" || idx_hilo="+idx_hilo + " || zzSize=" + zzSize);
			return zzSize;
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
			//Update();
			if(printOut)
				Print(barNo + " DrawZZSizeText called");
//			double zzhi = getLastZZHighLow(CurrentBar, 1, out idx_hi); //zigZagHighZigZags.Get(idx);
//			double zzlo = getLastZZHighLow(CurrentBar, -1, out idx_lo); // zigZagLowZigZags.Get(idx);
			
			double zzSize = zigZagSizeZigZag.Get(barNo);//the previous zz size
			IText it = null;
			if(zzSize < 0) {
				it = DrawText(tag+barNo.ToString(), Time[CurrentBar-barNo].ToString().Substring(10)+"\r\n"+barNo.ToString()+":"+zzSize, CurrentBar-barNo, double.Parse(High[CurrentBar-barNo].ToString())+0.5, Color.Red);
			}
			if(zzSize > 0) {
				it = DrawText(tag+barNo.ToString(), Time[CurrentBar-barNo].ToString().Substring(10)+"\r\n"+barNo.ToString()+":"+zzSize, CurrentBar-barNo, double.Parse(Low[CurrentBar-barNo].ToString())-0.5, Color.Green);
			}
//			it.Locked = false;
			
			for (int idx = barNo-1; idx >= BarsRequired; idx--)
			{
				zzSize = zigZagSizeZigZag.Get(idx);
				if(zzSize < 0) {
					idx_hilo = idx;
					if(printOut)
						Print(idx + " DrawZZSize called");
					it = DrawText(tag+idx.ToString(), Time[CurrentBar-idx].ToString().Substring(10)+"\r\n"+idx.ToString()+":"+zzSize, CurrentBar-idx, double.Parse(High[CurrentBar-idx].ToString())+0.5, Color.Red);
					break;
				}
				if(zzSize > 0) {
					idx_hilo = idx;
					if(printOut)
						Print(idx + " DrawZZSize called");
					it = DrawText(tag+idx.ToString(), Time[CurrentBar-idx].ToString().Substring(10)+"\r\n"+idx.ToString()+":"+zzSize, CurrentBar-idx, double.Parse(Low[CurrentBar-idx].ToString())-0.5, Color.Green);
					break;
				}
				//it.Locked = false;
			}
			return true; 
		}
		
		/// <summary>
		/// Remove the drawing object for barNo
		/// </summary>
		/// <param name="barNo"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool RemoveText(int barNo, string tag)
		{
			RemoveDrawObject(tag);
			Update();
			if(printOut)
				Print(tag + " was removed from chart");
			return true; 
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {	
//			if(double.IsNaN(Close[-1])) {
//				Print("Last Bar:"+ CurrentBar);
//			}
//			if(!Historical) 
//				Print(CurrentBar + ": GIZZ OnBarUpdate - " + Time[0].ToShortTimeString());
			//if(IsLastBarOnChart() > 0) {
				//Print(ToTime(Time[0]).ToString() + "-Last Bar:"+ CurrentBar + ", Close[0]=" + Close[0].ToString() + ", Close[-1]=" + Close[-1].ToString() + ", High[-1]=" + High[-1].ToString() + ", Low[-1]=" + Low[-1].ToString());// + ", Close[-4]=" + Close[-4].ToString() + ", High[-4]=" + High[-4].ToString() + ", Low[-4]=" + Low[-4].ToString());
				//Print("this.ChartControl.LastBarPainted=" + this.ChartControl.LastBarPainted + ", Input.count=" + IsLastBarOnChart());
				//PrintZZSize();
				//PrintZZHiLo();
			//}
			//Print(CurrentBar+ " BarsRequired=" + BarsRequired);		
			
//			Print("this.LastVisibleBar=" + this.LastVisibleBar);
//			Print("this.LastBarIndexPainted=" + this.LastBarIndexPainted);
//			Print("this.ChartControl.BarsPainted=" + this.ChartControl.BarsPainted);
//			Print("zigZagSizeSeries.count=" + zigZagSizeSeries.Count);
//			Print("zigZagSizeZigZag.count=" + zigZagSizeZigZag.Count+ ", Historical=" + Historical);
//			if(drawTxt)
//				DrawText("txt-"+CurrentBar.ToString(), Time.ToString().Substring(10)+"-"+CurrentBar.ToString()+":"+lastSwingIdx.ToString(), 0, double.Parse(Low.ToString())-0.5, Color.Black);
			if (CurrentBar < 2) // need 3 bars to calculate Low/High
			{
				zigZagHighSeries.Set(0);
				zigZagHighZigZags.Set(0);
				zigZagLowSeries.Set(0);
				zigZagLowZigZags.Set(0);
				zigZagSizeSeries.Set(0);
				zigZagSizeZigZag.Set(0);
				return;
			}
			
			//Print((CurrentBar-1).ToString() + " - ZZLo, ZZHi=" + zigZagLowZigZags.Get(CurrentBar-1) + ", " + zigZagHighZigZags.Get(CurrentBar-1));
			//Print((CurrentBar-1).ToString() + " - ZZLoSeries, ZZHiSeries=" + zigZagLowSeries.Get(CurrentBar-1) + ", " + zigZagHighSeries.Get(CurrentBar-1));

			// Initialization
			if (lastSwingPrice == 0.0)
				lastSwingPrice = Input[0];

			IDataSeries highSeries	= High;
			IDataSeries lowSeries	= Low;

			if (!useHighLow)
			{
				highSeries	= Input;
				lowSeries	= Input;
			}

			// Calculation always for 1-bar ago !

			double tickSize = Bars.Instrument.MasterInstrument.TickSize;
			bool isSwingHigh	= highSeries[1] >= highSeries[0] - double.Epsilon 
								&& highSeries[1] >= highSeries[2] - double.Epsilon;
			bool isSwingLow		= lowSeries[1] <= lowSeries[0] + double.Epsilon 
								&& lowSeries[1] <= lowSeries[2] + double.Epsilon;  
			bool isOverHighDeviation	= (deviationType == DeviationType.Percent && IsPriceGreater(highSeries[1], (lastSwingPrice * (1.0 + deviationValue * 0.01))))
										|| (deviationType == DeviationType.Points && IsPriceGreater(highSeries[1], lastSwingPrice + deviationValue));
			bool isOverLowDeviation		= (deviationType == DeviationType.Percent && IsPriceGreater(lastSwingPrice * (1.0 - deviationValue * 0.01), lowSeries[1]))
										|| (deviationType == DeviationType.Points && IsPriceGreater(lastSwingPrice - deviationValue, lowSeries[1]));

			if(printOut)
				Print(CurrentBar.ToString() + ": lastSwingPrice, lastSwingIdx=" + lastSwingPrice + ", " + lastSwingIdx + ",isOverHighDeviation=" + isOverHighDeviation + ",isOverLowDeviation=" + isOverLowDeviation);
			
			double	saveValue	= 0.0;
			bool	addHigh		= false; 
			bool	addLow		= false; 
			bool	updateHigh	= false; 
			bool	updateLow	= false; 

			zigZagHighZigZags.Set(0);
			zigZagLowZigZags.Set(0);
			zigZagSizeSeries.Set(0);
			zigZagSizeZigZag.Set(0);

		
//			if(CurrentBar == BarsRequired)
//			{
//				lastZigZagIdx = CurrentBar;
//				lastZigZagPrice = Input[0];
//			}
			
			
			//find the ZigZag low/high at the first bar that is over deviation 
			//instead of waiting for the swing point appears.
//			if (CurrentBar > BarsRequired )
//			{
//				if(trendDir == -1 && isOverHighDeviation && zigZagSizeSeries.Get(CurrentBar-1)<=0) //Find the ZigZag low point
//				{
//					zigZagSizeZigZag.Set(CurrentBar-lastSwingIdx, lastSwingPrice-lastZigZagPrice);
//					zigZagSizeSeries.Set(High[0]-lastSwingPrice);
//					lastZigZagPrice = lastSwingPrice;
//					lastZigZagIdx = lastSwingIdx;
//				}
//				else if(trendDir == 1 && isOverLowDeviation && zigZagSizeSeries.Get(CurrentBar-1)>=0) //Find the ZigZag high point
//				{
//					zigZagSizeZigZag.Set(CurrentBar-lastSwingIdx, lastSwingPrice-lastZigZagPrice);
//					zigZagSizeSeries.Set(Low[0]-lastSwingPrice);
//					lastZigZagPrice = lastSwingPrice;
//					lastZigZagIdx = lastSwingIdx;
//				}
//				else if(trendDir == -1 && zigZagSizeZigZag.Get(lastZigZagIdx)>0) //not zigzag point with downtrend
//				{
//					zigZagSizeSeries.Set(Low[0]-lastZigZagPrice);
//				}
//				else if(trendDir == 1 && zigZagSizeZigZag.Get(lastZigZagIdx)<0) //not zigzag point with uptrend
//				{
//					zigZagSizeSeries.Set(High[0]-lastZigZagPrice);
//				}
//			}

			if (CurrentBar > BarsRequired && lastZigZagIdx > 0)
			{
				if(printOut)
					Print(CurrentBar + ": zigZagSizeZigZag.Get(lastZigZagIdx)=" + zigZagSizeZigZag.Get(lastZigZagIdx) + " , lastZigZagIdx, lastSwingIdx=" + lastZigZagIdx + ", " + lastSwingIdx);
				if(zigZagSizeZigZag.Get(lastZigZagIdx)>0) 
				{
					if(printOut)
						Print(CurrentBar.ToString() + " - lastZigZagIdx, lastSwingIdx=" + lastZigZagIdx + ", " + lastSwingIdx + ",isOverHighDeviation=" + isOverHighDeviation + ",zigZagSizeZigZag=" + zigZagSizeZigZag.Get(lastZigZagIdx) + ",lastZigZagPrice=" + lastZigZagPrice);
					if(isOverHighDeviation) //Find the ZigZag low point
					{
						zigZagSizeZigZag.Set(CurrentBar-lastSwingIdx, lastSwingPrice-lastZigZagPrice);
						//zigZagSizeSeries.Set(High[0]-lastSwingPrice);
						lastZigZagPrice = lastSwingPrice;
						lastZigZagIdx = lastSwingIdx;
						for(int idx=0; idx<CurrentBar-lastZigZagIdx; idx++)
						{
							zigZagSizeSeries.Set(idx, High[idx]-lastZigZagPrice);
						}
					}
					else //not zigzag point with downtrend
					{
						zigZagSizeSeries.Set(Low[0]-lastZigZagPrice);
					}
				}
				else if(zigZagSizeZigZag.Get(lastZigZagIdx)<0) 
				{
					if(printOut)
						Print(CurrentBar.ToString() + " - lastZigZagIdx, lastSwingIdx=" + lastZigZagIdx + ", " + lastSwingIdx + ",isOverLowDeviation=" + isOverLowDeviation + ",zigZagSizeZigZag=" + zigZagSizeZigZag.Get(lastZigZagIdx) + ",lastZigZagPrice=" + lastZigZagPrice);
					if(isOverLowDeviation) //Find the ZigZag high point
					{
						zigZagSizeZigZag.Set(CurrentBar-lastSwingIdx, lastSwingPrice-lastZigZagPrice);
						//zigZagSizeSeries.Set(Low[0]-lastSwingPrice);
						lastZigZagPrice = lastSwingPrice;
						lastZigZagIdx = lastSwingIdx;
						for(int idx=0; idx<CurrentBar-lastZigZagIdx; idx++)
						{
							zigZagSizeSeries.Set(idx, Low[idx]-lastZigZagPrice);
						}
					}
					else //not zigzag point with uptrend
					{
						zigZagSizeSeries.Set(High[0]-lastZigZagPrice);
					}
				}
			}

			if (!isSwingHigh && !isSwingLow)
			{
				if(printOut)
					Print(CurrentBar.ToString() + " not swing hi/lo - lastZigZagIdx, lastSwingIdx=" + lastZigZagIdx + ", " + lastSwingIdx);
				zigZagHighSeries.Set(currentZigZagHigh);
				zigZagLowSeries.Set(currentZigZagLow);
				//SetZigZagSize( CurrentBar, lastSwingIdx, lastSwingPrice);
				return;
			}
			
			if (trendDir <= 0 && isSwingHigh && isOverHighDeviation)
			{	
				saveValue	= highSeries[1];
				addHigh		= true;
				trendDir	= 1;				
				//DrawText("txt-"+(CurrentBar-1).ToString(), Time[1].ToString().Substring(10)+"\r\n"+(CurrentBar-1).ToString()+":"+lastSwingIdx.ToString(), 1, double.Parse(Low[1].ToString())-0.5, Color.Green);
			}	
			else if (trendDir >= 0 && isSwingLow && isOverLowDeviation)
			{	
				saveValue	= lowSeries[1];
				addLow		= true;
				trendDir	= -1;
				//DrawText("txt-"+(CurrentBar-1).ToString(), Time[1].ToString().Substring(10)+"\r\n"+(CurrentBar-1).ToString()+":"+lastSwingIdx.ToString(), 1, double.Parse(High[1].ToString())+0.5, Color.Red);
			}	
			else if (trendDir == 1 && isSwingHigh && IsPriceGreater(highSeries[1], lastSwingPrice)) 
			{
				saveValue	= highSeries[1];
				updateHigh	= true;
				//DrawText("txt-"+(CurrentBar-1).ToString(), Time[1].ToString().Substring(10)+"-"+(CurrentBar-1).ToString()+"\r\n"+highSeries[1].ToString()+":"+getLastZZHighLow(CurrentBar-1, CurrentBar-2, -1, true).ToString(), 1, double.Parse(Low[1].ToString())-0.5, Color.Green);
			}
			else if (trendDir == -1 && isSwingLow && IsPriceGreater(lastSwingPrice, lowSeries[1])) 
			{
				saveValue	= lowSeries[1];
				updateLow	= true;
				//DrawText("txt-"+(CurrentBar-1).ToString(), Time[1].ToString().Substring(10)+"-"+(CurrentBar-1).ToString()+"\r\n"+lowSeries[1].ToString()+":"+getLastZZHighLow(CurrentBar-1, CurrentBar-2, 1, true).ToString(), 1, double.Parse(High[1].ToString())+0.5, Color.Red);
			}
//Print(CurrentBar + "- lastSwingPrice=" + lastSwingPrice + ", lowSeries[1]=" + lowSeries[1] + ", highSeries[1]=" + highSeries[1]);
			if (addHigh || addLow || updateHigh || updateLow)
			{
				if (updateHigh && lastSwingIdx >= 0)
				{
					zigZagHighZigZags.Set(CurrentBar - lastSwingIdx, 0);
					Value.Reset(CurrentBar - lastSwingIdx);
					if(printOut)
						Print(CurrentBar+ ":" + " zigZagHighZigZags(" + lastSwingIdx.ToString() + ")=0");
				}
				else if (updateLow && lastSwingIdx >= 0)
				{
					zigZagLowZigZags.Set(CurrentBar - lastSwingIdx, 0);
					Value.Reset(CurrentBar - lastSwingIdx);
					if(printOut)
						Print(CurrentBar+ ":" + " zigZagLowZigZags(" + lastSwingIdx.ToString() + ")=0");
				}

				if (addHigh || updateHigh)
				{
					zigZagHighZigZags.Set(1, saveValue);
					zigZagHighZigZags.Set(0, 0);

					currentZigZagHigh = saveValue;
					zigZagHighSeries.Set(1, currentZigZagHigh);
					Value.Set(1, currentZigZagHigh);
					if(printOut)
						Print(CurrentBar+ ":" + " zigZagHighZigZags(" + (CurrentBar-1).ToString() + ")=" + saveValue + ", zigZagHighSeries(" + (CurrentBar-1).ToString() + ")=" + currentZigZagHigh + " || addHigh, updateHigh=" + addHigh + "," + updateHigh);
					
				}
				else if (addLow || updateLow) 
				{
					zigZagLowZigZags.Set(1, saveValue);
					zigZagLowZigZags.Set(0, 0);

					currentZigZagLow = saveValue;
					zigZagLowSeries.Set(1, currentZigZagLow);
					Value.Set(1, currentZigZagLow);
					if(printOut)
						Print(CurrentBar+ ":" + " zigZagLowZigZags(" + (CurrentBar-1).ToString() + ")=" + saveValue + ", zigZagLowSeries(" + (CurrentBar-1).ToString() + ")=" + currentZigZagLow + " || addLow, updateLow=" + addLow + "," + updateLow);
				}

				lastSwingIdx	= CurrentBar - 1;
				lastSwingPrice	= saveValue;
				
				if(lastZigZagIdx < 0 && lastSwingIdx > 0) // initialize the lastZigZagIdx, lastZigZagPrice and zigZagSizeZigZag
				{
					Print(CurrentBar+ ": initial lastZigZagIdx,lastZigZagPrice" + lastZigZagIdx + "," + lastZigZagPrice + ",Low[2]=" + Low[2] + ",High[2]=" + High[2] + "- lastSwingPrice, lastSwingIdx=" + lastSwingPrice + ", " + lastSwingIdx);
					lastZigZagIdx = lastSwingIdx;
					lastZigZagPrice = lastSwingPrice;
					if (isSwingHigh)
						zigZagSizeZigZag.Set(CurrentBar-lastSwingIdx, lastZigZagPrice-Low[2]);
					else if (isSwingLow)
						zigZagSizeZigZag.Set(CurrentBar-lastSwingIdx, lastZigZagPrice-High[2]);
				}
			}

			zigZagHighSeries.Set(currentZigZagHigh);
			zigZagLowSeries.Set(currentZigZagLow);
			
//			if(CurrentBar > BarsRequired)
//				SetZigZagSize(addHigh, addLow, updateHigh, updateLow);
        }
		
		protected override void OnTermination()
		{   
//			this.ChartControl.LastBarPainted;
//			this.LastVisibleBar;
//			this.LastBarIndexPainted;
			Print("OnTermination zigZagHighSeries=" + zigZagHighSeries.Count.ToString());
		}
		
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			//Print("Plot=" + Time.ToString());
			if (Bars == null || ChartControl == null)
				return;

			IsValidPlot(Bars.Count - 1 + (CalculateOnBarClose ? -1 : 0)); // make sure indicator is calculated until last (existing) bar

			int preDiff = 1;
			for (int i = FirstBarIndexPainted - 1; i >= BarsRequired; i--)
			{
				if (i < 0)
					break;

				bool isHigh	= zigZagHighZigZags.IsValidPlot(i) && zigZagHighZigZags.Get(i) > 0;
				bool isLow	= zigZagLowZigZags.IsValidPlot(i) && zigZagLowZigZags.Get(i) > 0;
				
				if (isHigh || isLow)
					break;

				preDiff++;
			}

			int postDiff = 0;
			for (int i = LastBarIndexPainted; i <= zigZagHighZigZags.Count; i++)
			{
				if (i < 0)
					break;

				bool isHigh	= zigZagHighZigZags.IsValidPlot(i) && zigZagHighZigZags.Get(i) > 0;
				bool isLow	= zigZagLowZigZags.IsValidPlot(i) && zigZagLowZigZags.Get(i) > 0;

				if (isHigh || isLow)
					break;

				postDiff++;
			}

			bool linePlotted = false;
			using (GraphicsPath path = new GraphicsPath()) 
			{
				int		barWidth	= ChartControl.ChartStyle.GetBarPaintWidth(Bars.BarsData.ChartStyle.BarWidthUI);

				int		lastIdx		= -1; 
				double	lastValue	= -1; 

				for (int idx = this.FirstBarIndexPainted - preDiff; idx <= this.LastBarIndexPainted + postDiff; idx++)
				{
					if (idx - Displacement < 0 || idx - Displacement >= Bars.Count || (!ChartControl.ShowBarsRequired && idx - Displacement < BarsRequired))
						continue;

					bool isHigh	= zigZagHighZigZags.IsValidPlot(idx) && zigZagHighZigZags.Get(idx) > 0;
					bool isLow	= zigZagLowZigZags.IsValidPlot(idx) && zigZagLowZigZags.Get(idx) > 0;

					if (!isHigh && !isLow)
						continue;
					
					double value = isHigh ? zigZagHighZigZags.Get(idx) : zigZagLowZigZags.Get(idx);
					if (lastValue >= 0)
					{	
						int x0	= ChartControl.GetXByBarIdx(BarsArray[0], lastIdx);
						int x1	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
						int y0	= ChartControl.GetYByValue(this, lastValue);
						int y1	= ChartControl.GetYByValue(this, value);

						path.AddLine(x0, y0, x1, y1);
						linePlotted = true;						
						//DrawText("txt-"+Time.ToString(), Time.ToString()+"-"+lastValue.ToString(), idx-lastIdx, double.Parse(High.ToString())+2, Color.Black);
					}

					// save as previous point
					lastIdx		= idx; 
					lastValue	= value; 
				}

				SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.DrawPath(Plots[0].Pen, path);
				graphics.SmoothingMode = oldSmoothingMode;
			}

			if (!linePlotted)
				DrawTextFixed("ZigZagErrorMsg", "GIZigZag can't plot any values since the deviation value is too large. Please reduce it.", TextPosition.BottomRight);
		}


        #region Properties
        [Description("Deviation in percent or points regarding on the deviation type")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Deviation value")]
        public double DeviationValue
        {
            get { return deviationValue; }
            set { deviationValue = Math.Max(0.0, value); }
        }

        [Description("Type of the deviation value")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Deviation type")]
        public DeviationType DeviationType
        {
            get { return deviationType; }
            set { deviationType = value; }
        }

        [Description("If true, high and low instead of selected price type is used to plot indicator.")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Use high and low")]
		[RefreshProperties(RefreshProperties.All)]
        public bool UseHighLow
        {
            get { return useHighLow; }
            set { useHighLow = value; }
        }

        [Description("If true, print out the console.")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Print out console")]
		[RefreshProperties(RefreshProperties.All)]
        public bool PrintOut
        {
            get { return printOut; }
            set { printOut = value; }
        }
		
		[Description("If true, draw text on chart.")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Draw Text")]
		[RefreshProperties(RefreshProperties.All)]
        public bool DrawTxt
        {
            get { return drawTxt; }
            set { drawTxt = value; }
        }
		
		[Description("If true, draw ZigZag Text.")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Draw ZigZag Text")]
		[RefreshProperties(RefreshProperties.All)]
        public bool DrawZZTxt
        {
            get { return drawZZTxt; }
            set { drawZZTxt = value; }
        }
		
		/// <summary>
		/// Gets the GIZigZag high points.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries ZigZagHigh
		{
			get 
			{ 
				Update();
				return zigZagHighSeries; 
			}
		}

		/// <summary>
		/// Gets the GIZigZag low points.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries ZigZagLow
		{
			get 
			{ 
				Update();
				return zigZagLowSeries; 
			}
		}

		/// <summary>
		/// Gets the GIZigZag gap between current bar with last ZZ price.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries ZigZagGap
		{
			get 
			{ 
				Update();
				return zigZagSizeSeries;
			}
		}

		/// <summary>
		/// Gets the GIZigZag ZZ size.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries ZigZagSize
		{
			get 
			{ 
				Update();
				return zigZagSizeZigZag;
			}
		}
        #endregion

		#region Miscellaneous

		/// <summary>
		/// #ENS#
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void GetMinMaxValues(Gui.Chart.ChartControl chartControl, ref double min, ref double max)
		{
			if (BarsArray[0] == null || ChartControl == null)
				return;

			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
			{
				for (int idx = this.FirstBarIndexPainted; idx <= this.LastBarIndexPainted; idx++)
				{
					if (zigZagHighZigZags.IsValidPlot(idx) && zigZagHighZigZags.Get(idx) != 0)
						max = Math.Max(max, zigZagHighZigZags.Get(idx));
					if (zigZagLowZigZags.IsValidPlot(idx) && zigZagLowZigZags.Get(idx) != 0)
						min = Math.Min(min, zigZagLowZigZags.Get(idx));
				}
			}
		}

		private bool IsPriceGreater(double a, double b)
		{
			if (a > b && a - b > TickSize / 2)
				return true; 
			else 
				return false;
		}
		
		public int IsLastBarOnChart() {
			if(Input.Count - CurrentBar <= 2) {
				return Input.Count;
			} else {
				return -1;
			}
		}
		
		#endregion
    }
	
//	public class GIZZSwing : DataSeries
//    {
//		private double			ZigZagSize	= 0;
//		private DateTime			StartTime ;
//		private DateTime			EndTime ;
//	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private GIZigZag[] cacheGIZigZag = null;

        private static GIZigZag checkGIZigZag = new GIZigZag();

        /// <summary>
        /// The GIZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        public GIZigZag GIZigZag(DeviationType deviationType, double deviationValue, bool drawTxt, bool drawZZTxt, bool printOut, bool useHighLow)
        {
            return GIZigZag(Input, deviationType, deviationValue, drawTxt, drawZZTxt, printOut, useHighLow);
        }

        /// <summary>
        /// The GIZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        public GIZigZag GIZigZag(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool drawTxt, bool drawZZTxt, bool printOut, bool useHighLow)
        {
            if (cacheGIZigZag != null)
                for (int idx = 0; idx < cacheGIZigZag.Length; idx++)
                    if (cacheGIZigZag[idx].DeviationType == deviationType && Math.Abs(cacheGIZigZag[idx].DeviationValue - deviationValue) <= double.Epsilon && cacheGIZigZag[idx].DrawTxt == drawTxt && cacheGIZigZag[idx].DrawZZTxt == drawZZTxt && cacheGIZigZag[idx].PrintOut == printOut && cacheGIZigZag[idx].UseHighLow == useHighLow && cacheGIZigZag[idx].EqualsInput(input))
                        return cacheGIZigZag[idx];

            lock (checkGIZigZag)
            {
                checkGIZigZag.DeviationType = deviationType;
                deviationType = checkGIZigZag.DeviationType;
                checkGIZigZag.DeviationValue = deviationValue;
                deviationValue = checkGIZigZag.DeviationValue;
                checkGIZigZag.DrawTxt = drawTxt;
                drawTxt = checkGIZigZag.DrawTxt;
                checkGIZigZag.DrawZZTxt = drawZZTxt;
                drawZZTxt = checkGIZigZag.DrawZZTxt;
                checkGIZigZag.PrintOut = printOut;
                printOut = checkGIZigZag.PrintOut;
                checkGIZigZag.UseHighLow = useHighLow;
                useHighLow = checkGIZigZag.UseHighLow;

                if (cacheGIZigZag != null)
                    for (int idx = 0; idx < cacheGIZigZag.Length; idx++)
                        if (cacheGIZigZag[idx].DeviationType == deviationType && Math.Abs(cacheGIZigZag[idx].DeviationValue - deviationValue) <= double.Epsilon && cacheGIZigZag[idx].DrawTxt == drawTxt && cacheGIZigZag[idx].DrawZZTxt == drawZZTxt && cacheGIZigZag[idx].PrintOut == printOut && cacheGIZigZag[idx].UseHighLow == useHighLow && cacheGIZigZag[idx].EqualsInput(input))
                            return cacheGIZigZag[idx];

                GIZigZag indicator = new GIZigZag();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DeviationType = deviationType;
                indicator.DeviationValue = deviationValue;
                indicator.DrawTxt = drawTxt;
                indicator.DrawZZTxt = drawZZTxt;
                indicator.PrintOut = printOut;
                indicator.UseHighLow = useHighLow;
                Indicators.Add(indicator);
                indicator.SetUp();

                GIZigZag[] tmp = new GIZigZag[cacheGIZigZag == null ? 1 : cacheGIZigZag.Length + 1];
                if (cacheGIZigZag != null)
                    cacheGIZigZag.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGIZigZag = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// The GIZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GIZigZag GIZigZag(DeviationType deviationType, double deviationValue, bool drawTxt, bool drawZZTxt, bool printOut, bool useHighLow)
        {
            return _indicator.GIZigZag(Input, deviationType, deviationValue, drawTxt, drawZZTxt, printOut, useHighLow);
        }

        /// <summary>
        /// The GIZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        public Indicator.GIZigZag GIZigZag(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool drawTxt, bool drawZZTxt, bool printOut, bool useHighLow)
        {
            return _indicator.GIZigZag(input, deviationType, deviationValue, drawTxt, drawZZTxt, printOut, useHighLow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The GIZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GIZigZag GIZigZag(DeviationType deviationType, double deviationValue, bool drawTxt, bool drawZZTxt, bool printOut, bool useHighLow)
        {
            return _indicator.GIZigZag(Input, deviationType, deviationValue, drawTxt, drawZZTxt, printOut, useHighLow);
        }

        /// <summary>
        /// The GIZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        public Indicator.GIZigZag GIZigZag(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool drawTxt, bool drawZZTxt, bool printOut, bool useHighLow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GIZigZag(input, deviationType, deviationValue, drawTxt, drawZZTxt, printOut, useHighLow);
        }
    }
}
#endregion
