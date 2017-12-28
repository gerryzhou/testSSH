#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;

using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This file holds all user defined strategy methods.
    /// </summary>
 
	public enum SessionBreak {AfternoonClose, EveningOpen, MorningOpen, NextDay};
	partial class Strategy
    {
		public int IsLastBarOnChart() {
			if(Input.Count - CurrentBar <= 2) {
				return Input.Count;
			} else {
				return -1;
			}
		}
		
		public string GetTimeDate(String str_timedate, int time_date) {
			char[] delimiterChars = { ' '};
			string[] str_arr = str_timedate.Split(delimiterChars);
			return str_arr[time_date];
		}
		
		public string GetTimeDate(DateTime dt, int time_date) {
			string str_dt = dt.ToString("MMddyyyy HH:mm:ss");
			char[] delimiterChars = {' '};
			string[] str_arr = str_dt.Split(delimiterChars);
			return str_arr[time_date];
		}
		
		public double GetTimeDiff(DateTime dt_st, DateTime dt_en) {
			double diff = -1;
			//if(diff < 0 ) return 100; 
			try {
			
			if((int)dt_st.DayOfWeek==(int)dt_en.DayOfWeek) { //Same day
				if(CompareTimeWithSessionBreak(dt_st, SessionBreak.AfternoonClose)*CompareTimeWithSessionBreak(dt_en, SessionBreak.AfternoonClose) > 0) {
					diff = dt_en.Subtract(dt_st).TotalMinutes;
				}
				else if(CompareTimeWithSessionBreak(dt_st, SessionBreak.AfternoonClose) < 0 && CompareTimeWithSessionBreak(dt_en, SessionBreak.AfternoonClose) > 0) {
					diff = GetTimeDiffSession(dt_st, SessionBreak.AfternoonClose) + GetTimeDiffSession(dt_en, SessionBreak.EveningOpen);
				}
			}
			else if((dt_st.DayOfWeek==DayOfWeek.Friday && dt_en.DayOfWeek==DayOfWeek.Sunday) || ((int)dt_st.DayOfWeek>(int)dt_en.DayOfWeek) || ((int)dt_st.DayOfWeek<(int)dt_en.DayOfWeek-1)) { // Fiday - Sunday or Cross to next Week or have day off trade
				diff = GetTimeDiffSession(dt_st, SessionBreak.AfternoonClose) + GetTimeDiffSession(dt_en, SessionBreak.EveningOpen);
			}
			else if((int)dt_st.DayOfWeek==(int)dt_en.DayOfWeek-1) { //Same day or next day
				if(CompareTimeWithSessionBreak(dt_st, SessionBreak.AfternoonClose) < 0) {
					diff = GetTimeDiffSession(dt_st, SessionBreak.AfternoonClose) + GetTimeDiffSession(dt_en, SessionBreak.NextDay);
				}
				else if(CompareTimeWithSessionBreak(dt_st, SessionBreak.AfternoonClose) > 0) { // dt_st passed evening open, no need to adjust
					diff = diff = dt_en.Subtract(dt_st).TotalMinutes;
				}
			}
			else {
				diff = dt_en.Subtract(dt_st).TotalMinutes;
			}
			} catch(Exception ex) {
				Print("GetTimeDiff ex:" + dt_st.ToString() + "--" + dt_en.ToString() + "--" + ex.Message);
				diff = 100;
			}
			return Math.Round(diff, 2);
		}

		public double GetMinutesDiff(DateTime dt_st, DateTime dt_en) {
			double diff = -1;
			TimeSpan ts = dt_en.Subtract(dt_st);
			diff = ts.TotalMinutes;
			return Math.Round(diff, 2);
		}
		
		public int CompareTimeWithSessionBreak(DateTime dt_st, SessionBreak sb) {
			DateTime dt = DateTime.Now;
			try {
			switch(sb) {
				case SessionBreak.AfternoonClose:
					dt = GetNewDateTime(dt_st.Year,dt_st.Month,dt_st.Day,16,0,0);
					break;
				case SessionBreak.EveningOpen:
					if(dt_st.Hour < 16)
						dt = GetNewDateTime(dt_st.Year,dt_st.Month,dt_st.Day-1,17,0,0);
					//if(dt_st.Hour >= 17)
					else dt = GetNewDateTime(dt_st.Year,dt_st.Month,dt_st.Day,16,0,0);
					break;
				default:
					dt = GetNewDateTime(dt_st.Year,dt_st.Month,dt_st.Day,16,0,0);
					break;
			}
			} catch(Exception ex) {
				Print("CompareTimeWithSessionBreak ex:" + dt_st.ToString() + "--" + sb.ToString() + "--" + ex.Message);
			}
			return dt_st.CompareTo(dt);
		}

		public double GetTimeDiffSession(DateTime dt_st, SessionBreak sb) {			
			DateTime dt_session = DateTime.Now;
			TimeSpan ts = dt_session.Subtract(dt_st);
			double diff = 100;
			try{
			switch(sb) {
				case SessionBreak.AfternoonClose:
					dt_session = GetNewDateTime(dt_st.Year, dt_st.Month, dt_st.Day, 16, 0, 0);
					ts = dt_session.Subtract(dt_st);
					break;
				case SessionBreak.EveningOpen:
					if(dt_st.Hour < 16)
						dt_session = GetNewDateTime(dt_st.Year,dt_st.Month,dt_st.Day-1,17,0,0);
					else 
						dt_session = GetNewDateTime(dt_st.Year, dt_st.Month, dt_st.Day, 17, 0, 0);
					ts = dt_st.Subtract(dt_session);
					break;
				case SessionBreak.NextDay:
					dt_session = GetNewDateTime(dt_st.Year, dt_st.Month, dt_st.Day-1, 17, 0, 0);
					ts = dt_st.Subtract(dt_session);
					break;
				default:
					dt_session = GetNewDateTime(dt_st.Year, dt_st.Month, dt_st.Day-1, 17, 0, 0);
					ts = dt_st.Subtract(dt_session);
					break;
			}
			diff = ts.TotalMinutes;
			} catch(Exception ex) {
				Print("GetTimeDiffSession ex:" + dt_st.ToString() + "--" + sb.ToString() + "--" + ex.Message);
			}
			
			return Math.Round(diff, 2);
		}
		
		public DateTime GetNewDateTime(int year, int month, int day, int hr, int min, int sec) {
			//DateTime(dt_st.Year,dt_st.Month,dt_st.Day,16,0,0);
			if(day == 0) {
				if(month == 1) {
					year = year-1;
					month = 12;
					day = 31;
				}
				else if (month == 3) {
					month = 2;
					day = 28;
				}
				else {
					month--;
					if(month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10)
						day = 31;
					else day = 30;
				}
			}
			return new DateTime(year, month, day, hr, min, sec);
		}		
    }
}
