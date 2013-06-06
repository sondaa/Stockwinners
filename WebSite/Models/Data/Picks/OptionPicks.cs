using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;

namespace WebSite.Models.Data.Picks
{
    [Table("OptionPicks")]
    public class OptionPick : Pick
    {
        /// <summary>
        /// Legs that this option pick contains.
        /// </summary>
        public virtual ICollection<OptionPickLeg> Legs { get; set; }

        [ForeignKey("Type")]
        [Display(Name = "Type of option strategy")]
        public int OptionPickTypeId { get; set; }
        public virtual OptionPickType Type { get; set; }

        /// <summary>
        /// Tracks whether the curve for this contract (its graph/series) has been calculated.
        /// </summary>
        private bool _curveCalculated;

        /// <summary>
        /// A series of tuples of strike price and corresponding contract price at the given strike.
        /// </summary>
        private List<Tuple<decimal, decimal>> _valueSeries;
        private List<Tuple<decimal, decimal>> ValueSeries
        {
            get
            {
                this.CalculateCurve();

                return _valueSeries;
            }
        }

        /// <summary>
        /// The point at which the value of the option is at maximum.
        /// </summary>
        private Tuple<decimal, decimal> _minimumPoint;
        private Tuple<decimal, decimal> MinimumPoint
        {
            get
            {
                this.CalculateCurve();

                return _minimumPoint;
            }
        }

        /// <summary>
        /// The point at which the value of the contract is at its minimum.
        /// </summary>
        private Tuple<decimal, decimal> _maximumPoint;
        private Tuple<decimal, decimal> MaximumPoint
        {
            get
            {
                this.CalculateCurve();

                return _maximumPoint;
            }
        }

        /// <summary>
        /// Minimum strike value considered for value calculations.
        /// </summary>
        private decimal? _minimumStrike;
        private decimal MinimumStrike
        {
            get
            {
                if (!_minimumStrike.HasValue)
                {
                    // We display the data 30% away from each min/max of strike prices to cover a decent range of profit/loss
                    _minimumStrike = this.Legs.Min(leg => leg.StrikePrice);
                    _minimumStrike = Math.Floor(_minimumStrike.Value * 0.7m);
                }

                return _minimumStrike.Value;
            }
        }

        /// <summary>
        /// Maximum strike price considered for value calculations.
        /// </summary>
        private decimal? _maximumStrike;
        private decimal MaximumStrike
        {
            get
            {
                if (!_maximumStrike.HasValue)
                {
                    // We display the data 30% away from each min/max of strike prices to cover a decent range of profit/loss
                    _maximumStrike = this.Legs.Max(leg => leg.StrikePrice);
                    _maximumStrike = Math.Ceiling(_maximumStrike.Value * 1.3m);
                }

                return _maximumStrike.Value;
            }
        }

        public override void Email(bool isPreview = false)
        {
            Helpers.Email.Send(this, isPreview);
        }

        public decimal Cost()
        {
            decimal purchasePrice = 0;

            foreach (var leg in Legs)
            {
                purchasePrice += leg.Quantity * leg.EntryPrice;
            }

            return purchasePrice;
        }

        public decimal Proceeds()
        {
            // Trade must be closed before we can determine its profit/loss
            if (!this.ClosingDate.HasValue)
            {
                return 0;
            }

            decimal salePrice = 0;

            foreach (var leg in Legs)
            {
                // All trade legs must be closed before we can calculate its profit/loss
                if (!leg.ClosingDate.HasValue || !leg.ExitPrice.HasValue)
                {
                    return 0;
                }

                salePrice += leg.Quantity * leg.ExitPrice.Value;
            }

            return salePrice;
        }

        public decimal Change()
        {
            // Trade must be closed before we can determine its profit/loss
            if (!this.ClosingDate.HasValue)
            {
                return 0;
            }

            decimal purchasePrice = this.Cost();
            decimal salePrice = this.Proceeds();

            // If the position cost was 0 and retrun is positive, declare 100% profit, otherwise, return 100% loss.
            if (purchasePrice == 0)
            {
                if (salePrice == 0)
                {
                    return 0;
                }
                else if (salePrice > 0)
                {
                    return 100;
                }
                else
                {
                    return -100;
                }
            }

            return (salePrice - purchasePrice) * 100 / Math.Abs(purchasePrice);
        }

        public Chart ExpiryGraph()
        {
            Chart chart = new Chart();

            chart.Font.Name = "Helvetica";

            ChartArea chartArea = new ChartArea();

            chartArea.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea.BackSecondaryColor = System.Drawing.Color.White;
            chartArea.BackGradientStyle = GradientStyle.TopBottom;

            chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;

            chart.ChartAreas.Add(chartArea);

            chart.Titles.Add("Expiry Graph");
            chart.Series.Add("Profit/Loss");

            // Properties of the series
            chart.Series[0].ChartType = SeriesChartType.Line;
            chart.Series[0].BorderWidth = 3;

            // Add profit/loss data
            foreach (var strikeAndValue in this.ValueSeries)
            {
                chart.Series[0].Points.Add(new DataPoint((double)strikeAndValue.Item1, (double)strikeAndValue.Item2));
            }

            return chart;
        }

