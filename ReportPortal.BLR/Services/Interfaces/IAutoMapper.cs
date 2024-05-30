namespace ReportPortal.BL.Services.Interfaces
{
    public interface IAutoMapperService
    {
        public T Map<K, T>(K entity) where T : class;
    }
}
