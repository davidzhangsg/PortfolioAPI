using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 📦 Assets
        CreateMap<StockCreateDto, Stock>();
        CreateMap<Stock, StockDto>()
            .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

        CreateMap<BondCreateDto, Bond>();
        CreateMap<Bond, BondDto>()
            .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

        CreateMap<FundCreateDto, Fund>();
        CreateMap<Fund, FundDto>()
            .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

        CreateMap<Asset, AssetDto>()
            .Include<Stock, StockDto>()
            .Include<Bond, BondDto>()
            .Include<Fund, FundDto>()
            .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

        // 📦 Transactions
        CreateMap<TransactionCreateDto, Transaction>();
        CreateMap<Transaction, TransactionDto>();

        CreateMap<TransactionUpdateDto, Transaction>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // Prevent overwriting PK
            .ForMember(dest => dest.AssetId, opt => opt.Ignore()); // Prevent changing FK

        // 📦 Portfolios
        CreateMap<PortfolioCreateDto, Portfolio>();
        CreateMap<PortfolioUpdateDto, Portfolio>();

        CreateMap<Portfolio, PortfolioDto>()
            .ForMember(dest => dest.Assets, opt => opt.MapFrom(src => src.Assets));

        CreateMap<Portfolio, PortfolioWithAssetsDto>()
            .ForMember(dest => dest.Assets, opt => opt.MapFrom(src => src.Assets));



        // 📦 Customers
        CreateMap<CustomerCreateDto, Customer>();
        CreateMap<CustomerUpdateDto, Customer>();

        CreateMap<Customer, CustomerDto>();
        CreateMap<Customer, CustomerPortfolioDto>()
            .ForMember(dest => dest.Portfolios, opt => opt.MapFrom(src => src.Portfolios));
    }
}