        public List<decimal> BreakevenPoints()
        {
            List<decimal> breakevenPoints = new List<decimal>(2); // Strategies usually have at most 2 breakeven points

            decimal minStrike = this.MinimumStrike;
            decimal maxStrike = this.MaximumStrike;
            decimal cost = this.Cost();

            // If the trade is setup such that it does not cost us anything, then there can't be any breakeven points
            if (cost == 0)
            {
                return breakevenPoints;
            }

            // Find the biggest increment we can use to calculate the breakeven point
            decimal increment = 0.01m;

            while (true)
            {
                if (cost % (increment * 10) == 0)
                {
                    increment *= 10;
                }
                else
                {
                    break;
                }
            }

            for (decimal price = minStrike; price <= maxStrike; price += increment)
            {
                if (this.ValueAtExpiry(price) == 0)
                {
                    breakevenPoints.Add(price);
                }
            }

            return breakevenPoints;
        }

        /// <summary>
        /// Returns a tuple consisting of the strike price and value for the maximum profit from this contract.
        /// </summary>
        /// <returns></returns>
        public Tuple<decimal, decimal> MaximumProfit()
        {
            var maximumPoint = this.MaximumPoint;

            // If the maximum point lies at boundaries, then the trade has unlimited growth
            if (maximumPoint.Item1 == this.MinimumStrike || maximumPoint.Item1 == this.MaximumStrike)
            {
                return null;
            }

            return maximumPoint;
        }

        /// <summary>
        /// Returns a tuple consisting of a strike price and value at which point this contract has its maximum loss.
        /// </summary>
        /// <returns></returns>
        public Tuple<decimal, decimal> MaximumLoss()
        {
            var minimumPoint = this.MinimumPoint;

            // If the maximum point lies at boundaries, then the trade has unlimited growth
            if (minimumPoint.Item1 == this.MinimumStrike || minimumPoint.Item1 == this.MaximumStrike)
            {
                return null;
            }

            return minimumPoint;
        }

        #region Private Helpers

        private void CalculateCurve()
        {
            if (_curveCalculated)
            {
                return;
            }

            _minimumPoint = null;
            _maximumPoint = null;
            _valueSeries = new List<Tuple<decimal, decimal>>(capacity: (int)(this.MaximumStrike - this.MinimumStrike));

            decimal halfPoint = Math.Floor(this.MinimumStrike + ((this.MaximumStrike - this.MinimumStrike) / 2));

            // Walk from center to both edges to be able to correctly calculate max and min points

            // Upper half
            for (decimal price = halfPoint; price < this.MaximumStrike; price++)
            {
                var valueAtPrice = Tuple.Create(price, this.ValueAtExpiry(price));

                if (_minimumPoint == null)
                {
                    _minimumPoint = valueAtPrice;
                }
                else if (_minimumPoint.Item2 > valueAtPrice.Item2)
                {
                    _minimumPoint = valueAtPrice;
                }

                if (_maximumPoint == null)
                {
                    _maximumPoint = valueAtPrice;
                }
                else if (_maximumPoint.Item2 < valueAtPrice.Item2)
                {
                    _maximumPoint = valueAtPrice;
                }

                _valueSeries.Add(valueAtPrice);
            }

            // Lower half
            for (decimal price = halfPoint - 1; price > this.MinimumStrike; price--)
            {
                var valueAtPrice = Tuple.Create(price, this.ValueAtExpiry(price));

                if (_minimumPoint == null)
                {
                    _minimumPoint = valueAtPrice;
                }
                else if (_minimumPoint.Item2 > valueAtPrice.Item2)
                {
                    _minimumPoint = valueAtPrice;
                }

                if (_maximumPoint == null)
                {
                    _maximumPoint = valueAtPrice;
                }
                else if (_maximumPoint.Item2 < valueAtPrice.Item2)
                {
                    _maximumPoint = valueAtPrice;
                }

                _valueSeries.Add(valueAtPrice);
            }

            // Sort the series
            _valueSeries.Sort(new Comparison<Tuple<decimal,decimal>>((a, b) => a.Item1.CompareTo(b.Item1)));

            _curveCalculated = true;
        }

        private decimal ValueAtExpiry(decimal underlyingPrice)
        {
            return this.Legs.Sum(leg => leg.ValueAtExpiry(underlyingPrice));
        }

        #endregion

        public class OptionPickComparer : IComparer<OptionPick>
        {
            public int Compare(OptionPick x, OptionPick y)
            {
                decimal difference = y.Change() - x.Change();

                if (difference == 0)
                {
                    return 0;
                }

                if (difference < 0)
                {
                    return -1;
                }

                return 1;
            }
        }
    }
}