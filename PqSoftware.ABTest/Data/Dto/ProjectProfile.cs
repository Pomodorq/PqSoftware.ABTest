using AutoMapper;
using PqSoftware.ABTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data.Dto
{
	public class ProjectProfile : Profile
	{
		public ProjectProfile()
		{
			CreateMap<PostProjectUserRequest, ProjectUser>();
		}
	}
}
