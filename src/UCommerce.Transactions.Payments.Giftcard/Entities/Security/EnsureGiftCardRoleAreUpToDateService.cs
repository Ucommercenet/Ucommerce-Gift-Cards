using System.Collections.Generic;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Queries.Security;
using UCommerce.Security;

namespace UCommerce.Transactions.Payments.GiftCard.Entities.Security
{
    public class EnsureGiftCardRoleAreUpToDateService : IEnsureRolesAreUpToDateService
    {
        private readonly IRepository<Role> _roleRepository;

        public EnsureGiftCardRoleAreUpToDateService(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public bool EnsureRolesAreUpToDate()
        {
            IList<Role> existingRoles = GetGiftCardRole();

            var loadedGiftCardRole = existingRoles.FirstOrDefault(x => x.GetType() == typeof(CreateGiftCardRole));
            Role createGiftCardRole =
                    loadedGiftCardRole ??
                    new CreateGiftCardRole();

            if (loadedGiftCardRole == null)
                createGiftCardRole.Save();

            return loadedGiftCardRole == null;
        }

        private List<Role> GetGiftCardRole()
        {
            return _roleRepository.Select(new AllRolesQuery()).ToList();
        }
    }
}
