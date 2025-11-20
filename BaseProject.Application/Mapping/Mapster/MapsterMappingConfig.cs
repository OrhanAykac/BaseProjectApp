using Mapster;

namespace BaseProject.Application.Mapping.Mapster;

public static class MapsterMappingConfig
{
    public static TypeAdapterConfig Configure()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        // var istanbulPoint = new Point(28.9784, 41.0082) { SRID = 4326 };

        config.Default.PreserveReference(true);

        //config
        //    .ForType<Point, Point>()
        //    .MapWith(src => new Point(src.X, src.Y) { SRID = 4326 });

        //config.NewConfig<Dealer, GetDealerByIdForAdminDealerResponse>()
        //    .Map(dest => dest.CorporateName, src => src.Corporate.Description)
        //    .Map(dest => dest.CityName, src => src.City.Name)
        //    .Map(dest => dest.DistrictName, src => src.Disctrict.Name);


        //config.NewConfig<Corporate, GetCorporatesForAdminListingResponse>()
        //    .Map(dest => dest.DealerCount, src => src.Dealers.Count);


        //config.NewConfig<Vendor, VendorDto>()
        //    .Map(dest => dest.TaxOfficeName, src => src.TaxOffice!.TaxOfficeName);

        //config.NewConfig<Category, CategoryDto>()
        //    .Map(dest => dest.ParentCategoryName, src => src.ParentCategory!.CategoryName);

        //User to Userdto
        //config.NewConfig<User, UserDto>()
        //    //.Ignore(dest => dest.UserId)
        //    .Map(dest => dest.UserId, src => src.Id);

        //config.NewConfig<User, UserForUserListDto>()
        //    .Map(dest => dest.UserId, src => src.Id);

        ////IList<IRegister> registers = config.Scan(Assembly.GetExecutingAssembly());
        ////config.Apply(registers);

        ////When NewConfig is called, any previous configuration for this particular TSource =>
        //config.NewConfig<User, UserDto>()
        ////differs in that it will create a new mapping if one doesn't exist, but if the
        ////specified TSource => TDestination mapping does already exist, it will enhance
        ////the existing mapping instead of dropping and replacing it.
        ////.ForType() 
        //.Ignore(dest => dest.UserId)
        //.Map(dest => dest.FirstName,//POCO to DTO
        //    src => string.Format("{0} {1}", src.FirstName, src.LastName));
        ////.TwoWays();
        ////.Map(dest => dest.FirstName,//POCO to DTO AND DTO to POCO
        ////src => string.Format("{0} {1}", src.FirstName, src.LastName))

        return config;
    }
}
