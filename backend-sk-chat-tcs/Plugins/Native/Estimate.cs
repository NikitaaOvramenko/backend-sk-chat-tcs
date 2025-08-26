using backend_sk_chat_tcs.Models;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace backend_sk_chat_tcs.Plugins.Native
{
    public class Estimate
    {
        [KernelFunction, Description("Calculates Price for Wall in metric units")]
        [return: Description("return price of the one wall")]
        public async Task<ResponseFormat> CalcWallPrice(
        [Description("Width of the wall in meters")] double width,
        [Description("Height of the wall in meters")] double height,
        [Description("Number of coats")] double numberOfCoats)
        {
            var literPrice = 14.53;          // price per liter of paint 3.785L/55$ = 1L/x$ ==> x = 14.53$/L
            var coverage = 10.0;            // m² per liter
            var area = height * width;      // wall area in m²

            var liters = area * numberOfCoats / coverage;


            return new ResponseFormat { Message = $"{liters * literPrice}", Url = null };
                
        }
    }
}
