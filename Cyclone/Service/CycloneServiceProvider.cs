namespace AV.Cyclone.Service
{
    public class CycloneServiceProvider
    {
        public static ICycloneService GetCycloneService()
        {
            return CycloneService.Instance;
        }
    }
}