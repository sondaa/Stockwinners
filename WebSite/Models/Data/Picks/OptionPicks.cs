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

            return (salePrice - purchasePrice) * 100 / purchasePrice;
        }

        public Chart ExpiryGraph()
        {
            Chart result = new Chart();

            // determine maximum and minimum strike prices
            decimal maxStrike = this.MaximumStrikeForCalculation();
            decimal minStrike = this.MinimumStrikeForCalculations();

            result.Series.Add("Profit/Loss");
            result.Series[0].ChartType = SeriesChartType.Line;

            // Add profit/loss data
            for (decimal price = minStrike; price < maxStrike; price++)
            {
                result.Series[0].Points.Add(new DataPoint((double)price, (double)this.ValueAtExpiry(price)));
            }

            return result;
        }

        public List<decimal> BreakevenPoints()
        {
            List<decimal> breakevenPoints = new List<decimal>(2); // Strategies usually have at most 2 breakeven points

            decimal minStrike = this.MinimumStrikeForCalculations();
            decimal maxStrike = this.MaximumStrikeForCalculation();
            decimal cost = this.Cost();

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

        public decimal MaximumProfit()
        {
            decimal maxProfit = decimal.MaxValue;

            // Loop over range of calculation and if 2 points have the same profit for a given underlying price, consider them as maximum profit
            decimal minStrike = this.MinimumStrikeForCalculations();
            decimal maxStrike = this.MaximumStrikeForCalculation();
            decimal profitCandidate = decimal.MaxValue;

            for (decimal price = maxStrike; price >= minStrike; price--)
            {
                decimal valueAtExpiry = this.ValueAtExpiry(price);

                if (profitCandidate == decimal.MaxValue)
                {
                    profitCandidate = valueAtExpiry;
                }
                else if (profitCandidate < valueAtExpiry)
                {
                    profitCandidate = valueAtExpiry;
                }
                else if (profitCandidate == valueAtExpiry)
                {
                    maxProfit = profitCandidate;
                }
            }

            return maxProfit;
        }

        public decimal MaximumLoss()
        {
            decimal maxLoss = decimal.MinValue;

            // Loop over range of calculation and if 2 points have the same loss for a given underlying price, consider them as maximum profit
            decimal minStrike = this.MinimumStrikeForCalculations();
            decimal maxStrike = this.MaximumStrikeForCalculation();
            decimal lossCandidate = decimal.MinValue;

            for (decimal price = minStrike; price <= maxStrike; price++)
            {
                decimal valueAtExpiry = this.ValueAtExpiry(price);

                if (lossCandidate == decimal.MinValue)
                {
                    lossCandidate = valueAtExpiry;
                }
                else if (lossCandidate > valueAtExpiry)
                {
                    lossCandidate = valueAtExpiry;
                }
                else if (lossCandidate == valueAtExpiry)
                {
                    maxLoss = lossCandidate;
                }
            }

            return maxLoss;
        }

        private decimal ValueAtExpiry(decimal underlyingPrice)
        {
            return this.Legs.Sum(leg => leg.ValueAtExpiry(underlyingPrice));
        }

        private decimal MinimumStrikeForCalculations()
        {
            // We display the data 30% away from each min/max of strike prices to cover a decent range of profit/loss
            decimal minStrike = this.Legs.Min(leg => leg.StrikePrice);
            minStrike = Math.Floor(minStrike - (minStrike * 0.3m));

            return minStrike;
        }

        private decimal MaximumStrikeForCalculation()
        {
            // We display the data 30% away from each min/max of strike prices to cover a decent range of profit/loss
            decimal maxStrike = this.Legs.Max(leg => leg.StrikePrice);
            maxStrike = Math.Ceiling(maxStrike * 1.3m);

            return maxStrike;
        }
    }
}