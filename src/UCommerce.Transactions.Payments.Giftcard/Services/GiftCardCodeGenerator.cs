using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCommerce.Marketing.Targets
{
    public class GiftCardCodeGenerator : IVoucherCodeGenerator
    {
        public IList<string> GenerateCodes(int numberToGen, int lengthOfVoucher, string prefix, string suffix)
        {
            var codes = new List<string>();

            string seperatorPrefix = "", seperatorSuffix = "";
            if (!string.IsNullOrEmpty(prefix))
                seperatorPrefix = "-";

            if (!string.IsNullOrEmpty(suffix))
                seperatorSuffix = "-";



            return codes;
        }
    }
}
