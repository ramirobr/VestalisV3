
using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace Cotecna.Vestalis.Core
{
    public class ParameterSearchServicerOrder
    {
        public FormCollection FormCollection { get; set; }
        public Guid BusinessApplicationId { get; set; }
        public List<string> RolesForUser { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool IsExport { get; set; }
        public bool IsClient { get; set; }
        public List<string> FielsdWithLike { get; set; }

        public ParameterSearchServicerOrder()
        {
            FormCollection = new FormCollection();
            RolesForUser = new List<string>();
            FielsdWithLike = new List<string>();
        }
    }
}
