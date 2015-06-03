namespace TrafalgarSquare.Web.Automapper
{
    using AutoMapper;

    internal interface IHaveCustomMappings
    {
        void CreateMappings(IConfiguration configuration);
    }
}
