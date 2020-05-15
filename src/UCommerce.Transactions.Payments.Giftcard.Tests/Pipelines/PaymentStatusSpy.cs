using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ucommerce.EntitiesV2;

namespace UCommerce.Pipelines.Test.GiftCards
{
	public class PaymentStatusSpy : PaymentStatus
	{
		public PaymentStatusSpy(int paymentStatusId)
		{
			this.PaymentStatusId = paymentStatusId;
		}
	}
}
