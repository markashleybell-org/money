using money.Support;

namespace money.Models
{
    public class IndexViewModel
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public IRequestContext RequestContext { get; set; }

        public string TestData { get; set; }
    }
}
