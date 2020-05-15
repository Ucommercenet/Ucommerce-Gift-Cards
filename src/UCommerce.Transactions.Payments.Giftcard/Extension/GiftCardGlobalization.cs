using System.Globalization;
using System.Threading;
using Ucommerce.Infrastructure.Globalization;

namespace UCommerce.Transactions.Payments.GiftCard.Extension
{
    public class GiftCardGlobalization : ILocalizationContext
    {
        public void SetCulture(CultureInfo cultureInfo)
        {
            CultureInfoInternal = cultureInfo;
        }

        private CultureInfo _cultureInfoInternal;

        private CultureInfo CultureInfoInternal
        {
            get
            {
                if (_cultureInfoInternal != null)
                    return _cultureInfoInternal;

                return Thread.CurrentThread.CurrentUICulture;
            }
            set { _cultureInfoInternal = value; }
        }

        /// <summary>
        /// Gets the current culture code.
        /// </summary>
        public virtual string CurrentCultureCode
        {
            get { return CultureInfoInternal.ToString(); }
        }

        public virtual CultureInfo CurrentCulture
        {
            get { return CultureInfoInternal; }
        }

        /// <summary>
        /// Gets the default culture.
        /// </summary>
        public virtual CultureInfo DefaultCulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }
    }
}
