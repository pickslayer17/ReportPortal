namespace ReportPortal.Services.Interfaces
{
    public interface IAutoMapperInnerService
    {
        public T Map<K, T>(K entity) where T : class;
    }
}
