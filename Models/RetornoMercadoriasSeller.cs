using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogo.Service.Api.Models
{
    public class RetornoMercadoriasSeller: WebReturn

    {
        public List<MercadoriaSeller> lstMercadoriaSeller { get;set;}
    }
}
