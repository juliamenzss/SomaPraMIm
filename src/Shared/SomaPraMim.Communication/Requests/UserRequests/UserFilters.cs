using SomaPraMim.Domain.Entities;
using System.Linq.Expressions;

namespace SomaPraMim.Communication.Requests.UserRequests
{
    public static class UserFilters
    {
        public static Expression<Func<User, bool>> SearchByName(UserSearch search)
        {
            if (string.IsNullOrWhiteSpace(search.Term))
            {
                return _ => true;
            }

            var term = search.Term.ToLower();
            return user => user.Name.Contains(term, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
