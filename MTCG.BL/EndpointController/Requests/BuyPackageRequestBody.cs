namespace MTCG.BL.Requests
{
    class BuyPackageRequestBody : JsonRequestBody<BuyPackageRequestBody>
    {
        public Guid PackageId { get; }
        public ushort PackageAmount { get; }

        public BuyPackageRequestBody(Guid packageId, ushort packageAmount)
        {
            PackageId = packageId;
            PackageAmount = packageAmount;
        }
    }
}
