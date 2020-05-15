using System;
using Ucommerce.EntitiesV2;

namespace UCommerce.Transactions.Payments.GiftCard.Entities
{
	/// <summary>
	/// The GiftCard class is the basic foundation of uCommerce Gift Cards. You can 
	/// buy a gift card with some amount an later use this as a valid payment.
	/// </summary>
    public partial class GiftCard : IEntity
	{
        ///<summary>
        /// Returns the id.
        ///</summary>
        public virtual int Id
        {
            get
            {
                return GiftCardId;
            }
        }

        /// <summary>
        /// This property is the primary key.
        ///  </summary>   
        public virtual int GiftCardId
        {
            get; protected set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual decimal Amount
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual decimal AmountUsed
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual bool Enabled
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual DateTime CreatedOn
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual string CreatedBy
        {
            get; set;
        }

        /// <summary>
        /// This property is not mandatory.
        ///  </summary>   
        public virtual string ModifiedBy
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual DateTime ModifiedOn
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual DateTime ExpiresOn
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual string Code
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual string OrderNumber
        {
            get; set;
        }

        /// <summary>
        /// This property is mandatory.
        ///  </summary>   
        public virtual string Note
        {
            get; set;
        }

        /// <summary>
        /// A giftCard is associated with a PaymentMethod if the giftcard is bought through the shop.
        /// </summary>
        public virtual PaymentMethod PaymentMethod { get; set; }
        
        /// <summary>
        /// A giftCard is associated with a specific currency, only allowing you to use the same currency as you initialy bought the giftcard with.
        /// </summary>
        public virtual Currency Currency { get; set; }

		/// <summary>
		/// Calculates the available balance
		/// </summary>
		/// <returns></returns>
		public virtual decimal AvailableBalance()
		{
			return Amount - AmountUsed;
		}
    }
}