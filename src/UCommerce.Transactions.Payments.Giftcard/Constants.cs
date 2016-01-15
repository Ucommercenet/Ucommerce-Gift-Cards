using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Transactions.Payments.GiftCard
{
	/// <summary>
	/// String constants.
	/// </summary>
    public class Constants
    {
        public static readonly string GiftCardProductDefinition = "GiftCard";
        public static readonly string GiftCardPaymentMethodName = "Gift Card";
	    public static readonly string GiftCardEmailTypeName = "GiftCard";
		public static readonly Guid GiftCardProductDefinitionGuid = new Guid("2C208D66-A735-439B-892B-3F19B2B49BCA");
    }
}
