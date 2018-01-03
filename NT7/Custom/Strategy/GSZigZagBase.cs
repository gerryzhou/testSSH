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
    /// Base class for GSZigZag
    /// </summary>
    [Description("Base class for GSZigZag")]
    public class GSZigZagBase : Strategy
    {
        #region Variables
		protected IOrder entryOrder = null;     
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		protected int minutesChkEnOrder = 60; //how long before checking an entry order filled or not
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
        }
		
		public bool CheckEnOrder()
        {
            double min_en = -1;

            if (entryOrder != null && entryOrder.OrderState == OrderState.Working)
            {
                min_en = GetMinutesDiff(entryOrder.Time, DateTime.Now);
                if (min_en >= minutesChkEnOrder)
                {
                    CancelOrder(entryOrder);
                    Print("Order cancelled for " + base.Account.Name + ":" + min_en + " mins elapsed--" + entryOrder.ToString());
                    return true;
                }
            }
            return false;
        }

        #region Properties
        #endregion
    }
}
