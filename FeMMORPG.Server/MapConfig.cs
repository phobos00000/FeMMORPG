using AutoMapper;
using FeMMORPG.Data;

namespace FeMMORPG.Server
{
    internal static class MapConfig
    {
        public static MapperConfiguration Config()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Character, Common.Models.Character>().ReverseMap();
            });
        }
    }
}
