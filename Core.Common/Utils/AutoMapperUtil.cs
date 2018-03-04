using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Utils
{
    public class AutoMapperUtil
    {
        public static D Map<S, D>(S source)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<S, D>();
            });

            IMapper iMapper = config.CreateMapper();
            var dest = iMapper.Map<S, D>(source);

            return dest;
        }
    }
}
